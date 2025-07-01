namespace lab1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProductTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TotalPriceTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TypeOfAmount = new System.Windows.Forms.ComboBox();
            this.AmountTB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.InMonthTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PriceLB = new System.Windows.Forms.Label();
            this.TruePriceLB = new System.Windows.Forms.Label();
            this.PriceForMonthLB = new System.Windows.Forms.Label();
            this.ProductLB = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProductTB
            // 
            this.ProductTB.Location = new System.Drawing.Point(248, 82);
            this.ProductTB.Name = "ProductTB";
            this.ProductTB.Size = new System.Drawing.Size(121, 22);
            this.ProductTB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Введите название товара:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Введите стоимость покупки:";
            // 
            // TotalPriceTB
            // 
            this.TotalPriceTB.Location = new System.Drawing.Point(248, 110);
            this.TotalPriceTB.Name = "TotalPriceTB";
            this.TotalPriceTB.Size = new System.Drawing.Size(121, 22);
            this.TotalPriceTB.TabIndex = 3;
            this.TotalPriceTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TotalPriceTB_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Введите объем:";
            // 
            // TypeOfAmount
            // 
            this.TypeOfAmount.FormattingEnabled = true;
            this.TypeOfAmount.Items.AddRange(new object[] {
            "кг",
            "литр",
            "штука"});
            this.TypeOfAmount.Location = new System.Drawing.Point(375, 138);
            this.TypeOfAmount.Name = "TypeOfAmount";
            this.TypeOfAmount.Size = new System.Drawing.Size(72, 24);
            this.TypeOfAmount.TabIndex = 6;
            // 
            // AmountTB
            // 
            this.AmountTB.Location = new System.Drawing.Point(248, 138);
            this.AmountTB.Name = "AmountTB";
            this.AmountTB.Size = new System.Drawing.Size(121, 22);
            this.AmountTB.TabIndex = 7;
            this.AmountTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AmountTB_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(27, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(327, 24);
            this.label4.TabIndex = 8;
            this.label4.Text = "Калькулятор расходов на товар";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Введите ежедневное потребление:";
            // 
            // InMonthTB
            // 
            this.InMonthTB.Location = new System.Drawing.Point(290, 175);
            this.InMonthTB.Name = "InMonthTB";
            this.InMonthTB.Size = new System.Drawing.Size(121, 22);
            this.InMonthTB.TabIndex = 10;
            this.InMonthTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.InMonthTB_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(791, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 31);
            this.button1.TabIndex = 11;
            this.button1.Text = "Рассчитать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(663, 212);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 31);
            this.button2.TabIndex = 12;
            this.button2.Text = "Очистить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(468, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "Бюджет на месяц:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(468, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "Себестоимость:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(468, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(203, 16);
            this.label9.TabIndex = 13;
            this.label9.Text = "Стоимость за 1 кг/литр/штуку:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(468, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 17);
            this.label10.TabIndex = 17;
            this.label10.Text = "Результат для";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(44, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "Ввод данных";
            // 
            // PriceLB
            // 
            this.PriceLB.AutoSize = true;
            this.PriceLB.Location = new System.Drawing.Point(699, 82);
            this.PriceLB.Name = "PriceLB";
            this.PriceLB.Size = new System.Drawing.Size(0, 16);
            this.PriceLB.TabIndex = 19;
            // 
            // TruePriceLB
            // 
            this.TruePriceLB.AutoSize = true;
            this.TruePriceLB.Location = new System.Drawing.Point(699, 110);
            this.TruePriceLB.Name = "TruePriceLB";
            this.TruePriceLB.Size = new System.Drawing.Size(0, 16);
            this.TruePriceLB.TabIndex = 20;
            // 
            // PriceForMonthLB
            // 
            this.PriceForMonthLB.AutoSize = true;
            this.PriceForMonthLB.Location = new System.Drawing.Point(699, 138);
            this.PriceForMonthLB.Name = "PriceForMonthLB";
            this.PriceForMonthLB.Size = new System.Drawing.Size(0, 16);
            this.PriceForMonthLB.TabIndex = 21;
            // 
            // ProductLB
            // 
            this.ProductLB.AutoSize = true;
            this.ProductLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProductLB.Location = new System.Drawing.Point(699, 49);
            this.ProductLB.Name = "ProductLB";
            this.ProductLB.Size = new System.Drawing.Size(0, 17);
            this.ProductLB.TabIndex = 22;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(906, 248);
            this.Controls.Add(this.ProductLB);
            this.Controls.Add(this.PriceForMonthLB);
            this.Controls.Add(this.TruePriceLB);
            this.Controls.Add(this.PriceLB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.InMonthTB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AmountTB);
            this.Controls.Add(this.TypeOfAmount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TotalPriceTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProductTB);
            this.Name = "Form1";
            this.Text = "Калькулятор расходов на товар";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ProductTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TotalPriceTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox TypeOfAmount;
        private System.Windows.Forms.TextBox AmountTB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox InMonthTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label PriceLB;
        private System.Windows.Forms.Label TruePriceLB;
        private System.Windows.Forms.Label PriceForMonthLB;
        private System.Windows.Forms.Label ProductLB;
    }
}

