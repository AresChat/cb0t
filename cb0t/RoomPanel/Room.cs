using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;

namespace cb0t
{
    class Room
    {
        public ChannelButton Button { get; set; }
        public FavouritesListItem Credentials { get; set; }
        public RoomPanel Panel { get; set; }
        public IPEndPoint EndPoint { get; set; }

        private SessionState state = SessionState.Sleeping;
        private int reconnect_count = 0;
        private AresSocket sock = new AresSocket();
        private uint ticks = 0;
        private uint last_lag = 0;
        private CryptoService crypto = new CryptoService();
        private List<User> users = new List<User>();
        private bool new_sbot = false;
        private String MyName = String.Empty;

        public Room(uint time, FavouritesListItem item)
        {
            this.Credentials = item.Copy();
            this.EndPoint = new IPEndPoint(item.IP, item.Port);
            this.ticks = (time - 19);
            this.sock.PacketReceived += this.PacketReceived;
        }

        public void ConnectSendBox()
        {
            this.Panel.SendBox.KeyDown += this.SendBoxKeyDown;
        }

        public void ScrollAndFocus()
        {
            this.Panel.SendBox.BeginInvoke((Action)(() => this.Panel.SendBox.Focus()));
            this.Panel.ScrollDown();
        }

        public void Release()
        {
            this.Panel.SendBox.KeyDown -= this.SendBoxKeyDown;
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

        private int death_code = 0;

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
                        this.sock.Send(TCPOutbound.Lag(this.MyName, Helpers.UnixTimeMS, this.crypto));
                }

                if (time >= (this.ticks + 90))
                {
                    this.ticks = time;
                    this.sock.Send(TCPOutbound.Update(this.crypto));
                }

                if (!this.sock.Service(time, out this.death_code))
                {
                    this.ticks = time;
                    this.state = SessionState.Sleeping;
                    this.sock.Disconnect();
                    this.reconnect_count++;
                    this.Panel.AnnounceText("Disconnected (" + death_code + ")");
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

        private void SendBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                String text = this.Panel.SendBox.Text;
                this.Panel.SendBox.Clear();

                if (text.Length > 0)
                {
                    if (text.StartsWith("/me "))
                    {
                        if (text.Length > 4)
                            this.sock.Send(TCPOutbound.Emote(text.Substring(4), this.crypto));
                    }
                    else if (text.StartsWith("/"))
                    {
                        if (text.Length > 1)
                            this.sock.Send(TCPOutbound.Command(text.Substring(1), this.crypto));
                    }
                    else this.sock.Send(TCPOutbound.Public(text, this.crypto));
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
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

            if (packet.Remaining > 0)
                this.Credentials.Name = packet.ReadString(this.crypto);

            this.Panel.Userlist.MyLevel = 0;
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
            String name = packet.ReadString();
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

            switch (command)
            {
                case "cb0t_latency_check":
                    lag = (Helpers.UnixTimeMS - ((ulong)packet));
                    this.Panel.Userlist.UpdateLag(lag);
                    break;

                default:
                    this.Panel.AnnounceText(command);
                    break;
            }
        }
    }
}
