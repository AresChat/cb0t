using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace cb0t
{
    class ChannelListView : ListView
    {
        private int CurrentWidth { get; set; }
        private int CurrentHover { get; set; }
        private int CurrentHoverClear { get; set; }

        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private Pen column_outline = new Pen(new SolidBrush(Color.FromArgb(109, 115, 123)), 1);
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);

        public ChannelListView()
        {
            this.DoubleBuffered = true;
            this.OwnerDraw = true;
            this.View = View.Details;
            this.Columns.Add("Name");
            this.Columns.Add("Topic");
            this.Columns[0].Width = 170;
            this.Columns[1].Width = (this.Width - 170);
            this.FullRowSelect = true;
            this.MultiSelect = false;

            this.CurrentWidth = this.Width;
            this.CurrentHover = -1;
            this.CurrentHoverClear = -1;
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;

            switch (e.ColumnIndex)
            {
                case 0:
                    e.NewWidth = 170;
                    break;

                case 1:
                    e.NewWidth = (this.Width - 170);
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Columns.Count == 2)
            {
                if (this.CurrentWidth != this.Width)
                {
                    this.CurrentWidth = this.Width;
                    this.Columns[1].Width = (this.Width - 170);
                }
            }

            this.Invalidate();
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            if (e.ColumnIndex == 1)
                r = new Rectangle(r.X, r.Y, r.Width - 5, r.Height);

            e.Graphics.DrawRectangle(this.column_outline, r);
            e.Graphics.DrawString(e.Header.Text, this.Font, this.column_text_brush, new PointF(e.Bounds.X + 3, e.Bounds.Y + 5));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            ListViewItem item = this.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                if (item.Index == this.CurrentHover)
                    return;

                if (this.CurrentHover > -1)
                {
                    if (this.Items.Count > this.CurrentHover)
                    {
                        this.CurrentHoverClear = this.CurrentHover;

                        if (this.Items.Count > this.CurrentHoverClear)
                            this.Invalidate(this.Items[this.CurrentHoverClear].Bounds);

                        if (!item.Selected)
                        {
                            this.CurrentHover = item.Index;
                            this.Invalidate(this.Items[this.CurrentHover].Bounds);
                        }
                        else this.CurrentHover = -1;
                    }
                }
                else
                {
                    if (!item.Selected)
                    {
                        this.CurrentHover = item.Index;
                        this.Invalidate(this.Items[this.CurrentHover].Bounds);
                    }
                    else this.CurrentHover = -1;
                }
            }
            else if (this.CurrentHover > -1)
            {
                this.CurrentHoverClear = this.CurrentHover;
                this.CurrentHover = -1;

                if (this.Items.Count > this.CurrentHoverClear)
                    this.Invalidate(this.Items[this.CurrentHoverClear].Bounds);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.CurrentHover > -1)
            {
                int i = this.CurrentHover;
                this.CurrentHover = -1;

                if (this.Items.Count > i)
                    this.Invalidate(this.Items[i].Bounds);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.SelectedIndices.Count > 0)
                this.Invalidate(this.Items[this.SelectedIndices[0]].Bounds);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x83:
                    int style = (int)GetWindowLong(this.Handle, GWL_STYLE);

                    if ((style & WS_HSCROLL) == WS_HSCROLL)
                        SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_HSCROLL);

                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private const int GWL_STYLE = -16;
        private const int WS_HSCROLL = 0x00100000;

        private static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        private static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
