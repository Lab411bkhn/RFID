using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Reader_Express
{
    class DatabaseXml
    {
        string path_xml = Directory.GetCurrentDirectory() + @"/rfidreader.xml" ;// @"Storage Card/Debug/rfidreader.xml";
        string path_xmlData = Directory.GetCurrentDirectory() + @"/rfidData.xml"; // @"Storage Card/Debug/rfidData.xml";
        public XmlDocument xml = new XmlDocument();
        public XmlDocument xml_Data = new XmlDocument();
        public XmlNode training;
        public XmlNode rfidNode;

        public DatabaseXml()
        {
            try
            {
                xml.Load(path_xml);
                training = xml.SelectSingleNode("//root/training");
                xml_Data.Load(path_xmlData);
                rfidNode = xml_Data.SelectSingleNode("//root/rfidNode");
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Cannot connect to Database_xml");
            }
        }

        public int GetTotalCard()
        {
            return Convert.ToInt32(rfidNode.Attributes[0].Value);
        }
        public int GetTotalTraining()
        {
            return Convert.ToInt32(training.Attributes[0].Value);
        }

        #region ...................................Training......................................................
        /// <summary>
        /// Lay gia tri rssi[index] cua the tagid
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getRssi(int cardId, int index)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/training/card[@id='" + cardId.ToString() + "']");
                return myxmlnode.Attributes[index + 1].Value;
            }
            catch { return "Empty"; } 
        }
        /// <summary>
        /// Lay vi tri the ung voi Id
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public string getPosition(int cardId)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/training/card[@id='" + cardId.ToString() + "']");
                return myxmlnode.Attributes[1].Value;
            }
            catch { return "Empty"; }
        }

        //creat CardID information 
        public void creatCard(int _tagId, object _rssi1, object _rssi2, object _rssi3, object _rssi4)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/training/card[@id='" + _tagId.ToString() + "']");
                //myxmlnode.Attributes["cell"].Value = _cell.ToString();
                myxmlnode.Attributes["rssi1"].Value = _rssi1.ToString();
                myxmlnode.Attributes["rssi2"].Value = _rssi2.ToString();
                myxmlnode.Attributes["rssi3"].Value = _rssi3.ToString();
                myxmlnode.Attributes["rssi4"].Value = _rssi4.ToString();
            }
            catch
            {
                int total_card = Convert.ToInt32(training.Attributes[0].Value);
                XmlNode node_card = xml.CreateElement("card");

                XmlAttribute id = xml.CreateAttribute("id");
                id.Value = _tagId.ToString();
                node_card.Attributes.Append(id);

                //XmlAttribute cell = xml.CreateAttribute("cell");
                //cell.Value = _cell.ToString();
                //node_card.Attributes.Append(cell);

                XmlAttribute rssi1 = xml.CreateAttribute("rssi1");
                rssi1.Value = _rssi1.ToString();
                node_card.Attributes.Append(rssi1);

                XmlAttribute rssi2 = xml.CreateAttribute("rssi2");
                rssi2.Value = _rssi2.ToString();
                node_card.Attributes.Append(rssi2);

                XmlAttribute rssi3 = xml.CreateAttribute("rssi3");
                rssi3.Value = _rssi3.ToString();
                node_card.Attributes.Append(rssi3);

                XmlAttribute rssi4 = xml.CreateAttribute("rssi4");
                rssi4.Value = _rssi4.ToString();
                node_card.Attributes.Append(rssi4);

                training.AppendChild(node_card);

                total_card++;
                training.Attributes[0].Value = total_card.ToString();
            }
            finally
            {
                xml.Save(path_xml);
            }
        }
        public void DeleteNodeTraining(int _tagId)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/training/card[@id='" + _tagId.ToString() + "']");
                int total_card = Convert.ToInt32(training.Attributes[0].Value);
                myxmlnode.ParentNode.RemoveChild(myxmlnode);
                total_card--;
            }
            catch
            {
            }
            finally
            {
                xml.Save(path_xml);
            }
        }
        public void DeleteAllTraining()
        {
            try
            {
                rfidNode.RemoveAll();
                XmlAttribute total_card = xml.CreateAttribute("total");
                total_card.Value = "0";
                training.Attributes.Append(total_card);
            }
            catch
            { }
            finally
            {
                xml.Save(path_xml);
            }
        }
        #endregion ...................................EndTraining...................................................
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////HaiDuong đz//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ......................................Data Rfid.....................................................
        public void InsertCard(int _tagId, object _rssi1, object _rssi2, object _rssi3, object _rssi4, uint _totalread1, uint _totalread2, uint _totalread3, uint _totalread4)
        {
            try
            {
                XmlElement element = xml_Data.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/rfidNode/card[@id='" + _tagId.ToString() + "']");
                myxmlnode.Attributes["rssi1"].Value = _rssi1.ToString();
                myxmlnode.Attributes["rssi2"].Value = _rssi2.ToString();
                myxmlnode.Attributes["rssi3"].Value = _rssi3.ToString();
                myxmlnode.Attributes["rssi4"].Value = _rssi4.ToString();
                //myxmlnode.Attributes["text"].Value = _text;
                myxmlnode.Attributes["totalread1"].Value = _totalread1.ToString();
                myxmlnode.Attributes["totalread2"].Value = _totalread2.ToString();
                myxmlnode.Attributes["totalread3"].Value = _totalread3.ToString();
                myxmlnode.Attributes["totalread4"].Value = _totalread4.ToString();
                //myxmlnode.Attributes["position"].Value = _position.ToString();
            }
            catch
            {
                int total_card = Convert.ToInt32(rfidNode.Attributes[0].Value);
                XmlNode node_card = xml_Data.CreateElement("card");

                XmlAttribute id = xml_Data.CreateAttribute("id");
                id.Value = _tagId.ToString();
                node_card.Attributes.Append(id);

                XmlAttribute rssi1 = xml_Data.CreateAttribute("rssi1");
                rssi1.Value = _rssi1.ToString();
                node_card.Attributes.Append(rssi1);

                XmlAttribute rssi2 = xml_Data.CreateAttribute("rssi2");
                rssi2.Value = _rssi2.ToString();
                node_card.Attributes.Append(rssi2);

                XmlAttribute rssi3 = xml_Data.CreateAttribute("rssi3");
                rssi3.Value = _rssi3.ToString();
                node_card.Attributes.Append(rssi3);

                XmlAttribute rssi4 = xml_Data.CreateAttribute("rssi4");
                rssi4.Value = _rssi4.ToString();
                node_card.Attributes.Append(rssi4);

                //XmlAttribute text = xml_Data.CreateAttribute("text");
                //text.Value = _text.ToString();
                //node_card.Attributes.Append(text);

                XmlAttribute totalread1 = xml_Data.CreateAttribute("totalread1");
                totalread1.Value = _totalread1.ToString();
                node_card.Attributes.Append(totalread1);

                XmlAttribute totalread2 = xml_Data.CreateAttribute("totalread2");
                totalread2.Value = _totalread2.ToString();
                node_card.Attributes.Append(totalread2);

                XmlAttribute totalread3 = xml_Data.CreateAttribute("totalread3");
                totalread3.Value = _totalread3.ToString();
                node_card.Attributes.Append(totalread3);

                XmlAttribute totalread4 = xml_Data.CreateAttribute("totalread4");
                totalread4.Value = _totalread4.ToString();
                node_card.Attributes.Append(totalread4);

                //XmlAttribute position = xml_Data.CreateAttribute("position");
                //position.Value = _position.ToString();
                //node_card.Attributes.Append(position);

                rfidNode.AppendChild(node_card);

                total_card++;
                rfidNode.Attributes[0].Value = total_card.ToString();
            }
            finally
            {
                xml_Data.Save(path_xmlData);
            }
        }
        public void DeleteNoderfid(int _tagId)
        {
            try
            {
            XmlElement element = xml_Data.DocumentElement;
            XmlNode myxmlnode = element.SelectSingleNode("//root/rfidNode/card[@id='" + _tagId.ToString() + "']");
            int total_card = Convert.ToInt32(rfidNode.Attributes[0].Value);
            myxmlnode.ParentNode.RemoveChild(myxmlnode);
            total_card--;  
            }
            catch 
            {
            }
            finally
            {
                xml_Data.Save(path_xmlData);
            }
        }
        public void DeleteAllrfid()
        {
            try
            {
                rfidNode.RemoveAll();
                XmlAttribute total_card = xml_Data.CreateAttribute("total") ;
                total_card.Value = "0";
                rfidNode.Attributes.Append(total_card);
            }
            catch
            { }
            finally
            {
                xml_Data.Save(path_xmlData);
            }
        }
        #endregion ...................................Data Rfid.....................................................
    }
}
