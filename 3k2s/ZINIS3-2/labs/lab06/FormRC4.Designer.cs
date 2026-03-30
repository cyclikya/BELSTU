namespace lab06
{
    partial class FormRC4
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
            this.lblKey = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.lblPlaintext = new System.Windows.Forms.Label();
            this.txtPlaintext = new System.Windows.Forms.TextBox();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.btnSpeed = new System.Windows.Forms.Button();
            this.lblTimeEnc = new System.Windows.Forms.Label();
            this.lblTimeDec = new System.Windows.Forms.Label();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // lblKey
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(12, 15);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(130, 13);
            this.lblKey.Text = "Ключ (n=8, через запятую):";

            // txtKey
            this.txtKey.Location = new System.Drawing.Point(150, 12);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(460, 20);
            this.txtKey.Text = "122, 125, 48, 84, 201";

            // btnInit
            this.btnInit.Location = new System.Drawing.Point(12, 40);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(155, 27);
            this.btnInit.Text = "Инициализация RC4";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);

            // lblPlaintext
            this.lblPlaintext.AutoSize = true;
            this.lblPlaintext.Location = new System.Drawing.Point(12, 78);
            this.lblPlaintext.Name = "lblPlaintext";
            this.lblPlaintext.Size = new System.Drawing.Size(86, 13);
            this.lblPlaintext.Text = "Открытый текст:";

            // txtPlaintext
            this.txtPlaintext.Location = new System.Drawing.Point(104, 75);
            this.txtPlaintext.Name = "txtPlaintext";
            this.txtPlaintext.Size = new System.Drawing.Size(506, 20);
            this.txtPlaintext.Text = "Угоренко Виолетта Романовна";

            // btnEncrypt
            this.btnEncrypt.Location = new System.Drawing.Point(12, 105);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(120, 27);
            this.btnEncrypt.Text = "Зашифровать";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);

            // btnDecrypt
            this.btnDecrypt.Location = new System.Drawing.Point(140, 105);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(120, 27);
            this.btnDecrypt.Text = "Расшифровать";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);

            // lblTimeEnc
            this.lblTimeEnc.AutoSize = true;
            this.lblTimeEnc.Location = new System.Drawing.Point(12, 142);
            this.lblTimeEnc.Name = "lblTimeEnc";
            this.lblTimeEnc.Size = new System.Drawing.Size(82, 13);
            this.lblTimeEnc.Text = "Шифрование: —";

            // lblTimeDec
            this.lblTimeDec.AutoSize = true;
            this.lblTimeDec.Location = new System.Drawing.Point(280, 142);
            this.lblTimeDec.Name = "lblTimeDec";
            this.lblTimeDec.Size = new System.Drawing.Size(98, 13);
            this.lblTimeDec.Text = "Расшифрование: —";

            // rtbOutput
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbOutput.Location = new System.Drawing.Point(12, 165);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(598, 370);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "";

            // FormRC4
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 547);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.lblPlaintext);
            this.Controls.Add(this.txtPlaintext);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnSpeed);
            this.Controls.Add(this.lblTimeEnc);
            this.Controls.Add(this.lblTimeDec);
            this.Controls.Add(this.rtbOutput);
            this.Name = "FormRC4";
            this.Text = "Приложение 2: RC4 (n=8, ключ: 122,125,48,84,201)";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Label lblPlaintext;
        private System.Windows.Forms.TextBox txtPlaintext;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Button btnSpeed;
        private System.Windows.Forms.Label lblTimeEnc;
        private System.Windows.Forms.Label lblTimeDec;
        private System.Windows.Forms.RichTextBox rtbOutput;
    }
}