namespace lab06
{
    partial class FormRSAGenerator
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
            this.lblP = new System.Windows.Forms.Label();
            this.txtP = new System.Windows.Forms.TextBox();
            this.lblQ = new System.Windows.Forms.Label();
            this.txtQ = new System.Windows.Forms.TextBox();
            this.lblE = new System.Windows.Forms.Label();
            this.txtE = new System.Windows.Forms.TextBox();
            this.lblSeed = new System.Windows.Forms.Label();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.nudCount = new System.Windows.Forms.NumericUpDown();
            this.btnGenParams = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCount)).BeginInit();
            this.SuspendLayout();

            // lblP
            this.lblP.AutoSize = true;
            this.lblP.Location = new System.Drawing.Point(12, 15);
            this.lblP.Name = "lblP";
            this.lblP.Size = new System.Drawing.Size(14, 13);
            this.lblP.Text = "p:";

            // txtP
            this.txtP.Location = new System.Drawing.Point(55, 12);
            this.txtP.Name = "txtP";
            this.txtP.Size = new System.Drawing.Size(560, 20);

            // lblQ
            this.lblQ.AutoSize = true;
            this.lblQ.Location = new System.Drawing.Point(12, 41);
            this.lblQ.Name = "lblQ";
            this.lblQ.Size = new System.Drawing.Size(14, 13);
            this.lblQ.Text = "q:";

            // txtQ
            this.txtQ.Location = new System.Drawing.Point(55, 38);
            this.txtQ.Name = "txtQ";
            this.txtQ.Size = new System.Drawing.Size(560, 20);

            // lblE
            this.lblE.AutoSize = true;
            this.lblE.Location = new System.Drawing.Point(12, 67);
            this.lblE.Name = "lblE";
            this.lblE.Size = new System.Drawing.Size(14, 13);
            this.lblE.Text = "e:";

            // txtE
            this.txtE.Location = new System.Drawing.Point(55, 64);
            this.txtE.Name = "txtE";
            this.txtE.Size = new System.Drawing.Size(250, 20);
            this.txtE.Text = "65537";

            // lblSeed
            this.lblSeed.AutoSize = true;
            this.lblSeed.Location = new System.Drawing.Point(320, 67);
            this.lblSeed.Name = "lblSeed";
            this.lblSeed.Size = new System.Drawing.Size(34, 13);
            this.lblSeed.Text = "seed:";

            // txtSeed
            this.txtSeed.Location = new System.Drawing.Point(360, 64);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(255, 20);

            // lblCount
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(12, 97);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(87, 13);
            this.lblCount.Text = "Кол-во бит ПСП:";

            // nudCount
            this.nudCount.Location = new System.Drawing.Point(105, 95);
            this.nudCount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudCount.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            this.nudCount.Name = "nudCount";
            this.nudCount.Size = new System.Drawing.Size(90, 20);
            this.nudCount.Value = new decimal(new int[] { 256, 0, 0, 0 });

            // btnGenParams
            this.btnGenParams.Location = new System.Drawing.Point(220, 92);
            this.btnGenParams.Name = "btnGenParams";
            this.btnGenParams.Size = new System.Drawing.Size(190, 27);
            this.btnGenParams.Text = "Сгенерировать параметры";
            this.btnGenParams.UseVisualStyleBackColor = true;
            this.btnGenParams.Click += new System.EventHandler(this.btnGenParams_Click);

            // btnGenerate
            this.btnGenerate.Location = new System.Drawing.Point(420, 92);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(195, 27);
            this.btnGenerate.Text = "Генерировать ПСП";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);

            // rtbOutput
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbOutput.Location = new System.Drawing.Point(12, 128);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(603, 400);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "";

            // FormRSAGenerator
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 540);
            this.Controls.Add(this.lblP);
            this.Controls.Add(this.txtP);
            this.Controls.Add(this.lblQ);
            this.Controls.Add(this.txtQ);
            this.Controls.Add(this.lblE);
            this.Controls.Add(this.txtE);
            this.Controls.Add(this.lblSeed);
            this.Controls.Add(this.txtSeed);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.nudCount);
            this.Controls.Add(this.btnGenParams);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.rtbOutput);
            this.Name = "FormRSAGenerator";
            this.Text = "Приложение 1: Генератор ПСП (RSA, 256 бит)";
            ((System.ComponentModel.ISupportInitialize)(this.nudCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblP;
        private System.Windows.Forms.TextBox txtP;
        private System.Windows.Forms.Label lblQ;
        private System.Windows.Forms.TextBox txtQ;
        private System.Windows.Forms.Label lblE;
        private System.Windows.Forms.TextBox txtE;
        private System.Windows.Forms.Label lblSeed;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.NumericUpDown nudCount;
        private System.Windows.Forms.Button btnGenParams;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.RichTextBox rtbOutput;
    }
}