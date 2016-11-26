using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

                case TCPMsg.MSG_CHAT_SERVER_HTML:
                    this.Eval_HTML(e.Packet);
                    break;
            }
        }

        private bool IsMOTDReceiving { get; set; }

        private void Eval_HTML(TCPPacketReader packet)
        {
            if (!Settings.CanHTML)
                return;

            String html = packet.ReadString();

            if (html == "<!--MOTDSTART-->")
            {
                this.Panel.SetScreenWidth(true);
                this.IsMOTDReceiving = true;
            }
            else if (html == "<!--MOTDEND-->")
            {
                this.Panel.SetScreenWidth(false);
                this.IsMOTDReceiving = false;
            }
            else
            {
                /* SANDBOXED CUSTOM HTML CONTENT */

                //prevent character reference injection
                html = WebUtility.HtmlDecode(html);

                //find 1 tag, and 1 tag only
                var opts = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                var regex = new Regex("<!--(?<tag>EMBEDYOUTUBE):(?<ex>.*?)-->|<(?<tag>.*?) (?<ex>.*?)>", opts);

                var match = regex.Match(html);
                if (match.Success) {

                    string tag = match.Groups[1].Value.ToLower();
                    string extra = Regex.Replace(match.Groups[2].Value, "on[a-z0-9_$]+?.*?=[^\\\\]*?\"[^\\\\]*?\"", "", opts);

                    switch (tag) {
                        case "img":
                            this.Panel.ShowCustomHTML("<img onload=\"imageLoaded(this)\" " + extra + ">");
                            break;
                        case "audio":
                            if (this.IsMOTDReceiving)
                                this.Panel.ShowCustomHTML("<audio " + extra + ">");
                            break;
                        case "video":
                            if (this.IsMOTDReceiving)
                                this.Panel.ShowCustomHTML("<video " + extra + ">");
                            break;
                        case "embedyoutube": 
                            if (this.IsMOTDReceiving) {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<iframe width=\"420\" height=\"315\" src=\"http://www.youtube.com/embed/");
                                sb.Append(Uri.EscapeUriString(extra));//more injection protection
                                sb.Append("\" frameborder=\"0\" allowfullscreen></iframe>");
                                this.Panel.ShowCustomHTML(sb.ToString());
                                sb.Clear();
                            }
                            break;
                        case "object"://do nothing with object tags
                            break;
                        default:
                            // let the script engine decide...
                            ScriptEvents.OnHTMLReceived(this, html);
                            break;
                    }
                }
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
            Scripting.ScriptManager.ClearUsers(this.EndPoint);
            this.Panel.ServerText(StringTemplate.Get(STType.Messages, 18) + "...");
            this.Panel.CanVC(false);
            this.CanVC = false;
            this.CanOpusVC = false;
            this.MyName = packet.ReadString(this.crypto);
            this.Panel.MyName = this.MyName;

            if (packet.Remaining > 0)
            {
                String room_name = packet.ReadString(this.crypto);
                bool update_name = false;

                if (!String.IsNullOrEmpty(this.Credentials.Name))
                    if (this.Credentials.Name != room_name)
                        if (this.EndPoint.Equals(this.Credentials.ToEndPoint()))
                            update_name = true;

                this.Credentials.Name = room_name;

                if (update_name)
                    this.RoomNameChanged(this.Credentials, EventArgs.Empty);
            }

            this.Panel.Userlist.MyLevel = 0;
            this.is_writing = false;
            this.Panel.ClearWriters();
        }

        private bool should_check_for_current_topic_update = false;

        private void Eval_Features(TCPPacketReader packet)
        {
            String version = packet.ReadString(this.crypto);
            this.Credentials.Server = version;
            this.Panel.ServerText(StringTemplate.Get(STType.Messages, 17) + ": " + version);
            this.Panel.Userlist.UpdateServerVersion(version);
            this.should_check_for_current_topic_update = true;

            if (version.StartsWith("sb0t 5."))
            {
                version = version.Substring(version.IndexOf(" ") + 1).Split(' ')[0];
                String vnum_str = new String(version.Where(x => Char.IsNumber(x)).ToArray());

                uint vnum;

                if (!uint.TryParse(vnum_str, out vnum))
                    vnum = 0;

                this.new_sbot = (vnum >= 514);
            }
            else if (version.StartsWith("Ares 2.") || version.StartsWith("Ares_2."))
                this.new_sbot = true; // maybe future Ares Server will support cb0t Custom Fonts?

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
            this.CanNP = true;

            if (has_html)
                this.Panel.Userlist.AcquireServerIcon(this.EndPoint);

            packet.SkipByte();
            this.Panel.ServerText(StringTemplate.Get(STType.Messages, 16) + ": " + (RoomLanguage)((byte)packet));
            uint cookie = packet;

            if (!String.IsNullOrEmpty(this.Credentials.Password))
                this.sock.Send(TCPOutbound.SecureAdminLogin(this.Credentials.Password, cookie, this.Credentials.IP));

            this.UpdatePersonalMessage();

            if (Avatar.Data != null)
                this.sock.SendTrickle(TCPOutbound.Avatar());

            if (Settings.GetReg<bool>("user_font_enabled", false))
                this.sock.SendTrickle(TCPOutbound.Font(this.new_sbot, this.crypto));

            if (Settings.GetReg<bool>("block_custom_names", false))
                this.sock.SendTrickle(TCPOutbound.BlockCustomNames(true));

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
            String str = packet.ReadString(this.crypto);
            str = str.Replace("\r\n", "\0");
            str = str.Replace("\r", "\0");
            str = str.Replace("\n", "\0");
            String[] lines = str.Split(new String[] { "\0" }, StringSplitOptions.None);
            String text;

            foreach (String l in lines)
            {
                text = l;
                text = ScriptEvents.OnAnnounceReceiving(this, text);

                if (text != null)
                {
                    this.Panel.AnnounceText(text);
                    ScriptEvents.OnAnnounceReceived(this, text);
                }
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
                this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 15).Replace("+x", redirect.Name));
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
            String check = addr.ToUpper();

            if (check.StartsWith("HTTP://") || check.StartsWith("ARLNK://") || check.StartsWith("WWW") || check.StartsWith("HTTPS://"))
            {
                String text = packet.ReadString(this.crypto);
                text = ScriptEvents.OnUrlReceiving(this, text, addr);

                if (!String.IsNullOrEmpty(text))
                    this.Panel.SetURL(text, addr);
            }
        }

        private void Eval_Avatar(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            User u = this.users.Find(x => x.Name == name);

            if (u != null)
            {
                if (!ScriptEvents.OnUserAvatarReceiving(this, u))
                    return;

                byte[] data = packet;

                if (data.Length <= 10)
                    u.ClearAvatar();
                else
                    u.SetAvatar(data);

                this.Panel.Userlist.UpdateUserAppearance(u);
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

            User ghost = this.users.Find(x => x.Name == u.Name);

            if (ghost != null)
            {
                this.users.RemoveAll(x => x.Name == ghost.Name);

                if (ghost.Writing)
                {
                    ghost.Writing = false;
                    this.Panel.UpdateWriter(ghost);
                }

                this.Panel.Userlist.RemoveUserItem(ghost);

                if (ScriptEvents.OnUserParting(this, ghost))
                    this.Panel.AnnounceText(GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Part, this.BlackBG) + StringTemplate.Get(STType.Messages, 12).Replace("+x", ghost.Name));

                ScriptEvents.OnUserParted(this, ghost);
                Scripting.ScriptManager.RemoveUser(this.EndPoint, u);
                ghost.Dispose();
                ghost = null;
            }

            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);
            Scripting.ScriptManager.AddUser(this.EndPoint, u);

            if (ScriptEvents.OnUserJoining(this, u))
                this.Panel.AnnounceText(GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Join, this.BlackBG) + StringTemplate.Get(STType.Messages, 13).Replace("+x", u.Name));

            if (u.Name == this.MyName)
            {
                u.IsAway = Settings.IsAway;
                this.Panel.Userlist.MyLevel = u.Level;
            }

            ScriptEvents.OnUserJoined(this, u);

            if (u.IsFriend)
                if (!Settings.GetReg<bool>("block_friend_popup", false))
                    this.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 4), StringTemplate.Get(STType.Messages, 14).Replace("+x", u.Name).Replace("+y", this.Credentials.Name), PopupSound.Friend);
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
                    this.Panel.AnnounceText(GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Part, this.BlackBG) + StringTemplate.Get(STType.Messages, 12).Replace("+x", u.Name));

                ScriptEvents.OnUserParted(this, u);
                Scripting.ScriptManager.RemoveUser(this.EndPoint, u);
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

            if (this.users.Find(x => x.Name == u.Name) != null)
                return;

            this.users.Add(u);
            this.Panel.Userlist.AddUserItem(u);
            Scripting.ScriptManager.AddUser(this.EndPoint, u);

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
            String str = ScriptEvents.OnTopicReceiving(this, text);

            if (updated)
            {
                this.Panel.ServerText(StringTemplate.Get(STType.Messages, 11) + ": " + text);
                this.Credentials.Topic = text;
                this.Panel.SetTopic(text);
            }
            else if (!String.IsNullOrEmpty(str))
            {
                this.Credentials.Topic = str;
                this.Panel.SetTopic(text);
            }

            if (this.should_check_for_current_topic_update)
            {
                this.should_check_for_current_topic_update = false;
                this.TopicChanged(this.Credentials, EventArgs.Empty);
            }
        }

        private void Eval_IsIgnoringYou(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            this.Panel.PMTextReceived(null, null, name, StringTemplate.Get(STType.Messages, 10), null, PMTextReceivedType.Announce);
        }

        private void Eval_OfflineUser(TCPPacketReader packet)
        {
            String name = packet.ReadString(this.crypto);
            this.Panel.PMTextReceived(null, null, name, StringTemplate.Get(STType.Messages, 9), null, PMTextReceivedType.Announce);
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

            text = ScriptEvents.OnPmReceiving(this, u, text);

            if (!String.IsNullOrEmpty(text))
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

            text = ScriptEvents.OnTextReceiving(this, name, text);

            if (!String.IsNullOrEmpty(text))
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

            text = ScriptEvents.OnEmoteReceiving(this, name, text);

            if (!String.IsNullOrEmpty(text))
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

                case TCPMsg.MSG_CHAT_SERVER_FAVICON:
                    this.Eval_FavIcon(e.Packet);
                    break;

                case TCPMsg.MSG_CHAT_SERVER_NOW_PLAYING_EVENT:
                    this.CanNP = ((byte)e.Packet) == 1;
                    break;
            }
        }

        private void Eval_FavIcon(TCPPacketReader packet)
        {
            byte[] buf = packet;
            this.Panel.Userlist.HereFavicon(buf);
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
                            if (ScriptEvents.OnVoiceClipReceiving(this, u, true))
                            {
                                this.Panel.PMTextReceived(this, u, vc.Sender, GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Server, this.BlackBG) + "--- \\\\voice_clip_#" + vc.ShortCut + " " + StringTemplate.Get(STType.Messages, 8).Replace("+x", vc.Sender), null, PMTextReceivedType.Announce);
                                ScriptEvents.OnVoiceClipReceived(this, u, true);
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
                                if (ScriptEvents.OnVoiceClipReceiving(this, u, true))
                                {
                                    this.Panel.PMTextReceived(this, u, vc.Sender, GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Server, this.BlackBG) + "--- \\\\voice_clip_#" + vc.ShortCut + " " + StringTemplate.Get(STType.Messages, 8).Replace("+x", vc.Sender), null, PMTextReceivedType.Announce);
                                    ScriptEvents.OnVoiceClipReceived(this, u, true);
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
                            if (ScriptEvents.OnVoiceClipReceiving(this, u, false))
                            {
                                if (this.CanAutoPlayVC)
                                {
                                    vc.Auto = true;
                                    VoicePlayer.QueueItem(vc);
                                }
                                else this.Panel.ShowVoice(vc.Sender, vc.ShortCut);

                                ScriptEvents.OnVoiceClipReceived(this, u, false);
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
                                if (ScriptEvents.OnVoiceClipReceiving(this, u, false))
                                {
                                    if (this.CanAutoPlayVC)
                                    {
                                        vc.Auto = true;
                                        VoicePlayer.QueueItem(vc);
                                    }
                                    else this.Panel.ShowVoice(vc.Sender, vc.ShortCut);

                                    ScriptEvents.OnVoiceClipReceived(this, u, false);
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
            {
                if (packet.Remaining > 2)
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

                    if (ScriptEvents.OnUserFontChanging(this, u, f))
                        u.Font = f;
                }
                else u.Font = null;
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
                    this.sock.Send(TCPOutbound.Public(StringTemplate.Get(STType.Messages, 7) + ": " + lag + " milliseconds", this.crypto));
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
                    else if (!user.Ignored)
                        this.Panel.PMScribbleReceived(this, user, user.Name, data);
                    else return;

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

            text = ScriptEvents.OnPmReceiving(this, u, text);

            if (!String.IsNullOrEmpty(text))
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
                    this.Panel.AnnounceText(GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Server, this.BlackBG) + "--- " + StringTemplate.Get(STType.Messages, 6).Replace("+x", user.Name));
                    return;
                }

            if (Settings.GetReg<bool>("receive_nudge", true) && !user.Ignored)
            {
                if (time > this.last_nudge)
                    this.last_nudge = time;
                else return;

                if (ScriptEvents.OnNudgeReceiving(this, user))
                {
                    this.Panel.AnnounceText(GlobalSettings.GetDefaultColorString(GlobalSettings.DefaultColorType.Server, this.BlackBG) + "--- " + StringTemplate.Get(STType.Messages, 5).Replace("+x", user.Name));
                    this.owner_frm.Nudge();
                    this.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 3), StringTemplate.Get(STType.Messages, 5).Replace("+x", user.Name), PopupSound.None);
                }
                else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
            }
            else this.sock.Send(TCPOutbound.NudgeReject(user.Name, this.crypto));
        }
    }
}
