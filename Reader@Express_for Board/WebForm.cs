using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Reader_Express
{
    class WebForm
    {
        public Thread threadProcessWeb = null;
        
        
        public void ProcessDataSend(int position, string MaThe)
        {
            string url = "http://bkradapitmt.sanslab.vn/RFID/InsertRFID?MaThe=";
            try
            {
                string year = "0";
                string month = "0";
                string min = "0";
                string hour = "0";
                string second = "0";
                string day = "0";
                string dataSend = null;
                int yearI = DateTime.Now.Year;
                if (yearI < 10)
                {
                    year = year + yearI.ToString();
                }
                else year= yearI.ToString();
                int secondI = DateTime.Now.Second;
                if (secondI < 10)
                {
                    second = second + secondI.ToString();
                }
                else second = secondI.ToString();
                int minI = DateTime.Now.Minute;
                if (minI < 10)
                {
                    min = min + minI.ToString();
                }
                else min = minI.ToString();
                int hourI = DateTime.Now.Hour;
                if (hourI < 10)
                {
                    hour = hour + hourI.ToString();
                }
                else hour = hourI.ToString();
                int dayI = DateTime.Now.Day;
                if (dayI < 10)
                {
                    day = day + dayI.ToString();
                }
                else day = dayI.ToString();
                int monthI = DateTime.Now.Month;
                if (monthI < 10)
                {
                    month = month + monthI.ToString();
                }
                else month = monthI.ToString();
                dataSend = dataSend + MaThe + "&Location=" + position.ToString() + "&NgayTao=" + year + "-" + month + "-" + day + "%20" + hour + ":" + min + ":" + second;
                url = url + dataSend + "&Code=bkradRFID";
                //MessageBox.Show(url);
                //MainForm.DisplayData(tbShow, "Sended Data:" + dataSend);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                request.ProtocolVersion = HttpVersion.Version11;
                request.KeepAlive = true;
                StreamReader responseStream = null;

                HttpWebResponse webResponse = null;

                string webResponseStream = string.Empty;

                // Get response for http web request

                webResponse = (HttpWebResponse)request.GetResponse();
                
                responseStream = new StreamReader(webResponse.GetResponseStream());
                webResponseStream = responseStream.ReadToEnd();
                //MessageBox.Show("Response from web: " + webResponseStream);
                //close webresponse
                webResponse.Close();
                responseStream.Close();
                // show data receive
                
               //MainForm.DisplayData(tbShow, webResponseStream);
            }
            catch
            { 
            
            }
        }
       
    }
}
