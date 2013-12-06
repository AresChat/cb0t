using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace cb0t
{
    class UserListBox : ListBox
    {
        private Bitmap browse_icon = null;
        private Bitmap music_icon = null;
        private Bitmap away_icon = null;
        private Bitmap voice_icon = null;
        private int hover_item = -1;
        private bool updating = false;
        private UserListToolTip popup;
        private Bitmap def_av = null;

        public bool IsBlack { get; set; }

        public UserListBox()
        {
            this.DoubleBuffered = true;
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.HorizontalScrollbar = false;
            this.ScrollAlwaysVisible = true;
            this.BackColor = Color.White;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.browse_icon = (Bitmap)Properties.Resources.folder.Clone();
            this.music_icon = (Bitmap)Properties.Resources.music.Clone();
            this.away_icon = (Bitmap)Properties.Resources.away.Clone();
            this.voice_icon = (Bitmap)Properties.Resources.button4.Clone();
            this.popup = new UserListToolTip();

            if (Avatar.DefaultAvatar != null)
                this.createdefav();
        }

        private void createdefav()
        {
            using (MemoryStream ms = new MemoryStream(Avatar.DefaultAvatar))
            using (Bitmap dav = new Bitmap(ms))
            using (Bitmap sized = new Bitmap(53, 53))
            using (Graphics sized_g = Graphics.FromImage(sized))
            {
                sized_g.SmoothingMode = SmoothingMode.HighQuality;
                sized_g.CompositingQuality = CompositingQuality.HighQuality;
                sized_g.DrawImage(dav, new Rectangle(0, 0, 53, 53), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                this.def_av = new Bitmap(53, 53);

                using (Graphics av_g = Graphics.FromImage(this.def_av))
                using (GraphicsPath path = new Rectangle(0, 0, 52, 52).Rounded(8))
                using (TextureBrush brush = new TextureBrush(sized))
                {
                    av_g.SmoothingMode = SmoothingMode.HighQuality;
                    av_g.CompositingQuality = CompositingQuality.HighQuality;

                    using (SolidBrush sb = new SolidBrush(Color.White))
                        av_g.FillPath(sb, path);

                    av_g.FillPath(brush, path);

                    using (Pen pen = new Pen(Color.Gainsboro, 1))
                        av_g.DrawPath(pen, path);
                }
            }
        }

        public void UpdateBlackImgs()
        {
            this.music_icon.Dispose();
            this.voice_icon.Dispose();
            this.music_icon = (Bitmap)Properties.Resources.musicbg.Clone();
            this.voice_icon = (Bitmap)Properties.Resources.button4bg.Clone();
        }

        public void Free()
        {
            this.popup.Free();
            this.popup.Dispose();
            this.popup = null;
            this.browse_icon.Dispose();
            this.browse_icon = null;
            this.music_icon.Dispose();
            this.music_icon = null;
            this.away_icon.Dispose();
            this.away_icon = null;
            this.voice_icon.Dispose();
            this.voice_icon = null;

            if (this.def_av != null)
            {
                this.def_av.Dispose();
                this.def_av = null;
            }
        }

        public new void BeginUpdate()
        {
            this.updating = true;
            base.BeginUpdate();
        }

        public new void EndUpdate()
        {
            this.updating = false;
            base.EndUpdate();
        }

        public void SortAdmins()
        {
            this.Sort();
        }

        public void OnItemRemoved(int index)
        {
            if (index == this.hover_item)
                this.hover_item = -1;
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

                this.hover_item = i == this.SelectedIndex ? -1 : i;

                if (this.hover_item > -1)
                    if (this.hover_item >= 0 && this.hover_item < this.Items.Count)
                        this.Invalidate(this.GetItemRectangle(this.hover_item));
            }

            if (i > -1 && i < this.Items.Count)
                if (this.Items[i] is UserListBoxItem)
                {
                    User u = ((UserListBoxItem)this.Items[i]).Owner;

                    if (this.popup.CurrentUser == null)
                    {
                        this.popup.CurrentUser = u;
                        this.popup.SetToolTip(this, "popup");
                    }
                    else if (this.popup.CurrentUser.Name != u.Name)
                    {
                        this.popup.CurrentUser = u;
                        this.popup.SetToolTip(this, "popup");
                    }
                }
                else if (this.popup.Active)
                {
                    this.popup.SetToolTip(this, String.Empty);
                    this.popup.CurrentUser = null;
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

            if (this.popup.Active)
            {
                this.popup.SetToolTip(this, String.Empty);
                this.popup.CurrentUser = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int i = this.IndexFromPoint(e.Location);

            if (i > -1 && i < this.Items.Count)
                if (!(this.Items[i] is UserListBoxItem))
                    this.SelectedIndex = -1;

            if (e.Button == MouseButtons.Right)
                if (i > -1 && i < this.Items.Count)
                    if (this.Items[i] is UserListBoxItem)
                        this.SelectedIndex = i;
        }

        protected override void Sort()
        {
            int start_index = -1, end_index = -1;

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is UserListBoxSectionItem)
                    if (((UserListBoxSectionItem)this.Items[i]).Section == UserListBoxSectionType.Admins)
                        start_index = i;

                if (this.Items[i] is UserListBoxSectionItem)
                    if (((UserListBoxSectionItem)this.Items[i]).Section == UserListBoxSectionType.Users)
                        end_index = i;
            }

            if (start_index > -1 && end_index > start_index)
            {
                if (this.Items[end_index - 1] is UserListBoxEmptyItem)
                    return;

                bool ordered = true;
                int current_level = 3;

                for (int i = (start_index + 1); i < end_index; i++)
                {
                    if (((UserListBoxItem)this.Items[i]).Owner.Level > current_level)
                    {
                        ordered = false;
                        break;
                    }

                    current_level = ((UserListBoxItem)this.Items[i]).Owner.Level;
                }

                if (!ordered)
                {
                    bool should_update = !this.updating;

                    if (should_update)
                        this.BeginUpdate();

                    List<UserListBoxItem> list = new List<UserListBoxItem>();
                    int ti = this.TopIndex;

                    for (int i = (start_index + 1); i < end_index; i++)
                    {
                        list.Add((UserListBoxItem)this.Items[start_index + 1]);
                        this.Items.RemoveAt(start_index + 1);
                    }

                    list.Sort((x, y) =>
                    {
                        int c = x.Owner.Level.CompareTo(y.Owner.Level);

                        if (c != 0)
                            return c;
                        else
                            return y.Owner.Ident.CompareTo(x.Owner.Ident);
                    });

                    for (int i = 0; i < list.Count; i++)
                        this.Items.Insert(start_index + 1, list[i]);

                    this.TopIndex = ti < this.Items.Count ? ti : (this.Items.Count - 1);

                    if (should_update)
                        this.EndUpdate();
                }
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                if (this.Items[e.Index] is UserListBoxItem)
                    ((UserListBoxItem)this.Items[e.Index]).Draw(e, ref browse_icon, ref music_icon, this.SelectedIndex == e.Index, this.hover_item == e.Index, ref this.away_icon, ref this.voice_icon, this.IsBlack, ref this.def_av);
                else if (this.Items[e.Index] is UserListBoxSectionItem)
                    ((UserListBoxSectionItem)this.Items[e.Index]).Draw(e);
                else if (this.Items[e.Index] is UserListBoxEmptyItem)
                    ((UserListBoxEmptyItem)this.Items[e.Index]).Draw(e);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            this.Invalidate();
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            if (e.Index > -1 && e.Index < this.Items.Count)
            {
                if (this.Items[e.Index] is UserListBoxEmptyItem)
                    e.ItemHeight = 16;
                else if (this.Items[e.Index] is UserListBoxSectionItem)
                    e.ItemHeight = 16;
                else if (this.Items[e.Index] is UserListBoxItem)
                    e.ItemHeight = 56;
            }

            base.OnMeasureItem(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Region region = new Region(e.ClipRectangle))
            {
                using (SolidBrush sb = new SolidBrush(this.BackColor))
                    e.Graphics.FillRegion(sb, region);

                if (this.Items.Count > 0)
                    for (int i = 0; i < this.Items.Count; ++i)
                    {
                        Rectangle rectangle = this.GetItemRectangle(i);

                        if (e.ClipRectangle.IntersectsWith(rectangle))
                        {
                            this.OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                    rectangle, i,
                                    DrawItemState.Selected, this.ForeColor,
                                    this.BackColor));

                            region.Complement(rectangle);
                        }
                    }
            }
        }

    }
}
