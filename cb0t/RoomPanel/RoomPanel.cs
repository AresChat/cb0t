using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Drawing.Drawing2D;
using Jurassic.Library;

namespace cb0t
{
    public partial class RoomPanel : UserControl
    {
        private Topic topic { get; set; }
        private Bitmap b1 { get; set; }
        private Bitmap b2 { get; set; }
        private Bitmap b3 { get; set; }
        private Bitmap b4 { get; set; }
        private Bitmap b5 { get; set; }
        private ImageList tab_imgs { get; set; }

        public IPEndPoint EndPoint { get; set; }
        public ScreenMode Mode { get; set; }
        public String PMName { get; set; }
        public String MyName { get; set; }

        public event EventHandler CloseClicked;
        public event EventHandler CheckUnread;
        public event EventHandler CancelWriting;
        public event EventHandler SendAutoReply;
        public event EventHandler WantScribble;
        public event EventHandler<RoomMenuItemClickedEventArgs> RoomMenuItemClicked;
        public event EventHandler<RoomMenuJSItemClickedEventArgs> RoomMenuJSItemClicked;
        public event EventHandler HashlinkClicked;
        public event EventHandler EditScribbleClicked;

        private ContextMenuStrip rc_ctx { get; set; }

        public void ShowVoice(String sender, uint sc)
        {
            this.webControl1.ShowVoice(sender, sc);
        }

        public void UpdateFont()
        {
            this.accuTextBox1.Font.Dispose();
            this.accuTextBox1.Font = new Font(Settings.GetReg<String>("global_font", "Tahoma"), 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        }

        public void SetScreenWidth(bool wide)
        {
            this.webControl1.IsWideText = wide;
        }

        public void InjectHTMLFromScript(String str)
        {
            this.webControl1.ShowCustomHTML(str);
        }

        public void InjectJSFromScript(String str)
        {
            this.webControl1.ShowCustomScript(str);
        }

        public int ViewID
        {
            get
            {
                if (this.webControl1 != null)
                    return this.webControl1.ViewIdent;

                return -1;
            }
        }

        public void ProcessLinkClicked(String url)
        {
            this.webControl1.OnLinkClicked(new LinkClickedEventArgs(url));
        }

        public void ShowCustomHTML(String html)
        {
            this.webControl1.ShowCustomHTML(html);
        }

        public RoomPanel(FavouritesListItem creds)
        {
            this.InitializeComponent();
            this.toolStripDropDownButton1.DropDownDirection = ToolStripDropDownDirection.BelowLeft;
            this.Mode = ScreenMode.Main;
            this.accuTextBox1.Font = new Font(Settings.GetReg<String>("global_font", "Tahoma"), 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.PMName = String.Empty;
            this.MyName = String.Empty;
            this.EndPoint = new IPEndPoint(creds.IP, creds.Port);
            this.webControl1.EndPoint = this.EndPoint;
            this.topic = new Topic();
            this.b1 = (Bitmap)Emoticons.emotic[47].Clone();
            this.toolStripButton5.Image = this.b1;
            this.b2 = (Bitmap)Emoticons.emotic[48].Clone();
            this.toolStripButton6.Image = this.b2;
            this.b3 = (Bitmap)Properties.Resources.button3.Clone();
            this.toolStripButton7.Image = this.b3;
            this.b4 = (Bitmap)Properties.Resources.button4.Clone();
            this.toolStripButton8.Image = this.b4;
            this.b5 = (Bitmap)Properties.Resources.scribble.Clone();
            this.toolStripButton9.Image = this.b5;
            this.toolStrip1.Renderer = this.topic;
            this.toolStripButton1.Image = (Bitmap)Properties.Resources.close.Clone();
            this.toolStripDropDownButton1.Image = (Bitmap)Properties.Resources.settings.Clone();
            this.toolStripDropDownButton1.DropDownItems.Add("Export hashlink");//0
            this.toolStripDropDownButton1.DropDownItems.Add("Add to favourites");//1
            this.toolStripDropDownButton1.DropDownItems.Add("Copy room name to clipboard");//2
            this.toolStripDropDownButton1.DropDownItems.Add("Auto play voice clips");//3
            this.toolStripDropDownButton1.DropDownItems.Add("Close sub tabs");//4
            this.toolStripDropDownButton1.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripDropDownButton1.DropDownItems[5].Visible = false;
            this.toolStrip2.Renderer = new ButtonBar();
            this.tab_imgs = new ImageList();
            this.tab_imgs.ImageSize = new Size(16, 16);
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab1.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab_read.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab_unread.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.folder2.Clone());
            this.tabControl1.ImageList = this.tab_imgs;
            this.tabPage1.ImageIndex = 0;
            this.webControl1.HashlinkClicked += this.LinkHashlinkClicked;
            this.webControl1.EditScribbleClicked += this.DoEditScribbleClicked;
            this.webControl1.NameClicked += this.ScreenNameClicked;
            this.webControl1.IsMainScreen = true;
            this.rc_ctx = new ContextMenuStrip();
            this.rc_ctx.Items.Add("Close");
            this.rc_ctx.ItemClicked += this.CTXItemClicked;
            this.splitContainer1.SplitterMoved += this.SplitterMoved;
            this.SetUserlistWidth();
        }

        public void CreateScreen()
        {
            this.webControl1.CreateScreen();
        }

        private void DoEditScribbleClicked(object sender, EventArgs e)
        {
            this.EditScribbleClicked(sender, e);
        }

        private bool can_resize = true;

        public void SetUserlistWidth()
        {
            int ul_split = Settings.GetReg<int>("userlist_width", -1);

            if (ul_split > 0 && (this.splitContainer1.ClientSize.Width - ul_split) > 0)
            {
                this.can_resize = false;
                this.splitContainer1.SplitterDistance = (this.splitContainer1.ClientSize.Width - ul_split);
                this.can_resize = true;
            }
        }

        private void SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.can_resize)
            {
                int ul_split = Settings.GetReg<int>("userlist_width", -1);

                if (this.splitContainer1.Panel2.Width != ul_split)
                {
                    Settings.SetReg("userlist_width", this.splitContainer1.Panel2.Width + 4);
                    
                    RoomPool.Rooms.ForEach((x) =>
                    {
                        if (!x.EndPoint.Equals(this.EndPoint))
                            x.Panel.SetUserlistWidth();
                    });
                }
            }
        }

