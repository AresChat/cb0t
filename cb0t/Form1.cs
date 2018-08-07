using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Diagnostics;
using FormEx;
using FormEx.JumpListEx;
using FormEx.PreviewToolStripEx;
using FormEx.TaskBarProgressBarEx;

namespace cb0t
{
    public partial class Form1 : DwmForm
    {
        private ChannelBar channel_bar { get; set; }
        private AudioControlBar audio_bar { get; set; }
        
        public static SettingsPanel SettingsContent { get; set; }

        private AudioPanel audio_content { get; set; }
        private ChannelListPanel clist_content { get; set; }
        private Thread sock_thread { get; set; }
        private Bitmap img_play { get; set; }
        private Bitmap img_pause { get; set; }
        private VolumeControl volume { get; set; }

        private MediaIPC.WMP.WMPListener wmp { get; set; }
        private MediaIPC.Winamp.WinampListener winamp { get; set; }
        private MediaIPC.VLC.VLCListener vlc { get; set; }
        private MediaIPC.Foobar2000.FoobarListener foobar { get; set; }
        private MediaIPC.iTunes.iTunesListener itunes { get; set; }
        private MediaIPC.Spotify.Spotify spotify { get; set; }

        private ulong last_trickle = 0;
        private String inithash = null;

        public Form1(String init_hashlink)
        {
            Settings.Create();
            Helpers.LoadScreenHTML();
            Friends.Load();
            Emoticons.Load();
            Avatar.Load();
            Filter.Load();
            Menus.Load();
            AutoIgnores.Load();
            Narrator.Init();
            VoicePlayer.Init();
            StringTemplate.Load();

            if (this.JumpList != null)
            {
                this.JumpList.ImageResource = Settings.AppPath + "listres.dll";
                this.JumpList.Add(new JumpListItem("show as away", "cbjl_online"), 6);
                this.JumpList.Add(new JumpListSeparator());
                this.JumpList.Add(new JumpListItem("next", "cbjl_next"), 0);
                this.JumpList.Add(new JumpListItem("play/pause", "cbjl_playpause"), 1);
                this.JumpList.Add(new JumpListItem("previous", "cbjl_previous"), 3);
                this.JumpList.Add(new JumpListItem("stop", "cbjl_stop"), 4);
                this.JumpList.Add(new JumpListSeparator());
                this.JumpList.Add(new JumpListItem("exit", "cbjl_exit"), 7);
            }

            if (this.OverlayIcon != null)
                this.OverlayIcon.Image = (Bitmap)Properties.Resources.awayoverlay.Clone();

            if (this.PreviewToolStrip != null)
            {
                this.PreviewToolStrip.ItemClicked += this.PreviewToolStripItemClicked;
                PreviewToolStripItem item1 = new PreviewToolStripItem((Bitmap)Properties.Resources.online.Clone(), "show as online");
                item1.Tag = 1;
                PreviewToolStripItem item2 = new PreviewToolStripItem((Bitmap)Properties.Resources.away.Clone(), "show as away");
                item2.Tag = 2;
                this.PreviewToolStrip.CreateItems(item1, item2);
                this.PreviewToolStrip[0].Button.Enabled = false;
            }

            this.InitializeComponent();
            this.inithash = init_hashlink;
            this.img_play = (Bitmap)Properties.Resources.audio_play.Clone();
            this.img_pause = (Bitmap)Properties.Resources.audio_pause.Clone();
            this.toolStrip1.Items.Add(new SettingsButton());
            this.toolStrip1.Items.Add(new ToolStripSeparator());
            this.toolStrip1.Items.Add(new AudioButton());
            this.toolStrip1.Items.Add(new ToolStripSeparator());
            this.toolStrip1.Items.Add(new ChannelListButton());
            this.channel_bar = new ChannelBar();
            this.toolStrip1.Renderer = this.channel_bar;
            this.audio_bar = new AudioControlBar();
            this.toolStrip2.Renderer = this.audio_bar;

            SettingsContent = new SettingsPanel();
            SettingsContent.BackColor = Color.White;
            SettingsContent.Dock = DockStyle.Fill;
            SettingsContent.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            SettingsContent.JoinFromHashlinkClicked += this.JoinFromHashlinkClicked;
            this.audio_content = new AudioPanel();
            this.audio_content.BackColor = Color.White;
            this.audio_content.Dock = DockStyle.Fill;
            this.audio_content.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.clist_content = new ChannelListPanel();
            this.clist_content.BackColor = Color.WhiteSmoke;
            this.clist_content.Dock = DockStyle.Fill;
            this.clist_content.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.SetToList();

            Aero.HideIconAndText(this, true, true);
        }

