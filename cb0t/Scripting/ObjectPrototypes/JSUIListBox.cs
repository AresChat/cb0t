using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIListBox")]
    class JSUIListBox : ClrFunction
    {
        public JSUIListBox(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIListBox", new Objects.JSUIListBox(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIListBox Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIListBox Construct(object a)
        {
            return null;
        }
    }
}
