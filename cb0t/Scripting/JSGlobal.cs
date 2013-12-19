using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t.Scripting
{
    class JSGlobal
    {
        [JSFunction(Name = "alert", Flags = JSFunctionFlags.HasEngineParameter)]
        public static void Alert(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String text = a.ToString();
                MessageBox.Show(text, eng.ScriptName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [JSFunction(Name = "confirm", Flags = JSFunctionFlags.HasEngineParameter)]
        public static bool Confirm(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String text = a.ToString();
                DialogResult result = MessageBox.Show(text, eng.ScriptName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                    return true;
            }

            return false;
        }

        [JSFunction(Name = "getControlById", Flags = JSFunctionFlags.HasEngineParameter)]
        public static object GetControlByID(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String id = a.ToString();

                if (!String.IsNullOrEmpty(id))
                {
                    JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                    if (script != null)
                        foreach (ICustomUI item in script.Elements)
                            if (item.ID == id)
                                return item;
                }
            }

            return null;
        }
    }
}
