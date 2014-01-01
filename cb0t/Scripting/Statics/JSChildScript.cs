using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "ChildScript")]
    class JSChildScript : ObjectInstance
    {
        public JSChildScript(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "ChildScript"; }
        }

        [JSFunction(Name = "eval", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static String C_Eval(ScriptEngine eng, object a)
        {
            String name = eng.ScriptName;
            name = Path.GetFileNameWithoutExtension(name);
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == name);
            String result = null;

            if (!(a is Undefined))
                if (script != null)
                {
                    String src = a.ToString();
                    result = script.Eval(src);
                }

            return result;
        }

        [JSFunction(Name = "reset", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void C_Reset(ScriptEngine eng)
        {
            String name = eng.ScriptName;
            name = Path.GetFileNameWithoutExtension(name);
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == name);

            if (script != null)
                script.ResetScript();
        }
    }
}
