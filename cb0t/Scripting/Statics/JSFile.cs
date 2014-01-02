using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "File")]
    class JSFile : ObjectInstance
    {
        public JSFile(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "File"; }
        }

        [JSFunction(Name = "append", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool JS_Append(ScriptEngine eng, object a, object b)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
                if (!(a is Undefined) && !(b is Undefined))
                {
                    String path = a.ToString();
                    path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                    path = Path.Combine(script.DataPath, path);

                    if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                        return false;

                    try
                    {
                        using (StreamWriter writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                            writer.Write(b.ToString());

                        return true;
                    }
                    catch { }
                }

            return false;
        }

        [JSFunction(Name = "browse", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void JS_Browse(ScriptEngine eng)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
                if (Directory.Exists(script.DataPath))
                    try { Process.Start("explorer.exe", script.DataPath); }
                    catch { }
        }

        [JSFunction(Name = "delete", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool JS_Delete(ScriptEngine eng, object a)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return false;

                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        [JSFunction(Name = "exists", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool JS_Exists(ScriptEngine eng, object a)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return false;

                return File.Exists(path);
            }

            return false;
        }

        [JSFunction(Name = "load", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static String JS_Load(ScriptEngine eng, object a)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return null;

                try
                {
                    return File.ReadAllText(path);
                }
                catch { }
            }

            return null;
        }

        [JSFunction(Name = "playSound", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool JS_PlaySound(ScriptEngine eng, object a)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return false;

                if (File.Exists(path))
                {
                    InternalSounds.Script(path);
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "save", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool JS_Save(ScriptEngine eng, object a, object b)
        {
            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return false;

                try
                {
                    File.WriteAllText(path, b.ToString());
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
