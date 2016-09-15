using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Reader_Express
{
    class Item
    {
        int x, y;
        Image img;
        public Item(int x1, int y1, Image img)
        {
            x1 = x;
            y1 = y;
            this.img = img;
        }
        public int getX
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public int getWidth()
        {
                return img.Width;
        }
        public int getHeight()
        {
            return img.Height;
        }
        public int getY
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        public void drawImageItem(Graphics g)
        {
            //Point imgPoint = new Point(x, y);
            g.DrawImage(this.img,x,y);
        }
        public void deleteItem()
        {
            
        }
    }
}
