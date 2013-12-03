using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class BrowseLeftContainer : UserControl
    {
        public event EventHandler ChangeListContents;

        private ImageList ilist;
        private Bitmap bmp;

        public BrowseLeftContainer()
        {
            this.InitializeComponent();
            this.bmp = (Bitmap)Properties.Resources.folder.Clone();
            this.ilist = new ImageList();
            this.ilist.Images.Add(this.bmp);
            this.treeView1.ImageList = this.ilist;

            foreach (TreeNode node in this.treeView1.Nodes)
            {
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                node.StateImageIndex = 0;
            }

            this.treeView1.BeforeSelect += this.BeforeNodeClicked;
            this.treeView1.AfterSelect += this.AfterNodeClicked;
        }

        public bool CanSelectNode { get; set; }

        public void UpdateTemplate()
        {
            this.treeView1.Nodes[0].Text = this.treeView1.Nodes[0].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 1));
            this.treeView1.Nodes[1].Text = this.treeView1.Nodes[1].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 2));
            this.treeView1.Nodes[2].Text = this.treeView1.Nodes[2].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 3));
            this.treeView1.Nodes[3].Text = this.treeView1.Nodes[3].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 4));
            this.treeView1.Nodes[4].Text = this.treeView1.Nodes[4].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 5));
            this.treeView1.Nodes[5].Text = this.treeView1.Nodes[5].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 6));
            this.treeView1.Nodes[6].Text = this.treeView1.Nodes[6].Text.Replace("All", StringTemplate.Get(STType.BrowseTab, 7));
        }

        private void AfterNodeClicked(object sender, TreeViewEventArgs e)
        {
            if (this.CanSelectNode)
                if (this.ChangeListContents != null)
                    this.ChangeListContents(e.Node.Index, EventArgs.Empty);
        }

        private void BeforeNodeClicked(object sender, TreeViewCancelEventArgs e)
        {
            if (!this.CanSelectNode)
                e.Cancel = true;
        }

        public void SetHeader(String text)
        {
            this.browseLeftHeader1.HeaderText = text;
            this.browseLeftHeader1.Invalidate();
        }

        public void UpdateTotals(int a, int b, int c, int d, int e, int f, int g)
        {
            this.treeView1.BeginInvoke((Action)(() =>
            {
                this.treeView1.Nodes[0].Text += " (" + a + ")";
                this.treeView1.Nodes[1].Text += " (" + b + ")";
                this.treeView1.Nodes[2].Text += " (" + c + ")";
                this.treeView1.Nodes[3].Text += " (" + d + ")";
                this.treeView1.Nodes[4].Text += " (" + e + ")";
                this.treeView1.Nodes[5].Text += " (" + f + ")";
                this.treeView1.Nodes[6].Text += " (" + g + ")";
            }));
        }

        public void DestroyContainer()
        {
            this.Controls.Clear();
            this.treeView1.BeforeSelect -= this.BeforeNodeClicked;
            this.treeView1.AfterSelect -= this.AfterNodeClicked;
            this.treeView1.Nodes.Clear();
            this.treeView1.ImageList = null;
            this.treeView1.Dispose();
            this.treeView1 = null;
            this.browseLeftHeader1.ReleaseResources();
            this.browseLeftHeader1.Dispose();
            this.browseLeftHeader1 = null;
            this.ilist.Images.Clear();
            this.ilist.Dispose();
            this.ilist = null;
            this.bmp.Dispose();
            this.bmp = null;
        }
    }
}
