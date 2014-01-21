using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.ObjectPrototypes
{
    [JSEmbed(Name = "ScribbleImage")]
    class JSScribbleImage : ClrFunction
    {
        public JSScribbleImage(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "ScribbleImage", new Objects.JSScribbleImage(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSScribbleImage Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSScribbleImage Construct(object a)
        {
            return null;
        }
    }
}
