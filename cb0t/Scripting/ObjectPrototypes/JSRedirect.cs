using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Redirect")]
    class JSRedirect : ClrFunction
    {
        public JSRedirect(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Redirect", new Objects.JSRedirect(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRedirect Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRedirect Construct(object a)
        {
            return null;
        }
    }
}
