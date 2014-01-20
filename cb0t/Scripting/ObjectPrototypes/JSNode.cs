using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Node")]
    class JSNode : ClrFunction
    {
        public JSNode(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Node", new Objects.JSNode(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNode Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNode Construct(object a)
        {
            return null;
        }
    }
}
