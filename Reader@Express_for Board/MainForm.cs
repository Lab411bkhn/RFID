using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using idro.reader.api;

using System.IO;
using System.Xml;
using System.IO.Ports;
using idro.controls;
using idro.profile;
using idro.controls.panel;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace Reader_Express
{
    public partial class MainForm : Form
    {
        const int MAX_CONNECTION = 10;
        const int BUFFER_SIZE = 1024;
        TcpClient client;
        Stream stream = Stream.Null;
        Queue dataSockets = new Queue();
        Queue sendSockets = new Queue();
        Queue dataSerial = new Queue();
        SerialPort ComPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
        public Thread threadProcess = null;
        public Thread receiveSoc = null;
        public Thread threadSendToSocket = null;
        public Thread threadDelay = null;

        private static uint totalReadCounts1 = 0;
        private static uint totalReadCounts2 = 0;
        private static uint totalReadCounts3 = 0;
        private static uint totalReadCounts4 = 0;
         
        private uint totalRead;

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        
        

        private string num5, text;
        public int box = 0;
        public string MaThe = "0001";
        private float[,] RssiTrain = new float[9, 4];
        private int[] arrRssi = new int[20];
        static System.Windows.Forms.Timer traningTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer inventoryTimer = new System.Windows.Forms.Timer();
        
        private static int timeDelay = 20000;
        //private static int inventoryTimeDelay = 11000;
        private static int timeDelay_ = 6000;
        private static ArrayList arrRssi1 = new ArrayList();
        private static ArrayList arrRssi2 = new ArrayList();
        private static ArrayList arrRssi3 = new ArrayList();
        private static ArrayList arrRssi4 = new ArrayList();
        ArrayList tagId = new ArrayList();
        ArrayList tagId1 = new ArrayList();
        List<String> t = new List<string>();
        CipherAlgorithm cipher = new CipherAlgorithm();
        string[] stt = new string[2];
        // private static List<Dictionary<String, Double>> dataTraining = new List<Dictionary<string, double>>();
        private static List<Lut> dataTraining = new List<Lut>();
        private static List<Lut> dataRssi = new List<Lut>();
        DateTime time;
        private bool trainingFlag = false;
        long tick;
        int count = 0;
        
       
        private static Reader reader = null;
        public delegate void sendBox(int box);
        DatabaseXml data = new DatabaseXml();
        
        #region P/Invoke...
        [DllImport("User32.dll")]
        static extern Boolean MessageBeep(UInt32 beepType);
        #endregion
        public MainForm()
        {
            InitializeComponent();
            cbModelType.SelectedIndex = 2;
            cbConnectType.SelectedIndex = 0;
            cbTagType.SelectedIndex = 0;
            time = DateTime.Now;
            tick = time.Ticks;
            reader = new Reader();
            //myTimer.Tick += new EventHandler(ProcessMap);
            //myTimer.Interval = 1000;
            //tsFunctions.Visible = true;
            //traningTimer.Tick += new EventHandler(trainingTimerEventProcessor);
            //traningTimer.Interval = timeDelay;
            //inventoryTimer.Tick += new EventHandler(inventoryTimerEventProcessor);
            //inventoryTimer.Interval = inventoryTimeDelay;
        }
        private void timerEventProcessor(object obj, EventArgs args, String p1, int pw)
        {
            setPortPower(p1, pw);
        }

        public void Delay(Object obj, EventArgs arg)
        {
            myTimer.Enabled = false;

        }
        public void DisplayData(TextBox _tb, string msg)
        {
            tbShow.Invoke(new EventHandler(delegate
            {
                _tb.Text += msg + "\r\n";
                _tb.SelectionStart = tbShow.Text.Length;
                _tb.ScrollToCaret();
            }));
        }
        public void ReplaceText(TextBox _tb, string msg)
        {
            tbShow.Invoke(new EventHandler(delegate
            {
                _tb.Text = msg;
            }));
        }
        private static void OpenConnection()
        {
            try
            {
                //conn.Open();
                MessageBox.Show("Mo ket noi co so du lieu nhung chua viet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private static bool CloseConnection()
        {
            try
            {
                //conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static int findMin(List<Double> list)
        {
            double min;
            int box = 0;
            min = list[0];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                {
                    min = list[i];
                    box = i;
                }
            }
            return box;
        }

        public static double averageRssi(ArrayList arr)
        {
            double result = 0;
            foreach (int item in arr)
            {
                result += item;
            }
            return (double)(result / 5);
        }

        //private static ConnectType conType;
        public ConnectType ConnectType
        {
            get
            {
                ConnectType connectType = (cbConnectType.SelectedIndex == 0) ? ConnectType.Tcp : ConnectType.Serial;
                //MessageBox.Show("get.COnType:" + connectType.ToString());
                return connectType;
            }
            //set { conType = value; }
        }
        public ModelType ModelType
        {
            get
            {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), cbModelType.Text, false);
                //MessageBox.Show(cbModelType.SelectedItem.ToString());
                return modelType;
            }
        }
        public TagType TagType
        {
            get
            {
                TagType tagType = (TagType)Enum.Parse(typeof(TagType), cbTagType.Text, false);
                return tagType;
            }
        }
        public void getCell(string cell)
        {
            double r1 = 0, r2 = 0, r3 = 0, r4 = 0;

            foreach (int item in arrRssi1)
            {
                r1 += item;
            }
            // MessageBox.Show("Rssi1: " + ((double)(r1 / arrRssi1.Count)).ToString());
            foreach (int item in arrRssi2)
            {
                r2 += item;
            }
            //MessageBox.Show("Rssi2: " + ((double)(r2 / arrRssi2.Count)).ToString());
            foreach (int item in arrRssi3)
            {
                r3 += item;
            }
            // MessageBox.Show("Rssi3: " + ((double)(r3 / arrRssi3.Count)).ToString());
            foreach (int item in arrRssi4)
            {
                r4 += item;
            }
            //MessageBox.Show("Rssi4: " + ((double)(r4 / arrRssi4.Count)).ToString());
            //cmd = new MySqlCommand("INSERT INTO rfidreader.training(cell ,rssi1,rssi2, rssi3, rssi4) VALUE('" + cell + "', '" + ((double)(r1 / arrRssi1.Count)).ToString() + "','" + ((double)(r2 / arrRssi2.Count)).ToString() + "',  '" + ((double)(r3 / arrRssi3.Count)).ToString() + "', '" + ((double)(r4 / arrRssi4.Count)).ToString() + "')", conn);
            //MySqlDataReader myReader;
            try
            {
                DatabaseXml trainingData = new DatabaseXml();
                int total = trainingData.GetTotalTraining();
                for (int i = int.Parse(cell); i <= total; i++)
                {
                    trainingData.DeleteNodeTraining(i);
                }
                //rssi1_ = averageRssi(arrRssi1);
                //rssi2_ = averageRssi(arrRssi2);
                //rssi3_ = averageRssi(arrRssi3);
                //rssi4_ = averageRssi(arrRssi4);

                trainingData.creatCard(int.Parse(cell), (double)(r1 / arrRssi1.Count), (double)(r2 / arrRssi2.Count), (double)(r3 / arrRssi3.Count), (double)(r4 / arrRssi4.Count));
            }
            catch
            {

            }
        }
        private void OnReaderEvent2(string dataRead)
        {
            //Received data are in the byte array (e.payload), it decodes into string using 
            //following function.
            //string szPayload = Encoding.ASCII.GetString(e.Payload, 0, e.Payload.Length);
            // MessageBox.Show(szPayload);
            //Many responses can be contained in the generated event and it separates first.
            //Received data may contain tag ID, set value, response code, etc. according to call 
            //function, byte array(e.payload) may contain more than 1 response therefore it
            //separates using following separator.

            string szResponse = dataRead.Substring(1);


            // DisplayData(tbShow, szResponse);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            // Response Code : [#]C##
            // Tag Memory : [#]T3000111122223333444455556666[##]
            // RSSI: [#]RFD##
            // Settings Values : p0, c1, ...
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            string szValue;
            bool bCheckSum = true;//reader.IsFixedType();
            bool bMultiPort = true; 
            reader.IsMultiPort();
            int nPos = bMultiPort ? 1 : 0;
            int flag1 = 0;
            int rssi1 = 0, rssi2 = 0, rssi3 = 0, rssi4 = 0, rssi = 0;
           

            Dictionary<string, int> dRss1 = new Dictionary<string, int>();
            Dictionary<string, int> dRss2 = new Dictionary<string, int>();
            Dictionary<string, int> dRss3 = new Dictionary<string, int>();
            Dictionary<string, int> dRss4 = new Dictionary<string, int>();
            // lbxResponses.Items.Insert(0, szPayload);
            string szTxt = null;
            //////////////////////////////////////////////////////
            try
            {

                totalRead += 1;
                //  code = 'R';
                switch (szResponse[nPos])
                {
                    case 'T':
                        {
                            //DisplayData(tbShow, szResponse);
                            szValue = szResponse.Substring(nPos + 1, szResponse.Length - (nPos + 1 + (bCheckSum ? 2 : 0)));//exclude [#]T/C, CheckSum
                            num5 = szValue;
                            if (szValue.Length > 4)
                            {
                                string hex = szValue.Substring(4, szValue.Length - 4);
                                szTxt = Reader.MakeTextFromHex(hex).ToString();
                                text = szTxt;

                            }
                            switch (szResponse[0])
                            {
                                case '1':
                                    ReplaceText(tbTagId, szValue);
                                    ReplaceText(tbPort, szResponse[0].ToString());
                                    ReplaceText(tbText, szTxt);
                                    foreach (string value in tagId)
                                    {
                                        if (value.CompareTo(szValue) == 0)
                                        {
                                            flag1 = 1;
                                            break;
                                        }
                                    }
                                    if (flag1 == 0)
                                    {
                                        tagId.Add(szValue);
                                    }
                                    break;
                                case '2':
                                    ReplaceText(tbTagId1, szValue);
                                    ReplaceText(tbPort1, szResponse[0].ToString());
                                    ReplaceText(tbText1, szTxt);
                                    break;
                                case '3':
                                    ReplaceText(tbTagId2, szValue);
                                    ReplaceText(tbPort2, szResponse[0].ToString());
                                    ReplaceText(tbText2, szTxt);
                                    break;
                                case '4':
                                    ReplaceText(tbTagId3, szValue);
                                    ReplaceText(tbPort3, szResponse[0].ToString());
                                    ReplaceText(tbText3, szTxt);
                                    break;
                                default:
                                    break;
                            }
                            DisplayData(tbShow, "Tag ID: " + szValue);
                            DisplayData(tbShow, "Port: " + szResponse[0]);
                            DisplayData(tbShow, "Text: " + szTxt);
                        }
                        break;
                    case 'C':
                        {
                            //DisplayData(tbShow, "C:" + szResponse);
                            szValue = szResponse.Substring(nPos + 1, szResponse.Length - (nPos + 1));//exclude [#]T/C
                            szValue = szValue + "-" + reader.Responses(szValue);
                            //if HEARTBEAT response, fire heartbeat timer again.
                            if (string.Compare(szValue, "FF", StringComparison.Ordinal) == 0)
                            {
                                setTimer(9000, "Heartbeat");
                            }
                            DisplayData(tbShow, "Data" + szValue);
                        }
                        break;
                    case 'R':
                        {
                            //DisplayData(tbShow, "R:" + szResponse);
                            szValue = szResponse.Substring(szResponse.Length - 2, 2);
                            rssi = sdk.ToInt32(szValue, 0x10);

                            double a = sdk.distanceFromRssi(rssi);

                            DisplayData(tbShow, "Rssi " + szResponse[0] + ":" + " -" + rssi);
                            switch (szResponse[0])
                            {
                                case '1':
                                    {
                                        rssi1 = rssi;
                                        ReplaceText(tbRssi, "-" + rssi.ToString());
                                        arrRssi1.Add(rssi);
                                        totalReadCounts1++;
                                        ReplaceText(tbRead, totalReadCounts1.ToString());
                                    }
                                    break;
                                case '2':
                                    {
                                        rssi2 = rssi;
                                        ReplaceText(tbRssi1, "-" + rssi.ToString());
                                        arrRssi2.Add(rssi);
                                        totalReadCounts2++;
                                        ReplaceText(tbRead1, totalReadCounts2.ToString());
                                    }
                                    break;
                                case '3':
                                    {
                                        ReplaceText(tbRssi2, "-" + rssi.ToString());
                                        rssi3 = rssi;
                                        arrRssi3.Add(rssi);
                                        totalReadCounts3++;
                                        ReplaceText(tbRead2, totalReadCounts3.ToString());
                                    }
                                    break;
                                case '4':
                                    {
                                        rssi4 = rssi;
                                        ReplaceText(tbRssi3, "-" + rssi.ToString());
                                        arrRssi4.Add(rssi);
                                        totalReadCounts4++;
                                        ReplaceText(tbRead3, totalReadCounts4.ToString());
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (trainingFlag == false)
                            {

                                //string query = "INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2, Rssi3, Rssi4, Text, TotalRead1, TotalRead2, TotalRead3, TotalRead4,Position) VALUE('" + num5 + "', '" + rssi1.ToString() + "','" + rssi2.ToString() + "',  '" + rssi3.ToString() + "', '" + rssi4.ToString() + "','" + text + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "', '" + "" + "')";
                                //cmd = new MySqlCommand(query, conn);
                                //MySqlDataReader myReader;
                                //try
                                //{
                                //    OpenConnection();
                                //    myReader = cmd.ExecuteReader();
                                //    CloseConnection();
                                //}
                                //catch (MySqlException ex)
                                //{
                                //    MessageBox.Show(ex.Message);
                                //    CloseConnection();
                                //}
                                rssi1 = rssi2 = rssi3 = rssi4 = 0;
                                totalRead = 0;
                            }
                        }
                        break;

                    default:
                        DisplayData(tbShow, "Cannot convert data: " + szResponse);
                        break;
                }
            }
            catch (Exception ex) { DisplayData(tbShow, ex.ToString()); }
        }



        private static void trainingTimerEventProcessor(Object obj, EventArgs args)
        {
            reader.StopOperation();
            traningTimer.Enabled = false;

            if (totalReadCounts2 >= 20 && totalReadCounts1 >= 20 && totalReadCounts3 >= 20 && totalReadCounts4 >= 20)
            {
                //for (int i = 0; i < 20; i++)
                //{
                //    cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                //    MySqlDataReader myReader;
                //    try
                //    {
                //        OpenConnection();
                //        myReader = cmd.ExecuteReader();
                //        CloseConnection();
                //    }
                //    catch (MySqlException ex)
                //    {
                //        MessageBox.Show(ex.Message);
                //        CloseConnection();
                //    }
                //}
                //traningTimer.Stop();
                MessageBox.Show("ket noi database1");
            }
            else
            {
                timeDelay += timeDelay_;
                if (timeDelay > 6000)
                {
                    if (totalReadCounts1 >= 20 && totalReadCounts2 >= 20 && totalReadCounts3 >= 20 && totalReadCounts4 < 20)
                    {
                        //for (int i = 0; i < 20; i++)
                        //{
                        //    cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                        //    MySqlDataReader myReader;
                        //    try
                        //    {
                        //        OpenConnection();
                        //        myReader = cmd.ExecuteReader();
                        //        CloseConnection();
                        //    }
                        //    catch (MySqlException ex)
                        //    {
                        //        MessageBox.Show(ex.Message);
                        //        CloseConnection();
                        //    }
                        //}
                        MessageBox.Show("ket noi database2");
                    }
                    if (totalReadCounts1 >= 20 && totalReadCounts2 >= 20 && totalReadCounts4 >= 20 && totalReadCounts3 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database3");
                        }
                    }
                    if (totalReadCounts1 >= 20 && totalReadCounts3 >= 20 && totalReadCounts4 >= 20 && totalReadCounts2 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database4");
                        }
                    }
                    if (totalReadCounts2 >= 20 && totalReadCounts3 >= 20 && totalReadCounts4 >= 20 && totalReadCounts1 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database5");
                        }
                    }
                    if (totalReadCounts1 >= 20 && totalReadCounts2 >= 20 && totalReadCounts3 < 20 && totalReadCounts4 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database6");
                        }
                    }
                    if (totalReadCounts1 >= 20 && totalReadCounts4 >= 20 && totalReadCounts3 < 20 && totalReadCounts2 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database7");
                        }
                    }
                    if (totalReadCounts3 >= 20 && totalReadCounts4 >= 20 && totalReadCounts1 < 20 && totalReadCounts2 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database8");
                        }
                    }
                    if (totalReadCounts1 >= 20 && totalReadCounts3 >= 20 && totalReadCounts4 < 20 && totalReadCounts2 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database9");
                        }
                    }
                    if (totalReadCounts4 >= 20 && totalReadCounts2 >= 20 && totalReadCounts1 < 20 && totalReadCounts3 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database10");
                        }
                    }
                    if (totalReadCounts2 >= 20 && totalReadCounts3 >= 20 && totalReadCounts1 < 20 && totalReadCounts4 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database11");
                        }
                    }

                    if (totalReadCounts1 >= 20 && totalReadCounts2 < 20 && totalReadCounts3 < 20 && totalReadCounts4 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database12");
                        }
                    }
                    if (totalReadCounts2 >= 20 && totalReadCounts1 < 20 && totalReadCounts3 < 20 && totalReadCounts4 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database13");
                        }
                    }
                    if (totalReadCounts3 >= 20 && totalReadCounts1 < 20 && totalReadCounts2 < 20 && totalReadCounts4 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (MySqlException ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database14");
                        }
                    }

                    if (totalReadCounts4 >= 20 && totalReadCounts1 < 20 && totalReadCounts3 < 20 && totalReadCounts2 < 20)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            //cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts1.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //MySqlDataReader myReader;
                            //try
                            //{
                            //    OpenConnection();
                            //    myReader = cmd.ExecuteReader();
                            //    CloseConnection();
                            //}
                            //catch (Exception ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //    CloseConnection();
                            //}
                            MessageBox.Show("ket noi database15");
                        }
                    }
                }
                else
                {
                    traningTimer.Interval = timeDelay_;
                    traningTimer.Enabled = false;
                    reader.InventoryMultiple();

                }
            }
        }
       
        public void setPortPower(string port, int PowerLevel)
        {
               
            string dataSoc = ">x " + port + " " + PowerLevel.ToString();
            sendSockets.Enqueue(dataSoc);
            //Thread.Sleep(300);
        }

        public int locate(int x1, int x2, int x3, int x4)
        {
            int posi;
            if ((x1 > 300) && (x2 > 300) && (x3 > 300) && (x4 > 300))
                posi = 0;
            else
                if ((x1 < 180)) posi = 1;
                else
                    if ((x2 < 180)) posi = 7;
                    else
                        if ((x3 < 180)) posi = 9;
                        else
                            if ((x4 < 180)) posi = 3;
                            else
                                if ((x3 < x1) && (x3 < x4) && (x2 <= x1) && (x2 < x4)) posi = 8;
                                else
                                    if ((x4 < x3) && (x4 < x2) && (x1 < x3) && (x4 <x2)) posi = 2;
                                    else
                                        if ((x4 < x2) && (x3 < x2) && (x4 <= x1) && (x3 <= x1)) posi = 6;
                                        else
                                            if ((x1 < x3) && (x1 < x4) && (x2 < x3) && (x2 < x4)) posi = 4;
                                            else
                                                posi = 5;
            return posi;
        }

        private int getTime()
        {
        int hour = DateTime.Now.Hour;
        int min = DateTime.Now.Minute;
        int sec = DateTime.Now.Second;
        return hour * 3600 + min * 60 + sec;
        }

        public void ProcessorMap()
        {
            while (true)
            {
                try
                {
                    //SendStop();
                    //threadProcess.Abort();
                    int pw1 = 160, pw2 = 160, pw3 = 160, pw4 = 160;

                    bool loading = true;

                    int time, time2;
                    while (loading == true)
                    {
                        stbConnect.Text = "Loading";
                        setPortPower("p1", 160);
                        setPortPower("p2", 160);
                        setPortPower("p3", 160);
                        setPortPower("p4", 160);
                        time = getTime();
                        time2 = getTime();
                        while (time2 - time < 1)
                        {
                            Application.DoEvents();
                            time2 = getTime();
                        }
                        totalReadCounts1 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
                        loading = false;
                    }
                    if (loading == false) stbConnect.Text = "Connected";
                    bool check1 = false, check2 = false, check3 = false, check4 = false;
                    totalReadCounts1 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;

                    sendSockets.Clear();
                    while ((check1 == false) || (check2 == false) || (check3 == false) || (check4 == false))
                    {
                        if ((pw1 <= 300))
                        {
                            if (totalReadCounts1 == 0)
                            {
                                pw1 = pw1 + 5;
                            }

                            else { check1 = true; };
                        }
                        else check1 = true;
                        if ((pw2 <= 300))
                        {
                            if (totalReadCounts2 == 0)
                            {
                                pw2 = pw2 + 5;
                            }

                            else { check2 = true; };
                        }
                        else check2 = true;
                        if ((pw3 <= 300))
                        {
                            if (totalReadCounts3 == 0)
                            {
                                pw3 = pw3 + 5;
                            }

                            else { check3 = true; };
                        }
                        else check3 = true;

                        if ((pw4 <= 300))
                        {
                            if (totalReadCounts4 == 0)
                            {
                                pw4 = pw4 + 5;
                            }

                            else { check4 = true; };
                        }
                        else check4 = true;

                        setPortPower("p1", pw1);
                        setPortPower("p2", pw2);
                        setPortPower("p3", pw3);
                        setPortPower("p4", pw4);

                        time = getTime();
                        time2 = getTime();
                        while (time2 - time < 1)
                        {
                            Application.DoEvents();
                            time2 = getTime();
                        }
                        txbPw1.Text = pw1.ToString();
                        txbPw2.Text = pw2.ToString();
                        txbPw3.Text = pw3.ToString();
                        txbPw4.Text = pw4.ToString();
                    }
                    box = locate(pw1, pw2, pw3, pw4);
                    MessageBox.Show(box.ToString());
                    Map map = new Map();
                    //map.getBox(box);
                    sendBox SendBox = new sendBox(map.getBox);
                    SendBox(box);
                    DatabaseXml history = new DatabaseXml();
                    int totalHis = history.GetTotalHistory();
                    int currentPos = int.Parse(history.getPosition(totalHis));
                    //MessageBox.Show(currentPos.ToString());
                    if (currentPos != box)
                    {
                        string date = DateTime.Now.ToString();
                        history.CreatHistory(totalHis + 1, box, date);
                        //WebForm web = new WebForm();
                        //web.ProcessDataSend(box, "0001");

                    }
                    totalReadCounts1 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
                    arrRssi1.Clear();
                    arrRssi1.Clear();
                    arrRssi3.Clear();
                    arrRssi4.Clear();

                    time = getTime();
                    time2 = getTime();
                    while (time2 - time < 4)
                    {
                        Application.DoEvents();
                        time2 = getTime();
                    }
                    //SendInventory();
                }
                catch { };
            } 
        }
      

        #region Timer Reconnect...
        System.Windows.Forms.Timer timer = null;
        private string timerMessage = string.Empty;
        public void setTimer(int nInterval, string nMessage)
        {
            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Tick += new System.EventHandler(TimerHandler);
            }
            timer.Interval = nInterval;
            timerMessage = nMessage;
            timer.Enabled = true;
        }
        public void StopTimer()
        {
            if (timer != null)
                timer.Enabled = false;
        }
        private void TimerHandler(object sender, EventArgs e)
        {
            timer.Enabled = false;
            //OnReaderEvent(this, new ReaderEventArgs(EventType.Timer, timerMessage));
        }
        public bool IsTiming
        {
            get { return timer == null ? false : timer.Enabled; }
        }

        private void Delete()
        {
            //string query = "DELETE FROM rfidreader.rfid";
            //cmd = new MySqlCommand(query, conn);
            //MySqlDataReader MyReader;
            //try
            //{
            //    OpenConnection();
            //    MyReader = cmd.ExecuteReader();
            //    MessageBox.Show("Delete all Database", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //    CloseConnection();
            //}
            //catch (MySqlException ex)
            //{
            MessageBox.Show("Xoa database nhung chua viet");
            //    CloseConnection();
            //}
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (reader != null)
            {
                if (reader.IsHandling)
                {
                    MessageBeep(0);
                    e.Cancel = true;
                    reader.Close(CloseType.FormClose);
                    return;
                }
            }
            base.OnClosing(e);
        }
        #endregion

        #region Socket...

        private void sendData()
        {
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            while (true)
            {
                if (sendSockets.Count > 0)
                {
                    string str = sendSockets.Dequeue().ToString();
                    writer.WriteLine(str);
                }
                //Thread.Sleep(500);
            }
        }
        public void ThreadSocket()
        {
            receiveSoc = new Thread(new ThreadStart(receiveSocket));
            receiveSoc.IsBackground = true;
            receiveSoc.Start();
        }
        void receiveSocket()
        {
            try
            {
                threadSendToSocket = new Thread(new ThreadStart(sendData));
                threadSendToSocket.IsBackground = true;
                threadSendToSocket.Start();
                stream = client.GetStream();
                var reader = new StreamReader(stream);
                while (true)
                {
                    try
                    {
                        string str = reader.ReadLine();
                        dataSockets.Enqueue(str);
                        //if (btnConnect.Text == "Connect") break;
                        //Thread.Sleep(150);
                        
                        
                    }
                    catch { }
                }
            }
            catch { }
            // 4. close

            client.Close();

        }

        public void ThreadProcessDataSocket()
        {
            threadProcess = new Thread(new ThreadStart(ProcessDataSocket));
            threadProcess.IsBackground = true;
            threadProcess.Start();
        }


        public void ProcessDataSocket()
        {
            while (true)
            {
                try
                {
                    
                    string sdj = dataSockets.Dequeue().ToString();
                    //DisplayData(tbShow, sdj);
                   OnReaderEvent2(sdj);
                    //Thread.Sleep(450);
                }
                catch { }
            }

        }
        #endregion Socket
        # region Serial Port...

        public void ReceiveSerialData(object sender, SerialDataReceivedEventArgs e)
        {
            string dataIn = ComPort.ReadExisting();
            dataSerial.Enqueue(dataIn);

        }
        public void ProcessSerial()
        {
            while (true)
            {
                try
                {
                    string Serdata = dataSerial.Dequeue().ToString();
                    OnReaderEvent2(Serdata);
                }
                catch { }
            }
        }
        #endregion
        #region Process Button...
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cbConnectType.SelectedIndex == 0)
            {
                switch (btnConnect.Text)
                {
                    case "Connect":
                        {
                            try
                            {
                                // 1. connect
                                //client = new TcpClient();
                                client = new TcpClient();
                                client.Connect("192.168.0.190", 5578);
                                ThreadSocket();
                                ThreadProcessDataSocket();
                            //    receiveSocket();
                                stbConnect.Text = "Connected";
                                btnConnect.Text = "Disconnect";
                                
                                DisplayData(tbShow, "Connected to Y2Server.");

                            }
                            catch(Exception ex) {

                            }
                        } break;

                    case "Disconnect":
                        {
                            try
                            {
                                btnConnect.Text = "Connect";
                                stbConnect.Text = "Disconnected!";
                                threadSendToSocket.Abort();
                                receiveSoc.Abort();
                                threadProcess.Abort();

                            }
                            catch (Exception ex) { DisplayData(tbShow, "Cannot disconnect: " + ex.ToString()); }
                        } break;
                }
            }
            else
            {
                if (btnConnect.Text == "Connect")
                {
                    ComPort.Open();
                    btnConnect.Text = "Disconnnect";
                    stbConnect.Text = "Connected";
                    ComPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveSerialData);
                    ProcessSerial();

                }
                else
                {
                    ComPort.Close();
                    btnConnect.Text = "Connect";
                    stbConnect.Text = "Disconnected";
                }
            }
        }

        private void cbConnectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAddess.Items.Add("192.168.0.190");
            cbAddess.SelectedIndex = 0;
            bool bTcpIp = ConnectType == ConnectType.Tcp;
            //MessageBox.Show("connectype:"+ConnectType.ToString());
            lblTypeConnection.Text = bTcpIp ? "IP:" : "Port:";
            cbAddess.Visible = bTcpIp;
            cbSerial.Visible = !bTcpIp;
        }

        private void cbModelType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbTagType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        public void SendInventory()
        {
            sendSockets.Enqueue(">f");
            //ThreadProcessDataSocket();
        }

        private void btnInventoryMul_Click(object sender, EventArgs e)
        {
            //traningTimer.Enabled = false;
            //trainingFlag = false;
            
            //reader.InventoryMultiple();
            //inventoryTimer.Enabled = true;
            SendInventory();
        }

        private void btnInventorySingle_Click(object sender, EventArgs e)
        {
            //reader.InventorySingle();
            //DisplayData(tbShow, cbAddess.Text);
            SendInventory();
        }

        public void SendStop()
        {
            sendSockets.Enqueue(">3");
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            SendStop();
        }

        private void btnReadMem_Click(object sender, EventArgs e)
        {
            reader.ReadMemory(MemoryType.EPC, 1, 3);

        }

        private void btnLock_Click(object sender, EventArgs e)
        {
         
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbShow.Text = null;
        }

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            
            setPortPower("p1", 160);
           
            setPortPower("p2", 160);
            
            setPortPower("p3", 160);
            
            setPortPower("p4", 160);
            
            totalReadCounts1 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
        }

        private void btnDataStore_Click(object sender, EventArgs e)
        {
            reader.Command("o", false);//Get Stored data
        }

        private void btnClearDataBase_Click(object sender, EventArgs e)
        {
            DatabaseXml deleteData = new DatabaseXml();
            deleteData.DeleteAllrfid();
        }

        private void ThreadMap()
        {
            threadDelay = new Thread(new ThreadStart(ProcessorMap));
            threadDelay.IsBackground = true;
            threadDelay.Start();
        }
        private void btnMapping_Click(object sender, EventArgs e)
        {
            ThreadMap();
            Map map = new Map();
            map.ShowDialog();
                    
        }

        private void btnBluetooth_Click(object sender, EventArgs e)
        {
            //DatabaseXml xml = new DatabaseXml();

            //MessageBox.Show(xml.getRssi(1 , 2));
            //long endTick = time.Ticks;
            //MessageBox.Show((endTick - tick).ToString()); 
        }

        private void btnTrainningData_Click(object sender, EventArgs e)
        {
            //double rssi1_ = 0, rssi2_ = 0, rssi3_ = 0, rssi4_ = 0;
            cell c = new cell();
            c.ShowDialog();
        }

        private void btnCalculator_Click(object sender, EventArgs e)
        {
            int IDnow;
            double rssi1_ = 0, rssi2_ = 0, rssi3_ = 0, rssi4_ = 0;
            uint totalread1_ = 0, totalread2_ = 0, totalread3_ = 0, totalread4_ = 0;
            rssi1_ = averageRssi(arrRssi1);
            rssi2_ = averageRssi(arrRssi2);
            rssi3_ = averageRssi(arrRssi3);
            rssi4_ = averageRssi(arrRssi4);
            totalread1_ = totalReadCounts1;
            totalread2_ = totalReadCounts2;
            totalread3_ = totalReadCounts3;
            totalread4_ = totalReadCounts4;
            try
            {
                DatabaseXml data = new DatabaseXml();
                IDnow = data.GetTotalCard();
                data.InsertCard(IDnow + 1, rssi1_, rssi2_, rssi3_, rssi4_, totalread1_, totalread2_, totalread3_, totalread4_);
            }
            catch { }
        }

        private void btnLUT_Click(object sender, EventArgs e)
        {
            //cmd = new MySqlCommand("SELECT * FROM rfidreader.training", conn);
            //MySqlDataReader myReader;
            Lut lut;
            double rssi1 = 0, rssi2 = 0, rssi3 = 0;
            int cell = 0;
            try
            {
                MessageBox.Show("Lay du lieu tu data base nhung chua viet");
                //    OpenConnection();
                //myReader = cmd.ExecuteReader();
                //while (myReader.Read())
                //{
                //    lut = new Lut(myReader.GetInt16(1), myReader.GetDouble(2), myReader.GetDouble(3), myReader.GetDouble(4));
                //    dataTraining.Add(lut);
                //};
                //CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //    CloseConnection();
            }
            lut = new Lut();
            while (count <= dataTraining.Count - 8)
            {
                for (int j = 0; j < 8; j++)
                {
                    lut = dataTraining[j + count];
                    cell = lut.getCell();
                    rssi1 += lut.getRssi1();
                    rssi2 += lut.getRssi2();
                    rssi3 += lut.getRssi3();
                }
                lut.setCell(cell);
                lut.setRssi1(rssi1 / 8);
                lut.setRssi2(rssi2 / 8);
                lut.setRssi3(rssi3 / 8);
                dataRssi.Add(lut);
                rssi1 = rssi2 = rssi3 = 0;
                count += 8;
            }
        }

        private void btnKill_Click(object sender, EventArgs e)
        {


        }
        #endregion Process Button
        public void send()
        {
            WebForm web = new WebForm();
            //int box = 2;
            //web.ProcessDataSend(box, MaThe);
            web.ProcessDataSend(box, MaThe);
            DisplayData(tbShow, "Sent data");
            //Thread.Sleep(5000);

        }
        private void btnSendToWeb_Click(object sender, EventArgs e)
        {
            WebForm web = new WebForm();
            web.threadProcessWeb = new Thread(new ThreadStart(send));
            web.threadProcessWeb.IsBackground = true;
            web.threadProcessWeb.Start();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            setPortPower("p1", 270);
            
            //sendSockets.Enqueue(">x p1 " + tbxSetPower.Text);
        }
    }
}