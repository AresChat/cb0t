using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormEx.JumpListEx
{
    public class JumpListContainer
    {
        private Form Owner { get; set; }
        private Microsoft.WindowsAPICodePack.Taskbar.JumpList JumpListControl { get; set; }
        private bool HandleCreated { get; set; }
        private List<IJumpListItem> Items { get; set; }

        public String ImageResource { get; set; }

        public JumpListContainer(Form form)
        {
            this.Items = new List<IJumpListItem>();
            this.Owner = form;
            this.Owner.Shown += this.FormCreated;
        }

        public void Add(IJumpListItem jumplist_item)
        {
            this.Items.Add(jumplist_item);

            if (this.HandleCreated)
            {
                this.JumpListControl.ClearAllUserTasks();

                foreach (IJumpListItem item in this.Items)
                    this.JumpListControl.AddUserTasks(item.ToJumpListTask());

                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }
        }

        public void Insert(int index, IJumpListItem jumplist_item)
        {
            this.Items.Insert(index, jumplist_item);

            if (this.HandleCreated)
            {
                this.JumpListControl.ClearAllUserTasks();

                foreach (IJumpListItem item in this.Items)
                    this.JumpListControl.AddUserTasks(item.ToJumpListTask());

                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }
        }

        public void Add(IJumpListItem jumplist_item, int image_index)
        {
            this.Items.Add(jumplist_item);

            if (jumplist_item.Type == JumpListItemType.Item)
                if (this.ImageResource != null)
                    ((JumpListItem)jumplist_item).IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference(this.ImageResource, image_index);

            if (this.HandleCreated)
            {
                this.JumpListControl.ClearAllUserTasks();

                foreach (IJumpListItem item in this.Items)
                    this.JumpListControl.AddUserTasks(item.ToJumpListTask());

                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }
        }

        public void Insert(int index, IJumpListItem jumplist_item, int image_index)
        {
            this.Items.Insert(index, jumplist_item);

            if (jumplist_item.Type == JumpListItemType.Item)
                if (this.ImageResource != null)
                    ((JumpListItem)jumplist_item).IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference(this.ImageResource, image_index);

            if (this.HandleCreated)
            {
                this.JumpListControl.ClearAllUserTasks();

                foreach (IJumpListItem item in this.Items)
                    this.JumpListControl.AddUserTasks(item.ToJumpListTask());

                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }
        }

        public void RemoveAt(int index)
        {
            if (this.HandleCreated)
                this.JumpListControl.ClearAllUserTasks();

            IJumpListItem old = this.Items[index];
            this.Items.RemoveAt(index);
            old.Release();

            if (this.HandleCreated)
            {
                foreach (IJumpListItem item in this.Items)
                    this.JumpListControl.AddUserTasks(item.ToJumpListTask());

                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }
        }

        public void Clear()
        {
            if (this.HandleCreated)
            {
                this.JumpListControl.ClearAllUserTasks();
                this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
                this.JumpListControl.Refresh();
            }

            this.Items.ForEach(x => x.Release());
            this.Items.Clear();
        }

        private void FormCreated(object sender, EventArgs e)
        {
            this.HandleCreated = true;
            this.Owner.Shown -= this.FormCreated;
            this.JumpListControl = Microsoft.WindowsAPICodePack.Taskbar.JumpList.CreateJumpList();
            this.JumpListControl.ClearAllUserTasks();

            foreach (IJumpListItem item in this.Items)
                this.JumpListControl.AddUserTasks(item.ToJumpListTask());

            this.JumpListControl.KnownCategoryToDisplay = Microsoft.WindowsAPICodePack.Taskbar.JumpListKnownCategoryType.Neither;
            this.JumpListControl.Refresh();
        }
    }
}
