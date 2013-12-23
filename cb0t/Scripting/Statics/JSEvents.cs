using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Events")]
    class JSEvents : ObjectInstance
    {
        public JSEvents(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Events"; }
        }

        [JSFunction(Name = "onload", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnLoad(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONLOAD = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onconnected", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnConnected(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONCONNECTED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserlistreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserListReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERLISTRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }


    }
}
