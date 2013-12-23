using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace cb0t.Scripting
{
    class ScriptManager
    {
        public const int SCRIPT_VERSION = 1001;

        public static List<JSScript> Scripts { get; private set; }
        public static SafeQueue<JSUIEventItem> PendingUIEvents { get; private set; }
        public static SafeQueue<IPEndPoint> PendingTerminators { get; private set; }

        public static void AddRoom(IPEndPoint ep)
        {
            foreach (JSScript script in Scripts)
                script.Rooms.Add(new Objects.JSRoom(script.JS.Object.InstancePrototype, ep));
        }

        public static void AddUser(IPEndPoint ep, User u)
        {
            foreach (JSScript script in Scripts)
                foreach (Objects.JSRoom room in script.Rooms)
                    if (room.EndPoint.Equals(ep))
                        room.UserList.Add(new Objects.JSUser(script.JS.Object.InstancePrototype, u));
        }

        public static void RemoveUser(IPEndPoint ep, User u)
        {
            foreach (JSScript script in Scripts)
                foreach (Objects.JSRoom room in script.Rooms)
                    if (room.EndPoint.Equals(ep))
                        for (int i = (room.UserList.Count - 1); i > -1; i--)
                            if (room.UserList[i].U_Name == u.Name)
                            {
                                room.UserList[i].SetToNull();
                                room.UserList.RemoveAt(i);
                            }
        }

        public static void EventCycle()
        {
            if (PendingTerminators.Pending)
            {
                IPEndPoint ep = null;

                while (PendingTerminators.TryDequeue(out ep))
                    foreach (JSScript script in Scripts)
                        script.Rooms.RemoveAll(x => x.EndPoint.Equals(ep));
            }

            if (PendingUIEvents.Pending)
            {
                JSUIEventItem item = null;

                while (PendingUIEvents.TryDequeue(out item))
                {
                    if (item.EventType == JSUIEventType.KeyPressed)
                        item.Element.KeyPressCallback((int)item.Arg);
                    else if (item.EventType == JSUIEventType.ValueChanged)
                        item.Element.ValueChangedCallback();
                    else if (item.EventType == JSUIEventType.Click)
                        item.Element.ClickCallback();
                    else if (item.EventType == JSUIEventType.Select)
                        item.Element.SelectCallback();
                    else if (item.EventType == JSUIEventType.ItemDoubleClick)
                        item.Element.ItemDoubleClickCallback();
                    else if (item.EventType == JSUIEventType.SelectedItemChanged)
                        item.Element.SelectedItemChangedCallback();
                }
            }
        }

        public static void Init()
        {
            Scripts = new List<JSScript>();
            PendingUIEvents = new SafeQueue<JSUIEventItem>();
            PendingTerminators = new SafeQueue<IPEndPoint>();

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

                if (script.EVENT_ONLOAD != null)
                    try
                    {
                        script.EVENT_ONLOAD.Call(script.JS.Global, script.UI);
                    }
                    catch { }

                script.UI.CanCreate = false;
                script.UI.CanAddControls = false;
            }
        }

    }
}
