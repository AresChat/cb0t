using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace cb0t_chat_client_v2
{
    class ChatEvents
    {
        private static List<ChatEventObject> items;

        public static String[] GetEvents(ChannelObject cobj, UserObject uobj, String _event, String text)
        {
            List<String> list1 = new List<String>();

            if (Settings.allow_events)
            {
                if (_event == "OnConnect")
                {
                    List<ChatEventObject> list2 = items.FindAll(delegate(ChatEventObject c) { return (c._room == "Any" || c._room == cobj.name) && c._event == _event; });

                    foreach (ChatEventObject c in list2)
                        list1.Add(c._result);
                }
                else
                {
                    List<ChatEventObject> list2 = items.FindAll(delegate(ChatEventObject c) { return (c._room == "Any" || c._room == cobj.name) && c._event == _event; });

                    foreach (ChatEventObject c in list2)
                    {
                        if (FindMatch(c, FindVar(c, uobj, text)))
                        {
                            String str = c._result;
                            str = str.Replace("+name", uobj.name);
                            str = str.Replace("+lip", uobj.localIp.ToString());
                            str = str.Replace("+eip", uobj.externalIp.ToString());
                            str = str.Replace("+port", uobj.dcPort.ToString());
                            str = str.Replace("+text", text);
                            list1.Add(str);
                        }
                    }
                }
            }

            return list1.ToArray();
        }

        private static bool FindMatch(ChatEventObject c, String _var)
        {
            var _arg = c._argument.ToUpper();

            switch (c._condition)
            {
                case "StartsWith":
                    return _var.StartsWith(_arg);

                case "EndsWidth":
                    return _var.EndsWith(_arg);

                case "Contains":
                    return _var.Contains(_arg);

                case "Equals":
                    return _var == _arg;

                default:
                    return false;
            }
        }

        private static String FindVar(ChatEventObject c, UserObject obj, String text)
        {
            switch (c._variable)
            {
                case "Name":
                    return obj.name.ToUpper();

                case "External IP":
                    return obj.externalIp.ToString();

                case "Local IP":
                    return obj.localIp.ToString();

                case "Port":
                    return obj.dcPort.ToString();

                case "Text":
                    return text.ToUpper();

                default:
                    return null;
            }
        }

        public static void LoadChatEvents()
        {
            items = new List<ChatEventObject>();

            try
            {
                FileStream f = new FileStream(Settings.folder_path + "events.xml", FileMode.Open);
                XmlReader xml = XmlReader.Create(new StreamReader(f));

                xml.MoveToContent();
                xml.ReadSubtree().ReadToFollowing("events");

                while (xml.ReadToFollowing("item"))
                {
                    ChatEventObject _obj = new ChatEventObject();

                    xml.ReadSubtree().ReadToFollowing("argument");
                    _obj._argument = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("condition");
                    _obj._condition = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("event");
                    _obj._event = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("result");
                    _obj._result = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("room");
                    _obj._room = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("variable");
                    _obj._variable = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    items.Add(_obj);
                }

                xml.Close();
                f.Flush();
                f.Close();
            }
            catch { }
        }

        private static void SaveChatEvents()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "events.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("events");

                foreach (ChatEventObject _obj in items)
                {
                    xml.WriteStartElement("item");
                    xml.WriteElementString("argument", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._argument)));
                    xml.WriteElementString("condition", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._condition)));
                    xml.WriteElementString("event", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._event)));
                    xml.WriteElementString("result", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._result)));
                    xml.WriteElementString("room", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._room)));
                    xml.WriteElementString("variable", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj._variable)));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }

        public static void Add(ChatEventObject obj)
        {
            items.Add(obj);
            SaveChatEvents();
        }

        public static void RemoveAt(int index)
        {
            items.RemoveAt(index);
            SaveChatEvents();
        }

        public static ChatEventObject[] GetItems()
        {
            return items.ToArray();
        }
    }

    class ChatEventObject
    {
        public String _event;
        public String _room;
        public String _variable;
        public String _condition;
        public String _argument;
        public String _result;

        public override bool Equals(object obj)
        {
            ChatEventObject tmp = (ChatEventObject)obj;

            if (this._argument != tmp._argument) return false;
            if (this._condition != tmp._condition) return false;
            if (this._event != tmp._event) return false;
            if (this._result != tmp._result) return false;
            if (this._room != tmp._room) return false;
            if (this._variable != tmp._variable) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
