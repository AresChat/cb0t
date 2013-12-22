using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting
{
    class JSUIEventItem
    {
        public object Arg { get; set; }
        public ICustomUI Element { get; set; }
        public JSUIEventType EventType { get; set; }
    }

    enum JSUIEventType
    {
        KeyPressed,
        ValueChanged,
        Click,
        Select,
        SelectedItemChanged,
        ItemDoubleClick
    }
}
