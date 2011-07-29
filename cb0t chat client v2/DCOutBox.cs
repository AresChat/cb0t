using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    class DCOutBox : RichTextBox
    {
        public event ChannelList.ChannelClickedDelegate OnHashlinkClicked;

        public DCOutBox()
        {
            this.HideSelection = false;
            this.DetectUrls = true;
            this.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Clear screen");
            this.ContextMenuStrip.Items[0].Click += new EventHandler(this.OnClearScreen);
            this.ContextMenuStrip.Items.Add("Open in Notepad");
            this.ContextMenuStrip.Items[1].Click += new EventHandler(this.OnOpenInNotepad);
            this.ContextMenuStrip.Items.Add("Copy all to clipboard");
            this.ContextMenuStrip.Items[2].Click += new EventHandler(this.OnCopyToClipBoard);
            this.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuOpening);
        }

        private void OnMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.SelectedText.Length == 0)
                this.ContextMenuStrip.Items[2].Text = "Copy all to clipboard";
            else
                this.ContextMenuStrip.Items[2].Text = "Copy selected text to clipboard";
        }

        private void OnCopyToClipBoard(object sender, EventArgs e)
        {
            Clipboard.Clear();

            if (this.SelectedText.Length == 0)
                Clipboard.SetText(this.Text.Replace("\n", "\r\n"));
            else
                Clipboard.SetText(this.SelectedText.Replace("\n", "\r\n"));
        }

        private void OnClearScreen(object sender, EventArgs e)
        {
            this.Clear();
        }

        private void OnOpenInNotepad(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllLines(Settings.folder_path + "chatlog.txt", this.Lines, Encoding.UTF8);
                Process.Start("notepad.exe", Settings.folder_path + "chatlog.txt");
            }
            catch { }
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        private const int SB_BOTTOM = 7;
        private const int WM_VSCROLL = 0x115;

        private String RTFFont = @"{\rtf1\ansi\ansicpg1252\deff0\deflang2057{\fonttbl{\f0\fswiss\fprq2\fcharset0 Verdana;}{\f1\fnil\fcharset0 Microsoft Sans Serif;}}";
        private String ColorTable = @"{\colortbl ;\red255\green255\blue255;\red0\green0\blue0;\red0\green0\blue128;\red0\green128\blue0;\red255\green0\blue0;\red128\green0\blue0;\red128\green0\blue128;\red255\green128\blue0;\red255\green255\blue0;\red0\green255\blue0;\red0\green128\blue128;\red0\green255\blue255;\red0\green0\blue255;\red255\green0\blue255;\red128\green128\blue128;\red192\green192\blue192;}";
        private String RTFHead = @"\viewkind4\uc1\pard\cf5\highlight1\lang1040\f0\fs20 ";
        private String RTFFoot = @"\highlight0\highlight0\lang1033\f1\fs17\par}";

        private delegate void AddAnnounceTextDelegate(String text);
        private delegate void AddSendTextDelegate(String name, String text);

        public void Announce(String text)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddAnnounceTextDelegate(this.Announce), text);
            }
            else
            {
                this.SelectionStart = this.Text.Length;

                text = text.Replace("\r\n", "\n");
                text = text.Replace("\r", "\n");

                String[] _lines = text.Split(new String[] { "\n" }, StringSplitOptions.None);

                if (Settings.enable_timestamps)
                    _lines[0] = this.TimeStamp() + " " + _lines[0];

                foreach (String _line in _lines)
                {
                    this.SelectedRtf = this.AnnounceTextToRTF(_line);
                }

                this.SelectionStart = this.Text.Length;
            }
        }

        public void Message(String name, String text)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddSendTextDelegate(this.Message), name, text);
            }
            else
            {
                this.SelectionStart = this.Text.Length;

                name = name.Replace("\r\n", "\n");
                name = name.Replace("\r", "\n");
                name = name.Split(new String[] { "\n" }, StringSplitOptions.None)[0];
                text = text.Replace("\r\n", "\n");
                text = text.Replace("\r", "\n");
                text = text.Split(new String[] { "\n" }, StringSplitOptions.None)[0];

                if (Settings.enable_timestamps)
                    name = this.TimeStamp() + " " + name;

                this.SelectedRtf = this.PMTextToRTF(name + ":", false);
                this.SelectionStart = this.Text.Length;
                this.SelectedRtf = this.PMTextToRTF("    " + text, true);
                this.SelectionStart = this.Text.Length;
            }
        }

        private String AnnounceTextToRTF(String text)
        {
            String result = String.Empty;
            result += this.RTFFont;
            result += this.ColorTable;
            result += this.RTFHead;
            result += this.ColorToRTF(Color.White, false);
            result += this.ColorToRTF(Color.Red, true);
            result += this.TextToRTF(text, Color.Red, true, true);
            result += this.RTFFoot;

            return result;
        }

        private String PMTextToRTF(String text, bool with_color)
        {
            String result = String.Empty;
            result += this.RTFFont;
            result += this.ColorTable;
            result += this.RTFHead;
            result += this.ColorToRTF(Color.White, false);
            result += this.ColorToRTF(with_color ? Color.Black : Color.Gray, true);
            result += this.TextToRTF(text, Color.Black, with_color, true);
            result += this.RTFFoot;

            return result;
        }

        private String ColorToRTF(Color color, bool foreground)
        {
            if (color == Color.White)
                return foreground ? "\\cf1 " : "\\highlight1 ";

            if (color == Color.Black)
                return foreground ? "\\cf2 " : "\\highlight2 ";

            if (color == Color.Navy)
                return foreground ? "\\cf3 " : "\\highlight3 ";

            if (color == Color.Green)
                return foreground ? "\\cf4 " : "\\highlight4 ";

            if (color == Color.Red)
                return foreground ? "\\cf5 " : "\\highlight5 ";

            if (color == Color.Maroon)
                return foreground ? "\\cf6 " : "\\highlight6 ";

            if (color == Color.Purple)
                return foreground ? "\\cf7 " : "\\highlight7 ";

            if (color == Color.Orange)
                return foreground ? "\\cf8 " : "\\highlight8 ";

            if (color == Color.Yellow)
                return foreground ? "\\cf9 " : "\\highlight9 ";

            if (color == Color.Lime)
                return foreground ? "\\cf10 " : "\\highlight10 ";

            if (color == Color.Teal)
                return foreground ? "\\cf11 " : "\\highlight11 ";

            if (color == Color.Aqua)
                return foreground ? "\\cf12 " : "\\highlight12 ";

            if (color == Color.Blue)
                return foreground ? "\\cf13 " : "\\highlight13 ";

            if (color == Color.Fuchsia)
                return foreground ? "\\cf14 " : "\\highlight14 ";

            if (color == Color.Gray)
                return foreground ? "\\cf15 " : "\\highlight15 ";

            if (color == Color.Silver)
                return foreground ? "\\cf16 " : "\\highlight16 ";

            return String.Empty;
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
            }

            return Color.White;
        }

        private String TextToRTF(String text, Color color, bool with_color, bool with_emoticons)
        {
            char[] chrs = text.ToCharArray();
            bool isBold = false;
            bool isItalic = false;
            bool isUnderline = false;
            String result = String.Empty;
            Color forecolor = color;
            Color backcolor = Color.White;
            int emote_count = 0;

            for (int i = 0; i < chrs.Length; i++)
            {
                String chr = chrs[i].ToString();

                if (with_color)
                {
                    if (chr == "\x0006") // bold
                    {
                        isBold = !isBold;
                        result += isBold ? "\\b " : "\\b0 ";
                        continue;
                    }

                    if (chr == "\x0007") // underline
                    {
                        isUnderline = !isUnderline;
                        result += isUnderline ? "\\ul " : "\\ulnone ";
                        continue;
                    }

                    if (chr == "\x0009") // italic
                    {
                        isItalic = !isItalic;
                        result += isItalic ? "\\i " : "\\i0 ";
                        continue;
                    }

                    if (chr == "\x0005") // back color
                    {
                        if (chrs.Length >= (i + 3))
                        {
                            String color_code = String.Empty;
                            color_code += chrs[i + 1].ToString();
                            color_code += chrs[i + 2].ToString();
                            int code;

                            if (int.TryParse(color_code, out code))
                            {
                                backcolor = this.GetColorFromCode(code);
                                i += 2;
                                result += this.ColorToRTF(backcolor, false);
                                continue;
                            }
                        }
                    }

                    if (chr == "\x0003") // fore color
                    {
                        if (chrs.Length >= (i + 3))
                        {
                            String color_code = String.Empty;
                            color_code += chrs[i + 1].ToString();
                            color_code += chrs[i + 2].ToString();
                            int code;

                            if (int.TryParse(color_code, out code))
                            {
                                forecolor = this.GetColorFromCode(code);
                                i += 2;
                                result += this.ColorToRTF(forecolor, true);
                                continue;
                            }
                        }
                    }
                }

                if (with_emoticons && emote_count < 10 && Settings.enable_emoticons)
                {
                    if (chr == ";" || chr == ":" || chr == "(") // emoticon
                    {
                        String emotetext = String.Empty;

                        if (chrs.Length >= (i + 3)) // max emoticon length 3 char
                        {
                            emotetext += chrs[i].ToString();
                            emotetext += chrs[i + 1].ToString();
                            emotetext += chrs[i + 2].ToString();
                            emotetext = emotetext.ToUpper();
                        }

                        if (chrs.Length >= (i + 2)) // max emoticon length 2 char
                        {
                            emotetext += chrs[i].ToString();
                            emotetext += chrs[i + 1].ToString();
                            emotetext = emotetext.ToUpper();
                        }

                        if (emotetext.Length > 0) // check if it is an emoticon
                        {
                            int _image = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(emotetext.ToUpper());

                            if (_image > -1)
                            {
                                emote_count++;
                                result += OutputTextBoxEmoticons.GetRTFEmoticon(_image, backcolor, this.CreateGraphics());
                                i += (EmoticonFinder.last_emote_length - 1);
                                continue;
                            }
                        }
                    }
                }

                result += "\\u" + ((int)chrs[i]) + "?";
            }

            return result;
        }

        private String TimeStamp()
        {
            return DateTime.Now.ToShortTimeString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                e.SuppressKeyPress = false;
            }
            else
            {
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Lines.Length > 1000)
            {
                int EM_SETEVENTMASK = 0x0431;
                int WM_SETREDRAW = 11;

                int oldEventMask = SendMessage(new HandleRef(this, this.Handle), EM_SETEVENTMASK, 0, 0);
                SendMessage(new HandleRef(this, this.Handle), WM_SETREDRAW, 0, 0);

                for (int i = 0; i < 500; i++)
                {
                    int _end = this.Text.IndexOf("\n");

                    if (_end == -1)
                        break;
                    else
                        _end++;

                    this.Select(0, _end);
                    this.SelectedText = String.Empty;
                }

                SendMessage(new HandleRef(this, this.Handle), WM_SETREDRAW, 1, 0);
                SendMessage(new HandleRef(this, this.Handle), EM_SETEVENTMASK, 0, oldEventMask);

                this.SelectionStart = this.Text.Length;
                this.ScrollToCaret();
            }
            else
            {
                base.OnTextChanged(e);
            }
        }

        protected override void OnLinkClicked(LinkClickedEventArgs e)
        {
            String str = e.LinkText;

            if (str.StartsWith("\\\\arlnk://"))
            {
                str = str.Substring(10);

                ChannelObject _obj = Hashlink.DecodeHashlink(str);

                if (_obj != null)
                    this.OnHashlinkClicked(_obj);
            }
            else
            {
                try
                {
                    Process.Start(str);
                }
                catch { }
            }
        }
    }
}
