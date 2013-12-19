using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UILabel")]
    class JSUILabel : ClrFunction
    {
        public JSUILabel(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UILabel", new Objects.JSUILabel(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUILabel Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUILabel Construct(object a)
        {
            return null;
        }
    }
}
