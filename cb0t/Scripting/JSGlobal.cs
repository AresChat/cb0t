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
                            if (!String.IsNullOrEmpty(item.ID))
                                if (item.ID == id)
                                    return item;
                }
            }

            return null;
        }

        [JSFunction(Name = "user")]
        public static Objects.JSUser GetUser(object a, object b)
        {
            if (a is Objects.JSRoom)
                if (!(b is Undefined))
                {
                    Objects.JSRoom r = (Objects.JSRoom)a;
                    String str = b.ToString();

                    if (!String.IsNullOrEmpty(str))
                    {
                        foreach (Objects.JSUser u in r.UserList)
                            if (u.U_Name == str)
                                return u;

                        foreach (Objects.JSUser u in r.UserList)
                            if (u.U_Name.StartsWith(str))
                                return u;
                    }
                }

            return null;
        }

        [JSFunction(Name = "users")]
        public static void DoUsersTask(object a, object b)
        {
            if (a is Objects.JSRoom && b is UserDefinedFunction)
            {
                Objects.JSRoom r = (Objects.JSRoom)a;
                UserDefinedFunction f = (UserDefinedFunction)b;

                foreach (Objects.JSUser u in r.UserList)
                    try { f.Call(r.Engine.Global, u); }
                    catch { }
            }
        }

        [JSFunction(Name = "room", Flags = JSFunctionFlags.HasEngineParameter)]
        public static Objects.JSRoom GetRoom(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                    if (script != null)
                    {
                        foreach (Objects.JSRoom r in script.Rooms)
                            if (r.R_Name == str)
                                return r;

                        foreach (Objects.JSRoom r in script.Rooms)
                            if (r.R_Name.StartsWith(str))
                                return r;
                    }
                }
            }

            return null;
        }

        [JSFunction(Name = "rooms", Flags = JSFunctionFlags.HasEngineParameter)]
        public static void DoRoomsTask(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                UserDefinedFunction f = (UserDefinedFunction)a;
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                    foreach (Objects.JSRoom r in script.Rooms)
                        try { f.Call(script.JS.Global, r); }
                        catch { }
            }
        }
    }
}
