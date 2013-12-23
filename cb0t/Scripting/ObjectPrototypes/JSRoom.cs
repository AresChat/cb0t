using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Room")]
    class JSRoom : ClrFunction
    {
        public JSRoom(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Room", new Objects.JSRoom(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRoom Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRoom Construct(object a)
        {
            return null;
        }
    }
}
