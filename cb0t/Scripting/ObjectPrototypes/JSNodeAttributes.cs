using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "NodeAttributes")]
    class JSNodeAttributes : ClrFunction
    {
        public JSNodeAttributes(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "NodeAttributes", new Objects.JSNodeAttributes(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNodeAttributes Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNodeAttributes Construct(object a)
        {
            return null;
        }
    }
}
