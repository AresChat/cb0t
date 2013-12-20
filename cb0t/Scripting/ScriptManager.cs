using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cb0t.Scripting
{
    class ScriptManager
    {
        public const int SCRIPT_VERSION = 1001;

        public static List<JSScript> Scripts { get; private set; }
        public static SafeQueue<JSUIEventItem> PendingEvents { get; private set; }

        public static void EventCycle()
        {
            if (PendingEvents.Pending)
            {
                JSUIEventItem item = null;

                while (PendingEvents.TryDequeue(out item))
                {
                    if (item.EventType == JSUIEventType.KeyPressed)
                        item.Element.KeyPressCallback((int)item.Arg);
                    else if (item.EventType == JSUIEventType.ValueChanged)
                        item.Element.ValueChangedCallback();
                    else if (item.EventType == JSUIEventType.Click)
                        item.Element.ClickCallback();
                    else if (item.EventType == JSUIEventType.Select)
                        item.Element.SelectCallback();
                }
            }
        }

        public static void Init()
        {
            Scripts = new List<JSScript>();
            PendingEvents = new SafeQueue<JSUIEventItem>();

            DirectoryInfo dir = new DirectoryInfo(Settings.ScriptPath);

            foreach (DirectoryInfo d in dir.GetDirectories().Where(x => x.Name.EndsWith(".js")))
                LoadScript(Path.Combine(d.FullName, d.Name));
        }

        private static void LoadScript(String path)
        {
            FileInfo file = new FileInfo(path);
            
            if (file.Exists)
            {
                String name = file.Name;

                if (Scripts.Find(x => x.ScriptName == name) != null)
                    return;

                JSScript script = new JSScript(file.Name);
                Scripts.Add(script);
                script.LoadScript(path);
                script.UI.CanCreate = true;

                try
                {
                    script.JS.CallGlobalFunction("onload", script.UI);
                }
                catch (Jurassic.JavaScriptException e)
                {
                    System.Windows.Forms.MessageBox.Show("error in onload: " + e.Message);
                }
                catch { }

                script.UI.CanCreate = false;
                script.UI.CanAddControls = false;
            }
        }

    }
}
