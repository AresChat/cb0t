using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class RtfScreen : RichTextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = e.KeyData != (Keys.Control | Keys.C);
        }

        public void Free()
        {
            this.Clear();

            while (this.CanUndo)
                this.ClearUndo();
        }

        public RtfScreen()
        {
            this.BackColor = Color.White;
            this.HideSelection = false;
            this.DetectUrls = true;
        }

        public void ScrollDown()
        {
            this.BeginInvoke((Action)(() => SendMessage(this.Handle, 277, 7, IntPtr.Zero)));
        }

        private int cls_count = 0;

        public void Scribble(byte[] data)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<byte[]>(this.Scribble), data);
            else
            {
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

                this.Render((ts ? (Helpers.Timestamp + name) : name) + ":", null, true, 1, name_font);
                this.Render("    " + text, null, true, 1, font);
            }
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
                this.Render(ts ? (Helpers.Timestamp + text) : text, null, true, 2, null);
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
                this.Render(text, ts ? (Helpers.Timestamp + name) : name, true, 12, font);
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
                this.Render((ts ? Helpers.Timestamp : "") + "* " + name + " " + text, null, false, 6, font);
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

        private String FindNewEmoticon(String str)
        {
            foreach (String s in Emoticons.CustomEmoticons)
            {
                String ce = "(" + s + ")";

                if (str.StartsWith(ce))
                    return s;
            }

            return null;
        }

        private void Render(String txt, String name, bool can_col, int first_col, AresFont ff)
        {
            String text = txt.Replace("\r\n", "\r").Replace("\n",
                "\r").Replace("", "").Replace("]̽", "").Replace(" ̽",
                "").Replace("͊", "").Replace("]͊", "").Replace("͠",
                "").Replace("̶", "").Replace("̅", "");

            List<Color> cols = new List<Color>();
            StringBuilder rtf = new StringBuilder();
            List<EmItem> emitems = new List<EmItem>();
            int col_index;

            if (ff == null)
                cols.Add(this.GetColorFromCode(first_col));
            else
                cols.Add(this.HTMLColorToColor(ff.TextColor));

            rtf.Append("\\cf1 ");

            if (this.GetColorIndex(ref cols, Color.White) == -1)
            {
                cols.Add(Color.White);
                rtf.Append("\\highlight2 ");
            }
            else rtf.Append("\\highlight1 ");

            char[] letters = text.ToCharArray();
            bool bold = false, italic = false, underline = false;
            bool can_emoticon = Settings.GetReg<bool>("can_emoticon", true);
            int emote_count = 0;
            Color back_color = Color.White;

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

                                String t_em = this.FindNewEmoticon(text.ToString().Substring(i).ToUpper());

                                if (!String.IsNullOrEmpty(t_em))
                                {
                                    if (emote_count++ < 8)
                                    {
                                        EmItem temitem = emitems.Find(x => x.Name == t_em);

                                        if (temitem == null)
                                        {
                                            String emrtf = Emoticons.GetRTFExtendedEmoticon(t_em, richtextbox);
                                            emitems.Add(new EmItem { Name = t_em, RTF = emrtf });
                                            rtf.Append(emrtf);
                                            emrtf = null;
                                        }
                                        else rtf.Append(temitem.RTF);

                                        i += (t_em.Length + 1);
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

            emitems.Clear();
            emitems = null;

            if (underline) rtf.Append("\\ul0");
            if (italic) rtf.Append("\\i0");
            if (bold) rtf.Append("\\b0");

            rtf.Append("\\highlight0\\cf0");

            if (!String.IsNullOrEmpty(name))
            {
                StringBuilder name_builder = new StringBuilder();

                if (ff == null)
                {
                    col_index = this.GetColorIndex(ref cols, Color.Black);

                    if (col_index > -1)
                        name_builder.Append("\\cf" + (col_index + 1) + " ");
                    else
                    {
                        cols.Add(Color.Black);
                        name_builder.Append("\\cf" + cols.Count + " ");
                    }
                }
                else
                {
                    col_index = this.GetColorIndex(ref cols, this.HTMLColorToColor(ff.NameColor));

                    if (col_index > -1)
                        name_builder.Append("\\cf" + (col_index + 1) + " ");
                    else
                    {
                        cols.Add(this.HTMLColorToColor(ff.NameColor));
                        name_builder.Append("\\cf" + cols.Count + " ");
                    }
                }

                col_index = this.GetColorIndex(ref cols, Color.White);

                if (col_index > -1)
                    name_builder.Append("\\highlight" + (col_index + 1) + " ");
                else
                {
                    cols.Add(Color.White);
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
                rtf.Insert(0, "\\fs" + (ff.Size * 2));

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
}
