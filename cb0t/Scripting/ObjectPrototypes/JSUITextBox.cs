using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UITextBox")]
    class JSUITextBox : ClrFunction
    {
        public JSUITextBox(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UITextBox", new Objects.JSUITextBox(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUITextBox Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUITextBox Construct(object a)
        {
            return null;
        }
    }
}
