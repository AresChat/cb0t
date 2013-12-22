using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UIImage")]
    class JSUIImage : ClrFunction
    {
        public JSUIImage(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UIImage", new Objects.JSUIImage(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIImage Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUIImage Construct(object a)
        {
            return null;
        }
    }
}
