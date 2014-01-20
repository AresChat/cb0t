using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Instances
{
    [JSEmbed(Name = "XmlParser")]
    class JSXmlParser : ClrFunction
    {
        public JSXmlParser(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "XmlParser", new JSXmlParserInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSXmlParserInstance Construct()
        {
            return new JSXmlParserInstance(this.InstancePrototype);
        }
    }
}
