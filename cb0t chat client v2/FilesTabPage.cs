using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class FilesTabPage : TabPage // 5
    {
        public List<ListViewItem> audio = new List<ListViewItem>();
        public List<ListViewItem> video = new List<ListViewItem>();
        public List<ListViewItem> image = new List<ListViewItem>();
        public List<ListViewItem> document = new List<ListViewItem>();
        public List<ListViewItem> software = new List<ListViewItem>();
        public List<ListViewItem> other = new List<ListViewItem>();

        private TreeView options;
        private FileList results;
        private TreeNode treeNode1 = new TreeNode("All");
        private TreeNode treeNode2 = new TreeNode("Audio");
        private TreeNode treeNode3 = new TreeNode("Image");
        private TreeNode treeNode4 = new TreeNode("Video");
        private TreeNode treeNode5 = new TreeNode("Document");
        private TreeNode treeNode6 = new TreeNode("Software");
        private TreeNode treeNode7 = new TreeNode("Other");

        private String username;
        private bool receiving_files = true;
        private byte mime_type;
        private int current_node = 0;
        private int files_so_far = 0;
        public ushort predicted_total = 0;

        public int tab_ident;
        public int session_ident;

        private delegate void FileResultsDelegate();
        private delegate void FileResultsDelegate2(int total);

        public FilesTabPage(String name, int tab_ident, int session_ident, byte mime_type)
        {
            this.mime_type = mime_type;
            this.username = name;
            this.tab_ident = tab_ident;
            this.session_ident = session_ident;
            this.ImageIndex = 3;
            this.BackColor = SystemColors.Control;
            this.Location = new Point(4, 22);
            this.Name = name;
            this.Padding = new Padding(3);
            this.Size = new Size(866, 435);
            this.TabIndex = 2;
            this.Text = name;
            this.Tag = "file";
            this.ContextMenuStrip = new ContextMenuStrip();

            this.options = new TreeView();
            this.options.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left)));
            this.options.Location = new Point(0, 0);
            this.options.Name = "options";
            this.options.ImageList = new ImageList();
            this.options.ImageList.TransparentColor = Color.Magenta;
            this.options.ImageList.Images.Add(AresImages.Files_Closed);
            this.treeNode1.Name = "Node0";
            this.treeNode1.Text = "All";
            this.treeNode1.ImageIndex = 0;
            this.treeNode2.Name = "Node1";
            this.treeNode2.Text = "Audio";
            this.treeNode2.ImageIndex = 0;
            this.treeNode3.Name = "Node2";
            this.treeNode3.Text = "Image";
            this.treeNode3.ImageIndex = 0;
            this.treeNode4.Name = "Node3";
            this.treeNode4.Text = "Video";
            this.treeNode4.ImageIndex = 0;
            this.treeNode5.Name = "Node4";
            this.treeNode5.Text = "Document";
            this.treeNode5.ImageIndex = 0;
            this.treeNode6.Name = "Node5";
            this.treeNode6.Text = "Software";
            this.treeNode6.ImageIndex = 0;
            this.treeNode7.Name = "Node6";
            this.treeNode7.Text = "Other";
            this.treeNode7.ImageIndex = 0;
            this.options.Nodes.AddRange(new TreeNode[] {
            this.treeNode1,
            this.treeNode2,
            this.treeNode3,
            this.treeNode4,
            this.treeNode5,
            this.treeNode6,
            this.treeNode7});
            this.options.Size = new Size(164, 435);
            this.options.TabIndex = 0;
            this.options.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.NodeMouseClick);
            this.Controls.Add(this.options);

            this.results = new FileList();
            this.results.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.results.GridLines = true;
            this.results.Location = new Point(163, 0);
            this.results.Name = "results";
            this.results.Size = new Size(703, 435);
            this.results.TabIndex = 1;
            this.results.UseCompatibleStateImageBehavior = false;
            this.results.View = View.Details;
            this.Controls.Add(this.results);
        }

        public void FreeResources()
        {
            this.Controls.Clear();
            this.options.Dispose();
            this.results.Dispose();
            this.Dispose();
        }

        public void UpdateFilesSoFar()
        {
            if (this.results.InvokeRequired)
            {
                this.results.BeginInvoke(new FileResultsDelegate(this.UpdateFilesSoFar));
            }
            else
            {
                if (this.TotalFiles() > (this.files_so_far + 49))
                {
                    this.files_so_far = this.TotalFiles();
                    this.results.Items[0] = new ListViewItem("Receiving file list... (" + this.files_so_far + " of " + this.predicted_total + ")");
                }
            }
        }

        public void ShowExpectedFileCount(int total)
        {
            if (this.results.InvokeRequired)
            {
                this.results.BeginInvoke(new FileResultsDelegate2(this.ShowExpectedFileCount), total);
            }
            else
            {
                this.results.Items[0] = new ListViewItem("Receiving file list... (0 of " + total + ")");
            }
        }

        private void NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!this.receiving_files)
            {
                if (this.current_node != e.Node.Index)
                {
                    this.current_node = e.Node.Index;
                    this.results.Items.Clear();
                    this.results.BeginUpdate();

                    switch (this.current_node)
                    {
                        case 0:
                            this.results.Items.AddRange(this.audio.ToArray());
                            this.results.Items.AddRange(this.software.ToArray());
                            this.results.Items.AddRange(this.video.ToArray());
                            this.results.Items.AddRange(this.document.ToArray());
                            this.results.Items.AddRange(this.image.ToArray());
                            this.results.Items.AddRange(this.other.ToArray());
                            break;

                        case 1:
                            this.results.Items.AddRange(this.audio.ToArray());
                            break;

                        case 2:
                            this.results.Items.AddRange(this.image.ToArray());
                            break;

                        case 3:
                            this.results.Items.AddRange(this.video.ToArray());
                            break;

                        case 4:
                            this.results.Items.AddRange(this.document.ToArray());
                            break;

                        case 5:
                            this.results.Items.AddRange(this.software.ToArray());
                            break;

                        case 6:
                            this.results.Items.AddRange(this.other.ToArray());
                            break;
                    }

                    this.results.EndUpdate();
                }
            }
        }

        public void OnFileBrowseComplete()
        {
            this.PopulateInitialList();
            this.SetTreeNumbers();
            this.SetTabText();
            this.receiving_files = false;
        }

        public int TotalFiles()
        {
            int total = 0;
            total += this.audio.Count;
            total += this.video.Count;
            total += this.document.Count;
            total += this.other.Count;
            total += this.image.Count;
            total += this.software.Count;
            return total;
        }

        private void SetTabText()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new FileResultsDelegate(this.SetTabText));
            }
            else
            {
                this.Text = this.username + " (" + this.TotalFiles() + ")";
            }
        }

        private void SetTreeNumbers()
        {
            if (this.options.InvokeRequired)
            {
                this.options.BeginInvoke(new FileResultsDelegate(this.SetTreeNumbers));
            }
            else
            {
                this.treeNode1.Text += " (" + this.TotalFiles() + ")";
                this.treeNode2.Text += " (" + this.audio.Count + ")";
                this.treeNode3.Text += " (" + this.image.Count + ")";
                this.treeNode4.Text += " (" + this.video.Count + ")";
                this.treeNode5.Text += " (" + this.document.Count + ")";
                this.treeNode6.Text += " (" + this.software.Count + ")";
                this.treeNode7.Text += " (" + this.other.Count + ")";

                switch (this.mime_type)
                {
                    case 0:
                        this.options.SelectedNode = this.options.Nodes[0];
                        this.current_node = 0;
                        break;

                    case 1:
                        this.options.SelectedNode = this.options.Nodes[1];
                        this.current_node = 1;
                        break;

                    case 3:
                        this.options.SelectedNode = this.options.Nodes[5];
                        this.current_node = 5;
                        break;

                    case 5:
                        this.options.SelectedNode = this.options.Nodes[3];
                        this.current_node = 3;
                        break;

                    case 6:
                        this.options.SelectedNode = this.options.Nodes[4];
                        this.current_node = 4;
                        break;

                    case 7:
                        this.options.SelectedNode = this.options.Nodes[2];
                        this.current_node = 2;
                        break;

                    case 8:
                        this.options.SelectedNode = this.options.Nodes[6];
                        this.current_node = 6;
                        break;
                }
            }
        }

        private void PopulateInitialList()
        {
            if (this.results.InvokeRequired)
            {
                this.results.BeginInvoke(new FileResultsDelegate(this.PopulateInitialList));
            }
            else
            {
                this.results.Items.Clear();
                this.results.BeginUpdate();

                switch (this.mime_type)
                {
                    case 0:
                        this.results.Items.AddRange(this.audio.ToArray());
                        this.results.Items.AddRange(this.software.ToArray());
                        this.results.Items.AddRange(this.video.ToArray());
                        this.results.Items.AddRange(this.document.ToArray());
                        this.results.Items.AddRange(this.image.ToArray());
                        this.results.Items.AddRange(this.other.ToArray());
                        break;

                    case 1:
                        this.results.Items.AddRange(this.audio.ToArray());
                        break;

                    case 3:
                        this.results.Items.AddRange(this.software.ToArray());
                        break;

                    case 5:
                        this.results.Items.AddRange(this.video.ToArray());
                        break;

                    case 6:
                        this.results.Items.AddRange(this.document.ToArray());
                        break;

                    case 7:
                        this.results.Items.AddRange(this.image.ToArray());
                        break;

                    case 8:
                        this.results.Items.AddRange(this.other.ToArray());
                        break;
                }

                this.results.EndUpdate();
            }
        }

        public void OnFileBrowseFailed()
        {
            if (this.results.InvokeRequired)
            {
                this.results.BeginInvoke(new FileResultsDelegate(this.OnFileBrowseComplete));
            }
            else
            {
                this.results.Items.Clear();
                this.results.Items.Add("Browse failed.");
            }
        }

    }
}
