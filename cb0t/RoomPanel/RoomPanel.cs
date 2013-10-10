using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

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
        public event EventHandler CloseClicked;
        public event EventHandler CheckUnread;

        public RoomPanel(IPEndPoint ep)
        {
            this.InitializeComponent();
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
            this.EndPoint = ep;
            this.tab_imgs = new ImageList();
            this.tab_imgs.ImageSize = new Size(16, 16);
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab1.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab1.Clone());
            this.tab_imgs.Images.Add((Bitmap)Properties.Resources.tab1.Clone());
            this.tabControl1.ImageList = this.tab_imgs;
            this.tabPage1.ImageIndex = 0;
        }

        public void Free()
        {
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




            this.Font.Dispose();
            this.Font = null;
        }

        private void CloseAllTabs(bool including_main)
        {
            if (including_main)
            {
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
    }
}
