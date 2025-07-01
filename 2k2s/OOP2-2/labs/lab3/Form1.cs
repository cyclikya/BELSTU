using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace lab3
{

    public partial class Form1 : Form
    {
        public delegate void BudgetEventHandler(decimal budget);
        public event BudgetEventHandler BudgetCalculated;

        public static List<Course> courses = new List<Course>();

        private ToolStrip toolStrip;
        public StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            InitializeStatusBar();
            InitializeToolBar();

            LoadFromFile();
            BudgetCalculated += UpdateBudgetLabel;
        }
        private void InitializeToolBar()
        {
            toolStrip = new ToolStrip();

            ToolStripButton searchButton = new ToolStripButton("Поиск");
            searchButton.Click += SearchMenu_Click;
            toolStrip.Items.Add(searchButton);

            ToolStripButton sortButton = new ToolStripButton("Сортировка");
            sortButton.Click += SortMenu_Click;
            toolStrip.Items.Add(sortButton);

            ToolStripButton clearButton = new ToolStripButton("Очистить");
            clearButton.Click += (s, e) => { courses.Clear(); UpdateStatus("Очистка списка"); };
            toolStrip.Items.Add(clearButton);

            ToolStripButton deleteButton = new ToolStripButton("Удалить");
            deleteButton.Click += (s, e) => { if (courses.Count > 0) { courses.RemoveAt(courses.Count - 1); UpdateStatus("Удалён последний элемент"); dataGrid.DataSource = null; dataGrid.DataSource = courses; SaveToFile(); } };
            toolStrip.Items.Add(deleteButton);

            ToolStripButton forwardButton = new ToolStripButton("Вперед");
            forwardButton.Click += (s, e) => UpdateStatus("Действие: Вперед");
            toolStrip.Items.Add(forwardButton);

            ToolStripButton backButton = new ToolStripButton("Назад");
            backButton.Click += (s, e) => UpdateStatus("Действие: Назад");
            toolStrip.Items.Add(backButton);

            Controls.Add(toolStrip);
        }
        private void InitializeStatusBar()
        {
            statusStrip = new StatusStrip();

            statusLabel = new ToolStripStatusLabel($"Текущее кол-во объектов: {courses.Count}");
            statusStrip.Items.Add(statusLabel);

            statusStrip.Items.Add(new ToolStripSeparator());

            ToolStripStatusLabel dateTimeLabel = new ToolStripStatusLabel(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            statusStrip.Items.Add(dateTimeLabel);

            Controls.Add(statusStrip);

            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (s, e) => dateTimeLabel.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            timer.Start();
        }


        public void btnShow_Click(object sender, EventArgs e)
        {
            //dataGrid.AutoGenerateColumns = false;
            //dataGrid.Columns.Clear();

            //dataGrid.Columns.Add("Name", "Name");

            //dataGrid.Columns.Add("AgeGroup", "Возрастная группа");
            //dataGrid.Columns.Add("Difficulty", "Сложность");
            //dataGrid.Columns.Add("DataStart", "Дата начала");
            //dataGrid.Columns.Add("Teacher.FullName", "Преподаватель");

            dataGrid.DataSource = null;
            dataGrid.DataSource = courses;

            UpdateStatus("Выыедена информация");
        }
        public void btnAddCourse_Click(object sender, EventArgs e)
        {
            using (Form2 form2 = new Form2())
            {
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    courses.Add(form2.NewCourse);
                    SaveToFile();
                    MessageBox.Show("Курс добавлен!");
                    UpdateStatus("Добавлен курс");
                }
            }
        }
        public void btnCalculateBudget_Click(object sender, EventArgs e)
        {
            decimal budget = courses.Count * 500;
            BudgetCalculated?.Invoke(budget);
            UpdateStatus("Расчитан бюджет");
        }
       

        public void UpdateBudgetLabel(decimal budget)
        {
            lblBudget.Text = $"{budget} руб.";
        }
        public static void LoadFromFile()
        {
            if (File.Exists("courses.json"))
            {
                string json = File.ReadAllText("courses.json");
                courses = JsonSerializer.Deserialize<List<Course>>(json) ?? new List<Course>();
            }
        }
        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("courses.json", json);
            
        }
        public void UpdateStatus(string message)
        {
            statusLabel.Text = $"Текущее кол-во объектов: {courses.Count} | {message}";
        }

        public class RoomValidationAttribute : ValidationAttribute 
        {
            public RoomValidationAttribute() : base("Неверный формат номера аудитории.") { }
            public override bool IsValid(object value)
            {
                if (value == null)
                    return false;
                string room = value as string;
                if (string.IsNullOrEmpty(room))
                    return false;
                var roomFormat = @"^\d{3}-[1-4]$";
                return Regex.IsMatch(room, roomFormat);
            }
            public override string FormatErrorMessage(string name)
            {
                return string.Format("Поле {0} не соответствует правильному формату номера аудитории. Ожидается формат: 000-4, где 000 - три цифры, а 4 - число от 1 до 4.", name);
            }
        }
        public class Course
        {
            [Required(ErrorMessage = "Название курса обязательно")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно содержать от 2 до 100 символов")]
            public string Name { get; set; }

            [Range(1, 100, ErrorMessage = "Возрастная группа должна быть в диапазоне от 1 до 50")]
            public int AgeGroup { get; set; }

            [Required(ErrorMessage = "Сложность обязательна")]
            [RegularExpression("^(Легкий|Средний|Сложный)$", ErrorMessage = "Допустимые значения: Легкий, Средний, Сложный")]
            public string Difficulty { get; set; }

            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Дата начала обязательна")]
            public DateTime DataStart { get; set; }

            [Range(1, 100, ErrorMessage = "Количество лекций должно быть в диапазоне от 1 до 20")]
            public int LectureCount { get; set; }

            [Range(0, 100, ErrorMessage = "Количество лабораторных работ должно быть в диапазоне от 0 до 20")]
            public int LabCount { get; set; }

            [Required(ErrorMessage = "Тип контроля знаний обязателен")]
            [RegularExpression("^(Экзамен|Зачет)$", ErrorMessage = "Допустимые значения: Экзамен, Зачет")]
            public string ControlType { get; set; }

            [Required(ErrorMessage = "ФИО преподавателя обязательно")]
            [StringLength(100, MinimumLength = 5, ErrorMessage = "ФИО должно содержать от 5 до 100 символов")]
            public string Teacher { get; set; }

            [Required(ErrorMessage = "Список литературы обязателен")]
            public List<Literature> Literature { get; set; }
        }
        //public class Teacher
        //{
        //    //[Required(ErrorMessage = "Кафедра обязательна")]
        //    //[StringLength(100, ErrorMessage = "Название кафедры не должно превышать 100 символов")]
        //    //public string Department { get; set; }

        //    [Required(ErrorMessage = "ФИО преподавателя обязательно")]
        //    [StringLength(100, MinimumLength = 5, ErrorMessage = "ФИО должно содержать от 5 до 100 символов")]
        //    public string FullName { get; set; }

        //    //[Required(ErrorMessage = "Аудитория обязательна")]
        //    //[RoomValidation]
        //    //public string Room { get; set; }
        //}
        public class Literature
        {
            [Required(ErrorMessage = "Название книги обязательно")]
            [StringLength(150, ErrorMessage = "Название книги не должно превышать 150 символов")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Автор обязателен")]
            [StringLength(100, ErrorMessage = "ФИО автора не должно превышать 100 символов")]
            public string Author { get; set; }

            [Range(1500, 2100, ErrorMessage = "Год издания должен быть в диапазоне от 1500 до 2100")]
            public int Year { get; set; }
        }

        private void SearchMenu_Click(object sender, EventArgs e)
        {
            using (Search search = new Search())
            {
                if (search.ShowDialog() == DialogResult.OK)
                {
                    UpdateStatus("Выполнен поиск");
                }
            }
        }
        private void SortMenu_Click(object sender, EventArgs e)
        {
            using (Sort sort = new Sort())
            {
                if (sort.ShowDialog() == DialogResult.OK)
                {
                    UpdateStatus("Выполнена сортировка");
                }
            }
        }
        private void InfoMenu_Click(object sender, EventArgs e)
        {
            string version = "Версия: 1.0.0";
            string developer = "Разработчик: Угоренко Виолетта Романовна ФИТ 2-1-2";
            MessageBox.Show($"{version}\n{developer}", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateStatus("Показана информация о программе");
        }
        private void HideShowMenu_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = !toolStrip.Visible;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGrid.DataSource = null;
        }
    }
}
