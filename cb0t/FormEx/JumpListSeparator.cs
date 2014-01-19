using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormEx.JumpListEx
{
    class JumpListSeparator : Microsoft.WindowsAPICodePack.Taskbar.JumpListSeparator, IJumpListItem
    {
        public JumpListItemType Type
        {
            get { return JumpListItemType.Separator; }
        }

        public Microsoft.WindowsAPICodePack.Taskbar.JumpListTask ToJumpListTask()
        {
            return (Microsoft.WindowsAPICodePack.Taskbar.JumpListSeparator)this;
        }

        public void Release()
        {
            this.Dispose();
        }
    }
}
