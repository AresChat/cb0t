using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
//using System.Drawing.Text;
using Microsoft.Win32;

namespace cb0t_chat_client_v2
{
    class Settings
    {
        public enum OnlineStatus : byte
        {
            online = 1,
            do_not_disturb = 2,
            be_back_later = 3,
            sleeping = 4
        };

        public static String folder_path = String.Empty;
        public static List<String> font_list = new List<String>();

        public static String[] favourites;

        public static List<IPAddress> blocked_dcs;
        public static String my_username;
        public static uint my_speed;
        public static Guid my_guid;
        public static bool cbot_visible;
        public static bool black_background;
        public static bool enable_emoticons;
        public static bool enable_timestamps;
        public static bool auto_add_rooms_to_favourites;
        public static bool receive_pm;
        public static bool auto_receive_channel_list;
        public static bool receive_nudges;
        public static bool enable_pm_reply;
        public static bool enable_dc_reply;
        public static bool dc_auto_accept;
        public static String pm_reply;
        public static String dc_reply;
        public static IPAddress local_ip;
        public static IPAddress external_ip;

        public static bool dc_enabled;
        public static ushort dc_port;

        public static String winamp_text;
        public static byte winamp_style;
        public static bool winamp_enabled;
        public static bool send_to_tray;

        public static String share_file_msg;
        public static bool share_file_on;

        public static String[] notify_msg;
        public static String[] pm_notify_msg;
        public static bool notify_on;
        public static bool notify_sound;
        public static bool pm_sound;
        public static bool scribble_enabled;

        public static String pre_color;

        // new proto

        public static byte age;
        public static byte sex;
        public static byte country;
        public static String region;
        public static String personal_message;

        public static OnlineStatus my_status;

        public static bool auto_load;
        public static bool mp3_repeat;
        public static bool mp3_audiosource_winamp;
        public static bool show_userlist_song;

        public static int chat_buffer_size;

        public static bool allow_events;
        public static bool allow_events_flood_check;
        public static bool send_custom_data;
        public static bool enable_custom_names;

        public static String font_name;
        public static int font_size;
        public static int size_x;
        public static int size_y;
        public static bool size_allow;
        public static String p_font_name;
        public static int p_font_size;

        public static bool ignore_cls;
        public static bool receive_custom_fonts;
        public static bool enable_my_custom_font;
        public static int p_name_col;
        public static int p_text_col;
        public static bool enable_whois_writing;

        public static bool enable_clips;
        public static bool receive_private_clips;
        public static bool auto_playback_clips1;
        public static bool record_quality;
        public static bool pm_notify;

        public static bool enable_custom_emotes;

        public static void Populate()
        {
            favourites = new String[] { };
            my_status = OnlineStatus.online;
            blocked_dcs = new List<IPAddress>();
            winamp_text = "/me np: +n";
            winamp_style = 0;
            winamp_enabled = false;
            dc_enabled = true;
            dc_port = 29992;
            pm_reply = "Hello +n, I'm not here at the moment.  Please leave me a message. :-)";
            dc_reply = "Hello +n, I'm not here at the moment.  Please leave me a message. :-)";
            enable_pm_reply = true;
            enable_dc_reply = true;
            local_ip = GetLocalIP();
            external_ip = local_ip;
            my_username = "cb0t user";
            my_speed = 0;
            my_guid = Guid.NewGuid();
            black_background = false;
            enable_emoticons = true;
            auto_add_rooms_to_favourites = false;
            receive_pm = true;
            auto_receive_channel_list = false;
            receive_nudges = true;
            send_to_tray = true;
            dc_auto_accept = false;
            share_file_msg = "cb0t 2.69 by oobe";
            share_file_on = true;
            notify_msg = new String[] { };
            pm_notify_msg = new String[] { };
            notify_on = false;
            notify_sound = true;
            age = 0;
            sex = 0;
            country = 0;
            region = String.Empty;
            personal_message = "cb0t chat client 2.69";
            scribble_enabled = true;
            auto_load = false;
            mp3_repeat = false;
            mp3_audiosource_winamp = false;
            show_userlist_song = true;
            chat_buffer_size = 800;
            allow_events = true;
            pre_color = String.Empty;
            allow_events_flood_check = true;
            send_custom_data = true;
            enable_custom_names = true;
            font_name = "Verdana";
            font_size = 10;
            size_x = 0;
            size_y = 0;
            size_allow = false;
            p_font_name = "Verdana";
            p_font_size = 10;
            ignore_cls = false;
            receive_custom_fonts = true;
            enable_my_custom_font = false;
            p_name_col = 1;
            p_text_col = 12;
            enable_whois_writing = true;
            enable_clips = true;
            receive_private_clips = true;
            auto_playback_clips1 = true;
            record_quality = false;
            pm_notify = true;
            pm_sound = true;
            enable_custom_emotes = true;

            String path = AppDomain.CurrentDomain.BaseDirectory + "data";

            if (!Directory.Exists(path))
                return;
            else
                folder_path = path + "\\";

            try
            {
                if (!File.Exists(path + "\\settings.xml"))
                {
                    try
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        path += "\\cb0t";

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        path += "\\data";

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        folder_path = path + "\\";
                    }
                    catch
                    {
                        folder_path = AppDomain.CurrentDomain.BaseDirectory + "data\\";
                    }
                }

                FileStream f = new FileStream(folder_path + "settings.xml", FileMode.Open);
                XmlReader xml = XmlReader.Create(new StreamReader(f));

                xml.MoveToContent();
                xml.ReadSubtree().ReadToFollowing("settings");

                xml.ReadSubtree().ReadToFollowing("name");
                my_username = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("speed");
                if (!uint.TryParse(xml.ReadElementContentAsString(), out my_speed))
                    my_speed = 0;

                my_speed = 0; // dropped from ares protocol

                xml.ReadToFollowing("title");
                byte[] temp_guid = Convert.FromBase64String(xml.ReadElementContentAsString());
                if (temp_guid.Length == 16)
                    my_guid = new Guid(temp_guid);

                xml.ReadToFollowing("background");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out black_background))
                    black_background = false;

