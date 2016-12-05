using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace Reader_Express
{
    
    public partial class Map : Form
    {
        
        Graphics g;
        public int[] flag2 = new int[1];
        public static int boxNum = 0, preBox = 0;
        
        //public static List<int> arrbox = new List<int>();

        
        public int box;
        int x, y;
        public int position;
        int te = 3;
        int width, height;
        private const int DISROW = 120;
        private const int DISCOL = 120;
        private const int START = 120;
        private static int roomWidth, roomHeight, roomDeep, ratio;
        static System.Windows.Forms.Timer updateMap = new System.Windows.Forms.Timer();
        Pen redPen = new Pen(Color.YellowGreen, 2);
        public bool start = false;
        private PictureBox pictureBox1 = new PictureBox();
        
        public Map()
        {
            InitializeComponent();
            
            //width = panel1.Width;
            //height = panel1.Height;
            //canPain = false;
            //flag = false;
            //flag1 = false;
            updateMap.Tick += new EventHandler(timerEventProcessor);
            updateMap.Interval = 10000;
            
        }

        private void timerEventProcessor(object obj, EventArgs args)
        {     
            while (true)
            {
                updateMap.Enabled = false;
                int Num = 0;
                //Num = (int)carryPo.Pop();
                if (Num != 0)
                {
                    drawBox(Num);
                }
                updateMap.Enabled = true;
            }  
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
            //if (i == -1) MessageBox.Show("Error:Not found tag");
            //else
            //{
                position =i;
               
                panel1.Paint += new PaintEventHandler(panel1_init);
                panel1.Refresh();
            //}
        }
        public void getBox(int box)
        {
            if (box != 0)
            {
                boxNum = box;
            }
        }
        
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            //btnRectangleClicked = true;
            
            
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Refresh();
            
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            updateMap.Enabled = false;
            
        }

        private int getTime()
        {
            int hour = DateTime.Now.Hour;
            int min = DateTime.Now.Minute;
            int sec = DateTime.Now.Second;
            return hour * 3600 + min * 60 + sec;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            while (true)
            {
                    drawBox(boxNum);
                    int time, time2;
                    time = getTime();
                    time2 = getTime();
                    while (time2 - time < 5)
                    {
                        Application.DoEvents();
                        time2 = getTime();
                    }
             }   
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            RoomParameter romParamaeter = new RoomParameter();
            romParamaeter.ShowDialog();
            //updateMap.Enabled = true;
            //flag1 = true;
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
           //string path = @"/Storage Card/Debug/nuclear1.jpg";
           string path = Directory.GetCurrentDirectory() + @"/nuclear1.jpg";
           Bitmap bmp = new Bitmap(path);
           Image img = (Image)bmp;
            //Item item = new Item(((9 - position) % 3) * START, ((position - 1) / 3) * START, img);
            //item.drawImageItem(g);
            g.DrawImage(img, ((9 - position) % 3) * START, ((9-position) / 3) * START);
        }
        private void statusBar1_ParentChanged(object sender, EventArgs e)
        {

        }
    }
}