using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "RegistryKeyCollection")]
    class JSRegistryKeyCollection : ClrFunction
    {
        public JSRegistryKeyCollection(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "RegistryKeyCollection", new Objects.JSRegistryKeyCollection(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRegistryKeyCollection Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRegistryKeyCollection Construct(object a)
        {
            return null;
        }
    }
}
