using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Instances
{
    [JSEmbed(Name = "HttpRequest")]
    class JSHttpRequest : ClrFunction
    {
        public JSHttpRequest(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "HttpRequest", new JSHttpRequestInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSHttpRequestInstance Construct()
        {
            return new JSHttpRequestInstance(this.InstancePrototype);
        }
    }
}
