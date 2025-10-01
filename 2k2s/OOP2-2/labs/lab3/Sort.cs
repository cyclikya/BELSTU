using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Windows.Forms;
using static lab3.Form1;

namespace lab3
{
    public partial class Sort : Form
    {
        private List<Course> sortResults;

        public Sort()
        {
            InitializeComponent();
            LoadFromFile();
        }

        private void BackMenu_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveMenu_Click(object sender, EventArgs e)
        {
            SaveToFile();
            MessageBox.Show("Результат сортировки сохранен");
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(sortResults, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("sort.json", json);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            bool ascending = rbDesc.Checked;
            
            sortResults = courses.OrderBy(c =>
            {
                object key;
                if (sortBy.SelectedItem.ToString() == "Названию")
                    key = c.Name;
                else if (sortBy.SelectedItem.ToString() == "Сложности")
                {
                    key = c.Difficulty == "Сложный" ? 1 :
                          c.Difficulty == "Средний" ? 2 :
                          c.Difficulty == "Легкий" ? 3 : 4;
                }
                else if (sortBy.SelectedItem.ToString() == "Кол-во лекций")
                    key = c.LectureCount;
                else if (sortBy.SelectedItem.ToString() == "Тип контроля знаний")
                    key = c.ControlType;
                else
                    key = c.Name;

                return (IComparable)key;
            }).ToList();

            if (!ascending)
            {
                sortResults.Reverse();
            }
            sortGrid.DataSource = null;
            sortGrid.DataSource = sortResults;
        }
    }
}
