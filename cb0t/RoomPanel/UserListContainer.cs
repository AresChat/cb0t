using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Drawing.Drawing2D;
using System.IO;
using Jurassic.Library;

namespace cb0t
{
    public partial class UserListContainer : UserControl
    {
        public event EventHandler OpenPMRequested;
        public event EventHandler SendAdminCommand;
        public event EventHandler CustomOptionClicked;
        public event EventHandler<ULCTXTaskEventArgs> MenuTask;

        private ContextMenuStrip ctx_menu { get; set; }
        private String CTXUserName { get; set; }

        public UserListContainer()
        {
            this.InitializeComponent();
            this.userListBox1.MouseDoubleClick += this.ItemMouseDoubleClick;
            this.ctx_menu = new ContextMenuStrip();
            this.ctx_menu.Items.Add(new ToolStripLabel());
            this.ctx_menu.Items[0].Enabled = false;
            this.ctx_menu.Items.Add(new ToolStripSeparator());//1
            this.ctx_menu.Items.Add("Nudge");//2
            this.ctx_menu.Items.Add("Whois");//3
            this.ctx_menu.Items.Add("Ignore/Unignore");//4
            this.ctx_menu.Items.Add("Copy name to clipboard");//5
            this.ctx_menu.Items.Add("Add/Remove friend");//6
            this.ctx_menu.Items.Add("Browse");//7
            this.ctx_menu.Items.Add(new ToolStripSeparator());//8
            this.ctx_menu.Items.Add("Kill");//9
            this.ctx_menu.Items.Add("Ban");//10
            this.ctx_menu.Items.Add("Muzzle");//11
            this.ctx_menu.Items.Add("Unmuzzle");//12
            this.ctx_menu.Items.Add(new ToolStripSeparator());//13
            this.ctx_menu.Items.Add("Host kill");//14
            this.ctx_menu.Items.Add("Host ban");//15
            this.ctx_menu.Items.Add("Host muzzle");//16
            this.ctx_menu.Items.Add("Host unmuzzle");//17
            this.ctx_menu.Items.Add(new ToolStripSeparator()); // 18
            this.ctx_menu.Items[18].Visible = false;
            this.ctx_menu.ShowImageMargin = false;
            this.ctx_menu.Opening += this.CTXMenuOpening;
            this.ctx_menu.ItemClicked += this.CTXItemClicked;
            this.userListBox1.ContextMenuStrip = this.ctx_menu;
        }

        public void UpdateTemplate()
        {
            this.ctx_menu.Items[2].Text = StringTemplate.Get(STType.UserList, 1);
            this.ctx_menu.Items[3].Text = StringTemplate.Get(STType.UserList, 2);
            this.ctx_menu.Items[4].Text = StringTemplate.Get(STType.UserList, 3);
            this.ctx_menu.Items[5].Text = StringTemplate.Get(STType.UserList, 4);
            this.ctx_menu.Items[6].Text = StringTemplate.Get(STType.UserList, 5);
            this.ctx_menu.Items[7].Text = StringTemplate.Get(STType.UserList, 6);
            this.ctx_menu.Items[9].Text = StringTemplate.Get(STType.UserList, 7);
            this.ctx_menu.Items[10].Text = StringTemplate.Get(STType.UserList, 8);
            this.ctx_menu.Items[11].Text = StringTemplate.Get(STType.UserList, 9);
            this.ctx_menu.Items[12].Text = StringTemplate.Get(STType.UserList, 10);
            this.ctx_menu.Items[14].Text = StringTemplate.Get(STType.UserList, 11);
            this.ctx_menu.Items[15].Text = StringTemplate.Get(STType.UserList, 12);
            this.ctx_menu.Items[16].Text = StringTemplate.Get(STType.UserList, 13);
            this.ctx_menu.Items[17].Text = StringTemplate.Get(STType.UserList, 14);
            this.userListHeader1.UpdateTemplate();
            this.userListBox1.Invalidate();
        }

        public void SetBlack()
        {
            this.userListBox1.BackColor = Color.Black;
            this.userListBox1.IsBlack = true;
            this.userListBox1.UpdateBlackImgs();
        }

