using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Jurassic.Library;

namespace cb0t
{
    partial class Room
    {
        public ChannelButton Button { get; set; }
        public FavouritesListItem Credentials { get; set; }
        public RoomPanel Panel { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public String MyName = String.Empty;
        public bool RoomIsVisible { get; set; }
        public bool CanNP { get; set; }

        private CryptoService crypto = new CryptoService();
        private SessionState state = SessionState.Sleeping;
        private int reconnect_count = 0;
        private AresSocket sock = new AresSocket();
        private List<byte> unknown_scribble_buffer = new List<byte>();
        private uint ticks = 0;
        private uint last_lag = 0;
        private uint last_nudge = 0;
        private List<User> users = new List<User>();
        private bool new_sbot = false;
        private Form1 owner_frm = null;
        private bool CanAutoPlayVC { get; set; }
        private bool CanVC { get; set; }
        private bool CanOpusVC { get; set; }
        private bool BlackBG { get; set; }
        private bool ScriptObjectCreated { get; set; }

        public Room(uint time, FavouritesListItem item, Form1 f, bool black_bg)
        {
            this.owner_frm = f;
            this.Credentials = item.Copy();
            this.Credentials.Server = String.Empty;
            this.EndPoint = new IPEndPoint(item.IP, item.Port);
            this.ticks = (time - 19);
            this.sock.PacketReceived += this.PacketReceived;
            this.BlackBG = black_bg;
        }

        public void UpdateIgnoreFromScripting(User u)
        {
            if (this.state == SessionState.Connected)
                if (this.sock != null)
                    this.sock.Send(TCPOutbound.Ignore(u.Name, u.Ignored, this.crypto));
        }

        public void DeleteAvatarFromScripting(User u)
        {
            u.ClearAvatar();
            this.Panel.Userlist.UpdateUserAppearance(u);
        }

        public User[] UserPool
        {
            get { return this.users.ToArray(); }
        }

        public void UpdateTemplate()
        {
            this.Panel.Userlist.UpdateTemplate();
            this.Panel.UpdateTemplate();
        }

        public void ConnectEvents()
        {
            this.Panel.SendBox.KeyDown += this.SendBoxKeyDown;
            this.Panel.CancelWriting += this.CancelWriting;
            this.Panel.SendBox.KeyUp += this.SendBoxKeyUp;
            this.Panel.SendAutoReply += this.SendAutoReply;
            this.Panel.Userlist.OpenPMRequested += this.OpenPMRequested;
            this.Panel.Userlist.SendAdminCommand += this.SendAdminCommand;
            this.Panel.Userlist.MenuTask += this.UserlistMenuTask;
            this.Panel.Userlist.CustomOptionClicked += this.UserlistCustomOptionClicked;
            this.Panel.WantScribble += this.WantScribble;
            this.Panel.RoomMenuItemClicked += this.RoomMenuItemClicked;
            this.Panel.RoomMenuJSItemClicked += this.RoomMenuJSItemClicked;
            this.Panel.HashlinkClicked += this.PanelHashlinkClicked;
            this.Panel.GetUserByName += this.PanelGetUserByName;
            this.Panel.VoiceRecordingButtonClicked += this.VoiceRecordingButtonClicked;
            this.Panel.EditScribbleClicked += this.EditScribbleClicked;

            if (this.BlackBG)
            {
                this.Panel.Userlist.SetBlack();
                this.Panel.SetToBlack();
            }

            this.UpdateTemplate();
        }

        private void RoomMenuJSItemClicked(object sender, RoomMenuJSItemClickedEventArgs e)
        {
            Scripting.CustomJSRoomMenuCallback cb = e.Item;
            cb.Room = this.EndPoint;
            Scripting.ScriptManager.PendingScriptingCallbacks.Enqueue(cb);
        }

        private void VoiceRecordingButtonClicked(object sender, EventArgs e)
        {
            if ((bool)sender)
                this.StartRecording();
            else
                this.StopRecording();
        }

        private bool is_recording = false;
        private int recording_time = -1;
        private uint last_recording_start_time = 0;

        public void StartRecording()
        {
            if (!VoiceRecorder.Recording)
            {
                if (Helpers.UnixTime >= (this.last_recording_start_time + 5))
                    this.last_recording_start_time = Helpers.UnixTime;
                else return;

                VoiceRecorder.RecordStart();
                this.is_recording = true;
                this.recording_time = 0;
                this.Panel.UpdateVoiceTime(this.recording_time);
                this.owner_frm.SetProgressLevel(0);
            }
        }

        public void StopRecording()
        {
            if (VoiceRecorder.Recording)
            {
                VoiceRecorder.RecordStop();
                this.is_recording = false;
                byte rec_len = (byte)this.recording_time;
                this.recording_time = 0;
                this.Panel.UpdateVoiceTime(-1);
                this.owner_frm.SetProgressLevel(0);
                bool should_opus = this.CanOpusVC;

                if (should_opus)
                    should_opus = Settings.GetReg<bool>("can_opus", true);

                if (should_opus)
                    if (this.Panel.Mode == ScreenMode.PM)
                    {
                        should_opus = false;
                        User user = this.users.Find(x => x.Name == this.Panel.PMName);

                        if (user != null)
                            should_opus = user.SupportsOpusVC;
                    }

                if (this.sock != null)
                    if (this.state == SessionState.Connected)
                    {
                        if (this.Panel.Mode == ScreenMode.PM)
                        {
                            byte[][] packets = VoiceRecorder.GetPackets(this.Panel.PMName, should_opus ? VoiceRecorderSendMethod.Opus : VoiceRecorderSendMethod.Zip, rec_len, this.crypto);

                            foreach (byte[] p in packets)
                                this.sock.SendTrickle(p);

                            this.Panel.MyPMAnnounce((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 28) + "...");
                        }
                        else
                        {
                            byte[][] packets = VoiceRecorder.GetPackets(should_opus ? VoiceRecorderSendMethod.Opus : VoiceRecorderSendMethod.Zip, rec_len);

                            foreach (byte[] p in packets)
                                this.sock.SendTrickle(p);

                            this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 28) + "...");
                        }
                    }
            }
        }

