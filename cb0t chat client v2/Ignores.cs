using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace cb0t_chat_client_v2
{
    class Ignores
    {
        private static List<IgnoresItem> items = new List<IgnoresItem>();

        public static IgnoresItem[] CurrentList
        {
            get { return items.ToArray(); }
        }

        public static bool IsIgnored(UserObject userobj)
        {
            foreach (IgnoresItem i in items)
            {
                String value = null;

                switch (i.type)
                {
                    case "Name":
                        value = userobj.name;
                        break;

                    case "IP":
                        value = userobj.externalIp.ToString();
                        break;

                    case "Port":
                        value = userobj.dcPort.ToString();
                        break;
                }

                if (value != null)
                {
                    value = value.ToUpper();

                    switch (i.condition)
                    {
                        case "StartsWith":
                            if (value.StartsWith(i.value.ToUpper()))
                                return true;
                            break;

                        case "EndsWith":
                            if (value.EndsWith(i.value.ToUpper()))
                                return true;
                            break;

                        case "Equals":
                            if (value == i.value.ToUpper())
                                return true;
                            break;

                        case "Contains":
                            if (value.Contains(i.value.ToUpper()))
                                return true;
                            break;
                    }
                }
            }

            return false;
        }

        public static void AddItem(IgnoresItem i)
        {
            items.Add(i);
            Save();
        }

        public static void RemoveItem(int index)
        {
            items.RemoveAt(index);
            Save();
        }

        public static void Load()
        {
            try
            {
                FileStream f = new FileStream(Settings.folder_path + "ignores.xml", FileMode.Open);
                XmlReader xml = XmlReader.Create(new StreamReader(f));

                xml.MoveToContent();
                xml.ReadSubtree().ReadToFollowing("ignores");

                while (xml.ReadToFollowing("item"))
                {
                    IgnoresItem _obj = new IgnoresItem();

                    xml.ReadSubtree().ReadToFollowing("value");
                    _obj.value = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("condition");
                    _obj.condition = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("type");
                    _obj.type = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    items.Add(_obj);
                }

                xml.Close();
                f.Flush();
                f.Close();
            }
            catch { }
        }

        private static void Save()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "ignores.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("ignores");

                foreach (IgnoresItem _obj in items)
                {
                    xml.WriteStartElement("item");
                    xml.WriteElementString("value", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.value)));
                    xml.WriteElementString("condition", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.condition)));
                    xml.WriteElementString("type", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.type)));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }
    }

    class IgnoresItem
    {
        public String value = String.Empty;
        public String condition = String.Empty;
        public String type = String.Empty;
    }
}
