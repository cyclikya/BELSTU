using OracleImportLib;

namespace CsvToOracleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string oracleConnectionString =
                "Data Source=//localhost:1521/orcl;User Id=system;Password=Ugorenko;";

            string csvFile = @"C:\General\BELSTU\3k2s\MSHOAD3-2\lab11(Migration)\file\Students.csv";

            var importer = new CsvToOracleImporter(oracleConnectionString);

            importer.ImportStudents(csvFile);

            Console.WriteLine("Импорт из CSV в Oracle завершен.");
        }
    }
}