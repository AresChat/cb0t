using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class StringTemplate
    {
        private static List<STItem> items { get; set; }

        public static void Load()
        {
            if (items != null)
                items.Clear();

            items = new List<STItem>();

            items.Add(new STItem { Type = STType.About, Index = 0, Text = "English template" });
            items.Add(new STItem { Type = STType.About, Index = 1, Text = "by oobe" });

            items.Add(new STItem { Type = STType.AudioBar, Index = 0, Text = "playlist options" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 1, Text = "Clear list" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 2, Text = "Random" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 3, Text = "Repeat" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 4, Text = "Import files" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 5, Text = "Import folder" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 6, Text = "volume" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 7, Text = "stop" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 8, Text = "previous" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 9, Text = "play/pause" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 10, Text = "next" });

            items.Add(new STItem { Type = STType.TopBar, Index = 0, Text = "Settings" });
            items.Add(new STItem { Type = STType.TopBar, Index = 1, Text = "Audio" });
            items.Add(new STItem { Type = STType.TopBar, Index = 2, Text = "Channels" });
            items.Add(new STItem { Type = STType.TopBar, Index = 3, Text = "Searching" });

            items.Add(new STItem { Type = STType.Settings, Index = 0, Text = "Client" });
            items.Add(new STItem { Type = STType.Settings, Index = 1, Text = "Client Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 2, Text = "Chat" });
            items.Add(new STItem { Type = STType.Settings, Index = 3, Text = "Chat Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 4, Text = "Hashlink" });
            items.Add(new STItem { Type = STType.Settings, Index = 5, Text = "Hashlink Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 6, Text = "Personal" });
            items.Add(new STItem { Type = STType.Settings, Index = 7, Text = "Personal Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 8, Text = "Audio" });
            items.Add(new STItem { Type = STType.Settings, Index = 9, Text = "Audio Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 10, Text = "Filter" });
            items.Add(new STItem { Type = STType.Settings, Index = 11, Text = "Filter Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 12, Text = "Menu" });
            items.Add(new STItem { Type = STType.Settings, Index = 13, Text = "Menu Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 14, Text = "Privacy" });
            items.Add(new STItem { Type = STType.Settings, Index = 15, Text = "Privacy Settings" });

            items.Add(new STItem { Type = STType.ClientSettings, Index = 0, Text = "open data folder" });
            items.Add(new STItem { Type = STType.ClientSettings, Index = 1, Text = "Template" });
            items.Add(new STItem { Type = STType.ClientSettings, Index = 2, Text = "For in-room commands, type /cmds" });

            items.Add(new STItem { Type = STType.ChatSettings, Index = 0, Text = "Enable timestamps" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 1, Text = "Enable black background" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 2, Text = "Enable emoticons" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 3, Text = "Enable cb0t encryption protocol" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 4, Text = "Send typing status" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 5, Text = "Receive private messages" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 6, Text = "Receive nudges" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 7, Text = "Receive scribbles" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 8, Text = "Receive server latency" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 9, Text = "Block redirect requests" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 10, Text = "Block clear screen requests" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 11, Text = "Block all popups" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 12, Text = "Block friend popups" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 13, Text = "Spell checking" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 14, Text = "Global font" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 15, Text = "Font" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 16, Text = "Size" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 17, Text = "Receive user fonts" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 18, Text = "Block custom names" });

            items.Add(new STItem { Type = STType.HashlinkSettings, Index = 0, Text = "connect" });

            items.Add(new STItem { Type = STType.PersonalSettings, Index = 0, Text = "Name" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 1, Text = "Country" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 2, Text = "Region" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 3, Text = "Age" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 4, Text = "Gender" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 5, Text = "Secret" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 6, Text = "Personal message" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 7, Text = "update" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 8, Text = "PM auto reply" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 9, Text = "Enable PM auto reply" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 10, Text = "Avatar" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 11, Text = "clear" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 12, Text = "User Font" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 13, Text = "Text" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 14, Text = "Font" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 15, Text = "Size" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 16, Text = "Enable user font" });

            items.Add(new STItem { Type = STType.AudioSettings, Index = 0, Text = "Voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 1, Text = "Record device" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 2, Text = "Playback device" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 3, Text = "Receive public voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 4, Text = "Receive private voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 5, Text = "Record using Opus codec" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 6, Text = "Now playing" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 7, Text = "Source" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 8, Text = "Show songs in user list" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 9, Text = "Play notication sounds" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 10, Text = "Enable chat narration" });

            items.Add(new STItem { Type = STType.FilterSettings, Index = 0, Text = "Add" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 1, Text = "Edit" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 2, Text = "Remove" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 3, Text = "Enable filter" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 4, Text = "ok" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 5, Text = "missing text in section 7" });

            items.Add(new STItem { Type = STType.MenuSettings, Index = 0, Text = "User list menu options" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 1, Text = "Room menu options" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 2, Text = "Name" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 3, Text = "Action" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 4, Text = "Remove" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 5, Text = "Add new menu option" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 6, Text = "Option name" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 7, Text = "Action text" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 8, Text = "add" });

            items.Add(new STItem { Type = STType.PrivacySettings, Index = 0, Text = "Auto ignored users" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 1, Text = "Condition" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 2, Text = "Name" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 3, Text = "Block" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 4, Text = "add" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 5, Text = "Remove" });

            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 0, Text = "thin brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 1, Text = "medium brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 2, Text = "thick brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 3, Text = "paste" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 4, Text = "color" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 5, Text = "undo" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 6, Text = "redo" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 7, Text = "clear" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 8, Text = "cancel" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 9, Text = "send" });

            items.Add(new STItem { Type = STType.ColorPicker, Index = 0, Text = "Chosen color" });
            items.Add(new STItem { Type = STType.ColorPicker, Index = 1, Text = "Pick a web color" });
            items.Add(new STItem { Type = STType.ColorPicker, Index = 2, Text = "Pick an ares color" });

            items.Add(new STItem { Type = STType.AudioPlayer, Index = 0, Text = "Are you sure you want to clear your play list?" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 1, Text = "Now playing" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 2, Text = "Title" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 3, Text = "Artist" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 4, Text = "Album" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 5, Text = "Duration" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 6, Text = "remove from playlist" });

            items.Add(new STItem { Type = STType.ChannelList, Index = 0, Text = "Refresh" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 1, Text = "Find" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 2, Text = "Language" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 3, Text = "Name" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 4, Text = "Topic" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 5, Text = "Export hashlink" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 6, Text = "Add to favourites" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 7, Text = "Remove from favourites" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 8, Text = "Auto join" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 9, Text = "Admin password" });

            items.Add(new STItem { Type = STType.Commands, Index = 0, Text = "Time: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 1, Text = "Client: +x - running for +d days, +h hours, +m minutes" });
            items.Add(new STItem { Type = STType.Commands, Index = 2, Text = "Uptime: +d days, +h hours, +m minutes" });
            items.Add(new STItem { Type = STType.Commands, Index = 3, Text = "Graphics: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 4, Text = "Disk Space: +xGB out of +yGB available (+z% usage)" });
            items.Add(new STItem { Type = STType.Commands, Index = 5, Text = "Operating System: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 6, Text = "Processor: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 7, Text = "Memory: +xMB out of +yMB available (+z% usage)" });

            items.Add(new STItem { Type = STType.UserList, Index = 0, Text = "Options for" });
            items.Add(new STItem { Type = STType.UserList, Index = 1, Text = "Nudge" });
            items.Add(new STItem { Type = STType.UserList, Index = 2, Text = "Whois" });
            items.Add(new STItem { Type = STType.UserList, Index = 3, Text = "Ignore/Unignore" });
            items.Add(new STItem { Type = STType.UserList, Index = 4, Text = "Copy name to clipboard" });
            items.Add(new STItem { Type = STType.UserList, Index = 5, Text = "Add/Remove friend" });
            items.Add(new STItem { Type = STType.UserList, Index = 6, Text = "Browse" });
            items.Add(new STItem { Type = STType.UserList, Index = 7, Text = "Kill" });
            items.Add(new STItem { Type = STType.UserList, Index = 8, Text = "Ban" });
            items.Add(new STItem { Type = STType.UserList, Index = 9, Text = "Muzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 10, Text = "Unmuzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 11, Text = "Host kill" });
            items.Add(new STItem { Type = STType.UserList, Index = 12, Text = "Host ban" });
            items.Add(new STItem { Type = STType.UserList, Index = 13, Text = "Host muzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 14, Text = "Host unmuzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 15, Text = "Users" });
            items.Add(new STItem { Type = STType.UserList, Index = 16, Text = "Server" });
            items.Add(new STItem { Type = STType.UserList, Index = 17, Text = "Lag" });
            items.Add(new STItem { Type = STType.UserList, Index = 18, Text = "Friends" });
            items.Add(new STItem { Type = STType.UserList, Index = 19, Text = "Admins" });

            items.Add(new STItem { Type = STType.InBox, Index = 0, Text = "Cut" });
            items.Add(new STItem { Type = STType.InBox, Index = 1, Text = "Copy" });
            items.Add(new STItem { Type = STType.InBox, Index = 2, Text = "Paste" });
            items.Add(new STItem { Type = STType.InBox, Index = 3, Text = "Add to dictionary" });

            items.Add(new STItem { Type = STType.OutBox, Index = 0, Text = "Save image" });
            items.Add(new STItem { Type = STType.OutBox, Index = 1, Text = "Save voice clip" });
            items.Add(new STItem { Type = STType.OutBox, Index = 2, Text = "Clear screen" });
            items.Add(new STItem { Type = STType.OutBox, Index = 3, Text = "Export text" });
            items.Add(new STItem { Type = STType.OutBox, Index = 4, Text = "Copy to clipboard" });
            items.Add(new STItem { Type = STType.OutBox, Index = 5, Text = "Pause/Unpause screen" });
            items.Add(new STItem { Type = STType.OutBox, Index = 6, Text = "Screen unpaused" });
            items.Add(new STItem { Type = STType.OutBox, Index = 7, Text = "Screen paused" });

            items.Add(new STItem { Type = STType.ButtonBar, Index = 0, Text = "bold" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 1, Text = "italic" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 2, Text = "underline" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 3, Text = "foreground" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 4, Text = "background" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 5, Text = "emoticons" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 6, Text = "To record a Voice Clip, hold down this button or press F2." });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 7, Text = "Release the button to send your message." });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 8, Text = "scribble" });

            items.Add(new STItem { Type = STType.RoomMenu, Index = 0, Text = "Options" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 1, Text = "Close" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 2, Text = "Export hashlink" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 3, Text = "Add to favourites" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 4, Text = "Copy room name to clipboard" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 5, Text = "Auto play voice clips" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 6, Text = "Close sub tabs" });

            items.Add(new STItem { Type = STType.BrowseTab, Index = 0, Text = "Loading" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 1, Text = "All" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 2, Text = "Audio" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 3, Text = "Image" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 4, Text = "Video" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 5, Text = "Document" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 6, Text = "Software" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 7, Text = "Other" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 8, Text = "Title" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 9, Text = "Artist" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 10, Text = "Media" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 11, Text = "Category" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 12, Text = "Size" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 13, Text = "Filename" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 14, Text = "Files" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 15, Text = "Browse failed" });

            items.Add(new STItem { Type = STType.Messages, Index = 0, Text = "Typing: +x" });
            items.Add(new STItem { Type = STType.Messages, Index = 1, Text = "RECORDING [+x seconds remaining]" });
            items.Add(new STItem { Type = STType.Messages, Index = 2, Text = "Notification" });
            items.Add(new STItem { Type = STType.Messages, Index = 3, Text = "Nudge" });
            items.Add(new STItem { Type = STType.Messages, Index = 4, Text = "Friend" });
            items.Add(new STItem { Type = STType.Messages, Index = 5, Text = "+x has nudged you!" });
            items.Add(new STItem { Type = STType.Messages, Index = 6, Text = "+x is not receiving nudges" });
            items.Add(new STItem { Type = STType.Messages, Index = 7, Text = "Lag test" });
            items.Add(new STItem { Type = STType.Messages, Index = 8, Text = "received from +x" });
            items.Add(new STItem { Type = STType.Messages, Index = 9, Text = "User is offline" });
            items.Add(new STItem { Type = STType.Messages, Index = 10, Text = "User is ignoring you" });
            items.Add(new STItem { Type = STType.Messages, Index = 11, Text = "Topic update" });
            items.Add(new STItem { Type = STType.Messages, Index = 12, Text = "+x has parted" });
            items.Add(new STItem { Type = STType.Messages, Index = 13, Text = "+x has joined" });
            items.Add(new STItem { Type = STType.Messages, Index = 14, Text = "+x has joined +y" });
            items.Add(new STItem { Type = STType.Messages, Index = 15, Text = "Redirecting to +x..." });
            items.Add(new STItem { Type = STType.Messages, Index = 16, Text = "Language" });
            items.Add(new STItem { Type = STType.Messages, Index = 17, Text = "Server" });
            items.Add(new STItem { Type = STType.Messages, Index = 18, Text = "Logged in, retrieving user's list" });
            items.Add(new STItem { Type = STType.Messages, Index = 19, Text = "pre text updated" });
            items.Add(new STItem { Type = STType.Messages, Index = 20, Text = "pre text disabled" });
            items.Add(new STItem { Type = STType.Messages, Index = 21, Text = "Reconnecting" });
            items.Add(new STItem { Type = STType.Messages, Index = 22, Text = "Disconnected" });
            items.Add(new STItem { Type = STType.Messages, Index = 23, Text = "Unable to connect" });
            items.Add(new STItem { Type = STType.Messages, Index = 24, Text = "Connected, handshaking" });
            items.Add(new STItem { Type = STType.Messages, Index = 25, Text = "Connecting to host, please wait" });
            items.Add(new STItem { Type = STType.Messages, Index = 26, Text = "Sending" });
            items.Add(new STItem { Type = STType.Messages, Index = 27, Text = "your voice clip was cancelled" });
            items.Add(new STItem { Type = STType.Messages, Index = 28, Text = "your voice clip has recorded and is now being sent" });

            items.Add(new STItem { Type = STType.SystemTray, Index = 0, Text = "Show as Online" });
            items.Add(new STItem { Type = STType.SystemTray, Index = 1, Text = "Show as Away" });
            items.Add(new STItem { Type = STType.SystemTray, Index = 2, Text = "Exit" });

            LoadTemplate();
        }

        private static void LoadTemplate()
        {
            List<String> lines = new List<String>();
            String path = Path.Combine(Settings.AppPath, "templates\\" + Settings.GetReg<String>("template_name", "English") + ".txt");

            try { lines.AddRange(File.ReadAllLines(path, Encoding.UTF8)); }
            catch { }

            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    String str = lines[i];
                    int id = str.IndexOf("|");

                    if (id > -1)
                    {
                        String x = str.Substring(0, id);
                        str = str.Substring(id + 1);
                        id = str.IndexOf("|");

                        if (id > -1)
                        {
                            String y = str.Substring(0, id);
                            str = str.Substring(id + 1);
                            int ix, iy;

                            if (int.TryParse(x, out ix))
                                if (int.TryParse(y, out iy))
                                {
                                    STItem item = items.Find(q => (int)q.Type == ix && q.Index == iy);

                                    if (item != null)
                                        item.Text = str;
                                }
                        }
                    }
                }
            }
        }

        public static String Get(STType type, int index)
        {
            try
            {
                return items.Find(x => x.Type == type && x.Index == index).Text;
            }
            catch { }

            return String.Empty;
        }
    }

    enum STType : int
    {
        About = 0,
        AudioBar = 1,
        TopBar = 2,
        Settings = 3,
        ClientSettings = 4,
        ChatSettings = 5,
        HashlinkSettings = 6,
        PersonalSettings = 7,
        AudioSettings = 8,
        FilterSettings = 9,
        MenuSettings = 10,
        PrivacySettings = 11,
        ScribbleEditor = 12,
        ColorPicker = 13,
        AudioPlayer = 14,
        ChannelList = 15,
        Commands = 16,
        UserList = 17,
        InBox = 18,
        OutBox = 19,
        ButtonBar = 20,
        RoomMenu = 21,
        BrowseTab = 22,
        Messages = 23,
        SystemTray = 24
    }

    class STItem
    {
        public STType Type { get; set; }
        public String Text { get; set; }
        public int Index { get; set; }
    }
}
