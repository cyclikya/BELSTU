namespace lab3
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.SearchMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SortMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.InfoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HideShowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(13, 636);
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
            this.lblBudget.Location = new System.Drawing.Point(344, 636);
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
            this.HeaderLB.Location = new System.Drawing.Point(23, 72);
            this.HeaderLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HeaderLB.Name = "HeaderLB";
            this.HeaderLB.Size = new System.Drawing.Size(369, 35);
            this.HeaderLB.TabIndex = 47;
            this.HeaderLB.Text = "Курс программирования";
            // 
            // btnCalculateBudget
            // 
            this.btnCalculateBudget.Location = new System.Drawing.Point(955, 669);
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
            this.btnShow.Location = new System.Drawing.Point(743, 669);
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
            this.btnAddCourse.Location = new System.Drawing.Point(531, 669);
            this.btnAddCourse.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddCourse.Name = "btnAddCourse";
            this.btnAddCourse.Size = new System.Drawing.Size(204, 45);
            this.btnAddCourse.TabIndex = 54;
            this.btnAddCourse.Text = "Добавить курс";
            this.btnAddCourse.UseVisualStyleBackColor = true;
            this.btnAddCourse.Click += new System.EventHandler(this.btnAddCourse_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SearchMenu,
            this.SortMenu,
            this.InfoMenu,
            this.HideShowMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1174, 28);
            this.menuStrip1.TabIndex = 55;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // SearchMenu
            // 
            this.SearchMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.SearchMenu.Name = "SearchMenu";
            this.SearchMenu.Size = new System.Drawing.Size(66, 26);
            this.SearchMenu.Text = "Поиск";
            this.SearchMenu.Click += new System.EventHandler(this.SearchMenu_Click);
            // 
            // SortMenu
            // 
            this.SortMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.SortMenu.Name = "SortMenu";
            this.SortMenu.Size = new System.Drawing.Size(128, 26);
            this.SortMenu.Text = "Сортировка по";
            this.SortMenu.Click += new System.EventHandler(this.SortMenu_Click);
            // 
            // InfoMenu
            // 
            this.InfoMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.InfoMenu.Name = "InfoMenu";
            this.InfoMenu.Size = new System.Drawing.Size(118, 26);
            this.InfoMenu.Text = "О программе";
            this.InfoMenu.Click += new System.EventHandler(this.InfoMenu_Click);
            // 
            // HideShowMenu
            // 
            this.HideShowMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.HideShowMenu.Name = "HideShowMenu";
            this.HideShowMenu.Size = new System.Drawing.Size(197, 26);
            this.HideShowMenu.Text = "Скрыть/Показать панель";
            this.HideShowMenu.Click += new System.EventHandler(this.HideShowMenu_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Location = new System.Drawing.Point(17, 110);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowHeadersWidth = 51;
            this.dataGrid.RowTemplate.Height = 24;
            this.dataGrid.Size = new System.Drawing.Size(1142, 513);
            this.dataGrid.TabIndex = 56;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1174, 749);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnAddCourse);
            this.Controls.Add(this.btnCalculateBudget);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.HeaderLB);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblBudget);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1(LR_3)";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SearchMenu;
        private System.Windows.Forms.ToolStripMenuItem SortMenu;
        private System.Windows.Forms.ToolStripMenuItem InfoMenu;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.ToolStripMenuItem HideShowMenu;
    }
}