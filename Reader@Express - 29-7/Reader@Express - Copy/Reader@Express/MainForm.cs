﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Timers;
using idro.reader.api;

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

using System.IO;
using System.IO.Ports;
using idro.controls;
using idro.profile;
using idro.controls.panel;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Reader_Express
{

    public partial class mainForm : Form
    {
        private static uint totalReadCounts1 = 0;
        private static uint totalReadCounts2 = 0;
        private static uint totalReadCounts3 = 0;
        private static uint totalReadCounts4 = 0;
        private uint totalRead;
        int pw1 = 50, pw2 = 50, pw3 = 50, pw4 = 50;
        int flag = 0, box = 0;
        List<int> dt1 = new List<int>();
        List<int> dt2 = new List<int>();
        List<int> dt3 = new List<int>();
        List<int> dt4 = new List<int>();
        int x1, x2, x3, x4;
        int a = 1;
        private string num5, text;
        private int[] arrRssi = new int[20];
        static System.Windows.Forms.Timer traningTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer inventoryTimer = new System.Windows.Forms.Timer();
        private static int timeDelay = 4000;
        private static int inventoryTimeDelay = 2200;
        private static int timeDelay_ = 1200;
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
        int local_1, local_2, local_3, local_4, local;
        private static Reader reader = null;
        public delegate void sendBox(int box);
        public mainForm()
        {
            time = DateTime.Now;
            tick = time.Ticks;
            InitializeComponent();
            reader = new Reader();
            tsFunctions.Visible = true;
            traningTimer.Tick += new EventHandler(trainingTimerEventProcessor);
            traningTimer.Interval = timeDelay;
            inventoryTimer.Tick += new EventHandler(inventoryTimerEventProcessor);
            inventoryTimer.Interval = inventoryTimeDelay;
        }
        // Init Server localhost, Table database is rssi, Port 3306, Id = root, pass=''
        static String connString = "Server=localhost;Database=rfidreader;Port=3306;User ID=root;Password=";
        // Create a connected database
        //MySqlConnection connect = new MySqlConnection(connString);
        private static MySqlConnection conn = new MySqlConnection(connString);
        // command query
        private static MySqlCommand cmd;

        #region P/Invoke...
        [DllImport("User32.dll")]
        static extern Boolean MessageBeep(UInt32 beepType);
        #endregion

        #region User Interface...
        //Funtion Open connected from databases localhost
        private static void OpenConnection()
        {
            try
            {
                conn.Open();
                //MessageBox.Show("OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //  close connect
        private static bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void Delete()
        {
            string query = "DELETE FROM rfidreader.rfid";
            cmd = new MySqlCommand(query, conn);
            MySqlDataReader MyReader;
            try
            {
                OpenConnection();
                MyReader = cmd.ExecuteReader();
                MessageBox.Show("Delete all Database", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                CloseConnection();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                CloseConnection();
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            tsModelType_Click(tsModelIDRO900F, EventArgs.Empty);
            tsConnectType_Click(tsConnectSerial, EventArgs.Empty);
            tsTagType_Click(tsMi6C1Gen2, EventArgs.Empty);
        }
        public ModelType ModelType
        {
            get
            {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), tsModelType.Tag.ToString());
                return modelType;
            }
        }
        public ConnectType ConnectType
        {
            get
            {
                ConnectType connectType = tsConnectType.Text.Contains("TCP/IP") ? ConnectType.Tcp : ConnectType.Serial;
                return connectType;
            }


        }
        public TagType TagType
        {
            get
            {
                TagType tagType = (TagType)Enum.Parse(typeof(TagType), tsTagType.Tag.ToString());
                return tagType;
            }
        }

        private void tsConnectType_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem nConnect = sender as ToolStripMenuItem;
            tsConnectType.Text = nConnect.Text;
            bool bTcpIp = ConnectType == ConnectType.Tcp;
            tsLbAddress.Text = bTcpIp ? "IP:" : "Port:";
            tsCbbAddress.Visible = bTcpIp;
            tsCbbSerial.Visible = !bTcpIp;
        }
        private void tsModelType_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem nItem = sender as ToolStripMenuItem;
            tsModelType.Tag = nItem.Tag;
            tsModelType.Text = nItem.Text;

        }

        private void tsTagType_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem nItem = sender as ToolStripMenuItem;
            tsTagType.Tag = nItem.Tag;
            tsTagType.Text = nItem.Text;
            if (reader != null)
                reader.TagType = TagType;
        }

        private void tsCbbSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Process Button..
        private void tsBtConnect_Click(object sender, EventArgs e)
        {
            lbxResponses.Items.Add("Attempting Connection...");
            if (reader != null)
            {
                if (reader.IsHandling)
                {
                    reader.Close(CloseType.Close);
                    return;
                }
            }
            reader.ReaderEvent += new ReaderEventHandler(OnReaderEvent);
            reader.TagType = TagType;
            reader.ConnectType = ConnectType;
            reader.ModelType = ModelType;

            if (ConnectType == ConnectType.Tcp)
            {
                reader.Open(tsCbbAddress.Text, 5578);
            }
            else
                reader.Open(tsCbbSerial.Text, 115200);
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
        private void OnReaderEvent(object sender, ReaderEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ReaderEventHandler(OnReaderEvent), new object[] { sender, e });
                return;
            }
            switch (e.Type)
            {
                case EventType.Connected:
                    {
                        tsLbState.Text = e.Message;
                        tsBtConnect.Text = "&Disconnect";
                        tsFunctions.Visible = true;
                        break;
                    }
                case EventType.Disconnected:
                    {
                        tsLbState.Text = e.Message;
                        tsBtConnect.Text = "&Connect";
                        tsFunctions.Visible = true;
                        if (e.CloseType == CloseType.FormClose)
                            Close();
                        else
                            setTimer(6000, "Reconnect");
                        break;
                    }
                case EventType.Timeout:
                    {
                        tsLbState.Text = e.Message;
                        break;
                    }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // BASIC OPERATIONS EVENTS
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                case EventType.Inventory:
                case EventType.ReadMemory:
                case EventType.WriteMemory:
                case EventType.Command:
                case EventType.Lock:
                case EventType.Kill:
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // CONFIGURATIONS EVENTS
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                case EventType.Buzzer:
                case EventType.ContinueMode:
                case EventType.Power:
                case EventType.Version:
                case EventType.AccessPwd:
                case EventType.GlobalBand:
                case EventType.Port:
                case EventType.Selection:
                case EventType.Filtering:
                case EventType.Algorithm:
                case EventType.TcpIp:
                    {

                        //Received data are in the byte array (e.payload), it decodes into string using 
                        //following function.
                        string szPayload = Encoding.ASCII.GetString(e.Payload);
                        // MessageBox.Show(szPayload);
                        //Many responses can be contained in the generated event and it separates first.
                        //Received data may contain tag ID, set value, response code, etc. according to call 
                        //function, byte array(e.payload) may contain more than 1 response therefore it
                        //separates using following separator.

                        string[] szResponses = szPayload.Split(new string[] { "\r\n>" }, StringSplitOptions.RemoveEmptyEntries);
                        ///////////////////////////////////////////////////////////////////////////////////////////////////
                        // Response Code : [#]C##
                        // Tag Memory : [#]T3000111122223333444455556666[##]
                        // RSSI: [#]RFD##
                        // Settings Values : p0, c1, ...
                        ///////////////////////////////////////////////////////////////////////////////////////////////////
                        char code;
                        string szValue;
                        bool bCheckSum = reader.IsFixedType();
                        bool bMultiPort = reader.IsMultiPort();
                        int nPos = bMultiPort ? 1 : 0;
                        int flag1 = 0;
                        int rssi1 = 0, rssi2 = 0, rssi3 = 0, rssi4 = 0, rssi = 0;
                        Dictionary<string, int> dRss1 = new Dictionary<string, int>();
                        Dictionary<string, int> dRss2 = new Dictionary<string, int>();
                        Dictionary<string, int> dRss3 = new Dictionary<string, int>();
                        Dictionary<string, int> dRss4 = new Dictionary<string, int>();
                        // lbxResponses.Items.Insert(0, szPayload);
                        string szTxt = string.Empty;
                        foreach (string szResponse in szResponses)
                        {
                            code = szResponse[nPos];
                            this.totalRead += 1;
                            //  code = 'R';
                            switch (code)
                            {
                                case 'T':
                                    {
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
                                                tbTagId.Text = szValue;
                                                tbPort.Text = szResponse[0].ToString();
                                                tbText.Text = szTxt;
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
                                                tbTagId1.Text = szValue;
                                                tbPort1.Text = szResponse[0].ToString();
                                                tbText1.Text = szTxt;
                                                break;
                                            case '3':
                                                tbTagId2.Text = szValue;
                                                tbPort2.Text = szResponse[0].ToString();
                                                tbText2.Text = szTxt;
                                                break;
                                            case '4':
                                                tbTagId3.Text = szValue;
                                                tbPort3.Text = szResponse[0].ToString();
                                                tbText3.Text = szTxt;
                                                break;
                                            default:
                                                break;
                                        }
                                        lbxResponses.Items.Insert(0, "Tag ID: " + szValue);
                                        lbxResponses.Items.Insert(1, "Port: " + szResponse[0]);
                                        lbxResponses.Items.Insert(2, "Text: " + szTxt);
                                    }
                                    break;
                                case 'C':
                                    {
                                        szValue = szResponse.Substring(nPos + 1, szResponse.Length - (nPos + 1));//exclude [#]T/C
                                        szValue = szValue + "-" + reader.Responses(szValue);
                                        //if HEARTBEAT response, fire heartbeat timer again.
                                        if (string.Compare(szValue, "FF", StringComparison.Ordinal) == 0)
                                        {
                                            setTimer(9000, "Heartbeat");
                                        }
                                        lbxResponses.Items.Insert(0, "Data" + szValue);
                                    }
                                    break;
                                case 'R':
                                    {
                                        szValue = szResponse.Substring(szResponse.Length - 2, 2);
                                        rssi = Convert.ToInt32(szValue, 0x10);

                                        double a = sdk.distanceFromRssi(rssi);

                                        lbxResponses.Items.Insert(0, "Rssi " + szResponse[0] + ":" + " -" + rssi);
                                        switch (szResponse[0])
                                        {
                                            case '1':
                                                {
                                                    rssi1 = rssi;
                                                    tbRssi.Text = "-" + rssi.ToString();
                                                    //if (rssi != 0) dt1.Add(totalReadCounts1);
                                                    arrRssi1.Add(rssi);
                                                    totalReadCounts2++;
                                                    tbRead.Text = totalReadCounts2.ToString();
                                                }
                                                break;
                                            case '2':
                                                {
                                                    rssi2 = rssi;
                                                    tbRssi1.Text = "-" + rssi.ToString();
                                                    //if (rssi != 0) dt2.Add(totalReadCounts1);
                                                    arrRssi2.Add(rssi);
                                                    totalReadCounts2++;
                                                    tbRead1.Text = totalReadCounts2.ToString();
                                                }
                                                break;
                                            case '3':
                                                {
                                                    this.tbRssi2.Text = "-" + rssi.ToString();
                                                    rssi3 = rssi;
                                                    //if (rssi != 0) dt3.Add(totalReadCounts1);
                                                    arrRssi3.Add(rssi);
                                                    totalReadCounts3++;
                                                    tbRead2.Text = totalReadCounts3.ToString();
                                                }
                                                break;
                                            case '4':
                                                {
                                                    rssi4 = rssi;
                                                    tbRssi3.Text = "-" + rssi.ToString();
                                                    //if (rssi != 0) dt4.Add(totalReadCounts1);
                                                    arrRssi4.Add(rssi);
                                                    totalReadCounts4++;
                                                    tbRead3.Text = totalReadCounts4.ToString();
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        if (trainingFlag == false)
                                        {

                                            string query = "INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2, Rssi3, Rssi4, Text, TotalRead1, TotalRead2, TotalRead3, TotalRead4,Position) VALUE('" + num5 + "', '" + rssi1.ToString() + "','" + rssi2.ToString() + "',  '" + rssi3.ToString() + "', '" + rssi4.ToString() + "','" + text + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "', '" + "" + "')";
                                            cmd = new MySqlCommand(query, conn);
                                            MySqlDataReader myReader;
                                            try
                                            {
                                                OpenConnection();
                                                myReader = cmd.ExecuteReader();
                                                CloseConnection();
                                            }
                                            catch (MySqlException ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                                CloseConnection();
                                            }
                                            rssi1 = rssi2 = rssi3 = rssi4 = 0;
                                            this.totalRead = 0;
                                        }
                                    }
                                    break;

                                default:
                                    lbxResponses.Items.Insert(1, szResponse);
                                    break;
                            }

                        }
                        break;
                    }
                default:
                    break;
            }
        }
        private void trainingTimerEventProcessor(Object obj, EventArgs args)
        {
            reader.StopOperation();
            traningTimer.Stop();

            if (totalReadCounts1 >= a && totalReadCounts2 >= a && totalReadCounts3 >= a && totalReadCounts4 >= a)
            {
                for (int i = 0; i < a; i++)
                {
                    cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                    MySqlDataReader myReader;
                    try
                    {
                        OpenConnection();
                        myReader = cmd.ExecuteReader();
                        CloseConnection();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                        CloseConnection();
                    }
                }
                traningTimer.Stop();
            }
            else
            {
                timeDelay += timeDelay_;
                if (timeDelay > 12000)
                {
                    if (totalReadCounts1 >= a && totalReadCounts2 >= a && totalReadCounts3 >= a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            //  arrRssi4[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts1 >= a && totalReadCounts2 >= a && totalReadCounts4 >= a && totalReadCounts3 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi3[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts1 >= a && totalReadCounts3 >= a && totalReadCounts4 >= a && totalReadCounts2 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts2 >= a && totalReadCounts3 >= a && totalReadCounts4 >= a && totalReadCounts1 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi1[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts1 >= a && totalReadCounts2 >= a && totalReadCounts3 < a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi4[i] = 0;
                            arrRssi3[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts1 >= a && totalReadCounts4 >= a && totalReadCounts3 < a && totalReadCounts2 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {

                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi2[i] = 0;
                            arrRssi3[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts3 >= a && totalReadCounts4 >= a && totalReadCounts2 < a && totalReadCounts1 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {

                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi1[i] = 0;
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts1 >= a && totalReadCounts3 >= a && totalReadCounts4 < a && totalReadCounts2 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi4[i] = 0;
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts4 >= a && totalReadCounts2 >= a && totalReadCounts1 < a && totalReadCounts3 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {

                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi1[i] = 0;
                            arrRssi3[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts2 >= a && totalReadCounts3 >= a && totalReadCounts1 < a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);

                            arrRssi4[i] = 0;
                            arrRssi1[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }

                    if (totalReadCounts1 >= a && totalReadCounts2 < a && totalReadCounts3 < a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + arrRssi1[i].ToString() + "','" + "0" + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi4[i] = 0;
                            arrRssi3[i] = 0;
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts2 >= a && totalReadCounts1 < a && totalReadCounts3 < a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + arrRssi2[i].ToString() + "','" + "0" + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi4[i] = 0;
                            arrRssi3[i] = 0;
                            arrRssi1[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                    if (totalReadCounts3 >= a && totalReadCounts2 < a && totalReadCounts1 < a && totalReadCounts4 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + arrRssi3[i].ToString() + "','" + "0" + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi4[i] = 0;
                            arrRssi1[i] = 0;
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }

                    if (totalReadCounts4 >= a && totalReadCounts2 < a && totalReadCounts3 < a && totalReadCounts2 < a)
                    {
                        for (int i = 0; i < a; i++)
                        {
                            cmd = new MySqlCommand("INSERT INTO rfidreader.rfid(TagId ,Rssi1,Rssi2,Rssi3,Rssi4,Text, TotalRead1, TotalRead2,TotalRead3,TotalRead4,Position) VALUE('" + "khanh" + "', '" + "0" + "','" + "0" + "','" + "0" + "','" + arrRssi4[i].ToString() + "','" + "4" + "' , '" + totalReadCounts2.ToString() + "','" + totalReadCounts2.ToString() + "','" + totalReadCounts3.ToString() + "','" + totalReadCounts4.ToString() + "','" + "" + "')", conn);
                            arrRssi1[i] = 0;
                            arrRssi3[i] = 0;
                            arrRssi2[i] = 0;
                            MySqlDataReader myReader;
                            try
                            {
                                OpenConnection();
                                myReader = cmd.ExecuteReader();
                                CloseConnection();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                CloseConnection();
                            }
                        }
                    }
                }
                else
                {
                    traningTimer.Interval = timeDelay_;
                    traningTimer.Start();
                    reader.InventoryMultiple();

                }
            }
        }
        private void inventoryTimerEventProcessor(object obj, EventArgs args)
        {
            reader.StopOperation();
            traningTimer.Stop();
            List<Double> diffRssi;
            double rssi1_ = 0, rssi2_ = 0, rssi3_ = 0, rssi4_ = 0;
            List<int> arr1 = new List<int>();
            List<int> result = new List<int>();
            List<int> arr2 = new List<int>();
            double[,] data1 = new double[3, 3];
            double[,] data2 = new double[3, 3];
            double[,] data3 = new double[3, 3];
            double[,] data4 = new double[3, 3];

            double diffRssi1 = 0, diffRssi2 = 0, diffRssi3 = 0;
            int i = 0, j = 0;
            Dictionary<String, Double> dicDiff = new Dictionary<string, double>();
            Map map = new Map();
            sendBox SendBox = new sendBox(map.getBox);

            rssi1_ = averageRssi(arrRssi1);
            rssi2_ = averageRssi(arrRssi2);
            rssi3_ = averageRssi(arrRssi3);
            rssi4_ = averageRssi(arrRssi4);


            // tbPosition.Text = rssi1_.ToString();
            if ((rssi1_ != 0))
            {
                //tbPosition.Text =rssi1_.ToString();
                dt1.Add((pw1 - 50) / 5);
            }
            if (totalReadCounts2 != 0)
            {
               // tbPosition1.Text =((pw2 - 130) / 10).ToString();
                if (((pw2 - 50) / 5) != 0) dt2.Add((pw2 - 50) / 5);
            }
            if ((rssi3_ != 0))
            {
             //tbPosition2.Text = rssi3_.ToString();
                dt3.Add((pw3 - 50) / 5);
            }
            if ((rssi4_ != 0))
            {
                 //tbPosition3.Text = ((pw4 - 100) / 10).ToString();
                if (((pw4 - 50) / 5) != 0) dt4.Add((pw4 - 50) / 5);
            }
            if ((rssi1_ != 0) && (rssi2_ != 0) && (rssi3_ != 0) && (rssi4_ != 0))
            {

               tbPosition.Text = dt1[0].ToString();
                //if (dt2[0] != 0)
                //{
                   //tbPosition1.Text = dt2[0].ToString();
                //}
                //else
               
                    tbPosition1.Text = dt2[0].ToString();
               
               tbPosition2.Text = dt3[0].ToString();
                // if (dt1[0] == 0) tbPosition3.Text = dt4[].ToString();
               tbPosition3.Text = dt4[0].ToString();
               box = sdk.locate(dt1[0], dt2[0], dt3[0], dt4[0]);

                pw1 = pw2 = pw3 = pw4 = 45;
                dt1.Clear();
                dt2.Clear();
                dt3.Clear();
                dt4.Clear();
                // rssi1_ = rssi2_ = rssi3_=rssi4_ = 0;

                //tinh box tu dt////

              //  box = sdk.locate(dt1[0], dt2[0], dt3[0], dt4[0]);
////////////////////////////////////////////////////////////////////////////
           





                ////////////////////////////////////////////////////////////////////////////////
            }
            //tbPosition.Text = rssi1_.ToString();
            textBox1.Text = pw1.ToString();
            textBox2.Text = pw2.ToString();
            textBox3.Text = pw3.ToString();
            textBox4.Text = pw4.ToString();
            pw1 = pw1 + 5;
            pw2 = pw2 + 5;
            pw3 = pw3 + 5;
            pw4 = pw4 + 5;
            if (pw1 > 300)
            {

                pw1 = pw2 = pw3 = pw4 = 50;
                if (dt1.Count == 0) dt1[0] = 50;
                if (dt2.Count == 0) dt2[0] = 50;
                if (dt3.Count == 0) dt3[0] = 50;
                if (dt4.Count == 0) dt4[0] = 50;
                box = sdk.locate(dt1[0], dt2[0], dt3[0], dt4[0]);
                dt1.Clear();
                dt2.Clear();
                dt3.Clear();
                dt4.Clear();
                //tinh box tu dt/////
                
                rssi1_ = rssi2_ = rssi3_ = rssi4_ = 0;

                


            }


            reader.SetPortPower(1, pw1);
            reader.SetPortPower(2, pw2);
            reader.SetPortPower(3, pw3);
            reader.SetPortPower(4, pw4);
            //     //time_++;
            //     //dt1++;


            //if ((rssi1_ != 0)&&(f1 = false))
            //{
            //    dt1 = time_;
            //    f1 = true;
            //    tbPosition.Text = dt1.ToString();
            //}
            //if ((rssi2_ != 0) && (f2 = false))
            //{
            //    dt2 = time_;
            //    f2 = true;
            //}
            //if ((rssi3_ != 0) && (f3 = false))
            //{
            //    dt3 = time_;
            //    f3 = true;
            //}
            //if ((rssi4_ != 0) && (f4 = false))
            //{
            //    dt4 = time_;
            //    f4 = true;
            //}
            //if (f1 = f2 = f3 = f4 = true)
            //{
            //    tbPosition.Text = dt1.ToString();
            //    tbPosition1.Text = dt2.ToString();
            //    tbPosition2.Text = dt3.ToString();
            //    tbPosition3.Text = dt4.ToString();
            //    dt1 = dt2 = dt3 = dt4 = 0;
            //    f1 =  f2 = f3 =  f4 = false;
            //    pw1 = pw2 = pw3 = pw4 = 0;
            //    time_ = 0;
            //}

            //cmd = new MySqlCommand("SELECT * FROM rfidreader.training", conn);
            //MySqlDataReader myReader;
            //try
            //{
            //    OpenConnection();
            //    myReader = cmd.ExecuteReader();
            //    while (myReader.Read())
            //    {
            //        data1[i, j] = myReader.GetDouble(2);
            //        data2[i, j] = myReader.GetDouble(3);
            //        data3[i, j] = myReader.GetDouble(4);
            //        data4[i, j++] = myReader.GetDouble(5);
            //        if (j == 3)
            //        {
            //            i++;
            //            j = 0;
            //        }
            //    };
            //    CloseConnection();
            //}
            //catch (MySqlException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    CloseConnection();
            //}

            /////////////////////////////////////////////////////////////////

            //if (rssi1_ != 0) local_1 = 1;
            //else local_1 = 0;
            //if (rssi2_ != 0) local_2 = 10;
            //else local_2 = 0;
            //if (rssi3_ != 0) local_3 = 100;
            //else local_3 = 0;
            //if (rssi4_ != 0) local_4 = 1000;
            //else local_4 = 0;
            //local = local_1 + local_2 + local_3 + local_4;
            //switch (local)
            //{
            //    case 0:
            //        {
            //            //box = 0; break;
            //            flag = 1;
            //            if ((pw1 == 300) && (pw2 == 300) && (pw3 == 300) && (pw4 == 300))
            //            {
            //                box = 0;
            //                break;
            //            }
            //            reader.SetPortPower(1, pw1 + 5);
            //            reader.SetPortPower(2, pw2 + 5);
            //            reader.SetPortPower(3, pw3 + 5);
            //            reader.SetPortPower(4, pw4 + 5);
            //            break;
            //        }
            //    case 1:
            //        {
            //            if (flag == 0) box = 1;
            //            break;
            //        }
            //    case 10:
            //        {
            //            if (flag == 0) box = 7;
            //            break;
            //        }
            //    case 100:
            //        {
            //            if (flag == 0) box = 9;
            //            break;
            //        }
            //    case 1000:
            //        {
            //            if (flag == 0) box = 3;
            //            break;
            //        }

            //    case 11:
            //        {
            //            box = 4;
            //            flag = 0;
            //            break;
            //        }
            //    case 110:
            //        {
            //            box = 8;
            //            flag = 0;
            //            break;
            //        }
            //    case 1100:
            //        {
            //            box = 6;
            //            flag = 0;
            //            break;
            //        }
            //    case 1001:
            //        {
            //            box = 2;
            //            flag = 0;
            //            break;
            //        }
            //    case 1111:
            //        {
            //            box = 5; break;
            //        }
            //}



            //  box = 7;

            //////////////////////////////////////////////////////////////////
            //int local_xy1 = sdk.local_xy(data1, rssi1_);
            //int local_xy2 = sdk.local_xy(data2, rssi2_);
            //int local_xy3 = sdk.local_xy(data3, rssi3_);
            //int local_xy4 = sdk.local_xy(data4, rssi4_);

            ////////////////////////////////////////////////////////////////////

            // int k =0,h1 =0,h2 =0,h3 =0;

            /*
            for (i = 0; i < local_xy1.Length; i++)
            {
                for(j = 0; j<local_xy2.Length;j++)
                    if ((local_xy1[i] == local_xy2[j])&&(local_xy1[i] != 0))
                    
                        arr1.Add(local_xy1[i]);
                    
            }

            for (i = 0; i < local_xy3.Length; i++)
            {
                for (j = 0; j < local_xy4.Length; j++)
                    if ((local_xy3[i] == local_xy4[j]) && (local_xy3[i] != 0))

                        arr2.Add(local_xy3[i]);

            }

            for (i = 0; i < arr1.Count; i++)
                for (j = 0; j < arr2.Count; j++)
                    if (arr1[i] == arr2[j])
                        result.Add(arr1[i]);
            */







            // for (i = 0; i < local_xy1.Length; i++)
            // {
            //     if (local_xy1[i] != 0)
            //         if (h1 < sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy1[i])) 
            //     {
            //         h1 = sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy1[i]);
            //         h3 = i;
            //     }
            // }
            // h2 = local_xy1[h3];
            // for (i = 0; i < local_xy2.Length; i++)
            // {
            //     if (local_xy2[i] != 0)
            //         if (h1 < sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy2[i])) 
            //     {
            //         h1 = sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy2[i]);
            //         h3 = i;
            //         k = 1;
            //     }
            // }
            // if(k ==1) h2 = local_xy2[h3];
            // k = 0;
            // for (i = 0; i < local_xy3.Length; i++)
            // {
            //     if (local_xy3[i] != 0)
            //         if (h1 < sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy3[i])) 
            //     {
            //         h1 = sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy3[i]);
            //         h3 = i;
            //         k = 1;
            //     }

            // }
            // if(k ==1) h2 = local_xy3[h3];
            // k = 0;
            // for (i = 0; i < local_xy4.Length; i++)
            // {

            //     if (local_xy4[i] != 0)
            //         if (h1 < sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy4[i])) 
            //     {
            //         h1 = sdk.in_array1(local_xy1, local_xy2, local_xy3, local_xy4, local_xy4[i]);
            //         h3 = i;
            //         k = 1;
            //     }
            // }
            // if(k ==1) h2 = local_xy4[h3];
            // k = 0;
            // box = h2;
            //// box =  result[0];







            //box = sdk.int_((local_xy1+local_xy2+local_xy3+local_xy4)/4);





            // box = 3;
            //////////////////////////////////////////////////////////////////
            //////////Truyen vi tri len CSDL SQL///////////////////
         //   SendToSever(num5, box);
            //  box = 3;
            ////////////////////////////////////////////////////////////////

            SendBox(box);
            totalReadCounts1 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
            rssi1_ = rssi2_ = rssi3_ = rssi4_ = 0;
            arrRssi1.Clear();
            arrRssi1.Clear();
            arrRssi3.Clear();
            arrRssi4.Clear();
            reader.InventoryMultiple();
            // traningTimer.Start();
        }
        private void tsBtInventoryMulti_Click(object sender, EventArgs e)
        {
            traningTimer.Stop();
            trainingFlag = false;
            totalReadCounts2 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
            arrRssi1.Clear();
            arrRssi1.Clear();
            arrRssi3.Clear();
            arrRssi4.Clear();
            reader.InventoryMultiple();
            inventoryTimer.Start();
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
            //return (double)(result/(arr.Count));
            return (double)(result / 5);
        }
        private void tsBtInventorySingle_Click(object sender, EventArgs e)
        {
            reader.InventorySingle();
        }

        private void tsBtStop_Click(object sender, EventArgs e)
        {
            reader.StopOperation();
            traningTimer.Stop();
            // trainingTim
        }

        private void tsBtReadMemeory_Click(object sender, EventArgs e)
        {
            reader.ReadMemory(MemoryType.EPC, 1, 3);
        }

        private void tsBtWriteMemory_Click(object sender, EventArgs e)
        {
            string tx = null;
            List<string> tx1 = cipher.transpositionCipherCryptography(cbTextWrite.Text, "ascii.txt");
            for (int i = 0; i < tx1.Count; i++)
            {
                tx += tx1[i];
            }

            reader.WriteMemory(tx);
        }

        private void tsBtLock_Click(object sender, EventArgs e)
        {
            //reader.Lock("0030",);
            // reader.SetPortPower(1, 300);
        }
        private void tsBtKill_Click(object sender, EventArgs e)
        {
            //reader.Kill("12345678");
        }

        private void tsBtClear_Click(object sender, EventArgs e)
        {
            lbxResponses.Items.Clear();
        }
        private void tsBtConfigurations_Click(object sender, EventArgs e)
        {
            //Configures conf = new Configures();
            // conf.Show();
            // reader.SetDefaultSettings();//3E 78 20 64

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //GENERAL - COMMON
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //reader.SetPower(1);//3E 78 20 70 20 31
            //reader.SetAccessPwd("12345678");//3E 78 20 77 20 31 32 33 34 35 36 37 38
            //reader.SetGlobalBand(1);//3E 78 20 66 20 31

            //reader.GetPower();//3E 79 20 70
            //reader.GetFwVersion();//3E 79 20 76 0A 0D
            ////3E 79 31 31 
            //reader.GetAccessPwd();
            //reader.GetGlobalBand();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //OPTIONS - COMMON
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // reader.SetBuzzer(false);
            //reader.SetContinueMode(false);
            //reader.GetBuzzer();
            //reader.GetContinueMode();
            //   reader.SetPower(0);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // PORT - FIXED TYPE
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            reader.SetPortActive(1);
            reader.SetPortActive(2);
            reader.SetPortActive(3);
            reader.SetPortActive(4);
            reader.SetPortPower(1, 300);
            reader.SetPortPower(2, 300);
            reader.SetPortPower(3, 300);
            reader.SetPortPower(4, 300);
            reader.SetPortInventoryCount(50);
            reader.SetPortInventoryTime(100);
            reader.SetPortIdleTime(110);

            //reader.GetPortActive();
            //   reader.GetPortPower(1);
            //   reader.GetPortPower(2);
            //reader.GetPortPower(3);
            //reader.GetPortPower(4);
            //reader.GetPortInventoryCount();
            //reader.GetPortInventoryTime();
            //reader.GetPortIdleTime();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //SELECTION - FIXED TYPE
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //reader.SetSelectionBank(MemoryType.EPC);
            //reader.SetSelectionOffset(16);
            //reader.SetSelectionAction(SelectionActionType.Matching);
            //reader.SetSelectionSession(0);

            //reader.GetSelectionBank();
            //reader.GetSelectionOffset();
            //reader.GetSelectionAction();
            //reader.GetSelectionSession();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //ALGORITHM - FIXED TYPE
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //reader.SetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 0, 7);
            //reader.SetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 1, 0);
            //reader.SetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 2, 15);

            //reader.GetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 0);
            //reader.GetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 1);
            //reader.GetAlgorithmParameter(Algorithm.DYNAMICQ_THRESH, 2);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //TCP/IP - FIXED TYPE
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //reader.SetConnectionSetting(ConnectionSetting.Address, "192.168.9.6");

            //reader.GetConnectionSetting(ConnectionSetting.Address);
            //reader.GetConnectionSetting(ConnectionSetting.Port);
            //reader.GetConnectionSetting(ConnectionSetting.Baudrate);
        }
        private void tsBtGetDataStore_Click(object sender, EventArgs e)
        {
            //In the case of a large store of data,
            //Because it may cause problems, The following process is recommended if necessary
            //1. stop heartbeat
            //2. stop inventory
            reader.Command("o", false);//Get Stored data
        }

        #endregion
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
            OnReaderEvent(this, new ReaderEventArgs(EventType.Timer, timerMessage));
        }
        public bool IsTiming
        {
            get { return timer == null ? false : timer.Enabled; }
        }
        private void tsBtClearDatabase_Click(object sender, EventArgs e)
        {
            Delete();
        }
        private void btMapping_Click(object sender, EventArgs e)
        {
            Map map = new Map();
            map.ShowDialog();
            /* String path1 = "ascii.txt";
             List<String> m = new List<string>();
             char[,] b = null;
             double c;
             string input1 = "AEMHKUDJG";
             Dictionary<String, Double> res;
             m = cipher.transpositionCipherCryptography(input1, path1);
             b = cipher.transpositionCipherDecryption(m);
             string d = cipher.substitutionCipherDecryption(b, path1);
             MessageBox.Show(d);*/
        }
        #endregion
        #region Process Delegate
        public void getDataConfig(int power, string st2, string st3, string st4)
        {
        }
        public void getDataWrite(String str)
        {
            stt[0] = str;
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
            cmd = new MySqlCommand("INSERT INTO rfidreader.measure(cell ,rssi1,rssi2, rssi3, rssi4) VALUE('" + cell + "', '" + ((double)(r1 / arrRssi1.Count)).ToString() + "','" + ((double)(r2 / arrRssi2.Count)).ToString() + "',  '" + ((double)(r3 / arrRssi3.Count)).ToString() + "', '" + ((double)(r4 / arrRssi4.Count)).ToString() + "')", conn);
            MySqlDataReader myReader;
            try
            {
                OpenConnection();
                myReader = cmd.ExecuteReader();
                CloseConnection();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                CloseConnection();
            }
        }
        #endregion

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        private void btn_Bluetooth(object sender, EventArgs e)
        {
            long endTick = time.Ticks;
            MessageBox.Show((endTick - tick).ToString());
        }

        private void tbTagId_Click(object sender, EventArgs e)
        {

        }

        private void tbText_Click(object sender, EventArgs e)
        {

        }



        private void trainingData_click(object sender, EventArgs e)
        {
            totalReadCounts2 = totalReadCounts2 = totalReadCounts3 = totalReadCounts4 = 0;
            trainingFlag = true;
            arrRssi1.Clear();
            arrRssi1.Clear();
            arrRssi3.Clear();
            arrRssi4.Clear();
            traningTimer.Start();
            reader.InventoryMultiple();
        }

        private void calculator_click(object sender, EventArgs e)
        {
            cell Cell = new cell();
            Cell.ShowDialog();
        }

        private void btnLut_click(object sender, EventArgs e)
        {
            cmd = new MySqlCommand("SELECT * FROM rfidreader.training", conn);
            MySqlDataReader myReader;
            Lut lut;
            double rssi1 = 0, rssi2 = 0, rssi3 = 0, rssi4 = 0;
            int cell = 0;
            try
            {
                OpenConnection();
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    lut = new Lut(myReader.GetInt16(1), myReader.GetDouble(2), myReader.GetDouble(3), myReader.GetDouble(4), myReader.GetDouble(5));
                    dataTraining.Add(lut);
                };
                CloseConnection();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                CloseConnection();
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
                    rssi4 += lut.getRssi4();
                }
                lut.setCell(cell);
                lut.setRssi1(rssi1 / 8);
                lut.setRssi2(rssi2 / 8);
                lut.setRssi3(rssi3 / 8);
                lut.setRssi4(rssi4 / 8);
                dataRssi.Add(lut);
                rssi1 = rssi2 = rssi3 = rssi4 = 0;
                count += 8;
            }
            //  MessageBox.Show(dataRssi.Count.ToString());
        }

        private void tsCbbAddress_Click(object sender, EventArgs e)
        {

        }

        private void tsMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tsFunctions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tbTagId2_Click(object sender, EventArgs e)
        {

        }



        private void lbxResponses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //////////Truyen vi tri len CSDL SQL///////////////////
        public void SendToSever(string a, int b)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-PMCDQ3D\\SQLEXPRESS;Initial Catalog=RFID;Integrated Security=True");
            SqlCommand MysqlCmd = con.CreateCommand();
            MysqlCmd.CommandText = "EXECUTE InsertRFID @Mathe, @Location, @NgayTao";
            MysqlCmd.Parameters.AddWithValue("@Mathe", a);
            MysqlCmd.Parameters.AddWithValue("@Location", b);
            MysqlCmd.Parameters.Add("@NgayTao", SqlDbType.DateTime, 10).Value = DateTime.Now;

            con.Open();
            MysqlCmd.ExecuteNonQuery();
            con.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            reader.SetPortPower(1, 50);
            reader.SetPortPower(2, 50);
            reader.SetPortPower(3, 50);
            reader.SetPortPower(4, 50);
        }

      



    }

}
