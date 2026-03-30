using lab06;
using System;
using System.Windows.Forms;

namespace lab06
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnApp1_Click(object sender, EventArgs e)
        {
            FormRSAGenerator form = new FormRSAGenerator();
            form.Show();
        }

        private void btnApp2_Click(object sender, EventArgs e)
        {
            FormRC4 form = new FormRC4();
            form.Show();
        }
    }
}