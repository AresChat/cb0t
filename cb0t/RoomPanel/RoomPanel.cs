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

namespace cb0t
{
    public partial class RoomPanel : UserControl
    {
        private Topic topic { get; set; }
        private Bitmap b1 { get; set; }
        private Bitmap b2 { get; set; }
        private Bitmap b3 { get; set; }
        private Bitmap b4 { get; set; }
        private ImageList tab_imgs { get; set; }

        public IPEndPoint EndPoint { get; set; }
        public ScreenMode Mode { get; set; }
        public String PMName { get; set; }
        public String MyName { get; set; }

        public event EventHandler CloseClicked;
        public event EventHandler CheckUnread;
        public event EventHandler CancelWriting;
        public event EventHandler SendAutoReply;

        public RoomPanel(FavouritesListItem creds)
        {
            this.InitializeComponent();
            this.Mode = ScreenMode.Main;
            this.PMName = String.Empty;
            this.MyName = String.Empty;
            this.EndPoint = new IPEndPoint(creds.IP, creds.Port);
            this.topic = new Topic();
            this.b1 = (Bitmap)Emoticons.emotic[47].Clone();
            this.toolStripButton5.Image = this.b1;
            this.b2 = (Bitmap)Emoticons.emotic[48].Clone();
            this.toolStripButton6.Image = this.b2;
            this.b3 = (Bitmap)Properties.Resources.button3.Clone();
            this.toolStripButton7.Image = this.b3;
            this.b4 = (Bitmap)Properties.Resources.button4.Clone();
            this.toolStripButton8.Image = this.b4;
            this.toolStrip1.Renderer = this.topic;
            this.toolStripButton1.Image = (Bitmap)Properties.Resources.close.Clone();
            this.toolStripDropDownButton1.Image = (Bitmap)Properties.Resources.settings.Clone();
            this.toolStripDropDownButton1.DropDownItems.Add("Export hashlink");
            this.toolStripDropDownButton1.DropDownItems.Add("Add to favourites");
            this.toolStripDropDownButton1.DropDownItems.Add("Copy room name");
            this.toolStripDropDownButton1.DropDownItems.Add("Auto play voice clips");
            this.toolStripDropDownButton1.DropDownItems.Add("Close sub tabs");
            this.toolStrip2.Renderer = new ButtonBar();
            this.tab_imgs = new ImageList();
            this.tab_imgs.ImageSize = new Size(16, 16);
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab1.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab_read.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab_unread.Clone());
            this.tabControl1.ImageList = this.tab_imgs;
            this.tabPage1.ImageIndex = 0;
        }

        public void MyPMText(String text, AresFont font)
        {
            if (this.tabControl1.SelectedTab != null)
                if (this.tabControl1.SelectedTab is PMTab)
                    ((PMTab)this.tabControl1.SelectedTab).PM(this.MyName, text, font);
        }

        public void MyPMAnnounce(String text)
        {
            if (this.tabControl1.SelectedTab != null)
                if (this.tabControl1.SelectedTab is PMTab)
                    ((PMTab)this.tabControl1.SelectedTab).Announce(text);
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

            PMTab new_tab = new PMTab(name);
            new_tab.ImageIndex = 1;
            this.tabControl1.TabPages.Add(new_tab);
            this.tabControl1.SelectedIndex = (this.tabControl1.TabPages.Count - 1);
        }

        public void PMTextReceived(String name, String text, AresFont font, PMTextReceivedType type)
        {
            this.tabControl1.BeginInvoke((Action)(() =>
            {
                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                    if (this.tabControl1.TabPages[i] is PMTab)
                        if (this.tabControl1.TabPages[i].Text == name)
                        {
                            PMTab tab = (PMTab)this.tabControl1.TabPages[i];

                            if (type == PMTextReceivedType.Announce)
                                tab.Announce(text);
                            else
                            {
                                tab.PM(name, text, font);
                                tab.SetRead(this.Mode == ScreenMode.PM && this.PMName == name);

                                if (!tab.AutoReplySent)
                                {
                                    this.SendAutoReply(name, EventArgs.Empty);
                                    //local copy of auto reply
                                    tab.AutoReplySent = true;
                                }
                            }

                            return;
                        }

                PMTab new_tab = new PMTab(name);
                new_tab.ImageIndex = 2;
                this.tabControl1.TabPages.Add(new_tab);

                if (type == PMTextReceivedType.Announce)
                    new_tab.Announce(text);
                else
                {
                    new_tab.PM(name, text, font);
                    this.SendAutoReply(name, EventArgs.Empty);
                    //local copy of auto reply
                    new_tab.AutoReplySent = true;
                }
            }));
        }

        public void ServerText(String text) { this.rtfScreen1.ShowServerText(text); }
        public void AnnounceText(String text) { this.rtfScreen1.ShowAnnounceText(text); }
        public void PublicText(String name, String text, AresFont font) { this.rtfScreen1.ShowPublicText(name, text, font); }
        public void EmoteText(String name, String text, AresFont font) { this.rtfScreen1.ShowEmoteText(name, text, font); }
        public void CheckUnreadStatus() { this.CheckUnread(this.EndPoint, EventArgs.Empty); }

        public void ScrollDown()
        {
            this.rtfScreen1.ScrollDown();
        }

        public UserListContainer Userlist { get { return this.userListContainer1; } }
        public TextBox SendBox { get { return this.textBox1; } }

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

        public void CanVC(bool can)
        {
            this.toolStrip2.BeginInvoke((Action)(() => this.toolStripButton8.Enabled = can));
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
            this.tabControl1.SelectedIndexChanged -= this.tabControl1_SelectedIndexChanged;

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
            this.toolStrip2.ItemClicked -= this.toolStrip2_ItemClicked;
            this.panel1.Paint -= this.panel1_Paint;

            while (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);

            this.panel1.Dispose();
            this.panel1 = null;
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
            this.toolStripLabel1.Dispose();
            this.toolStripLabel1 = null;
            this.textBox1.Font.Dispose();
            this.textBox1.Dispose();
            this.textBox1 = null;

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
            this.rtfScreen1.Free();
            this.rtfScreen1.Dispose();
            this.rtfScreen1 = null;
            this.writingPanel1.Free();
            this.writingPanel1.Dispose();
            this.writingPanel1 = null;
            this.Font.Dispose();
            this.Font = null;
        }

        private void CloseAllTabs(bool including_main)
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
                    pm.Free();
                    pm.Dispose();
                    pm = null;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.CloseClicked(this.EndPoint, EventArgs.Empty);
        }

        private void toolStripDropDownButton1_DropDownOpening(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, this.panel1.Width, 25);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.WhiteSmoke, Color.Gainsboro, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex > -1)
            {
                this.textBox1.BeginInvoke((Action)(() => this.textBox1.Focus()));

                if (this.tabControl1.SelectedTab is PMTab)
                {
                    ((PMTab)this.tabControl1.SelectedTab).SetRead(true);
                    this.PMName = this.tabControl1.SelectedTab.Text;
                    this.Mode = ScreenMode.PM;
                    this.CancelWriting(null, EventArgs.Empty);
                }
                else this.Mode = ScreenMode.Main;
            }
        }
    }
}
