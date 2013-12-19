using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UITextArea")]
    class JSUITextArea : ClrFunction
    {
        public JSUITextArea(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UITextArea", new Objects.JSUITextArea(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUITextArea Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUITextArea Construct(object a)
        {
            return null;
        }
    }
}
