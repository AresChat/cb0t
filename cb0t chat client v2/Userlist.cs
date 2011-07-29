using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace cb0t_chat_client_v2
{
    class Userlist : ListView
    {
        private List<UserObject> UserPool = new List<UserObject>();

        public bool i_am_admin = false;
        public bool i_am_host = false;
        private bool is_updating = false;
        private bool black_background = false;

        private delegate void UserObjectDelegate(UserObject userobj);
        private delegate void SetUpdateDelegate(bool updating);
        private delegate void ClearListDelegate();

        public delegate void PMRequestDelegate(String name);
        public delegate void FileBrowseRequest(String name, byte type);
        public event PMRequestDelegate OnPMRequested;
        public event FileBrowseRequest OnFileBrowseRequested;
        public event PMRequestDelegate OnWhoisRequested;
        public event PMRequestDelegate OnIgnoreRequested;
        public event PMRequestDelegate OnVCIgnoreRequested;
        public event PMRequestDelegate OnNudgeRequested;
        public event Packets.SendPacketDelegate OnAdminCommandRequested;
        public event PMRequestDelegate OnDCRequest;
        public event PMRequestDelegate OnScribbleRequested;
        public event PMRequestDelegate OnCopyNameRequesting;

        private int CurrentHover { get; set; }
        private int CurrentHoverClear { get; set; }
        private bool ShouldReverseSorting { get; set; }

        private SolidBrush hover_brush;
        private SolidBrush clear_brush;
        private SolidBrush selected_brush;

        private UserlistPopup popup = new UserlistPopup();
        private Bitmap def_av = null;

        public Userlist()
        {
            using (MemoryStream ms = new MemoryStream(Avatar.def_av))
                this.def_av = new Bitmap(ms);

            this.CurrentHover = -1;
            this.CurrentHoverClear = -1;
            this.ShouldReverseSorting = false;

            this.black_background = Settings.black_background;
            this.BackColor = this.black_background ? Color.Black : Color.White;
            this.hover_brush = new SolidBrush(this.black_background ? Color.DimGray : Color.Silver);
            this.clear_brush = new SolidBrush(this.black_background ? Color.Black : Color.White);
            this.selected_brush = new SolidBrush(this.black_background ? Color.SeaGreen : Color.CornflowerBlue);

            this.Columns.Add(new ColumnHeader());

            if (this.Columns.Count > 0) // IDE would get confused :-/
            {
                this.Columns[0].Width = this.Width - 28;
                this.Columns[0].Text = "Users";
            }

            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.SmallImageList = new ImageList();
            this.SmallImageList.ImageSize = new Size(56, 56);

            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Font = this.Font;
            this.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuOpening);
            this.ContextMenuStrip.Items.Add("Options for: "); // 0
            this.ContextMenuStrip.Items[0].Enabled = false;
            this.ContextMenuStrip.Items.Add(new ToolStripSeparator()); // 1
            this.ContextMenuStrip.Items.Add("Direct Chat"); // 2
            this.ContextMenuStrip.Items[2].Click += new EventHandler(this.OnDCRequesting);
            this.ContextMenuStrip.Items.Add("Nudge!"); // 3
            this.ContextMenuStrip.Items[3].Click += new EventHandler(this.NudgeRequested);
            this.ContextMenuStrip.Items.Add("Whois"); // 4
            this.ContextMenuStrip.Items[4].Click += new EventHandler(this.WhoisRequested);
            this.ContextMenuStrip.Items.Add("Ignore/Unignore"); // 5
            this.ContextMenuStrip.Items[5].Click += new EventHandler(this.IgnoreRequested);
            this.ContextMenuStrip.Items.Add("Voice Ignore/Unignore"); // 5
            this.ContextMenuStrip.Items[6].Click += new EventHandler(this.VCIgnoreRequested);
            this.ContextMenuStrip.Items.Add("Browse Files"); // 6

            ToolStripMenuItem t = (ToolStripMenuItem)this.ContextMenuStrip.Items[7];
            t.DropDown.Opacity = 0.9;
            t.DropDown.ForeColor = Color.Black;
            t.DropDown.Renderer = new ContextMenuRenderer();
            t.DropDown.Items.Add("All"); // 0
            t.DropDown.Items[0].Click += new EventHandler(this.OnFileBrowse_All);
            t.DropDown.Items.Add(new ToolStripSeparator()); // 1
            t.DropDown.Items.Add("Audio"); // 2
            t.DropDown.Items[2].Click += new EventHandler(this.OnFileBrowse_Audio);
            t.DropDown.Items.Add("Video"); // 3
            t.DropDown.Items[3].Click += new EventHandler(this.OnFileBrowse_Video);
            t.DropDown.Items.Add("Image"); // 4
            t.DropDown.Items[4].Click += new EventHandler(this.OnFileBrowse_Image);
            t.DropDown.Items.Add("Document"); // 5
            t.DropDown.Items[5].Click += new EventHandler(this.OnFileBrowse_Document);
            t.DropDown.Items.Add("Software"); // 6
            t.DropDown.Items[6].Click += new EventHandler(this.OnFileBrowse_Software);
            t.DropDown.Items.Add("Other"); // 7
            t.DropDown.Items[7].Click += new EventHandler(this.OnFileBrowse_Other);

            this.ContextMenuStrip.Items.Add("Scribble"); // 18
            this.ContextMenuStrip.Items[8].Click += new EventHandler(this.OnScribbleRequest);
            this.ContextMenuStrip.Items.Add("Copy name"); // 19
            this.ContextMenuStrip.Items[9].Click += new EventHandler(this.OnCopyNameRequest);



            this.ContextMenuStrip.Items.Add(new ToolStripSeparator()); // 10
            this.ContextMenuStrip.Items.Add("Kill"); // 8
            this.ContextMenuStrip.Items[11].Click += new EventHandler(this.OnKillRequested);
            this.ContextMenuStrip.Items.Add("Ban"); // 9
            this.ContextMenuStrip.Items[12].Click += new EventHandler(this.OnBanRequested);
            this.ContextMenuStrip.Items.Add("Muzzle"); // 10
            this.ContextMenuStrip.Items[13].Click += new EventHandler(this.OnMuzzleRequested);
            this.ContextMenuStrip.Items.Add("Unmuzzle"); // 11
            this.ContextMenuStrip.Items[14].Click += new EventHandler(this.OnUnmuzzleRequested);

            this.ContextMenuStrip.Items.Add(new ToolStripSeparator()); // 15
            this.ContextMenuStrip.Items.Add("Host Kill"); // 13
            this.ContextMenuStrip.Items[16].Click += new EventHandler(this.OnHostKillRequested);
            this.ContextMenuStrip.Items.Add("Host Ban"); // 14
            this.ContextMenuStrip.Items[17].Click += new EventHandler(this.OnHostBanRequested);
            this.ContextMenuStrip.Items.Add("Host Muzzle"); // 15
            this.ContextMenuStrip.Items[18].Click += new EventHandler(this.OnHostMuzzleRequested);
            this.ContextMenuStrip.Items.Add("Host Unmuzzle"); // 16
            this.ContextMenuStrip.Items[19].Click += new EventHandler(this.OnHostUnmuzzleRequested);

            this.ContextMenuStrip.Opacity = 0.9;
            this.ContextMenuStrip.ForeColor = Color.Black;
            this.ContextMenuStrip.Renderer = new ContextMenuRenderer();
        }

        public void UserListUpdateUserCount()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new ClearListDelegate(this.UserListUpdateUserCount));
            else
                this.Columns[0].Text = "users (" + this.UserPool.Count + ")";
        }

        private void OnMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                UserObject u = this.UserPool.Find(delegate(UserObject x) { return x.name == ((UserlistItem)this.SelectedItems[0]).UserName; });

                this.ContextMenuStrip.Items[0].Text = "Options for: " + u.name;
                this.ContextMenuStrip.Items[2].Visible = Settings.dc_enabled;
                this.ContextMenuStrip.Items[3].Visible = Settings.send_custom_data;
                this.ContextMenuStrip.Items[8].Visible = Settings.send_custom_data;

                if (!this.i_am_admin)
                {
                    this.ContextMenuStrip.Items[10].Visible = false;
                    this.ContextMenuStrip.Items[11].Visible = false;
                    this.ContextMenuStrip.Items[12].Visible = false;
                    this.ContextMenuStrip.Items[13].Visible = false;
                    this.ContextMenuStrip.Items[14].Visible = false;
                }
                else
                {
                    this.ContextMenuStrip.Items[10].Visible = true;
                    this.ContextMenuStrip.Items[11].Visible = true;
                    this.ContextMenuStrip.Items[12].Visible = true;
                    this.ContextMenuStrip.Items[13].Visible = true;
                    this.ContextMenuStrip.Items[14].Visible = true;
                }

                this.ContextMenuStrip.Items[5].Visible = u.level == 0;

                if (u.browse && u.files > 0)
                    this.ContextMenuStrip.Items[7].Visible = true;
                else
                    this.ContextMenuStrip.Items[7].Visible = false;

                if (!this.i_am_host)
                {
                    this.ContextMenuStrip.Items[15].Visible = false;
                    this.ContextMenuStrip.Items[16].Visible = false;
                    this.ContextMenuStrip.Items[17].Visible = false;
                    this.ContextMenuStrip.Items[18].Visible = false;
                    this.ContextMenuStrip.Items[19].Visible = false;
                }
                else
                {
                    this.ContextMenuStrip.Items[15].Visible = true;
                    this.ContextMenuStrip.Items[16].Visible = true;
                    this.ContextMenuStrip.Items[17].Visible = true;
                    this.ContextMenuStrip.Items[18].Visible = true;
                    this.ContextMenuStrip.Items[19].Visible = true;
                }

                while (this.ContextMenuStrip.Items.Count > 20)
                    this.ContextMenuStrip.Items.RemoveAt(20);

                MenuOptionObject[] menuoptions = MenuOptions.GetItems();

                if (menuoptions.Length > 0)
                {
                    this.ContextMenuStrip.Items.Add(new ToolStripSeparator());

                    foreach (MenuOptionObject obj in menuoptions)
                    {
                        this.ContextMenuStrip.Items.Add(obj.name);
                        this.ContextMenuStrip.Items[this.ContextMenuStrip.Items.Count - 1].Click += new EventHandler(this.CustomOptionClicked);
                        this.ContextMenuStrip.Items[this.ContextMenuStrip.Items.Count - 1].Tag = new CustomOption(u, obj.text);
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void CustomOptionClicked(object sender, EventArgs e)
        {
            CustomOption custom = (CustomOption)((ToolStripItem)sender).Tag;
            String str = custom.text;

            if (custom.userobj != null)
            {
                str = str.Replace("+name", custom.userobj.name);
                str = str.Replace("+lip", custom.userobj.localIp.ToString());
                str = str.Replace("+eip", custom.userobj.externalIp.ToString());
                str = str.Replace("+port", custom.userobj.dcPort.ToString());
            }

            if (str.StartsWith("/me "))
                this.OnAdminCommandRequested(Packets.EmotePacket(str.Substring(4)));
            else if (str.StartsWith("/"))
                this.OnAdminCommandRequested(Packets.CommandPacket(str.Substring(1)));
            else
                this.OnAdminCommandRequested(Packets.TextPacket(str));
        }

        public UserObject GetCurrentSelection()
        {
            if (this.SelectedItems.Count > 0)
                return this.UserPool.Find(delegate(UserObject x) { return x.name == ((UserlistItem)this.SelectedItems[0]).UserName; });

            return null;
        }

        private void OnCopyNameRequest(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnCopyNameRequesting(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void OnScribbleRequest(object sender, EventArgs e) // 0
        {
            if (this.SelectedItems.Count > 0)
                this.OnScribbleRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void OnFileBrowse_All(object sender, EventArgs e) // 0
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 0);
        }

        private void OnFileBrowse_Audio(object sender, EventArgs e) // 1
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 1);
        }

        private void OnFileBrowse_Video(object sender, EventArgs e) // 5
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 5);
        }

        private void OnFileBrowse_Image(object sender, EventArgs e) // 7
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 7);
        }

        private void OnFileBrowse_Document(object sender, EventArgs e) // 6
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 6);
        }

        private void OnFileBrowse_Software(object sender, EventArgs e) // 3
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 3);
        }

        private void OnFileBrowse_Other(object sender, EventArgs e) // 8
        {
            if (this.SelectedItems.Count > 0)
                this.OnFileBrowseRequested(((UserlistItem)this.SelectedItems[0]).UserName, 8);
        }

        private void OnKillRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("kill " + ((UserlistItem)this.SelectedItems[0]).UserName));
        }

        private void OnBanRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("ban " + ((UserlistItem)this.SelectedItems[0]).UserName));
        }

        private void OnMuzzleRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("muzzle " + ((UserlistItem)this.SelectedItems[0]).UserName));
        }

        private void OnUnmuzzleRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("unmuzzle " + ((UserlistItem)this.SelectedItems[0]).UserName));
        }

        private void OnHostKillRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("hostkill \"" + ((UserlistItem)this.SelectedItems[0]).UserName + "\""));
        }

        private void OnHostBanRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("hostban \"" + ((UserlistItem)this.SelectedItems[0]).UserName + "\""));
        }

        private void OnHostMuzzleRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("hostmuzzle \"" + ((UserlistItem)this.SelectedItems[0]).UserName + "\""));
        }

        private void OnHostUnmuzzleRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnAdminCommandRequested(Packets.CommandPacket("hostunmuzzle \"" + ((UserlistItem)this.SelectedItems[0]).UserName + "\""));
        }

        private void OnDCRequesting(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnDCRequest(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void NudgeRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnNudgeRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void IgnoreRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnIgnoreRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void VCIgnoreRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnVCIgnoreRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        private void WhoisRequested(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnWhoisRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        public void AddUser(UserObject userobj)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UserObjectDelegate(this.AddUser), userobj);
            }
            else
            {
                this.UserPool.Add(userobj);
                this.Items.Add(new UserlistItem(userobj.name));

                if (userobj.me)
                {
                    this.i_am_admin = userobj.level > 0;
                    this.i_am_host = userobj.level == 3;
                }
            }
        }

        public void RemoveUser(UserObject userobj)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UserObjectDelegate(this.RemoveUser), userobj);
            }
            else
            {
                this.UserPool.RemoveAll(delegate(UserObject x) { return x.name == userobj.name; });

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (((UserlistItem)this.Items[i]).UserName == userobj.name)
                    {
                        this.Items.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public void ClearList()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new ClearListDelegate(this.ClearList));
            else
            {
                this.UserPool.Clear();
                this.Items.Clear();
            }
        }

        public void SetUpdateMode(bool updating)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetUpdateDelegate(this.SetUpdateMode), updating);
            }
            else
            {
                this.is_updating = updating;

                if (updating)
                    this.BeginUpdate();
                else
                    this.EndUpdate();
            }
        }

        private delegate void UpdateUserDelegate(UserObject userobj, bool avatar_updated, bool has_avatar, bool had_avatar);
        public void UpdateUser(UserObject userobj, bool avatar_updated, bool has_avatar, bool had_avatar)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUserDelegate(this.UpdateUser), userobj, avatar_updated, has_avatar, had_avatar);
            }
            else
            {
                UserObject this_user = this.UserPool.Find(delegate(UserObject x) { return x.name == userobj.name; });

                if (this_user != null)
                {
                    if (userobj.me)
                    {
                        this.i_am_admin = userobj.level > 0;
                        this.i_am_host = userobj.level == 3;
                    }

                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        if (((UserlistItem)this.Items[i]).UserName == userobj.name)
                        {
                            this.Invalidate(this.Items[i].Bounds);
                            break;
                        }
                    }
                }
            }
        }

        private bool sort_reverse = false;

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            List<UserlistItem> list = new List<UserlistItem>();

            for (int i = 0; i < this.Items.Count; i++)
                list.Add((UserlistItem)this.Items[i]);

            list.Sort(delegate(UserlistItem x, UserlistItem y) { return String.Compare(x.UserName, y.UserName); });

            if (!this.sort_reverse)
                this.sort_reverse = true;
            else
            {
                this.sort_reverse = false;
                list.Reverse();
            }

            this.BeginUpdate();
            this.Items.Clear();
            this.Items.AddRange(list.ToArray());
            this.EndUpdate();
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Columns.Count > 0)
                this.Columns[0].Width = this.Width - 28;
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(this.selected_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
            }
            else
            {
                if (e.Item.Index == this.CurrentHover)
                    e.Graphics.FillRectangle(this.hover_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
                else if (e.Item.Index == this.CurrentHoverClear)
                {
                    e.Graphics.FillRectangle(this.clear_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
                    this.CurrentHoverClear = -1;
                }
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            String user = ((UserlistItem)e.Item).UserName;
            ((UserlistItem)e.Item).PaintItem(this.UserPool.Find(delegate(UserObject x) { return x.name == user; }), e, this.CurrentHover, this.black_background, ref this.def_av);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.SelectedIndices.Count > 0)
                this.Invalidate(this.Items[this.SelectedIndices[0]].Bounds);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnPMRequested(((UserlistItem)this.SelectedItems[0]).UserName);
        }

        protected override void OnMouseMove(MouseEventArgs e) // hot tracking
        {
            base.OnMouseMove(e);

            ListViewItem item = this.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                UserlistItem uli = (UserlistItem)item;
                UserObject uo = this.UserPool.Find(delegate(UserObject uo2) { return uo2.name == uli.UserName; });

                if (uo != null)
                {
                    if (!this.popup.IsUser(uo))
                    {
                        this.popup.User = uo;
                        this.popup.SetToolTip(this, "popup");
                    }
                }

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

        protected override void OnMouseLeave(EventArgs e) // cancel hot tracking
        {
            base.OnMouseLeave(e);

            if (this.CurrentHover > -1)
            {
                int i = this.CurrentHover;
                this.CurrentHover = -1;

                if (this.Items.Count > i)
                    this.Invalidate(this.Items[i].Bounds);
            }

            if (this.popup.Active)
            {
                this.popup.SetToolTip(this, String.Empty);
                this.popup.Reset();
            }
        }

        public void FindUserInList(String name)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                UserlistItem item = (UserlistItem)this.Items[i];

                if (item.UserName.StartsWith(name))
                {
                    this.Items[i].Selected = true;
                    this.EnsureVisible(i);
                    break;
                }
            }
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Cancel = true;
                e.NewWidth = this.Width - 28;
                return;
            }

            base.OnColumnWidthChanging(e);
        }

    }

    class CustomOption
    {
        public UserObject userobj;
        public String text;

        public CustomOption(UserObject userobj, String text)
        {
            this.userobj = userobj;
            this.text = text;
        }
    }
}
