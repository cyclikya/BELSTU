using System.Windows;
using System.Xml.Linq;

namespace Bank
{
    public partial class DepartmentDialog : Window
    {
        public string DepartmentName { get; set; }
        public string Location { get; set; }

        public DepartmentDialog()
        {
            InitializeComponent();
            txtName.Text = DepartmentName;
            txtLocation.Text = Location;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название отдела.");
                return;
            }

            DepartmentName = txtName.Text.Trim();
            Location = string.IsNullOrWhiteSpace(txtLocation.Text) ? null : txtLocation.Text.Trim();

            DialogResult = true;
        }
    }
}
