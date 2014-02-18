using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIGroupBox")]
    class JSUIGroupBox : ClrFunction
    {
        public JSUIGroupBox(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIGroupBox", new Objects.JSUIGroupBox(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIGroupBox Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIGroupBox Construct(object a)
        {
            return null;
        }
    }
}
