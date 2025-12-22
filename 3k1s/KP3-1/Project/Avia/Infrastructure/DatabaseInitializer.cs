using Avia.Data;
using Avia.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.IO;
using System.Text;

namespace Avia.Infrastructure;

public class DatabaseInitializer
{
    private readonly AviaDbContext _context;
    private readonly string _connectionString;

    public DatabaseInitializer(AviaDbContext context, string connectionString)
    {
        _context = context;
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Проверяем, существует ли база данных
            if (!await _context.Database.CanConnectAsync())
            {
                await CreateDatabaseIfNotExistsAsync();
            }

            // Проверяем, существует ли схема avia
            var schemaExists = await CheckSchemaExistsAsync();
            System.Diagnostics.Debug.WriteLine($"Schema exists: {schemaExists}");
            
            // Проверяем, существуют ли таблицы
            var tablesExist = await CheckTablesExistAsync();
            System.Diagnostics.Debug.WriteLine($"Tables exist: {tablesExist}");
            
            if (!schemaExists || !tablesExist)
            {
                // Выполняем SQL скрипт для создания всех объектов БД
                System.Diagnostics.Debug.WriteLine("Executing database script...");
                await ExecuteDatabaseScriptAsync();
                
                // Проверяем еще раз после выполнения скрипта
                tablesExist = await CheckTablesExistAsync();
                System.Diagnostics.Debug.WriteLine($"Tables exist after script: {tablesExist}");
                
                if (!tablesExist)
                {
                    // Пробуем создать таблицы через EF Core как запасной вариант
                    System.Diagnostics.Debug.WriteLine("Attempting to create tables using EF Core...");
                    try
                    {
                        await _context.Database.EnsureCreatedAsync();
                        tablesExist = await CheckTablesExistAsync();
                        System.Diagnostics.Debug.WriteLine($"Tables exist after EF Core: {tablesExist}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"EF Core EnsureCreated failed: {ex.Message}");
                    }
                    
                    if (!tablesExist)
                    {
                        throw new InvalidOperationException(
                            "Не удалось создать таблицы в базе данных. " +
                            "Убедитесь, что PostgreSQL сервер запущен и SQL скрипт выполнен корректно.");
                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка инициализации базы данных: {ex.Message}", ex);
        }
    }

    private async Task CreateDatabaseIfNotExistsAsync()
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.Database;
        builder.Database = "postgres"; // Подключаемся к системной БД

        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        // Проверяем, существует ли база данных
        var checkDbCommand = new NpgsqlCommand(
            $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection);
        var exists = await checkDbCommand.ExecuteScalarAsync() != null;

        if (!exists)
        {
            // Создаем базу данных
            var createDbCommand = new NpgsqlCommand(
                $"CREATE DATABASE \"{databaseName}\"", connection);
            await createDbCommand.ExecuteNonQueryAsync();
        }

        await connection.CloseAsync();
    }

    private async Task<bool> CheckSchemaExistsAsync()
    {
        var connection = _context.Database.GetDbConnection();
        var connectionWasOpen = connection.State == System.Data.ConnectionState.Open;
        
        try
        {
            if (!connectionWasOpen)
            {
                await _context.Database.OpenConnectionAsync();
            }
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM information_schema.schemata WHERE schema_name = 'avia'";
            var result = await command.ExecuteScalarAsync();
            
            var exists = result != null;
            System.Diagnostics.Debug.WriteLine($"CheckSchemaExistsAsync: Schema 'avia' exists: {exists}");
            
            return exists;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking schema: {ex.Message}");
            return false;
        }
        finally
        {
            if (!connectionWasOpen)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }

    private async Task<bool> CheckTablesExistAsync()
    {
        var connection = _context.Database.GetDbConnection();
        var connectionWasOpen = connection.State == System.Data.ConnectionState.Open;
        
        try
        {
            if (!connectionWasOpen)
            {
                await _context.Database.OpenConnectionAsync();
            }

            // Устанавливаем search_path
            try
            {
                var setPathCommand = connection.CreateCommand();
                setPathCommand.CommandText = "SET search_path TO avia";
                await setPathCommand.ExecuteNonQueryAsync();
            }
            catch
            {
            }
            
            var command = connection.CreateCommand();
            // PostgreSQL приводит имена к нижнему регистру, если они не в кавычках
            // Поэтому ищем таблицы в нижнем регистре
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_schema = 'avia' 
                AND LOWER(table_name) IN ('users', 'flights', 'tickets')";
            
            var count = await command.ExecuteScalarAsync();
            var result = count != null && Convert.ToInt32(count) == 3;
            
            System.Diagnostics.Debug.WriteLine($"CheckTablesExistAsync: Found {count} tables, expected 3, result: {result}");
            
            if (!result)
            {
                // Проверяем, какие таблицы действительно существуют
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = 'avia'";
                using (var reader = await checkCommand.ExecuteReaderAsync())
                {
                    var tables = new List<string>();
                    while (await reader.ReadAsync())
                    {
                        tables.Add(reader.GetString(0));
                    }
                    System.Diagnostics.Debug.WriteLine($"Actual tables in schema 'avia': {string.Join(", ", tables)}");
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking tables: {ex.Message}");
            return false;
        }
        finally
        {
            if (!connectionWasOpen)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }

    private async Task ExecuteDatabaseScriptAsync()
    {
        // Читаем SQL скрипт - ищем в нескольких местах
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var currentDir = Directory.GetCurrentDirectory();
        
        // Вычисляем пути к родительским директориям
        var parent1 = Directory.GetParent(baseDir)?.FullName ?? "";
        var parent2 = Directory.GetParent(parent1)?.FullName ?? "";
        var parent3 = Directory.GetParent(parent2)?.FullName ?? "";
        var parent4 = Directory.GetParent(parent3)?.FullName ?? "";
        var parent5 = Directory.GetParent(parent4)?.FullName ?? ""; // Это должен быть корень проекта
        
        var currentParent1 = Directory.GetParent(currentDir)?.FullName ?? "";
        var currentParent2 = Directory.GetParent(currentParent1)?.FullName ?? "";
        var currentParent3 = Directory.GetParent(currentParent2)?.FullName ?? "";
        var currentParent4 = Directory.GetParent(currentParent3)?.FullName ?? "";
        var currentParent5 = Directory.GetParent(currentParent4)?.FullName ?? "";
        
        var possiblePaths = new List<string>
        {
            // В текущей директории приложения
            Path.Combine(baseDir, "DBCreate.sql"),
            Path.Combine(currentDir, "DBCreate.sql"),
            
            // На уровень выше
            Path.Combine(parent1, "DBCreate.sql"),
            Path.Combine(currentParent1, "DBCreate.sql"),
            
            // На два уровня выше
            Path.Combine(parent2, "DBCreate.sql"),
            Path.Combine(currentParent2, "DBCreate.sql"),
            
            // На три уровня выше
            Path.Combine(parent3, "DBCreate.sql"),
            Path.Combine(currentParent3, "DBCreate.sql"),
            
            // На четыре уровня выше
            Path.Combine(parent4, "DBCreate.sql"),
            Path.Combine(currentParent4, "DBCreate.sql"),
            
            // На пять уровней выше (корень проекта)
            Path.Combine(parent5, "DBCreate.sql"),
            Path.Combine(currentParent5, "DBCreate.sql")
        };

        // Убираем дубликаты и пустые пути
        possiblePaths = possiblePaths
            .Where(p => !string.IsNullOrEmpty(p) && p != "DBCreate.sql" && !p.EndsWith("\\DBCreate.sql"))
            .Distinct()
            .ToList();

        string? scriptPath = null;
        System.Diagnostics.Debug.WriteLine("Searching for DBCreate.sql in the following paths:");
        foreach (var path in possiblePaths)
        {
            System.Diagnostics.Debug.WriteLine($"  Checking: {path}");
            if (File.Exists(path))
            {
                scriptPath = path;
                System.Diagnostics.Debug.WriteLine($"  Found at: {path}");
                break;
            }
        }

        // Если не нашли в списке, ищем рекурсивно вверх по директориям
        if (scriptPath == null)
        {
            System.Diagnostics.Debug.WriteLine("File not found in predefined paths, searching upward...");
            var searchDir = new DirectoryInfo(baseDir);
            while (searchDir != null && searchDir.Exists)
            {
                var testPath = Path.Combine(searchDir.FullName, "DBCreate.sql");
                System.Diagnostics.Debug.WriteLine($"  Checking upward: {testPath}");
                if (File.Exists(testPath))
                {
                    scriptPath = testPath;
                    System.Diagnostics.Debug.WriteLine($"  Found at: {scriptPath}");
                    break;
                }
                searchDir = searchDir.Parent;
            }
        }

        if (scriptPath == null)
        {
            var errorMessage = $"SQL скрипт DBCreate.sql не найден. Проверенные пути:\n" +
                string.Join("\n", possiblePaths.Select(p => $"  - {p}"));
            System.Diagnostics.Debug.WriteLine(errorMessage);
            throw new FileNotFoundException(errorMessage);
        }

        var script = await File.ReadAllTextAsync(scriptPath, Encoding.UTF8);
        System.Diagnostics.Debug.WriteLine($"Loaded SQL script from: {scriptPath}");

        // Используем Npgsql напрямую для выполнения скрипта
        var connection = _context.Database.GetDbConnection() as NpgsqlConnection;
        if (connection == null)
        {
            throw new InvalidOperationException("Не удалось получить NpgsqlConnection");
        }

        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        
        if (wasClosed)
        {
            await connection.OpenAsync();
        }

        try
        {
            // Разбиваем скрипт на команды и выполняем по отдельности
            System.Diagnostics.Debug.WriteLine("Executing SQL script...");
            
            var commands = SplitScriptIntoCommands(script);
            System.Diagnostics.Debug.WriteLine($"Split script into {commands.Count} commands");

            int successCount = 0;
            int errorCount = 0;
            int skipCount = 0;

            foreach (var commandText in commands)
            {
                if (string.IsNullOrWhiteSpace(commandText))
                    continue;

                var trimmed = commandText.Trim();
                
                // Пропускаем комментарии
                if (trimmed.StartsWith("--") || trimmed.Length < 5)
                    continue;

                // Логируем важные команды
                if (trimmed.StartsWith("CREATE SCHEMA", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("CREATE TYPE", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"Executing: {trimmed.Substring(0, Math.Min(50, trimmed.Length))}...");
                }

                try
                {
                    using (var command = new NpgsqlCommand(commandText, connection))
                    {
                        command.CommandTimeout = 60;
                        await command.ExecuteNonQueryAsync();
                        successCount++;
                        
                        // После создания схемы, устанавливаем search_path
                        if (trimmed.StartsWith("CREATE SCHEMA", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var setPathCommand = new NpgsqlCommand("SET search_path TO avia", connection))
                            {
                                await setPathCommand.ExecuteNonQueryAsync();
                                System.Diagnostics.Debug.WriteLine("Schema 'avia' created and search_path set");
                            }
                        }
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "42710" || ex.SqlState == "42P07" || ex.SqlState == "42P16")
                {
                    // Игнорируем ошибки "уже существует"
                    skipCount++;
                    System.Diagnostics.Debug.WriteLine($"Skipping existing object (SQL State: {ex.SqlState}): {ex.Message}");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    System.Diagnostics.Debug.WriteLine($"Error executing command: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Command preview: {trimmed.Substring(0, Math.Min(100, trimmed.Length))}...");
                    
                    // Для критических ошибок прерываем выполнение
                    if (!ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) &&
                        !(ex is PostgresException pgEx && (pgEx.SqlState == "42710" || pgEx.SqlState == "42P07" || pgEx.SqlState == "42P16")))
                    {
                        System.Diagnostics.Debug.WriteLine($"Critical error, stopping script execution");
                        throw;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Script execution completed: {successCount} successful, {errorCount} errors, {skipCount} skipped");

            // Убеждаемся, что search_path установлен
            using (var setPathCommand = new NpgsqlCommand("SET search_path TO avia", connection))
            {
                await setPathCommand.ExecuteNonQueryAsync();
            }

            // Даем время на завершение транзакций и индексацию
            await Task.Delay(1000);
            
            System.Diagnostics.Debug.WriteLine("Waiting for database operations to complete...");

            // Проверяем, что таблицы созданы
            var tablesCheck = await CheckTablesExistAsync();
            if (!tablesCheck)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: Tables were not created after script execution!");
                System.Diagnostics.Debug.WriteLine("Attempting to verify schema and tables manually...");
                
                // Проверяем схему
                using (var schemaCheckCommand = new NpgsqlCommand(
                    "SELECT 1 FROM information_schema.schemata WHERE schema_name = 'avia'", connection))
                {
                    var schemaExists = await schemaCheckCommand.ExecuteScalarAsync() != null;
                    System.Diagnostics.Debug.WriteLine($"Schema 'avia' exists: {schemaExists}");
                    
                    if (schemaExists)
                    {
                        // Проверяем таблицы напрямую
                        using (var tablesCheckCommand = new NpgsqlCommand(
                            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'avia'", connection))
                        {
                            using (var reader = await tablesCheckCommand.ExecuteReaderAsync())
                            {
                                var tables = new List<string>();
                                while (await reader.ReadAsync())
                                {
                                    tables.Add(reader.GetString(0));
                                }
                                System.Diagnostics.Debug.WriteLine($"Found tables in schema 'avia': {string.Join(", ", tables)}");
                            }
                        }
                    }
                }
                
                var errorDetails = new StringBuilder();
                errorDetails.AppendLine("Таблицы не были созданы после выполнения SQL скрипта.");
                errorDetails.AppendLine();
                errorDetails.AppendLine("Убедитесь, что:");
                errorDetails.AppendLine("1. PostgreSQL сервер запущен");
                errorDetails.AppendLine("2. Строка подключения в appsettings.json корректна");
                errorDetails.AppendLine("3. База данных доступна");
                errorDetails.AppendLine("4. Файл DBCreate.sql находится в корне проекта");
                errorDetails.AppendLine();
                errorDetails.AppendLine("Проверьте окно Output (Debug) для детальной информации.");
                
                throw new InvalidOperationException(errorDetails.ToString());
            }
            
            System.Diagnostics.Debug.WriteLine("Database script execution completed successfully");
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }
    }

    private string RemoveComments(string script)
    {
        var result = new StringBuilder();
        var lines = script.Split('\n');
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // Пропускаем пустые строки и комментарии
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("--"))
            {
                continue;
            }
            
            // Удаляем inline комментарии (все после --)
            var commentIndex = trimmed.IndexOf("--");
            if (commentIndex >= 0)
            {
                trimmed = trimmed.Substring(0, commentIndex).TrimEnd();
            }
            
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                result.AppendLine(trimmed);
            }
        }
        
        return result.ToString();
    }

    private List<string> SplitScriptIntoCommands(string script)
    {
        var commands = new List<string>();
        var currentCommand = new StringBuilder();
        var inString = false;
        var stringChar = '\0';
        var inDollarQuote = false;
        var dollarTag = "";

        for (int i = 0; i < script.Length; i++)
        {
            var ch = script[i];
            var nextCh = i + 1 < script.Length ? script[i + 1] : '\0';

            // Обработка dollar-quoted strings ($$...$$) - должна быть первой
            if (ch == '$' && !inString && !inDollarQuote)
            {
                // Начинаем поиск тега dollar-quote
                var tagStart = i;
                var tagEnd = i + 1;
                
                // Сначала проверяем простой случай $$
                if (tagEnd < script.Length && script[tagEnd] == '$')
                {
                    dollarTag = "$$";
                    inDollarQuote = true;
                    currentCommand.Append(dollarTag);
                    i = tagEnd;
                    continue;
                }
                
                // Ищем закрывающий $ для тега с идентификатором ($tag$)
                while (tagEnd < script.Length && script[tagEnd] != '$')
                {
                    // Проверяем, что это валидный символ для тега (буквы, цифры, подчеркивания)
                    var tagChar = script[tagEnd];
                    if (!char.IsLetterOrDigit(tagChar) && tagChar != '_')
                    {
                        break;
                    }
                    tagEnd++;
                }
                
                if (tagEnd < script.Length && script[tagEnd] == '$')
                {
                    dollarTag = script.Substring(tagStart, tagEnd - tagStart + 1);
                    inDollarQuote = true;
                    currentCommand.Append(dollarTag);
                    i = tagEnd;
                    continue;
                }
            }
            
            // Проверяем закрытие dollar-quote
            if (inDollarQuote && ch == '$')
            {
                // Проверяем, достаточно ли символов для закрывающего тега
                if (i + dollarTag.Length - 1 < script.Length)
                {
                    var possibleEnd = script.Substring(i, dollarTag.Length);
                    if (possibleEnd == dollarTag)
                    {
                        currentCommand.Append(dollarTag);
                        i += dollarTag.Length - 1; // -1 потому что цикл увеличит i
                        inDollarQuote = false;
                        dollarTag = "";
                        continue;
                    }
                }
            }

            if (inDollarQuote)
            {
                currentCommand.Append(ch);
                continue;
            }

            // Обработка обычных строковых литералов
            if ((ch == '\'' || ch == '"') && (i == 0 || script[i - 1] != '\\'))
            {
                if (!inString)
                {
                    inString = true;
                    stringChar = ch;
                }
                else if (ch == stringChar)
                {
                    inString = false;
                }
                currentCommand.Append(ch);
                continue;
            }

            if (inString)
            {
                currentCommand.Append(ch);
                continue;
            }

            // Пропускаем комментарии
            if (ch == '-' && nextCh == '-')
            {
                // Пропускаем до конца строки
                while (i < script.Length && script[i] != '\n')
                    i++;
                continue;
            }

            if (ch == '/' && nextCh == '*')
            {
                // Пропускаем многострочные комментарии
                i += 2;
                while (i < script.Length - 1)
                {
                    if (script[i] == '*' && script[i + 1] == '/')
                    {
                        i++;
                        break;
                    }
                    i++;
                }
                continue;
            }

            // Разделитель команд - точка с запятой (только если не внутри dollar-quote или строки)
            if (ch == ';' && !inDollarQuote && !inString)
            {
                currentCommand.Append(ch);
                var cmd = currentCommand.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    commands.Add(cmd);
                }
                currentCommand.Clear();
                continue;
            }

            currentCommand.Append(ch);
        }

        // Добавляем последнюю команду, если она есть
        var lastCmd = currentCommand.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(lastCmd))
        {
            commands.Add(lastCmd);
        }

        return commands;
    }

}


