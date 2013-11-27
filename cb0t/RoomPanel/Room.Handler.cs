using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    partial class Room
    {
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
                    this.CustomProtoReceived(e.Packet, e.Time);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_STARTOFBROWSE:
                    this.Eval_StartBrowse(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_BROWSEITEM:
                    this.Eval_BrowseItem(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_BROWSEERROR:
                    this.Eval_BrowseError(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_ENDOFBROWSE:
                    this.Eval_EndBrowse(e.Packet);
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
            this.users = new List<User>();
            this.Panel.ServerText("Logged in, retrieving user's list...");
            this.Panel.CanVC(false);
            this.CanVC = false;
            this.CanOpusVC = false;
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

            ServerFeatures flag = (ServerFeatures)((byte)packet);
            this.CanVC = ((flag & ServerFeatures.SERVER_SUPPORTS_VC) == ServerFeatures.SERVER_SUPPORTS_VC);
            bool has_html = ((flag & ServerFeatures.SERVER_SUPPORTS_HTML) == ServerFeatures.SERVER_SUPPORTS_HTML);
            bool has_scribble = ((flag & ServerFeatures.SERVER_SUPPORTS_ROOM_SCRIBBLES) == ServerFeatures.SERVER_SUPPORTS_ROOM_SCRIBBLES);
            bool has_pm_scribble = ((flag & ServerFeatures.SERVER_SUPPORTS_PM_SCRIBBLES) == ServerFeatures.SERVER_SUPPORTS_PM_SCRIBBLES);

            this.CanOpusVC = ((flag & ServerFeatures.SERVER_SUPPORTS_OPUS_VC) == ServerFeatures.SERVER_SUPPORTS_OPUS_VC);
            this.Panel.CanVC(this.CanVC);
            this.Panel.CanScribbleAll(has_scribble);
            this.Panel.CanScribblePM(has_pm_scribble);
            this.Panel.InitScribbleButton();

            if (has_html)
                this.Panel.Userlist.AcquireServerIcon(this.EndPoint);

            packet.SkipByte();
            this.Panel.ServerText("Language: " + (RoomLanguage)((byte)packet));
            uint cookie = packet;

            if (!String.IsNullOrEmpty(this.Credentials.Password))
                this.sock.Send(TCPOutbound.SecureAdminLogin(this.Credentials.Password, cookie, this.Credentials.IP));

            this.UpdatePersonalMessage();

            if (Avatar.Data != null)
                this.sock.SendTrickle(TCPOutbound.Avatar());

            if (Settings.GetReg<bool>("user_font_enabled", false))
                this.sock.SendTrickle(TCPOutbound.Font(this.new_sbot, this.crypto));

            ScriptEvents.OnConnected(this);
        }

        private void Eval_StartBrowse(TCPPacketReader packet)
        {
            ushort ident = packet;
            ushort count = packet;
            this.Panel.StartBrowse(ident, count);
        }

        private void Eval_BrowseItem(TCPPacketReader packet)
        {
            ushort ident = packet;
            BrowseItem item = new BrowseItem(packet, this.crypto);
            this.Panel.BrowseItemReceived(ident, item);
        }

        private void Eval_BrowseError(TCPPacketReader packet)
        {
            ushort ident = packet;
            this.Panel.BrowseError(ident);
        }

        private void Eval_EndBrowse(TCPPacketReader packet)
        {
            ushort ident = packet;
            this.Panel.BrowseEnd(ident);
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
            if (Settings.GetReg<bool>("block_redirect", false))
                return;

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
            u.IsFriend = Friends.IsFriend(u.Name);

            if (packet.Remaining > 0)
            {
                ClientFeatures features = (ClientFeatures)((byte)packet);
                u.SupportsVC = ((features & ClientFeatures.CLIENT_SUPPORTS_VC) == ClientFeatures.CLIENT_SUPPORTS_VC);
                u.SupportsOpusVC = ((features & ClientFeatures.CLIENT_SUPPORTS_OPUS_VC) == ClientFeatures.CLIENT_SUPPORTS_OPUS_VC);

                if (u.SupportsOpusVC)
                    u.SupportsVC = true;
            }

            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);

            if (ScriptEvents.OnUserJoining(this, u))
                this.Panel.AnnounceText((this.BlackBG ? "\x000309" : "\x000303") + u.Name + " has joined");

            if (u.Name == this.MyName)
            {
                u.IsAway = Settings.IsAway;
                this.Panel.Userlist.MyLevel = u.Level;
            }

            ScriptEvents.OnUserJoined(this, u);

            if (u.IsFriend)
                if (!Settings.GetReg<bool>("block_friend_popup", false))
                    this.ShowPopup("cb0t :: Friend", u.Name + " has joined " + this.Credentials.Name, PopupSound.Friend);
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

            if (packet.Remaining > 0)
            {
                ClientFeatures features = (ClientFeatures)((byte)packet);
                u.SupportsVC = ((features & ClientFeatures.CLIENT_SUPPORTS_VC) == ClientFeatures.CLIENT_SUPPORTS_VC);
                u.SupportsOpusVC = ((features & ClientFeatures.CLIENT_SUPPORTS_OPUS_VC) == ClientFeatures.CLIENT_SUPPORTS_OPUS_VC);

                if (u.SupportsOpusVC)
                    u.SupportsVC = true;
            }

            u.IsFriend = Friends.IsFriend(u.Name);
            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);

            if (u.Name == this.MyName)
            {
                u.IsAway = Settings.IsAway;
                this.Panel.Userlist.MyLevel = u.Level;
            }
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
            this.Panel.PMTextReceived(null, null, name, "User is ignoring you", null, PMTextReceivedType.Announce);
        }

        private void Eval_OfflineUser(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            this.Panel.PMTextReceived(null, null, name, "User is offline", null, PMTextReceivedType.Announce);
        }

        private void Eval_Private(TCPPacketReader packet)
        {
            if (!Settings.GetReg<bool>("can_receive_pms", true))
                return;

            String name = packet.ReadString(this.crypto);
            String text = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u == null)
                return;

            AresFont font = null;

            if (u.Font != null)
                font = u.Font;

            if (u.Ignored)
                return;

            if (ScriptEvents.OnPmReceiving(this, u, text))
            {
                this.Panel.PMTextReceived(this, u, name, text, font, PMTextReceivedType.Text);
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
            {
                if (u.Ignored)
                    return;

                if (u.Font != null)
                    font = u.Font;
            }

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
            {
                if (u.Ignored)
                    return;

                if (u.Font != null)
                    font = u.Font;
            }

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

                case TCPMsg.MSG_CHAT_SERVER_VC_SUPPORTED:
                    this.Eval_VC_Supported(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_VC_USER_SUPPORTED:
                    this.Eval_VC_UserSupported(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_VC_FIRST:
                    this.Eval_VC_First(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_VC_CHUNK:
                    this.Eval_VC_Chunk(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_FIRST_TO:
                    this.Eval_VC_PM_First(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_CHUNK_TO:
                    this.Eval_VC_PM_Chunk(e.Packet);
                    break;

                default:
                    this.Panel.AnnounceText(msg.ToString());
                    break;
            }
        }

        private void Eval_VC_PM_First(TCPPacketReader packet)
        {
            VoicePlayerInboundItem item = new VoicePlayerInboundItem(packet, this.crypto, this.EndPoint);
            VoicePlayer.Inbound.RemoveAll(x => x.Ident == item.Ident && x.EndPoint.Equals(this.EndPoint));

            if (item.Received)
            {
                item.Save();

                if (!String.IsNullOrEmpty(item.FileName))
                {
                    VoicePlayerItem vc = item.ToVoicePlayerItem(++VoicePlayer.NEXT_SHORTCUT, this.BlackBG);
                    VoicePlayer.Records.Add(vc);
                    User u = this.users.Find(x => x.Name == vc.Sender);

                    if (u != null)
                        if (!u.Ignored)
                            if (ScriptEvents.OnVoiceClipReceiving(this, u))
                            {
                                this.Panel.PMTextReceived(this, u, vc.Sender, (this.BlackBG ? "\x000315" : "\x000314") + "--- \\\\voice_clip_#" + vc.ShortCut + " received from " + vc.Sender, null, PMTextReceivedType.Announce);
                                ScriptEvents.OnVoiceClipReceived(this, u);
                            }
                }
            }
            else VoicePlayer.Inbound.Add(item);
        }

        private void Eval_VC_PM_Chunk(TCPPacketReader packet)
        {
            String sender = packet.ReadString(this.crypto);
            uint ident = packet;
            int index = VoicePlayer.Inbound.FindIndex(x => x.EndPoint.Equals(this.EndPoint) && x.Ident == ident);
            byte[] chunk = packet;

            if (index > -1)
            {
                VoicePlayerInboundItem item = VoicePlayer.Inbound[index];
                item.AddChunk(chunk);

                if (item.Received)
                {
                    VoicePlayer.Inbound.RemoveAt(index);
                    item.Save();

                    if (!String.IsNullOrEmpty(item.FileName))
                    {
                        VoicePlayerItem vc = item.ToVoicePlayerItem(++VoicePlayer.NEXT_SHORTCUT, this.BlackBG);
                        VoicePlayer.Records.Add(vc);
                        User u = this.users.Find(x => x.Name == vc.Sender);

                        if (u != null)
                            if (!u.Ignored)
                                if (ScriptEvents.OnVoiceClipReceiving(this, u))
                                {
                                    this.Panel.PMTextReceived(this, u, vc.Sender, (this.BlackBG ? "\x000315" : "\x000314") + "--- \\\\voice_clip_#" + vc.ShortCut + " received from " + vc.Sender, null, PMTextReceivedType.Announce);
                                    ScriptEvents.OnVoiceClipReceived(this, u);
                                }
                    }
                }
            }
        }

        private void Eval_VC_First(TCPPacketReader packet)
        {
            VoicePlayerInboundItem item = new VoicePlayerInboundItem(packet, this.crypto, this.EndPoint);
            VoicePlayer.Inbound.RemoveAll(x => x.Ident == item.Ident && x.EndPoint.Equals(this.EndPoint));

            if (item.Received)
            {
                item.Save();

                if (!String.IsNullOrEmpty(item.FileName))
                {
                    VoicePlayerItem vc = item.ToVoicePlayerItem(++VoicePlayer.NEXT_SHORTCUT, this.BlackBG);
                    VoicePlayer.Records.Add(vc);
                    User u = this.users.Find(x => x.Name == vc.Sender);

                    if (u != null)
                        if (!u.Ignored)
                            if (ScriptEvents.OnVoiceClipReceiving(this, u))
                            {
                                if (this.CanAutoPlayVC)
                                {
                                    vc.Auto = true;
                                    VoicePlayer.QueueItem(vc);
                                }
                                else this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- \\\\voice_clip_#" + vc.ShortCut + " received from " + vc.Sender);

                                ScriptEvents.OnVoiceClipReceived(this, u);
                            }
                }
            }
            else VoicePlayer.Inbound.Add(item);
        }

        private void Eval_VC_Chunk(TCPPacketReader packet)
        {
            String sender = packet.ReadString(this.crypto);
            uint ident = packet;
            int index = VoicePlayer.Inbound.FindIndex(x => x.EndPoint.Equals(this.EndPoint) && x.Ident == ident);
            byte[] chunk = packet;

            if (index > -1)
            {
                VoicePlayerInboundItem item = VoicePlayer.Inbound[index];
                item.AddChunk(chunk);

                if (item.Received)
                {
                    VoicePlayer.Inbound.RemoveAt(index);
                    item.Save();

                    if (!String.IsNullOrEmpty(item.FileName))
                    {
                        VoicePlayerItem vc = item.ToVoicePlayerItem(++VoicePlayer.NEXT_SHORTCUT, this.BlackBG);
                        VoicePlayer.Records.Add(vc);
                        User u = this.users.Find(x => x.Name == vc.Sender);

                        if (u != null)
                            if (!u.Ignored)
                                if (ScriptEvents.OnVoiceClipReceiving(this, u))
                                {
                                    if (this.CanAutoPlayVC)
                                    {
                                        vc.Auto = true;
                                        VoicePlayer.QueueItem(vc);
                                    }
                                    else this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- \\\\voice_clip_#" + vc.ShortCut + " received from " + vc.Sender);

                                    ScriptEvents.OnVoiceClipReceived(this, u);
                                }
                    }
                }
            }
        }

        private void Eval_VC_UserSupported(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);

            if (packet.Remaining < 2)
                return;

            User user = this.users.Find(x => x.Name == name);

            if (user != null)
            {
                bool can_public = ((byte)packet) == 1;

                if (!can_public)
                {
                    user.SupportsVC = false;
                    user.SupportsOpusVC = false;
                    this.Panel.Userlist.UpdateUserAppearance(user);
                }
                else if (!user.SupportsVC)
                {
                    user.SupportsVC = true;
                    user.SupportsOpusVC = false;
                    this.Panel.Userlist.UpdateUserAppearance(user);
                }
            }
        }

        private void Eval_VC_Supported(TCPPacketReader packet)
        {
            bool supported = ((byte)packet) == 1;

            if (!supported)
            {
                this.CanVC = false;
                this.CanOpusVC = false;
                this.Panel.CanVC(false);
            }
            else if (!this.CanVC)
            {
                this.CanVC = true;
                this.CanOpusVC = false;
                this.Panel.CanVC(true);
                this.sock.Send(TCPOutbound.EnableClips());
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

        private void CustomProtoReceived(TCPPacketReader packet, uint time)
        {
            String command = packet.ReadString(this.crypto);
            String sender = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == sender);

            ulong lag;
            bool b;

            switch (command)
            {
                case "cb0t_writing":
                    if (u == null) return;
                    b = ((byte)packet) == 2;

                    if (b != u.Writing)
                    {
                        u.Writing = b;
                        this.Panel.UpdateWriter(u);
                        ScriptEvents.OnUserWritingStatusChanged(this, u);
                    }
                    break;

                case "cb0t_latency_check":
                    lag = (Helpers.UnixTimeMS - ((ulong)packet));
                    this.Panel.Userlist.UpdateLag(lag);
                    break;

                case "cb0t_latency_mcheck":
                    lag = (Helpers.UnixTimeMS - ((ulong)packet));
                    this.sock.Send(TCPOutbound.Public("Lag test: " + lag + " milliseconds", this.crypto));
                    break;

                case "cb0t_online_status":
                    if (u == null) return;
                    b = ((byte)packet) != 1;

                    if (u.IsAway != b)
                    {
                        u.IsAway = b;
                        this.Panel.Userlist.UpdateUserAppearance(u);
                        ScriptEvents.OnUserOnlineStatusChanged(this, u);
                    }

                    break;

                case "cb0t_nudge":
                    if (u == null) return;
                    this.Eval_Nudge(u, ((byte[])packet), time);
                    break;

                case "cb0t_pm_msg":
                    if (u == null) return;
                    this.Eval_cb0t_pm_msg(u, ((byte[])packet));
                    break;

                case "cb0t_scribble_once":
                    if (u != null)
                    {
                        u.ScribbleBuffer = new List<byte>();
                        u.ScribbleBuffer.AddRange((byte[])packet);
                        this.Eval_Scribble(u);
                    }
                    else if (String.IsNullOrEmpty(sender))
                    {
                        this.unknown_scribble_buffer = new List<byte>();
                        this.unknown_scribble_buffer.AddRange((byte[])packet);
                        this.Eval_Scribble_Unknown();
                    }
                    break;

                case "cb0t_scribble_first":
                    if (u != null)
                    {
                        u.ScribbleBuffer = new List<byte>();
                        u.ScribbleBuffer.AddRange((byte[])packet);
                    }
                    else if (String.IsNullOrEmpty(sender))
                    {
                        this.unknown_scribble_buffer = new List<byte>();
                        this.unknown_scribble_buffer.AddRange((byte[])packet);
                    }
                    break;

                case "cb0t_scribble_chunk":
                    if (u != null)
                        u.ScribbleBuffer.AddRange((byte[])packet);
                    else if (String.IsNullOrEmpty(sender))
                        this.unknown_scribble_buffer.AddRange((byte[])packet);
                    break;

                case "cb0t_scribble_last":
                    if (u != null)
                    {
                        u.ScribbleBuffer.AddRange((byte[])packet);
                        this.Eval_Scribble(u);
                    }
                    else if (String.IsNullOrEmpty(sender))
                    {
                        this.unknown_scribble_buffer.AddRange((byte[])packet);
                        this.Eval_Scribble_Unknown();
                    }
                    break;

                default:
                    if (command.StartsWith("cb3_custom_"))
                    {
                        command = command.Substring(11);

                        if (u != null)
                        {
                            String c_text = Encoding.UTF8.GetString((byte[])packet);
                            ScriptEvents.OnCustomDataReceived(this, u, command, c_text);
                        }
                    }
                    break;
            }
        }

        private void Eval_Scribble(User user)
        {
            byte[] data = user.ScribbleBuffer.ToArray();
            user.ScribbleBuffer = new List<byte>();

            if (Settings.GetReg<bool>("receive_scribbles", true) && !user.Ignored)
                if (ScriptEvents.OnScribbleReceiving(this, user))
                {
                    data = Zip.Decompress(data);

                    if (user.Name == this.users[0].Name)
                        this.Panel.Scribble(data);
                    else
                        this.Panel.PMScribbleReceived(this, user, user.Name, data);

                    this.Panel.CheckUnreadStatus();
                    ScriptEvents.OnScribbleReceived(this, user);
                }
        }

        private void Eval_Scribble_Unknown()
        {
            byte[] data = this.unknown_scribble_buffer.ToArray();
            this.unknown_scribble_buffer = new List<byte>();

            if (Settings.GetReg<bool>("receive_scribbles", true))
            {
                data = Zip.Decompress(data);
                this.Panel.Scribble(data);
            }
        }

        private void Eval_cb0t_pm_msg(User user, byte[] data)
        {
            if (!Settings.GetReg<bool>("can_receive_pms", true))
                return;

            String name = user.Name;
            String text = Encoding.UTF8.GetString(PMCrypto.SoftDecrypt(this.MyName, data));
            User u = this.users.Find(x => x.Name == name);

            if (u == null)
                return;

            if (u.Ignored)
                return;

            AresFont font = null;

            if (u.Font != null)
                font = u.Font;

            if (ScriptEvents.OnPmReceiving(this, u, text))
            {
                this.Panel.PMTextReceived(this, u, name, text, font, PMTextReceivedType.Text);
                this.Panel.CheckUnreadStatus();
                ScriptEvents.OnPmReceived(this, u, text);
            }
        }

        private void Eval_Nudge(User user, byte[] data, uint time)
        {
            if (data.Length == 4)
                if (data.SequenceEqual(new byte[] { 78, 65, 61, 61 }))
                {
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + user.Name + " is not receiving nudges");
                    return;
                }

            if (Settings.GetReg<bool>("receive_nudge", true) && !user.Ignored)
            {
                if (time > this.last_nudge)
                    this.last_nudge = time;
                else return;

                if (ScriptEvents.OnNudgeReceiving(this, user))
                {
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + user.Name + " has nudged you!");
                    this.owner_frm.Nudge();
                    this.ShowPopup("cb0t :: Nudge", user.Name + " nudged you", PopupSound.None);
                }
                else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
            }
            else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
        }
    }
}
