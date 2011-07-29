using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace cb0t_chat_client_v2
{
    class MenuOptions
    {
        private static List<MenuOptionObject> items;

        public static void Load()
        {
            items = new List<MenuOptionObject>();

            try
            {
                FileStream f = new FileStream(Settings.folder_path + "menuoptions.xml", FileMode.Open);
                XmlReader xml = XmlReader.Create(new StreamReader(f));

                xml.MoveToContent();
                xml.ReadSubtree().ReadToFollowing("menuoptions");

                while (xml.ReadToFollowing("item"))
                {
                    MenuOptionObject _obj = new MenuOptionObject();

                    xml.ReadSubtree().ReadToFollowing("name");
                    _obj.name = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("text");
                    _obj.text = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

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
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "menuoptions.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("menuoptions");

                foreach (MenuOptionObject _obj in items)
                {
                    xml.WriteStartElement("item");
                    xml.WriteElementString("name", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.name)));
                    xml.WriteElementString("text", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.text)));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }

        public static void Add(MenuOptionObject obj)
        {
            items.Add(obj);
            Save();
        }

        public static void RemoveAt(int index)
        {
            items.RemoveAt(index);
            Save();
        }

        public static MenuOptionObject[] GetItems()
        {
            return items.ToArray();
        }
    }

    class MenuOptionObject
    {
        public String name;
        public String text;
    }
}
