using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public class ULCTXTaskEventArgs : EventArgs
    {
        public ULCTXTask Task { get; private set; }

        public ULCTXTaskEventArgs(ULCTXTask t)
        {
            this.Task = t;
        }
    }

    public enum ULCTXTask
    {
        Nudge,
        Whois,
        IgnoreUnignore,
        CopyName,
        AddRemoveFriend,
        Browse
    }
}
