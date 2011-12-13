using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class InputTextBox : TextBox
    {
        private bool pm_mode = false;
        private bool writing = false;
        private String target_name = String.Empty;

        public event Packets.SendPacketDelegate OnPacketDispatching;
        public event Packets.SendPacketDelegate OnPacketToAll;

        public delegate void OnPMSendingDelegate(String text);
        public event OnPMSendingDelegate OnPMSending;
        public event OnPMSendingDelegate OnWhisperRequested;
        public event OnPMSendingDelegate OnFindUserInList;
        public event OnPMSendingDelegate OnPreColorChanged;

        public delegate void LagTestDelegate();
        public event LagTestDelegate OnLagTesting;

        public delegate void WritingDelegate(bool is_writing);
        public event WritingDelegate OnWriting;

        public event EventHandler CloseTabsCmd;

        private int last_press = 0;

        public InputTextBox(bool pm_mode)
        {
            this.pm_mode = pm_mode;
        }

        public InputTextBox(bool pm_mode, String target_name)
        {
            this.pm_mode = pm_mode;
            this.target_name = target_name;
        }

        public void TimeoutCheck(int time)
        {
            if (this.writing)
            {
                if (time > (this.last_press + 5))
                {
                    this.OnWriting(false);
                    this.writing = false;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            String text = this.Text;

            if (e.KeyCode == Keys.Up)
            {
                this.Text = ChatHistory.GetLine();
                this.Select(this.Text.Length, 0);
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                ChatHistory.ResetIndex();
                this.Clear();
                return;
            }

            if (e.KeyCode == Keys.B && e.Control)
            {
                this.Text += "\x0002";
                this.SelectionStart = this.Text.Length;
                return;
            }

            if (e.KeyCode == Keys.Enter && text.Length > 0)
            {
                ChatHistory.ResetIndex();
                ChatHistory.AddLine(text);

                if (this.pm_mode)
                {
                    this.OnPMSending(Helpers.FormatAresColorCodes(text));
                    this.OnPacketDispatching(Packets.PMPacket(this.target_name, text));
                    this.Clear();
                    return;
                }

                if (e.Shift) // whisper thingy for vyk :-)
                {
                    this.OnWhisperRequested(text);
                }
                else
                {
                    if (text.StartsWith("/"))
                    {
                        switch (text.Split(new String[] { " " }, StringSplitOptions.None)[0])
                        {
                            case "/me": // emote text
                                if (text.Length > 4)
                                    this.OnPacketDispatching(Packets.EmotePacket(text.Substring(4)));

                                break;

                            case "/login": // login
                                if (text.Length > 7)
                                    this.OnPacketDispatching(Packets.PasswordPacket(text.Substring(7)));

                                break;

                            case "/all": // send to all rooms
                                if (text.Length > 5 && !this.pm_mode)
                                {
                                    text = text.Substring(5);

                                    if (text.StartsWith("/me "))
                                    {
                                        text = text.Substring(4);

                                        if (text.Length > 0)
                                        {
                                            this.OnPacketToAll(Packets.EmotePacket(text));
                                        }
                                    }
                                    else
                                    {
                                        if (text.Length > 0)
                                        {
                                            this.OnPacketToAll(Packets.TextPacket(text));
                                        }
                                    }
                                }

                                break;

                            case "/ram":
                                this.OnPacketDispatching(Packets.TextPacket(ClientCommands.GetMemory()));
                                break;

                            case "/gfx":
                                String gfx = ClientCommands.GetGraphicsCardName();

                                if (!String.IsNullOrEmpty(gfx))
                                    this.OnPacketDispatching(Packets.TextPacket("Graphics: " + gfx));

                                break;

                            case "/cpu":
                                this.OnPacketDispatching(Packets.TextPacket(ClientCommands.GetCPU()));
                                break;

                            case "/closetabs":
                                this.CloseTabsCmd(true, new EventArgs());
                                break;

                            case "/time":
                                this.OnPacketDispatching(Packets.TextPacket(ClientCommands.GetTime()));
                                break;

                            case "/uptime":
                                this.OnPacketDispatching(Packets.TextPacket(ClientCommands.GetUptime()));
                                break;

                            case "/os":
                                this.OnPacketDispatching(Packets.TextPacket(ClientCommands.OSVer()));
                                break;

                            case "/lag":
                                this.OnLagTesting();
                                break;

                            case "/np":
                                this.PrintNP();
                                break;

                            case "/find":
                                if (text.Length > 7)
                                {
                                    text = text.Substring(6).Replace("\"", "");

                                    if (text.Length > 0)
                                        this.OnFindUserInList(text);
                                }

                                break;

                            case "/color":
                                if (text == "/color") // disable
                                {
                                    Settings.pre_color = String.Empty;
                                    Settings.UpdateRecords();
                                    this.OnPreColorChanged("\x000314--- Text coloring disabled");
                                }
                                else
                                {
                                    if (text.Length > 7)
                                    {
                                        text = text.Substring(7);

                                        if (text.Length > 16)
                                            text = text.Substring(0, 16);

                                        Settings.pre_color = text;
                                        Settings.UpdateRecords();
                                        this.OnPreColorChanged("\x000314--- Text coloring updated: " + Helpers.FormatAresColorCodes(Settings.pre_color) + "example text");
                                    }
                                }

                                break;

                            default: // command text
                                if (text.Length > 1)
                                    this.OnPacketDispatching(Packets.CommandPacket(text.Substring(1)));

                                break;
                        }
                    }
                    else // public text
                    {
                        this.OnPacketDispatching(Packets.TextPacket((text.StartsWith("#") ? "" : Settings.pre_color) + text));
                    }
                }

                if (this.writing)
                {
                    this.OnWriting(false);
                    this.writing = false;
                }

                this.Clear();
                return;
            }

            base.OnKeyDown(e);
        }

        private void PrintNP()
        {
            if (AudioSettings.choice == AudioPlayerChoice.Winamp)
            {
                if (!String.IsNullOrEmpty(Winamp.current_song))
                {
                    String str = AudioSettings.np_text;

                    if (str.StartsWith("/me "))
                    {
                        str = str.Substring(4);
                        str = Helpers.FormatAresColorCodes(str);
                        str = Helpers.StripColors(str);
                        str = str.Replace("+n", Winamp.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.EmotePacket(str));
                    }
                    else
                    {
                        str = Helpers.FormatAresColorCodes(str);
                        str = str.Replace("+n", Winamp.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.TextPacket(str));
                    }
                }
            }
            else if (AudioSettings.choice == AudioPlayerChoice.Itunes)
            {
                if (!String.IsNullOrEmpty(iTunes.current_song))
                {
                    String str = AudioSettings.np_text;

                    if (str.StartsWith("/me "))
                    {
                        str = str.Substring(4);
                        str = Helpers.FormatAresColorCodes(str);
                        str = Helpers.StripColors(str);
                        str = str.Replace("+n", iTunes.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.EmotePacket(str));
                    }
                    else
                    {
                        str = Helpers.FormatAresColorCodes(str);
                        str = str.Replace("+n", iTunes.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.TextPacket(str));
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(AudioSettings.current_song))
                {
                    String str = AudioSettings.np_text;

                    if (str.StartsWith("/me "))
                    {
                        str = str.Substring(4);
                        str = Helpers.FormatAresColorCodes(str);
                        str = Helpers.StripColors(str);
                        str = str.Replace("+n", AudioSettings.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.EmotePacket(str));
                    }
                    else
                    {
                        str = Helpers.FormatAresColorCodes(str);
                        str = str.Replace("+n", AudioSettings.current_song);

                        if (AudioSettings.unicode_effect)
                            str = this.UnicodeText(str);

                        this.OnPacketDispatching(Packets.TextPacket(str));
                    }
                }
            }
        }

        private String UnicodeText(String text)
        {
            string outText = text;

            char[] oldU = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] oldL = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] newU = "ΛβÇĐΞ₣ĢĦÏĴКĿMИΘ₱QЯŠŦЏ√ШЖ¥Ź".ToCharArray();
            char[] newL = "αвс∂εfgнιјκłмησρqяѕтυνωxчz".ToCharArray();

            for (int x = 0; x < 26; x++)
            {
                outText = outText.Replace(oldL[x].ToString(), newL[x].ToString());
                outText = outText.Replace(oldU[x].ToString(), newU[x].ToString());
            }

            return outText;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!this.pm_mode)
            {
                this.last_press = (int)Math.Round((double)Environment.TickCount / 1000);

                if (this.Text.Length > 0)
                {
                    if (!this.writing)
                    {
                        this.writing = true;
                        this.OnWriting(true);
                    }
                }
                else
                {
                    if (this.writing)
                    {
                        this.writing = false;
                        this.OnWriting(false);
                    }
                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyPress(e);
            }
        }
    }
}
