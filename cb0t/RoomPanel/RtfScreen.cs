using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class RtfScreen : RichTextBox
    {
        private byte[] img_data { get; set; }
        private uint vc_sc { get; set; }
        private ContextMenuStrip ctx { get; set; }
        private List<PausedItem> paused_items = new List<PausedItem>();
        private bool IsPaused { get; set; }

        public event EventHandler HashlinkClicked;
        public event EventHandler NameClicked;
        public event EventHandler EditScribbleClicked;

        public bool IsMainScreen { get; set; }
        public bool IsBlack { get; set; }

        protected override void OnLinkClicked(LinkClickedEventArgs e)
        {
            base.OnLinkClicked(e);

            if (e.LinkText.StartsWith("\\\\arlnk://"))
            {
                DecryptedHashlink hashlink = Hashlink.DecodeHashlink(e.LinkText.Substring(10));

                if (hashlink != null)
                    this.HashlinkClicked(hashlink, EventArgs.Empty);
            }
            else if (e.LinkText.StartsWith("\\\\cb0t://script/?file="))
            {
                String str = e.LinkText.Substring(22);

                if (!String.IsNullOrEmpty(str))
                    Scripting.ScriptManager.InstallScript(str);
            }
            else if (e.LinkText.StartsWith("\\\\voice_clip_#"))
            {
                String vc_check = e.LinkText.Substring(14);
                uint vc_finder = 0;

                if (uint.TryParse(vc_check, out vc_finder))
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

                if (check.StartsWith("HTTP://") || check.StartsWith("HTTPS://") || check.StartsWith("WWW."))
                {
                    Scripting.JSOutboundTextItem cb = new Scripting.JSOutboundTextItem();
                    cb.Type = Scripting.JSOutboundTextItemType.Link;
                    cb.Text = e.LinkText;
                    cb.EndPoint = this.EndPoint;
                    Scripting.ScriptManager.PendingUIText.Enqueue(cb);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = e.KeyData != (Keys.Control | Keys.C);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int char_index = this.GetCharIndexFromPosition(e.Location);
                int line = -1;
                int _start = 0;
                String[] _split = this.Text.Split(new String[] { "\n" }, StringSplitOptions.None);

                for (int i = 0; i < _split.Length; i++)
                {
                    if (char_index >= _start && char_index <= (_start + _split[i].Length))
                    {
                        line = i;
                        break;
                    }

                    _start += (_split[i].Length + 1);
                }

                if (line > -1 && line < this.Lines.Length)
                {
                    String[] rtf_lines = this.Rtf.Split(new String[] { "\\par" }, StringSplitOptions.None);
                    int img_line = line;

                    if (img_line == (this.Lines.Length - 1))
                        img_line++;

                    if ((img_line + 1) < rtf_lines.Length)
                    {
                        String rtf = rtf_lines[img_line + 1];
                        int first_cf = rtf.IndexOf("\\cf");
                        int is_img = -1;

                        if ((first_cf + 4) < rtf.Length)
                            int.TryParse(rtf.Substring(first_cf + 3, 1), out is_img);

                        if (is_img == 0)
                            if (rtf.Contains("{\\pict") && rtf.Contains("\\picw") && rtf.Contains("\\pich"))
                            {
                                float dx = -1, dy = -1;

                                using (Graphics g = this.CreateGraphics())
                                {
                                    dx = g.DpiX;
                                    dy = g.DpiY;
                                }

                                Size size = Helpers.GetScribbleSize(rtf, dx, dy);

                                if (!size.IsEmpty)
                                {
                                    byte[] data = Helpers.GetScribbleBytesFromRTF(rtf);

                                    if (data != null)
                                        this.img_data = Helpers.GetPngBytesFromScribbleBytes(data, size);
                                }
                            }
                    }

                    String vc_check = this.Lines[line];

                    if (vc_check.Contains("\\\\voice_clip_#"))
                    {
                        vc_check = vc_check.Substring(vc_check.IndexOf("\\\\voice_clip_#") + 14);

                        if (vc_check.Contains(" "))
                        {
                            vc_check = vc_check.Substring(0, vc_check.IndexOf(" "));
                            uint vc_finder = 0;

                            if (uint.TryParse(vc_check, out vc_finder))
                                this.vc_sc = vc_finder;
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                int char_index = this.GetCharIndexFromPosition(e.Location);

                if (char_index <= 0)
                    return;

                String[] _split = this.Text.Split(new String[] { "\n" }, StringSplitOptions.None);

                int _start = 0;

                for (int i = 0; i < _split.Length; i++)
                {
                    if (char_index >= _start && char_index <= (_start + _split[i].Length))
                    {
                        String text = _split[i];

                        if (Settings.GetReg<bool>("can_timestamp", false))
                        {
                            int first_space = text.IndexOf(" ");

                            if (first_space == -1)
                                return;

                            text = text.Substring(first_space + 1);
                        }

                        int _gt = text.IndexOf(">");
                        int star = text.IndexOf("* ");

                        if (star == 0)
                        {
                            text = text.Substring(2);

                            int first_space = text.IndexOf(" ");

                            if (first_space < 2 || first_space > 20)
                                return;

                            text = text.Substring(0, first_space);
                            this.NameClicked(text, EventArgs.Empty);
                        }
                        else
                        {
                            if (_gt >= 2 && _gt <= 20)
                            {
                                text = text.Substring(0, _gt);
                                this.NameClicked(text, EventArgs.Empty);
                            }
                        }

                        return;
                    }

                    _start += (_split[i].Length + 1);
                }
            }

            base.OnMouseDown(e);
        }

        public void Free()
        {
            this.paused_items.Clear();
            this.paused_items = null;
            this.ContextMenuStrip = null;
            this.ctx.Opening -= this.CTXOpening;
            this.ctx.Closed -= this.CTXClosed;
            this.ctx.ItemClicked -= this.CTXItemClicked;

            while (this.ctx.Items.Count > 0)
                this.ctx.Items[0].Dispose();

            this.ctx.Dispose();
            this.ctx = null;
            this.Clear();

            while (this.CanUndo)
                this.ClearUndo();
        }

        public IPEndPoint EndPoint { get; set; }

        public RtfScreen(IPEndPoint ep)
        {
            this.EndPoint = ep;
            this.BackColor = Color.White;
            this.HideSelection = false;
            this.DetectUrls = true;
            this.ctx = new ContextMenuStrip();
            this.ctx.ShowImageMargin = false;
            this.ctx.ShowCheckMargin = false;
            this.ctx.Items.Add("Save image...");//0
            this.ctx.Items[0].Visible = false;
            this.ctx.Items.Add("Edit...");//1
            this.ctx.Items[1].Visible = false;
            this.ctx.Items.Add("Save voice clip...");//2
            this.ctx.Items[2].Visible = false;
            this.ctx.Items.Add("Clear screen");//3
            this.ctx.Items.Add("Export text");//4
            this.ctx.Items.Add("Copy to clipboard");//5
            this.ctx.Items.Add("Pause/Unpause screen");//6
            this.ctx.Opening += this.CTXOpening;
            this.ctx.Closed += this.CTXClosed;
            this.ctx.ItemClicked += this.CTXItemClicked;
            this.ContextMenuStrip = this.ctx;
            this.paused_items.Clear();
            this.paused_items = new List<PausedItem>();
            this.UpdateTemplate();
        }

        public RtfScreen()
        {
            this.BackColor = Color.White;
            this.HideSelection = false;
            this.DetectUrls = true;
            this.ctx = new ContextMenuStrip();
            this.ctx.ShowImageMargin = false;
            this.ctx.ShowCheckMargin = false;
            this.ctx.Items.Add("Save image...");//0
            this.ctx.Items[0].Visible = false;
            this.ctx.Items.Add("Edit...");//1
            this.ctx.Items[1].Visible = false;
            this.ctx.Items.Add("Save voice clip...");//2
            this.ctx.Items[2].Visible = false;
            this.ctx.Items.Add("Clear screen");//3
            this.ctx.Items.Add("Export text");//4
            this.ctx.Items.Add("Copy to clipboard");//5
            this.ctx.Items.Add("Pause/Unpause screen");//6
            this.ctx.Opening += this.CTXOpening;
            this.ctx.Closed += this.CTXClosed;
            this.ctx.ItemClicked += this.CTXItemClicked;
            this.ContextMenuStrip = this.ctx;
            this.paused_items.Clear();
            this.paused_items = new List<PausedItem>();
            this.UpdateTemplate();
        }

        public void UpdateTemplate()
        {
            this.ctx.Items[0].Text = StringTemplate.Get(STType.OutBox, 0) + "...";
            this.ctx.Items[1].Text = StringTemplate.Get(STType.FilterSettings, 1) + "...";
            this.ctx.Items[2].Text = StringTemplate.Get(STType.OutBox, 1) + "...";
            this.ctx.Items[3].Text = StringTemplate.Get(STType.OutBox, 2);
            this.ctx.Items[4].Text = StringTemplate.Get(STType.OutBox, 3);
            this.ctx.Items[5].Text = StringTemplate.Get(STType.OutBox, 4);
            this.ctx.Items[6].Text = StringTemplate.Get(STType.OutBox, 5);
        }

        private void CTXItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(this.ctx.Items[0]))
            {
                if (this.img_data != null)
                {
                    byte[] tmp = this.img_data;
                    this.ctx.Hide();
                    SharedUI.SaveFile.Filter = "Image|*.png";
                    SharedUI.SaveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    SharedUI.SaveFile.FileName = String.Empty;

                    if (SharedUI.SaveFile.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.WriteAllBytes(SharedUI.SaveFile.FileName, tmp);
                        }
                        catch { }
                    }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[1]))
            {
                if (this.img_data != null)
                {
                    byte[] tmp = this.img_data;
                    this.ctx.Hide();
                    this.EditScribbleClicked(tmp, EventArgs.Empty);
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[2]))
            {
                VoicePlayerItem item = VoicePlayer.Records.Find(x => x.ShortCut == this.vc_sc);
                this.ctx.Hide();
                SharedUI.SaveFile.Filter = "Wav|*.wav";
                SharedUI.SaveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                SharedUI.SaveFile.FileName = String.Empty;

                if (SharedUI.SaveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (item != null)
                        {
                            String org_path = Path.Combine(Settings.VoicePath, item.FileName + ".wav");
                            File.Copy(org_path, SharedUI.SaveFile.FileName);
                        }
                    }
                    catch { }
                }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[3]))
            {
                this.Clear();

                while (this.CanUndo)
                    this.ClearUndo();
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[4]))
            {
                try
                {
                    File.WriteAllLines(Settings.DataPath + "export.txt", this.Lines);
                    Process.Start("notepad.exe", Settings.DataPath + "export.txt");
                }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[5]))
            {
                try
                {
                    if (this.SelectionLength == 0)
                        Clipboard.SetText(this.Text);
                    else
                        Clipboard.SetText(this.Text.Substring(this.SelectionStart, this.SelectionLength));
                }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[6]))
            {
                if (this.IsPaused)
                {
                    this.IsPaused = false;

                    while (this.paused_items.Count > 0)
                    {
                        PausedItem item = this.paused_items[0];
                        this.paused_items.RemoveAt(0);

                        switch (item.Type)
                        {
                            case PausedItemType.Announce:
                                this.ShowAnnounceText(item.Text);
                                break;

                            case PausedItemType.Emote:
                                this.ShowEmoteText(item.Name, item.Text, null);
                                break;

                            case PausedItemType.PM:
                                this.ShowPMText(item.Name, item.Text, null);
                                break;

                            case PausedItemType.Public:
                                this.ShowPublicText(item.Name, item.Text, null);
                                break;

                            case PausedItemType.Server:
                                this.ShowServerText(item.Text);
                                break;
                        }
                    }

                    this.paused_items = new List<PausedItem>();
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
            this.img_data = new byte[] { };
            this.img_data = null;
            this.vc_sc = 0;
        }

        private void CTXOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ctx.Items[0].Visible = this.img_data != null;
            this.ctx.Items[1].Visible = this.img_data != null;
            this.ctx.Items[2].Visible = this.vc_sc > 0;
        }        

        public void ScrollDown()
        {
            if (!this.IsHandleCreated)
                return;

            this.BeginInvoke((Action)(() => SendMessage(this.Handle, 277, 7, IntPtr.Zero)));
        }

        private int cls_count = 0;

        public void Scribble(byte[] data)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<byte[]>(this.Scribble), data);
            else
            {
                if (this.IsPaused)
                    return;

                StringBuilder rtf = new StringBuilder();
                rtf.Append("{");
                rtf.Append("\\rtf");
                rtf.Append("\\par");

                using (Graphics g = this.CreateGraphics())
                    rtf.Append(Emoticons.GetRTFScribble(data, g));

                rtf.Append("}");

                this.SelectionLength = 0;
                this.SelectionStart = this.Text.Length;
                this.TrimLines();
                this.SelectedRtf = rtf.ToString();
                this.SelectionLength = 0;
                this.SelectionStart = this.Text.Length;

                rtf = null;
            }
        }

        public void ShowPMText(String name, String text, AresFont font)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String, String, AresFont>(this.ShowPMText), name, text, font);
            else
            {
                if (this.IsPaused)
                {
                    this.paused_items.Add(new PausedItem { Type = PausedItemType.PM, Name = name, Text = text });
                    return;
                }

                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                AresFont name_font = null;

                if (font != null)
                {
                    name_font = new AresFont();
                    name_font.FontName = font.FontName;
                    name_font.NameColor = font.NameColor;
                    name_font.TextColor = font.NameColor;
                    name_font.Size = font.Size;
                }

                this.Render((ts ? (Helpers.Timestamp + name) : name) + ":", null, true, this.IsBlack ? 0 : 1, name_font);
                this.Render("    " + text, null, true, this.IsBlack ? 0 : 1, font);
            }
        }

        public void ShowAnnounceText(String text)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<String>(this.ShowAnnounceText), text);
            else
            {
                if (this.IsPaused)
                {
                    this.paused_items.Add(new PausedItem { Type = PausedItemType.Announce, Text = text });
                    return;
                }

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
                if (this.IsPaused)
                {
                    this.paused_items.Add(new PausedItem { Type = PausedItemType.Server, Text = text });
                    return;
                }

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
                if (this.IsPaused)
                {
                    this.paused_items.Add(new PausedItem { Type = PausedItemType.Public, Name = name, Text = text });
                    return;
                }

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
                if (this.IsPaused)
                {
                    this.paused_items.Add(new PausedItem { Type = PausedItemType.Emote, Name = name, Text = text });
                    return;
                }

                this.cls_count = 0;
                bool ts = Settings.GetReg<bool>("can_timestamp", false);
                this.Render((ts ? Helpers.Timestamp : "") + "* " + name + " " + text, null, false, this.IsBlack ? 13 : 6, font);
            }
        }

        private class EmItem
        {
            public String Name { get; set; }
            public String RTF { get; set; }
        }

        private int GetColorIndex(ref List<Color> list, Color col)
        {
            return list.FindIndex(x => x.R == col.R &&
                                       x.G == col.G &&
                                       x.B == col.B);
        }

        private String ColorsToRTFColorTable(Color[] cols)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Color c in cols)
            {
                sb.Append("\\red" + c.R);
                sb.Append("\\green" + c.G);
                sb.Append("\\blue" + c.B + ";");
            }

            return "{\\colortbl;" + sb + "}";
        }

        private Color GetColorFromCode(int code)
        {
            switch (code)
            {
                case 1: return Color.Black;
                case 2: return Color.Navy;
                case 3: return Color.Green;
                case 4: return Color.Red;
                case 5: return Color.Maroon;
                case 6: return Color.Purple;
                case 7: return Color.Orange;
                case 8: return Color.Yellow;
                case 9: return Color.Lime;
                case 10: return Color.Teal;
                case 11: return Color.Aqua;
                case 12: return Color.Blue;
                case 13: return Color.Fuchsia;
                case 14: return Color.Gray;
                case 15: return Color.Silver;
                default: return Color.White;
            }
        }

        private Color HTMLColorToColor(String h)
        {
            byte r = byte.Parse(h.Substring(1, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(h.Substring(3, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(h.Substring(5, 2), NumberStyles.HexNumber);
            return Color.FromArgb(r, g, b);
        }

        private Emotic FindNewEmoticon(String str)
        {
            for (int i = 0; i < Emoticons.ex_emotic.Length; i++)
                if (str.StartsWith("(" + Emoticons.ex_emotic[i].ShortcutText + ")"))
                    return new Emotic { Index = i, Shortcut = "(" + Emoticons.ex_emotic[i].ShortcutText + ")" };

            return null;
        }

        private void Render(String txt, String name, bool can_col, int first_col, AresFont _ff)
        {
            String text = txt.Replace("\r\n", "\r").Replace("\n",
                "\r").Replace("", "").Replace("]̽", "").Replace(" ̽",
                "").Replace("͊", "").Replace("]͊", "").Replace("͠",
                "").Replace("̶", "").Replace("̅", "");

            List<Color> cols = new List<Color>();
            StringBuilder rtf = new StringBuilder();
            int col_index;
            String bg_test;
            AresFont ff = _ff;

            if (!Settings.GetReg<bool>("receive_ppl_fonts", true))
                ff = null;

            if (ff == null)
                cols.Add(this.GetColorFromCode(first_col));
            else
            {
                bg_test = ff.TextColor.ToUpper().Replace("#", String.Empty);

                if (bg_test == "000000" && this.IsBlack)
                    bg_test = "#FFFFFF";
                else if (bg_test == "FFFFFF" && !this.IsBlack)
                    bg_test = "#000000";
                else
                    bg_test = ff.TextColor;

                cols.Add(this.HTMLColorToColor(bg_test));
            }

            rtf.Append("\\cf1 ");

            if (this.GetColorIndex(ref cols, this.IsBlack ? Color.Black : Color.White) == -1)
            {
                cols.Add(this.IsBlack ? Color.Black : Color.White);
                rtf.Append("\\highlight2 ");
            }
            else rtf.Append("\\highlight1 ");

            char[] letters = text.ToCharArray();
            bool bold = false, italic = false, underline = false;
            bool can_emoticon = Settings.GetReg<bool>("can_emoticon", true);
            int emote_count = 0;
            Color back_color = this.IsBlack ? Color.Black : Color.White;
            bool bg_code_used = false;

            String tmp;
            int itmp;

            using (Graphics richtextbox = this.CreateGraphics())
            {
                for (int i = 0; i < letters.Length; i++)
                {
                    switch (letters[i])
                    {
                        case '\x0006':
                            bold = !bold;
                            rtf.Append(bold ? "\\b" : "\\b0");
                            break;

                        case '\x0007':
                            underline = !underline;
                            rtf.Append(underline ? "\\ul" : "\\ul0");
                            break;

                        case '\x0009':
                            italic = !italic;
                            rtf.Append(italic ? "\\i" : "\\i0");
                            break;

                        case '\x03':
                            if (letters.Length >= (i + 8))
                            {
                                tmp = text.Substring((i + 1), 7);

                                if (Helpers.IsHexCode(tmp))
                                {
                                    if (!bg_code_used)
                                    {
                                        if (this.IsBlack && tmp == "#000000")
                                            itmp = 0;
                                        else if (!this.IsBlack && tmp.ToUpper() == "#FFFFFF")
                                            itmp = 1;
                                    }

                                    col_index = this.GetColorIndex(ref cols, this.HTMLColorToColor(tmp));

                                    if (col_index > -1)
                                        rtf.Append("\\cf0\\cf" + (col_index + 1) + " ");
                                    else
                                    {
                                        cols.Add(this.HTMLColorToColor(tmp));
                                        rtf.Append("\\cf0\\cf" + cols.Count + " ");
                                    }

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

                                    col_index = this.GetColorIndex(ref cols, this.GetColorFromCode(itmp));

                                    if (col_index > -1)
                                        rtf.Append("\\cf0\\cf" + (col_index + 1) + " ");
                                    else
                                    {
                                        cols.Add(this.GetColorFromCode(itmp));
                                        rtf.Append("\\cf0\\cf" + cols.Count + " ");
                                    }

                                    i += 2;
                                    break;
                                }
                            }
                            goto default;

                        case '\x05':
                            if (letters.Length >= (i + 8))
                            {
                                tmp = text.Substring((i + 1), 7);

                                if (Helpers.IsHexCode(tmp))
                                {
                                    back_color = this.HTMLColorToColor(tmp);
                                    col_index = this.GetColorIndex(ref cols, back_color);

                                    if (col_index > -1)
                                        rtf.Append("\\highlight0\\highlight" + (col_index + 1) + " ");
                                    else
                                    {
                                        cols.Add(back_color);
                                        rtf.Append("\\highlight0\\highlight" + cols.Count + " ");
                                    }

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
                                    back_color = this.GetColorFromCode(itmp);
                                    col_index = this.GetColorIndex(ref cols, back_color);

                                    if (col_index > -1)
                                        rtf.Append("\\highlight0\\highlight" + (col_index + 1) + " ");
                                    else
                                    {
                                        cols.Add(back_color);
                                        rtf.Append("\\highlight0\\highlight" + cols.Count + " ");
                                    }

                                    bg_code_used = true;
                                    i += 2;
                                    break;
                                }
                            }
                            goto default;

                        case '(':
                        case ':':
                        case ';':
                            if (can_emoticon)
                            {
                                Emotic em = Emoticons.FindEmoticon(text.ToString().Substring(i).ToUpper());

                                if (em != null)
                                {
                                    if (emote_count++ < 8)
                                    {
                                        rtf.Append(Emoticons.GetRTFEmoticon(em.Index, back_color, richtextbox));
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
                                        rtf.Append(Emoticons.GetRTFExEmoticon(em.Index, back_color, richtextbox));
                                        i += (em.Shortcut.Length - 1);
                                        break;
                                    }
                                    else goto default;
                                }
                            }
                            goto default;

                        default:
                            rtf.Append("\\u" + ((int)letters[i]) + "?");
                            break;
                    }
                }
            }

            if (underline) rtf.Append("\\ul0");
            if (italic) rtf.Append("\\i0");
            if (bold) rtf.Append("\\b0");

            rtf.Append("\\highlight0\\cf0");

            if (!String.IsNullOrEmpty(name))
            {
                StringBuilder name_builder = new StringBuilder();

                if (ff == null)
                {
                    col_index = this.GetColorIndex(ref cols, this.IsBlack ? Color.Yellow : Color.Black);

                    if (col_index > -1)
                        name_builder.Append("\\cf" + (col_index + 1) + " ");
                    else
                    {
                        cols.Add(this.IsBlack ? Color.Yellow : Color.Black);
                        name_builder.Append("\\cf" + cols.Count + " ");
                    }
                }
                else
                {
                    bg_test = ff.NameColor.ToUpper().Replace("#", String.Empty);

                    if (bg_test == "000000" && this.IsBlack)
                        bg_test = "#FFFF00";
                    else if (bg_test == "FFFFFF" && !this.IsBlack)
                        bg_test = "#000000";
                    else
                        bg_test = ff.NameColor;

                    col_index = this.GetColorIndex(ref cols, this.HTMLColorToColor(bg_test));

                    if (col_index > -1)
                        name_builder.Append("\\cf" + (col_index + 1) + " ");
                    else
                    {
                        cols.Add(this.HTMLColorToColor(bg_test));
                        name_builder.Append("\\cf" + cols.Count + " ");
                    }
                }

                col_index = this.GetColorIndex(ref cols, this.IsBlack ? Color.Black : Color.White);

                if (col_index > -1)
                    name_builder.Append("\\highlight" + (col_index + 1) + " ");
                else
                {
                    cols.Add(this.IsBlack ? Color.Black : Color.White);
                    name_builder.Append("\\highlight" + cols.Count + " ");
                }

                char[] name_chrs = (name + "> ").ToCharArray();

                for (int i = 0; i < name_chrs.Length; i++)
                    name_builder.Append("\\u" + ((int)name_chrs[i]) + "?");

                name_builder.Append("\\highlight0\\cf0");
                rtf.Insert(0, name_builder.ToString());
                name_builder = null;
            }

            if (this.Lines.Length > 0)
                rtf.Insert(0, "\\par");

            if (ff == null)
                rtf.Insert(0, "\\fs" + (Settings.GetReg<int>("global_font_size", 10) * 2));
            else
            {
                int org_size = Settings.GetReg<int>("global_font_size", 10);
                int difference = (org_size - 10);
                int user_size = ff.Size + difference;
                rtf.Insert(0, "\\fs" + (user_size * 2));
            }

            StringBuilder header = new StringBuilder();
            header.Append("\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0");

            if (ff == null)
                header.Append(Settings.GetReg<String>("global_font", "Tahoma") + ";}}");
            else
                header.Append(ff.FontName + ";}}");

            header.Append(this.ColorsToRTFColorTable(cols.ToArray()));

            this.SelectionLength = 0;
            this.SelectionStart = this.Text.Length;
            this.TrimLines();
            this.SelectedRtf = "{" + header + rtf + "}";
            this.SelectionLength = 0;
            this.SelectionStart = this.Text.Length;

            cols.Clear();
            cols = null;
            rtf = null;
            text = null;

            while (this.CanUndo)
                this.ClearUndo();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        private void TrimLines()
        {
            if (this.Lines.Length > 500)
            {
                SendMessage(this.Handle, 0x000B, 0, IntPtr.Zero);
                IntPtr eventMask = SendMessage(this.Handle, (0x400 + 59), 0, IntPtr.Zero);

                while (this.Lines.Length > 300)
                {
                    int i = this.Text.IndexOf("\n");

                    if (i == -1)
                        break;

                    String line_text = this.Text.Substring(0, i);

                    this.Select(0, (i + 1));
                    this.SelectedText = String.Empty;
                    this.ClearUndo();
                }

                SendMessage(this.Handle, (0x400 + 69), 0, eventMask);
                SendMessage(this.Handle, 0x000B, 1, IntPtr.Zero);

                this.SelectionLength = 0;
                this.SelectionStart = this.Text.Length;
            }
        }
    }

    public enum PausedItemType
    {
        Public,
        Emote,
        PM,
        Announce,
        Server
    }

    public class PausedItem
    {
        public PausedItemType Type { get; set; }
        public String Name { get; set; }
        public String Text { get; set; }
    }
}
