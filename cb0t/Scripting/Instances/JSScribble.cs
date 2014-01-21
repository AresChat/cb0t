using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Instances
{
    [JSEmbed(Name = "Scribble")]
    class JSScribble : ClrFunction
    {
        public JSScribble(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Scribble", new JSScribbleInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSScribbleInstance Construct()
        {
            return new JSScribbleInstance(this.InstancePrototype);
        }
    }
}
