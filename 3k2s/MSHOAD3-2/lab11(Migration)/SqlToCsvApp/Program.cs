using SqlExportLib;

namespace SqlToCsvApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                @"Server=localhost;Database=lab11_migration;TrustServerCertificate=True;Integrated Security=True;";

            string outputFile = @"C:\General\BELSTU\3k2s\MSHOAD3-2\lab11(Migration)\file\Students.csv";

            var exporter = new SqlToCsvExporter(connectionString);

            exporter.ExportTableToCsv("Students", outputFile);

            Console.WriteLine("Экспорт из MS SQL Server в CSV завершен.");
        }
    }
}