using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reader_Express
{
    public partial class RoomParameter : Form
    {
        public RoomParameter()
        {
            InitializeComponent();
        }
        public delegate void sendData(string st1, string st2, string st3, string st4);
        sendData SendData;

        private void btnRoomOK_Click(object sender, EventArgs e)
        {
            Map map = new Map();
            string st1_, st2_, st3_, st4_;
            st1_ = cbRoomWidth.Text;
            st2_ = cbRoomHeight.Text;
            st3_ = cbRoomDepth.Text;
            st4_ = cbRoomRatio.Text;
            SendData = new sendData(map.getData);
            if ((chb2D.Checked) && (!chb3D.Checked))
            {
                SendData(st1_, st2_, "", "");
                this.Close();
            }
            else if ((!chb2D.Checked) && (chb3D.Checked))
            {
                SendData(st1_, st2_, st3_, st4_);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please, choice 2D or 3D", "Warning", MessageBoxButtons.RetryCancel,MessageBoxIcon.Asterisk,MessageBoxDefaultButton.Button1);
            }
        }

    }
}