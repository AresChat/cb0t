using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "HttpRequestResult")]
    class JSHttpRequestResult : ClrFunction
    {
        public JSHttpRequestResult(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "HttpRequestResult", new Objects.JSHttpRequestResult(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSHttpRequestResult Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSHttpRequestResult Construct(object a)
        {
            return null;
        }
    }
}
