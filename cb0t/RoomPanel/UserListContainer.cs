using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace cb0t
{
    public partial class UserListContainer : UserControl
    {
        public UserListContainer()
        {
            InitializeComponent();
        }

        public void Free()
        {
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

        public void UpdateServerVersion(String text)
        {
            this.userListHeader1.ServerVersion = text;
        }

        public void AcquireServerIcon(IPEndPoint ep)
        {
            this.userListHeader1.AcquireServerIcon(ep);
        }

        public void ClearUserList()
        {
            this.userListBox1.BeginInvoke((Action)(() =>
            {
                this.userListBox1.Items.Clear();
                this.userListBox1.BeginUpdate();
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Friends));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Friends));
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Admins));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Admins));
                this.userListBox1.Items.Add(new UserListBoxSectionItem(UserListBoxSectionType.Users));
                this.userListBox1.Items.Add(new UserListBoxEmptyItem(UserListBoxSectionType.Users));
            }));
        }

        public void ResumeUserlist()
        {
            this.userListBox1.BeginInvoke((Action)(() => this.userListBox1.EndUpdate()));
        }

        public void AddUserItem(User user)
        {
            this.userListBox1.BeginInvoke((Action)(() =>
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

                this.userListHeader1.HeaderText = "Users (" + total_count + ")";
                this.userListHeader1.Invalidate();
            }));
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
            this.userListBox1.BeginInvoke((Action)(() =>
            {
                for (int i = 0; i < this.userListBox1.Items.Count; i++)
                    if (this.userListBox1.Items[i] is UserListBoxItem)
                        if (((UserListBoxItem)this.userListBox1.Items[i]).Owner.Name == user.Name)
                        {
                            this.userListBox1.Invalidate(this.userListBox1.GetItemRectangle(i));
                            break;
                        }
            }));
        }

        public void UpdateUserLevel(User user, byte before)
        {
            this.userListBox1.BeginInvoke((Action)(() =>
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
            }));
        }

        public void UpdateUserFriendship(User user)
        {
            this.userListBox1.BeginInvoke((Action)(() =>
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
            }));
        }

        public void RemoveUserItem(User user)
        {
            this.userListBox1.BeginInvoke((Action)(() =>
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

                this.userListHeader1.HeaderText = "Users (" + total_count + ")";
                this.userListHeader1.Invalidate();
            }));
        }

        public void SetCrypto(bool is_crypto)
        {
            this.userListHeader1.SetCrypto(is_crypto);
        }
    }
}
