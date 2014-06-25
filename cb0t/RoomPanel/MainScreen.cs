using Awesomium.Core;
using Awesomium.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Diagnostics;

namespace cb0t
{
    class MainScreen : WebControl
    {
        public MainScreen(IContainer c)
            : base(c)
        {
            this.ctx = new ContextMenuStrip();
            this.ctx.ShowImageMargin = false;
            this.ctx.ShowCheckMargin = false;
            this.ctx.Items.Add("Save image...");//0
            this.ctx.Items[0].Visible = false;
            this.ctx.Items.Add("Edit...");//1
            this.ctx.Items[1].Visible = false;
            this.ctx.Items.Add("Save voice clip...");//2
            this.ctx.Items[2].Visible = false;
            this.ctx.Items.Add("Export hashlink...");//3
            this.ctx.Items[3].Visible = false;
            this.ctx.Items.Add("Clear screen");//4
            this.ctx.Items.Add("Export text");//5
            this.ctx.Items.Add("Copy to clipboard");//6
            this.ctx.Items.Add("Pause/Unpause screen");//7
            this.ctx.Closed += this.CTXClosed;
            this.ctx.ItemClicked += this.CTXItemClicked;

            this.PendingQueue = new ConcurrentQueue<String>();
            this.PausedQueue = new ConcurrentQueue<String>();
        }

        private ConcurrentQueue<String> PendingQueue { get; set; }
        private ConcurrentQueue<String> PausedQueue { get; set; }
        private ContextMenuStrip ctx { get; set; }

        public void CreateScreen()
        {
            this.ViewIdent = -1;
            this.LoadingFrameComplete += this.ScreenIsReady;
            this.DocumentReady += this.ReadyEvent;
            this.ShowContextMenu += this.DefaultContextMenu;
            String template = Helpers.ScreenHTML;
            template = template.Replace("[font]", Settings.GetReg<String>("global_font", "Tahoma"));
            template = template.Replace("[size]", Settings.GetReg<int>("global_font_size", 10) + "pt;");

            if (this.IsBlack)
                template = template.Replace("FFFFFF", "000000");

            base.LoadHTML(template);
        }

        private bool IsPaused { get; set; }

