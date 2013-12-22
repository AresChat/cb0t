using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIComboBox")]
    class JSUIComboBox : ClrFunction
    {
        public JSUIComboBox(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIComboBox", new Objects.JSUIComboBox(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIComboBox Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIComboBox Construct(object a)
        {
            return null;
        }
    }
}
