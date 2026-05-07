using Oracle.ManagedDataAccess.Client;
using System.Globalization;

namespace OracleImportLib
{
    public class CsvToOracleImporter
    {
        private readonly string _connectionString;
        private readonly char _delimiter = ';';

        public CsvToOracleImporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ImportStudents(string csvFilePath)
        {
            string[] lines = File.ReadAllLines(csvFilePath);

            using OracleConnection connection = new OracleConnection(_connectionString);
            connection.Open();

            using OracleTransaction transaction = connection.BeginTransaction();

            string sql = @"
                INSERT INTO Students
                (StudentId, FullName, GroupName, AverageMark, AdmissionDate)
                VALUES
                (:StudentId, :FullName, :GroupName, :AverageMark, :AdmissionDate)";

            using OracleCommand command = new OracleCommand(sql, connection);
            command.Transaction = transaction;

            try
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(_delimiter);

                    command.Parameters.Clear();

                    command.Parameters.Add(":StudentId", OracleDbType.Int32)
                        .Value = int.Parse(values[0]);

                    command.Parameters.Add(":FullName", OracleDbType.NVarchar2)
                        .Value = values[1];

                    command.Parameters.Add(":GroupName", OracleDbType.NVarchar2)
                        .Value = values[2];

                    command.Parameters.Add(":AverageMark", OracleDbType.Decimal)
                        .Value = decimal.Parse(values[3], CultureInfo.InvariantCulture);

                    command.Parameters.Add(":AdmissionDate", OracleDbType.Date)
                        .Value = DateTime.ParseExact(values[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}