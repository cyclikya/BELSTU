using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using static lab3.Form1;

namespace lab3
{
    public partial class SearchResults : Form
    {
        public SearchResults(List<Course> results)
        {
            InitializeComponent();
            searchGrid.DataSource = results;
        }

        private void BackMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveMenu_Click(object sender, EventArgs e)
        {
            SaveToFile();
            MessageBox.Show("Результат поиска сохранен");
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("search.json", json);

        }
    }
}