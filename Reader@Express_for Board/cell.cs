using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reader_Express
{
    public partial class cell : Form
    {
        public delegate void sendCell(string cell);
        sendCell SendCell;
        public cell()
        {
            InitializeComponent();
        }
        private void btnCell_Click(object sender, EventArgs e)
        {
            MainForm mainF = new MainForm();
            string cell;
            cell = cbCell.Text;
            SendCell = new sendCell(mainF.getCell);
            SendCell(cell);
            this.Hide();
        }
    }
}