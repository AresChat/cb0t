using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace cb0t_chat_client_v2
{
    class ChatTabPage : TabPage // 3
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
        private ToolStripLabel urltag;
        private InputTextBox sendbox;
        private Topic topic;
        private Userlist userlist;
        private EmoticonMenu emotemenu;
        private ColorMenu colormenu;
        private SplitContainer splitter;
        private WhoIsWriting whois_panel;
        private VoiceRecorder vrec = new VoiceRecorder();
        
        public String my_username;

        public event Packets.SendPacketDelegate OnPacketSending;
        public event Userlist.PMRequestDelegate OnPMRequesting;
        public event ChannelList.ChannelClickedDelegate HashlinkClicked;
        public event Userlist.FileBrowseRequest OnFileBrowseRequesting;
        public event Userlist.PMRequestDelegate OnWhoisRequested;
        public event Userlist.PMRequestDelegate OnIgnoreRequested;
        public event Userlist.PMRequestDelegate OnVCIgnoreRequested;
        public event Userlist.PMRequestDelegate OnNudgeRequested;
        public event Packets.SendPacketDelegate OnSendToAll;
        public event Userlist.PMRequestDelegate OnDCReq;
        public event InputTextBox.LagTestDelegate OnBeginLagTest;
        public event InputTextBox.WritingDelegate OnWritingProceed;
        public event Userlist.PMRequestDelegate OnMiddleClicked;

        public delegate void ShowPlayListDelegate();
        public event ShowPlayListDelegate OnPlaylistRequesting;

        public event OutputTextBox.CATDelegate OnCATReceived;

        private bool is_recording = false;
        private int recording_tick = 0;
        public bool can_auto_play = false;
        public bool high_quality = false;

        public delegate bool UploadVoicePacketsHandler(byte[][] packets);
        public event UploadVoicePacketsHandler UploadVoicePackets;

        public event ChatContainerTabPage.PauseVCHandler PauseVCNow;

        public ChatTabPage(String my_username)
        {
            this.my_username = my_username;
            this.ImageIndex = 0;
            this.BackColor = SystemColors.Control;
            this.Location = new Point(4, 22);
            this.Name = "Chat";
            this.Padding = new Padding(3);
            this.Size = new Size(866, 435); // 866, 435
            this.TabIndex = 1;
            this.Text = "Chat";
            this.Tag = "chat";

            this.splitter = new SplitContainer();
            this.splitter.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.splitter.Location = new Point(7, 34);
            this.splitter.Size = new Size(852, 341);
            this.splitter.SplitterDistance = 582;


            this.chatscreen = new OutputTextBox(true);
            this.chatscreen.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.chatscreen.BorderStyle = BorderStyle.None;
            this.chatscreen.Location = new Point(0,0);
            this.chatscreen.Name = "outputTextBox1";
            this.chatscreen.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.chatscreen.Size = new Size(582, 326); // 582, 341
            this.chatscreen.TabIndex = 8;
            this.chatscreen.Text = "";
            this.chatscreen.OnHashlinkClicked += new ChannelList.ChannelClickedDelegate(this.OnHashlinkClicking);
            this.chatscreen.OnCopyNameRequesting += new Userlist.PMRequestDelegate(this.OnMiddleClicking);
            this.chatscreen.OnCAT += new OutputTextBox.CATDelegate(this.OnCAT);
            this.chatscreen.OnDeleteVC += new OutputTextBox.VCDelHandler(this.chatscreen_OnDeleteVC);
            this.chatscreen.OnClickedVC += new OutputTextBox.VCHandler(this.chatscreen_OnClickedVC);
            this.chatscreen.OnSaveVC += new OutputTextBox.VCHandler(this.chatscreen_OnSaveVC);
            this.chatscreen.RadioHashlinkClicked += new OutputTextBox.RadioHashlinkClickedHandler(this.RadioHashlinkClicked);
            this.splitter.Panel1.Controls.Add(this.chatscreen);

            this.whois_panel = new WhoIsWriting();
            this.whois_panel.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            this.whois_panel.BorderStyle = BorderStyle.None;
            this.whois_panel.Location = new Point(0, 326);
            this.whois_panel.Size = new Size(582, 15);
            this.splitter.Panel1.Controls.Add(this.whois_panel);

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
            this.voiceclipbutton.Enabled = false;

            this.mutebutton = new ToolStripButton();
            this.mutebutton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.mutebutton.Image = AresImages.GoldStar_Files;
            this.mutebutton.ImageTransparentColor = Color.Magenta;
            this.mutebutton.Name = "auto play voice clips";
            this.mutebutton.Size = new Size(23, 22);
            this.mutebutton.Text = "auto play voice clips";
            this.mutebutton.Click += new EventHandler(this.mutebutton_Click);

            this.urltag = new ToolStripLabel();
            this.urltag.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.urltag.IsLink = true;
            this.urltag.Name = "urltag";
            this.urltag.Size = new Size(28, 22);
            this.urltag.Text = "";
            this.urltag.ToolTipText = "";
            this.urltag.ForeColor = SystemColors.Control;
            this.urltag.LinkColor = SystemColors.Control;
            this.urltag.ActiveLinkColor = SystemColors.Control;
            this.urltag.Click += new EventHandler(this.OnLinkClicked);
            this.urltag.Paint += new PaintEventHandler(this.OnDrawUrlTag);
            this.urltag.MouseDown += new MouseEventHandler(this.urltag_MouseDown);
            this.urltag.MouseUp += new MouseEventHandler(this.urltag_MouseUp);
            this.urltag.MouseLeave += new EventHandler(this.urltag_MouseLeave);

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
            this.mutebutton,
            this.urltag});
            this.buttons.Location = new Point(7, 378);
            this.buttons.Name = "buttons";
            this.buttons.RenderMode = ToolStripRenderMode.System;
            this.buttons.Size = new Size(169, 25);
            this.buttons.TabIndex = 7;
            this.buttons.Text = "buttons";
            this.Controls.Add(this.buttons);

            this.sendbox = new InputTextBox(false);
            this.sendbox.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            this.sendbox.BorderStyle = BorderStyle.FixedSingle;
            this.sendbox.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.sendbox.Location = new Point(7, 406);
            this.sendbox.Name = "sendbox";
            this.sendbox.Size = new Size(852, 23);
            this.sendbox.TabIndex = 0;
            this.sendbox.OnPacketDispatching += new Packets.SendPacketDelegate(this.OnPacketDispatching);
            this.sendbox.OnPacketToAll += new Packets.SendPacketDelegate(this.OnPacketToAll);
            this.sendbox.OnLagTesting += new InputTextBox.LagTestDelegate(this.OnLagTesting);
            this.sendbox.OnWriting += new InputTextBox.WritingDelegate(this.OnWriting);
            this.sendbox.OnWhisperRequested += new InputTextBox.OnPMSendingDelegate(this.OnWhisperRequested);
            this.sendbox.OnFindUserInList += new InputTextBox.OnPMSendingDelegate(this.OnFindUserInList);
            this.sendbox.OnPreColorChanged += new InputTextBox.OnPMSendingDelegate(this.OnPreColorChanged);
            this.sendbox.CloseTabsCmd += new EventHandler(this.CloseTabsCmd);
            this.Controls.Add(this.sendbox);

            this.topic = new Topic();
            this.topic.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            this.topic.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.topic.Location = new Point(7, 7);
            this.topic.Margin = new Padding(4);
            this.topic.Name = "topic";
            this.topic.Size = new Size(852, 20);
            this.topic.TabIndex = 5;
            this.Controls.Add(this.topic);

            this.userlist = new Userlist();
            this.userlist.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.userlist.BorderStyle = BorderStyle.FixedSingle;
            this.userlist.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.userlist.Location = new Point(0, 0);
            this.userlist.Name = "userlist";
            this.userlist.Size = new Size(266, 341);
            this.userlist.TabIndex = 4;
            this.userlist.UseCompatibleStateImageBehavior = false;
            this.userlist.View = View.Details;
            this.userlist.OnPMRequested += new Userlist.PMRequestDelegate(this.OnPMRequested);
            this.userlist.OnFileBrowseRequested += new Userlist.FileBrowseRequest(this.OnFileBrowseRequested);
            this.userlist.OnWhoisRequested += new Userlist.PMRequestDelegate(this.OnWhoisRequesting);
            this.userlist.OnIgnoreRequested += new Userlist.PMRequestDelegate(this.OnIgnoreRequesting);
            this.userlist.OnVCIgnoreRequested += new Userlist.PMRequestDelegate(this.userlist_OnVCIgnoreRequested);
            this.userlist.OnAdminCommandRequested += new Packets.SendPacketDelegate(this.OnPacketDispatching);
            this.userlist.OnNudgeRequested += new Userlist.PMRequestDelegate(this.OnNudgeRequesting);
            this.userlist.OnDCRequest += new Userlist.PMRequestDelegate(this.OnDCRequest);
            this.userlist.OnScribbleRequested += new Userlist.PMRequestDelegate(this.OnScribbleRequested);
            this.userlist.OnCopyNameRequesting += new Userlist.PMRequestDelegate(this.OnCopyNameRequesting);
            this.splitter.Panel2.Controls.Add(this.userlist);

            this.Controls.Add(this.splitter);
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel2.ResumeLayout(false);
            this.splitter.ResumeLayout(false);

            this.emotemenu = new EmoticonMenu(this.sendbox);
            this.colormenu = new ColorMenu(this.sendbox);
        }

        public event EventHandler CloseTabs;
        private void CloseTabsCmd(object sender, EventArgs e)
        {
            this.CloseTabs(sender, e);
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
            this.whois_panel.Dispose();
            this.bold.Dispose();
            this.italic.Dispose();
            this.underline.Dispose();
            this.fore.Dispose();
            this.back.Dispose();
            this.emoticons.Dispose();
            this.voiceclipbutton.Dispose();
            this.mutebutton.Dispose();
            this.urltag.Dispose();
            this.buttons.Dispose();
            this.sendbox.Dispose();
            this.topic.Dispose();
            this.userlist.Dispose();
            this.splitter.Dispose();
            this.emotemenu.Dispose();
            this.colormenu.Dispose();
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
                this.whois_panel.SetTicks(this.recording_tick, Settings.record_quality);
                this.RecordingStarts();
            }
            else
            {
                if (!this.is_recording)
                    return;

                this.whois_panel.SetTicks(-1, Settings.record_quality);
                this.is_recording = false;
                this.RecordingFinished();
            }
        }

        public void VCEscapeCancel()
        {
            if (this.is_recording)
            {
                this.whois_panel.SetTicks(-1, Settings.record_quality);
                this.is_recording = false;
                this.PauseVCNow(false);
                this.vrec.Cancel();
                this.chatscreen.VoiceClipCancel();
                this.recording_tick = -1;
            }
            else
            {
                this.PauseVCNow(true);
                this.PauseVCNow(false);
            }
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
                this.whois_panel.SetTicks(this.recording_tick, Settings.record_quality);
                this.RecordingStarts();
            }
        }

        private void voiceclipbutton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!this.is_recording)
                    return;

                this.whois_panel.SetTicks(-1, Settings.record_quality);
                this.is_recording = false;
                this.RecordingFinished();
            }
        }

        public void OnVCTimerTick()
        {
            if (this.is_recording)
            {
                this.whois_panel.SetTicks(++this.recording_tick, Settings.record_quality);

                if (this.recording_tick > (Settings.record_quality ? 15 : 25))
                {
                    this.whois_panel.SetTicks(-1, Settings.record_quality);
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

            if (this.UploadVoicePackets(this.vrec.GetAresPackets(false, null, this.recording_tick)))
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

        private void OnPreColorChanged(String text)
        {
            this.DisplayAnnounceText(text);
        }

        public void UpdateLatency(ulong lat)
        {
            this.whois_panel.SetLatency(lat);
        }

        private void OnFindUserInList(String text)
        {
            this.userlist.FindUserInList(text);
        }

        private Color url_color = Color.Blue;

        private void urltag_MouseLeave(object sender, EventArgs e)
        {
            if (!this.url_color.Equals(Color.Blue))
            {
                this.url_color = Color.Blue;
                this.urltag.Invalidate();
            }
        }

        private void urltag_MouseUp(object sender, MouseEventArgs e)
        {
            this.url_color = Color.Blue;
            this.urltag.Invalidate();
        }

        private void urltag_MouseDown(object sender, MouseEventArgs e)
        {
            this.url_color = Color.Red;
            this.urltag.Invalidate();
        }

        private void OnDrawUrlTag(object sender, PaintEventArgs e)
        {
            if (this.urltag.Text.Trim().Length == 0)
                return;

            String str = Helpers.FormatAresColorCodes(this.urltag.Text);
            str = Helpers.StripColors(str);
            char[] letters = str.ToCharArray();
            int x = 0;

            for (int i = 0; i < letters.Length; i++)
            {
                switch (letters[i])
                {
                    case ' ': // space
                        x += 2;
                        break;

                    case '+':
                    case '(':
                    case ':':
                    case ';': // emoticons
                        int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(str.Substring(i).ToUpper());

                        if (emote_index > -1)
                        {
                            if ((x + 15) > (e.ClipRectangle.Width - 1))
                            {
                                x += 15;
                                break;
                            }

                            e.Graphics.DrawImage(AresImages.TransparentEmoticons[emote_index], new RectangleF(x + 2, 0, 15, 15));
                            x += 15;
                            i += (EmoticonFinder.last_emote_length - 1);
                            break;
                        }
                        else goto default;

                    default: // text
                        int width = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), this.urltag.Font, 100, StringFormat.GenericTypographic).Width);

                        if ((x + width) > (e.ClipRectangle.Width - 1))
                        {
                            x += width;
                            break;
                        }

                        using (SolidBrush brush = new SolidBrush(Color.Blue))
                            e.Graphics.DrawString(letters[i].ToString(), this.urltag.Font, brush, new PointF(x, 4));

                        x += width;
                        break;
                }

                if (x > (e.ClipRectangle.Width - 1)) // run out of space - stop drawing!!
                    break;
            }

            using (SolidBrush brush = new SolidBrush(Color.Blue))
            using (Pen pen = new Pen(brush, 1))
                e.Graphics.DrawLine(pen, new Point(2, 16), new Point(x + 2, 16));
        }

        private void OnCAT()
        {
            this.OnCATReceived();
        }

        public void AddToInputBox(String text)
        {
            this.sendbox.Text += text;
            this.sendbox.SelectionStart = this.sendbox.Text.Length;
            this.sendbox.Focus();
        }

        public void UserListUpdateUserCount()
        {
            this.userlist.UserListUpdateUserCount();
        }

        private void OnMiddleClicking(String name)
        {
            this.OnMiddleClicked(name);
        }

        private void OnCopyNameRequesting(String name)
        {
            this.sendbox.Text += name;
            this.sendbox.SelectionStart = this.sendbox.Text.Length;
            this.sendbox.Focus();
        }

        private void OnWhisperRequested(String text)
        {
            UserObject userobj = this.userlist.GetCurrentSelection();

            if (userobj != null)
                this.OnPacketSending(Packets.CommandPacket("whisper \"" + userobj.name + "\" " + text));
            else
            {
                if (text.StartsWith("/me "))
                    this.OnPacketSending(Packets.EmotePacket(text.Substring(4)));
                else if (text.StartsWith("/"))
                    this.OnPacketSending(Packets.CommandPacket(text.Substring(1)));
                else
                    this.OnPacketSending(Packets.TextPacket(text));
            }
        }

        public void TimeoutCheck(int time)
        {
            this.sendbox.TimeoutCheck(time);
        }

        private void OnWriting(bool is_writing)
        {
            this.OnWritingProceed(is_writing);
        }

        public void FixScreen()
        {
            if (this.chatscreen != null)
                this.chatscreen.FixScreen();
        }

        private void OnScribbleRequested(String name)
        {
            Scribble s = new Scribble();
            s.Text = "Scribble to " + name;

            if (s.ShowDialog() == DialogResult.OK)
            {
                ScribbleObject obj = s.GetScribble();

                if (obj == null)
                    return;

                if (obj.data.Length == 0 || obj.image == null)
                    return;

                List<byte> b = new List<byte>(obj.data);

                if (b.Count <= 4000)
                {
                    this.OnPacketSending(Packets.ScribbleOncePacket(name, obj.data));
                }
                else
                {
                    List<byte[]> p = new List<byte[]>();

                    while (b.Count > 4000)
                    {
                        p.Add(b.GetRange(0, 4000).ToArray());
                        b.RemoveRange(0, 4000);
                    }

                    if (b.Count > 0)
                        p.Add(b.ToArray());

                    for (int i = 0; i < p.Count; i++)
                    {
                        if (i == 0)
                        {
                            this.OnPacketSending(Packets.ScribbleFirstPacket(name, p[i]));
                        }
                        else
                        {
                            if (i == (p.Count - 1))
                            {
                                this.OnPacketSending(Packets.ScribbleLastPacket(name, p[i]));
                            }
                            else
                            {
                                this.OnPacketSending(Packets.ScribbleChunkPacket(name, p[i]));
                            }
                        }
                    }
                }

                this.chatscreen.Scribble("Scribble to " + name + ": ", obj.image);
                this.sendbox.Focus();
            }
        }

        public void DisplayScribble(String name, byte[] data)
        {
            try
            {
                data = ZLib.Zlib.Decompress(data, true);
                this.chatscreen.Scribble("Scribble from " + name + ": ", data);
            }
            catch { }
        }

        public void RedrawTopic()
        {
            if (this.topic != null)
                this.topic.Invalidate();

            if (this.urltag != null)
                this.urltag.Invalidate();

            if (this.whois_panel != null)
                this.whois_panel.Invalidate();
        }

        public void WhoisWritingUpdate(String text)
        {
            this.whois_panel.SetText(text);
        }

        private void OnLagTesting()
        {
            this.OnBeginLagTest();
        }

        private void OnDCRequest(String name)
        {
            this.OnDCReq(name);
        }

        private void OnPacketToAll(byte[] packet)
        {
            this.OnSendToAll(packet);
        }

        private void OnNudgeRequesting(String name)
        {
            this.OnNudgeRequested(name);
        }

        private void OnIgnoreRequesting(String name)
        {
            this.OnIgnoreRequested(name);
        }

        private void userlist_OnVCIgnoreRequested(String name)
        {
            this.OnVCIgnoreRequested(name);
        }

        private void OnWhoisRequesting(String name)
        {
            this.OnWhoisRequested(name);
        }

        private void OnFileBrowseRequested(String name, byte type)
        {
            this.OnFileBrowseRequesting(name, type);
        }

        private void OnHashlinkClicking(ChannelObject cObj)
        {
            this.HashlinkClicked(cObj);
        }

        private void OnPMRequested(String name)
        {
            this.OnPMRequesting(name);
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

        private void OnPlaylistClicked(object sender, EventArgs e)
        {
            this.OnPlaylistRequesting();
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
        
        public void SetTopicFirst(String text)
        {
            this.topic.Text = text;
        }

        public void SetTopic(String text)
        {
            this.chatscreen.Server("Topic update: " + text);
            this.topic.Text = text;
        }

        public void DisplayServerText(String text)
        {
            this.chatscreen.Server(text);
        }

        public void DisplayAnnounceText(String text)
        {
            this.chatscreen.Announce(text);
        }

        public void DisplayPublicText(String name, String text)
        {
            this.chatscreen.Public(name, text);
        }

        public void DisplayEmoteText(String name, String text)
        {
            this.chatscreen.Emote(name, text);
        }

        public void DisplayPublicText(String name, String text, UserFont font, object custom_emotes)
        {
            this.chatscreen.Public(name, text, font, custom_emotes);
        }

        public void DisplayEmoteText(String name, String text, UserFont font, object custom_emotes)
        {
            this.chatscreen.Emote(name, text, font, custom_emotes);
        }

        public void DisplayJoinText(UserObject userobj)
        {
            this.chatscreen.Join(userobj);
        }

        public void DisplayPartText(UserObject userobj)
        {
            this.chatscreen.Part(userobj);
        }

        private delegate void UpdateUrlDelegate(String link, String tag);

        public void UpdateUrl(String link, String tag)
        {
            if (this.buttons.InvokeRequired)
            {
                this.buttons.BeginInvoke(new UpdateUrlDelegate(this.UpdateUrl), link, tag);
            }
            else
            {
                this.urltag.Text = "";
                this.urltag.Text = tag;
                this.urltag.ToolTipText = link;
            }
        }

        private void OnLinkClicked(object sender, EventArgs e)
        {
            String str = this.urltag.ToolTipText;

            if (str.StartsWith("arlnk://"))
            {
                ChannelObject _obj = Hashlink.DecodeHashlink(str);

                if (_obj != null)
                    this.OnHashlinkClicking(_obj);
            }
            else
            {
                try
                {
                    Process.Start(str);
                }
                catch { }
            }
        }

        public void UserListAdd(UserObject userobj)
        {
            this.userlist.AddUser(userobj);
        }

        public void UserListRemove(UserObject userobj)
        {
            this.userlist.RemoveUser(userobj);
        }

        public void UserListClear()
        {
            this.userlist.ClearList();
        }

        public void UserListUpdateMode(bool updating)
        {
            this.userlist.SetUpdateMode(updating);
        }

        public void UserListUpdateUser(UserObject userobj, bool avatar_updated, bool has_avatar, bool had_avatar)
        {
            this.userlist.UpdateUser(userobj, avatar_updated, has_avatar, had_avatar);
        }

        public void NudgeScreen(String name, Random rnd)
        {
            this.chatscreen.NudgeScreen(name, rnd);
        }

        public void DisableIPS()
        {
            this.chatscreen.show_ips = false;
        }

    }
}
