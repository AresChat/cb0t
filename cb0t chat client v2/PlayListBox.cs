using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace cb0t_chat_client_v2
{
    class PlayListBox : ListView
    {
        public PlayListBox()
        {
            this.Columns.AddRange(new ColumnHeader[] { new ColumnHeader(), new ColumnHeader() });
            this.Columns[0].TextAlign = HorizontalAlignment.Left;
            this.Columns[1].TextAlign = HorizontalAlignment.Right;
            this.Columns[0].Width = (this.Width - 95);
            this.Columns[1].Width = 70;
            this.DoubleBuffered = true;
            this.MultiSelect = false;
            this.FullRowSelect = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Columns.Count == 2)
            {
                this.Columns[0].Width = (this.Width - 95);
                this.Columns[1].Width = 70;
            }
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    e.NewWidth = (this.Width - 95);
                    break;

                case 1:
                    e.NewWidth = 70;
                    break;
            }

            e.Cancel = true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            ListViewItem item = this.GetItemAt(e.X, e.Y);

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (item != null)
                {
                    if (this.Items[i].Index != item.Index)
                    {
                        if (!this.Items[i].BackColor.Equals(Color.White))
                            this.Items[i].BackColor = Color.White;
                    }
                    else if (!this.Items[i].BackColor.Equals(Color.Silver))
                        this.Items[i].BackColor = Color.Silver;
                }
                else if (!this.Items[i].BackColor.Equals(Color.Silver))
                    this.Items[i].BackColor = Color.Silver;
            }
        }

        public int MoveItem(int index, bool up)
        {
            if (up)
            {
                if (index == 0)
                    return -1;

                ListViewItem p1 = (ListViewItem)this.Items[index];
                ListViewItem p2 = (ListViewItem)this.Items[index - 1];

                this.Items.RemoveAt(index);
                this.Items.RemoveAt(index - 1);
                this.Items.Insert(index - 1, p1);
                this.Items.Insert(index, p2);
                this.Items[index - 1].Selected = true;

                for (int i = 0; i < this.Items.Count; i++)
                    if (this.Items[i].ForeColor.Equals(Color.Red))
                        return i;
            }
            else
            {
                if (index == (this.Items.Count - 1))
                    return -1;

                ListViewItem p1 = (ListViewItem)this.Items[index];
                ListViewItem p2 = (ListViewItem)this.Items[index + 1];

                this.Items.RemoveAt(index + 1);
                this.Items.RemoveAt(index);
                this.Items.Insert(index, p2);
                this.Items.Insert(index + 1, p1);
                this.Items[index + 1].Selected = true;

                for (int i = 0; i < this.Items.Count; i++)
                    if (this.Items[i].ForeColor.Equals(Color.Red))
                        return i;
            }

            return -1;
        }

        private delegate void AddItemHandler(PlaylistItem item);
        public void AddItem(PlaylistItem item)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new AddItemHandler(this.AddItem), item);
            else
            {
                ListViewItem p = new ListViewItem();
                String str = item.Name;

                if (item.Artist.Length > 0)
                    str = item.Artist + " - " + str;

                p.SubItems[0].Text = str;
                p.SubItems.Add(item.Length);
                this.Items.Add(p);
            }
        }

        public void SetActiveItem(int index)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (i == index)
                {
                    if (!this.Items[i].ForeColor.Equals(Color.Red))
                        this.Items[i].ForeColor = Color.Red;
                }
                else if (!this.Items[i].ForeColor.Equals(Color.Black))
                    this.Items[i].ForeColor = Color.Black;
            }
        }

        

    }
}
