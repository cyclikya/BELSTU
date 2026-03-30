namespace lab07
{
    partial class Form1
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
            this.lblEncoding = new System.Windows.Forms.Label();
            this.cmbEncoding = new System.Windows.Forms.ComboBox();
            this.lblZ = new System.Windows.Forms.Label();
            this.nudZ = new System.Windows.Forms.NumericUpDown();
            this.btnGenerateKeys = new System.Windows.Forms.Button();
            this.lblPlaintext = new System.Windows.Forms.Label();
            this.txtPlaintext = new System.Windows.Forms.TextBox();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.lblTimeEncrypt = new System.Windows.Forms.Label();
            this.lblTimeDecrypt = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnAnalyze = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).BeginInit();
            this.SuspendLayout();

            // lblEncoding
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(12, 15);
            this.lblEncoding.Text = "Кодировка:";

            // cmbEncoding
            this.cmbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoding.Items.AddRange(new object[] { "ASCII (z=8)", "Base64 (z=6)" });
            this.cmbEncoding.Location = new System.Drawing.Point(90, 12);
            this.cmbEncoding.Size = new System.Drawing.Size(130, 23);
            this.cmbEncoding.SelectedIndex = 0;
            this.cmbEncoding.SelectedIndexChanged += new System.EventHandler(this.cmbEncoding_SelectedIndexChanged);

            // lblZ
            this.lblZ.AutoSize = true;
            this.lblZ.Location = new System.Drawing.Point(240, 15);
            this.lblZ.Text = "z (размер блока):";

            // nudZ
            this.nudZ.Location = new System.Drawing.Point(360, 12);
            this.nudZ.Minimum = 6;
            this.nudZ.Maximum = 256;
            this.nudZ.Value = 8;
            this.nudZ.Size = new System.Drawing.Size(70, 23);

            // btnGenerateKeys
            this.btnGenerateKeys.Location = new System.Drawing.Point(450, 10);
            this.btnGenerateKeys.Size = new System.Drawing.Size(160, 28);
            this.btnGenerateKeys.Text = "Генерация ключей";
            this.btnGenerateKeys.Click += new System.EventHandler(this.btnGenerateKeys_Click);

            // lblPlaintext
            this.lblPlaintext.AutoSize = true;
            this.lblPlaintext.Location = new System.Drawing.Point(12, 50);
            this.lblPlaintext.Text = "Открытый текст (ФИО):";

            // txtPlaintext
            this.txtPlaintext.Location = new System.Drawing.Point(170, 47);
            this.txtPlaintext.Size = new System.Drawing.Size(440, 23);
            this.txtPlaintext.Text = "Угоренко Виолетта Романовна";

            // btnEncrypt
            this.btnEncrypt.Location = new System.Drawing.Point(12, 80);
            this.btnEncrypt.Size = new System.Drawing.Size(140, 30);
            this.btnEncrypt.Text = "Зашифровать";
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);

            // btnDecrypt
            this.btnDecrypt.Location = new System.Drawing.Point(162, 80);
            this.btnDecrypt.Size = new System.Drawing.Size(140, 30);
            this.btnDecrypt.Text = "Расшифровать";
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);

            // lblTimeEncrypt
            this.lblTimeEncrypt.AutoSize = true;
            this.lblTimeEncrypt.Location = new System.Drawing.Point(12, 120);
            this.lblTimeEncrypt.Text = "Время шифрования: —";

            // lblTimeDecrypt
            this.lblTimeDecrypt.AutoSize = true;
            this.lblTimeDecrypt.Location = new System.Drawing.Point(300, 120);
            this.lblTimeDecrypt.Text = "Время расшифрования: —";

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 140);
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblStatus.Text = "";

            // rtbOutput
            this.rtbOutput.Location = new System.Drawing.Point(12, 165);
            this.rtbOutput.Size = new System.Drawing.Size(600, 370);
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 9F);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 545);
            this.Controls.Add(this.lblEncoding);
            this.Controls.Add(this.cmbEncoding);
            this.Controls.Add(this.lblZ);
            this.Controls.Add(this.nudZ);
            this.Controls.Add(this.btnGenerateKeys);
            this.Controls.Add(this.lblPlaintext);
            this.Controls.Add(this.txtPlaintext);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.lblTimeEncrypt);
            this.Controls.Add(this.lblTimeDecrypt);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.rtbOutput);
            this.Text = "Ранцевый шифр Меркла-Хеллмана";

            ((System.ComponentModel.ISupportInitialize)(this.nudZ)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.ComboBox cmbEncoding;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.NumericUpDown nudZ;
        private System.Windows.Forms.Button btnGenerateKeys;
        private System.Windows.Forms.Label lblPlaintext;
        private System.Windows.Forms.TextBox txtPlaintext;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Label lblTimeEncrypt;
        private System.Windows.Forms.Label lblTimeDecrypt;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnAnalyze;
    }
}