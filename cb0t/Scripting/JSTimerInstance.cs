using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting
{
    class JSTimerInstance
    {
        public String ScriptName { get; set; }
        public UserDefinedFunction Callback { get; set; }
        public int Interval { get; set; }
        public ulong Time { get; set; }
        public int Ident { get; set; }
        public bool Loop { get; set; }
    }
}
