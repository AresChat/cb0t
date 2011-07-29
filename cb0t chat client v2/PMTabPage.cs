using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class PMTabPage : TabPage // 4
    {
        private OutputTextBox chatscreen;
        private ToolStrip buttons;
        private ToolStripButton bold;
        private ToolStripButton italic;
        private ToolStripButton underline;
        private ToolStripButton fore;
        private ToolStripButton back;
        private ToolStripButton emoticons;
        private ToolStripButton voiceclipbutton;
        private ToolStripButton mutebutton;
        private InputTextBox sendbox;
        private EmoticonMenu emotemenu;
        private ColorMenu colormenu;
        private RecordingMonitor recording_monitor;

        public String username;
        private String my_username;
        private String ip;
        private bool first_post = true;
        private bool is_recording = false;
        private int recording_tick = 0;
        public int tab_ident;
        private VoiceRecorder vrec = new VoiceRecorder();
        public bool can_auto_play = false;
        public bool high_quality = false;

        public event Packets.SendPacketDelegate OnPacketSending;
        public event ChannelList.ChannelClickedDelegate OnHashlinkClicked;

        public delegate bool UploadVoicePacketsHandler(byte[][] packets);
        public event UploadVoicePacketsHandler UploadVoicePackets;

        public event ChatContainerTabPage.PauseVCHandler PauseVCNow;

        public PMTabPage(String name, String my_username, int tab_ident, String ip, bool can_vc, bool vc_quality)
        {
            this.high_quality = vc_quality;
            this.username = name;
            this.my_username = my_username;
            this.tab_ident = tab_ident;
            this.ip = ip;

            this.ImageIndex = 1;
            this.BackColor = SystemColors.Control;
            this.Location = new Point(4, 22);
            this.Name = name;
            this.Padding = new Padding(3);
            this.Size = new Size(866, 435);
            this.TabIndex = 1;
            this.Text = name;
            this.Tag = "pm";

            this.chatscreen = new OutputTextBox(false);
            this.chatscreen.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.chatscreen.BorderStyle = BorderStyle.None;
            this.chatscreen.Location = new Point(7, 6);
            this.chatscreen.Name = "chatscreen";
            this.chatscreen.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.chatscreen.Size = new Size(852, 354);
            this.chatscreen.TabIndex = 9;
            this.chatscreen.Text = "";
            this.chatscreen.OnHashlinkClicked += new ChannelList.ChannelClickedDelegate(this.OnHashlinkClicking);
            this.chatscreen.OnDeleteVC += new OutputTextBox.VCDelHandler(this.chatscreen_OnDeleteVC);
            this.chatscreen.OnClickedVC += new OutputTextBox.VCHandler(this.chatscreen_OnClickedVC);
            this.chatscreen.OnSaveVC += new OutputTextBox.VCHandler(this.chatscreen_OnSaveVC);
            this.chatscreen.RadioHashlinkClicked += new OutputTextBox.RadioHashlinkClickedHandler(this.RadioHashlinkClicked);
            this.Controls.Add(this.chatscreen);

            this.recording_monitor = new RecordingMonitor();
            this.recording_monitor.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            this.recording_monitor.BorderStyle = BorderStyle.None;
            this.recording_monitor.Location = new Point(7, 360);
            this.recording_monitor.Size = new Size(852, 15);
            this.Controls.Add(this.recording_monitor);

            this.bold = new ToolStripButton();
            this.bold.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.bold.ImageTransparentColor = Color.Magenta;
            this.bold.Name = "bold";
            this.bold.Size = new Size(23, 22);
            this.bold.Text = "B";
            this.bold.Font = new Font(this.Font, FontStyle.Bold);
            this.bold.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.bold.Click += new EventHandler(this.OnBoldClicked);
            this.bold.ToolTipText = "bold";

            this.italic = new ToolStripButton();
            this.italic.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.italic.ImageTransparentColor = Color.Magenta;
            this.italic.Name = "italic";
            this.italic.Size = new Size(23, 22);
            this.italic.Text = "I";
            this.italic.Font = new Font(this.Font, FontStyle.Italic);
            this.italic.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.italic.Click += new EventHandler(this.OnItalicClicked);
            this.italic.ToolTipText = "italic";

            this.underline = new ToolStripButton();
            this.underline.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.underline.ImageTransparentColor = Color.Magenta;
            this.underline.Name = "underline";
            this.underline.Size = new Size(23, 22);
            this.underline.Text = "U";
            this.underline.Font = new Font(this.Font, FontStyle.Underline);
            this.underline.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.underline.Click += new EventHandler(this.OnUnderlineClicked);
            this.underline.ToolTipText = "underline";

            this.fore = new ToolStripButton();
            this.fore.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.fore.ImageTransparentColor = Color.Magenta;
            this.fore.Name = "fore color";
            this.fore.Size = new Size(23, 22);
            this.fore.Text = "fore color";
            this.fore.Image = AresImages.TransparentEmoticons[47];
            this.fore.Click += new EventHandler(this.OnFColorMenuClicked);

            this.back = new ToolStripButton();
            this.back.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.back.ImageTransparentColor = Color.Magenta;
            this.back.Name = "back color";
            this.back.Size = new Size(23, 22);
            this.back.Text = "back color";
            this.back.Image = AresImages.TransparentEmoticons[48];
            this.back.Click += new EventHandler(this.OnBColorMenuClicked);

            this.emoticons = new ToolStripButton();
            this.emoticons.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.emoticons.Image = AresImages.TransparentEmoticons[0];
            this.emoticons.ImageTransparentColor = Color.Magenta;
            this.emoticons.Name = "emoticons";
            this.emoticons.Size = new Size(23, 22);
            this.emoticons.Text = "emoticons";
            this.emoticons.Click += new EventHandler(this.OnEmoticonMenuClicked);

            this.voiceclipbutton = new ToolStripButton();
            this.voiceclipbutton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.voiceclipbutton.Image = AresImages.GrayStar_Files;
            this.voiceclipbutton.ImageTransparentColor = Color.Magenta;
            this.voiceclipbutton.Name = "record a voice clip";
            this.voiceclipbutton.Size = new Size(23, 22);
            this.voiceclipbutton.Text = "record a voice clip";
            this.voiceclipbutton.ToolTipText = "To record a Voice Clip, hold down\r\nthis button or press F2.  Release the\r\nbutton to send your message.";
            this.voiceclipbutton.MouseDown += new MouseEventHandler(this.voiceclipbutton_MouseDown);
            this.voiceclipbutton.MouseUp += new MouseEventHandler(this.voiceclipbutton_MouseUp);
            this.voiceclipbutton.Enabled = can_vc;

            this.mutebutton = new ToolStripButton();
            this.mutebutton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.mutebutton.Image = AresImages.GoldStar_Files;
            this.mutebutton.ImageTransparentColor = Color.Magenta;
            this.mutebutton.Name = "auto play voice clips";
            this.mutebutton.Size = new Size(23, 22);
            this.mutebutton.Text = "auto play voice clips";
            this.mutebutton.Click += new EventHandler(this.mutebutton_Click);

            this.buttons = new ToolStrip();
            this.buttons.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.buttons.Dock = DockStyle.None;
            this.buttons.GripStyle = ToolStripGripStyle.Hidden;
            this.buttons.Items.AddRange(new ToolStripItem[] {
            this.bold,
            this.italic,
            this.underline,
            this.fore,
            this.back,
            this.emoticons,
            this.voiceclipbutton,
            this.mutebutton});
            this.buttons.Location = new Point(7, 378);
            this.buttons.Name = "buttons";
            this.buttons.RenderMode = ToolStripRenderMode.System;
            this.buttons.Size = new Size(169, 25);
            this.buttons.TabIndex = 7;
            this.buttons.Text = "buttons";
            this.Controls.Add(this.buttons);

            this.sendbox = new InputTextBox(true, this.username);
            this.sendbox.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            this.sendbox.BorderStyle = BorderStyle.FixedSingle;
            this.sendbox.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.sendbox.Location = new Point(7, 406);
            this.sendbox.Name = "sendbox";
            this.sendbox.Size = new Size(852, 23);
            this.sendbox.TabIndex = 0;
            this.sendbox.OnPacketDispatching += new Packets.SendPacketDelegate(this.OnPacketDispatching);
            this.sendbox.OnPMSending += new InputTextBox.OnPMSendingDelegate(this.OnPMSending);
            this.Controls.Add(this.sendbox);

            this.emotemenu = new EmoticonMenu(this.sendbox);
            this.colormenu = new ColorMenu(this.sendbox);
        }

        public event OutputTextBox.RadioHashlinkClickedHandler RadioHashlink;

        private void RadioHashlinkClicked(String url)
        {
            this.RadioHashlink(url);
        }

        public event OutputTextBox.VCDelHandler OnDeleteVC;
        public event OutputTextBox.VCHandler OnClickedVC;
        public event OutputTextBox.VCHandler OnSaveVC;

        private void chatscreen_OnSaveVC(String id, bool is_pm)
        {
            this.OnSaveVC(id, is_pm);
        }

        private void chatscreen_OnClickedVC(String id, bool is_pm)
        {
            this.OnClickedVC(id, is_pm);
        }

        private void chatscreen_OnDeleteVC(String id, bool is_pm)
        {
            this.OnDeleteVC(id, is_pm);
        }

        public void FreeResources()
        {
            this.Controls.Clear();
            this.chatscreen.Dispose();
            this.recording_monitor.Dispose();
            this.bold.Dispose();
            this.italic.Dispose();
            this.underline.Dispose();
            this.fore.Dispose();
            this.back.Dispose();
            this.emoticons.Dispose();
            this.voiceclipbutton.Dispose();
            this.mutebutton.Dispose();
            this.buttons.Dispose();
            this.sendbox.Dispose();
            this.colormenu.Dispose();
            this.emotemenu.Dispose();
            this.Dispose();
        }

        public void HotKeyPressed(bool down)
        {
            if (down)
            {
                if (this.is_recording)
                    return;

                this.is_recording = true;
                this.recording_tick = 0;
                this.recording_monitor.SetTicks(this.recording_tick, Settings.record_quality);
                this.RecordingStarts();
            }
            else
            {
                if (!this.is_recording)
                    return;

                this.recording_monitor.SetTicks(-1, Settings.record_quality);
                this.is_recording = false;
                this.RecordingFinished();
            }
        }

        public void VCEscapeCancel()
        {
            this.recording_monitor.SetTicks(-1, Settings.record_quality);
            this.is_recording = false;
            this.PauseVCNow(false);
            this.vrec.Cancel();
            this.chatscreen.VoiceClipCancel();
            this.recording_tick = -1;
        }

        public void AddClip(VoiceClipReceived c)
        {
            this.chatscreen.ReceivedAVoiceClip("voice_clip_#" + c.hash + " received from " + c.from + " \\\\save_#" + c.hash);
        }

        private void mutebutton_Click(object sender, EventArgs e)
        {
            this.can_auto_play = !this.can_auto_play;
            this.mutebutton.Image = this.can_auto_play ? AresImages.GoldStar_NoFiles : AresImages.GoldStar_Files;
        }

        private void voiceclipbutton_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.is_recording)
                return;

            if (e.Button == MouseButtons.Left)
            {
                this.is_recording = true;
                this.recording_tick = 0;
                this.recording_monitor.SetTicks(this.recording_tick, Settings.record_quality);
                this.RecordingStarts();
            }
        }

        private void voiceclipbutton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!this.is_recording)
                    return;

                this.recording_monitor.SetTicks(-1, Settings.record_quality);
                this.is_recording = false;
                this.RecordingFinished();
            }
        }

        public void OnVCTimerTick()
        {
            if (this.is_recording)
            {
                this.recording_monitor.SetTicks(++this.recording_tick, Settings.record_quality);

                if (this.recording_tick > (Settings.record_quality ? 15 : 25))
                {
                    this.recording_monitor.SetTicks(-1, Settings.record_quality);
                    this.is_recording = false;
                    this.RecordingFinished();
                }
            }
        }

        private void RecordingStarts()
        {
            this.PauseVCNow(true);
            this.vrec.Start(this.high_quality && Settings.record_quality);
        }

        private void RecordingFinished()
        {
            this.PauseVCNow(false);
            this.vrec.Stop();

            if (this.UploadVoicePackets(this.vrec.GetAresPackets(true, this.username, this.recording_tick)))
                this.chatscreen.SentAVoiceClip();

            this.recording_tick = -1;
        }


        private delegate void SetVCEnabledButtonDelegate(bool yes, bool vchq);
        public void SetVCEnabledButton(bool yes, bool vchq)
        {
            if (this.buttons.InvokeRequired)
                this.buttons.BeginInvoke(new SetVCEnabledButtonDelegate(this.SetVCEnabledButton), yes, vchq);
            else
            {
                this.voiceclipbutton.Enabled = yes;
                this.high_quality = vchq;
            }
        }

        public void FixScreen()
        {
            if (this.chatscreen != null)
                this.chatscreen.FixScreen();
        }

        private void OnHashlinkClicking(ChannelObject cObj)
        {
            this.OnHashlinkClicked(cObj);
        }

        public void OnPMReceived(String name, String text, UserFont font, object custom_emotes, bool supports_emoticons)
        {
            this.chatscreen.PM(name, text, font, custom_emotes);

            if (this.first_post)
            {
                this.first_post = false;

                if (Settings.enable_pm_reply)
                {
                    String[] lines = Settings.pm_reply.Split(new String[] { "\r\n" }, StringSplitOptions.None);

                    foreach (String l in lines)
                    {
                        this.OnPMSending(Helpers.FormatAresColorCodes(l).Replace("+n", this.username).Replace("+ip", this.ip));
                        this.OnPacketDispatching(Packets.PMPacket(this.username, Helpers.FormatAresColorCodes(l).Replace("+n", this.username).Replace("+ip", this.ip)));
                    }
                }
            }
        }

        private void OnPMSending(String text)
        {
            this.chatscreen.PM(this.my_username, text, new UserFont(Settings.p_font_name, Settings.p_font_size, Settings.p_name_col, Settings.p_text_col), CustomEmotes.Emotes);
        }

        private void OnPacketDispatching(byte[] packet)
        {
            this.OnPacketSending(packet);
        }

        public void OnFocusReceived()
        {
            this.sendbox.Focus();
        }

        private void OnEmoticonMenuClicked(object sender, EventArgs e)
        {
            this.emotemenu.Show();
        }

        public void SetAnnounceText(String text)
        {
            this.chatscreen.Announce(text);
        }

        private void OnFColorMenuClicked(object sender, EventArgs e)
        {
            this.colormenu.foreground = true;
            this.colormenu.Show(MousePosition.X - 16, MousePosition.Y - (16 * 23));
        }

        private void OnBColorMenuClicked(object sender, EventArgs e)
        {
            this.colormenu.foreground = false;
            this.colormenu.Show(MousePosition.X - 16, MousePosition.Y - (16 * 23));
        }

        private void OnBoldClicked(object sender, EventArgs e)
        {
            this.sendbox.Text += "\x00026";
            this.sendbox.SelectionStart = this.sendbox.Text.Length;
        }

        private void OnItalicClicked(object sender, EventArgs e)
        {
            this.sendbox.Text += "\x00029";
            this.sendbox.SelectionStart = this.sendbox.Text.Length;
        }

        private void OnUnderlineClicked(object sender, EventArgs e)
        {
            this.sendbox.Text += "\x00027";
            this.sendbox.SelectionStart = this.sendbox.Text.Length;
        }


    }
}
