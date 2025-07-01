namespace lab2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label16 = new System.Windows.Forms.Label();
            this.lblBudget = new System.Windows.Forms.Label();
            this.HeaderLB = new System.Windows.Forms.Label();
            this.btnCalculateBudget = new System.Windows.Forms.Button();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnAddCourse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(13, 83);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(309, 24);
            this.label16.TabIndex = 46;
            this.label16.Text = "Результат рассчета бюджета:";
            // 
            // lblBudget
            // 
            this.lblBudget.AutoSize = true;
            this.lblBudget.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblBudget.Location = new System.Drawing.Point(344, 83);
            this.lblBudget.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBudget.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblBudget.Name = "lblBudget";
            this.lblBudget.Size = new System.Drawing.Size(0, 20);
            this.lblBudget.TabIndex = 45;
            // 
            // HeaderLB
            // 
            this.HeaderLB.AutoSize = true;
            this.HeaderLB.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLB.Location = new System.Drawing.Point(23, 20);
            this.HeaderLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HeaderLB.Name = "HeaderLB";
            this.HeaderLB.Size = new System.Drawing.Size(369, 35);
            this.HeaderLB.TabIndex = 47;
            this.HeaderLB.Text = "Курс программирования";
            // 
            // btnCalculateBudget
            // 
            this.btnCalculateBudget.Location = new System.Drawing.Point(649, 691);
            this.btnCalculateBudget.Margin = new System.Windows.Forms.Padding(4);
            this.btnCalculateBudget.Name = "btnCalculateBudget";
            this.btnCalculateBudget.Size = new System.Drawing.Size(204, 45);
            this.btnCalculateBudget.TabIndex = 53;
            this.btnCalculateBudget.Text = "Рассчитать бюджет";
            this.btnCalculateBudget.UseVisualStyleBackColor = true;
            this.btnCalculateBudget.Click += new System.EventHandler(this.btnCalculateBudget_Click);
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(437, 691);
            this.btnShow.Margin = new System.Windows.Forms.Padding(4);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(204, 45);
            this.btnShow.TabIndex = 52;
            this.btnShow.Text = "Вывести данные";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnAddCourse
            // 
            this.btnAddCourse.Location = new System.Drawing.Point(225, 691);
            this.btnAddCourse.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddCourse.Name = "btnAddCourse";
            this.btnAddCourse.Size = new System.Drawing.Size(204, 45);
            this.btnAddCourse.TabIndex = 54;
            this.btnAddCourse.Text = "Добавить курс";
            this.btnAddCourse.UseVisualStyleBackColor = true;
            this.btnAddCourse.Click += new System.EventHandler(this.btnAddCourse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 749);
            this.Controls.Add(this.btnAddCourse);
            this.Controls.Add(this.btnCalculateBudget);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.HeaderLB);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblBudget);
            this.Name = "Form1";
            this.Text = "Form1(LR_2)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblBudget;
        private System.Windows.Forms.Label HeaderLB;
        private System.Windows.Forms.Button btnCalculateBudget;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnAddCourse;
    }
}