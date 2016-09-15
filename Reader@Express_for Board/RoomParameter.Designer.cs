namespace Reader_Express
{
    partial class RoomParameter
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
            this.btnRoomOK = new System.Windows.Forms.Button();
            this.chb2D = new System.Windows.Forms.CheckBox();
            this.chb3D = new System.Windows.Forms.CheckBox();
            this.cbRoomWidth = new System.Windows.Forms.ComboBox();
            this.cbRoomHeight = new System.Windows.Forms.ComboBox();
            this.cbRoomDepth = new System.Windows.Forms.ComboBox();
            this.cbRoomRatio = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRoomOK
            // 
            this.btnRoomOK.Location = new System.Drawing.Point(74, 230);
            this.btnRoomOK.Name = "btnRoomOK";
            this.btnRoomOK.Size = new System.Drawing.Size(85, 30);
            this.btnRoomOK.TabIndex = 0;
            this.btnRoomOK.Text = "OK";
            this.btnRoomOK.Click += new System.EventHandler(this.btnRoomOK_Click);
            // 
            // chb2D
            // 
            this.chb2D.Location = new System.Drawing.Point(24, 24);
            this.chb2D.Name = "chb2D";
            this.chb2D.Size = new System.Drawing.Size(56, 20);
            this.chb2D.TabIndex = 1;
            this.chb2D.Text = "2D";
            // 
            // chb3D
            // 
            this.chb3D.Location = new System.Drawing.Point(146, 24);
            this.chb3D.Name = "chb3D";
            this.chb3D.Size = new System.Drawing.Size(53, 20);
            this.chb3D.TabIndex = 1;
            this.chb3D.Text = "3D";
            // 
            // cbRoomWidth
            // 
            this.cbRoomWidth.Items.Add("60");
            this.cbRoomWidth.Items.Add("90");
            this.cbRoomWidth.Items.Add("120");
            this.cbRoomWidth.Location = new System.Drawing.Point(86, 60);
            this.cbRoomWidth.Name = "cbRoomWidth";
            this.cbRoomWidth.Size = new System.Drawing.Size(113, 23);
            this.cbRoomWidth.TabIndex = 2;
            // 
            // cbRoomHeight
            // 
            this.cbRoomHeight.Items.Add("60");
            this.cbRoomHeight.Items.Add("90");
            this.cbRoomHeight.Items.Add("120");
            this.cbRoomHeight.Location = new System.Drawing.Point(86, 103);
            this.cbRoomHeight.Name = "cbRoomHeight";
            this.cbRoomHeight.Size = new System.Drawing.Size(113, 23);
            this.cbRoomHeight.TabIndex = 2;
            // 
            // cbRoomDepth
            // 
            this.cbRoomDepth.Items.Add("60");
            this.cbRoomDepth.Items.Add("90");
            this.cbRoomDepth.Location = new System.Drawing.Point(86, 142);
            this.cbRoomDepth.Name = "cbRoomDepth";
            this.cbRoomDepth.Size = new System.Drawing.Size(113, 23);
            this.cbRoomDepth.TabIndex = 2;
            // 
            // cbRoomRatio
            // 
            this.cbRoomRatio.Items.Add("3");
            this.cbRoomRatio.Items.Add("4");
            this.cbRoomRatio.Location = new System.Drawing.Point(86, 186);
            this.cbRoomRatio.Name = "cbRoomRatio";
            this.cbRoomRatio.Size = new System.Drawing.Size(113, 23);
            this.cbRoomRatio.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 20);
            this.label4.Text = "Ratio";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 20);
            this.label3.Text = "Depth";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 21);
            this.label2.Text = "Height";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.Text = "Width";
            // 
            // RoomParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(227, 273);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbRoomRatio);
            this.Controls.Add(this.cbRoomDepth);
            this.Controls.Add(this.cbRoomHeight);
            this.Controls.Add(this.cbRoomWidth);
            this.Controls.Add(this.chb3D);
            this.Controls.Add(this.chb2D);
            this.Controls.Add(this.btnRoomOK);
            this.Name = "RoomParameter";
            this.Text = " Room Parameter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRoomOK;
        private System.Windows.Forms.CheckBox chb2D;
        private System.Windows.Forms.CheckBox chb3D;
        private System.Windows.Forms.ComboBox cbRoomWidth;
        private System.Windows.Forms.ComboBox cbRoomHeight;
        private System.Windows.Forms.ComboBox cbRoomDepth;
        private System.Windows.Forms.ComboBox cbRoomRatio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}