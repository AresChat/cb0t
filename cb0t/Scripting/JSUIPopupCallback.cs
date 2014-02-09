using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting
{
    public class JSUIPopupCallback
    {
        public UserDefinedFunction Callback { get; set; }
        public IPEndPoint Room { get; set; }
    }
}
