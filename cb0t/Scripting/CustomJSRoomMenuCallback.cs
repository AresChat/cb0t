using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting
{
    class CustomJSRoomMenuCallback : IScriptingCallback
    {
        public UserDefinedFunction Callback { get; set; }
        public IPEndPoint Room { get; set; }
        public JSCheckState IsChecked { get; set; }
    }

    enum JSCheckState : int
    {
        Unused = -1,
        Unchecked = 0,
        Checked = 1
    }
}
