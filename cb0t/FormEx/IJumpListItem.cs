using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormEx.JumpListEx
{
    public interface IJumpListItem
    {
        JumpListItemType Type { get; }
        Microsoft.WindowsAPICodePack.Taskbar.JumpListTask ToJumpListTask();
        void Release();
    }
}