                xml.ReadToFollowing("emoticons");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_emoticons))
                    enable_emoticons = true;

                xml.ReadToFollowing("timestamps");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_timestamps))
                    enable_timestamps = false;

                xml.ReadToFollowing("autofavourite");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out auto_add_rooms_to_favourites))
                    auto_add_rooms_to_favourites = false;

                xml.ReadToFollowing("receivepm");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out receive_pm))
                    receive_pm = true;

                xml.ReadToFollowing("receiveclist");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out auto_receive_channel_list))
                    auto_receive_channel_list = false;

                xml.ReadToFollowing("receivenudges");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out receive_nudges))
                    receive_nudges = true;

                xml.ReadToFollowing("enablepmreply");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_pm_reply))
                    enable_pm_reply = true;

                xml.ReadToFollowing("enabledcreply");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_dc_reply))
                    enable_dc_reply = true;

                xml.ReadToFollowing("pmreply");
                pm_reply = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("dcreply");
                dc_reply = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("enabledc");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out dc_enabled))
                    dc_enabled = true;

                xml.ReadToFollowing("dcport");
                if (!ushort.TryParse(xml.ReadElementContentAsString(), out dc_port))
                    dc_port = 29992;

                xml.ReadToFollowing("winamptext");
                winamp_text = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("winampstyle");
                if (!byte.TryParse(xml.ReadElementContentAsString(), out winamp_style))
                    winamp_style = 0;

                if (winamp_style > 4)
                    winamp_style = 0;

                xml.ReadToFollowing("winampenabled");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out winamp_enabled))
                    winamp_enabled = true;

                xml.ReadToFollowing("sendtotray");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out send_to_tray))
                    send_to_tray = true;

                xml.ReadToFollowing("dcautoaccept");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out dc_auto_accept))
                    dc_auto_accept = false;

                xml.ReadToFollowing("sfmsg");
                share_file_msg = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("sfenabled");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out share_file_on))
                    share_file_on = true;

                xml.ReadToFollowing("notifymsg");
                notify_msg = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString())).Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                xml.ReadToFollowing("notifyenabled");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out notify_on))
                    notify_on = false;

                xml.ReadToFollowing("notifysound");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out notify_sound))
                    notify_sound = false;

                xml.ReadToFollowing("age");
                if (!byte.TryParse(xml.ReadElementContentAsString(), out age))
                    age = 0;

                xml.ReadToFollowing("sex");
                if (!byte.TryParse(xml.ReadElementContentAsString(), out sex))
                    sex = 0;

                xml.ReadToFollowing("country");
                if (!byte.TryParse(xml.ReadElementContentAsString(), out country))
                    country = 0;

                xml.ReadToFollowing("region");
                region = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                region = region.Trim();

                if (region.Length > 20)
                    region = region.Substring(0, 20);

                xml.ReadToFollowing("personal_message");
                personal_message = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                personal_message = personal_message.Trim();

                if (personal_message.Length > 150)
                    personal_message = personal_message.Substring(0, 150);

                xml.ReadToFollowing("scribble");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out scribble_enabled))
                    scribble_enabled = false;

                xml.ReadToFollowing("mp3repeat");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out mp3_repeat))
                    mp3_repeat = false;

                xml.ReadToFollowing("mp3_audiosource_winamp");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out mp3_audiosource_winamp))
                    mp3_audiosource_winamp = false;

                xml.ReadToFollowing("show_userlist_song");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out show_userlist_song))
                    show_userlist_song = false;

                xml.ReadToFollowing("chat_buffer_size");
                if (!int.TryParse(xml.ReadElementContentAsString(), out chat_buffer_size))
                    chat_buffer_size = 800;

                xml.ReadToFollowing("allow_events");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out allow_events))
                    allow_events = true;

                xml.ReadToFollowing("precolor");
                pre_color = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("allow_events_flood_check");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out allow_events_flood_check))
                    allow_events_flood_check = true;

                xml.ReadToFollowing("send_custom_data");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out send_custom_data))
                    send_custom_data = true;

                xml.ReadToFollowing("enable_custom_names");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_custom_names))
                    enable_custom_names = true;

                xml.ReadToFollowing("font_name");
                font_name = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("font_size");
                if (!int.TryParse(xml.ReadElementContentAsString(), out font_size))
                    font_size = 10;

                if (font_size < 6 || font_size > 16)
                    font_size = 10;

                xml.ReadToFollowing("size_x");
                if (!int.TryParse(xml.ReadElementContentAsString(), out size_x))
                    size_x = 0;

                xml.ReadToFollowing("size_y");
                if (!int.TryParse(xml.ReadElementContentAsString(), out size_y))
                    size_y = 0;

                xml.ReadToFollowing("p_font_name");
                p_font_name = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                xml.ReadToFollowing("p_font_size");
                if (!int.TryParse(xml.ReadElementContentAsString(), out p_font_size))
                    p_font_size = 10;

                if (p_font_size < 8 || p_font_size > 16)
                    p_font_size = 10;

                xml.ReadToFollowing("ignore_cls");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out ignore_cls))
                    ignore_cls = false;

                xml.ReadToFollowing("receive_custom_fonts");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out receive_custom_fonts))
                    receive_custom_fonts = true;

                xml.ReadToFollowing("enable_my_custom_font");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_my_custom_font))
                    enable_my_custom_font = false;

                xml.ReadToFollowing("p_name_col");
                if (!int.TryParse(xml.ReadElementContentAsString(), out p_name_col))
                    p_name_col = 1;

                if (p_name_col < 0 || p_name_col > 15)
                    p_name_col = 1;

                xml.ReadToFollowing("p_text_col");
                if (!int.TryParse(xml.ReadElementContentAsString(), out p_text_col))
                    p_text_col = 12;

                if (p_text_col < 0 || p_text_col > 15)
                    p_text_col = 12;

                xml.ReadToFollowing("enable_whois_writing");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_whois_writing))
                    enable_whois_writing = true;

                xml.ReadToFollowing("enable_clips");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_clips))
                    enable_clips = true;

                xml.ReadToFollowing("receive_private_clips");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out receive_private_clips))
                    receive_private_clips = true;

                xml.ReadToFollowing("auto_playback_clips");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out auto_playback_clips1))
                    auto_playback_clips1 = true;

                xml.ReadToFollowing("record_quality");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out record_quality))
                    record_quality = true;

                xml.ReadToFollowing("pm_notify");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out pm_notify))
                    pm_notify = true;

                xml.ReadToFollowing("pm_sound");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out pm_sound))
                    pm_sound = true;

                xml.ReadToFollowing("pm_notify_msg");
                pm_notify_msg = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString())).Split(new String[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);

                xml.ReadToFollowing("enable_custom_emotes");
                if (!bool.TryParse(xml.ReadElementContentAsString(), out enable_custom_emotes))
                    enable_custom_emotes = true;

                xml.Close();
                f.Flush();
                f.Close();
            }
            catch { }

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

            if (key != null)
            {
                if (key.GetValue("cb0t") != null)
                {
                    auto_load = true;
                    key.Close();
                }
            }
        }

        public static void UpdateAutoLoad(bool state)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (state) // auto start
            {
                if (key != null)
                    key.SetValue("cb0t", AppDomain.CurrentDomain.BaseDirectory + "cb0t.exe");
            }
            else
            {
                if (key != null)
                    if (key.GetValue("cb0t") != null)
                        key.DeleteValue("cb0t");
            }

            if (key != null)
                key.Close();
        }

        public static void UpdateRecords()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(folder_path + "settings.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("settings");

                xml.WriteElementString("name", Convert.ToBase64String(Encoding.UTF8.GetBytes(my_username)));
                xml.WriteElementString("speed", "0"); // dropped from ares protocol
                xml.WriteElementString("title", Convert.ToBase64String(my_guid.ToByteArray()));
                xml.WriteElementString("background", black_background.ToString());
                xml.WriteElementString("emoticons", enable_emoticons.ToString());
                xml.WriteElementString("timestamps", enable_timestamps.ToString());
                xml.WriteElementString("autofavourite", auto_add_rooms_to_favourites.ToString());
                xml.WriteElementString("receivepm", receive_pm.ToString());
                xml.WriteElementString("receiveclist", auto_receive_channel_list.ToString());
                xml.WriteElementString("receivenudges", receive_nudges.ToString());
                xml.WriteElementString("enablepmreply", enable_pm_reply.ToString());
                xml.WriteElementString("enabledcreply", enable_dc_reply.ToString());
                xml.WriteElementString("pmreply", Convert.ToBase64String(Encoding.UTF8.GetBytes(pm_reply)));
                xml.WriteElementString("dcreply", Convert.ToBase64String(Encoding.UTF8.GetBytes(dc_reply)));
                xml.WriteElementString("enabledc", dc_enabled.ToString());
                xml.WriteElementString("dcport", dc_port.ToString());
                xml.WriteElementString("winamptext", Convert.ToBase64String(Encoding.UTF8.GetBytes(winamp_text)));
                xml.WriteElementString("winampstyle", winamp_style.ToString());
                xml.WriteElementString("winampenabled", winamp_enabled.ToString());
                xml.WriteElementString("sendtotray", send_to_tray.ToString());
                xml.WriteElementString("dcautoaccept", dc_auto_accept.ToString());
                xml.WriteElementString("sfmsg", Convert.ToBase64String(Encoding.UTF8.GetBytes(share_file_msg)));
                xml.WriteElementString("sfenabled", share_file_on.ToString());
                xml.WriteElementString("notifymsg", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join(",", notify_msg))));
                xml.WriteElementString("notifyenabled", notify_on.ToString());
                xml.WriteElementString("notifysound", notify_sound.ToString());
                xml.WriteElementString("age", age.ToString());
                xml.WriteElementString("sex", sex.ToString());
                xml.WriteElementString("country", country.ToString());
                xml.WriteElementString("region", Convert.ToBase64String(Encoding.UTF8.GetBytes(region)));
                xml.WriteElementString("personal_message", Convert.ToBase64String(Encoding.UTF8.GetBytes(personal_message)));
                xml.WriteElementString("scribble", scribble_enabled.ToString());
                xml.WriteElementString("mp3repeat", mp3_repeat.ToString());
                xml.WriteElementString("mp3_audiosource_winamp", mp3_audiosource_winamp.ToString());
                xml.WriteElementString("show_userlist_song", show_userlist_song.ToString());
                xml.WriteElementString("chat_buffer_size", chat_buffer_size.ToString());
                xml.WriteElementString("allow_events", allow_events.ToString());
                xml.WriteElementString("precolor", Convert.ToBase64String(Encoding.UTF8.GetBytes(pre_color)));
                xml.WriteElementString("allow_events_flood_check", allow_events_flood_check.ToString());
                xml.WriteElementString("send_custom_data", send_custom_data.ToString());
                xml.WriteElementString("enable_custom_names", enable_custom_names.ToString());
                xml.WriteElementString("font_name", Convert.ToBase64String(Encoding.UTF8.GetBytes(font_name)));
                xml.WriteElementString("font_size", font_size.ToString());
                xml.WriteElementString("size_x", size_x.ToString());
                xml.WriteElementString("size_y", size_y.ToString());
                xml.WriteElementString("p_font_name", Convert.ToBase64String(Encoding.UTF8.GetBytes(p_font_name)));
                xml.WriteElementString("p_font_size", p_font_size.ToString());
                xml.WriteElementString("ignore_cls", ignore_cls.ToString());
                xml.WriteElementString("receive_custom_fonts", receive_custom_fonts.ToString());
                xml.WriteElementString("enable_my_custom_font", enable_my_custom_font.ToString());
                xml.WriteElementString("p_name_col", p_name_col.ToString());
                xml.WriteElementString("p_text_col", p_text_col.ToString());
                xml.WriteElementString("enable_whois_writing", enable_whois_writing.ToString());
                xml.WriteElementString("enable_clips", enable_clips.ToString());
                xml.WriteElementString("receive_private_clips", receive_private_clips.ToString());
                xml.WriteElementString("auto_playback_clips", auto_playback_clips1.ToString());
                xml.WriteElementString("record_quality", record_quality.ToString());
                xml.WriteElementString("pm_notify", pm_notify.ToString());
                xml.WriteElementString("pm_sound", pm_sound.ToString());
                xml.WriteElementString("pm_notify_msg", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join("\0", pm_notify_msg))));
                xml.WriteElementString("enable_custom_emotes", enable_custom_emotes.ToString());

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }

        private static IPAddress GetLocalIP()
        {
            try
            {
                IPHostEntry list = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in list.AddressList)
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip;
            }
            catch { }

            return IPAddress.Loopback;
        }

    }
}
