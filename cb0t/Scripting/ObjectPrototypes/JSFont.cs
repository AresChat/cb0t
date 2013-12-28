using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Font")]
    class JSFont : ClrFunction
    {
        public JSFont(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Font", new Objects.JSFont(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSFont Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSFont Construct(object a)
        {
            return null;
        }
    }
}
