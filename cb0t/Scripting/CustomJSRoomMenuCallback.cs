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
    }
}
