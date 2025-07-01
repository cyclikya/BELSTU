using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using static lab2.Form2;

namespace lab2
{
    public partial class Form1 : Form
    {
        public delegate void BudgetEventHandler(decimal budget);
        public event BudgetEventHandler BudgetCalculated;

        private List<Course> courses = new List<Course>();

        public Form1()
        {
            InitializeComponent();

            LoadFromFile();
            BudgetCalculated += UpdateBudgetLabel;
        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            int yOffset = 100;
            foreach (var course in courses)
            {
                Label newLabel = new Label
                {
                    Text = course.GetCourseInfo(),
                    Location = new Point(10, yOffset),
                    Size = new Size(800, 60)
                };

                this.Controls.Add(newLabel);
                yOffset += newLabel.Height + 3;
            }
        }
        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            using (Form2 form2 = new Form2())
            {
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    courses.Add(form2.NewCourse);
                    SaveToFile();
                    MessageBox.Show("Курс добавлен!");
                }
            }
        }

        private void btnCalculateBudget_Click(object sender, EventArgs e)
        {
            decimal budget = courses.Count * 500;
            BudgetCalculated?.Invoke(budget);
        }

        private void UpdateBudgetLabel(decimal budget)
        {
            lblBudget.Text = $"{budget} руб.";
        }

        private void LoadFromFile()
        {
            if (File.Exists("courses.json"))
            {
                string json = File.ReadAllText("courses.json");
                courses = JsonSerializer.Deserialize<List<Course>>(json) ?? new List<Course>();
            }
        }

        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("courses.json", json);
            
        }
        public class Course
        {
            public string Name { get; set; }
            public int AgeGroup { get; set; }
            public string Difficulty { get; set; }
            public DateTime DataStart { get; set; }
            public int LectureCount { get; set; }
            public int LabCount { get; set; }
            public string ControlType { get; set; }
            public Teacher Teacher { get; set; }
            public List<Literature> Literature { get; set; }

            public string GetCourseInfo()
            {
                string literatureInfo = Literature != null
                    ? string.Join(", ", Literature.ConvertAll(l => $"{l.Title} ({l.Author}, {l.Year})"))
                    : "Нет литературы";

                return $"Курс: {Name} " +
                        $"\n\t\t Возраст: {AgeGroup}, Сложность: {Difficulty}, " +
                        $"Дата начала: {DataStart:dd.MM.yyyy}, Лекции: {LectureCount}, " +
                        $"Лабораторные: {LabCount}, Контроль: {ControlType}, " +
                        $"\n\t\tПреподаватель: {Teacher.FullName} ({Teacher.Department}, Аудитория: {Teacher.Room}), " +
                        $"Литература: {literatureInfo}";
            }
        }

        public class Teacher
        {
            public string Department { get; set; }
            public string FullName { get; set; }
            public string Room { get; set; }
        }

        public class Literature
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public int Year { get; set; }
        }
    }
}
