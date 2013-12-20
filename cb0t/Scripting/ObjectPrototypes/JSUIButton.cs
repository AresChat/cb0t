using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIButton")]
    class JSUIButton : ClrFunction
    {
        public JSUIButton(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIButton", new Objects.JSUIButton(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIButton Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIButton Construct(object a)
        {
            return null;
        }
    }
}