        public void CancelRecording()
        {
            if (VoiceRecorder.Recording)
            {
                VoiceRecorder.RecordCancel();
                this.is_recording = false;
                this.recording_time = 0;
                this.Panel.UpdateVoiceTime(-1);
                this.owner_frm.SetProgressLevel(0);

                if (this.Panel.Mode == ScreenMode.PM)
                    this.Panel.MyPMAnnounce((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 27));
                else
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 27));
            }
        }

        public void VCTick()
        {
            if (this.is_recording)
            {
                this.recording_time++;
                this.Panel.UpdateVoiceTime(this.recording_time);
                this.owner_frm.SetProgressLevel(this.recording_time);

                if (this.recording_time == 15)
                    this.StopRecording();
            }
        }

        private User PanelGetUserByName(String name)
        {
            if (this.users == null)
                return null;

            return this.users.Find(x => x.Name == name);
        }

        public void SpellCheck()
        {
            this.Panel.SpellCheck();
        }

        private void PanelHashlinkClicked(object sender, EventArgs e)
        {
            this.owner_frm.JoinFromHashlinkClicked(sender, e);
        }

        private void UserlistCustomOptionClicked(object sender, EventArgs e)
        {
            if (this.state != SessionState.Connected)
                return;

            if (sender is Scripting.CustomJSUserListMenuCallback)
            {
                Scripting.CustomJSUserListMenuCallback cb = (Scripting.CustomJSUserListMenuCallback)sender;
                cb.Room = this.EndPoint;
                Scripting.ScriptManager.PendingScriptingCallbacks.Enqueue(cb);
            }
            else
            {
                String text = (String)sender;

                if (text.StartsWith("/me ") && text.Length > 4)
                    this.sock.Send(TCPOutbound.Emote(text.Substring(4), this.crypto));
                else if (text.StartsWith("/") && text.Length > 1)
                    this.sock.Send(TCPOutbound.Command(text.Substring(1), this.crypto));
                else if (text.Length > 0)
                    this.sock.Send(TCPOutbound.Public(text, this.crypto));
            }
        }

        private void RoomMenuItemClicked(object sender, RoomMenuItemClickedEventArgs e)
        {
            if (e.Item == RoomMenuItem.ExportHashlink)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(this.Credentials.Name);
                sb.Append("arlnk://");
                sb.AppendLine(Hashlink.EncodeHashlink(this.Credentials));

                try
                {
                    File.WriteAllText(Settings.DataPath + "hashlink.txt", sb.ToString());
                    Process.Start("notepad.exe", Settings.DataPath + "hashlink.txt");
                }
                catch { }
            }
            else if (e.Item == RoomMenuItem.AddToFavourites)
                this.owner_frm.AddToFavourite(this.Credentials);
            else if (e.Item == RoomMenuItem.CopyRoomName)
            {
                try { Clipboard.SetText(this.Credentials.Name); }
                catch { }
            }
            else if (e.Item == RoomMenuItem.AutoPlayVoiceClips)
                this.CanAutoPlayVC = (bool)e.Arg;
            else if (e.Item == RoomMenuItem.CloseSubTabs)
                this.Panel.CloseAllTabs(false);
            else if (e.Item == RoomMenuItem.Custom)
            {
                if (this.state != SessionState.Connected)
                    return;

                String text = e.Arg.ToString();

                if (text.StartsWith("/me ") && text.Length > 4)
                    this.sock.Send(TCPOutbound.Emote(text.Substring(4).Replace("+n", this.Credentials.Name), this.crypto));
                else if (text.StartsWith("/") && text.Length > 1)
                    this.sock.Send(TCPOutbound.Command(text.Substring(1).Replace("+n", this.Credentials.Name), this.crypto));
                else if (text.Length > 0)
                    this.sock.Send(TCPOutbound.Public(text.Replace("+n", this.Credentials.Name), this.crypto));
            }
        }

        public void UpdateAwayStatus(bool away)
        {
            if (this.state == SessionState.Connected)
            {
                this.sock.SendTrickle(TCPOutbound.OnlineStatus(away, this.crypto));
                User u = this.users.Find(x => x.Name == this.MyName);

                if (u != null)
                {
                    u.IsAway = away;
                    this.Panel.Userlist.UpdateUserAppearance(u);
                }
            }
        }

        public void UpdateBlockCustomNamesStatus(bool block)
        {
            if (this.state == SessionState.Connected)
                if (this.sock != null)
                    this.sock.SendTrickle(TCPOutbound.BlockCustomNames(block));
        }

        public void CancelWritingStatus()
        {
            this.Panel.UpdateMyWriting(this.MyName, false);

            if (this.state == SessionState.Connected)
                this.sock.Send(TCPOutbound.Writing(false, this.crypto));
        }

        private void EditScribbleClicked(object sender, EventArgs e)
        {
            if (this.state != SessionState.Connected)
                return;

            SharedUI.ScribbleEditor.StartPosition = FormStartPosition.CenterParent;
            byte[] data = SharedUI.ScribbleEditor.GetScribble((byte[])sender);

            if (data == null)
                return;

            if (this.Panel.Mode == ScreenMode.Main)
            {
                this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 26) + "...");
                this.Panel.Scribble(data);
            }
            else if (this.Panel.Mode == ScreenMode.PM)
            {
                this.Panel.PMTextReceived(null, null, this.Panel.PMName, (this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 26) + "...", null, PMTextReceivedType.Announce);
                this.Panel.PMScribbleReceived(null, null, this.Panel.PMName, data);
            }

            data = Zip.Compress(data);

            List<byte> full = new List<byte>(data);
            data = null;

            if (full.Count <= 4000)
            {
                if (this.Panel.Mode == ScreenMode.Main)
                    this.sock.SendTrickle(TCPOutbound.ScribbleRoomFirst((uint)full.Count, 0, full.ToArray()));
                else if (this.Panel.Mode == ScreenMode.PM)
                    this.sock.SendTrickle(TCPOutbound.PMScribbleOnce(this.Panel.PMName, full.ToArray(), this.crypto));
            }
            else
            {
                List<byte[]> p = new List<byte[]>();
                uint s_size = (uint)full.Count;

                while (full.Count > 4000)
                {
                    p.Add(full.GetRange(0, 4000).ToArray());
                    full.RemoveRange(0, 4000);
                }

                if (full.Count > 0)
                    p.Add(full.ToArray());

                if (this.Panel.Mode == ScreenMode.Main)
                {
                    for (int i = 0; i < p.Count; i++)
                        if (i == 0)
                            this.sock.SendTrickle(TCPOutbound.ScribbleRoomFirst(s_size, (ushort)(p.Count - 1), p[i]));
                        else
                            this.sock.SendTrickle(TCPOutbound.ScribbleRoomChunk(p[i]));
                }
                else if (this.Panel.Mode == ScreenMode.PM)
                {
                    for (int i = 0; i < p.Count; i++)
                        if (i == 0)
                            this.sock.SendTrickle(TCPOutbound.PMScribbleFirst(this.Panel.PMName, p[i], this.crypto));
                        else if (i == (p.Count - 1))
                            this.sock.SendTrickle(TCPOutbound.PMScribbleLast(this.Panel.PMName, p[i], this.crypto));
                        else
                            this.sock.SendTrickle(TCPOutbound.PMScribbleChunk(this.Panel.PMName, p[i], this.crypto));
                }

                p.Clear();
                p = null;
            }

            full.Clear();
            full = null;
        }

        private void WantScribble(object sender, EventArgs e)
        {
            if (this.state != SessionState.Connected)
                return;

            SharedUI.ScribbleEditor.StartPosition = FormStartPosition.CenterParent;
            byte[] data = SharedUI.ScribbleEditor.GetScribble();

            if (data == null)
                return;

            if (this.Panel.Mode == ScreenMode.Main)
            {
                this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 26) + "...");
                this.Panel.Scribble(data);
            }
            else if (this.Panel.Mode == ScreenMode.PM)
            {
                this.Panel.PMTextReceived(null, null, this.Panel.PMName, (this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.Messages, 26) + "...", null, PMTextReceivedType.Announce);
                this.Panel.PMScribbleReceived(null, null, this.Panel.PMName, data);
            }

            data = Zip.Compress(data);

            List<byte> full = new List<byte>(data);
            data = null;

            if (full.Count <= 4000)
            {
                if (this.Panel.Mode == ScreenMode.Main)
                    this.sock.SendTrickle(TCPOutbound.ScribbleRoomFirst((uint)full.Count, 0, full.ToArray()));
                else if (this.Panel.Mode == ScreenMode.PM)
                    this.sock.SendTrickle(TCPOutbound.PMScribbleOnce(this.Panel.PMName, full.ToArray(), this.crypto));
            }
            else
            {
                List<byte[]> p = new List<byte[]>();
                uint s_size = (uint)full.Count;

                while (full.Count > 4000)
                {
                    p.Add(full.GetRange(0, 4000).ToArray());
                    full.RemoveRange(0, 4000);
                }

                if (full.Count > 0)
                    p.Add(full.ToArray());

                if (this.Panel.Mode == ScreenMode.Main)
                {
                    for (int i = 0; i < p.Count; i++)
                        if (i == 0)
                            this.sock.SendTrickle(TCPOutbound.ScribbleRoomFirst(s_size, (ushort)(p.Count - 1), p[i]));
                        else
                            this.sock.SendTrickle(TCPOutbound.ScribbleRoomChunk(p[i]));
                }
                else if (this.Panel.Mode == ScreenMode.PM)
                {
                    for (int i = 0; i < p.Count; i++)
                        if (i == 0)
                            this.sock.SendTrickle(TCPOutbound.PMScribbleFirst(this.Panel.PMName, p[i], this.crypto));
                        else if (i == (p.Count - 1))
                            this.sock.SendTrickle(TCPOutbound.PMScribbleLast(this.Panel.PMName, p[i], this.crypto));
                        else
                            this.sock.SendTrickle(TCPOutbound.PMScribbleChunk(this.Panel.PMName, p[i], this.crypto));
                }

                p.Clear();
                p = null;
            }

            full.Clear();
            full = null;
        }

        public void NudgeUser(User u)
        {
            if (this.state == SessionState.Connected)
                if (this.sock != null)
                    this.sock.Send(TCPOutbound.Nudge(this.MyName, u.Name, this.crypto));
        }

        private void UserlistMenuTask(object sender, ULCTXTaskEventArgs e)
        {
            String name = (String)sender;

            if (e.Task == ULCTXTask.AddRemoveFriend)
                Friends.FriendStatusChanged(name);
            else if (e.Task == ULCTXTask.Browse)
            {
                if (this.state != SessionState.Connected)
                    return;

                if (this.users.Find(x => x.HasFiles && x.Name == name) != null)
                {
                    ushort bid = Helpers.BrowseIdent;

                    if (this.Panel.CreateFileBrowseTab(name, bid))
                        this.sock.Send(TCPOutbound.Browse(name, bid, this.crypto));
                }
            }
            else if (e.Task == ULCTXTask.CopyName)
            {
                try
                {
                    Clipboard.SetText(name);
                }
                catch { }
            }
            else if (e.Task == ULCTXTask.IgnoreUnignore)
            {
                if (this.state != SessionState.Connected)
                    return;

                User u = this.users.Find(x => x.Name == name);

                if (u != null)
                {
                    u.Ignored = !u.Ignored;
                    this.sock.Send(TCPOutbound.Ignore(name, u.Ignored, this.crypto));
                }
            }
            else if (e.Task == ULCTXTask.Nudge)
            {
                if (this.state != SessionState.Connected)
                    return;

                User u = this.users.Find(x => x.Name == name);

                if (u != null)
                {
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- " + StringTemplate.Get(STType.UserList, 1) + " " + u.Name);
                    this.sock.Send(TCPOutbound.Nudge(this.MyName, name, this.crypto));
                }
            }
            else if (e.Task == ULCTXTask.Whois)
            {
                User u = this.users.Find(x => x.Name == name);

                if (u != null)
                {
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- Whois: " + u.Name);
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- ASL: " + u.ToASLString());
                    this.Panel.AnnounceText((this.BlackBG ? "\x000315" : "\x000314") + "--- Personal Message: " + u.PersonalMessage);
                }
            }
        }

        private void SendAdminCommand(object sender, EventArgs e)
        {
            if (this.state == SessionState.Connected)
                if (this.sock != null)
                    this.sock.Send(TCPOutbound.Command((String)sender, this.crypto));
        }

        public void FriendStatusChanged(String name, bool friend)
        {
            if (this.users != null)
            {
                User u = this.users.Find(x => x.Name == name);

                if (u != null)
                {
                    u.IsFriend = friend;
                    this.Panel.Userlist.UpdateUserFriendship(u);
                }
            }
        }

        public void ScrollAndFocus()
        {
            this.Panel.SendBox.BeginInvoke((Action)(() => this.Panel.SendBox.Focus()));
            this.Panel.ScrollDown();
        }

        public void Release()
        {
            Scripting.ScriptManager.PendingTerminators.Enqueue(this.EndPoint);
            this.owner_frm = null;
            this.Panel.VoiceRecordingButtonClicked -= this.VoiceRecordingButtonClicked;
            this.Panel.GetUserByName -= this.PanelGetUserByName;
            this.Panel.SendBox.KeyDown -= this.SendBoxKeyDown;
            this.Panel.CancelWriting -= this.CancelWriting;
            this.Panel.SendBox.KeyUp -= this.SendBoxKeyUp;
            this.Panel.SendAutoReply -= this.SendAutoReply;
            this.Panel.RoomMenuItemClicked -= this.RoomMenuItemClicked;
            this.Panel.RoomMenuJSItemClicked -= this.RoomMenuJSItemClicked;
            this.Panel.Userlist.OpenPMRequested -= this.OpenPMRequested;
            this.Panel.Userlist.SendAdminCommand -= this.SendAdminCommand;
            this.Panel.Userlist.MenuTask -= this.UserlistMenuTask;
            this.Panel.Userlist.CustomOptionClicked -= this.UserlistCustomOptionClicked;
            this.Panel.HashlinkClicked -= this.PanelHashlinkClicked;
            this.Panel.WantScribble -= this.WantScribble;
            this.Panel.EditScribbleClicked -= this.EditScribbleClicked;
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
            this.users = new List<User>();
            this.unknown_scribble_buffer.Clear();
            this.unknown_scribble_buffer = new List<byte>();
        }

        public void UpdatePersonalMessage()
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.SendTrickle(TCPOutbound.PersonalMessage(this.crypto));
        }

        public void UpdateAvatar()
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
            {
                if (Avatar.Data == null)
                    this.sock.SendTrickle(TCPOutbound.ClearAvatar());
                else
                    this.sock.SendTrickle(TCPOutbound.Avatar());
            }
        }

        public void UpdateFont()
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.SendTrickle(TCPOutbound.Font(this.new_sbot, this.crypto));
        }

        public void ShowAnnounceText(String text)
        {
            if (this.sock != null)
                this.Panel.AnnounceText(text);
        }

        public void SendText(String text)
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.Send(TCPOutbound.Public(text, this.crypto));
        }

        public void SendEmote(String text)
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.Send(TCPOutbound.Emote(text, this.crypto));
        }

        public void SendCommand(String text)
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.Send(TCPOutbound.Command(text, this.crypto));
        }

        public void SendPersonalMessage(String text)
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.SendTrickle(TCPOutbound.PersonalMessage(text, this.crypto));
        }

        public void SendPersonalMessage()
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.SendTrickle(TCPOutbound.PersonalMessage(this.crypto));
        }

        public void SendCustomData(String ident, String data)
        {
            if (this.state != SessionState.Connected)
                return;

            if (this.sock != null)
                this.sock.Send(TCPOutbound.CustomCbotData(ident, Encoding.UTF8.GetBytes(data), this.crypto));
        }

        public void ShowPopup(String title, String msg, PopupSound sound)
        {
            if (Settings.GetReg<bool>("can_popup", true))
                this.owner_frm.BeginInvoke((Action)(() => this.owner_frm.ShowPopup(title, msg, this.EndPoint, sound)));
        }

        public void TrickleTasks()
        {
            if (this.sock == null)
                return;

            if (this.state == SessionState.Connected)
                this.sock.DequeueTrickle();
        }

        public void SocketTasks(uint time)
        {
            if (this.sock == null)
                return;

            if (this.state == SessionState.Sleeping)
            {
                if (time >= (this.ticks + 20))
                {
                    if (!this.ScriptObjectCreated)
                    {
                        this.ScriptObjectCreated = true;
                        Scripting.ScriptManager.AddRoom(this.EndPoint);
                    }

                    this.state = SessionState.Connecting;
                    this.sock.Connect(new IPEndPoint(this.Credentials.IP, this.Credentials.Port));
                    this.ticks = time;
                    this.crypto.Mode = CryptoMode.Unencrypted;
                    this.Panel.Userlist.SetCrypto(false);
                    this.Panel.CanScribbleAll(false);
                    this.Panel.CanScribblePM(false);
                    this.Panel.InitScribbleButton();
                    this.new_sbot = false;

                    if (this.reconnect_count > 0)
                        this.Panel.ServerText(StringTemplate.Get(STType.Messages, 25) + "... #" + this.reconnect_count);
                    else
                        this.Panel.ServerText(StringTemplate.Get(STType.Messages, 25) + "...");

                    ScriptEvents.OnConnecting(this);
                }
            }
            else if (this.state == SessionState.Connecting)
            {
                if (this.sock.Connected)
                {
                    this.state = SessionState.Connected;
                    this.ticks = time;
                    this.last_lag = time;
                    this.Panel.ServerText(StringTemplate.Get(STType.Messages, 24) + "...");
                    this.sock.Clear();
                    this.sock.Send(TCPOutbound.Login());
                }
                else if (time >= (this.ticks + 10))
                {
                    this.ticks = time;
                    this.state = SessionState.Sleeping;
                    this.sock.Disconnect();
                    this.reconnect_count++;
                    this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 23));
                    ScriptEvents.OnDisconnected(this);
                }
            }
            else if (this.sock != null)
            {
                if (time >= (this.last_lag + 30))
                {
                    this.last_lag = time;

                    if (Settings.GetReg<bool>("lag_check", true))
                        if (!String.IsNullOrEmpty(this.MyName))
                            this.sock.SendPriority(TCPOutbound.Lag(this.MyName, Helpers.UnixTimeMS, this.crypto));
                }

                if (time >= (this.ticks + 90))
                {
                    this.ticks = time;
                    this.sock.SendPriority(TCPOutbound.Update(this.crypto));

                    if (Settings.IsAway)
                        this.sock.SendTrickle(TCPOutbound.OnlineStatus(true, this.crypto));
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
                        this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 22) + " (" + this.sock.SockCode + ")");
                    else
                        this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 22) + " (remote disconnect)");
                }
            }
        }

        private void Reconnect()
        {
            this.ticks = (Settings.Time - 19);
            this.state = SessionState.Sleeping;
            this.sock.Disconnect();
            this.reconnect_count++;
            this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 21) + "...");
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
                this.Panel.SendBox.SelectionStart = this.Panel.SendBox.Text.Length;
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
                                Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                                {
                                    EndPoint = this.EndPoint,
                                    Text = text.Substring(4),
                                    Type = Scripting.JSOutboundTextItemType.Emote
                                });
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
                                this.sock.SendPriority(TCPOutbound.ManualLag(this.MyName, Helpers.UnixTimeMS, this.crypto));
                            else if (text == "/client")
                                this.sock.Send(TCPOutbound.Public(InternalCommands.CMD_CLIENT, this.crypto));
                            else if (text == "/np")
                            {
                                if (!String.IsNullOrEmpty(Helpers.NP))
                                    this.sock.Send(TCPOutbound.Emote("np: " + Helpers.NP, this.crypto));
                            }
                            else if (text == "/cmds")
                            {
                                foreach (String item in Helpers.cmds)
                                    this.Panel.AnnounceText(item);
                            }
                            else if (text.StartsWith("/all "))
                            {
                                text = text.Substring(5);

                                if (text.StartsWith("/me ") && text.Length > 4)
                                    RoomPool.Rooms.ForEach(x => x.SendEmote(text.Substring(4)));
                                else if (text.StartsWith("/") && text.Length > 1)
                                    RoomPool.Rooms.ForEach(x => x.SendCommand(text.Substring(1)));
                                else
                                    RoomPool.Rooms.ForEach(x => x.SendText(text));
                            }
                            else if (text.StartsWith("/find "))
                            {
                                String arg = text.Substring(6);
                                User u = this.users.Find(x => x.Name == arg);

                                if (u == null)
                                    u = this.users.Find(x => x.Name.StartsWith(arg));

                                if (u != null)
                                    this.Panel.Userlist.SetToUser(u.Name);
                            }
                            else if (text == "/pretext" || text == "/pretext ")
                            {
                                Settings.SetReg("pretext", String.Empty);
                                this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 20));
                            }
                            else if (text.StartsWith("/pretext "))
                            {
                                String arg = text.Substring(9);
                                Settings.SetReg("pretext", arg);
                                this.Panel.AnnounceText(StringTemplate.Get(STType.Messages, 19));
                            }
                            else if (text.Length > 1)
                            {
                                Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                                {
                                    EndPoint = this.EndPoint,
                                    Text = text.Substring(1),
                                    Type = Scripting.JSOutboundTextItemType.Command
                                });
                            }
                        }
                        else
                        {
                            if (e.Shift)
                            {
                                String whisper_name = this.Panel.Userlist.GetSelectedName;

                                if (!String.IsNullOrEmpty(whisper_name))
                                    Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                                    {
                                        EndPoint = this.EndPoint,
                                        Text = "whisper \"" + whisper_name + "\" " + text,
                                        Type = Scripting.JSOutboundTextItemType.Command
                                    });
                            }
                            else Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                            {
                                EndPoint = this.EndPoint,
                                Text = Settings.GetReg<String>("pretext", String.Empty) + text,
                                Type = Scripting.JSOutboundTextItemType.Public
                            });
                        }
                    }
                    else if (this.Panel.Mode == ScreenMode.PM)
                        Scripting.ScriptManager.PendingUIText.Enqueue(new Scripting.JSOutboundTextItem
                        {
                            EndPoint = this.EndPoint,
                            Name = this.Panel.PMName,
                            Text = text,
                            Type = Scripting.JSOutboundTextItemType.Private
                        });
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        public void SendPM(String name, String text)
        {
            if (this.Panel.Mode == ScreenMode.PM)
                if (name == this.Panel.PMName)
                {
                    User u = this.users.Find(x => x.Name == this.Panel.PMName);

                    if (u != null)
                    {
                        AresFont f = null;

                        if (Settings.MyFont != null)
                            f = Settings.MyFont.Copy();

                        this.Panel.MyPMText(text, f); // my font

                        if (!u.SupportsPMEnc)
                            this.sock.Send(TCPOutbound.Private(this.Panel.PMName, text, this.crypto));
                        else
                            this.sock.Send(TCPOutbound.CustomPM(this.Panel.PMName, text, this.crypto));
                    }
                    else this.Panel.MyPMAnnounce(StringTemplate.Get(STType.Messages, 9));
                }
        }

        public void SendJSPM(String name, String text)
        {
            User u = this.users.Find(x => x.Name == name);

            if (u != null)
            {
                AresFont f = null;

                if (Settings.MyFont != null)
                    f = Settings.MyFont.Copy();

                this.Panel.MyPMJSText(name, text, f); // my font

                if (!u.SupportsPMEnc)
                    this.sock.Send(TCPOutbound.Private(this.Panel.PMName, text, this.crypto));
                else
                    this.sock.Send(TCPOutbound.CustomPM(this.Panel.PMName, text, this.crypto));
            }
        }

        private void SendAutoReply(object sender, EventArgs e)
        {
            String pmname = (String)sender;

            if (this.state == SessionState.Connected)
            {
                String[] lines = Settings.GetReg<String>("pm_reply", "Hello +n, please leave a message.").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (String str in lines)
                {
                    String text = Helpers.FormatAresColorCodes(str.Replace("+n", pmname));

                    if (!String.IsNullOrEmpty(text))
                    {
                        while (Encoding.UTF8.GetByteCount(text) > 200)
                            text = text.Substring(0, text.Length - 1);

                        this.sock.Send(TCPOutbound.Private(pmname, text, this.crypto));
                    }
                }
            }
        }

        private void OpenPMRequested(object sender, EventArgs e)
        {
            String pmname = (String)sender;
            this.Panel.MyPMCreateOrShowTab(pmname);
        }
    }
}
