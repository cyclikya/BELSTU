namespace lab3
{
    partial class Sort
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BackMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HeaderLB = new System.Windows.Forms.Label();
            this.sortGrid = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.sortBy = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.rbAsc = new System.Windows.Forms.RadioButton();
            this.rbDesc = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sortGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BackMenu,
            this.SaveMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1159, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BackMenu
            // 
            this.BackMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.BackMenu.Name = "BackMenu";
            this.BackMenu.Size = new System.Drawing.Size(65, 24);
            this.BackMenu.Text = "Назад";
            this.BackMenu.Click += new System.EventHandler(this.BackMenu_Click);
            // 
            // SaveMenu
            // 
            this.SaveMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.SaveMenu.Name = "SaveMenu";
            this.SaveMenu.Size = new System.Drawing.Size(97, 24);
            this.SaveMenu.Text = "Сохранить";
            this.SaveMenu.Click += new System.EventHandler(this.SaveMenu_Click);
            // 
            // HeaderLB
            // 
            this.HeaderLB.AutoSize = true;
            this.HeaderLB.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLB.Location = new System.Drawing.Point(13, 56);
            this.HeaderLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HeaderLB.Name = "HeaderLB";
            this.HeaderLB.Size = new System.Drawing.Size(185, 35);
            this.HeaderLB.TabIndex = 59;
            this.HeaderLB.Text = "Сортировка";
            // 
            // sortGrid
            // 
            this.sortGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.sortGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sortGrid.Location = new System.Drawing.Point(14, 197);
            this.sortGrid.Name = "sortGrid";
            this.sortGrid.RowHeadersWidth = 51;
            this.sortGrid.RowTemplate.Height = 24;
            this.sortGrid.Size = new System.Drawing.Size(1133, 337);
            this.sortGrid.TabIndex = 60;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(971, 541);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(175, 35);
            this.btnSearch.TabIndex = 61;
            this.btnSearch.Text = "Найти";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // sortBy
            // 
            this.sortBy.FormattingEnabled = true;
            this.sortBy.Items.AddRange(new object[] {
            "Названию",
            "Сложности",
            "Кол-во лекций",
            "Тип контроля знаний"});
            this.sortBy.Location = new System.Drawing.Point(168, 127);
            this.sortBy.Name = "sortBy";
            this.sortBy.Size = new System.Drawing.Size(198, 24);
            this.sortBy.TabIndex = 62;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new System.Drawing.Point(15, 127);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(146, 20);
            this.label17.TabIndex = 66;
            this.label17.Text = "Сортировать по";
            // 
            // rbAsc
            // 
            this.rbAsc.AutoSize = true;
            this.rbAsc.Location = new System.Drawing.Point(420, 128);
            this.rbAsc.Name = "rbAsc";
            this.rbAsc.Size = new System.Drawing.Size(137, 20);
            this.rbAsc.TabIndex = 67;
            this.rbAsc.Text = "По возрастанию";
            this.rbAsc.UseVisualStyleBackColor = true;
            // 
            // rbDesc
            // 
            this.rbDesc.AutoSize = true;
            this.rbDesc.Location = new System.Drawing.Point(563, 127);
            this.rbDesc.Name = "rbDesc";
            this.rbDesc.Size = new System.Drawing.Size(116, 20);
            this.rbDesc.TabIndex = 68;
            this.rbDesc.Text = "По убыванию";
            this.rbDesc.UseVisualStyleBackColor = true;
            // 
            // Sort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1159, 607);
            this.Controls.Add(this.rbDesc);
            this.Controls.Add(this.rbAsc);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.sortBy);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.sortGrid);
            this.Controls.Add(this.HeaderLB);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Sort";
            this.Text = "Sort";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sortGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BackMenu;
        private System.Windows.Forms.Label HeaderLB;
        private System.Windows.Forms.ToolStripMenuItem SaveMenu;
        private System.Windows.Forms.DataGridView sortGrid;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox sortBy;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.RadioButton rbAsc;
        private System.Windows.Forms.RadioButton rbDesc;
    }
}