namespace lab06
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnApp1 = new System.Windows.Forms.Button();
            this.btnApp2 = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(350, 25);
            this.lblTitle.Text = "Лаб. работа №6: Потоковые шифры";

            this.btnApp1.Location = new System.Drawing.Point(15, 55);
            this.btnApp1.Name = "btnApp1";
            this.btnApp1.Size = new System.Drawing.Size(210, 45);
            this.btnApp1.Text = "Приложение 1:\r\nГенератор ПСП (RSA)";
            this.btnApp1.UseVisualStyleBackColor = true;
            this.btnApp1.Click += new System.EventHandler(this.btnApp1_Click);

            this.btnApp2.Location = new System.Drawing.Point(240, 55);
            this.btnApp2.Name = "btnApp2";
            this.btnApp2.Size = new System.Drawing.Size(210, 45);
            this.btnApp2.Text = "Приложение 2:\r\nПотоковый шифр RC4";
            this.btnApp2.UseVisualStyleBackColor = true;
            this.btnApp2.Click += new System.EventHandler(this.btnApp2_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 115);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnApp1);
            this.Controls.Add(this.btnApp2);
            this.Name = "MainForm";
            this.Text = "Лабораторная работа №6";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnApp1;
        private System.Windows.Forms.Button btnApp2;
        private System.Windows.Forms.Label lblTitle;
    }
}