        private void CTXItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.CTXUserName))
                if (e.ClickedItem != null)
                {
                    if (e.ClickedItem.Equals(this.ctx_menu.Items[2]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.Nudge));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[3]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.Whois));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[4]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.IgnoreUnignore));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[5]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.CopyName));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[6]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.AddRemoveFriend));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[7]))
                        this.MenuTask(this.CTXUserName, new ULCTXTaskEventArgs(ULCTXTask.Browse));
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[9]))
                        this.SendAdminCommand("kill " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[10]))
                        this.SendAdminCommand("ban " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[11]))
                        this.SendAdminCommand("muzzle " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[12]))
                        this.SendAdminCommand("unmuzzle " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[14]))
                        this.SendAdminCommand("hostkill " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[15]))
                        this.SendAdminCommand("hostban " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[16]))
                        this.SendAdminCommand("hostmuzzle " + this.CTXUserName, EventArgs.Empty);
                    else if (e.ClickedItem.Equals(this.ctx_menu.Items[17]))
                        this.SendAdminCommand("hostunmuzzle " + this.CTXUserName, EventArgs.Empty);
                    else
                    {
                        for (int i = 19; i < this.ctx_menu.Items.Count; i++)
                            if (e.ClickedItem.Equals(this.ctx_menu.Items[i]))
                            {
                                int index = (i - 19);

                                if (index >= 0 && index < Menus.UserList.Count)
                                {
                                    String ctext = Menus.UserList[index].Text.Replace("+n", this.CTXUserName);
                                    this.CustomOptionClicked(ctext, EventArgs.Empty);
                                }
                                else
                                {
                                    index -= Menus.UserList.Count;

                                    if (index >= 0 && index < Scripting.ScriptManager.UserListMenuOptions.Count)
                                    {
                                        UserDefinedFunction cback = Scripting.ScriptManager.UserListMenuOptions[index].Callback;
                                        
                                        Scripting.CustomJSUserListMenuCallback js = new Scripting.CustomJSUserListMenuCallback
                                        {
                                            Callback = cback,
                                            Target = this.CTXUserName
                                        };

                                        this.CustomOptionClicked(js, EventArgs.Empty);
                                    }
                                }

                                break;
                            }
                    }
                }
        }

        private void CTXMenuOpening(object sender, CancelEventArgs e)
        {
            int i = this.userListBox1.SelectedIndex;
            bool can_show = false;

            if (i > -1 && i < this.userListBox1.Items.Count)
                if (this.userListBox1.Items[i] is UserListBoxItem)
                {
                    can_show = true;
                    this.CTXUserName = ((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name;
                    this.ctx_menu.Items[0].Text = StringTemplate.Get(STType.UserList, 0) + ": " + this.CTXUserName;
                    this.ctx_menu.Items[7].Visible = ((UserListBoxItem)this.userListBox1.Items[i]).Owner.HasFiles;

                    for (int x = 8; x < 13; x++)
                        this.ctx_menu.Items[x].Visible = this.MyLevel > 0;

                    for (int x = 13; x < 18; x++)
                        this.ctx_menu.Items[x].Visible = this.MyLevel == 3;
                }

            while (this.ctx_menu.Items.Count > 19)
                this.ctx_menu.Items[19].Dispose();

            bool custom_options = false;

            if (Menus.UserList.Count > 0)
            {
                this.ctx_menu.Items[18].Visible = true;
                Menus.UserList.ForEach(x => this.ctx_menu.Items.Add(x.Name));
                custom_options = true;
            }

            if (Scripting.ScriptManager.UserListMenuOptions.Count > 0)
            {
                if (!this.ctx_menu.Items[18].Visible)
                    this.ctx_menu.Items[18].Visible = true;

                Scripting.ScriptManager.UserListMenuOptions.ForEach(x => this.ctx_menu.Items.Add(x.Text));
                custom_options = true;
            }

            if (!custom_options)
                this.ctx_menu.Items[18].Visible = false;

            if (!can_show)
                e.Cancel = true;
        }

        private void ItemMouseDoubleClick(object sender, MouseEventArgs e)
        {
            int i = this.userListBox1.SelectedIndex;

            if (i > -1 && i < this.userListBox1.Items.Count)
                if (this.userListBox1.Items[i] is UserListBoxItem)
                {
                    String name = ((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name;
                    this.OpenPMRequested(name, EventArgs.Empty);
                }
        }

        public int MyLevel { get; set; }

        public void Free()
        {
            this.userListBox1.ContextMenuStrip = null;
            this.ctx_menu.Opening -= this.CTXMenuOpening;
            this.ctx_menu.ItemClicked -= this.CTXItemClicked;

            while (this.ctx_menu.Items.Count > 0)
                this.ctx_menu.Items[0].Dispose();

            this.ctx_menu.Dispose();
            this.ctx_menu = null;
            this.userListBox1.MouseDoubleClick -= this.ItemMouseDoubleClick;

            while (this.Controls.Count > 0)
                this.Controls.RemoveAt(0);

            this.userListHeader1.Free();
            this.userListHeader1.Dispose();
            this.userListHeader1 = null;
            this.userListBox1.Items.Clear();
            this.userListBox1.Free();
            this.userListBox1.Dispose();
            this.userListBox1 = null;
        }

        public void UpdateLag(ulong lag)
        {
            this.userListHeader1.UpdateLag(lag);
        }

        public void SetToUser(String name)
        {
            for (int i = 0; i < this.userListBox1.Items.Count; i++)
                if (this.userListBox1.Items[i] is UserListBoxItem)
                    if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == name)
                    {
                        this.userListBox1.SelectedIndex = i;
                        this.userListBox1.TopIndex = i;
                        break;
                    }
        }

        public String GetSelectedName
        {
            get
            {
                if (this.userListBox1.SelectedItem != null)
                    if (this.userListBox1.SelectedItem is UserListBoxItem)
                        return ((UserListBoxItem)this.userListBox1.SelectedItem).Owner.Name;

                return null;
            }
        }

        public void UpdateServerVersion(String text)
        {
            this.userListHeader1.ServerVersion = text;
        }

        public void AcquireServerIcon(IPEndPoint ep)
        {
            this.userListHeader1.AcquireServerIcon(ep);
        }

        private delegate void CUHandler();
        public void ClearUserList()
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new CUHandler(this.ClearUserList));
            else
            {
                this.userListBox1.Items.Clear();
                this.userListBox1.BeginUpdate();
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Friends));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Friends));
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Admins));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Admins));
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Users));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Users));
            }
        }

        public void ResumeUserlist()
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new CUHandler(this.ResumeUserlist));
            else
                this.userListBox1.EndUpdate();
        }

        public void AddUserItem(User user)
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new RUIHandler(this.AddUserItem), user);
            else
            {
                if (user.IsFriend)
                {
                    int start_index = -1, end_index = -1;

                    for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    {
                        if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                            if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Friends)
                                start_index = i;

                        if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                            if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Admins)
                                end_index = i;
                    }

                    if (start_index > -1 && end_index > start_index)
                    {
                        if (this.userListBox1.Items[end_index - 1] is UserListBoxEmptyItem)
                        {
                            int ti = this.userListBox1.TopIndex;
                            this.userListBox1.Items.RemoveAt(end_index - 1);
                            this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                            this.userListBox1.Items.Insert((end_index - 1), new UserListBoxItem(user));
                        }
                        else this.userListBox1.Items.Insert(end_index, new UserListBoxItem(user));
                    }
                }
                else if (user.Level > 0)
                {
                    int start_index = -1, end_index = -1;

                    for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    {
                        if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                            if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Admins)
                                start_index = i;

                        if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                            if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Users)
                                end_index = i;
                    }

                    if (start_index > -1 && end_index > start_index)
                    {
                        if (this.userListBox1.Items[end_index - 1] is UserListBoxEmptyItem)
                        {
                            int ti = this.userListBox1.TopIndex;
                            this.userListBox1.Items.RemoveAt(end_index - 1);
                            this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                            this.userListBox1.Items.Insert((end_index - 1), new UserListBoxItem(user));
                        }
                        else this.userListBox1.Items.Insert(end_index, new UserListBoxItem(user));

                        this.userListBox1.SortAdmins();
                    }
                }
                else
                {
                    int ti = this.userListBox1.TopIndex;

                    if (this.userListBox1.Items[this.userListBox1.Items.Count - 1] is UserListBoxEmptyItem)
                        this.userListBox1.Items.RemoveAt(this.userListBox1.Items.Count - 1);

                    this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                    this.userListBox1.Items.Add(new UserListBoxItem(user));
                }

                int total_count = 0;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                        total_count++;

                this.userListHeader1.HeaderText = StringTemplate.Get(STType.UserList, 15) + " (" + total_count + ")";
                this.userListHeader1.Invalidate();
            }
        }

        private void CheckSectionEmpty(UserListBoxSectionType section)
        {
            if (section == UserListBoxSectionType.Friends)
            {
                int start_index = -1, end_index = -1;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                {
                    if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Friends)
                            start_index = i;

                    if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Admins)
                            end_index = i;
                }

                if (start_index > -1)
                    if (end_index < (start_index + 2))
                        this.userListBox1.Items.Insert((start_index + 1), new UserListBoxEmptyItem(UserListBoxSectionType.Friends));
            }
            else if (section == UserListBoxSectionType.Admins)
            {
                int start_index = -1, end_index = -1;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                {
                    if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Admins)
                            start_index = i;

                    if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        if (((UserListBoxSectionItem)this.userListBox1.Items[i]).Section == UserListBoxSectionType.Users)
                            end_index = i;
                }

                if (start_index > -1)
                    if (end_index < (start_index + 2))
                        this.userListBox1.Items.Insert((start_index + 1), new UserListBoxEmptyItem(UserListBoxSectionType.Admins));
            }
            else if (this.userListBox1.Items.Count > 0)
            {
                if (!(this.userListBox1.Items[this.userListBox1.Items.Count - 1] is UserListBoxItem))
                    this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Users));
            }
        }

        public void UpdateUserAppearance(User user)
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new RUIHandler(this.UpdateUserAppearance), user);
            else
            {
                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                        if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == user.Name)
                        {
                            this.userListBox1.Invalidate(this.userListBox1.GetItemRectangle(i));
                            break;
                        }
            }
        }

        private delegate void UULHandler(User user, byte before);
        public void UpdateUserLevel(User user, byte before)
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new UULHandler(this.UpdateUserLevel), user, before);
            else
            {
                bool section_changing = ((before == 0 && user.Level > 0) || (before > 0 && user.Level == 0)) && !user.IsFriend;
                int index = -1;
                UserListBoxSectionType section = UserListBoxSectionType.Friends;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                    {
                        if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == user.Name)
                        {
                            index = i;
                            break;
                        }
                    }
                    else if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        section = ((UserListBoxSectionItem)this.userListBox1.Items[i]).Section;

                if (index > -1)
                {
                    UserListBoxItem item = (UserListBoxItem)this.userListBox1.Items[index];

                    if (section_changing)
                    {
                        int ti = this.userListBox1.TopIndex;
                        this.userListBox1.Items.RemoveAt(index);
                        this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                        ti = this.userListBox1.TopIndex;
                        this.CheckSectionEmpty(section);
                        this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                        this.AddUserItem(user);
                    }
                    else this.userListBox1.Invalidate(this.userListBox1.GetItemRectangle(index));
                }
            }
        }

        public void UpdateUserFriendship(User user)
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new RUIHandler(this.UpdateUserFriendship), user);
            else
            {
                int index = -1;
                UserListBoxSectionType section = UserListBoxSectionType.Friends;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                    {
                        if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == user.Name)
                        {
                            index = i;
                            break;
                        }
                    }
                    else if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        section = ((UserListBoxSectionItem)this.userListBox1.Items[i]).Section;

                if (index > -1)
                {
                    UserListBoxItem item = (UserListBoxItem)this.userListBox1.Items[index];
                    int ti = this.userListBox1.TopIndex;
                    this.userListBox1.Items.RemoveAt(index);
                    this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                    ti = this.userListBox1.TopIndex;
                    this.CheckSectionEmpty(section);
                    this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                    this.AddUserItem(user);
                }
            }
        }

        private delegate void RUIHandler(User u);
        public void RemoveUserItem(User user)
        {
            if (this.userListBox1.InvokeRequired)
                this.userListBox1.BeginInvoke(new RUIHandler(this.RemoveUserItem), user);
            else
            {
                int index = -1;
                UserListBoxSectionType section = UserListBoxSectionType.Friends;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                    {
                        if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == user.Name)
                        {
                            index = i;
                            break;
                        }
                    }
                    else if (this.userListBox1.Items[i] is UserListBoxSectionItem)
                        section = ((UserListBoxSectionItem)this.userListBox1.Items[i]).Section;

                if (index > -1)
                {
                    int ti = this.userListBox1.TopIndex;
                    this.userListBox1.OnItemRemoved(index);
                    this.userListBox1.Items.RemoveAt(index);
                    this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                    ti = this.userListBox1.TopIndex;
                    this.CheckSectionEmpty(section);
                    this.userListBox1.TopIndex = ti < this.userListBox1.Items.Count ? ti : (this.userListBox1.Items.Count - 1);
                }

                int total_count = 0;

                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                        total_count++;

                this.userListHeader1.HeaderText = StringTemplate.Get(STType.UserList, 15) + " (" + total_count + ")";
                this.userListHeader1.Invalidate();
            }
        }

        public void SetCrypto(bool is_crypto)
        {
            this.userListHeader1.SetCrypto(is_crypto);
        }

        public void HereFavicon(byte[] data)
        {
            this.userListHeader1.HereFavicon(data);
        }
    }
}
