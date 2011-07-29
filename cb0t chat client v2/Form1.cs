using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hwnd);

        private bool appearance_set = false;
        private MainToolStripAppearance mtsa;
        private bool terminate = false;
        private int tab_select = 1;

        public Form1(String start_hashlink)
        {
            Avatar.LoadRegularAv();
            Settings.Populate();
            ChatEvents.LoadChatEvents();
            MenuOptions.Load();
            AresImages.LoadLargeImages();
            AresImages.LoadEmoticons();
            AresImages.LoadChatImages();
            AresImages.LoadChannelListImages();
            AresImages.LoadMiniImages();
            MySoundEffects.LoadSounds();
            CustomEmotes.Load();

            InitializeComponent();

            this.controlPanel1.SendCustomEmotePacket += new ControlPanel.SendCustomEmotePacketHandler(this.SendCustomEmotePacket);
            this.chat1.OnTick += new EventHandler(this.SecondTick);
            this.chat1.RadioHashlink += new OutputTextBox.RadioHashlinkClickedHandler(this.RadioHashlink);
            this.chat1.OnVoiceClipStarted += new EventHandler(this.OnVoiceClipStarted);
            this.chat1.OnVoiceClipStopped += new EventHandler(this.OnVoiceClipStopped);
            this.audioPlayer1.SongTitleChanged += new EventHandler(this.SongTitleChanged);

            if (start_hashlink != null)
            {
                String str = start_hashlink;

                if (str.StartsWith("cb0t://"))
                {
                    str = str.Substring(7);

                    if (str.EndsWith("/"))
                        str = str.Substring(0, str.Length - 1);

                    if (str.StartsWith("radio:"))
                    {
                        str = str.Substring(6);
                        this.audioPlayer1.DownloadPlsFile(this, new DownloadPlsFileEventArgs(str));
                    }
                    else this.ProcessExternalHashlink(str);
                }
                else
                {
                    String[] types = new String[] { ".WAV", ".MP3", ".WMA" };

                    foreach (String t in types)
                        if (start_hashlink.ToUpper().EndsWith(t))
                        {
                            this.load_playlist = false;
                            this.audioPlayer1.ExternalSongRequest(start_hashlink);
                            break;
                        }
                }
            }
        }

        private void OnVoiceClipStopped(object sender, EventArgs e)
        {
            this.audioPlayer1.VoiceClipStopped();
        }

        private void OnVoiceClipStarted(object sender, EventArgs e)
        {
            this.audioPlayer1.VoiceClipStarted();
        }

        private void SongTitleChanged(object sender, EventArgs e)
        {
            this.chat1.InternalSongChanged();
        }

        private void RadioHashlink(String url)
        {
            this.audioPlayer1.RadioHashlink(url);
        }

        private void SecondTick(object sender, EventArgs e)
        {
            this.audioPlayer1.SecondsTick();
        }

        private void SendCustomEmotePacket(byte[] packet)
        {
            this.chat1.SendCustomEmoticonPacket(packet);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x004A:
                    Win32.CopyDataStruct st = (Win32.CopyDataStruct)Marshal.PtrToStructure(m.LParam, typeof(Win32.CopyDataStruct));
                    String str = Marshal.PtrToStringUni(st.lpData);

                    if (str.StartsWith("cb0t://"))
                    {
                        str = str.Substring(7);

                        if (str.EndsWith("/"))
                            str = str.Substring(0, str.Length - 1);

                        if (str.StartsWith("radio:"))
                        {
                            str = str.Substring(6);
                            this.audioPlayer1.DownloadPlsFile(this, new DownloadPlsFileEventArgs(str));
                        }
                        else this.ProcessExternalHashlink(str);
                    }
                    else
                    {
                        String[] types = new String[] { ".WAV", ".MP3", ".WMA" };

                        foreach (String t in types)
                            if (str.ToUpper().EndsWith(t))
                            {
                                this.audioPlayer1.ExternalSongRequest(str);
                                break;
                            }
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }

        }

        private void ProcessExternalHashlink(String hashlink)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (!this.Visible)
                        this.Show();

                    SetForegroundWindow(this.Handle);
                }));
            }

            ChannelObject obj = Hashlink.DecodeHashlink(hashlink);

            if (obj != null)
                this.chat1.OnChannelClicked(obj);
        }

        private bool load_playlist = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.appearance_set)
            {
                if (Settings.size_x > 10 && Settings.size_y > 10)
                    this.Size = new Size(Settings.size_x, Settings.size_y);

                Settings.size_allow = true;
                Settings.cbot_visible = true;

                this.controlPanel1.Visible = false;
                this.audioPlayer1.Visible = false;
                this.toolStripButton1.Image = AresImages.ChatTabIcon;
                this.toolStripButton2.Image = AresImages.ControlPanelTabIcon;
                this.toolStripButton3.Image = AresImages.AudioPlayerIcon;
                this.appearance_set = true;
                this.mtsa = new MainToolStripAppearance();
                this.toolStrip1.Renderer = this.mtsa;
                this.chat1.session_loaded = true;
                this.chat1.OnRaiseBalloon += new ChatContainerTabPage.NotifyDelegate(this.OnRaiseBalloon);
                this.controlPanel1.OnHashlinkDownloaded += new ControlPanel.DownloadHashlinkDelegate(this.OnHashlinkDownloaded);
                this.controlPanel1.OnDCPortChanged += new ControlPanel.DCPortChangedDelegate(this.OnDCPortChanged);
                this.controlPanel1.OnAvatarProtocolPacket += new ControlPanel.AvatarProtocolPacketDelegate(this.OnAvatarProtocolPacket);
                this.controlPanel1.OnEnableCustomNames += new ControlPanel.EnableCustomNamesDelegate(this.OnEnableCustomNames);
                this.controlPanel1.PopulateSettings();

                if (this.load_playlist)
                    this.audioPlayer1.LoadPlaylist();

                this.chat1.Init();
            }
        }

        private void OnEnableCustomNames(bool enabled)
        {
            this.chat1.SendGlobalPacket(Packets.DisableCustomNames(!enabled));
        }

        private void OnRaiseBalloon(String text, int tab_ident)
        {
            String[] str_split = text.Split(new String[] { "\0" }, StringSplitOptions.None);

            if (str_split.Length == 3)
            {
                this.tab_select = tab_ident;
                this.notifyIcon1.BalloonTipTitle = str_split[1];

                if (str_split[0] == "[pm_notify]")
                {
                    this.notifyIcon1.BalloonTipText = "\r\n" + str_split[2] + " sent you a Private Message";

                    if (Settings.pm_sound)
                        MySoundEffects.PlayPMSound();
                }
                else if (str_split[2] != "\x0001") // nudge
                {
                    this.notifyIcon1.BalloonTipText = "\r\nNotification word \"" + str_split[2] + "\" was ";

                    if (str_split[0].Length > 0)
                        this.notifyIcon1.BalloonTipText += "typed by " + str_split[0];
                    else
                        this.notifyIcon1.BalloonTipText += "announced";

                    if (Settings.notify_sound)
                        MySoundEffects.PlayNotifySound();
                }
                else // word trigger
                {
                    this.notifyIcon1.BalloonTipText = "\r\nYou were nudged by " + str_split[0];
                }

                this.notifyIcon1.BalloonTipText += "\r\nClick here to view this chat room...\r\n";
                this.notifyIcon1.ShowBalloonTip(5000);
            }
        }

        private void OnDCPortChanged()
        {
            this.chat1.ResetDCListener();
        }

        private void OnHashlinkDownloaded(ChannelObject cObj) // join by hashlink
        {
            this.mtsa.Selected = MainToolStripAppearance.SelectedTab.Chat;
            this.toolStripButton2.Invalidate();
            this.toolStripButton1.Invalidate();
            this.controlPanel1.Visible = false;
            this.chat1.Visible = true;
            this.chat1.OnChannelClicked(cObj);
        }

        private void OnAvatarProtocolPacket(byte[] packet)
        {
            this.chat1.SendAvatarPacket(packet);
        }

        private void toolStripButton1_Click(object sender, EventArgs e) // chat
        {
            this.mtsa.Selected = MainToolStripAppearance.SelectedTab.Chat;
            this.toolStripButton2.Invalidate();
            this.toolStripButton3.Invalidate();
            this.controlPanel1.Visible = false;
            this.audioPlayer1.Visible = false;
            this.chat1.Visible = true;
            Settings.UpdateRecords();

            if (this.controlPanel1.send_font_packet)
            {
                if (Settings.enable_my_custom_font)
                {
                    this.controlPanel1.send_font_packet = false;
                    this.chat1.SendGlobalPacket(Packets.FontPacket());
                }
                else
                {
                    this.controlPanel1.send_font_packet = false;
                    this.chat1.SendGlobalPacket(Packets.FontOffPacket());
                }
            }

            if (this.controlPanel1.send_vc_support_packet)
            {
                this.controlPanel1.send_vc_support_packet = false;
                this.chat1.SendVCSupportPacket(Packets.EnableClips(Settings.enable_clips, Settings.receive_private_clips));
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) // control panel
        {
            this.mtsa.Selected = MainToolStripAppearance.SelectedTab.ControlPanel;
            this.toolStripButton1.Invalidate();
            this.toolStripButton3.Invalidate();
            this.chat1.Visible = false;
            this.audioPlayer1.Visible = false;
       //     this.controlPanel1.ResetEventTab();
       //     this.controlPanel1.ResetMenuOptionsTab();
            this.controlPanel1.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.send_to_tray && !this.terminate)
            {
                this.Hide();
                Settings.cbot_visible = false;
                e.Cancel = true;
                return;
            }

            this.chat1.terminate = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();

                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;

                Settings.cbot_visible = true;
                SetForegroundWindow(this.Handle);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.terminate = true;
            this.Close();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (!this.Visible)
                this.Show();

            this.chat1.ViewSpecificChat(this.tab_select);

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            Settings.cbot_visible = true;
            SetForegroundWindow(this.Handle);
        }

        protected override void OnMove(EventArgs e)
        {
            if (this.chat1 != null)
                this.chat1.RedrawTopic();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (Settings.size_allow)
                {
                    Settings.size_x = this.Size.Width;
                    Settings.size_y = this.Size.Height;
                    Settings.UpdateRecords();
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Settings.cbot_visible = !(this.WindowState == FormWindowState.Minimized);
        }

        private void timer1_Tick(object sender, EventArgs e) // 1 second tick on GUI thread
        {
            this.chat1.GUITick();
        }



        /* voice chat hot key */

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                this.chat1.VCHotKey(true);

            if (e.KeyCode == Keys.Escape)
                this.chat1.VCRecordCancel();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                this.chat1.VCHotKey(false);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.mtsa.Selected = MainToolStripAppearance.SelectedTab.AudioPlayer;
            this.toolStripButton1.Invalidate();
            this.toolStripButton2.Invalidate();
            this.chat1.Visible = false;
            this.controlPanel1.Visible = false;
            this.audioPlayer1.Visible = true;
        }

    }
}