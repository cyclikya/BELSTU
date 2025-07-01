using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static lab2.Form1;

namespace lab2
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            NewCourse = new Course
            {
                Name = txtCourseName.Text,
                AgeGroup = (int)numAge.Value,
                Difficulty = lblDifficulty.Text,
                DataStart = dataStart.Value,
                LectureCount = (int)numLectures.Value,
                LabCount = (int)numLabs.Value,
                ControlType = rbExam.Checked ? "Экзамен" : "Зачет",
                Teacher = new Teacher
                {
                    Department = txtDepartment.Text,
                    FullName = txtTeacherName.Text,
                    Room = txtRoom.Text
                },
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

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
