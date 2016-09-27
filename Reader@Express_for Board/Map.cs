using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Reader_Express
{
    public partial class Map : Form
    {
        bool canPain, flag, flag1;
        Graphics g;
        public int[] flag2 = new int[1];
        public static int boxNum = 0, preBox = 0;
        public static List<int> arrbox = new List<int>();
        public int box;
        int x, y;
        public int position;
        int width, height;
        private const int DISROW = 120;
        private const int DISCOL = 120;
        private const int START = 120;
        private static int roomWidth, roomHeight, roomDeep, ratio;
        static System.Windows.Forms.Timer updateMap = new System.Windows.Forms.Timer();
        Pen redPen = new Pen(Color.YellowGreen, 2);

        private PictureBox pictureBox1 = new PictureBox();
        
        public Map()
        {
            InitializeComponent();
            
            //width = panel1.Width;
            //height = panel1.Height;
            canPain = false;
            flag = false;
            flag1 = false;
            updateMap.Tick += new EventHandler(timerEventProcessor);
            updateMap.Interval = 5000;
        }

        private void timerEventProcessor(object obj, EventArgs args)
        {
            updateMap.Enabled = false;
            if (boxNum != 0)
            {
                drawBox(boxNum);
            }
            else drawBox(-1);
            updateMap.Enabled = true;
        }

        public void getData(string st1, string st2, string st3, string st4)
        {
            if ((st3 == "") && (st4 == ""))
            {
                roomWidth = int.Parse(st1);
                roomHeight = int.Parse(st2);
                //ratio = int.Parse(st4);
            }
            else
            {
                roomWidth = int.Parse(st1);
                roomHeight = int.Parse(st2);
                roomDeep = int.Parse(st3);
                ratio = int.Parse(st4);
            }
        }
        private void drawBox(int i)
        {
            if (i == -1) MessageBox.Show("Error:Not found tag");
            else
            {
                position =i;
               
                panel1.Paint += new PaintEventHandler(panel1_init);
                panel1.Refresh();
            }
        }
        public void getBox(int box)
        {
            if (box != 0)
            {
                boxNum = box;
                if (boxNum != preBox)
                {
                    arrbox.Add(box);
                }
                preBox = boxNum;
            }
            else boxNum = 0;
        }
        //private Panel panel1 = new Panel();
        //g = panelMap.CreateGraphics();
        //private bool btnRectangleClicked = false;
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            //btnRectangleClicked = true;
            
            
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Refresh();
            //if (flag1)
            //{

            //    panel1.Refresh();
            //    for (int i = 0; i <= 360; i = i + roomWidth)
            //    {
            //        g.DrawLine(redPen, i, 0, i, 360);
            //    }
            //    for (int i = 0; i <= 360; i = i + roomHeight)
            //    {
            //        g.DrawLine(redPen, 0, i, 360, i);
            //    }
            //    /*Font drawFont = new Font("Arial", 16);
            //    SolidBrush drawBrush = new SolidBrush(Color.Black);
            //     Create point for upper-left corner of drawing.
            //    PointF drawPoint = new PointF(10, 10);
            //    g.DrawString("1", drawFont, drawBrush, drawPoint);*/
            //    flag1 = false;
            //}
            //else
            //{
            //    MessageBox.Show("Please, run setting", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
            //}
              //MessageBox.Show(box.ToString());
        }

        private void btnEclipse_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i <= 360; i = i + roomWidth)
                {
                    g.DrawLine(redPen, i, 0, i, 360);
                }
                for (int i = 0; i <=360; i = i + roomHeight)
                {
                    g.DrawLine(redPen, 0, i, 360, i);
                }
                for (int i = 0; i < arrbox.Count; i++)
                {
                    drawBox(arrbox[i]);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            updateMap.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            MainForm main = new MainForm();
           
            updateMap.Enabled = true;
            main.ProcessorMap();

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            RoomParameter romParamaeter = new RoomParameter();
            romParamaeter.ShowDialog();
            //updateMap.Enabled = true;
            flag1 = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
           
            for (int i = 0; i <= 360; i = i + roomWidth)
            {
                g.DrawLine(redPen, i, 0, i,360);
            }
            for (int i = 0; i <= 360; i = i + roomHeight)
            {
                g.DrawLine(redPen, 0, i, 360, i);
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            
                for (int i = 0; i <= 360; i = i + roomWidth)
                {
                    g.DrawLine(redPen, i, 0, i, 360);
                }
                for (int i = 0; i <= 360; i = i + roomHeight)
                {
                    g.DrawLine(redPen, 0, i, 360, i);
                }
                
            
        }
        
        private void panel1_init(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
           string path = @"/Storage Card/Debug/nuclear1.jpg";
           //string path = Directory.GetCurrentDirectory() + @"/nuclear1.jpg";
           Bitmap bmp = new Bitmap(path);
           Image img = (Image)bmp;
            //Item item = new Item(((9 - position) % 3) * START, ((position - 1) / 3) * START, img);
            //item.drawImageItem(g);
            g.DrawImage(img, (( position-1) % 3) * START, ((9-position) / 3) * START);
        }
    }
}