using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace cb0t.Scripting
{
    class JSScript
    {
        public String ScriptName { get; private set; }
        public ScriptEngine JS { get; private set; }
        public Objects.JSUI UI { get; private set; }
        public List<ICustomUI> Elements { get; private set; }

        public JSScript(String name)
        {
            this.Elements = new List<ICustomUI>();
            this.ScriptName = name;

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            this.JS = new ScriptEngine();
            this.JS.ScriptName = name;
            this.JS.EmbedGlobalClass(typeof(JSGlobal));
            this.JS.EmbedStatics(types.Where(x => x.Namespace == "cb0t.Scripting.Statics" && x.IsSubclassOf(typeof(ObjectInstance))).ToArray());
            this.JS.EmbedInstances(types.Where(x => x.Namespace == "cb0t.Scripting.Instances" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());
            this.JS.EmbedObjectPrototypes(types.Where(x => x.Namespace == "cb0t.Scripting.ObjectPrototypes" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());

            this.UI = new Objects.JSUI(this.JS.Object.InstancePrototype);

            StringBuilder events = new StringBuilder();
            events.AppendLine("function onload(ui) { }");
            this.JS.Evaluate(events.ToString());
        }

        public void LoadScript(String path)
        {
            try
            {
                this.JS.ExecuteFile(path);
            }
            catch (JavaScriptException e)
            {
                System.Windows.Forms.MessageBox.Show("error in script: " + e.Message);
            }
            catch { }
        }
    }
}
