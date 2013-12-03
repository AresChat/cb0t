using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class BrowseView : ListView
    {
        private int CurrentWidth { get; set; }

        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private Pen column_outline = new Pen(new SolidBrush(Color.FromArgb(109, 115, 123)), 1);
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);

        public void KillResources()
        {
            this.column_outline.Dispose();
            this.column_text_brush.Dispose();
            this.Clear();
        }

        public BrowseView()
        {
            this.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader(),
                new ColumnHeader(),
                new ColumnHeader(),
                new ColumnHeader(),
                new ColumnHeader(),
                new ColumnHeader()
            });

            this.Columns[0].Text = "Title";
            this.Columns[0].Width = 170;
            this.Columns[1].Text = "Artist";
            this.Columns[1].Width = 64;
            this.Columns[2].Text = "Media";
            this.Columns[2].Width = 64;
            this.Columns[3].Text = "Category";
            this.Columns[3].Width = 96;
            this.Columns[4].Text = "Size";
            this.Columns[4].Width = 64;
            this.Columns[5].Text = "Filename";
            this.Columns[5].Width = (this.Width - 394) > 0 ? (this.Width - 394) : 1;

            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.CurrentWidth = this.Width;
            this.Scrollable = true;
        }

        public void UpdateTemplate()
        {
            this.Columns[0].Text = StringTemplate.Get(STType.BrowseTab, 8);
            this.Columns[1].Text = StringTemplate.Get(STType.BrowseTab, 9);
            this.Columns[2].Text = StringTemplate.Get(STType.BrowseTab, 10);
            this.Columns[3].Text = StringTemplate.Get(STType.BrowseTab, 11);
            this.Columns[4].Text = StringTemplate.Get(STType.BrowseTab, 12);
            this.Columns[5].Text = StringTemplate.Get(STType.BrowseTab, 13);
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;

            if (e.ColumnIndex == 0)
                e.NewWidth = 170;
            else if (e.ColumnIndex == 3)
                e.NewWidth = 96;
            else if (e.ColumnIndex >= 1 && e.ColumnIndex <= 4)
                e.NewWidth = 64;
            else if (e.ColumnIndex == 5)
                e.NewWidth = (this.Width - 394) > 0 ? (this.Width - 394) : 1;
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Columns.Count == 6)
            {
                if (this.CurrentWidth != this.Width)
                {
                    this.CurrentWidth = this.Width;
                    this.Columns[5].Width = (this.Width - 394) > 0 ? (this.Width - 394) : 1;
                }
            }

            this.Refresh();
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);

            if (e.ColumnIndex == 5)
                r = new Rectangle(r.X, r.Y, r.Width - 2, r.Height);

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            e.Graphics.DrawRectangle(this.column_outline, r);
            e.Graphics.DrawString(e.Header.Text, this.Font, this.column_text_brush, new PointF(e.Bounds.X + 3, e.Bounds.Y + 5));
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
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
