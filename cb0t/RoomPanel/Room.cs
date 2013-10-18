using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace cb0t
{
    class Room
    {
        public ChannelButton Button { get; set; }
        public FavouritesListItem Credentials { get; set; }
        public RoomPanel Panel { get; set; }
        public IPEndPoint EndPoint { get; set; }

        private CryptoService crypto = new CryptoService();
        private SessionState state = SessionState.Sleeping;
        private int reconnect_count = 0;
        private AresSocket sock = new AresSocket();
        private uint ticks = 0;
        private uint last_lag = 0;
        private List<User> users = new List<User>();
        private bool new_sbot = false;
        private String MyName = String.Empty;
        private Form1 owner_frm = null;

        public Room(uint time, FavouritesListItem item, Form1 f)
        {
            this.owner_frm = f;
            this.Credentials = item.Copy();
            this.EndPoint = new IPEndPoint(item.IP, item.Port);
            this.ticks = (time - 19);
            this.sock.PacketReceived += this.PacketReceived;
        }

        public void ConnectEvents()
        {
            this.Panel.SendBox.KeyDown += this.SendBoxKeyDown;
            this.Panel.CancelWriting += this.CancelWriting;
            this.Panel.SendBox.KeyUp += this.SendBoxKeyUp;
            this.Panel.SendAutoReply += this.SendAutoReply;
            this.Panel.Userlist.OpenPMRequested += this.OpenPMRequested;
        }

        public void ScrollAndFocus()
        {
            this.Panel.SendBox.BeginInvoke((Action)(() => this.Panel.SendBox.Focus()));
            this.Panel.ScrollDown();
        }

        public void Release()
        {
            this.owner_frm = null;
            this.Panel.SendBox.KeyDown -= this.SendBoxKeyDown;
            this.Panel.CancelWriting -= this.CancelWriting;
            this.Panel.SendBox.KeyUp -= this.SendBoxKeyUp;
            this.Panel.SendAutoReply -= this.SendAutoReply;
            this.Panel.Userlist.OpenPMRequested -= this.OpenPMRequested;
            this.sock.Disconnect();
            this.sock.PacketReceived -= this.PacketReceived;
            this.sock.Free();
            this.sock = null;
            this.Button.Free();
            this.Button.Dispose();
            this.Button = null;
            this.Panel.Free();
            this.Panel.Dispose();
            this.Panel = null;
            this.users.ForEach(x => x.Dispose());
            this.users.Clear();
            this.users = null;
        }

        public void SendText(String text)
        {
            this.sock.Send(TCPOutbound.Public(text, this.crypto));
        }

        public void SendEmote(String text)
        {
            this.sock.Send(TCPOutbound.Emote(text, this.crypto));
        }

        public void SendCommand(String text)
        {
            this.sock.Send(TCPOutbound.Command(text, this.crypto));
        }

        public void ForEachUser(Action<User> u)
        {
            this.users.ForEach(u);
        }

        public void SocketTasks(uint time)
        {
            if (this.state == SessionState.Sleeping)
            {
                if (time >= (this.ticks + 20))
                {
                    this.state = SessionState.Connecting;
                    this.sock.Connect(new IPEndPoint(this.Credentials.IP, this.Credentials.Port));
                    this.ticks = time;
                    this.crypto.Mode = CryptoMode.Unencrypted;
                    this.Panel.Userlist.SetCrypto(false);
                    this.new_sbot = false;

                    if (this.reconnect_count > 0)
                        this.Panel.ServerText("Connecting to host, please wait... #" + this.reconnect_count);
                    else
                        this.Panel.ServerText("Connecting to host, please wait...");

                    ScriptEvents.OnConnecting(this);
                }
            }
            else if (this.state == SessionState.Connecting)
            {
                if (this.sock.Connected)
                {
                    this.state = SessionState.Connected;
                    this.ticks = time;
                    this.last_lag = (time - 25);
                    this.Panel.ServerText("Connected, handshaking...");
                    this.sock.Clear();
                    this.sock.Send(TCPOutbound.Login());
                }
                else if (time >= (this.ticks + 10))
                {
                    this.ticks = time;
                    this.state = SessionState.Sleeping;
                    this.sock.Disconnect();
                    this.reconnect_count++;
                    this.Panel.AnnounceText("Unable to connect");
                    ScriptEvents.OnDisconnected(this);
                }
            }
            else if (this.sock != null)
            {
                if (time >= (this.last_lag + 30))
                {
                    this.last_lag = time;

                    if (Settings.GetReg<bool>("lag_check", true))
                        this.sock.SendPriority(TCPOutbound.Lag(this.MyName, Helpers.UnixTimeMS, this.crypto));
                }

                if (time >= (this.ticks + 90))
                {
                    this.ticks = time;
                    this.sock.SendPriority(TCPOutbound.Update(this.crypto));
                }

                if (this.is_writing)
                    if (time >= (this.last_key_press + 5))
                    {
                        this.is_writing = false;

                        if (Settings.GetReg<bool>("can_write", true))
                        {
                            this.Panel.UpdateMyWriting(this.MyName, false);
                            this.sock.Send(TCPOutbound.Writing(false, this.crypto));
                        }
                    }

                if (!this.sock.Service(time))
                {
                    this.ticks = time;
                    this.state = SessionState.Sleeping;
                    this.sock.Disconnect();
                    this.reconnect_count++;

                    if (this.sock.SockCode > 0)
                        this.Panel.AnnounceText("Disconnected (" + this.sock.SockCode + ")");
                    else
                        this.Panel.AnnounceText("Disconnected (remote disconnect)");
                }
            }
        }

        private void Reconnect()
        {
            this.ticks = (Settings.Time - 19);
            this.state = SessionState.Sleeping;
            this.sock.Disconnect();
            this.reconnect_count++;
            this.Panel.AnnounceText("Reconnecting...");
        }

        private bool is_writing = false;
        private uint last_key_press = 0;

        private void CancelWriting(object sender, EventArgs e)
        {
            if (this.state == SessionState.Connected)
                if (this.is_writing)
                {
                    this.is_writing = false;

                    if (Settings.GetReg<bool>("can_write", true))
                    {
                        this.Panel.UpdateMyWriting(this.MyName, false);
                        this.sock.Send(TCPOutbound.Writing(false, this.crypto));
                    }
                }
        }

        private void SendBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (this.state == SessionState.Connected && this.Panel.Mode == ScreenMode.Main)
            {
                if (!this.is_writing)
                {
                    if (this.Panel.SendBox.Text.Length > 0)
                    {
                        this.is_writing = true;

                        if (Settings.GetReg<bool>("can_write", true))
                        {
                            this.Panel.UpdateMyWriting(this.MyName, true);
                            this.sock.Send(TCPOutbound.Writing(true, this.crypto));
                        }
                    }
                }
                else if (this.Panel.SendBox.Text.Length == 0)
                {
                    this.is_writing = false;

                    if (Settings.GetReg<bool>("can_write", true))
                    {
                        this.Panel.UpdateMyWriting(this.MyName, false);
                        this.sock.Send(TCPOutbound.Writing(false, this.crypto));
                    }
                }

                this.last_key_press = Settings.Time;
            }
        }

        private void SendBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.Panel.SendBox.Text = History.GetText();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.Panel.SendBox.Clear();
                History.Reset();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                String text = this.Panel.SendBox.Text;
                this.Panel.SendBox.Clear();

                if (text.Length > 0)
                {
                    if (text == "/reconnect")
                    {
                        this.Reconnect();
                        this.Panel.UpdateMyWriting(this.MyName, false);
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                        return;
                    }

                    if (this.state != SessionState.Connected)
                        return;

                    History.AddText(text);

                    if (this.Panel.Mode == ScreenMode.Main)
                    {
                        if (text.StartsWith("/me "))
                        {
                            if (text.Length > 4)
                                this.sock.Send(TCPOutbound.Emote(text.Substring(4), this.crypto));
                        }
                        else if (text.StartsWith("/"))
                        {
                            if (text == "/time")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_TIME, this.crypto));
                            else if (text == "/uptime")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_UPTIME, this.crypto));
                            else if (text == "/gfx")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_GFX, this.crypto));
                            else if (text == "/hdd")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_HDD, this.crypto));
                            else if (text == "/os")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_OS, this.crypto));
                            else if (text == "/cpu")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_CPU, this.crypto));
                            else if (text == "/ram")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_RAM, this.crypto));
                            else if (text == "/lag")
                            {

                            }
                            else if (text == "/cmds")
                            {

                            }
                            else if (text.StartsWith("/all "))
                            {

                            }
                            else if (text.StartsWith("/find "))
                            {

                            }
                            else if (text.StartsWith("/pretext"))
                            {

                            }
                            else if (text.Length > 1)
                                this.sock.Send(TCPOutbound.Command(text.Substring(1), this.crypto));
                        }
                        else this.sock.Send(TCPOutbound.Public(text, this.crypto));
                    }
                    else if (this.Panel.Mode == ScreenMode.PM)
                    {
                        User u = this.users.Find(x => x.Name == this.Panel.PMName);

                        if (u != null)
                        {
                            this.Panel.MyPMText(text, null); // my font

                            if (!u.SupportsPMEnc)
                                this.sock.Send(TCPOutbound.Private(this.Panel.PMName, text, this.crypto));
                            else
                                this.sock.Send(TCPOutbound.CustomPM(this.Panel.PMName, text, this.crypto));
                        }
                        else this.Panel.MyPMAnnounce("User is offline");
                    }
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void SendAutoReply(object sender, EventArgs e)
        {
            String pmname = (String)sender;

        }

        private void OpenPMRequested(object sender, EventArgs e)
        {
            String pmname = (String)sender;
            this.Panel.MyPMCreateOrShowTab(pmname);
        }

        private void PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Msg)
            {
                case TCPMsg.MSG_CHAT_SERVER_LOGIN_ACK:
                    this.Eval_Ack(e.Packet, e.Time);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_MYFEATURES:
                    this.Eval_Features(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST:
                    this.Eval_UserlistItem(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST_END:
                    this.Eval_UserlistEnds();
                    break;

                case TCPMsg.MSG_CHAT_SERVER_ERROR:
                    this.Panel.AnnounceText(e.Packet.ReadString(this.crypto));
                    break;

                case TCPMsg.MSG_CHAT_SERVER_NOSUCH:
                    this.Eval_Announce(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_TOPIC:
                    this.Eval_Topic(e.Packet.ReadString(this.crypto), true);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_TOPIC_FIRST:
                    this.Eval_Topic(e.Packet.ReadString(this.crypto), false);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_AVATAR:
                    this.Eval_Avatar(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_PERSONAL_MESSAGE:
                    this.Eval_PersonalMessage(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_URL:
                    this.Eval_URL(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_UPDATE_USER_STATUS:
                    this.Eval_UpdateUserStatus(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_JOIN:
                    this.Eval_Join(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_PART:
                    this.Eval_Part(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_PUBLIC:
                    this.Eval_Public(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_EMOTE:
                    this.Eval_Emote(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_REDIRECT:
                    this.Eval_Redirect(e.Packet, e.Time);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_PVT:
                    this.Eval_Private(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_ISIGNORINGYOU:
                    this.Eval_IsIgnoringYou(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_OFFLINEUSER:
                    this.Eval_OfflineUser(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL:
                    this.UnofficialProtoReceived(e);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_CUSTOM_DATA:
                    this.CustomProtoReceived(e.Packet);
                    break;

                default:
                    this.Panel.AnnounceText(e.Msg.ToString());
                    break;
            }
        }

        private void Eval_Ack(TCPPacketReader packet, uint time)
        {
            this.ticks = time;
            this.last_lag = (time - 25);
            this.reconnect_count = 0;
            this.Panel.CheckUnreadStatus();
            this.Panel.Userlist.ClearUserList();
            this.users.ForEach(x => x.Dispose());
            this.users.Clear();
            this.Panel.ServerText("Logged in, retrieving user's list...");
            this.Panel.CanVC(false);
            this.MyName = packet.ReadString(this.crypto);
            this.Panel.MyName = this.MyName;

            if (packet.Remaining > 0)
                this.Credentials.Name = packet.ReadString(this.crypto);

            this.Panel.Userlist.MyLevel = 0;
            this.is_writing = false;
            this.Panel.ClearWriters();
        }

        private void Eval_Features(TCPPacketReader packet)
        {
            String version = packet.ReadString(this.crypto);
            this.Panel.ServerText("Server: " + version);
            this.Panel.Userlist.UpdateServerVersion(version);

            if (version.StartsWith("sb0t 5."))
            {
                String vnum_str = new String(version.Where(x => Char.IsNumber(x)).ToArray());
                uint vnum;

                if (!uint.TryParse(vnum_str, out vnum))
                    vnum = 0;

                this.new_sbot = (vnum >= 514);
            }

            byte flag = packet;
            bool has_vc = ((flag & 8) == 8);
            bool has_html = ((flag & 128) == 128);
            this.Panel.CanVC(has_vc);

            if (has_html)
                this.Panel.Userlist.AcquireServerIcon(this.EndPoint);

            packet.SkipByte();
            this.Panel.ServerText("Language: " + (RoomLanguage)((byte)packet));
            uint cookie = packet;

            // send my av+pmsg+font+autopassword

            ScriptEvents.OnConnected(this);
        }

        private void Eval_Announce(TCPPacketReader packet)
        {
            String text = packet.ReadString(this.crypto);

            if (ScriptEvents.OnAnnounceReceiving(this, text))
            {
                this.Panel.AnnounceText(text);
                ScriptEvents.OnAnnounceReceived(this, text);
            }
        }

        private void Eval_Redirect(TCPPacketReader packet, uint time)
        {
            Redirect redirect = new Redirect();
            redirect.IP = packet;
            redirect.Port = packet;
            packet.SkipBytes(4);
            redirect.Name = packet.ReadString(this.crypto);
            redirect.Hashlink = Hashlink.EncodeHashlink(redirect);

            if (ScriptEvents.OnRedirecting(this, redirect))
            {
                this.Credentials.IP = redirect.IP;
                this.Credentials.Port = redirect.Port;
                this.Credentials.Name = redirect.Name;
                this.ticks = (time - 19);
                this.state = SessionState.Sleeping;
                this.sock.Disconnect();
                this.Panel.AnnounceText("Redirecting to " + redirect.Name + "...");
            }
        }

        private void Eval_UpdateUserStatus(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u != null)
            {
                packet.SkipBytes(13);
                byte level = packet;

                if (u.Level != level)
                {
                    byte before = u.Level;
                    u.Level = level;
                    this.Panel.Userlist.UpdateUserLevel(u, before);
                    ScriptEvents.OnUserLevelChanged(this, u);
                }

                if (u.Name == this.MyName)
                    this.Panel.Userlist.MyLevel = u.Level;
            }
        }

        private void Eval_URL(TCPPacketReader packet)
        {
            String addr = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);

            if (ScriptEvents.OnUrlReceiving(this, text, addr))
                this.Panel.SetURL(text, addr);
        }

        private void Eval_Avatar(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u != null)
            {
                if (!ScriptEvents.OnUserAvatarReceiving(this, u))
                    return;

                this.Panel.Userlist.BeginInvoke((Action)(() =>
                {
                    byte[] data = packet;

                    if (data.Length <= 10)
                        u.ClearAvatar();
                    else
                        u.SetAvatar(data);

                    this.Panel.Userlist.UpdateUserAppearance(u);
                }));
            }
        }

        private void Eval_PersonalMessage(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);

            if (!String.IsNullOrEmpty(text))
                text = Helpers.FormatAresColorCodes(text);

            User u = this.users.Find(x => x.Name == name);

            if (u != null)
                if (ScriptEvents.OnUserMessageReceiving(this, u, text))
                {
                    u.PersonalMessage = text;
                    this.Panel.Userlist.UpdateUserAppearance(u);
                }
        }

        private void Eval_Join(TCPPacketReader packet)
        {
            User u = new User();
            ushort files = packet;
            packet.SkipBytes(4);
            u.ExternalIP = packet;
            u.Port = packet;
            packet.SkipBytes(4);
            u.SupportsPMEnc = ((ushort)packet) == 65535;
            packet.SkipByte();
            u.Name = packet.ReadString(this.crypto);
            u.LocalIP = packet;
            u.HasFiles = ((byte)packet) == 1 && files > 0;
            u.Level = packet;
            u.Age = packet;
            u.Gender = packet;
            byte country = packet;
            u.Country = Helpers.CountryCodeToString(country);
            u.Region = packet.ReadString(this.crypto);
            u.IsFriend = false;
            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);

            if (ScriptEvents.OnUserJoining(this, u))
                this.Panel.AnnounceText("\x000303" + u.Name + " has joined");

            if (u.Name == this.MyName)
                this.Panel.Userlist.MyLevel = u.Level;

            ScriptEvents.OnUserJoined(this, u);
        }

        private void Eval_Part(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);
            this.users.RemoveAll(x => x.Name == name);

            if (u != null)
            {
                if (u.Writing)
                {
                    u.Writing = false;
                    this.Panel.UpdateWriter(u);
                }

                this.Panel.Userlist.RemoveUserItem(u);

                if (ScriptEvents.OnUserParting(this, u))
                    this.Panel.AnnounceText("\x000307" + u.Name + " has parted");

                ScriptEvents.OnUserParted(this, u);
                u.Dispose();
                u = null;
            }
        }

        private void Eval_UserlistItem(TCPPacketReader packet)
        {
            User u = new User();
            ushort files = packet;
            packet.SkipBytes(4);
            u.ExternalIP = packet;
            u.Port = packet;
            packet.SkipBytes(4);
            u.SupportsPMEnc = ((ushort)packet) == 65535;
            packet.SkipByte();
            u.Name = packet.ReadString(this.crypto);
            u.LocalIP = packet;
            u.HasFiles = ((byte)packet) == 1 && files > 0;
            u.Level = packet;
            u.Age = packet;
            u.Gender = packet;
            byte country = packet;
            u.Country = Helpers.CountryCodeToString(country);
            u.Region = packet.ReadString(this.crypto);
            u.IsFriend = false;
            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);

            if (u.Name == this.MyName)
                this.Panel.Userlist.MyLevel = u.Level;
        }

        private void Eval_UserlistEnds()
        {
            this.Panel.Userlist.ResumeUserlist();
            ScriptEvents.OnUserlistReceived(this);
        }

        private void Eval_Topic(String text, bool updated)
        {
            if (updated)
                this.Panel.ServerText("Topic update: " + text);

            if (ScriptEvents.OnTopicReceiving(this, text))
                this.Panel.SetTopic(text);
        }

        private void Eval_IsIgnoringYou(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            this.Panel.PMTextReceived(name, "User is ignoring you", null, PMTextReceivedType.Announce);
        }

        private void Eval_OfflineUser(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            this.Panel.PMTextReceived(name, "User is offline", null, PMTextReceivedType.Announce);
        }

        private void Eval_Private(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u == null)
                return;

            AresFont font = null;

            if (u.Font != null)
                font = u.Font;

            if (ScriptEvents.OnPmReceiving(this, u, text))
            {
                this.Panel.PMTextReceived(name, text, font, PMTextReceivedType.Text);
                this.Panel.CheckUnreadStatus();
                ScriptEvents.OnPmReceived(this, u, text);
            }
        }

        private void Eval_Public(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);
            AresFont font = null;

            if (u != null)
                if (u.Font != null)
                    font = u.Font;

            if (ScriptEvents.OnTextReceiving(this, name, text))
            {
                this.Panel.PublicText(name, text, font);
                this.Panel.CheckUnreadStatus();
                ScriptEvents.OnTextReceived(this, name, text);
            }
        }

        private void Eval_Emote(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);
            AresFont font = null;

            if (u != null)
                if (u.Font != null)
                    font = u.Font;

            if (ScriptEvents.OnEmoteReceiving(this, name, text))
            {
                this.Panel.EmoteText(name, text, font);
                this.Panel.CheckUnreadStatus();
                ScriptEvents.OnEmoteReceived(this, name, text);
            }
        }

        private void UnofficialProtoReceived(PacketReceivedEventArgs e)
        {
            e.Packet.SkipBytes(2);
            TCPMsg msg = (TCPMsg)((byte)e.Packet);

            switch (msg)
            {
                case TCPMsg.MSG_CHAT_SERVER_CRYPTO_KEY:
                    this.crypto.SetCrypto(e.Packet);
                    this.Panel.Userlist.SetCrypto(true);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_CUSTOM_FONT:
                    this.Eval_Font(e.Packet);
                    break;

                default:
                    this.Panel.AnnounceText(msg.ToString());
                    break;
            }
        }

        private void Eval_Font(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u != null)
                if (ScriptEvents.OnUserFontChanging(this, u))
                {
                    AresFont f = new AresFont();
                    f.Size = (int)((byte)packet);
                    f.FontName = packet.ReadString(this.crypto);
                    byte oldN = packet;
                    byte oldT = packet;

                    if (oldN == 255 || oldT == 255)
                    {
                        u.Font = null;
                        return;
                    }

                    if (this.new_sbot)
                    {
                        if (packet.Remaining > 0)
                            f.NameColor = packet.ReadString(this.crypto);

                        if (packet.Remaining > 0)
                            f.TextColor = packet.ReadString(this.crypto);
                    }

                    if (String.IsNullOrEmpty(f.NameColor))
                        f.NameColor = Helpers.AresColorToHTMLColor(oldN);

                    if (String.IsNullOrEmpty(f.TextColor))
                        f.TextColor = Helpers.AresColorToHTMLColor(oldT);

                    u.Font = f;
                }
        }

        private void CustomProtoReceived(TCPPacketReader packet)
        {
            String command = packet.ReadString(this.crypto);
            String sender = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == sender);
            ulong lag;
            bool b;

            switch (command)
            {
                case "cb0t_writing":
                    u.Writing = ((byte)packet) == 2;
                    this.Panel.UpdateWriter(u);
                    ScriptEvents.OnUserWritingStatusChanged(this, u);
                    break;

                case "cb0t_latency_check":
                    lag = (Helpers.UnixTimeMS - ((ulong)packet));
                    this.Panel.Userlist.UpdateLag(lag);
                    break;

                case "cb0t_online_status":
                    b = ((byte)packet) != 1;

                    if (u.IsAway != b)
                    {
                        u.IsAway = b;
                        this.Panel.Userlist.UpdateUserAppearance(u);
                        ScriptEvents.OnUserOnlineStatusChanged(this, u);
                    }

                    break;

                case "cb0t_nudge":
                    this.Eval_Nudge(u, ((byte[])packet));
                    break;

                case "cb0t_pm_msg":
                    this.Eval_cb0t_pm_msg(u, ((byte[])packet));
                    break;

                case "cb0t_scribble_once":
                    u.ScribbleBuffer.Clear();
                    u.ScribbleBuffer.AddRange((byte[])packet);
                    this.Eval_Scribble(u);
                    break;

                case "cb0t_scribble_first":
                    u.ScribbleBuffer.Clear();
                    u.ScribbleBuffer.AddRange((byte[])packet);
                    break;

                case "cb0t_scribble_chunk":
                    u.ScribbleBuffer.AddRange((byte[])packet);
                    break;

                case "cb0t_scribble_last":
                    u.ScribbleBuffer.AddRange((byte[])packet);
                    this.Eval_Scribble(u);
                    break;

                default:
                    this.Panel.AnnounceText(command);
                    break;
            }
        }

        private void Eval_Scribble(User user)
        {
            byte[] data = user.ScribbleBuffer.ToArray();
            user.ScribbleBuffer.Clear();

            if (Settings.GetReg<bool>("receive_scribbles", true))
                if (ScriptEvents.OnScribbleReceiving(this, user))
                {
                    data = Zip.Decompress(data);
                    this.Panel.AnnounceText("\x000314--- " + user.Name + ":");
                    this.Panel.Scribble(data);
                    ScriptEvents.OnScribbleReceived(this, user);
                }
        }

        private void Eval_cb0t_pm_msg(User user, byte[] data)
        {
            String name = user.Name;
            String text = Encoding.UTF8.GetString(PMCrypto.SoftDecrypt(this.MyName, data));
            User u = this.users.Find(x => x.Name == name);

            if (u == null)
                return;

            AresFont font = null;

            if (u.Font != null)
                font = u.Font;

            if (ScriptEvents.OnPmReceiving(this, u, text))
            {
                this.Panel.PMTextReceived(name, text, font, PMTextReceivedType.Text);
                this.Panel.CheckUnreadStatus();
                ScriptEvents.OnPmReceived(this, u, text);
            }
        }

        private void Eval_Nudge(User user, byte[] data)
        {
            if (data.Length == 4)
                if (data.SequenceEqual(new byte[] { 78, 65, 61, 61 }))
                {
                    this.Panel.AnnounceText("\x000314--- " + user.Name + " is not receiving nudges");
                    return;
                }

            if (Settings.GetReg<bool>("receive_nudge", true))
            {
                if (ScriptEvents.OnNudgeReceiving(this, user))
                {
                    this.Panel.AnnounceText("\x000314--- " + user.Name + " has nudged you!");
                    this.owner_frm.Nudge();
                }
                else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
            }
            else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
        }


    }
}