        private delegate void SetProgressLevelHandler(int amount);
        public void SetProgressLevel(int amount)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new SetProgressLevelHandler(this.SetProgressLevel), amount);
            else if (this.TaskBarProgress != null)
            {
                if (amount == 0)
                    this.TaskBarProgress.Reset();
                else
                {
                    this.TaskBarProgress.Color = TaskBarProgressColor.Red;
                    this.TaskBarProgress.SetValue(amount <= 15 ? amount : 15, 15);
                }
            }
        }

        private void PreviewToolStripItemClicked(object sender, PreviewToolStripItemClickedEventArgs e)
        {
            if (((int)e.Item.Tag) == 1)
                this.showAsOnlineToolStripMenuItem_Click(null, EventArgs.Empty);
            else
                this.showAsAwayToolStripMenuItem_Click(null, EventArgs.Empty);
        }

        public void AddToFavourite(FavouritesListItem f)
        {
            this.clist_content.AddToFavourites(f);
        }

        public void JoinFromHashlinkClicked(object sender, EventArgs e)
        {
            DecryptedHashlink hashlink = (DecryptedHashlink)sender;
            OpenChannelEventArgs args = new OpenChannelEventArgs();
            FavouritesListItem item = new FavouritesListItem();
            item.IP = hashlink.IP;
            item.Name = hashlink.Name;
            item.Port = hashlink.Port;
            item.Topic = hashlink.Name;
            item.Password = String.Empty;
            item.AutoJoin = false;
            args.Room = item;
            this.OpenChannel(null, args);
        }

        private IPEndPoint popup_ep = null;
        private bool can_show_popup = true;
        private Scripting.JSUIPopupCallback popup_cb = null;

        public void ShowPopup(String title, String msg, IPEndPoint room, PopupSound sound)
        {
            if (Settings.GetReg<bool>("block_popups", false))
                return;

            if (!this.can_show_popup)
                return;

            this.popup_cb = null;
            this.can_show_popup = false;
            this.popup_ep = room;
            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipTitle = title;
            this.notifyIcon1.BalloonTipText = msg;
            this.notifyIcon1.ShowBalloonTip(10000);

            if (sound == PopupSound.Notify)
                InternalSounds.Notify();
            else if (sound == PopupSound.Friend)
                InternalSounds.Friend();
        }

        public void ShowPopup(String title, String msg, Scripting.JSUIPopupCallback pcb)
        {
            if (Settings.GetReg<bool>("block_popups", false))
                return;

            if (!this.can_show_popup)
                return;

            this.popup_cb = pcb;
            this.can_show_popup = false;
            this.popup_ep = pcb.Room;
            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipTitle = title;
            this.notifyIcon1.BalloonTipText = msg;
            this.notifyIcon1.ShowBalloonTip(10000);
            InternalSounds.Notify();
        }

        public byte[] GetScribble()
        {
            int pos_x = this.Location.X + (this.Width / 2) - (SharedUI.ScribbleEditor.Width / 2);
            int pos_y = this.Location.Y + (this.Height / 2) - (SharedUI.ScribbleEditor.Height / 2);

            if (pos_x < 0)
                pos_x = 0;

            if (pos_y < 0)
                pos_y = 0;

            SharedUI.ScribbleEditor.StartPosition = FormStartPosition.Manual;
            SharedUI.ScribbleEditor.Location = new Point(pos_x, pos_y);
            SharedUI.ScribbleEditor.ResetCanvas();
            byte[] scr = SharedUI.ScribbleEditor.GetScribble();
            SharedUI.ScribbleEditor.ResetCanvas();
            return scr;
        }

        private void PopupClicked(object sender, EventArgs e)
        {
            if (this.popup_ep != null)
            {
                this.channel_bar.SelectedButton = this.popup_ep;
                this.channel_bar.Mode = ChannelBar.ModeOption.Channel;
                this.toolStrip1.Invalidate();
                this.SetToRoom(this.popup_ep);
                this.popup_ep = null;

                this.BeginInvoke((Action)(() =>
                {
                    if (!this.Visible)
                        this.Show();

                    if (this.WindowState == FormWindowState.Minimized)
                        this.WindowState = FormWindowState.Normal;

                    this.Activate();
                }));

                if (this.popup_cb != null)
                {
                    Scripting.JSUIPopupCallback pcb = this.popup_cb;
                    this.popup_cb = null;
                    Scripting.ScriptManager.PendingPopupCallbacks.Enqueue(pcb);
                }
            }
        }

        public void Nudge()
        {
            this.BeginInvoke((Action)(() =>
            {
                InternalSounds.Nudge();

                if (this.WindowState == FormWindowState.Normal)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Random rnd = new Random();
                    Point start = this.Location;
                    Point current;

                    while (true)
                    {
                        if (sw.ElapsedMilliseconds > 1000)
                            break;

                        int x = rnd.Next(-3, 3);
                        int y = rnd.Next(-3, 3);
                        current = new Point(start.X + x, start.Y + y);
                        this.Location = current;
                    }

                    sw.Stop();
                    sw = null;
                    rnd = null;
                }
            }));
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is SettingsButton)
            {
                this.channel_bar.Mode = ChannelBar.ModeOption.Settings;
                this.SetToSettings();
            }
            else if (e.ClickedItem is AudioButton && AudioPanel.Available)
            {
                this.channel_bar.Mode = ChannelBar.ModeOption.Audio;
                this.SetToAudio();
            }
            else if (e.ClickedItem is ChannelListButton)
            {
                this.channel_bar.Mode = ChannelBar.ModeOption.ChannelList;
                this.SetToList();
            }
            else if (e.ClickedItem is ChannelButton)
            {
                this.channel_bar.Mode = ChannelBar.ModeOption.Channel;
                this.channel_bar.SelectedButton = ((ChannelButton)e.ClickedItem).EndPoint;
                this.SetToRoom(((ChannelButton)e.ClickedItem).EndPoint);
            }
            else return;

            this.toolStrip1.Invalidate();
        }

        private void SetToList()
        {
            RoomPool.Rooms.ForEach(x => x.RoomIsVisible = false);

            while (this.content1.Controls.Count > 0)
                this.content1.Controls.RemoveAt(0);

            this.content1.Controls.Add(this.clist_content);
            Narrator.ClearList();
        }

        private void SetToSettings()
        {
            RoomPool.Rooms.ForEach(x => x.RoomIsVisible = false);

            while (this.content1.Controls.Count > 0)
                this.content1.Controls.RemoveAt(0);

            this.content1.Controls.Add(SettingsContent);
            Narrator.ClearList();
        }

        private void SetToAudio()
        {
            RoomPool.Rooms.ForEach(x => x.RoomIsVisible = false);

            while (this.content1.Controls.Count > 0)
                this.content1.Controls.RemoveAt(0);

            this.content1.Controls.Add(this.audio_content);
            Narrator.ClearList();
        }

        private void SetToRoom(IPEndPoint ep)
        {
            while (this.content1.Controls.Count > 0)
                this.content1.Controls.RemoveAt(0);

            foreach (Room room in RoomPool.Rooms)
                if (room.EndPoint.Equals(ep))
                {
                    this.content1.Controls.Add(room.Panel);
                    room.Button.MakeRead();
                    room.ScrollAndFocus();
                    room.RoomIsVisible = true;
                }
                else room.RoomIsVisible = false;

            Narrator.ClearList();
        }

        private void toolStrip1_Resize(object sender, EventArgs e)
        {
            Aero.ExtendAero(this, this.toolStrip1.Height, this.toolStrip2.Height);
        }

        private bool do_once = false;
        private bool terminate = false;
        private IPCListener ipc { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.do_once)
            {
                if (this.SystemMenu != null)
                    this.SystemMenu.ItemClicked += this.SystemMenuItemClicked;

                int frm_size_x = Settings.GetReg<int>("form_x", -1);
                int frm_size_y = Settings.GetReg<int>("form_y", -1);

                if (frm_size_x > 0 && frm_size_y > 0)
                    this.ClientSize = new Size(frm_size_x, frm_size_y);

                this.do_once = true;
                Aero.ExtendAero(this, this.toolStrip1.Height, this.toolStrip2.Height);
                SharedUI.Init();
                Settings.CAN_WRITE_REG = false;
                this.clist_content.LabelChanged += this.ChannelListLabelChanged;
                this.clist_content.OpenChannel += this.OpenChannel;
                this.clist_content.Create();
                this.notifyIcon1.MouseClick += this.SysTrayMouseClicked;
                this.notifyIcon1.BalloonTipClicked += this.PopupClicked;
                SettingsContent.SpellCheckUpdate2 += this.SpellCheckUpdate2;
                SettingsContent.OnTemplateChanged += this.OnTemplateChanged;
                SettingsContent.BlockCustomNamesUpdate2 += this.BlockCustomNamesUpdate;
                SettingsContent.CreateSettings();
                this.audio_content.LoadPlaylist();

                Settings.CAN_WRITE_REG = true;

                if (Settings.IsAway)
                {
                    this.showAsAwayToolStripMenuItem.Enabled = false;
                    this.showAsOnlineToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.showAsAwayToolStripMenuItem.Enabled = true;
                    this.showAsOnlineToolStripMenuItem.Enabled = false;
                }

                int vol = Settings.GetReg<int>("audio_volume", 70);
                this.audio_content.SetVolume(vol);
                this.audio_content.PlayPauseIconChanged += this.PlayPauseIconChanged;
                this.audio_content.ShowAudioText += this.ShowAudioText;
                this.toolStripButton5.Image = this.img_play;
                this.playpauseToolStripMenuItem.Image = this.img_play;
                this.toolStrip1.Items[2].Enabled = AudioPanel.Available;
                this.toolStrip2.Enabled = AudioPanel.Available;
                this.wmp = new MediaIPC.WMP.WMPListener(this, AudioPanel.Available);
                this.winamp = new MediaIPC.Winamp.WinampListener();
                this.vlc = new MediaIPC.VLC.VLCListener();
                this.foobar = new MediaIPC.Foobar2000.FoobarListener();
                this.itunes = new MediaIPC.iTunes.iTunesListener();
                this.spotify = new MediaIPC.Spotify.Spotify();

                this.volume = new VolumeControl();
                this.volume.VolumeChanged += this.VolumeChanged;
                this.volume.SetVolume(vol);

                this.UpdateTemplate();

                Scripting.ScriptManager.Init();

                foreach (FavouritesListItem f in this.clist_content.GetAutoJoinRooms())
                    this.OpenChannel(null, new OpenChannelEventArgs { Room = f });

                this.sock_thread = new Thread(new ThreadStart(this.SocketThread));
                this.sock_thread.Start();
                this.timer2.Enabled = true;
                this.clist_content.RefreshIfEmpty();

                this.ipc = new IPCListener();
                this.ipc.HashlinkReceived += this.IPCHashlinkReceived;
                this.ipc.CommandReceived += this.IPCCommandReceived;

                if (!String.IsNullOrEmpty(this.inithash))
                    if (this.inithash.StartsWith("cb0t://"))
                    {
                        this.inithash = this.inithash.Substring(7);

                        if (this.inithash.StartsWith("script/?file="))
                        {
                            String filename = this.inithash.Substring(13);

                            if (!String.IsNullOrEmpty(filename))
                                Scripting.ScriptManager.InstallScript(filename);
                        }
                        else
                        {
                            DecryptedHashlink dh = Hashlink.DecodeHashlink(this.inithash);

                            if (dh == null)
                                if (this.inithash.EndsWith("/"))
                                    this.inithash = this.inithash.Substring(0, this.inithash.Length - 1);

                            dh = Hashlink.DecodeHashlink(this.inithash);

                            if (dh != null)
                                this.IPCHashlinkReceived(null, new IPCListenerEventArgs { Hashlink = dh });
                        }
                    }

                this.ipc.Start();
            }
        }

        private void BlockCustomNamesUpdate(object sender, EventArgs e)
        {
            bool block = Settings.GetReg<bool>("block_custom_names", false);
            RoomPool.Rooms.ForEach(x => x.UpdateBlockCustomNamesStatus(block));
        }

        private void IPCHashlinkReceived(object sender, IPCListenerEventArgs e)
        {
            this.BeginInvoke((Action)(() =>
            {
                if (!this.Visible)
                    this.Show();

                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;

                this.Activate();

                FavouritesListItem fav = new FavouritesListItem();

                fav.IP = e.Hashlink.IP;
                fav.Name = e.Hashlink.Name;
                fav.Port = e.Hashlink.Port;
                fav.Topic = e.Hashlink.Name;

                this.OpenChannel(null, new OpenChannelEventArgs { Room = fav });
            }));
        }

        private void IPCCommandReceived(object sender, IPCListenerEventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new EventHandler<IPCListenerEventArgs>(this.IPCCommandReceived), sender, e);
            else
            {
                switch (e.Command)
                {
                    case "online":
                        if (!Settings.IsAway)
                            this.showAsAwayToolStripMenuItem_Click(null, EventArgs.Empty);
                        else
                            this.showAsOnlineToolStripMenuItem_Click(null, EventArgs.Empty);
                        break;

                    case "next":
                        this.audio_content.NextClicked();
                        break;

                    case "playpause":
                        this.audio_content.PlayPauseClicked();
                        break;

                    case "previous":
                        this.audio_content.PreviousClicked();
                        break;

                    case "stop":
                        this.audio_content.StopClicked();
                        break;

                    case "exit":
                        this.can_close = true;
                        this.Close();
                        break;
                }
            }
        }

        private void OnTemplateChanged(object sender, EventArgs e)
        {
            StringTemplate.Load();
            this.UpdateTemplate();
        }

        private IntPtr[] sysmenubmps = null;

        private void UpdateTemplate()
        {
            SettingsContent.UpdateTemplate();

            if (this.SystemMenu != null)
            {
                if (this.sysmenubmps == null)
                {
                    this.sysmenubmps = new IntPtr[4];
                    this.sysmenubmps[0] = FormEx.SystemMenuEx.SystemMenuContainer.ImageToHandle((Bitmap)Properties.Resources.away.Clone());
                    this.sysmenubmps[1] = FormEx.SystemMenuEx.SystemMenuContainer.ImageToHandle((Bitmap)Properties.Resources.online.Clone());
                    this.sysmenubmps[2] = FormEx.SystemMenuEx.SystemMenuContainer.ImageToHandle((Bitmap)Properties.Resources.audio.Clone());
                    this.sysmenubmps[3] = FormEx.SystemMenuEx.SystemMenuContainer.ImageToHandle((Bitmap)Properties.Resources.close.Clone());
                }

                this.SystemMenu.Clear();

                if (Settings.IsAway)
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.SystemTray, 0), this.sysmenubmps[1])); //0
                else
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.SystemTray, 1), this.sysmenubmps[0])); //0

                this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuSeperator()); //1

                if (AudioPanel.Available)
                {
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.AudioBar, 10), this.sysmenubmps[2])); //2
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.AudioBar, 9), this.sysmenubmps[2])); //3
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.AudioBar, 8), this.sysmenubmps[2])); //4
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.AudioBar, 7), this.sysmenubmps[2])); //5
                    this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuSeperator()); //6
                }

                this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.SystemTray, 2), this.sysmenubmps[3])); //2 or 7
                this.SystemMenu.Add(new FormEx.SystemMenuEx.SystemMenuSeperator()); //8
            }

            if (this.JumpList != null)
            {
                this.JumpList.Clear();

                if (Settings.IsAway)
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.SystemTray, 0), "cbjl_online"), 5);
                else
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.SystemTray, 1), "cbjl_online"), 6);

                this.JumpList.Add(new JumpListSeparator());

                if (AudioPanel.Available)
                {
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.AudioBar, 10), "cbjl_next"), 0);
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.AudioBar, 9), "cbjl_playpause"), this.audio_content.IsPlaying ? 2 : 1);
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.AudioBar, 8), "cbjl_previous"), 3);
                    this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.AudioBar, 7), "cbjl_stop"), 4);
                    this.JumpList.Add(new JumpListSeparator());
                }

                this.JumpList.Add(new JumpListItem(StringTemplate.Get(STType.SystemTray, 2), "cbjl_exit"), 7);
            }

            this.audioToolStripMenuItem.Enabled = AudioPanel.Available;

            if (this.PreviewToolStrip != null)
            {
                this.PreviewToolStrip[0].Button.Tooltip = StringTemplate.Get(STType.SystemTray, 0);
                this.PreviewToolStrip[1].Button.Tooltip = StringTemplate.Get(STType.SystemTray, 1);
            }

            this.toolStripDropDownButton1.ToolTipText = StringTemplate.Get(STType.AudioBar, 0);
            this.clearListToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 1);
            this.randomToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 2);
            this.repeatToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 3);
            this.importFilesToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 4);
            this.importFolderToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 5);
            this.toolStripButton1.ToolTipText = StringTemplate.Get(STType.AudioBar, 6);
            this.toolStripButton3.ToolTipText = StringTemplate.Get(STType.AudioBar, 7);
            this.toolStripButton4.ToolTipText = StringTemplate.Get(STType.AudioBar, 8);
            this.toolStripButton5.ToolTipText = StringTemplate.Get(STType.AudioBar, 9);
            this.toolStripButton6.ToolTipText = StringTemplate.Get(STType.AudioBar, 10);
            this.audioToolStripMenuItem.Text = StringTemplate.Get(STType.TopBar, 1);
            this.nextToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 10);
            this.playpauseToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 9);
            this.previousToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 8);
            this.stopToolStripMenuItem.Text = StringTemplate.Get(STType.AudioBar, 7);
            this.toolStrip1.Items[0].ToolTipText = StringTemplate.Get(STType.TopBar, 0);
            this.toolStrip1.Items[2].ToolTipText = StringTemplate.Get(STType.TopBar, 1);
            this.toolStrip1.Items[4].ToolTipText = StringTemplate.Get(STType.TopBar, 2);
            this.showAsOnlineToolStripMenuItem.Text = StringTemplate.Get(STType.SystemTray, 0);
            this.showAsAwayToolStripMenuItem.Text = StringTemplate.Get(STType.SystemTray, 1);
            this.exitToolStripMenuItem.Text = StringTemplate.Get(STType.SystemTray, 2);
            this.audio_content.UpdateTemplate();
            this.clist_content.UpdateTemplate();
            RoomPool.Rooms.ForEach(x => x.UpdateTemplate());
            SharedUI.UpdateTemplate();
        }

        private void VolumeChanged(object sender, VolumeControlValueChangedEventArgs e)
        {
            int vol = e.Volume;
            Settings.SetReg("audio_volume", vol);
            this.audio_content.SetVolume(vol);
        }

        private void SpellCheckUpdate2(object sender, EventArgs e)
        {
            bool enable = (bool)sender;
            this.timer1.Enabled = enable;
        }

        private void ShowAudioText(object sender, EventArgs e)
        {
            this.audio_bar.DisplayText = (String)sender;
            this.toolStrip2.Invalidate();
        }

        private void PlayPauseIconChanged(object sender, EventArgs e)
        {
            this.toolStripButton5.Image = ((bool)sender) ? this.img_play : this.img_pause;
            this.playpauseToolStripMenuItem.Image = ((bool)sender) ? this.img_play : this.img_pause;

            if (this.JumpList != null)
            {
                this.JumpList.RemoveAt(3);
                this.JumpList.Insert(3, new JumpListItem(StringTemplate.Get(STType.AudioBar, 9), "cbjl_playpause"), ((bool)sender) ? 1 : 2);
            }
        }

        private void SysTrayMouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.BeginInvoke((Action)(() =>
                {
                    if (!this.Visible)
                        this.Show();

                    if (this.WindowState == FormWindowState.Minimized)
                        this.WindowState = FormWindowState.Normal;

                    this.Activate();

                    if (Settings.IsAway)
                        if (this.OverlayIcon != null)
                            this.OverlayIcon.Show();
                }));
        }

        private String last_song = String.Empty;
        private uint last_song_check = 0;

        private void SocketThread()
        {
            this.last_song = String.Empty;
            this.last_song_check = Settings.Time;

            while (true)
            {
                if (this.terminate)
                    return;

                ulong time_long = Settings.TimeLong;
                Scripting.ScriptManager.EventCycle(time_long);
                Room[] pool = RoomPool.Rooms.ToArray();
                uint time = Settings.Time;
                bool check_song = time > this.last_song_check;
                bool do_timer = time > this.last_song_check;
                bool do_trickle = time_long >= (this.last_trickle + 500);

                if (do_trickle)
                    this.last_trickle = time_long;

                int player_type = -1;

                if (check_song)
                {
                    this.last_song_check = time;
                    int src = Settings.GetReg<int>("m_listener_index", 0);
                    player_type = src;
                    String c_song = String.Empty;

                    switch (src)
                    {
                        case 1:
                            c_song = AudioPanel.Song;
                            break;

                        case 2:
                            c_song = this.wmp.Song;
                            break;

                        case 3:
                            c_song = this.vlc.Song;
                            break;

                        case 4:
                            c_song = this.winamp.Song;
                            break;

                        case 5:
                            c_song = this.foobar.Song;
                            break;

                        case 6:
                            c_song = this.itunes.Song;
                            break;

                        case 7:
                            c_song = this.spotify.Song;
                            break;
                    }

                    if (c_song != this.last_song)
                    {
                        this.last_song = c_song;
                        Helpers.NP = c_song;
                    }
                    else check_song = false;
                }

                for (int i = 0; i < pool.Length; i++)
                    if (pool[i] != null)
                    {
                        pool[i].SocketTasks(time);

                        if (do_trickle)
                            pool[i].TrickleTasks();
                    }

                if (check_song)
                    ScriptEvents.OnSongChanged(this.last_song, player_type);

                if (do_timer)
                    ScriptEvents.OnTimer();

                Thread.Sleep(30);
            }
        }

        private void OpenChannel(object sender, OpenChannelEventArgs e)
        {
            IPEndPoint ep = new IPEndPoint(e.Room.IP, e.Room.Port);
            int index = RoomPool.Rooms.FindIndex(x => x.EndPoint.Equals(ep));

            if (index > -1)
            {
                this.SetToRoom(RoomPool.Rooms[index].EndPoint);
                this.channel_bar.Mode = ChannelBar.ModeOption.Channel;
                this.channel_bar.SelectedButton = RoomPool.Rooms[index].EndPoint;
                this.toolStrip1.Invalidate();
            }
            else
            {
                if (String.IsNullOrEmpty(e.Room.Name))
                    return;

                Room room = new Room(Settings.Time, e.Room, this, Settings.GetReg<bool>("back_bg", false));
                room.Button = new ChannelButton(room.Credentials);
                room.Panel = new RoomPanel(room.Credentials);
                room.Panel.BackColor = Color.WhiteSmoke;
                room.Panel.Dock = DockStyle.Fill;
                room.Panel.CloseClicked += this.CloseChannel;
                room.Panel.CheckUnread += this.CheckUnread;
                room.RoomNameChanged += this.RoomNameChanged;
                room.TopicChanged += this.RoomTopicChanged;
                room.ConnectEvents();
                RoomPool.Rooms.Add(room);
                this.toolStrip1.Items.Add(room.Button);
                this.SetToRoom(room.EndPoint);
                this.channel_bar.Mode = ChannelBar.ModeOption.Channel;
                this.channel_bar.SelectedButton = room.EndPoint;
                this.toolStrip1.Invalidate();
                room.Panel.CreateScreen();
            }
        }

        private void RoomTopicChanged(object sender, EventArgs e)
        {
            FavouritesListItem creds = (FavouritesListItem)sender;
            this.clist_content.UpdateFavouriteTopic(creds);
        }

        private void RoomNameChanged(object sender, EventArgs e)
        {
            FavouritesListItem creds = (FavouritesListItem)sender;
            Room room = RoomPool.Rooms.Find(x => x.Credentials.ToEndPoint().Equals(creds.ToEndPoint()));

            if (room != null)
            {
                this.toolStrip1.BeginInvoke((Action)(() =>
                {
                    room.Button.UpdateRoomName(creds.Name);
                    this.toolStrip1.Invalidate();
                }));
            }

            this.clist_content.UpdateFavouriteName(creds);
        }

        private void CloseChannel(object sender, EventArgs e)
        {
            IPEndPoint ep = (IPEndPoint)sender;
            int index = RoomPool.Rooms.FindIndex(x => x.EndPoint.Equals(ep));

            if (index > -1)
            {
                RoomPool.Rooms[index].Panel.CloseClicked -= this.CloseChannel;
                RoomPool.Rooms[index].Panel.CheckUnread -= this.CheckUnread;
                RoomPool.Rooms[index].RoomNameChanged -= this.RoomNameChanged;
                RoomPool.Rooms[index].TopicChanged -= this.RoomTopicChanged;

                for (int i = 0; i < this.toolStrip1.Items.Count; i++)
                    if (this.toolStrip1.Items[i] is ChannelButton)
                        if (((ChannelButton)this.toolStrip1.Items[i]).EndPoint.Equals(ep))
                        {
                            this.toolStrip1.Items.RemoveAt(i);
                            break;
                        }

                while (this.content1.Controls.Count > 0)
                    this.content1.Controls.RemoveAt(0);

                Room room = RoomPool.Rooms[index];
                room.Release();
                RoomPool.Rooms.RemoveAt(index);
                room = null;

                this.channel_bar.Mode = ChannelBar.ModeOption.ChannelList;
                this.toolStrip1.Invalidate();
                this.content1.Controls.Add(this.clist_content);
                this.BeginInvoke((Action)(() => this.Activate()));
            }
        }

        private void CheckUnread(object sender, EventArgs e)
        {
            if (this.toolStrip1.InvokeRequired)
                this.toolStrip1.BeginInvoke(new EventHandler(this.CheckUnread), sender, e);
            else
            {
                IPEndPoint ep = (IPEndPoint)sender;
                Room room = RoomPool.Rooms.Find(x => x.EndPoint.Equals(ep));

                if (room != null)
                {
                    bool should_update = false;

                    if (this.channel_bar.Mode != ChannelBar.ModeOption.Channel)
                        should_update = true;
                    else if (!this.channel_bar.SelectedButton.Equals(ep))
                        should_update = true;

                    if (should_update)
                        room.Button.MakeUnread();
                }
            }
        }

        private void ChannelListLabelChanged(object sender, ChannelListLabelChangedEventArgs e)
        {
            this.toolStrip1.BeginInvoke((Action)(() =>
            {
                for (int i = 0; i < this.toolStrip1.Items.Count; i++)
                    if (this.toolStrip1.Items[i] is ChannelListButton)
                    {
                        this.toolStrip1.Items[i].Text = e.Text.Replace("Channels", StringTemplate.Get(STType.TopBar, 2)).Replace("Searching", StringTemplate.Get(STType.TopBar, 3));
                        break;
                    }
            }));
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.channel_bar.Mode != ChannelBar.ModeOption.Channel)
                e.Cancel = true;
            else
            {
                IPEndPoint ep = this.channel_bar.SelectedButton;

                for (int i = 0; i < this.toolStrip1.Items.Count; i++)
                    if (this.toolStrip1.Items[i] is ChannelButton)
                        if (((ChannelButton)this.toolStrip1.Items[i]).EndPoint.Equals(ep))
                        {
                            this.moveLeftToolStripMenuItem.Enabled = i > 5;
                            this.moveRightToolStripMenuItem.Enabled = i < (this.toolStrip1.Items.Count - 1);

                            if (!this.moveLeftToolStripMenuItem.Enabled && !this.moveRightToolStripMenuItem.Enabled)
                                e.Cancel = true;

                            break;
                        }
            }
        }

        private bool can_close = false;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.can_close)
            {
                this.terminate = true;
                this.clist_content.Terminate = true;

             //   if (Awesomium.Core.WebCore.IsInitialized)
               //     Awesomium.Core.WebCore.Shutdown();

                Thread.Sleep(500);
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (this.do_once)
            {
                int frm_size_x = Settings.GetReg<int>("form_x", -1);
                int frm_size_y = Settings.GetReg<int>("form_y", -1);

                if (this.ClientSize.Width != frm_size_x || this.ClientSize.Height != frm_size_y)
                {
                    Settings.SetReg("form_x", this.ClientSize.Width);
                    Settings.SetReg("form_y", this.ClientSize.Height);
                }
            }
        }

        private void moveLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPEndPoint ep = this.channel_bar.SelectedButton;

            for (int i = 0; i < this.toolStrip1.Items.Count; i++)
                if (this.toolStrip1.Items[i] is ChannelButton)
                    if (((ChannelButton)this.toolStrip1.Items[i]).EndPoint.Equals(ep))
                    {
                        this.toolStrip1.Items.RemoveAt(i);
                        this.toolStrip1.Items.Insert((i - 1), RoomPool.Rooms.Find(x => x.EndPoint.Equals(ep)).Button);
                        this.toolStrip1.Invalidate();
                        break;
                    }
        }

        private void moveRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPEndPoint ep = this.channel_bar.SelectedButton;

            for (int i = 0; i < this.toolStrip1.Items.Count; i++)
                if (this.toolStrip1.Items[i] is ChannelButton)
                    if (((ChannelButton)this.toolStrip1.Items[i]).EndPoint.Equals(ep))
                    {
                        this.toolStrip1.Items.RemoveAt(i);
                        this.toolStrip1.Items.Insert((i + 1), RoomPool.Rooms.Find(x => x.EndPoint.Equals(ep)).Button);
                        this.toolStrip1.Invalidate();
                        break;
                    }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.channel_bar.IsFocused = true;
            this.toolStrip1.Refresh();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.channel_bar.IsFocused = false;
            this.toolStrip1.Refresh();
        }

        private void SystemMenuItemClicked(object sender, FormEx.SystemMenuEx.SystemMenuItem e)
        {
            int button = e.Index;

            if (this.SystemMenu.Count == 4)
                if (button == 2)
                    button = 7;

            switch (button)
            {
                case 0: // away|online
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "online" });
                    break;

                case 2: // next
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "next" });
                    break;

                case 3: // play|pause
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "playpause" });
                    break;

                case 4: // previous
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "previous" });
                    break;

                case 5: // stop
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "stop" });
                    break;

                case 7: // exit
                    this.IPCCommandReceived(null, new IPCListenerEventArgs { Command = "exit" });
                    break;
            }
        }

        private void showAsOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showAsAwayToolStripMenuItem.Enabled = true;
            this.showAsOnlineToolStripMenuItem.Enabled = false;

            if (this.JumpList != null)
            {
                this.JumpList.RemoveAt(0);
                this.JumpList.Insert(0, new JumpListItem(StringTemplate.Get(STType.SystemTray, 1), "cbjl_online"), 6);
            }

            if (this.SystemMenu != null)
            {
                this.SystemMenu.RemoveAt(0);
                this.SystemMenu.Insert(0, new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.SystemTray, 1), this.sysmenubmps[0]));
            }

            if (this.OverlayIcon != null)
                this.OverlayIcon.Hide();

            if (this.PreviewToolStrip != null)
            {
                this.PreviewToolStrip[0].Button.Enabled = false;
                this.PreviewToolStrip[1].Button.Enabled = true;
            }

            Settings.IsAway = false;
            RoomPool.Rooms.ForEach(x => x.UpdateAwayStatus(false));
        }

        private void showAsAwayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showAsAwayToolStripMenuItem.Enabled = false;
            this.showAsOnlineToolStripMenuItem.Enabled = true;

            if (this.JumpList != null)
            {
                this.JumpList.RemoveAt(0);
                this.JumpList.Insert(0, new JumpListItem(StringTemplate.Get(STType.SystemTray, 0), "cbjl_online"), 5);
            }

            if (this.SystemMenu != null)
            {
                this.SystemMenu.RemoveAt(0);
                this.SystemMenu.Insert(0, new FormEx.SystemMenuEx.SystemMenuItem(StringTemplate.Get(STType.SystemTray, 0), this.sysmenubmps[1]));
            }

            if (this.OverlayIcon != null)
                this.OverlayIcon.Show();

            if (this.PreviewToolStrip != null)
            {
                this.PreviewToolStrip[0].Button.Enabled = true;
                this.PreviewToolStrip[1].Button.Enabled = false;
            }

            Settings.IsAway = true;
            RoomPool.Rooms.ForEach(x => x.UpdateAwayStatus(true));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.can_close = true;
            this.Close();
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!AudioPanel.Available)
                return;

            if (e.ClickedItem.Equals(this.toolStripButton1))
            {
                this.volume.StartPosition = FormStartPosition.Manual;
                this.volume.Location = new Point(MousePosition.X - 40, MousePosition.Y - 70);
                this.volume.Show();
            }
            else if (e.ClickedItem.Equals(this.toolStripButton3))
                this.audio_content.StopClicked();
            else if (e.ClickedItem.Equals(this.toolStripButton4))
                this.audio_content.PreviousClicked();
            else if (e.ClickedItem.Equals(this.toolStripButton5))
                this.audio_content.PlayPauseClicked();
            else if (e.ClickedItem.Equals(this.toolStripButton6))
                this.audio_content.NextClicked();
        }

        private void toolStripDropDownButton1_DropDownOpening(object sender, EventArgs e)
        {
            this.randomToolStripMenuItem.Checked = AudioPanel.DoRandom;
            this.repeatToolStripMenuItem.Checked = AudioPanel.DoRepeat;
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(this.randomToolStripMenuItem))
                AudioPanel.DoRandom = !AudioPanel.DoRandom;
            else if (e.ClickedItem.Equals(this.repeatToolStripMenuItem))
                AudioPanel.DoRepeat = !AudioPanel.DoRepeat;
            else if (e.ClickedItem.Equals(this.clearListToolStripMenuItem))
            {
                DialogResult dr = MessageBox.Show(StringTemplate.Get(STType.AudioPlayer, 0),
                                                  "cb0t",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                    this.audio_content.ClearPlaylist();
            }
            else if (e.ClickedItem.Equals(this.importFilesToolStripMenuItem))
            {
                this.toolStripDropDownButton1.HideDropDown();
                SharedUI.OpenFile.Multiselect = true;
                SharedUI.OpenFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                SharedUI.OpenFile.FileName = String.Empty;

                if (SharedUI.OpenFile.ShowDialog() == DialogResult.OK)
                {
                    String[] files = SharedUI.OpenFile.FileNames;

                    if (files != null)
                        if (files.Length > 0)
                            this.audio_content.ImportFilesFromDialog(files);
                }
            }
            else if (e.ClickedItem.Equals(this.importFolderToolStripMenuItem))
            {
                this.toolStripDropDownButton1.HideDropDown();
                SharedUI.OpenFolder.ShowNewFolderButton = false;
                SharedUI.OpenFolder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

                if (SharedUI.OpenFolder.ShowDialog() == DialogResult.OK)
                {
                    String folder = SharedUI.OpenFolder.SelectedPath;

                    if (!String.IsNullOrEmpty(folder))
                        this.audio_content.ImportFolderFromDialog(folder);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Room room = RoomPool.Rooms.Find(x => x.RoomIsVisible);

            if (room != null)
                room.SpellCheck();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            VoicePlayer.Tick(this);

            if (VoiceRecorder.Recording)
            {
                Room room = RoomPool.Rooms.Find(x => x.RoomIsVisible);

                if (room != null)
                    room.VCTick();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 953)
            {
                if (m.WParam != (IntPtr)4)
                    VoicePlayer.PlaybackCompleted();
            }
            else base.WndProc(ref m);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Room room = RoomPool.Rooms.Find(x => x.RoomIsVisible);

            if (e.KeyCode == Keys.F2)
            {
                if (room != null)
                    room.StartRecording();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (!VoiceRecorder.Recording)
                    VoicePlayer.PlaybackCompleted();
                else if (room != null)
                    room.CancelRecording();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Room room = RoomPool.Rooms.Find(x => x.RoomIsVisible);

            if (room != null)
                if (e.KeyCode == Keys.F2)
                    room.StopRecording();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.can_show_popup = true;
        }

        private void notifyIcon1_BalloonTipClosed(object sender, EventArgs e)
        {
            this.can_show_popup = true;
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.audio_content.NextClicked();
        }

        private void playpauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.audio_content.PlayPauseClicked();
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.audio_content.PreviousClicked();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.audio_content.StopClicked();
        }
    }
}
