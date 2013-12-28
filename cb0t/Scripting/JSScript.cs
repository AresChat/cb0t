using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.IO;
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
        public List<Objects.JSRoom> Rooms { get; private set; }

        public String ScriptPath { get; private set; }
        public String DataPath { get; private set; }

        public UserDefinedFunction EVENT_ONLOAD { get; set; }
        public UserDefinedFunction EVENT_ONCONNECTED { get; set; }
        public UserDefinedFunction EVENT_ONUSERLISTRECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONDISCONNECTED { get; set; }
        public UserDefinedFunction EVENT_ONCONNECTING { get; set; }
        public UserDefinedFunction EVENT_ONREDIRECTING { get; set; }
        public UserDefinedFunction EVENT_ONCOMMANDSENDING { get; set; }
        public UserDefinedFunction EVENT_ONEMOTESENDING { get; set; }
        public UserDefinedFunction EVENT_ONTEXTSENDING { get; set; }
        public UserDefinedFunction EVENT_ONPMSENDING { get; set; }
        public UserDefinedFunction EVENT_ONTIMER { get; set; }
        public UserDefinedFunction EVENT_ONTEXTRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONTEXTRECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONEMOTERECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONEMOTERECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONANNOUNCERECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONANNOUNCERECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONUSERJOINING { get; set; }
        public UserDefinedFunction EVENT_ONUSERJOINED { get; set; }
        public UserDefinedFunction EVENT_ONUSERPARTING { get; set; }
        public UserDefinedFunction EVENT_ONUSERPARTED { get; set; }
        public UserDefinedFunction EVENT_ONUSERLEVELCHANGED { get; set; }
        public UserDefinedFunction EVENT_ONUSERAVATARRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONUSERMESSAGERECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONTOPICRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONURLRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONUSERFONTCHANGING { get; set; }
        public UserDefinedFunction EVENT_ONUSERWRITINGSTATUSCHANGED { get; set; }
        public UserDefinedFunction EVENT_ONUSERONLINESTATUSCHANGED { get; set; }
        public UserDefinedFunction EVENT_ONPMRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONPMRECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONNUDGERECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONSCRIBBLERECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONSCRIBBLERECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONSONGCHANGED { get; set; }
        public UserDefinedFunction EVENT_ONVOICECLIPRECEIVING { get; set; }
        public UserDefinedFunction EVENT_ONVOICECLIPRECEIVED { get; set; }
        public UserDefinedFunction EVENT_ONCUSTOMDATARECEIVED { get; set; }

        public JSScript(String name)
        {
            this.Elements = new List<ICustomUI>();
            this.Rooms = new List<Objects.JSRoom>();
            this.ScriptName = name;
            this.ScriptPath = Path.Combine(Settings.ScriptPath, name);
            this.DataPath = Path.Combine(this.ScriptPath, "data");

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            this.JS = new ScriptEngine();
            this.JS.ScriptName = name;
            this.JS.EmbedGlobalClass(typeof(JSGlobal));
            this.JS.EmbedStatics(types.Where(x => x.Namespace == "cb0t.Scripting.Statics" && x.IsSubclassOf(typeof(ObjectInstance))).ToArray());
            this.JS.EmbedInstances(types.Where(x => x.Namespace == "cb0t.Scripting.Instances" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());
            this.JS.EmbedObjectPrototypes(types.Where(x => x.Namespace == "cb0t.Scripting.ObjectPrototypes" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());

            this.UI = new Objects.JSUI(this.JS.Object.InstancePrototype);
        }

        public void LoadScript(String path)
        {
            try
            {
                this.JS.ExecuteFile(path);
            }
            catch (JavaScriptException e)
            {
                
            }
            catch { }
        }
    }
}
