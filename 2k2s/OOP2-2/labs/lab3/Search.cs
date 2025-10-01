using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static lab3.Form1;
using System.Linq;


namespace lab3
{
    public partial class Search : Form
    {
        private Dictionary<CheckBox, string> regexPatterns;

        public Search()
        {
            InitializeComponent();
            LoadFromFile();
            InitializeRegexPatterns();
        }

        private void InitializeRegexPatterns()
        {
            regexPatterns = new Dictionary<CheckBox, string>
            {
                { cb1, "^[А-ЯЁA-Z].*" }, // Начинается с заглавной буквы
                { cb2, "\\d+" }, // Содержит цифры
                { cb3, "^[А-ЯЁA-Z]+$" } // Только заглавные буквы
            };
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchName.Text.Trim();
            var selectedPatterns = regexPatterns
                .Where(kv => kv.Key.Checked)
                .Select(kv => kv.Value)
                .ToList();

            int minAge = (int)(num1.Value > 0 ? num1.Value : 10);
            int maxAge = (int)(num2.Value > 0 ? num2.Value : 50);

            string agePattern = $"^({string.Join("|", Enumerable.Range(minAge, maxAge - minAge + 1))})$";

            if (string.IsNullOrEmpty(searchText) && selectedPatterns.Count == 0 && num1.Value == 0 && num2.Value == 0)
            {
                MessageBox.Show("Введите текст для поиска, выберите фильтр или укажите возрастной диапазон.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filteredResults = courses
                .Where(c =>
                    (string.IsNullOrEmpty(searchText) ||
                     c.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                     c.Teacher.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (selectedPatterns.Count == 0 || selectedPatterns.Any(pattern =>
                        Regex.IsMatch(c.Name, pattern))) &&
                    Regex.IsMatch(c.AgeGroup.ToString(), agePattern)
                )
                .ToList();

            if (filteredResults.Count > 0)
            {
                SearchResults resultsForm = new SearchResults(filteredResults);
                resultsForm.ShowDialog();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Курсы не найдены.", "Результаты поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BackMenu_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
