using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace cb0t_chat_client_v2
{
    class CEmoteList : ListBox
    {
        private int hover_item = -1;
        private int selected_item = -1;
        private Bitmap empty = null;

        public CEmoteList()
        {
            this.DoubleBuffered = true;
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.HorizontalScrollbar = false;
            this.ScrollAlwaysVisible = true;
            this.ItemHeight = 50;
            this.BackColor = Color.White;

            this.SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint,
            true);
        }

        public int SelectedEmoticon
        {
            get { return this.selected_item; }
        }

        public void LoadEmoticons()
        {
            for (int i = 0; i < 16; i++)
                this.Items.Add(CustomEmotes.Emotes[i]);

            if (AresImages.mini_busy != null)
            {
                this.empty = (Bitmap)AresImages.DCBlock;
                this.empty.MakeTransparent(Color.Magenta);
            }
        }

        public void AddItem(int index, CEmoteItem item)
        {
            this.Items[index] = item;
            this.Invalidate(this.GetItemRectangle(index));
        }

        public void DeleteItem(int index)
        {
            this.Items[index] = new CEmoteItem();
            this.Invalidate(this.GetItemRectangle(index));
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index > -1 && e.Index < this.Items.Count)
            {
                CEmoteItem item = (CEmoteItem)this.Items[e.Index];

                if (e.Index == this.hover_item)
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
                else if (e.Index == this.selected_item)
                    e.Graphics.FillRectangle(Brushes.LightSteelBlue, e.Bounds);
                else
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 20, 48), Color.SteelBlue, Color.WhiteSmoke, 0f))
                    e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 20, 48));

                using (SolidBrush sb = new SolidBrush(Color.Black))
                {
                    if (item.Image == null)
                    {
                        using (Pen pen = new Pen(sb, 1))
                            e.Graphics.DrawRectangle(pen, new Rectangle(e.Bounds.X + 22, e.Bounds.Y + 1, 47, 47));

                        e.Graphics.DrawImage(this.empty, new Point(e.Bounds.X + 38, e.Bounds.Y + 17));
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream(item.Image))
                        {
                            using (Bitmap bmp = new Bitmap(ms))
                            {
                                switch (item.Size)
                                {
                                    case 48:
                                        e.Graphics.DrawImage(bmp, new Point(e.Bounds.X + 22, e.Bounds.Y + 1));
                                        break;

                                    case 32:
                                        e.Graphics.DrawImage(bmp, new Point(e.Bounds.X + 30, e.Bounds.Y + 9));
                                        break;

                                    case 16:
                                        e.Graphics.DrawImage(bmp, new Point(e.Bounds.X + 38, e.Bounds.Y + 17));
                                        break;
                                }
                            }
                        }

                        e.Graphics.DrawString(item.Shortcut, this.Font, sb, new PointF(e.Bounds.X + 75, e.Bounds.Y + 20));
                    }

                    int width = (int)Math.Round(e.Graphics.MeasureString((e.Index + 1).ToString(), this.Font, 100, StringFormat.GenericTypographic).Width);
                    int start = (10 - (width / 2)) - 2;
                    e.Graphics.DrawString((e.Index + 1).ToString(), this.Font, sb, new PointF(e.Bounds.X + start, e.Bounds.Y + 20));

                    if (item.Shortcut == null)
                        e.Graphics.DrawString("Unset emoticon available", this.Font, sb, new PointF(e.Bounds.X + 75, e.Bounds.Y + 20));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);

            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    Rectangle irect = this.GetItemRectangle(i);

                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));

                        iRegion.Complement(irect);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int i = this.IndexFromPoint(e.Location);

            if (i != this.hover_item)
            {
                if (this.hover_item > -1)
                {
                    int old = this.hover_item;
                    this.hover_item = -1;

                    if (old >= 0 && old < this.Items.Count)
                        this.Invalidate(this.GetItemRectangle(old));
                }

                this.hover_item = i == this.selected_item ? -1 : i;

                if (this.hover_item > -1)
                    if (this.hover_item >= 0 && this.hover_item < this.Items.Count)
                        this.Invalidate(this.GetItemRectangle(this.hover_item));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.hover_item > -1)
            {
                int old = this.hover_item;
                this.hover_item = -1;

                if (old >= 0 && old < this.Items.Count)
                    this.Invalidate(this.GetItemRectangle(old));
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int i = this.IndexFromPoint(e.Location);

            if (i == this.hover_item)
                this.hover_item = -1;

            if (i != this.selected_item)
            {
                if (this.selected_item > -1)
                {
                    int old = this.selected_item;
                    this.selected_item = -1;

                    if (old >= 0 && old < this.Items.Count)
                        this.Invalidate(this.GetItemRectangle(old));
                }

                this.selected_item = i;

                if (this.selected_item >= 0 && this.selected_item < this.Items.Count)
                    this.Invalidate(this.GetItemRectangle(this.selected_item));
            }
        }
    }

    class CEmoteItem : IDisposable
    {
        public byte[] Image = null;
        public String Shortcut = null;
        public int Size = -1;
        public String Rtf = null;

        public void Dispose()
        {
            this.Image = null;
            this.Shortcut = null;
            this.Rtf = null;
        }
    }
}
