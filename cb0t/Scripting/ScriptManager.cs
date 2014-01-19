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
        public const int SCRIPT_VERSION = 2001;

        public static List<JSScript> Scripts { get; private set; }
        public static SafeQueue<JSUIEventItem> PendingUIEvents { get; private set; }
        public static SafeQueue<IPEndPoint> PendingTerminators { get; private set; }
        public static SafeQueue<JSOutboundTextItem> PendingUIText { get; private set; }
        public static SafeQueue<IScriptingCallback> PendingScriptingCallbacks { get; private set; }
        public static List<CustomJSMenuOption> UserListMenuOptions { get; private set; }
        public static List<CustomJSMenuOption> RoomMenuOptions { get; private set; }

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
                        room.UserList.Add(new Objects.JSUser(script.JS.Object.InstancePrototype, u, ep));
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

        public static void ClearUsers(IPEndPoint ep)
        {
            foreach (JSScript script in Scripts)
                foreach (Objects.JSRoom room in script.Rooms)
                    if (room.EndPoint.Equals(ep))
                        for (int i = (room.UserList.Count - 1); i > -1; i--)
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
                {
                    ClearUsers(ep);

                    foreach (JSScript script in Scripts)
                        script.Rooms.RemoveAll(x => x.EndPoint.Equals(ep));
                }
            }

            if (PendingUIText.Pending)
            {
                JSOutboundTextItem item = null;

                while (PendingUIText.TryDequeue(out item))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(item.EndPoint));

                    if (r != null)
                        if (item.Type == JSOutboundTextItemType.Command)
                        {
                            String str = ScriptEvents.OnCommandSending(r, item.Text);

                            if (!String.IsNullOrEmpty(str))
                                r.SendCommand(str);
                        }
                        else if (item.Type == JSOutboundTextItemType.Public)
                        {
                            String str = ScriptEvents.OnTextSending(r, item.Text);

                            if (!String.IsNullOrEmpty(str))
                                r.SendText(str);
                        }
                        else if (item.Type == JSOutboundTextItemType.Emote)
                        {
                            String str = ScriptEvents.OnEmoteSending(r, item.Text);

                            if (!String.IsNullOrEmpty(str))
                                r.SendEmote(str);
                        }
                        else if (item.Type == JSOutboundTextItemType.Private)
                        {
                            String str = ScriptEvents.OnPmSending(r, item.Name, item.Text);

                            if (!String.IsNullOrEmpty(str))
                                r.SendPM(item.Name, str);
                        }
                }
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

            if (PendingScriptingCallbacks.Pending)
            {
                IScriptingCallback item = null;

                while (PendingScriptingCallbacks.TryDequeue(out item))
                {
                    if (item is CustomJSUserListMenuCallback)
                    {
                        CustomJSUserListMenuCallback cb = (CustomJSUserListMenuCallback)item;
                        JSScript script = Scripts.Find(x => x.ScriptName == cb.Callback.Engine.ScriptName);

                        if (script != null)
                        {
                            Objects.JSRoom room = script.Rooms.Find(x => x.EndPoint.Equals(cb.Room));

                            if (room != null)
                            {
                                Objects.JSUser user = room.UserList.Find(x => x.U_Name == cb.Target);

                                if (user != null)
                                    try { cb.Callback.Call(script.JS.Global, room, user); }
                                    catch { }
                            }
                        }
                    }
                    else if (item is CustomJSRoomMenuCallback)
                    {
                        CustomJSRoomMenuCallback cb = (CustomJSRoomMenuCallback)item;
                        JSScript script = Scripts.Find(x => x.ScriptName == cb.Callback.Engine.ScriptName);

                        if (script != null)
                        {
                            Objects.JSRoom room = script.Rooms.Find(x => x.EndPoint.Equals(cb.Room));

                            if (room != null)
                                try { cb.Callback.Call(script.JS.Global, room); }
                                catch { }
                        }
                    }
                }
            }
        }

        public static void Init()
        {
            Scripts = new List<JSScript>();
            PendingUIEvents = new SafeQueue<JSUIEventItem>();
            PendingTerminators = new SafeQueue<IPEndPoint>();
            PendingUIText = new SafeQueue<JSOutboundTextItem>();
            UserListMenuOptions = new List<CustomJSMenuOption>();
            RoomMenuOptions = new List<CustomJSMenuOption>();
            PendingScriptingCallbacks = new SafeQueue<IScriptingCallback>();

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

                JSScript script = new JSScript(file.Name, false);
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

                JSScript sscript = new JSScript(Path.GetFileNameWithoutExtension(file.Name), true);
                Scripts.Add(sscript);
            }
        }
    }
}
