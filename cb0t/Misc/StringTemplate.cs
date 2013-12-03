using System;
using System.Collections.Generic;
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

            items.Add(new STItem { Type = STType.About, Index = 0, Text = "1English template" });
            items.Add(new STItem { Type = STType.About, Index = 1, Text = "1by oobe" });

            items.Add(new STItem { Type = STType.AudioBar, Index = 0, Text = "1playlist options" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 1, Text = "1Clear list" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 2, Text = "1Random" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 3, Text = "1Repeat" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 4, Text = "1Import files" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 5, Text = "1Import folder" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 6, Text = "1volume" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 7, Text = "1stop" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 8, Text = "1previous" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 9, Text = "1play/pause" });
            items.Add(new STItem { Type = STType.AudioBar, Index = 10, Text = "1next" });

            items.Add(new STItem { Type = STType.TopBar, Index = 0, Text = "1Settings" });
            items.Add(new STItem { Type = STType.TopBar, Index = 1, Text = "1Audio" });
            items.Add(new STItem { Type = STType.TopBar, Index = 2, Text = "1Channels" });
            items.Add(new STItem { Type = STType.TopBar, Index = 3, Text = "1Searching" });

            items.Add(new STItem { Type = STType.Settings, Index = 0, Text = "1Client" });
            items.Add(new STItem { Type = STType.Settings, Index = 1, Text = "1Client Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 2, Text = "1Chat" });
            items.Add(new STItem { Type = STType.Settings, Index = 3, Text = "1Chat Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 4, Text = "1Hashlink" });
            items.Add(new STItem { Type = STType.Settings, Index = 5, Text = "1Hashlink Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 6, Text = "1Personal" });
            items.Add(new STItem { Type = STType.Settings, Index = 7, Text = "1Personal Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 8, Text = "1Audio" });
            items.Add(new STItem { Type = STType.Settings, Index = 9, Text = "1Audio Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 10, Text = "1Filter" });
            items.Add(new STItem { Type = STType.Settings, Index = 11, Text = "1Filter Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 12, Text = "1Menu" });
            items.Add(new STItem { Type = STType.Settings, Index = 13, Text = "1Menu Settings" });
            items.Add(new STItem { Type = STType.Settings, Index = 14, Text = "1Privacy" });
            items.Add(new STItem { Type = STType.Settings, Index = 15, Text = "1Privacy Settings" });

            items.Add(new STItem { Type = STType.ClientSettings, Index = 0, Text = "1open data folder" });
            items.Add(new STItem { Type = STType.ClientSettings, Index = 1, Text = "1Template" });
            items.Add(new STItem { Type = STType.ClientSettings, Index = 2, Text = "1For in-room commands, type /cmds" });

            items.Add(new STItem { Type = STType.ChatSettings, Index = 0, Text = "1Enable timestamps" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 1, Text = "1Enable black background" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 2, Text = "1Enable emoticons" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 3, Text = "1Enable cb0t encryption protocol" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 4, Text = "1Send typing status" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 5, Text = "1Receive private messages" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 6, Text = "1Receive nudges" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 7, Text = "1Receive scribbles" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 8, Text = "1Receive server latency" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 9, Text = "1Block redirect requests" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 10, Text = "1Block clear screen requests" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 11, Text = "1Block all popups" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 12, Text = "1Block friend popups" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 13, Text = "1Spell checking" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 14, Text = "1Global font" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 15, Text = "1Font" });
            items.Add(new STItem { Type = STType.ChatSettings, Index = 16, Text = "1Size" });

            items.Add(new STItem { Type = STType.HashlinkSettings, Index = 0, Text = "1connect" });

            items.Add(new STItem { Type = STType.PersonalSettings, Index = 0, Text = "1Name" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 1, Text = "1Country" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 2, Text = "1Region" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 3, Text = "1Age" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 4, Text = "1Gender" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 5, Text = "1Secret" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 6, Text = "1Personal message" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 7, Text = "1update" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 8, Text = "1PM auto reply" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 9, Text = "1Enable PM auto reply" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 10, Text = "1Avatar" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 11, Text = "1clear" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 12, Text = "1User Font" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 13, Text = "1Text" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 14, Text = "1Font" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 15, Text = "1Size" });
            items.Add(new STItem { Type = STType.PersonalSettings, Index = 16, Text = "1Enable user font" });

            items.Add(new STItem { Type = STType.AudioSettings, Index = 0, Text = "1Voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 1, Text = "1Record device" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 2, Text = "1Playback device" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 3, Text = "1Receive public voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 4, Text = "1Receive private voice clips" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 5, Text = "1Record using Opus codec" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 6, Text = "1Now playing" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 7, Text = "1Source" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 8, Text = "1Show songs in user list" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 9, Text = "1Play notication sounds" });
            items.Add(new STItem { Type = STType.AudioSettings, Index = 10, Text = "1Enable chat narration" });

            items.Add(new STItem { Type = STType.FilterSettings, Index = 0, Text = "1Add" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 1, Text = "1Edit" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 2, Text = "1Remove" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 3, Text = "1Enable filter" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 4, Text = "1ok" });
            items.Add(new STItem { Type = STType.FilterSettings, Index = 5, Text = "1missing text in section 7" });

            items.Add(new STItem { Type = STType.MenuSettings, Index = 0, Text = "1User list menu options" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 1, Text = "1Room menu options" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 2, Text = "1Name" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 3, Text = "1Action" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 4, Text = "1Remove" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 5, Text = "1Add new menu option" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 6, Text = "1Option name" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 7, Text = "1Action text" });
            items.Add(new STItem { Type = STType.MenuSettings, Index = 8, Text = "1add" });

            items.Add(new STItem { Type = STType.PrivacySettings, Index = 0, Text = "1Auto ignored users" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 1, Text = "1Condition" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 2, Text = "1Name" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 3, Text = "1Block" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 4, Text = "1add" });
            items.Add(new STItem { Type = STType.PrivacySettings, Index = 5, Text = "1Remove" });

            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 0, Text = "1thin brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 1, Text = "1medium brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 2, Text = "1thick brush" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 3, Text = "1paste" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 4, Text = "1color" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 5, Text = "1undo" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 6, Text = "1redo" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 7, Text = "1clear" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 8, Text = "1cancel" });
            items.Add(new STItem { Type = STType.ScribbleEditor, Index = 9, Text = "1send" });

            items.Add(new STItem { Type = STType.ColorPicker, Index = 0, Text = "1Chosen color" });
            items.Add(new STItem { Type = STType.ColorPicker, Index = 1, Text = "1Pick a web color" });
            items.Add(new STItem { Type = STType.ColorPicker, Index = 2, Text = "1Pick an ares color" });

            items.Add(new STItem { Type = STType.AudioPlayer, Index = 0, Text = "1Are you sure you want to clear your play list?" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 1, Text = "1Now playing" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 2, Text = "1Title" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 3, Text = "1Artist" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 4, Text = "1Album" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 5, Text = "1Duration" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 6, Text = "1remove from playlist" });
            items.Add(new STItem { Type = STType.AudioPlayer, Index = 7, Text = "1Now playing" });

            items.Add(new STItem { Type = STType.ChannelList, Index = 0, Text = "1Refresh" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 1, Text = "1Find" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 2, Text = "1Language" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 3, Text = "1Name" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 4, Text = "1Topic" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 5, Text = "1Export hashlink" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 6, Text = "1Add to favourites" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 7, Text = "1Remove from favourites" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 8, Text = "1Auto join" });
            items.Add(new STItem { Type = STType.ChannelList, Index = 9, Text = "1Admin password" });

            items.Add(new STItem { Type = STType.Commands, Index = 0, Text = "1Time: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 1, Text = "1Client: +x - running for +d days, +h hours, +m minutes" });
            items.Add(new STItem { Type = STType.Commands, Index = 2, Text = "1Uptime: +d days, +h hours, +m minutes" });
            items.Add(new STItem { Type = STType.Commands, Index = 3, Text = "1Graphics: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 4, Text = "1Disk Space: +xGB out of +yGB available (+z% usage)" });
            items.Add(new STItem { Type = STType.Commands, Index = 5, Text = "1Operating System: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 6, Text = "1Processor: +x" });
            items.Add(new STItem { Type = STType.Commands, Index = 7, Text = "1Memory: +xMB out of +yMB available (+z% usage)" });

            items.Add(new STItem { Type = STType.UserList, Index = 0, Text = "1Options for" });
            items.Add(new STItem { Type = STType.UserList, Index = 1, Text = "1Nudge" });
            items.Add(new STItem { Type = STType.UserList, Index = 2, Text = "1Whois" });
            items.Add(new STItem { Type = STType.UserList, Index = 3, Text = "1Ignore/Unignore" });
            items.Add(new STItem { Type = STType.UserList, Index = 4, Text = "1Copy name to clipboard" });
            items.Add(new STItem { Type = STType.UserList, Index = 5, Text = "1Add/Remove friend" });
            items.Add(new STItem { Type = STType.UserList, Index = 6, Text = "1Browse" });
            items.Add(new STItem { Type = STType.UserList, Index = 7, Text = "1Kill" });
            items.Add(new STItem { Type = STType.UserList, Index = 8, Text = "1Ban" });
            items.Add(new STItem { Type = STType.UserList, Index = 9, Text = "1Muzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 10, Text = "1Unmuzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 11, Text = "1Host kill" });
            items.Add(new STItem { Type = STType.UserList, Index = 12, Text = "1Host ban" });
            items.Add(new STItem { Type = STType.UserList, Index = 13, Text = "1Host muzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 14, Text = "1Host unmuzzle" });
            items.Add(new STItem { Type = STType.UserList, Index = 15, Text = "1Users" });
            items.Add(new STItem { Type = STType.UserList, Index = 16, Text = "1Server" });
            items.Add(new STItem { Type = STType.UserList, Index = 17, Text = "1Lag" });

            items.Add(new STItem { Type = STType.InBox, Index = 0, Text = "1Cut" });
            items.Add(new STItem { Type = STType.InBox, Index = 1, Text = "1Copy" });
            items.Add(new STItem { Type = STType.InBox, Index = 2, Text = "1Paste" });
            items.Add(new STItem { Type = STType.InBox, Index = 3, Text = "1Add to dictionary" });

            items.Add(new STItem { Type = STType.OutBox, Index = 0, Text = "1Save image" });
            items.Add(new STItem { Type = STType.OutBox, Index = 1, Text = "1Save voice clip" });
            items.Add(new STItem { Type = STType.OutBox, Index = 2, Text = "1Clear screen" });
            items.Add(new STItem { Type = STType.OutBox, Index = 3, Text = "1Export text" });
            items.Add(new STItem { Type = STType.OutBox, Index = 4, Text = "1Copy to clipboard" });
            items.Add(new STItem { Type = STType.OutBox, Index = 5, Text = "1Pause/Unpause screen" });
            items.Add(new STItem { Type = STType.OutBox, Index = 6, Text = "1Screen unpaused" });
            items.Add(new STItem { Type = STType.OutBox, Index = 7, Text = "1Screen paused" });

            items.Add(new STItem { Type = STType.ButtonBar, Index = 0, Text = "1bold" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 1, Text = "1italic" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 2, Text = "1underline" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 3, Text = "1foreground" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 4, Text = "1background" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 5, Text = "1emoticons" });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 6, Text = "1To record a Voice Clip, hold down this button or press F2." });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 7, Text = "1Release the button to send your message." });
            items.Add(new STItem { Type = STType.ButtonBar, Index = 8, Text = "1scribble" });

            items.Add(new STItem { Type = STType.RoomMenu, Index = 0, Text = "1Options" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 1, Text = "1Close" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 2, Text = "1Export hashlink" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 3, Text = "1Add to favourites" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 4, Text = "1Copy room name to clipboard" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 5, Text = "1Auto play voice clips" });
            items.Add(new STItem { Type = STType.RoomMenu, Index = 6, Text = "1Close sub tabs" });

            items.Add(new STItem { Type = STType.BrowseTab, Index = 0, Text = "1Loading" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 1, Text = "1All" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 2, Text = "1Audio" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 3, Text = "1Image" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 4, Text = "1Video" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 5, Text = "1Document" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 6, Text = "1Software" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 7, Text = "1Other" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 8, Text = "1Title" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 9, Text = "1Artist" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 10, Text = "1Media" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 11, Text = "1Category" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 12, Text = "1Size" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 13, Text = "1Filename" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 14, Text = "1Files" });
            items.Add(new STItem { Type = STType.BrowseTab, Index = 15, Text = "1Browse failed" });

            items.Add(new STItem { Type = STType.Messages, Index = 0, Text = "1Typing: +x" });
            items.Add(new STItem { Type = STType.Messages, Index = 1, Text = "1RECORDING [+x seconds remaining]" });
            items.Add(new STItem { Type = STType.Messages, Index = 2, Text = "1Notification" });
            items.Add(new STItem { Type = STType.Messages, Index = 3, Text = "1Nudge" });
            items.Add(new STItem { Type = STType.Messages, Index = 4, Text = "1Friend" });
            items.Add(new STItem { Type = STType.Messages, Index = 5, Text = "1+x has nudged you!" });
            items.Add(new STItem { Type = STType.Messages, Index = 6, Text = "1+x is not receiving nudges" });
            items.Add(new STItem { Type = STType.Messages, Index = 7, Text = "1Lag test" });
            items.Add(new STItem { Type = STType.Messages, Index = 8, Text = "1received from +x" });
            items.Add(new STItem { Type = STType.Messages, Index = 9, Text = "1User is offline" });
            items.Add(new STItem { Type = STType.Messages, Index = 10, Text = "1User is ignoring you" });
            items.Add(new STItem { Type = STType.Messages, Index = 11, Text = "1Topic update" });
            items.Add(new STItem { Type = STType.Messages, Index = 12, Text = "1+x has parted" });
            items.Add(new STItem { Type = STType.Messages, Index = 13, Text = "1+x has joined" });
            items.Add(new STItem { Type = STType.Messages, Index = 14, Text = "1+x has joined +y" });
            items.Add(new STItem { Type = STType.Messages, Index = 15, Text = "1Redirecting to +x..." });
            items.Add(new STItem { Type = STType.Messages, Index = 16, Text = "1Language" });
            items.Add(new STItem { Type = STType.Messages, Index = 17, Text = "1Server" });
            items.Add(new STItem { Type = STType.Messages, Index = 18, Text = "1Logged in, retrieving user's list" });
            items.Add(new STItem { Type = STType.Messages, Index = 19, Text = "1pre text updated" });
            items.Add(new STItem { Type = STType.Messages, Index = 20, Text = "1pre text disabled" });
            items.Add(new STItem { Type = STType.Messages, Index = 21, Text = "1Reconnecting" });
            items.Add(new STItem { Type = STType.Messages, Index = 22, Text = "1Disconnected" });
            items.Add(new STItem { Type = STType.Messages, Index = 23, Text = "1Unable to connect" });
            items.Add(new STItem { Type = STType.Messages, Index = 24, Text = "1Connected, handshaking" });
            items.Add(new STItem { Type = STType.Messages, Index = 25, Text = "1Connecting to host, please wait" });
            items.Add(new STItem { Type = STType.Messages, Index = 26, Text = "1Sending" });
            items.Add(new STItem { Type = STType.Messages, Index = 27, Text = "1your voice clip was cancelled" });
            items.Add(new STItem { Type = STType.Messages, Index = 28, Text = "1your voice clip has recorded and is now being sent" });

            items.Add(new STItem { Type = STType.SystemTray, Index = 0, Text = "Show as Online" });
            items.Add(new STItem { Type = STType.SystemTray, Index = 1, Text = "Show as Away" });
            items.Add(new STItem { Type = STType.SystemTray, Index = 2, Text = "Exit" });
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
