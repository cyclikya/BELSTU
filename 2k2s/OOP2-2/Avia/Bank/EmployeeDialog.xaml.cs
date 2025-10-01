using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;

namespace Bank
{
    public partial class EmployeeDialog : Window
    {
        private string connectionString;

        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime HireDate { get; set; }
        public byte[] Photo { get; set; }

        public EmployeeDialog(string connStr)
        {
            InitializeComponent();
            connectionString = connStr;
            LoadDepartments();
            dpHireDate.SelectedDate = DateTime.Today;
        }

        private void LoadDepartments()
        {
            List<Department> departments = new List<Department>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT department_id, name FROM departments ORDER BY name", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                DepartmentId = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            cmbDepartments.ItemsSource = departments;
        }

        private void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|All files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Photo = File.ReadAllBytes(dlg.FileName);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(dlg.FileName);
                    bmp.EndInit();
                    imgPhoto.Source = bmp;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки фото: " + ex.Message);
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDepartments.SelectedValue == null)
            {
                MessageBox.Show("Выберите отдел.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию.");
                return;
            }
            if (dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату найма.");
                return;
            }

            DepartmentId = (int)cmbDepartments.SelectedValue;
            FirstName = txtFirstName.Text.Trim();
            LastName = txtLastName.Text.Trim();
            HireDate = dpHireDate.SelectedDate.Value;

            DialogResult = true;
        }

        private class Department
        {
            public int DepartmentId { get; set; }
            public string Name { get; set; }
        }
    }
}