        private void DownloadedImageReceived(DownloadedImagedReceivedEventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<DownloadedImagedReceivedEventArgs>(this.DownloadedImageReceived), e);
            else
            {
                if (e.ImageBytes == null)
                    return;

                if (!e.Save)
                    this.EditScribbleClicked(e.ImageBytes, EventArgs.Empty);
                else
                {
                    SharedUI.SaveFile.Filter = "Image|*.png";
                    SharedUI.SaveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    SharedUI.SaveFile.FileName = String.Empty;

                    if (SharedUI.SaveFile.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.WriteAllBytes(SharedUI.SaveFile.FileName, e.ImageBytes);
                        }
                        catch { }
                    }
                }
            }
        }

        private void CTXItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(this.ctx.Items[0])) // save image
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getRightClickMenuData()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    if (!String.IsNullOrEmpty(str))
                    {
                        this.ctx.Close();
                        SharedUI.ScribbleDownloader.DownloadImage(this, str, this.DownloadedImageReceived, true);
                    }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[1])) // edit image
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getRightClickMenuData()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    if (!String.IsNullOrEmpty(str))
                    {
                        this.ctx.Close();
                        SharedUI.ScribbleDownloader.DownloadImage(this, str, this.DownloadedImageReceived, false);
                    }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[2])) // save voice clip
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getRightClickMenuData()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    if (str.StartsWith("http://voice.clip/?i="))
                    {
                        str = str.Substring(21);
                        uint vc_finder;

                        if (uint.TryParse(str, out vc_finder))
                        {
                            VoicePlayerItem vc_item = VoicePlayer.Records.Find(x => x.ShortCut == vc_finder);

                            if (vc_item != null)
                            {
                                this.ctx.Hide();
                                SharedUI.SaveFile.Filter = "Wav|*.wav";
                                SharedUI.SaveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                                SharedUI.SaveFile.FileName = String.Empty;

                                if (SharedUI.SaveFile.ShowDialog() == DialogResult.OK)
                                {
                                    try
                                    {
                                        String org_path = Path.Combine(Settings.VoicePath, vc_item.FileName + ".wav");
                                        File.Copy(org_path, SharedUI.SaveFile.FileName);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[3])) // export hashlink
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getRightClickMenuData()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    if (str.StartsWith("http://hashlink.link/?h="))
                    {
                        str = str.Substring(24);
                        StringBuilder sb = new StringBuilder();
                        DecryptedHashlink dh = Hashlink.DecodeHashlink(str);


                        if (dh != null)
                        {
                            sb.AppendLine(dh.Name);
                            sb.Append("arlnk://");

                            sb.AppendLine(Hashlink.EncodeHashlink(new Redirect
                            {
                                Hashlink = str,
                                IP = dh.IP,
                                Name = dh.Name,
                                Port = dh.Port
                            }));

                            try
                            {
                                File.WriteAllText(Settings.DataPath + "hashlink.txt", sb.ToString());
                                Process.Start("notepad.exe", Settings.DataPath + "hashlink.txt");
                            }
                            catch { }
                        }
                    }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[4])) // clear screen
            {
                try { base.ExecuteJavascript("clearScreen()"); }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[5])) // export text
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getExportText()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    try
                    {
                        File.WriteAllText(Settings.DataPath + "export.txt", str);
                        Process.Start("notepad.exe", Settings.DataPath + "export.txt");
                    }
                    catch { }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[6])) // copy to clipboard
            {
                JSValue val = JSValue.Undefined;

                try { val = base.ExecuteJavascriptWithResult("getClipboardText()"); }
                catch { }

                if (!val.IsUndefined)
                {
                    String str = val.ToString();

                    try { Clipboard.SetText(str); }
                    catch { }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[7])) // pause/unpause
            {
                if (this.IsPaused)
                {
                    this.IsPaused = false;
                    String[] paused_items = this.PausedQueue.ToArray();

                    foreach (String str in paused_items)
                        try { base.ExecuteJavascript(str); }
                        catch { }

                    this.PausedQueue = new ConcurrentQueue<String>();
                    this.ShowAnnounceText((this.IsBlack ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.OutBox, 6));
                }
                else
                {
                    this.ShowAnnounceText((this.IsBlack ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.OutBox, 7));
                    this.IsPaused = true;
                }
            }
        }

        private void CTXClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            try { base.ExecuteJavascript("closeRightClickMenu()"); }
            catch { }
        }

        private void DefaultContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            e.SuppressKeyPress = e.KeyData != (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C);
        }

        public void OnLinkClicked(System.Windows.Forms.LinkClickedEventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<System.Windows.Forms.LinkClickedEventArgs>(this.OnLinkClicked), e);
            else
            {
                if (e.LinkText.StartsWith("http://hashlink.script/?h="))
                {
                    String str = e.LinkText.Substring(26);

                    if (!String.IsNullOrEmpty(str))
                        Scripting.ScriptManager.InstallScript(str);
                }
                else if (e.LinkText.StartsWith("http://hashlink.link/?h="))
                {
                    DecryptedHashlink hashlink = Hashlink.DecodeHashlink(e.LinkText.Substring(24));

                    if (hashlink != null)
                        this.HashlinkClicked(hashlink, EventArgs.Empty);
                }
                else if (e.LinkText.StartsWith("http://voice.clip/?i="))
                {
                    String str = e.LinkText.Substring(21);
                    uint vc_finder = 0;

                    if (uint.TryParse(str, out vc_finder))
                    {
                        VoicePlayerItem vc = VoicePlayer.Records.Find(x => x.ShortCut == vc_finder);

                        if (vc != null)
                        {
                            vc.Auto = false;
                            VoicePlayer.QueueItem(vc);
                        }
                    }
                }
                else
                {
                    String check = e.LinkText.ToUpper();

                    if (check.StartsWith("HTTP://") || check.StartsWith("HTTPS://"))
                    {
                        Scripting.JSOutboundTextItem cb = new Scripting.JSOutboundTextItem();
                        cb.Type = Scripting.JSOutboundTextItemType.Link;
                        cb.Text = e.LinkText;
                        cb.EndPoint = this.EndPoint;
                        Scripting.ScriptManager.PendingUIText.Enqueue(cb);
                    }
                }
            }
        }

        private bool IsScreenReady { get; set; }
        public int ViewIdent { get; set; }
        private int ready_state = 0;

        private void ScreenIsReady(object sender, Awesomium.Core.FrameEventArgs e)
        {
            this.LoadingFrameComplete -= this.ScreenIsReady;
            this.ready_state++;

            if (this.ready_state == 2)
                this.GoScreen();
        }

        private void ReadyEvent(object sender, UrlEventArgs e)
        {
            this.DocumentReady -= this.ReadyEvent;
            this.ready_state++;

            if (this.ready_state == 2)
                this.GoScreen();
        }

        private void GoScreen()
        {
            this.ViewIdent = this.Identifier;

            JSObject jsobject = base.CreateGlobalJavascriptObject("cb0t");
            jsobject.Bind("callbackMouseClick", false, this.JSMouseClicked);
            jsobject.Bind("callbackCopyRequested", false, this.JSCopyRequested);
            jsobject.Bind("callback", false, this.JSCallback);
            
            this.IsScreenReady = true;

            String[] pending = this.PendingQueue.ToArray();

            foreach (String str in pending)
                try { base.ExecuteJavascript(str); }
                catch { }

            this.PendingQueue = new ConcurrentQueue<String>();
        }

        private void JSCallback(object sender, JavascriptMethodEventArgs args)
        {
            if (args.Arguments.Length >= 2)
            {
                String arg1 = args.Arguments[0].ToString();
                String arg2 = args.Arguments[1].ToString();

                Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                {
                    EndPoint = this.EndPoint,
                    Name = arg1,
                    Text = arg2,
                    Type = Scripting.JSOutboundTextItemType.ChatScreenCallback
                });
            }
        }

        private void JSCopyRequested(object sender, JavascriptMethodEventArgs args)
        {
            String str = args.Arguments[0].ToString();

            if (!String.IsNullOrEmpty(str))
                try { Clipboard.SetText(str); }
                catch { }
        }

        private void JSMouseClicked(object sender, JavascriptMethodEventArgs args)
        {
            MainScreenButton button = (MainScreenButton)int.Parse(args.Arguments[0].ToString());

            switch (button)
            {
                case MainScreenButton.Middle:
                    this.GetUserNameFromMousePos();
                    break;

                case MainScreenButton.Right:
                    this.OpenRightClickMenu();
                    break;
            }
        }

        private void OpenRightClickMenu()
        {
            JSValue val = JSValue.Undefined;

            try { val = base.ExecuteJavascriptWithResult("openRightClickMenu()"); }
            catch { }

            String options = val.ToString();
            this.ctx.Items[3].Visible = options[0] == '1';
            this.ctx.Items[0].Visible = options[1] == '1';
            this.ctx.Items[1].Visible = options[1] == '1';
            this.ctx.Items[2].Visible = options[2] == '1';
            this.ctx.Show(MousePosition);
        }

        private void GetUserNameFromMousePos()
        {
            if (!this.IsScreenReady)
                return;

            JSValue val = JSValue.Undefined;

            try { val = base.ExecuteJavascriptWithResult("getUserNameFromMousePos()"); }
            catch { }

            if (val.IsString)
            {
                String str = val.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    if (Settings.GetReg<bool>("can_timestamp", false))
                    {
                        String[] parts = str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                            this.NameClicked(parts[1], EventArgs.Empty);
                    }

                    this.NameClicked(str, EventArgs.Empty);
                }
            }
        }

        public event EventHandler HashlinkClicked;
        public event EventHandler NameClicked;
        public event EventHandler EditScribbleClicked;

        public bool IsMainScreen { get; set; }
        public bool IsBlack { get; set; }
        public IPEndPoint EndPoint { get; set; }
        
        private int cls_count = 0;

        public void UpdateTemplate()
        {
            this.ctx.Items[0].Text = StringTemplate.Get(STType.OutBox, 0) + "...";
            this.ctx.Items[1].Text = StringTemplate.Get(STType.FilterSettings, 1) + "...";
            this.ctx.Items[2].Text = StringTemplate.Get(STType.OutBox, 1) + "...";
            this.ctx.Items[3].Text = StringTemplate.Get(STType.RoomMenu, 2) + "...";
            this.ctx.Items[4].Text = StringTemplate.Get(STType.OutBox, 2);
            this.ctx.Items[5].Text = StringTemplate.Get(STType.OutBox, 3);
            this.ctx.Items[6].Text = StringTemplate.Get(STType.OutBox, 4);
            this.ctx.Items[7].Text = StringTemplate.Get(STType.OutBox, 5);
        }

        public void ScrollDown()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(this.ScrollDown));
            else if (this.IsScreenReady)
                try { base.ExecuteJavascript("scrollDown()"); }
                catch { }
        }

        public void Scribble(object data)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<object>(this.Scribble), data);
            else
            {
                if (data == null)
                    return;

                if (!(data is byte[]))
                    return;

                int height = 0, width = 0;
                Guid type = Guid.Empty;
                byte[] b = (byte[])data;

                try
                {
                    using (MemoryStream stream = new MemoryStream(b))
                    using (Bitmap bmp = new Bitmap(stream))
                    {
                        height = bmp.Height;
                        width = bmp.Width;
                        type = bmp.RawFormat.Guid;
                    }
                }
                catch { }

                if (height > 0 && width > 0)
                {
                    String filename = (Settings.ScribbleIdent++) + this.BitmapToFileExtension(type);
                    File.WriteAllBytes(Path.Combine(Settings.ScribblePath, filename), b);
                    String html = "<img src=\"http://scribble.image/" + filename + "\" style=\"width: " + width + "px; height: " + height + "px; max-width: 420px; max-height: 420px;\" alt=\"\" />";

                    if (!this.IsScreenReady)
                        this.PendingQueue.Enqueue("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")");
                    else if (this.IsPaused)
                        this.PausedQueue.Enqueue("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")");
                    else
                        try { base.ExecuteJavascript("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")"); }
                        catch { }
                }
            }
        }

        public void ShowVoice(String sender, uint sc)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String, uint>(this.ShowVoice), sender, sc);
            else
            {
                this.ShowAnnounceText("\x000314--- " + sender + ":");

                StringBuilder html = new StringBuilder();
                html.Append("<div class=\"vc\">");
                html.Append("<a title=\"\" href=\"http://voice.clip/?i=" + sc + "\" style=\"cursor: pointer;\">");
                html.Append("<img class=\"vc\" alt=\"\" src=\"http://emotic.ui/voice.png\" />");
                html.Append("</a>");
                html.Append("</div>");

                if (!this.IsScreenReady)
                    this.PendingQueue.Enqueue("injectCustomHTML(\"" + html.ToString().Replace("\"", "\\\"") + "\")");
                else if (this.IsPaused)
                    this.PausedQueue.Enqueue("injectCustomHTML(\"" + html.ToString().Replace("\"", "\\\"") + "\")");
                else
                    try { base.ExecuteJavascript("injectCustomHTML(\"" + html.ToString().Replace("\"", "\\\"") + "\")"); }
                    catch { }

                html.Clear();
            }
        }

        private String BitmapToFileExtension(Guid guid)
        {
            if (guid.Equals(ImageFormat.Gif.Guid))
                return ".gif";
            if (guid.Equals(ImageFormat.Icon.Guid))
                return ".ico";
            if (guid.Equals(ImageFormat.Jpeg.Guid))
                return ".jpg";
            if (guid.Equals(ImageFormat.Png.Guid))
                return ".png";

            return ".bmp";
        }

        public void ShowAnnounceText(String text)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String>(this.ShowAnnounceText), text);
            else
            {
                if (text.Replace("\n", "").Replace("\r", "").Length == 0)
                {
                    if (this.cls_count++ > 6)
                        if (Settings.GetReg<bool>("block_cls", false))
                            return;

                    if (this.cls_count > 200)
                        return;

                    if (text.Count(x => x == '\r' || x == '\n') > 20)
                        if (Settings.GetReg<bool>("block_cls", false))
                            return;
                }
                else if (Helpers.StripColors(text).Length <= 2)
                {
                    if (this.cls_count++ > 6)
                        if (Settings.GetReg<bool>("block_cls", false))
                            return;

                    if (this.cls_count > 200)
                        return;
                }
                else this.cls_count = 0;

                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                this.Render(ts ? (Helpers.Timestamp + text) : text, null, true, 4, null);
            }
        }

        public void ShowServerText(String text)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String>(this.ShowServerText), text);
            else
            {
                this.cls_count = 0;
                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                this.Render(ts ? (Helpers.Timestamp + text) : text, null, true, this.IsBlack ? 15 : 2, null);
            }
        }

        public void ShowPublicText(String name, String text, AresFont font)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String, String, AresFont>(this.ShowPublicText), name, text, font);
            else
            {
                this.cls_count = 0;
                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                this.Render(text, ts ? (Helpers.Timestamp + name) : name, true, this.IsBlack ? 0 : 12, font);
            }
        }

        public void ShowEmoteText(String name, String text, AresFont font)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String, String, AresFont>(this.ShowEmoteText), name, text, font);
            else
            {
                this.cls_count = 0;
                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                this.Render((ts ? Helpers.Timestamp : "") + "* " + name + " " + text, null, false, this.IsBlack ? 13 : 6, font);
            }
        }

        public bool IsWideText { get; set; }

        public void ShowCustomHTML(String html)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String>(this.ShowCustomHTML), html);
            else
            {
                if (!this.IsScreenReady)
                    this.PendingQueue.Enqueue("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")");
                else if (this.IsPaused)
                    this.PausedQueue.Enqueue("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")");
                else
                    try { base.ExecuteJavascript("injectCustomHTML(\"" + html.Replace("\"", "\\\"") + "\")"); }
                    catch { }
            }
        }

        public void ShowCustomScript(String src)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String>(this.ShowCustomScript), src);
            else
            {
                if (!this.IsScreenReady)
                    this.PendingQueue.Enqueue("injectScript(\"" + src.Replace("\"", "\\\"") + "\")");
                else if (this.IsPaused)
                    this.PausedQueue.Enqueue("injectScript(\"" + src.Replace("\"", "\\\"") + "\")");
                else
                    try { base.ExecuteJavascript("injectScript(\"" + src.Replace("\"", "\\\"") + "\")"); }
                    catch { }
            }
        }

        private void Render(String txt, String name, bool can_col, int first_col, AresFont _ff)
        {
            String text = txt.Replace("\r\n", "\r").Replace("\n",
                "\r").Replace("", "").Replace("]̽", "").Replace(" ̽",
                "").Replace("͊", "").Replace("]͊", "").Replace("͠",
                "").Replace("̶", "").Replace("̅", "");

            StringBuilder html = new StringBuilder();
            String col_tester;
            AresFont ff = _ff;

            if (!Settings.GetReg<bool>("receive_ppl_fonts", true))
                ff = null;

            if (ff != null)
            {
                col_tester = ff.TextColor.ToUpper();

                if (col_tester == "#000000" && this.IsBlack)
                    ff.TextColor = "#FFFFFF";
                else if (col_tester == "#FFFFFF" && !this.IsBlack)
                    ff.TextColor = "#000000";
            }

            bool bold = false,
                 italic = false,
                 underline = false,
                 bg_code_used = false;
            
            bool can_emoticon = Settings.GetReg<bool>("can_emoticon", true);
            int emote_count = 0;
            String fore, back, tmp, link;
            bool can_link = true;
            int itmp;

            if (ff == null)
                fore = AresColorToHTMLColor(first_col);
            else
                fore = ff.TextColor;

            back = this.IsBlack ? "#000000" : "#FFFFFF";
            html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));

            char[] letters = text.ToCharArray();
            bool special_space = false;

            for (int i = 0; i < letters.Length; i++)
            {
                switch (letters[i])
                {
                    case '\x0006':
                        bold = !bold;
                        html.Append("</span>");
                        html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                        break;

                    case '\x0007':
                        underline = !underline;
                        html.Append("</span>");
                        html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                        break;

                    case '\x0009':
                        italic = !italic;
                        html.Append("</span>");
                        html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                        break;

                    case '\x0003':
                        if (letters.Length >= (i + 8))
                        {
                            tmp = text.Substring((i + 1), 7);

                            if (Helpers.IsHexCode(tmp))
                            {
                                if (!bg_code_used)
                                {
                                    if (this.IsBlack && tmp == "#000000")
                                        tmp = "#FFFFFF";
                                    else if (!this.IsBlack && tmp.ToUpper() == "#FFFFFF")
                                        tmp = "#000000";
                                }

                                fore = tmp;
                                html.Append("</span>");
                                html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                                i += 7;
                                break;
                            }
                        }

                        if (letters.Length >= (i + 3))
                        {
                            tmp = text.Substring((i + 1), 2);

                            if (int.TryParse(tmp, out itmp))
                            {
                                if (!bg_code_used)
                                {
                                    if (this.IsBlack && itmp == 1)
                                        itmp = 0;
                                    else if (!this.IsBlack && itmp == 0)
                                        itmp = 1;
                                }

                                fore = this.AresColorToHTMLColor(itmp);
                                html.Append("</span>");
                                html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                                i += 2;
                                break;
                            }
                        }
                        else can_link = false;
                        goto default;

                    case '\x0005':
                        if (letters.Length >= (i + 8))
                        {
                            tmp = text.Substring((i + 1), 7);

                            if (Helpers.IsHexCode(tmp))
                            {
                                back = tmp;
                                html.Append("</span>");
                                html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                                bg_code_used = true;
                                i += 7;
                                break;
                            }
                        }

                        if (letters.Length >= (i + 3))
                        {
                            tmp = text.Substring((i + 1), 2);

                            if (int.TryParse(tmp, out itmp))
                            {
                                back = this.AresColorToHTMLColor(itmp);
                                html.Append("</span>");
                                html.Append(this.BuildFontStyle(fore, back, bold, italic, underline));
                                bg_code_used = true;
                                i += 2;
                                break;
                            }
                        }
                        else can_link = false;
                        goto default;

                    case '(':
                    case ':':
                    case ';':
                        can_link = false;

                        if (can_emoticon)
                        {
                            Emotic em = Emoticons.FindEmoticon(text.ToString().Substring(i).ToUpper());

                            if (em != null)
                            {
                                if (emote_count++ < 8)
                                {
                                    html.Append("<img style=\"height: 19px; margin-bottom: -1px;\" alt=\"\" src=\"http://emotic.org/" + em.Index + ".gif\" />");
                                    i += (em.Shortcut.Length - 1);
                                    break;
                                }
                                else goto default;
                            }

                            em = this.FindNewEmoticon(text.ToString().Substring(i).ToUpper());

                            if (em != null)
                            {
                                if (emote_count++ < 8)
                                {
                                    itmp = Emoticons.GetExEmoticonHeight(em.Index);
                                    tmp = em.Shortcut.Substring(1, em.Shortcut.Length - 2).ToLower();
                                    html.Append("<img style=\"height: " + itmp + "px; margin-bottom: -1px;\" alt=\"\" src=\"http://emotic.ext/" + tmp + ".gif\" />");
                                    i += (em.Shortcut.Length - 1);
                                    break;
                                }
                                else goto default;
                            }
                        }
                        goto default;

                    case 'w':
                    case 'h':
                    case 'a':
                    case '\\':
                        tmp = text.Substring(i);

                        if (can_link)
                            if (tmp.IndexOf("http://") == 0 ||
                                tmp.IndexOf("https://") == 0 ||
                                tmp.IndexOf("www.") == 0 ||
                                tmp.IndexOf("arlnk://") == 0 ||
                                tmp.IndexOf("\\\\arlnk://") == 0 ||
                                tmp.IndexOf("\\\\cb0t://script/?file=") == 0)
                            {
                                int _end = tmp.IndexOf(" ");

                                if (_end > -1)
                                    tmp = tmp.Substring(0, _end);

                                i += (tmp.Length - 1);

                                link = "";
                                String pre_link = "";

                                if (tmp.IndexOf("\\\\cb0t://script/?file=") == 0)
                                {
                                    String lnk_text = "";

                                    for (int a = 0; a < tmp.Length; a++)
                                    {
                                        lnk_text += ("&#" + ((int)tmp[a]) + ";");

                                        if (a >= 2)
                                            pre_link += ("&#" + ((int)tmp[a]) + ";");
                                    }

                                    tmp = tmp.Substring(22);

                                    if (tmp.Length > 0)
                                    {
                                        tmp = "http://hashlink.script/?h=" + tmp;

                                        for (int a = 0; a < tmp.Length; a++)
                                            link += ("&#" + ((int)tmp[a]) + ";");

                                        html.Append("<a href=\"" + link + "\" title=\"" + pre_link + "\" style=\"text-decoration: underline; cursor: pointer;\">" + lnk_text + "</a>");
                                        break;
                                    }
                                }
                                else if (tmp.IndexOf("arlnk://") == 0 || tmp.IndexOf("\\\\arlnk://") == 0)
                                {
                                    String hash_str = tmp;

                                    if (hash_str.StartsWith("arlnk://"))
                                        hash_str = hash_str.Substring(8);

                                    if (hash_str.StartsWith("\\\\arlnk://"))
                                        hash_str = hash_str.Substring(10);

                                    DecryptedHashlink dec_hash = Hashlink.DecodeHashlink(hash_str);

                                    if (dec_hash == null)
                                        break;

                                    tmp = "arlnk://" + hash_str;
                                    String hash_title = String.Empty;

                                    for (int a = 0; a < tmp.Length; a++)
                                        hash_title += ("&#" + ((int)tmp[a]) + ";");

                                    tmp = "http://hashlink.link/?h=" + hash_str;
                                    String hash_href = String.Empty;

                                    for (int a = 0; a < tmp.Length; a++)
                                        hash_href += ("&#" + ((int)tmp[a]) + ";");

                                    tmp = dec_hash.Name;
                                    String hash_inner = String.Empty;

                                    for (int a = 0; a < tmp.Length; a++)
                                        hash_inner += ("&#" + ((int)tmp[a]) + ";");

                                    String hash_img = "<img style=\"margin-bottom: -1px; border-style: none; height: 24px;\" src=\"http://emotic.ui/hashlink.png\" alt=\"\" />";
                                    html.Append("<a href=\"" + hash_href + "\" title=\"" + hash_title + "\" style=\"text-decoration: underline; cursor: pointer;\">" + hash_img + hash_inner + "</a>");
                                    break;
                                }
                                else
                                {
                                    String enclink = tmp;

                                    if (enclink.IndexOf("www.") == 0)
                                        enclink = "http://" + enclink;

                                    StringBuilder elink = new StringBuilder();
                                    byte[] elbytes = Encoding.UTF8.GetBytes(enclink);

                                    foreach (byte c in enclink)
                                        elink.AppendFormat("{0:X2}", c);

                                    elink.Insert(0, "http://external.link/");

                                    for (int a = 0; a < tmp.Length; a++)
                                        link += ("&#" + ((int)tmp[a]) + ";");

                                    html.Append("<a href=\"" + elink.ToString() + "\" title=\"" + link + "\" style=\"text-decoration: underline; cursor: pointer;\">" + link + "</a>");
                                    elink.Clear();
                                    break;
                                }
                            }

                        can_link = false;
                        html.Append("&#" + ((int)letters[i]) + ";");
                        break;

                    case ' ':
                        can_link = true;

                        if (special_space)
                        {
                            html.Append("&nbsp;");
                            special_space = false;
                        }
                        else
                        {
                            html.Append("&#" + ((int)letters[i]) + ";");
                            special_space = true;
                        }
                        break;

                    default:
                        can_link = false;
                        special_space = false;
                        html.Append("&#" + ((int)letters[i]) + ";");
                        break;
                }
            }

            html.Append("</span>");

            if (!String.IsNullOrEmpty(name))
            {
                StringBuilder name_builder = new StringBuilder();

                if (ff != null)
                {
                    col_tester = ff.NameColor.ToUpper();

                    if (col_tester == "#000000" && this.IsBlack)
                        ff.NameColor = "#FFFF00";
                    else if (col_tester == "#FFFFFF" && !this.IsBlack)
                        ff.NameColor = "#000000";
                }

                if (ff == null)
                    fore = this.IsBlack ? "#FFFF00" : "#000000";
                else
                    fore = ff.NameColor;

                back = this.IsBlack ? "#000000" : "#FFFFFF";
                name_builder.Append(this.BuildFontStyle(fore, back, false, false, false));

                char[] name_chrs = (name + "> ").ToCharArray();

                for (int i = 0; i < name_chrs.Length; i++)
                    if (name_chrs[i] == ' ' && i < (name_chrs.Length - 1))
                        name_builder.Append("&nbsp;");
                    else
                        name_builder.Append("&#" + ((int)name_chrs[i]) + ";");

                name_builder.Append("</span>");
                html.Insert(0, name_builder.ToString());
                name_builder.Clear();
            }

            if (ff == null)
            {
                StringBuilder font_style = new StringBuilder();
                font_style.Append("<span style=\"");
                font_style.Append("font-family: " + Settings.GetReg<String>("global_font", "Tahoma") + ";");
                font_style.Append("font-size: " + Settings.GetReg<int>("global_font_size", 10) + "pt;");
                font_style.Append("\">");
                html.Insert(0, font_style.ToString());
                html.Append("</span>");
                font_style.Clear();
            }
            else
            {
                StringBuilder font_style = new StringBuilder();
                font_style.Append("<span style=\"");
                font_style.Append("font-family: " + ff.FontName + ";");
                int org_size = Settings.GetReg<int>("global_font_size", 10);
                int difference = (org_size - 10);
                int user_size = ff.Size + difference;
                font_style.Append("font-size: " + user_size + "pt;");
                font_style.Append("\">");
                html.Insert(0, font_style.ToString());
                html.Append("</span>");
                font_style.Clear();
            }

            if (!this.IsScreenReady)
                this.PendingQueue.Enqueue("injectText(\"" + html.ToString().Replace("\"", "\\\"") + "\", " + (this.IsWideText ? "true" : "false") + ")");
            else if (this.IsPaused)
                this.PausedQueue.Enqueue("injectText(\"" + html.ToString().Replace("\"", "\\\"") + "\", " + (this.IsWideText ? "true" : "false") + ")");
            else
                try { base.ExecuteJavascript("injectText(\"" + html.ToString().Replace("\"", "\\\"") + "\", " + (this.IsWideText ? "true" : "false") + ")"); }
                catch { }

            html.Clear();
        }

        private String AresColorToHTMLColor(int c)
        {
            switch (c)
            {
                case 1: return "#000000";
                case 0: return "#FFFFFF";
                case 8: return "#FFFF00";
                case 11: return "#00FFFF";
                case 12: return "#0000FF";
                case 2: return "#000080";
                case 6: return "#800080";
                case 9: return "#00FF00";
                case 13: return "#FF00FF";
                case 14: return "#808080";
                case 15: return "#C0C0C0";
                case 7: return "#FFA500";
                case 5: return "#800000";
                case 10: return "#008080";
                case 3: return "#008000";
                case 4: return "#FF0000";
                default: return "#FFFFFF";
            }
        }

        private Emotic FindNewEmoticon(String str)
        {
            for (int i = 0; i < Emoticons.ex_emotic.Length; i++)
                if (str.StartsWith("(" + Emoticons.ex_emotic[i].ShortcutText + ")"))
                    return new Emotic { Index = i, Shortcut = "(" + Emoticons.ex_emotic[i].ShortcutText + ")" };

            return null;
        }

        private String BuildFontStyle(String fc, String bc, bool b, bool i, bool u)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<span style=\"");
            sb.Append("color: " + fc + ";");
            sb.Append("background-color: " + bc + ";");

            if (b)
                sb.Append("font-weight: bold;");

            if (i)
                sb.Append("font-style: italic;");

            if (u)
                sb.Append("text-decoration: underline;");

            sb.Append("\">");
            return sb.ToString();
        }

        public void Free()
        {
            this.IsScreenReady = false;
            this.ShowContextMenu -= this.DefaultContextMenu;
            this.PausedQueue = new ConcurrentQueue<String>();
            this.PendingQueue = new ConcurrentQueue<String>();
            this.ctx.Closed -= this.CTXClosed;
            this.ctx.ItemClicked -= this.CTXItemClicked;

            while (this.ctx.Items.Count > 0)
                this.ctx.Items[0].Dispose();

            this.ctx.Dispose();
            this.ctx = null;
        }
    }

    enum MainScreenButton
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }
}
