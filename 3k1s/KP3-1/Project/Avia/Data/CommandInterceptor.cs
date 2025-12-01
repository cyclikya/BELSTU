using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Avia.Data;

public class SearchPathCommandInterceptor : DbCommandInterceptor
{
    private static readonly HashSet<string> _connectionsWithSearchPath = new();

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        SetSearchPath(command);
        FixEnumParameters(command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void FixEnumParameters(DbCommand command)
    {
        if (command is NpgsqlCommand npgsqlCommand)
        {
            // Создаем словарь для отслеживания параметров ENUM типов
            var enumParams = new Dictionary<string, string>();

            // Проверяем параметры по значениям и определяем их ENUM типы
            foreach (NpgsqlParameter parameter in npgsqlCommand.Parameters)
            {
                if (parameter.Value is string stringValue)
                {
                    string? enumType = null;
                    
                    // Проверяем, является ли это значением role_type
                    if (stringValue.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("client", StringComparison.OrdinalIgnoreCase))
                    {
                        enumType = "avia.role_type";
                        parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Unknown;
                        parameter.DataTypeName = enumType;
                    }
                    // Проверяем, является ли это значением class_type
                    else if (stringValue.Equals("economy", StringComparison.OrdinalIgnoreCase) ||
                             stringValue.Equals("business", StringComparison.OrdinalIgnoreCase))
                    {
                        enumType = "avia.class_type";
                        parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Unknown;
                        parameter.DataTypeName = enumType;
                    }
                    // Проверяем, является ли это значением ticket_status
                    else if (stringValue.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                             stringValue.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        enumType = "avia.ticket_status";
                        parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Unknown;
                        parameter.DataTypeName = enumType;
                    }

                    if (enumType != null)
                    {
                        enumParams[parameter.ParameterName] = enumType;
                    }
                }
            }

            // Модифицируем SQL для добавления CAST на основе имени столбца
            var sql = npgsqlCommand.CommandText;
            var originalSql = sql;

            // Для UPDATE: заменяем SET column = @param на SET column = @param::enum_type
            sql = Regex.Replace(sql, 
                @"SET\s+accessrole\s*=\s*@(\w+)", 
                "SET accessrole = @$1::role_type", 
                RegexOptions.IgnoreCase);

            sql = Regex.Replace(sql, 
                @"SET\s+classtype\s*=\s*@(\w+)", 
                "SET classtype = @$1::class_type", 
                RegexOptions.IgnoreCase);
            
            sql = Regex.Replace(sql, 
                @"SET\s+status\s*=\s*@(\w+)", 
                "SET status = @$1::ticket_status", 
                RegexOptions.IgnoreCase);

            // Для INSERT: используем простой подход - заменяем параметры на основе позиции в списке столбцов
            // Находим INSERT команду
            if (sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase) && 
                sql.Contains("accessrole", StringComparison.OrdinalIgnoreCase))
            {
                // Простая замена: если в SQL есть accessrole и параметр с ENUM значением, добавляем CAST
                foreach (var paramName in enumParams.Keys)
                {
                    if (enumParams[paramName] == "avia.role_type")
                    {
                        // Заменяем @paramName на @paramName::role_type, но только если это не уже с CAST
                        sql = Regex.Replace(sql, 
                            $@"@({paramName})(?!::)", 
                            $"@$1::role_type", 
                            RegexOptions.IgnoreCase);
                    }
                    else if (enumParams[paramName] == "avia.class_type")
                    {
                        sql = Regex.Replace(sql, 
                            $@"@({paramName})(?!::)", 
                            $"@$1::class_type", 
                            RegexOptions.IgnoreCase);
                    }
                    else if (enumParams[paramName] == "avia.ticket_status")
                    {
                        sql = Regex.Replace(sql, 
                            $@"@({paramName})(?!::)", 
                            $"@$1::ticket_status", 
                            RegexOptions.IgnoreCase);
                    }
                }
            }

            if (sql != originalSql)
            {
                npgsqlCommand.CommandText = sql;
            }
        }
    }

    private void SetSearchPath(DbCommand command)
    {
        if (command.Connection is NpgsqlConnection npgsqlConnection && 
            npgsqlConnection.State == System.Data.ConnectionState.Open)
        {
            var connectionId = npgsqlConnection.ConnectionString;
            
            // Проверяем, установлен ли уже search_path для этого соединения
            if (_connectionsWithSearchPath.Contains(connectionId))
            {
                return;
            }

            try
            {
                var setPathCommand = npgsqlConnection.CreateCommand();
                setPathCommand.CommandText = "SET search_path TO avia";
                setPathCommand.ExecuteNonQuery();
                _connectionsWithSearchPath.Add(connectionId);
            }
            catch
            {
                // Игнорируем ошибку
            }
        }
    }
}

