using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting
{
    class CustomJSMenuOption
    {
        public UserDefinedFunction Callback { get; set; }
        public String Text { get; set; }
        public bool IsChecked { get; set; }
        public bool CanCheck { get; set; }
    }
}
