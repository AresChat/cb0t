using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace cb0t_chat_client_v2
{
    class AudioSettings
    {
        public static bool repeat;
        public static bool shuffle;
        public static bool mute;
        public static bool radio_mode;
        public static String last_folder_path;
        public static String last_radio;
        public static String current_song;
        public static bool winamp_;
        public static bool voice_mute;
        public static bool show_album_art;
        public static bool show_in_userlist;
        public static String np_text;
        public static bool unicode_effect;
        public static AudioPlayerChoice choice;
        

        public static void Load()
        {
            repeat = false;
            shuffle = false;
            mute = false;
            radio_mode = false;
            last_folder_path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            current_song = String.Empty;
            last_radio = String.Empty;
            winamp_ = false;
            voice_mute = true;
            show_album_art = true;
            show_in_userlist = true;
            np_text = "/me np: +n";
            unicode_effect = false;
            choice = AudioPlayerChoice.Internal;

            try
            {
                using (FileStream f = new FileStream(Settings.folder_path + "audiosettings.xml", FileMode.Open))
                {
                    try
                    {
                        XmlReader xml = XmlReader.Create(new StreamReader(f));

                        xml.MoveToContent();
                        xml.ReadSubtree().ReadToFollowing("audiosettings");
                        xml.ReadToFollowing("settings");
                        xml.ReadSubtree().ReadToFollowing("repeat");
                        repeat = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("shuffle");
                        shuffle = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("mute");
                        mute = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("last_folder_path");
                        last_folder_path = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("last_radio");
                        last_radio = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("winamp");
                        winamp_ = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("voice_mute");
                        voice_mute = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("show_album_art");
                        show_album_art = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("show_in_userlist");
                        show_in_userlist = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("np_text");
                        np_text = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("unicode_effect");
                        unicode_effect = bool.Parse(xml.ReadElementContentAsString());
                        xml.ReadToFollowing("audio_choice");
                        choice = (AudioPlayerChoice)int.Parse(xml.ReadElementContentAsString());
                        xml.Close();
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void Save()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "audiosettings.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("audiosettings");
                xml.WriteStartElement("settings");
                xml.WriteElementString("repeat", repeat.ToString().ToLower());
                xml.WriteElementString("shuffle", shuffle.ToString().ToLower());
                xml.WriteElementString("mute", mute.ToString().ToLower());
                xml.WriteElementString("last_folder_path", Convert.ToBase64String(Encoding.UTF8.GetBytes(last_folder_path)));
                xml.WriteElementString("last_radio", Convert.ToBase64String(Encoding.UTF8.GetBytes(last_radio)));
                xml.WriteElementString("winamp", winamp_.ToString().ToLower());
                xml.WriteElementString("voice_mute", voice_mute.ToString().ToLower());
                xml.WriteElementString("show_album_art", show_album_art.ToString().ToLower());
                xml.WriteElementString("show_in_userlist", show_in_userlist.ToString().ToLower());
                xml.WriteElementString("np_text", Convert.ToBase64String(Encoding.UTF8.GetBytes(np_text)));
                xml.WriteElementString("unicode_effect", unicode_effect.ToString().ToLower());
                xml.WriteElementString("audio_choice", ((int)choice).ToString());
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }

    }

    enum AudioPlayerChoice : int
    {
        Internal = 0,
        Winamp = 1,
        Itunes = 2
    }
}
