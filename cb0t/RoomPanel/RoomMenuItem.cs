using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public enum RoomMenuItem
    {
        ExportHashlink,
        AddToFavourites,
        CopyRoomName,
        AutoPlayVoiceClips,
        CloseSubTabs,
        Custom
    }

    public class RoomMenuItemClickedEventArgs : EventArgs
    {
        public RoomMenuItem Item { get; set; }
        public object Arg { get; set; }
    }
}
