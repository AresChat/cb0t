using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSScribbleImage : ObjectInstance, IScriptingCallback
    {
        public JSScribbleImage(ObjectInstance prototype)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["ScribbleImage"]).InstancePrototype)
        {
            this.PopulateFunctions();
        }

        internal JSScribbleImage(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public byte[] Data { get; set; }
        public UserDefinedFunction Callback { get; set; }
        public String ScriptName { get; set; }
        public String Arg { get; set; }
        public int Height { get; set; }
        public String URL { get; set; }

        protected override string InternalClassName
        {
            get { return "ScribbleImage"; }
        }

        [JSProperty(Name = "arg")]
        public String GetArgument
        {
            get { return this.Arg; }
            set { }
        }

        [JSProperty(Name = "exists")]
        public bool DoesExist
        {
            get { return this.Data != null; }
            set { }
        }

        [JSFunction(Name = "dispose", IsWritable = false, IsEnumerable = true)]
        public void ScReset()
        {
            this.Data = new byte[] { };
            this.Data = null;
        }

        [JSFunction(Name = "save", IsWritable = false, IsEnumerable = true)]
        public bool Save(object a)
        {
            if (this.Data == null)
                return false;

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
            {
                String path = a.ToString();
                path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                path = Path.Combine(script.DataPath, path);

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                    return false;

                try
                {
                    File.WriteAllBytes(path, Zip.Decompress(this.Data));
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
