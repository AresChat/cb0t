using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormEx.JumpListEx
{
    public class JumpListItem : Microsoft.WindowsAPICodePack.Taskbar.JumpListLink, IJumpListItem
    {
        public JumpListItemType Type
        {
            get { return JumpListItemType.Item; }
        }

        public Microsoft.WindowsAPICodePack.Taskbar.JumpListTask ToJumpListTask()
        {
            return (Microsoft.WindowsAPICodePack.Taskbar.JumpListLink)this;
        }

        public JumpListItem(String text)
            : base(JumpListHelpers.ApplicationPath, text)
        {

        }

        public JumpListItem(String text, String ident)
            : base(JumpListHelpers.ApplicationPath, text)
        {
            this.Arguments = ident;
        }

        public void Release()
        {
            this.Dispose();
        }
    }
}