        private void CTXItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.toolStripButton1_Click(null, EventArgs.Empty);
        }

        public void UpdateTemplate()
        {
            this.accuTextBox1.UpdateTemplate();
            this.webControl1.UpdateTemplate();
            this.toolStripButton2.ToolTipText = StringTemplate.Get(STType.ButtonBar, 0);
            this.toolStripButton3.ToolTipText = StringTemplate.Get(STType.ButtonBar, 1);
            this.toolStripButton4.ToolTipText = StringTemplate.Get(STType.ButtonBar, 2);
            this.toolStripButton5.ToolTipText = StringTemplate.Get(STType.ButtonBar, 3);
            this.toolStripButton6.ToolTipText = StringTemplate.Get(STType.ButtonBar, 4);
            this.toolStripButton7.ToolTipText = StringTemplate.Get(STType.ButtonBar, 5);
            this.toolStripButton8.ToolTipText = StringTemplate.Get(STType.ButtonBar, 6) + "\r\n" + StringTemplate.Get(STType.ButtonBar, 7);
            this.toolStripButton9.ToolTipText = StringTemplate.Get(STType.ButtonBar, 8);
            this.toolStripDropDownButton1.ToolTipText = StringTemplate.Get(STType.RoomMenu, 0);
            this.toolStripButton1.ToolTipText = StringTemplate.Get(STType.RoomMenu, 1);
            this.toolStripDropDownButton1.DropDownItems[0].Text = StringTemplate.Get(STType.RoomMenu, 2);
            this.toolStripDropDownButton1.DropDownItems[1].Text = StringTemplate.Get(STType.RoomMenu, 3);
            this.toolStripDropDownButton1.DropDownItems[2].Text = StringTemplate.Get(STType.RoomMenu, 4);
            this.toolStripDropDownButton1.DropDownItems[3].Text = StringTemplate.Get(STType.RoomMenu, 5);
            this.toolStripDropDownButton1.DropDownItems[4].Text = StringTemplate.Get(STType.RoomMenu, 6);
            this.rc_ctx.Items[0].Text = StringTemplate.Get(STType.RoomMenu, 1);

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is PMTab)
                    ((PMTab)this.tabControl1.TabPages[i]).UpdateTemplate();
                else if (this.tabControl1.TabPages[i] is BrowseTab)
                    ((BrowseTab)this.tabControl1.TabPages[i]).UpdateTemplate();
        }

        private bool IsBlack { get; set; }

        public void SetToBlack()
        {
            this.writingPanel1.IsBlack = true;
            this.writingPanel1.Invalidate();
            this.webControl1.IsBlack = true;
            this.webControl1.BackColor = Color.Black;
        }

        public void UpdateVoiceTime(int seconds)
        {
            if (this.tabControl1.SelectedTab is PMTab)
            {
                PMTab tab = (PMTab)this.tabControl1.SelectedTab;
                tab.UpdateVoiceTime(seconds);
            }
            else if (!(this.tabControl1.SelectedTab is BrowseTab))
            {
                if (seconds == -1)
                {
                    this.writingPanel1.Mode = WritingPanelMode.Writing;
                    this.writingPanel1.RecordingTime = 0;
                }
                else
                {
                    this.writingPanel1.Mode = WritingPanelMode.Recording;
                    this.writingPanel1.RecordingTime = seconds;
                }

                this.writingPanel1.Invalidate();
            }            
        }

        public void SpellCheck()
        {
            this.accuTextBox1.SpellCheck();
        }

        private void ScreenNameClicked(object sender, EventArgs e)
        {
            String name = (String)sender;
            this.accuTextBox1.AppendText(name);
            this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
            this.accuTextBox1.BeginInvoke((Action)(() => this.accuTextBox1.Focus()));
        }

        private void LinkHashlinkClicked(object sender, EventArgs e)
        {
            this.HashlinkClicked(sender, e);
        }

        public void MyPMText(String text, AresFont font)
        {
            this.tabControl1.BeginInvoke((Action)(() =>
            {
                if (this.tabControl1.SelectedTab != null)
                    if (this.tabControl1.SelectedTab is PMTab)
                        ((PMTab)this.tabControl1.SelectedTab).PM(this.MyName, text, font);
            }));
        }

        private delegate void MyPMJSTextHandler(String target, String text, AresFont font);
        public void MyPMJSText(String target, String text, AresFont font)
        {
            if (this.tabControl1.InvokeRequired)
                this.tabControl1.BeginInvoke(new MyPMJSTextHandler(this.MyPMJSText), target, text, font);
            else
            {
                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                    if (this.tabControl1.TabPages[i] is PMTab)
                        if (this.tabControl1.TabPages[i].Text == target)
                        {
                            PMTab tab = (PMTab)this.tabControl1.TabPages[i];
                            tab.PM(this.MyName, text, font);
                            tab.SetRead(this.Mode == ScreenMode.PM && this.PMName == target);
                            return;
                        }

                PMTab new_tab = new PMTab(target, this.EndPoint);

                if (this.IsBlack)
                    new_tab.SetToBlack();

                new_tab.HashlinkClicked += this.LinkHashlinkClicked;
                new_tab.EditScribbleClicked += this.DoEditScribbleClicked;
                new_tab.ImageIndex = 2;
                this.tabControl1.TabPages.Add(new_tab);
                new_tab.PM(this.MyName, text, font);
            }
        }

        public void MyPMAnnounce(String text)
        {
            this.tabControl1.BeginInvoke((Action)(() =>
            {
                if (this.tabControl1.SelectedTab != null)
                    if (this.tabControl1.SelectedTab is PMTab)
                        ((PMTab)this.tabControl1.SelectedTab).Announce(text);
            }));
        }

        public void MyPMCreateOrShowTab(String name)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is PMTab)
                    if (this.tabControl1.TabPages[i].Text == name)
                    {
                        this.tabControl1.SelectedIndex = i;
                        return;
                    }

            PMTab new_tab = new PMTab(name, this.EndPoint);

            if (this.IsBlack)
                new_tab.SetToBlack();

            new_tab.HashlinkClicked += this.LinkHashlinkClicked;
            new_tab.EditScribbleClicked += this.DoEditScribbleClicked;
            new_tab.ImageIndex = 1;
            this.tabControl1.TabPages.Add(new_tab);
            this.tabControl1.SelectedIndex = (this.tabControl1.TabPages.Count - 1);
        }

        internal void PMScribbleReceived(Room room, User user, String name, byte[] data)
        {
            this.tabControl1.BeginInvoke((Action)(() =>
            {
                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                    if (this.tabControl1.TabPages[i] is PMTab)
                        if (this.tabControl1.TabPages[i].Text == name)
                        {
                            PMTab tab = (PMTab)this.tabControl1.TabPages[i];
                            tab.Scribble(data);
                            tab.SetRead(this.Mode == ScreenMode.PM && this.PMName == name);

                            if (!tab.First)
                            {
                                tab.First = true;

                                if (room != null && user != null)
                                    ScriptEvents.OnPmFirstReceived(room, user);
                            }

                            return;
                        }

                PMTab new_tab = new PMTab(name, this.EndPoint);

                if (this.IsBlack)
                    new_tab.SetToBlack();

                new_tab.HashlinkClicked += this.LinkHashlinkClicked;
                new_tab.EditScribbleClicked += this.DoEditScribbleClicked;
                new_tab.ImageIndex = 2;
                this.tabControl1.TabPages.Add(new_tab);
                new_tab.Scribble(data);
                new_tab.First = true;

                if (room != null && user != null)
                    ScriptEvents.OnPmFirstReceived(room, user);
            }));
        }

        internal void PMTextReceived(Room room, User user, String name, String text, AresFont font, PMTextReceivedType type)
        {
            this.tabControl1.BeginInvoke((Action)(() =>
            {
                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                    if (this.tabControl1.TabPages[i] is PMTab)
                        if (this.tabControl1.TabPages[i].Text == name)
                        {
                            PMTab tab = (PMTab)this.tabControl1.TabPages[i];

                            if (type == PMTextReceivedType.Announce)
                            {
                                tab.Announce(text);

                                if (text.Contains("voice_clip"))
                                    tab.SetRead(this.Mode == ScreenMode.PM && this.PMName == name);
                            }
                            else
                            {
                                tab.PM(name, text, font);
                                tab.SetRead(this.Mode == ScreenMode.PM && this.PMName == name);

                                if (!tab.AutoReplySent)
                                {
                                    if (Settings.GetReg<bool>("can_pm_reply", true))
                                    {
                                        this.SendAutoReply(name, EventArgs.Empty);
                                        String[] lines = Settings.GetReg<String>("pm_reply", "Hello +n, please leave a message.").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                        AresFont f = null;

                                        if (Settings.MyFont != null)
                                            f = Settings.MyFont.Copy();

                                        foreach (String str in lines)
                                        {
                                            String rtext = str.Replace("+n", name);

                                            if (!String.IsNullOrEmpty(rtext))
                                            {
                                                while (Encoding.UTF8.GetByteCount(rtext) > 200)
                                                    rtext = rtext.Substring(0, rtext.Length - 1);

                                                tab.PM(this.MyName, rtext, f);
                                            }
                                        }
                                    }

                                    tab.AutoReplySent = true;
                                }
                            }

                            if (!tab.First)
                            {
                                tab.First = true;

                                if (room != null && user != null)
                                    ScriptEvents.OnPmFirstReceived(room, user);
                            }

                            return;
                        }

                PMTab new_tab = new PMTab(name, this.EndPoint);

                if (this.IsBlack)
                    new_tab.SetToBlack();

                new_tab.HashlinkClicked += this.LinkHashlinkClicked;
                new_tab.EditScribbleClicked += this.DoEditScribbleClicked;
                new_tab.ImageIndex = 2;
                this.tabControl1.TabPages.Add(new_tab);

                if (type == PMTextReceivedType.Announce)
                    new_tab.Announce(text);
                else
                {
                    new_tab.PM(name, text, font);

                    if (Settings.GetReg<bool>("can_pm_reply", true))
                    {
                        this.SendAutoReply(name, EventArgs.Empty);
                        String[] lines = Settings.GetReg<String>("pm_reply", "Hello +n, please leave a message.").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        AresFont f = null;

                        if (Settings.MyFont != null)
                            f = Settings.MyFont.Copy();

                        foreach (String str in lines)
                        {
                            String rtext = str.Replace("+n", name);

                            if (!String.IsNullOrEmpty(text))
                            {
                                while (Encoding.UTF8.GetByteCount(rtext) > 200)
                                    rtext = rtext.Substring(0, rtext.Length - 1);

                                new_tab.PM(this.MyName, rtext, f);
                            }
                        }
                    }

                    new_tab.AutoReplySent = true;
                    new_tab.First = true;

                    if (room != null && user != null)
                        ScriptEvents.OnPmFirstReceived(room, user);
                }
            }));
        }

        public bool CreateFileBrowseTab(String name, ushort ident)
        {
            int index = -1;

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is BrowseTab)
                    if (this.tabControl1.TabPages[i].Text == name)
                    {
                        index = i;
                        break;
                    }

            if (index > -1)
            {
                this.tabControl1.SelectedIndex = index;
                return false;
            }
            else
            {
                BrowseTab new_tab = new BrowseTab(name);
                new_tab.ImageIndex = 3;
                new_tab.BrowseIdent = ident;
                this.tabControl1.TabPages.Add(new_tab);
                this.tabControl1.SelectedIndex = (this.tabControl1.TabPages.Count - 1);
                return true;
            }
        }

        public void StartBrowse(ushort ident, ushort count)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is BrowseTab)
                {
                    BrowseTab tab = (BrowseTab)this.tabControl1.TabPages[i];

                    if (tab.BrowseIdent == ident)
                    {
                        tab.StartReceived(count);
                        break;
                    }
                }
        }

        public void BrowseItemReceived(ushort ident, BrowseItem item)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is BrowseTab)
                {
                    BrowseTab tab = (BrowseTab)this.tabControl1.TabPages[i];

                    if (tab.BrowseIdent == ident)
                    {
                        tab.ItemReceived(item);
                        break;
                    }
                }
        }

        public void BrowseEnd(ushort ident)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is BrowseTab)
                {
                    BrowseTab tab = (BrowseTab)this.tabControl1.TabPages[i];

                    if (tab.BrowseIdent == ident)
                    {
                        tab.EndReceived();
                        break;
                    }
                }
        }

        public void BrowseError(ushort ident)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                if (this.tabControl1.TabPages[i] is BrowseTab)
                {
                    BrowseTab tab = (BrowseTab)this.tabControl1.TabPages[i];

                    if (tab.BrowseIdent == ident)
                    {
                        tab.ErrorReceived();
                        break;
                    }
                }
        }

        public void ServerText(String text) { this.webControl1.ShowServerText(text); }
        public void AnnounceText(String text) { this.webControl1.ShowAnnounceText(text); }
        public void PublicText(String name, String text, AresFont font) { this.webControl1.ShowPublicText(name, text, font); }
        public void EmoteText(String name, String text, AresFont font) { this.webControl1.ShowEmoteText(name, text, font); }
        public void CheckUnreadStatus() { this.CheckUnread(this.EndPoint, EventArgs.Empty); }
        public void Scribble(byte[] data) { this.webControl1.Scribble(data); }

        public void ScrollDown()
        {
            this.webControl1.ScrollDown();
        }

        public UserListContainer Userlist { get { return this.userListContainer1; } }
        internal AccuTextBox SendBox { get { return this.accuTextBox1; } }

        public void ClearWriters()
        {
            this.writingPanel1.BeginInvoke((Action)(() => this.writingPanel1.ClearWriters()));
        }

        public void UpdateWriter(User user)
        {
            this.writingPanel1.BeginInvoke((Action)(() => this.writingPanel1.RemoteWritingStatusChanged(user.Name, user.Writing)));
        }

        public void UpdateMyWriting(String name, bool status)
        {
            this.writingPanel1.BeginInvoke((Action)(() => this.writingPanel1.MyWritingStatusChanged(name, status)));
        }

        private bool VCAll { get; set; }

        public void CanVC(bool can)
        {
            this.toolStrip2.BeginInvoke((Action)(() => this.toolStripButton8.Enabled = can));
            this.VCAll = can;
        }

        private bool ScribbleAllAvailable { get; set; }
        private bool ScribblePMAvailable { get; set; }

        public void CanScribbleAll(bool can)
        {
            this.ScribbleAllAvailable = can;
        }

        public void CanScribblePM(bool can)
        {
            this.ScribblePMAvailable = can;
        }

        private delegate void InitScribbleDelegate();
        public void InitScribbleButton()
        {
            if (!this.toolStrip2.IsHandleCreated)
                return;

            if (this.toolStrip2.InvokeRequired)
                this.toolStrip2.BeginInvoke(new InitScribbleDelegate(this.InitScribbleButton));
            else
            {
                if (this.Mode == ScreenMode.Main)
                    this.toolStripButton9.Enabled = this.ScribbleAllAvailable;
                else if (this.Mode == ScreenMode.PM)
                    this.toolStripButton9.Enabled = this.ScribblePMAvailable;
            }
        }

        private String url_tag = String.Empty;

        public void SetURL(String text, String addr)
        {
            this.toolStrip2.BeginInvoke((Action)(() =>
            {
                this.toolStripLabel1.Text = text;
                this.toolStripLabel1.ToolTipText = addr;
                this.url_tag = addr;
            }));
        }

        public void SetTopic(String text)
        {
            this.toolStrip1.BeginInvoke((Action)(() =>
            {
                this.topic.TopicText = text;
                this.toolStrip1.Invalidate();
            }));
        }

        public void Free()
        {
            this.splitContainer1.SplitterMoved -= this.SplitterMoved;
            this.tabControl1.SelectedIndexChanged -= this.tabControl1_SelectedIndexChanged;
            this.tabControl1.MouseClick -= this.tabControl1_MouseClick;

            while (this.Controls.Count > 0)
                this.Controls.RemoveAt(0);

            this.toolStrip1.Items.Clear();
            this.toolStrip1.Renderer = null;
            this.topic.Free();
            this.topic = null;
            this.toolStrip1.Dispose();
            this.toolStrip1 = null;
            this.toolStripButton1.Click -= this.toolStripButton1_Click;
            this.toolStripButton1.Dispose();
            this.toolStripButton1 = null;
            this.toolStripDropDownButton1.DropDownOpening -= this.toolStripDropDownButton1_DropDownOpening;
            this.toolStripDropDownButton1.DropDownItemClicked -= this.toolStripDropDownButton1_DropDownItemClicked;

            while (this.toolStripDropDownButton1.DropDownItems.Count > 0)
                this.toolStripDropDownButton1.DropDownItems[0].Dispose();

            this.toolStripDropDownButton1.Dispose();
            this.toolStripDropDownButton1 = null;
            this.toolStripButton8.MouseDown -= this.toolStripButton8_MouseDown;
            this.toolStripButton8.MouseUp -= this.toolStripButton8_MouseUp;
            this.toolStripButton8.MouseLeave -= this.toolStripButton8_MouseLeave;
            this.toolStrip2.ItemClicked -= this.toolStrip2_ItemClicked;
            this.panel1.Paint -= this.panel1_Paint;

            while (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);

            this.panel1.Dispose();
            this.panel1 = null;

            while (this.panel3.Controls.Count > 0)
                this.panel3.Controls.RemoveAt(0);

            this.panel3.Dispose();
            this.panel3 = null;
            this.toolStrip2.Items.Clear();
            this.toolStrip2.Renderer = null;
            this.toolStrip2.Dispose();
            this.toolStrip2 = null;
            this.toolStripButton2.Font.Dispose();
            this.toolStripButton2.Dispose();
            this.toolStripButton2 = null;
            this.toolStripButton3.Font.Dispose();
            this.toolStripButton3.Dispose();
            this.toolStripButton3 = null;
            this.toolStripButton4.Font.Dispose();
            this.toolStripButton4.Dispose();
            this.toolStripButton4 = null;
            this.toolStripButton5.Image = null;
            this.toolStripButton5.Dispose();
            this.toolStripButton5 = null;
            this.b1.Dispose();
            this.b1 = null;
            this.toolStripButton6.Image = null;
            this.toolStripButton6.Dispose();
            this.toolStripButton6 = null;
            this.b2.Dispose();
            this.b2 = null;
            this.toolStripButton7.Image = null;
            this.toolStripButton7.Dispose();
            this.toolStripButton7 = null;
            this.b3.Dispose();
            this.b3 = null;
            this.toolStripButton8.Image = null;
            this.toolStripButton8.Dispose();
            this.toolStripButton8 = null;
            this.b4.Dispose();
            this.b4 = null;
            this.toolStripButton9.Image = null;
            this.toolStripButton9.Dispose();
            this.toolStripButton9 = null;
            this.b5.Dispose();
            this.b5 = null;
            this.toolStripLabel1.Dispose();
            this.toolStripLabel1 = null;
            this.accuTextBox1.Free();
            this.accuTextBox1.Font.Dispose();
            this.accuTextBox1.Dispose();
            this.accuTextBox1 = null;

            this.CloseAllTabs(true);

            this.tabControl1.ImageList = null;
            this.tabControl1.Dispose();
            this.tabControl1 = null;

            while (this.tab_imgs.Images.Count > 0)
            {
                Image bmp = this.tab_imgs.Images[0];
                this.tab_imgs.Images.RemoveAt(0);
                bmp.Dispose();
                bmp = null;
            }

            this.tab_imgs.Dispose();
            this.tab_imgs = null;

            while (this.panel2.Controls.Count > 0)
                this.panel2.Controls.RemoveAt(0);

            this.panel2.Dispose();
            this.panel2 = null;
            this.webControl1.HashlinkClicked -= this.LinkHashlinkClicked;
            this.webControl1.EditScribbleClicked -= this.DoEditScribbleClicked;
            this.webControl1.Free();
            this.webControl1.Dispose();
            this.webControl1 = null;
            this.writingPanel1.Free();
            this.writingPanel1.Dispose();
            this.writingPanel1 = null;
            this.Font.Dispose();
            this.Font = null;
            this.rc_ctx.ItemClicked += this.CTXItemClicked;

            while (this.rc_ctx.Items.Count > 0)
                this.rc_ctx.Items[0].Dispose();

            this.rc_ctx.Dispose();
            this.rc_ctx = null;
        }

        public void CloseAllTabs(bool including_main)
        {
            if (including_main)
            {
                this.tabControl1.TabPages.RemoveAt(0);

                while (this.splitContainer1.Panel1.Controls.Count > 0)
                    this.splitContainer1.Panel1.Controls.RemoveAt(0);

                while (this.splitContainer1.Panel2.Controls.Count > 0)
                    this.splitContainer1.Panel2.Controls.RemoveAt(0);

                this.splitContainer1.Dispose();
                this.splitContainer1 = null;

                while (this.tabPage1.Controls.Count > 0)
                    this.tabPage1.Controls.RemoveAt(0);

                this.tabPage1.Dispose();
                this.tabPage1 = null;
                this.userListContainer1.Free();
                this.userListContainer1.Dispose();
                this.userListContainer1 = null;
            }

            // pm and file tabs
            for (int i = (this.tabControl1.TabPages.Count - 1); i > -1; i--)
            {
                if (this.tabControl1.TabPages[i] is PMTab)
                {
                    PMTab pm = (PMTab)this.tabControl1.TabPages[i];
                    this.tabControl1.TabPages.RemoveAt(i);
                    pm.HashlinkClicked -= this.LinkHashlinkClicked;
                    pm.EditScribbleClicked -= this.DoEditScribbleClicked;
                    pm.Free();
                    pm.Dispose();
                    pm = null;
                }
                else if (this.tabControl1.TabPages[i] is BrowseTab)
                {
                    BrowseTab bt = (BrowseTab)this.tabControl1.TabPages[i];
                    this.tabControl1.TabPages.RemoveAt(i);
                    bt.Free();
                    bt.Dispose();
                    bt = null;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.Mode == ScreenMode.Main)
                this.CloseClicked(this.EndPoint, EventArgs.Empty);
            else
            {
                int index = this.tabControl1.SelectedIndex;

                if (index > 0)
                {
                    this.tabControl1.SelectedIndex = 0;

                    if (this.tabControl1.TabPages[index] is PMTab)
                    {
                        PMTab pm = (PMTab)this.tabControl1.TabPages[index];
                        this.tabControl1.TabPages.RemoveAt(index);
                        pm.HashlinkClicked -= this.LinkHashlinkClicked;
                        pm.EditScribbleClicked -= this.DoEditScribbleClicked;
                        pm.Free();
                        pm.Dispose();
                        pm = null;
                    }
                    else if (this.tabControl1.TabPages[index] is BrowseTab)
                    {
                        BrowseTab bt = (BrowseTab)this.tabControl1.TabPages[index];
                        this.tabControl1.TabPages.RemoveAt(index);
                        bt.Free();
                        bt.Dispose();
                        bt = null;
                    }
                }
            }
        }

        private void toolStripDropDownButton1_DropDownOpening(object sender, EventArgs e)
        {
            while (this.toolStripDropDownButton1.DropDownItems.Count > 6)
                this.toolStripDropDownButton1.DropDownItems.RemoveAt(6);

            bool custom_options = false;

            if (Menus.Room.Count > 0)
            {
                this.toolStripDropDownButton1.DropDownItems[5].Visible = true;
                Menus.Room.ForEach(x => this.toolStripDropDownButton1.DropDownItems.Add(x.Name));
                custom_options = true;
            }

            if (Scripting.ScriptManager.RoomMenuOptions.Count > 0)
            {
                if (!this.toolStripDropDownButton1.DropDownItems[5].Visible)
                    this.toolStripDropDownButton1.DropDownItems[5].Visible = true;

                Scripting.ScriptManager.RoomMenuOptions.ForEach(x =>
                {
                    this.toolStripDropDownButton1.DropDownItems.Add(x.Text);

                    if (x.CanCheck)
                        ((ToolStripMenuItem)this.toolStripDropDownButton1.DropDownItems[this.toolStripDropDownButton1.DropDownItems.Count - 1]).Checked = x.IsChecked;
                });

                custom_options = true;
            }

            if (!custom_options)
                this.toolStripDropDownButton1.DropDownItems[5].Visible = false;
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[0]))
                this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.ExportHashlink });
            else if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[1]))
                this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.AddToFavourites });
            else if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[2]))
                this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.CopyRoomName });
            else if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[3]))
            {
                ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
                item.Checked = !item.Checked;
                this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.AutoPlayVoiceClips, Arg = item.Checked });
            }
            else if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[4]))
                this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.CloseSubTabs });
            else
            {
                for (int i = 6; i < this.toolStripDropDownButton1.DropDownItems.Count; i++)
                    if (e.ClickedItem.Equals(this.toolStripDropDownButton1.DropDownItems[i]))
                    {
                        int index = (i - 6);

                        if (index >= 0 && index < Menus.Room.Count)
                        {
                            String ctext = Menus.Room[index].Text;
                            this.RoomMenuItemClicked(null, new RoomMenuItemClickedEventArgs { Item = RoomMenuItem.Custom, Arg = ctext });
                        }
                        else
                        {
                            index -= Menus.Room.Count;

                            if (index >= 0 && index < Scripting.ScriptManager.RoomMenuOptions.Count)
                            {
                                Scripting.CustomJSMenuOption option = Scripting.ScriptManager.RoomMenuOptions[index];

                                if (option.CanCheck)
                                    option.IsChecked = !option.IsChecked;

                                UserDefinedFunction cback = option.Callback;
                                Scripting.CustomJSRoomMenuCallback js = new Scripting.CustomJSRoomMenuCallback { Callback = cback };

                                if (option.CanCheck)
                                    js.IsChecked = option.IsChecked ? Scripting.JSCheckState.Checked : Scripting.JSCheckState.Unchecked;
                                else
                                    js.IsChecked = Scripting.JSCheckState.Unused;

                                this.RoomMenuJSItemClicked(null, new RoomMenuJSItemClickedEventArgs { Item = js });
                            }
                        }

                        break;
                    }
            }
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(this.toolStripButton2))
            {
                this.accuTextBox1.AppendText("\x00026");
                this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
            }
            else if (e.ClickedItem.Equals(this.toolStripButton3))
            {
                this.accuTextBox1.AppendText("\x00029");
                this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
            }
            else if (e.ClickedItem.Equals(this.toolStripButton4))
            {
                this.accuTextBox1.AppendText("\x00027");
                this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
            }
            else if (e.ClickedItem.Equals(this.toolStripButton5))
            {
                SharedUI.CMenu.StartPosition = FormStartPosition.Manual;
                SharedUI.CMenu.Location = new Point(MousePosition.X - 40, MousePosition.Y - 164);
                SharedUI.CMenu.SetCallback(this, false);
                SharedUI.CMenu.Show();
            }
            else if (e.ClickedItem.Equals(this.toolStripButton6))
            {
                SharedUI.CMenu.StartPosition = FormStartPosition.Manual;
                SharedUI.CMenu.Location = new Point(MousePosition.X - 40, MousePosition.Y - 164);
                SharedUI.CMenu.SetCallback(this, true);
                SharedUI.CMenu.Show();
            }
            else if (e.ClickedItem.Equals(this.toolStripButton7))
            {
                SharedUI.EMenu.StartPosition = FormStartPosition.Manual;
                SharedUI.EMenu.Location = new Point(MousePosition.X - 40, MousePosition.Y - 276);
                SharedUI.EMenu.SetCallback(this);
                SharedUI.EMenu.Show();
            }
            else if (e.ClickedItem.Equals(this.toolStripButton9))
                this.WantScribble(null, EventArgs.Empty);
            else if (e.ClickedItem.Equals(this.toolStripLabel1))
            {
                if (this.url_tag.StartsWith("\\\\arlnk://"))
                {
                    DecryptedHashlink hashlink = Hashlink.DecodeHashlink(this.url_tag.Substring(10));

                    if (hashlink != null)
                        this.HashlinkClicked(hashlink, EventArgs.Empty);
                }
                else if (this.url_tag.StartsWith("arlnk://"))
                {
                    DecryptedHashlink hashlink = Hashlink.DecodeHashlink(this.url_tag.Substring(8));

                    if (hashlink != null)
                        this.HashlinkClicked(hashlink, EventArgs.Empty);
                }
                else
                {
                    String check = this.url_tag.ToUpper();

                    if (check.StartsWith("HTTP://") || check.StartsWith("HTTPS://") || check.StartsWith("WWW."))
                    {
                        Scripting.JSOutboundTextItem cb = new Scripting.JSOutboundTextItem();
                        cb.Type = Scripting.JSOutboundTextItemType.Link;
                        cb.Text = this.url_tag;
                        cb.EndPoint = this.EndPoint;
                        Scripting.ScriptManager.PendingUIText.Enqueue(cb);
                    }
                }
            }
        }

        public void ColorCallback(String sc)
        {
            this.accuTextBox1.AppendText(sc);
            this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
        }

        public void EmoticonCallback(String sc)
        {
            this.accuTextBox1.AppendText(sc);
            this.accuTextBox1.SelectionStart = this.accuTextBox1.Text.Length;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, this.panel1.Width, 25);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.WhiteSmoke, Color.Gainsboro, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);
        }

        public delegate User GetUserEventHandler(String name);
        public event GetUserEventHandler GetUserByName;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex > -1)
            {
                if (this.tabControl1.SelectedTab is BrowseTab)
                {
                    this.Mode = ScreenMode.Browse;

                    if (this.panel1.Visible)
                        this.panel1.Visible = false;
                }
                else
                {
                    if (!this.panel1.Visible)
                        this.panel1.Visible = true;

                    this.accuTextBox1.BeginInvoke((Action)(() => this.accuTextBox1.Focus()));

                    if (this.tabControl1.SelectedTab is PMTab)
                    {
                        ((PMTab)this.tabControl1.SelectedTab).SetRead(true);
                        this.PMName = this.tabControl1.SelectedTab.Text;
                        this.Mode = ScreenMode.PM;
                        this.CancelWriting(null, EventArgs.Empty);
                        User user = this.GetUserByName(this.PMName);

                        if (user == null)
                            this.toolStripButton8.Enabled = false;
                        else
                            this.toolStripButton8.Enabled = user.SupportsVC;

                        this.toolStripButton9.Enabled = true;
                    }
                    else
                    {
                        this.Mode = ScreenMode.Main;
                        this.toolStripButton8.Enabled = this.VCAll;
                        this.toolStripButton9.Enabled = this.ScribbleAllAvailable;
                    }
                }
            }
        }

        public event EventHandler VoiceRecordingButtonClicked;

        private void toolStripButton8_MouseDown(object sender, MouseEventArgs e)
        {
            this.VoiceRecordingButtonClicked(true, EventArgs.Empty);
        }

        private void toolStripButton8_MouseUp(object sender, MouseEventArgs e)
        {
            this.VoiceRecordingButtonClicked(false, EventArgs.Empty);
        }

        private void toolStripButton8_MouseLeave(object sender, EventArgs e)
        {
            this.VoiceRecordingButtonClicked(false, EventArgs.Empty);
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.rc_ctx.Show(this.tabControl1, e.Location);
        }
    }
}
