using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class BrowseTab : TabPage
    {
        private SplitContainer Panels { get; set; }
        private BrowseView Viewer { get; set; }
        private BrowseLeftContainer LeftContainer { get; set; }

        public ushort BrowseIdent { get; set; }

        private int expected = 0;
        private int so_far = 0;
        private int last_update = 0;
        private List<BrowseItem> files = new List<BrowseItem>();

        public void StartReceived(ushort count)
        {
            this.expected = count;
            this.LeftContainer.SetHeader("Loading... (0 / " + count + ")");
        }

        public void ItemReceived(BrowseItem item)
        {
            this.files.Add(item);
            this.so_far++;

            if (this.so_far >= (this.last_update + 50))
            {
                this.last_update = this.so_far;
                this.LeftContainer.SetHeader("Loading... (" + this.so_far + " / " + this.expected + ")");
            }
        }

        public void ErrorReceived()
        {
            this.LeftContainer.SetHeader("Browse failed");
        }

        public void EndReceived()
        {
            this.LeftContainer.SetHeader("Files (" + this.so_far + ")");
            int a = this.files.Count;
            int b = this.files.FindAll(x => x.Mime == BrowseType.Audio).Count;
            int c = this.files.FindAll(x => x.Mime == BrowseType.Image).Count;
            int d = this.files.FindAll(x => x.Mime == BrowseType.Video).Count;
            int e = this.files.FindAll(x => x.Mime == BrowseType.Document).Count;
            int f = this.files.FindAll(x => x.Mime == BrowseType.Software).Count;
            int g = this.files.FindAll(x => x.Mime == BrowseType.Other).Count;
            this.LeftContainer.UpdateTotals(a, b, c, d, e, f, g);
            this.LeftContainer.CanSelectNode = true;
            this.SetContent(0);
        }

        public BrowseTab(String name)
        {
            this.Viewer = new BrowseView();
            this.Viewer.BackColor = Color.White;
            this.Viewer.Dock = DockStyle.Fill;
            this.Viewer.FullRowSelect = true;
            this.Viewer.Location = new Point(0, 0);
            this.Viewer.MultiSelect = false;
            this.Viewer.OwnerDraw = true;
            this.Viewer.Size = new Size(352, 283);
            this.Viewer.TabIndex = 0;
            this.Viewer.UseCompatibleStateImageBehavior = false;
            this.Viewer.View = View.Details;
            this.Viewer.MouseDoubleClick += this.ViewerMouseDoubleClick;

            this.LeftContainer = new BrowseLeftContainer();
            this.LeftContainer.BackColor = Color.White;
            this.LeftContainer.BorderStyle = BorderStyle.FixedSingle;
            this.LeftContainer.Dock = DockStyle.Fill;
            this.LeftContainer.Location = new Point(0, 0);
            this.LeftContainer.Size = new Size(210, 283);
            this.LeftContainer.ChangeListContents += this.ChangeListContentsRequested;

            this.Panels = new SplitContainer();
            this.Panels.Dock = DockStyle.Fill;
            this.Panels.FixedPanel = FixedPanel.Panel1;
            this.Panels.Location = new Point(3, 3);
            this.Panels.Panel1.Controls.Add(this.LeftContainer);
            this.Panels.Panel2.Controls.Add(this.Viewer);
            this.Panels.Size = new Size(566, 283);
            this.Panels.SplitterDistance = 210;

            this.Location = new Point(4, 22);
            this.Size = new Size(542, 251);
            this.Text = name;
            this.UseVisualStyleBackColor = true;
            this.ImageIndex = 3;
            this.Controls.Add(this.Panels);
        }

        public void Free()
        {
            this.Controls.Remove(this.Panels);
            this.Panels.Panel1.Controls.Remove(this.LeftContainer);
            this.Panels.Panel2.Controls.Remove(this.Viewer);
            this.Panels.Dispose();
            this.Panels = null;
            this.Viewer.MouseDoubleClick -= this.ViewerMouseDoubleClick;
            this.Viewer.Items.Clear();
            this.Viewer.Clear();
            this.Viewer.KillResources();
            this.Viewer.Dispose();
            this.Viewer = null;
            this.LeftContainer.ChangeListContents -= this.ChangeListContentsRequested;
            this.LeftContainer.DestroyContainer();
            this.LeftContainer.Dispose();
            this.LeftContainer = null;
            this.files.Clear();
        }

        private void ChangeListContentsRequested(object sender, EventArgs e)
        {
            int ident = (int)sender;
            this.SetContent(ident);
        }

        private void SetContent(int ident)
        {
            if (this.Viewer.InvokeRequired)
                this.Viewer.BeginInvoke(new Action<int>(this.SetContent), ident);
            else
            {
                this.Viewer.Items.Clear();
                this.Viewer.BeginUpdate();

                if (ident == 0)
                {
                    foreach (BrowseItem b in this.files)
                        this.Viewer.Items.Add(new ListViewItem(new String[]
                        {
                            b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                        }));
                }
                else if (ident == 1)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Audio)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }
                else if (ident == 2)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Image)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }
                else if (ident == 3)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Video)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }
                else if (ident == 4)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Document)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }
                else if (ident == 5)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Software)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }
                else if (ident == 6)
                {
                    foreach (BrowseItem b in this.files)
                        if (b.Mime == BrowseType.Other)
                            this.Viewer.Items.Add(new ListViewItem(new String[]
                            {
                                b.Title, b.Artist, b.Mime.ToString(), b.Category, b.FileSizeString, b.FileName
                            }));
                }

                this.Viewer.EndUpdate();
            }
        }

        private void ViewerMouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.Viewer.SelectedIndices.Count > 0)
                {
                    ListViewItem item = this.Viewer.SelectedItems[0];

                    if (item != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Title: " + item.SubItems[0].Text);
                        sb.AppendLine("Artist: " + item.SubItems[1].Text);
                        sb.AppendLine("Media: " + item.SubItems[2].Text);
                        sb.AppendLine("Category: " + item.SubItems[3].Text);
                        sb.AppendLine("Size: " + item.SubItems[4].Text);
                        sb.AppendLine("Filename: " + item.SubItems[5].Text);
                        String path = Settings.DataPath + "filelog.txt";
                        File.WriteAllText(path, sb.ToString());
                        Process.Start("notepad.exe", path);
                    }
                }
            }
            catch { }
        }


    }
}
