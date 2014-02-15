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
        public UserDefinedFunction EVENT_ONLINKCLICKED { get; set; }
        public UserDefinedFunction EVENT_ONUISELECTED { get; set; }
        public UserDefinedFunction EVENT_ONROOMOPENED { get; set; }
        public UserDefinedFunction EVENT_ONROOMCLOSED { get; set; }
        public UserDefinedFunction EVENT_ONERROR { get; set; }

        public JSScript(String name, bool is_subscript)
        {
            if (!is_subscript)
                this.Elements = new List<ICustomUI>();

            this.Rooms = new List<Objects.JSRoom>();
            this.ScriptName = name;

            if (is_subscript)
                this.ScriptPath = Path.Combine(Settings.ScriptPath, name + ".js");
            else
                this.ScriptPath = Path.Combine(Settings.ScriptPath, name);

            this.DataPath = Path.Combine(this.ScriptPath, "data");

            if (!Directory.Exists(this.DataPath))
                try { Directory.CreateDirectory(this.DataPath); }
                catch { }

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            this.JS = new ScriptEngine();
            this.JS.ScriptName = name;
            this.JS.EmbedGlobalClass(typeof(JSGlobal));
            this.JS.EmbedStatics(types.Where(x => x.Namespace == "cb0t.Scripting.Statics" && x.IsSubclassOf(typeof(ObjectInstance))).ToArray());
            this.JS.EmbedInstances(types.Where(x => x.Namespace == "cb0t.Scripting.Instances" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());
            this.JS.EmbedObjectPrototypes(types.Where(x => x.Namespace == "cb0t.Scripting.ObjectPrototypes" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());

            if (!is_subscript)
                this.UI = new Objects.JSUI(this.JS.Object.InstancePrototype);
        }

        public void LoadScript(String path)
        {
            try
            {
                this.JS.ExecuteFile(path);
            }
            catch (Jurassic.JavaScriptException je)
            {
                ScriptManager.ErrorHandler(this.ScriptName, je.LineNumber, je.Message);
            }
            catch { }
        }

        public void ResetScript()
        {
            JSTimers.ClearTimers(this.ScriptName);
            this.EVENT_ONANNOUNCERECEIVED = null;
            this.EVENT_ONANNOUNCERECEIVING = null;
            this.EVENT_ONCOMMANDSENDING = null;
            this.EVENT_ONCONNECTED = null;
            this.EVENT_ONCONNECTING = null;
            this.EVENT_ONCUSTOMDATARECEIVED = null;
            this.EVENT_ONDISCONNECTED = null;
            this.EVENT_ONEMOTERECEIVED = null;
            this.EVENT_ONEMOTERECEIVING = null;
            this.EVENT_ONEMOTESENDING = null;
            this.EVENT_ONLOAD = null;
            this.EVENT_ONNUDGERECEIVING = null;
            this.EVENT_ONPMRECEIVED = null;
            this.EVENT_ONPMRECEIVING = null;
            this.EVENT_ONPMSENDING = null;
            this.EVENT_ONREDIRECTING = null;
            this.EVENT_ONSCRIBBLERECEIVED = null;
            this.EVENT_ONSCRIBBLERECEIVING = null;
            this.EVENT_ONSONGCHANGED = null;
            this.EVENT_ONTEXTRECEIVED = null;
            this.EVENT_ONTEXTRECEIVING = null;
            this.EVENT_ONTEXTSENDING = null;
            this.EVENT_ONTIMER = null;
            this.EVENT_ONTOPICRECEIVING = null;
            this.EVENT_ONURLRECEIVING = null;
            this.EVENT_ONUSERAVATARRECEIVING = null;
            this.EVENT_ONUSERFONTCHANGING = null;
            this.EVENT_ONUSERJOINED = null;
            this.EVENT_ONUSERJOINING = null;
            this.EVENT_ONUSERLEVELCHANGED = null;
            this.EVENT_ONUSERLISTRECEIVED = null;
            this.EVENT_ONUSERMESSAGERECEIVING = null;
            this.EVENT_ONUSERONLINESTATUSCHANGED = null;
            this.EVENT_ONUSERPARTED = null;
            this.EVENT_ONUSERPARTING = null;
            this.EVENT_ONUSERWRITINGSTATUSCHANGED = null;
            this.EVENT_ONVOICECLIPRECEIVED = null;
            this.EVENT_ONVOICECLIPRECEIVING = null;

            for (int i = (this.Rooms.Count - 1); i > -1; i--)
            {
                for (int x = (this.Rooms[i].UserList.Count - 1); x > -1; x--)
                {
                    this.Rooms[i].UserList[x].SetToNull();
                    this.Rooms[i].UserList.RemoveAt(x);
                }

                this.Rooms.RemoveAt(i);
            }

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            this.JS = new ScriptEngine();
            this.JS.ScriptName = this.ScriptName;
            this.JS.EmbedGlobalClass(typeof(JSGlobal));
            this.JS.EmbedStatics(types.Where(x => x.Namespace == "cb0t.Scripting.Statics" && x.IsSubclassOf(typeof(ObjectInstance))).ToArray());
            this.JS.EmbedInstances(types.Where(x => x.Namespace == "cb0t.Scripting.Instances" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());
            this.JS.EmbedObjectPrototypes(types.Where(x => x.Namespace == "cb0t.Scripting.ObjectPrototypes" && x.IsSubclassOf(typeof(ClrFunction))).ToArray());

            foreach (Room room in RoomPool.Rooms)
            {
                this.Rooms.Add(new Objects.JSRoom(this.JS.Object.InstancePrototype, room.EndPoint, room.Credentials));

                foreach (User user in room.UserPool)
                    this.Rooms[this.Rooms.Count - 1].UserList.Add(new Objects.JSUser(this.JS.Object.InstancePrototype, user, room.EndPoint));
            }
        }

        public String Eval(String src)
        {
            String result = null;

            try
            {
                object eval = this.JS.Evaluate(src);

                if (eval != null)
                {
                    if (eval is bool)
                        result = eval.ToString().ToLower();
                    else
                        result = eval.ToString();

                    if (result == "undefined")
                        result = null;
                }
            }
            catch (Jurassic.JavaScriptException je)
            {
                ScriptManager.ErrorHandler(this.ScriptName, je.LineNumber, je.Message);
            }
            catch { }

            return result;
        }
    }
}
