using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab1
{
    public partial class Form1 : Form
    {
        public delegate void CalculationCompletedHandler();
        public event CalculationCompletedHandler CalculationCompleted;

        private ICalculator calculator = new Calculator();

        public Form1()
        {
            InitializeComponent();

            CalculationCompleted += () =>
            {
                ProductLB.ForeColor = Color.Green;
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProductTB.Clear();
            TotalPriceTB.Clear();
            AmountTB.Clear();

            InMonthTB.Clear();
            TypeOfAmount.SelectedIndex = -1;
            PriceLB.Text = "";
            TruePriceLB.Text = "";
            PriceForMonthLB.Text = "";
            ProductLB.Text = "";

            ProductLB.ForeColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string product = calculator.Product(ProductTB.Text);
                double totalPrice = calculator.Num(TotalPriceTB.Text);
                double amount = calculator.Num(AmountTB.Text);
                double inMonth = calculator.Num(InMonthTB.Text);

                double price = calculator.Price(totalPrice, amount);
                double truePrice = calculator.TruePrice(price);
                double priceForMonth = calculator.PiceForMonth(price);

                ProductLB.Text = product;
                PriceLB.Text = price.ToString("F2");
                TruePriceLB.Text = truePrice.ToString("F2");
                PriceForMonthLB.Text = priceForMonth.ToString("F2");

                CalculationCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при вычислениях: " + ex.Message);
            }
        }

        private void AmountTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (TypeOfAmount.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите тип количества!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true;
                return;
            } else if (TypeOfAmount.SelectedIndex == 2)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            } else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == ',' && AmountTB.Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        private void InMonthTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (TypeOfAmount.SelectedIndex == 2)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == ',' && InMonthTB.Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        private void TotalPriceTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true; 
            }

            if (e.KeyChar == ',' && TotalPriceTB.Text.Contains(","))
            {
                e.Handled = true;
            }
        }
    }
}

namespace lab1
{
    public interface ICalculator
    {
        string Product(string product);
        double Num(string str);
        double Price(double x, double y);
        double TruePrice(double x);
        double PiceForMonth(double x);
    }

    public class Calculator : ICalculator
    {
        public string Product(string product)
        {
            return char.ToUpper(product[0]) + product.Substring(1);
        }
        public double Num(string str) => double.Parse(str);
        public double Price(double x, double y) => (double)(x / y);
        public double PiceForMonth(double x) => x * 30;
        public double TruePrice(double x) => x * 0.15;
    }
}
