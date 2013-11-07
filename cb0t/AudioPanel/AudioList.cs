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
    class AudioList : ListView
    {
        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private Pen column_outline = new Pen(new SolidBrush(Color.FromArgb(109, 115, 123)), 1);
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);

        public AudioList()
        {
            this.DoubleBuffered = true;
            this.OwnerDraw = true;
            this.View = View.Details;
            this.Columns.Add("Title");
            this.Columns.Add("Artist");
            this.Columns.Add("Album");
            this.Columns.Add("Duration");
            int cw = (this.Width / 4);
            this.Columns[0].Width = cw;
            this.Columns[1].Width = cw;
            this.Columns[2].Width = cw;
            this.Columns[3].Width = (this.Width - (cw * 3));
            this.FullRowSelect = true;
            this.MultiSelect = true;
            
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);

            if (r.Width <= 0 || r.Height <= 0)
                return;

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            e.Graphics.DrawRectangle(this.column_outline, r);
            e.Graphics.DrawString(e.Header.Text, this.Font, this.column_text_brush, new PointF(e.Bounds.X + 3, e.Bounds.Y + 5));
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Columns.Count == 4)
            {
                int cw = (this.Width / 4);
                this.Columns[0].Width = cw;
                this.Columns[1].Width = cw;
                this.Columns[2].Width = cw;
                this.Columns[3].Width = (this.Width - (cw * 3));
            }

            this.Invalidate();
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            int cw = (this.Width / 4);

            switch (e.ColumnIndex)
            {
                case 0:
                case 1:
                case 2:
                    e.NewWidth = cw;
                    break;

                case 3:
                    e.NewWidth = (this.Width - (cw * 3));
                    break;
            }
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
