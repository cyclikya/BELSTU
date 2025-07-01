using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using static lab3.Form1;

namespace lab3
{
    public partial class Form2 : Form
    {
        public Course NewCourse { get; private set; }

        public Form2()
        {
            InitializeComponent();
        }
        private void cbDifficulty_Scroll(object sender, EventArgs e)
        {
            string[] levels = { "Легкий", "Средний", "Сложный" };
            lblDifficulty.Text = levels[cbDifficulty.Value];
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NewCourse = new Course
            {
                Name = txtCourseName.Text,
                AgeGroup = (int)numAge.Value,
                Difficulty = string.IsNullOrWhiteSpace(lblDifficulty.Text) ? "Лёгкий" : lblDifficulty.Text,
                DataStart = dataStart.Value,
                LectureCount = (int)numLectures.Value,
                LabCount = (int)numLabs.Value,
                ControlType = rbExam.Checked ? "Экзамен" : "Зачет",

                Teacher = txtTeacherName.Text,

                //Teacher = new Teacher
                //{
                //    //Department = txtDepartment.Text,
                //    FullName = txtTeacherName.Text,
                //    //Room = txtRoom.Text
                //},

                Literature = new List<Literature>
                {
                    new Literature
                    {
                        Title = txtBookTitle.Text,
                        Author = txtAuthor.Text,
                        Year = (int)numYear.Value
                    }
                }
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(NewCourse);

            // Валидируем курс
            bool isValid = Validator.TryValidateObject(NewCourse, context, results, true);

            //// Валидируем преподавателя
            //var teacherResults = new List<ValidationResult>();
            //var teacherContext = new ValidationContext(NewCourse.Teacher);
            //isValid &= Validator.TryValidateObject(NewCourse.Teacher, teacherContext, teacherResults, true);
            //results.AddRange(teacherResults);

            // Валидируем список литературы
            foreach (var literature in NewCourse.Literature)
            {
                var literatureResults = new List<ValidationResult>();
                var literatureContext = new ValidationContext(literature);
                isValid &= Validator.TryValidateObject(literature, literatureContext, literatureResults, true);
                results.AddRange(literatureResults);
            }

            // Если есть ошибки, выводим их
            if (!isValid)
            {
                MessageBox.Show("Ошибка валидации:\n" + string.Join("\n", results.Select(r => r.ErrorMessage)));
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}