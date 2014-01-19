using Jurassic;
using Jurassic.Library;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        [JSFunction(Name = "clientVersion")]
        public static String DoClientVersion()
        {
            return Settings.APP_NAME + " " + Settings.APP_VERSION;
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

        [JSFunction(Name = "include", Flags = JSFunctionFlags.HasEngineParameter)]
        public static bool DoInclude(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String file = a.ToString();

                if (!String.IsNullOrEmpty(file))
                {
                    JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                    if (script != null)
                    {
                        file = new String(file.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                        file = Path.Combine(script.ScriptPath, file);

                        if (new FileInfo(file).Directory.FullName != new DirectoryInfo(script.ScriptPath).FullName)
                            return false;

                        try
                        {
                            script.JS.ExecuteFile(file);
                        }
                        catch { }

                        return true;
                    }
                }
            }

            return false;
        }

        [JSFunction(Name = "openBrowser")]
        public static bool DoOpenBrowser(object a)
        {
            if (a is Undefined)
                return false;

            String url = a.ToString();

            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command");

                if (key != null)
                {
                    object value = key.GetValue(String.Empty);

                    if (value is String)
                    {
                        String browser = (String)value;

                        if (browser.StartsWith("\""))
                            browser = browser.Substring(1);

                        int i = browser.IndexOf("\"");

                        if (i > -1)
                            browser = browser.Substring(0, i);

                        try
                        {
                            Process.Start(browser, url);
                            return true;
                        }
                        catch { }
                    }
                }
            }

            return false;
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

        [JSFunction(Name = "scriptName", Flags = JSFunctionFlags.HasEngineParameter)]
        public static String DoScriptName(ScriptEngine eng)
        {
            return eng.ScriptName;
        }

        [JSFunction(Name = "scriptVersion")]
        public static int DoScriptVersion()
        {
            return ScriptManager.SCRIPT_VERSION;
        }
    }
}
