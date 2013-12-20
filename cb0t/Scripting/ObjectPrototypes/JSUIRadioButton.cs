using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIRadioButton")]
    class JSUIRadioButton : ClrFunction
    {
        public JSUIRadioButton(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIRadioButton", new Objects.JSUIRadioButton(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIRadioButton Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIRadioButton Construct(object a)
        {
            return null;
        }
    }
}
