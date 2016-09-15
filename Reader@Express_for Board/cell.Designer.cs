namespace Reader_Express
{
    partial class cell
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCell = new System.Windows.Forms.ComboBox();
            this.btnCell = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Cell:";
            // 
            // cbCell
            // 
            this.cbCell.Items.Add("1");
            this.cbCell.Items.Add("2");
            this.cbCell.Items.Add("3");
            this.cbCell.Items.Add("4");
            this.cbCell.Items.Add("5");
            this.cbCell.Items.Add("6");
            this.cbCell.Items.Add("7");
            this.cbCell.Items.Add("8");
            this.cbCell.Items.Add("9");
            this.cbCell.Location = new System.Drawing.Point(52, 13);
            this.cbCell.Name = "cbCell";
            this.cbCell.Size = new System.Drawing.Size(168, 23);
            this.cbCell.TabIndex = 1;
            // 
            // btnCell
            // 
            this.btnCell.Location = new System.Drawing.Point(52, 58);
            this.btnCell.Name = "btnCell";
            this.btnCell.Size = new System.Drawing.Size(129, 24);
            this.btnCell.TabIndex = 2;
            this.btnCell.Text = "OK";
            this.btnCell.Click += new System.EventHandler(this.btnCell_Click);
            // 
            // cell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(231, 101);
            this.Controls.Add(this.btnCell);
            this.Controls.Add(this.cbCell);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "cell";
            this.Text = "cell";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCell;
        private System.Windows.Forms.Button btnCell;
    }
}