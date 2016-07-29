using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Reader_Express
{   
    class Lut
    {
        private int cell;
        private double rssi1, rssi2, rssi3,rssi4;
        public Lut()
        {
        }
        public Lut(int cell, double rssi1, double rssi2, double rssi3, double rssi4)
        {
            this.cell = cell;
            this.rssi1 = rssi1;
            this.rssi2 = rssi2;
            this.rssi3 = rssi3;
            this.rssi4 = rssi4;
        }
        public void setCell(int cell)
        {
            this.cell = cell;
        }
        public void setRssi1(double rssi1)
        {
            this.rssi1 = rssi1;
        }
        public void setRssi2(double rssi2)
        {
            this.rssi2 = rssi2;
        }
        public void setRssi3(double rssi3)
        {
            this.rssi3 = rssi3;
        }
        public void setRssi4(double rssi4)
        {
            this.rssi4 = rssi4;
        }
        
        public int getCell()
        {
            return cell;
        }
        public double getRssi1()
        {
            return rssi1;
        }
        public double getRssi2()
        {
            return rssi2;
        }
        public double getRssi3()
        {
            return rssi3;
        }
        public double getRssi4()
        {
            return rssi4;
        }

    }
}
