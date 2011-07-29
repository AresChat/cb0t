using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace cb0t_chat_client_v2
{
    public partial class Chat : UserControl
    {
        private Form2[] dc_sessions = new Form2[50];
        private TcpListener dc_listener;
        private Point custom_tab_close_button = new Point(0, 0);
        private bool dc_is_listening = false;

        private delegate void CrossThreadDCWindowDelegate(Form2 window);

        public bool session_loaded = false;

        internal event ChatContainerTabPage.NotifyDelegate OnRaiseBalloon;
        public event EventHandler OnTick;
        private ChannelBarRenderer cbr;

        private VoicePlayer vplayer = new VoicePlayer();
        private Winamp winamp = new Winamp();

        public event EventHandler OnVoiceClipStarted;
        public event EventHandler OnVoiceClipStopped;

        public Chat()
        {
            InitializeComponent();
            
            this.cbr = new ChannelBarRenderer();
            this.vplayer.OnPlayingNow += new VoicePlayer.SPNHandler(this.OnPlayingVCNow);
            this.vplayer.VoiceClipPlaying += new EventHandler(this.VoiceClipPlaying);
            this.vplayer.VoiceClipStopped += new EventHandler(this.VoiceClipStopped);
            this.toolStrip1.Renderer = this.cbr;
            this.toolStripButton1.Image = AresImages.ChannelList_Refresh;
            this.toolStripButton2.Image = AresImages.ChannelList_Search;
            this.toolStripButton3.Image = AresImages.ChannelList;
            this.toolStripButton3.Tag = -1;
            this.toolStrip2.Renderer = new ChannelListToolStripAppearance();
            this.comboBox1.SelectedIndex = 0;
            this.channelList1.OnChannelClicked += new ChannelList.ChannelClickedDelegate(this.OnChannelClicked);
            this.channelList1.OnChannelTotalUpdated += new ChannelList.ChannelTotalUpdateDelegate(this.OnChannelTotalUpdated);
            this.channelList1.OnChannelAddToFavourites += new ChannelList.ChannelClickedDelegate(this.OnChannelAddToFavourites);
            this.favouritesList1.OnChannelClicked += new ChannelList.ChannelClickedDelegate(this.OnChannelClicked);
            this.favouritesList1.Populate();
            this.channelList1.PopulateWithLastKnownList();
            this.LostFocus += new EventHandler(this.OnEnter);
            this.winamp.WinampSongChanged += new EventHandler(this.WinampSongChanged);
        }

        private void WinampSongChanged(object sender, EventArgs e)
        {
            if (AudioSettings.show_in_userlist)
                if (AudioSettings.winamp)
                    if (!String.IsNullOrEmpty(Winamp.current_song))
                        foreach (ChatContainerTabPage _room in this.open_rooms)
                            if (_room != null)
                                _room.AddGlobalPacket(Packets.UserlistSongPacket(Winamp.current_song));
        }

        public void InternalSongChanged()
        {
            if (AudioSettings.show_in_userlist)
                if (!AudioSettings.winamp)
                    if (!String.IsNullOrEmpty(AudioSettings.current_song))
                        foreach (ChatContainerTabPage _room in this.open_rooms)
                            if (_room != null)
                                _room.AddGlobalPacket(Packets.UserlistSongPacket(AudioSettings.current_song));
        }

        private void VoiceClipStopped(object sender, EventArgs e)
        {
            this.OnVoiceClipStopped(this, new EventArgs());   
        }

        private void VoiceClipPlaying(object sender, EventArgs e)
        {
            this.OnVoiceClipStarted(this, new EventArgs());
        }

        internal event OutputTextBox.RadioHashlinkClickedHandler RadioHashlink;

        private void RadioHashlinkClicked(String url)
        {
            this.RadioHashlink(url);
        }

        public void SendCustomEmoticonPacket(byte[] packet)
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    _room.SendCustomEmoticonPacket(packet);
        }

        private bool vc_hotkey_is_down = false;

        public void VCHotKey(bool down)
        {
            if (down && this.vc_hotkey_is_down)
                return;

            this.vc_hotkey_is_down = down;

            foreach (ChatContainerTabPage t in this.open_rooms)
            {
                if (t != null)
                {
                    if (t.tab_selected)
                    {
                        t.HotKeyClick(down);
                        break;
                    }
                }
            }
        }

        public void VCRecordCancel()
        {
            foreach (ChatContainerTabPage t in this.open_rooms)
            {
                if (t != null)
                {
                    if (t.tab_selected)
                    {
                        t.VCEscapeCancel();
                        break;
                    }
                }
            }
        }

        private void PauseVCPlayback(bool pause)
        {
            this.vplayer.Pause(pause);
        }

        private void OnPlayingVCNow(VoiceClipReceived vcr)
        {
            foreach (ChatContainerTabPage t in this.open_rooms)
                if (t != null)
                    t.DisplayVCNow(vcr);
        }

        private delegate void CRHandler1(VoiceClipReceived vcr, bool queue_if_busy);
        private void OnVoiceClipPlayRequest(VoiceClipReceived vcr, bool queue_if_busy)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new CRHandler1(this.OnVoiceClipPlayRequest), vcr, queue_if_busy);
            else
                this.vplayer.Play(vcr, queue_if_busy, this.Handle);
        }

        private void OnEnter(object sender, EventArgs e)
        {
            foreach (ChatContainerTabPage t in this.open_rooms)
            {
                if (t != null)
                {
                    if (t.tab_ident == this.cbr.selected_tab)
                    {
                        t.MakeSelected(true);
                        return;
                    }
                }
            }
        }

        private ChatContainerTabPage[] open_rooms = new ChatContainerTabPage[100];
        private Thread socket_thread;
        
        public bool terminate = false;

        public void Init()
        {
            this.socket_thread = new Thread(new ThreadStart(this.SocketThread));
            this.socket_thread.Start();
        }

        public void SendAvatarPacket(byte[] packet)
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    _room.AddGlobalAvatarPacket(packet);
        }

        public void GUITick()
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    _room.GUITick();

            this.vplayer.Tick(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 953)
            {
                if (m.WParam != (IntPtr)4)
                    this.vplayer.PlaybackComplete(this.Handle);
            }
            else base.WndProc(ref m);
        }

        public void SendGlobalPacket(byte[] packet)
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    _room.AddGlobalPacket(packet);
        }

        public void SendVCSupportPacket(byte[] packet)
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    if (_room.supports_voice_chat)
                        _room.AddGlobalPacket(packet);
        }

        public void ViewSpecificChat(int tab_ident)
        {
            this.cbr.selected_tab = tab_ident;
            this.toolStrip1.Invalidate();

            if (tab_ident == -1) // channel list
            {
                foreach (ChatContainerTabPage c in this.open_rooms)
                    if (c != null)
                        if (c.Visible)
                            c.Visible = false;

                this.panel1.Visible = true;
                this.SetSelectedChatroom(-1);
                this.favouritesList1.Invalidate();
                return;
            }
            else
            {
                this.SetSelectedChatroom(tab_ident);
            }

            this.panel1.Visible = false;

            foreach (ChatContainerTabPage c in this.open_rooms)
                if (c != null)
                    if (c.tab_ident == tab_ident)
                        c.Visible = true;
                    else
                        c.Visible = false;
            
            this.SetSelectedChatroom(tab_ident);
        }

        public void RedrawTopic()
        {
            foreach (ChatContainerTabPage c in this.open_rooms)
                if (c != null)
                    if (c.Visible)
                        c.RedrawTopic();
        }

        internal void OnChannelClicked(ChannelObject cObj)
        {
            int i = this.NextFreeChatRoomTab();

            if (i > -1)
            {
                int already_open = -1;

                for (int x = 0; x < this.open_rooms.Length; x++)
                {
                    ChatContainerTabPage c = this.open_rooms[x];

                    if (c != null)
                    {
                        if (c.SameAs(cObj))
                        {
                            this.SetSelectedChatroom(c.tab_ident);
                            already_open = c.tab_ident;
                            break;
                        }
                    }
                }
                
                if (already_open == -1)
                {
                    int time = (int)Math.Round((double)Environment.TickCount / 1000);
                    this.open_rooms[i] = new ChatContainerTabPage(cObj, i, time);
                    this.open_rooms[i].OnTopicUpdated += new ChatContainerTabPage.TopicUpdatedDelegate(this.OnTopicUpdated);
                    this.open_rooms[i].OnRedirected += new ChatContainerTabPage.NotifyDelegate(this.OnRedirected);
                    this.open_rooms[i].OnHashlinkClicked += new ChatContainerTabPage.TopicUpdatedDelegate(this.OnHashlinkClicked);
                    this.open_rooms[i].OnSendToAllRooms += new Packets.SendPacketDelegate(this.OnSendToAllRooms);
                    this.open_rooms[i].OnStartDCSession += new ChatContainerTabPage.DCRequestDelegate(this.OnStartDCSession);
                    this.open_rooms[i].OnTriggerWordReceived += new ChatContainerTabPage.NotifyDelegate(this.OnTriggerWordReceived);
                    this.open_rooms[i].MakeIconUnread += new ChatContainerTabPage.MakeIconUnreadDelegate(this.MakeIconUnread);
                    this.open_rooms[i].OnVoiceClipPlayRequest += new ChatContainerTabPage.VoicePlayerInboundHandler(this.OnVoiceClipPlayRequest);
                    this.open_rooms[i].OnVCPause += new ChatContainerTabPage.PauseVCHandler(this.PauseVCPlayback);
                    this.open_rooms[i].RadioHashlink += new OutputTextBox.RadioHashlinkClickedHandler(this.RadioHashlinkClicked);
                    this.open_rooms[i].CloseTabsCmd += new EventHandler(this.CloseTabsCmd);
                    this.Controls.Add(this.open_rooms[i]);
                    this.CreateButton(i, cObj.name);
                    this.ViewSpecificChat(i);
                    this.open_rooms[i].Location = new Point(4, this.toolStrip1.Location.Y + this.toolStrip1.Height + 1);
                    this.open_rooms[i].Size = new Size(this.Width - 7, this.Height - 28 - (((this.toolStrip1.Height / 25) - 1) * 25));

                    if (Settings.auto_add_rooms_to_favourites)
                        this.favouritesList1.AddRoom(cObj, true);
                }
                else
                {
                    this.ViewSpecificChat(already_open);
                }
            }
            else
            {
                MessageBox.Show("exceeded 100 rooms");
            }
        }

        private void CloseTabsCmd(object sender, EventArgs e)
        {
            foreach (ChatContainerTabPage _room in this.open_rooms)
                if (_room != null)
                    _room.CloseTabsNow();
        }

        private void OnRedirected(String text, int tab_ident)
        {
            for (int i = 0; i < this.toolStrip1.Items.Count; i++)
            {
                if (((int)this.toolStrip1.Items[i].Tag) == tab_ident)
                {
                    ((ToolStripButton)this.toolStrip1.Items[i]).Text = text;
                    return;
                }
            }
        }

        private void MakeIconUnread(int tab_ident)
        {
            for (int i = 0; i < this.toolStrip1.Items.Count; i++)
            {
                if (((int)this.toolStrip1.Items[i].Tag) == tab_ident)
                {
                    ((ToolStripButton)this.toolStrip1.Items[i]).Image = AresImages.Chat_Unread;
                    return;
                }
            }
        }

        private void OnTriggerWordReceived(String text, int tab_ident)
        {
            this.OnRaiseBalloon(text, tab_ident);
        }

        private void OnStartDCSession(UserObject userobj)
        {
            int _next_dc = this.NextFreeDCSession();

            if (_next_dc > -1)
            {
                this.dc_sessions[_next_dc] = new Form2();
                this.dc_sessions[_next_dc].MakeSessionEnd += new Form2.DCExternalDelegate(this.OnDCSessionExpired);
                this.dc_sessions[_next_dc].Show();
                this.dc_sessions[_next_dc].server_mode = false;
                this.dc_sessions[_next_dc].name = userobj.name;
                this.dc_sessions[_next_dc].ip = userobj.externalIp;
                this.dc_sessions[_next_dc].port = userobj.dcPort;
                this.dc_sessions[_next_dc].MakeClientConnect();
                this.dc_sessions[_next_dc].ident = _next_dc;
                this.dc_sessions[_next_dc].session_up = true;
                this.dc_sessions[_next_dc].OnHashlinkClicking += new ChannelList.ChannelClickedDelegate(this.OnChannelClicked);
            }
        }

        private void OnSendToAllRooms(byte[] packet)
        {
            foreach (ChatContainerTabPage t in this.open_rooms)
                if (t != null)
                    t.AddGlobalPacket(packet);
        }

        private void OnHashlinkClicked(ChannelObject cObj)
        {
            this.OnChannelClicked(cObj);
        }

        private void OnTopicUpdated(ChannelObject cObj)
        {
            this.favouritesList1.UpdateTopic(cObj);
        }

        private void SetSelectedChatroom(int tab_ident)
        {
            for (int i = 0; i < this.open_rooms.Length; i++)
            {
                ChatContainerTabPage c = this.open_rooms[i];

                if (c != null)
                {
                    if (c.tab_ident == tab_ident)
                        c.MakeSelected(true);
                    else
                        c.MakeSelected(false);
                }
            }
        }

        private int NextFreeChatRoomTab()
        {
            for (int i = 0; i < this.open_rooms.Length; i++)
                if (this.open_rooms[i] == null)
                    return i;

            return -1;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) // channel list refresh
        {
            this.channelList1.RefreshList(this.toolStripTextBox1.Text);
        }

        private void toolStripButton2_Click(object sender, EventArgs e) // channel list search
        {
            this.channelList1.SearchList(this.toolStripTextBox1.Text);
        }

        private void OnChannelTotalUpdated(String text)
        {
            if (this.toolStrip1.InvokeRequired)
            {
                this.toolStrip1.BeginInvoke(new ChannelList.ChannelTotalUpdateDelegate(this.OnChannelTotalUpdated), text);
            }
            else
            {
                this.toolStripButton3.Text = text;
            }
        }

        private void OnChannelAddToFavourites(ChannelObject cObj)
        {
            this.favouritesList1.AddRoom(cObj, true);
        }

        public void ResetDCListener()
        {
            if (this.dc_is_listening)
            {
                this.dc_is_listening = false;

                try
                {
                    this.dc_listener.Stop();
                }
                catch { }
            }

            this.dc_listener = new TcpListener(IPAddress.Any, Settings.dc_port);

            try
            {
                this.dc_listener.Start();
                this.dc_is_listening = true;
            }
            catch { }
        }

        private void SocketThread()
        {
            int cb_tick = (int)Math.Round((double)Environment.TickCount / 1000);
            uint unix_tick = Helpers.UnixTime();
          //  int gc_tick = (int)Math.Round((double)Environment.TickCount / 1000);
            this.dc_listener = new TcpListener(IPAddress.Any, Settings.dc_port);

            try
            {
                this.dc_listener.Start();
                this.dc_is_listening = true;
            }
            catch { }

            while (true)
            {
                if (this.terminate)
                {
                    this.channelList1.terminate = true;

                    foreach (ChatContainerTabPage c in this.open_rooms)
                        if (c != null)
                            c.CloseRoom();

                    return;
                }

                int time = (int)Math.Round((double)Environment.TickCount / 1000);
                uint unix = Helpers.UnixTime();

                if (unix_tick < unix)
                {
                    unix_tick = unix;

                    if (this.OnTick != null)
                        this.OnTick(null, new EventArgs());

                    this.winamp.Tick();
                }

                foreach (ChatContainerTabPage c in this.open_rooms)
                    if (c != null)
                        c.ServiceChatroom(time);

                if (this.dc_is_listening)
                {
                    if (this.dc_listener.Pending())
                    {
                        Socket _next_dc_socket = this.dc_listener.AcceptSocket();
                        IPAddress _next_dc_ip = ((IPEndPoint)_next_dc_socket.RemoteEndPoint).Address;
                        int _next_dc = this.NextFreeDCSession();

                        if (_next_dc == -1 || !Settings.dc_enabled || Settings.blocked_dcs.Contains(_next_dc_ip))
                        {
                            _next_dc_socket.Disconnect(false);
                        }
                        else
                        {
                            this.dc_sessions[_next_dc] = new Form2();
                            this.dc_sessions[_next_dc].MakeWindowPopUp += new Form2.DCExternalDelegate(this.OnDCPopup);
                            this.dc_sessions[_next_dc].MakeSessionEnd += new Form2.DCExternalDelegate(this.OnDCSessionExpired);
                            this.dc_sessions[_next_dc].server_mode = true;
                            this.dc_sessions[_next_dc].connected = true;
                            this.dc_sessions[_next_dc].socket = _next_dc_socket;
                            this.dc_sessions[_next_dc].socket.Blocking = false;
                            this.dc_sessions[_next_dc].ip = _next_dc_ip;
                            this.dc_sessions[_next_dc].port = 0;
                            this.dc_sessions[_next_dc].ident = _next_dc;
                            this.dc_sessions[_next_dc].MakeServerConnect();
                            this.dc_sessions[_next_dc].session_up = true;
                            this.dc_sessions[_next_dc].OnHashlinkClicking += new ChannelList.ChannelClickedDelegate(this.OnChannelClicked);
                        }
                    }
                }

                if (Settings.dc_enabled)
                    foreach (Form2 _dc in this.dc_sessions)
                        if (_dc != null)
                            if (_dc.session_up)
                                _dc.ServiceSocketConnection(time);

                Thread.Sleep(30);
            }
        }

        private void OnDCSessionExpired(int ident)
        {
            this.dc_sessions[ident] = null;
        }

        private void OnDCPopup(int ident)
        {
            this.BeginInvoke(new CrossThreadDCWindowDelegate(this.MakeDCWindowPopup), this.dc_sessions[ident]);
        }

        private void MakeDCWindowPopup(Form2 window)
        {
            window.Show();
            window.ResetOutputWindow();
        }

        private int NextFreeDCSession()
        {
            for (int i = 0; i < this.dc_sessions.Length; i++)
                if (this.dc_sessions[i] == null)
                    return i;

            return -1;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle r = e.ClipRectangle;

            g.DrawLine(new Pen(Brushes.Silver, 2), new Point(r.X, r.Y + r.Height - 1), new Point(r.X + r.Width, r.Y + r.Height - 1));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.my_status = (Settings.OnlineStatus)((byte)(this.comboBox1.SelectedIndex + 1));
            byte[] data = Packets.OnlineStatusPacket();

            foreach (ChatContainerTabPage c in this.open_rooms)
                if (c != null)
                    c.UpdateMyStatus(data);
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.channelList1.SearchList(this.toolStripTextBox1.Text);
        }

        private void CreateButton(int tab_ident, String text)
        {
            ToolStripButton b = new ToolStripButton();
            String[] _spl = text.Split(new String[] { " " }, StringSplitOptions.None);

            for (int i = 0; i < _spl.Length; i++)
                if (_spl[i].Length > 0)
                    _spl[i] = _spl[i].Substring(0, 1).ToUpper() + _spl[i].Substring(1).ToLower();

            b.Text = String.Join(" ", _spl);

            if (b.Text.Length > 20)
                b.Text = b.Text.Substring(0, 20) + "...";

            b.ToolTipText = text;
            b.Tag = tab_ident;
            b.Image = AresImages.Chat_Read;
            b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.AutoSize = false;
            b.Size = new Size(176, 22);
            b.ImageAlign = ContentAlignment.MiddleLeft;
            b.MouseDown += new MouseEventHandler(this.OnChatBarClick);
            b.MouseUp += new MouseEventHandler(this.OnCloseRoomClick);
            this.toolStrip1.Items.Add(b);
        }

        private void toolStrip1_Resize(object sender, EventArgs e)
        {
            this.panel1.Location = new Point(3, this.toolStrip1.Location.Y + this.toolStrip1.Height + 1);
            this.panel1.Size = new Size(this.Width - 6, this.Height - 38 - (((this.toolStrip1.Height / 25) - 1) * 25));

            Point _point = new Point(4, this.toolStrip1.Location.Y + this.toolStrip1.Height + 1);
            Size _size = new Size(this.Width - 7, this.Height - 28 - (((this.toolStrip1.Height / 25) - 1) * 25));

            foreach (ChatContainerTabPage c in this.open_rooms)
            {
                if (c != null)
                {
                    c.Location = _point;
                    c.Size = _size;
                }
            }

            this.favouritesList1.Invalidate();
            this.channelList1.Invalidate();
        }

        private void OnChatBarClick(object sender, MouseEventArgs e) // clicked view chatroom
        {
            this.ViewSpecificChat((int)((ToolStripButton)sender).Tag);
            ((ToolStripButton)sender).Image = AresImages.Chat_Read;
        }

        private void OnCloseRoomClick(object sender, MouseEventArgs e) // close room
        {
            ToolStripButton b = ((ToolStripButton)sender);
            int id = ((int)b.Tag);

            if (id == this.cbr.selected_tab) // correct button
            {
                Point m = e.Location;
                
                if (m.X >= (b.Width - 20) && m.X < (b.Width - 4))
                {
                    if (m.Y >= 3 && m.Y <= 19)
                    {
                        for (int i = 1; i < this.toolStrip1.Items.Count; i++)
                        {
                            if (((int)this.toolStrip1.Items[i].Tag) == id)
                            {
                                this.toolStrip1.Items.RemoveAt(i);
                                break;
                            }
                        }

                        for (int i = 2; i < this.Controls.Count; i++)
                        {
                            int get_id = (int)(((ChatContainerTabPage)this.Controls[i]).tab_ident);

                            if (get_id == id)
                            {
                                this.Controls.RemoveAt(i);
                                break;
                            }
                        }

                        for (int i = 0; i < this.open_rooms.Length; i++)
                        {
                            if (this.open_rooms[i] != null)
                            {
                                if (this.open_rooms[i].tab_ident == id)
                                {
                                    this.open_rooms[i].CloseRoom();
                                    this.open_rooms[i].FreeResources();
                                    this.open_rooms[i] = null;
                                    id = (int)(((ToolStripButton)this.toolStrip1.Items[toolStrip1.Items.Count - 1]).Tag);
                                    this.ViewSpecificChat(id);

                                    if (id > -1)
                                        ((ToolStripButton)this.toolStrip1.Items[toolStrip1.Items.Count - 1]).Image = AresImages.Chat_Read;

                                    GC.Collect();
                                    return;
                                }
                            }
                        }

                        this.ViewSpecificChat(-1);
                    }
                }
            }
        }

        private void toolStripButton3_MouseDown(object sender, MouseEventArgs e) // clicked view channel list
        {
            this.ViewSpecificChat(-1);
        }

    }
}
