using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Bank;
using Npgsql;
using System.Configuration;

namespace Bank
{
    public partial class MainWindow : Window
    {
        private string connectionString;
        private string currentTable = "departments";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                EnsureDatabaseAndSchema();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке/создании базы данных и схемы: " + ex.Message);
            }

            connectionString = ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = currentTable == "departments" ?
                    "SELECT department_id, name, location FROM departments ORDER BY department_id" :
                    "SELECT employee_id, department_id, first_name, last_name, hire_date FROM employees ORDER BY employee_id";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }
        private void EnsureDatabaseAndSchema()
        {
            string masterConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=vivi5567";
            string targetDbName = "company";

            using (var conn = new NpgsqlConnection(masterConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbname", conn))
                {
                    cmd.Parameters.AddWithValue("dbname", targetDbName);
                    var exists = cmd.ExecuteScalar();

                    if (exists == null)
                    {
                        using (var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{targetDbName}\"", conn))
                        {
                            createCmd.ExecuteNonQuery();
                        }
                        MessageBox.Show($"База данных '{targetDbName}' была создана.");
                    }
                }
            }

            string targetConnectionString = $"Host=localhost;Port=5432;Database={targetDbName};Username=postgres;Password=vivi5567";

            using (var conn = new NpgsqlConnection(targetConnectionString))
            {
                conn.Open();

                string createDepartmentsTable = @"
            CREATE TABLE IF NOT EXISTS departments (
                department_id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL UNIQUE,
                location VARCHAR(100)
            );";

                string createEmployeesTable = @"
            CREATE TABLE IF NOT EXISTS employees (
                employee_id SERIAL PRIMARY KEY,
                department_id INT NOT NULL REFERENCES departments(department_id) ON DELETE CASCADE,
                first_name VARCHAR(50) NOT NULL,
                last_name VARCHAR(50) NOT NULL,
                photo BYTEA,
                hire_date DATE NOT NULL
            );";

                string createFunction = @"
            CREATE OR REPLACE FUNCTION create_two_departments()
            RETURNS void AS $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM departments WHERE name = 'first') THEN
                    INSERT INTO departments (name, location) VALUES ('first', 'Москва');
                END IF;
                IF NOT EXISTS (SELECT 1 FROM departments WHERE name = 'second') THEN
                    INSERT INTO departments (name, location) VALUES ('second', 'Санкт-Петербург');
                END IF;
            END;
            $$ LANGUAGE plpgsql;";

                using (var cmd = new NpgsqlCommand(createDepartmentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand(createEmployeesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand(createFunction, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void BtnDepartments_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "departments";
            LoadData();
        }
        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "employees";
            LoadData();
        }
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable == "departments")
                AddDepartment();
            else
                AddEmployee();
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для редактирования.");
                return;
            }

            if (currentTable == "departments")
                EditDepartment();
            else
                EditEmployee();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            if (currentTable == "departments")
                DeleteDepartment();
            else
                DeleteEmployee();
        }
        private void BtnCreateTwoDepartments_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT create_two_departments();", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Два отдела успешно созданы (если не существовали).");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании отделов: " + ex.Message);
                }
            }
        }

        private void AddDepartment()
        {
            DepartmentDialog dialog = new DepartmentDialog();
            if (dialog.ShowDialog() == true)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand("INSERT INTO departments (name, location) VALUES (@name, @location)", conn))
                            {
                                cmd.Parameters.AddWithValue("name", dialog.DepartmentName);
                                cmd.Parameters.AddWithValue("location", dialog.Location ?? (object)DBNull.Value);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка добавления отдела: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void EditDepartment()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int id = (int)row["department_id"];

            DepartmentDialog dialog = new DepartmentDialog
            {
                DepartmentName = row["name"].ToString(),
                Location = row["location"] == DBNull.Value ? "" : row["location"].ToString()
            };

            if (dialog.ShowDialog() == true)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand("UPDATE departments SET name = @name, location = @location WHERE department_id = @id", conn))
                            {
                                cmd.Parameters.AddWithValue("name", dialog.DepartmentName);
                                cmd.Parameters.AddWithValue("location", dialog.Location ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("id", id);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка редактирования отдела: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void DeleteDepartment()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int id = (int)row["department_id"];

            if (MessageBox.Show("Удалить отдел?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand("DELETE FROM departments WHERE department_id = @id", conn))
                            {
                                cmd.Parameters.AddWithValue("id", id);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка удаления отдела: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void AddEmployee()
        {
            EmployeeDialog dialog = new EmployeeDialog(connectionString);
            if (dialog.ShowDialog() == true)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand(
                                "INSERT INTO employees (department_id, first_name, last_name, photo, hire_date) VALUES (@dept, @fname, @lname, @photo, @hire)", conn))
                            {
                                cmd.Parameters.AddWithValue("dept", dialog.DepartmentId);
                                cmd.Parameters.AddWithValue("fname", dialog.FirstName);
                                cmd.Parameters.AddWithValue("lname", dialog.LastName);
                                cmd.Parameters.AddWithValue("photo", dialog.Photo ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("hire", dialog.HireDate);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка добавления сотрудника: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void EditEmployee()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int id = (int)row["employee_id"];

            EmployeeDialog dialog = new EmployeeDialog(connectionString)
            {
                DepartmentId = (int)row["department_id"],
                FirstName = row["first_name"].ToString(),
                LastName = row["last_name"].ToString(),
                HireDate = (DateTime)row["hire_date"],
                Photo = null 
            };

            if (dialog.ShowDialog() == true)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand(
                                "UPDATE employees SET department_id=@dept, first_name=@fname, last_name=@lname, photo=@photo, hire_date=@hire WHERE employee_id=@id", conn))
                            {
                                cmd.Parameters.AddWithValue("dept", dialog.DepartmentId);
                                cmd.Parameters.AddWithValue("fname", dialog.FirstName);
                                cmd.Parameters.AddWithValue("lname", dialog.LastName);
                                cmd.Parameters.AddWithValue("photo", dialog.Photo ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("hire", dialog.HireDate);
                                cmd.Parameters.AddWithValue("id", id);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка редактирования сотрудника: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void DeleteEmployee()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int id = (int)row["employee_id"];

            if (MessageBox.Show("Удалить сотрудника?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand("DELETE FROM employees WHERE employee_id = @id", conn))
                            {
                                cmd.Parameters.AddWithValue("id", id);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Ошибка удаления сотрудника: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
