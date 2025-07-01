namespace lab3
{
    partial class SearchResults
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
            this.searchGrid = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BackMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HeaderLB = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.searchGrid)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchGrid
            // 
            this.searchGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.searchGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchGrid.Location = new System.Drawing.Point(12, 87);
            this.searchGrid.Name = "searchGrid";
            this.searchGrid.RowHeadersWidth = 51;
            this.searchGrid.RowTemplate.Height = 24;
            this.searchGrid.Size = new System.Drawing.Size(1133, 337);
            this.searchGrid.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BackMenu,
            this.SaveMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1157, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BackMenu
            // 
            this.BackMenu.Name = "BackMenu";
            this.BackMenu.Size = new System.Drawing.Size(65, 24);
            this.BackMenu.Text = "Назад";
            this.BackMenu.Click += new System.EventHandler(this.BackMenu_Click);
            // 
            // SaveMenu
            // 
            this.SaveMenu.Name = "SaveMenu";
            this.SaveMenu.Size = new System.Drawing.Size(97, 24);
            this.SaveMenu.Text = "Сохранить";
            this.SaveMenu.Click += new System.EventHandler(this.SaveMenu_Click);
            // 
            // HeaderLB
            // 
            this.HeaderLB.AutoSize = true;
            this.HeaderLB.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLB.Location = new System.Drawing.Point(13, 37);
            this.HeaderLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HeaderLB.Name = "HeaderLB";
            this.HeaderLB.Size = new System.Drawing.Size(303, 35);
            this.HeaderLB.TabIndex = 48;
            this.HeaderLB.Text = "Результаты поиска:";
            // 
            // SearchResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1157, 450);
            this.Controls.Add(this.HeaderLB);
            this.Controls.Add(this.searchGrid);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SearchResults";
            this.Text = "SearchResults";
            ((System.ComponentModel.ISupportInitialize)(this.searchGrid)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView searchGrid;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BackMenu;
        private System.Windows.Forms.Label HeaderLB;
        private System.Windows.Forms.ToolStripMenuItem SaveMenu;
    }
}