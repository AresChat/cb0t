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
        }

        public static String Get(STType type, int index)
        {
            return items.Find(x => x.Type == type && x.Index == index).Text;
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



        ScribbleEditor = 40,
        ColorPicker = 41
    }

    class STItem
    {
        public STType Type { get; set; }
        public String Text { get; set; }
        public int Index { get; set; }
    }
}
