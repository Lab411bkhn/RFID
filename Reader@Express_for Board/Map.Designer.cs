namespace Reader_Express
{
    partial class Map
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
            this.labelControl = new System.Windows.Forms.Label();
            this.pnMap = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnEclipse = new System.Windows.Forms.Button();
            this.btnRectangle = new System.Windows.Forms.Button();
            this.chbRaw = new System.Windows.Forms.CheckBox();
            this.pnMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl
            // 
            this.labelControl.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Regular);
            this.labelControl.Location = new System.Drawing.Point(3, 0);
            this.labelControl.Name = "labelControl";
            this.labelControl.Size = new System.Drawing.Size(82, 37);
            this.labelControl.Text = "Map";
            // 
            // pnMap
            // 
            this.pnMap.BackColor = System.Drawing.Color.PowderBlue;
            this.pnMap.Controls.Add(this.btnClear);
            this.pnMap.Controls.Add(this.btnSetting);
            this.pnMap.Controls.Add(this.btnStart);
            this.pnMap.Controls.Add(this.btnStop);
            this.pnMap.Controls.Add(this.btnEclipse);
            this.pnMap.Controls.Add(this.btnRectangle);
            this.pnMap.Controls.Add(this.chbRaw);
            this.pnMap.Controls.Add(this.labelControl);
            this.pnMap.Location = new System.Drawing.Point(3, 3);
            this.pnMap.Name = "pnMap";
            this.pnMap.Size = new System.Drawing.Size(105, 370);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.Turquoise;
            this.btnClear.Location = new System.Drawing.Point(3, 221);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(99, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.Turquoise;
            this.btnSetting.Location = new System.Drawing.Point(3, 190);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(99, 25);
            this.btnSetting.TabIndex = 2;
            this.btnSetting.Text = "Setting";
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Turquoise;
            this.btnStart.Location = new System.Drawing.Point(3, 159);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(99, 25);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Turquoise;
            this.btnStop.Location = new System.Drawing.Point(3, 128);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(99, 25);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnEclipse
            // 
            this.btnEclipse.BackColor = System.Drawing.Color.Turquoise;
            this.btnEclipse.Location = new System.Drawing.Point(3, 97);
            this.btnEclipse.Name = "btnEclipse";
            this.btnEclipse.Size = new System.Drawing.Size(99, 25);
            this.btnEclipse.TabIndex = 2;
            this.btnEclipse.Text = "Eclipse";
            this.btnEclipse.Click += new System.EventHandler(this.btnEclipse_Click);
            // 
            // btnRectangle
            // 
            this.btnRectangle.BackColor = System.Drawing.Color.Turquoise;
            this.btnRectangle.Location = new System.Drawing.Point(3, 66);
            this.btnRectangle.Name = "btnRectangle";
            this.btnRectangle.Size = new System.Drawing.Size(99, 25);
            this.btnRectangle.TabIndex = 2;
            this.btnRectangle.Text = "Rectangle";
            this.btnRectangle.Click += new System.EventHandler(this.btnRectangle_Click);
            // 
            // chbRaw
            // 
            this.chbRaw.Location = new System.Drawing.Point(3, 40);
            this.chbRaw.Name = "chbRaw";
            this.chbRaw.Size = new System.Drawing.Size(82, 20);
            this.chbRaw.TabIndex = 2;
            this.chbRaw.Text = "Raw";
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(504, 387);
            this.Controls.Add(this.pnMap);
            this.Name = "Map";
            this.Text = "Map";
            this.pnMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelControl;
        private System.Windows.Forms.Panel pnMap;
        private System.Windows.Forms.CheckBox chbRaw;
        private System.Windows.Forms.Button btnRectangle;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnEclipse;
        private System.Windows.Forms.Button btnClear;
    }
}