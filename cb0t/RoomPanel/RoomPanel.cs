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

        public IPEndPoint EndPoint { get; set; }
        public event EventHandler CloseClicked;
        public event EventHandler CheckUnread;

        public RoomPanel(IPEndPoint ep)
        {
            this.InitializeComponent();
            this.topic = new Topic();
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
            this.toolStripButton5.Dispose();
            this.toolStripButton5 = null;
            this.toolStripButton6.Dispose();
            this.toolStripButton6 = null;
            this.toolStripButton7.Dispose();
            this.toolStripButton7 = null;
            this.toolStripButton8.Dispose();
            this.toolStripButton8 = null;
            this.toolStripLabel1.Dispose();
            this.toolStripLabel1 = null;
            this.textBox1.Font.Dispose();
            this.textBox1.Dispose();
            this.textBox1 = null;


            this.Font.Dispose();
            this.Font = null;
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
