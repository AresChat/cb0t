using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace cb0t_chat_client_v2
{
    class DCListView : ListView
    {
        private delegate void AddNewSendFileDelegate(DCSendFileObject item, String path);
        private delegate void AddNewReceiveFileDelegate(DCReceiveFileObject item);
        private delegate void AmendStateDelegate(ushort referal, String state);
        private delegate void AmendSoFarDelegate(ushort referal, ulong so_far);
        private delegate void UpdateItemsDelegate();


        public DCListView()
        {
            this.View = View.Details;
            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.GridLines = true;
            this.MultiSelect = false;
            this.Columns.AddRange(new ColumnHeader[] { new ColumnHeader(),
                new ColumnHeader(), new ColumnHeader(), new ColumnHeader() });
            this.Columns[0].Text = "Filename";
            this.Columns[1].Text = "Task";
            this.Columns[2].Text = "Progress";
            this.Columns[3].Text = "Size";
            this.SmallImageList = new ImageList();
            this.SmallImageList.TransparentColor = Color.Magenta;

            if (AresImages.Upload != null && AresImages.Download != null)
                this.SmallImageList.Images.AddRange(new Image[] { AresImages.Upload, AresImages.Download });

            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Locate folder");
            this.ContextMenuStrip.Items[0].Click += new EventHandler(this.OnLocateFolder);
            this.ContextMenuStrip.Items.Add("Clear idle");
            this.ContextMenuStrip.Items[1].Click += new EventHandler(this.OnClearIdle);
        }

        private void OnClearIdle(object sender, EventArgs e)
        {
            while (true)
            {
                bool found_item = false;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    DCListViewItem item = (DCListViewItem)this.Items[i];

                    if (item.state == "complete" || item.state == "cancelled")
                    {
                        found_item = true;
                        this.Items.RemoveAt(i);
                        break;
                    }
                }

                if (!found_item)
                    return;
            }
        }

        private void OnLocateFolder(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                DCListViewItem item = (DCListViewItem)this.SelectedItems[0];

                try
                {
                    Process.Start("explorer.exe", item.path);
                }
                catch { }
            }
        }

        public void AddNewFile(DCSendFileObject item, String path)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddNewSendFileDelegate(this.AddNewFile), item);
            }
            else
            {
                DCListViewItem l = new DCListViewItem();
                l.referal = item.referal;
                l.receiving = false;
                l.connected = false;
                l.bytes_full = item.filesize;
                l.bytes_so_far = 0;
                l.filename = item.displayname;
                l.SubItems.AddRange(new String[] { String.Empty, String.Empty, String.Empty });
                l.ImageIndex = 0;
                l.path = path;

                if (l.path.Length > l.filename.Length)
                    if (l.path.LastIndexOf("/") > -1)
                        l.path = l.path.Substring(0, l.path.LastIndexOf("/"));

                this.Items.Add(l);
            }
        }

        public void AddNewFileRec(DCReceiveFileObject item)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddNewReceiveFileDelegate(this.AddNewFileRec), item);
            }
            else
            {
                DCListViewItem l = new DCListViewItem();
                l.referal = item.referal;
                l.receiving = true;
                l.connected = false;
                l.bytes_full = item.filesize;
                l.bytes_so_far = 0;
                l.filename = item.displayname;
                l.SubItems.AddRange(new String[] { String.Empty, String.Empty, String.Empty });
                l.ImageIndex = 1;
                l.state = "receiving";
                l.path = AppDomain.CurrentDomain.BaseDirectory + "/received files/";
                this.Items.Add(l);
            }
        }

        public void AmendSoFar(ushort referal, ulong so_far)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AmendSoFarDelegate(this.AmendSoFar), referal, so_far);
            }
            else
            {
                foreach (ListViewItem l in this.Items)
                {
                    DCListViewItem d = (DCListViewItem)l;

                    if (d.referal == referal)
                    {
                        if (!d.receiving)
                        {
                            d.bytes_so_far += so_far;
                            return;
                        }
                    }
                }
            }
        }

        public void AmendSoFarRec(ushort referal, ulong so_far)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AmendSoFarDelegate(this.AmendSoFarRec), referal, so_far);
            }
            else
            {
                foreach (ListViewItem l in this.Items)
                {
                    DCListViewItem d = (DCListViewItem)l;

                    if (d.referal == referal)
                    {
                        if (d.state == "receiving")
                        {
                            d.bytes_so_far += so_far;
                            return;
                        }
                    }
                }
            }
        }

        public void UpdateItems()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new UpdateItemsDelegate(this.UpdateItems));
            else
                this.Invalidate();
        }

        public void AmendState(ushort referal, String state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AmendStateDelegate(this.AmendState), referal, state);
            }
            else
            {
                foreach (ListViewItem l in this.Items)
                {
                    DCListViewItem d = (DCListViewItem)l;

                    if (d.referal == referal)
                    {
                        if (!d.receiving)
                        {
                            d.state = state;
                            return;
                        }
                    }
                }
            }
        }

        public void AmendStateRec(ushort referal, String state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AmendStateDelegate(this.AmendStateRec), referal, state);
            }
            else
            {
                foreach (ListViewItem l in this.Items)
                {
                    DCListViewItem d = (DCListViewItem)l;

                    if (d.referal == referal)
                    {
                        if (d.state == "receiving")
                        {
                            d.state = state;
                            return;
                        }
                    }
                }
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            DCListViewItem file = (DCListViewItem)e.Item;
            Graphics g = e.Graphics;
            Rectangle r = e.Bounds;

            switch (e.ColumnIndex)
            {
                case 0: // filename
                    g.DrawImage(this.SmallImageList.Images[e.Item.ImageIndex], new Point(r.X, r.Y));
                    RectangleF tmp = new RectangleF(r.X + 16, r.Y, r.Width - 16, r.Height);
                    g.DrawString(file.filename, this.Font, Brushes.Black, tmp);
                    break;

                case 1: // task
                    g.DrawString(file.state, this.Font, Brushes.Black, (RectangleF)r);
                    break;

                case 2: // progress bar
                    int percent = (int)Math.Round(((double)file.bytes_so_far / (double)file.bytes_full) * 100);
                    Rectangle indented_area = new Rectangle(r.X + 2, r.Y + 2, r.Width - 4, r.Height - 4);
                    g.FillRectangle(Brushes.Gainsboro, indented_area);
                    int percentage_width = (int)Math.Round(((double)indented_area.Width / 100) * percent);
                    Rectangle percentage_area = new Rectangle(indented_area.X, indented_area.Y, percentage_width, indented_area.Height);
                    g.FillRectangle(percent == 100 ? Brushes.PaleGreen : Brushes.CornflowerBlue, percentage_area);
                    Font font = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Regular);
                    float text_width = g.MeasureString(percent + "%", font).Width;

                    if (text_width < indented_area.Width)
                    {
                        float text_start_point = indented_area.X + ((indented_area.Width / 2) - (text_width / 2));
                        g.DrawString(percent + "%", font, Brushes.Black, new PointF(text_start_point, indented_area.Y));

                    }

                    g.DrawRectangle(new Pen(Brushes.Black, (float)0.5), indented_area);
                    break;

                case 3: // file size
                    g.DrawString(Helpers.DCPresentBytes(file.bytes_so_far, file.bytes_full), this.Font, Brushes.Black, (RectangleF)r);
                    break;
            }
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Width > 30)
            {
                int w = this.Width - 25;
                int w1 = (int)Math.Round((double)(w / 100) * 20);
                int w2 = w - (w1 * 3);

                if (w1 > 0 && w2 > 0)
                {
                    if (this.Columns.Count == 4)
                    {
                        this.Columns[0].Width = w2;
                        this.Columns[1].Width = w1;
                        this.Columns[2].Width = w1;
                        this.Columns[3].Width = w1;
                    }
                }
            }
        }

    }
}
