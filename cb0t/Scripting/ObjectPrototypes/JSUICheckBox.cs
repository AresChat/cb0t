using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UICheckBox")]
    class JSUICheckBox : ClrFunction
    {
        public JSUICheckBox(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UICheckBox", new Objects.JSUICheckBox(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUICheckBox Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUICheckBox Construct(object a)
        {
            return null;
        }
    }
}
