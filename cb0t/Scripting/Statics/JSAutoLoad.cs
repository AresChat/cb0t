using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "AutoLoad")]
    class JSAutoLoad : ObjectInstance
    {
        public JSAutoLoad(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "AutoLoad"; }
        }

        [JSFunction(Name = "add", IsWritable = false, IsEnumerable = true)]
        public static bool AddAutoLoad(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                    return ScriptManager.AddToAutoLoad(str);
            }

            return false;
        }

        [JSFunction(Name = "remove", IsWritable = false, IsEnumerable = true)]
        public static bool RemAutoLoad(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                    return ScriptManager.RemoveFromAutoLoad(str);
            }

            return false;
        }

        [JSFunction(Name = "scripts", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static ArrayInstance LoadedScripts(ScriptEngine eng)
        {
            String[] org = ScriptManager.AutoLoadedScripts;
            object[] results = new object[org.Length];

            for (int i = 0; i < results.Length; i++)
                results[i] = org[i];

            return eng.Array.New(results);
        }

        [JSFunction(Name = "available", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static ArrayInstance AvailableScripts(ScriptEngine eng)
        {
            String[] org = ScriptManager.AvailableScripts;
            object[] results = new object[org.Length];

            for (int i = 0; i < results.Length; i++)
                results[i] = org[i];

            return eng.Array.New(results);
        }
    }
}
