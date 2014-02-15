using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using FormEx;

namespace cb0t.Scripting
{
    class ScriptManager
    {
        public const int SCRIPT_VERSION = 2023;

        public static List<JSScript> Scripts { get; private set; }
        public static SafeQueue<JSUIEventItem> PendingUIEvents { get; private set; }
        public static SafeQueue<IPEndPoint> PendingTerminators { get; private set; }
        public static SafeQueue<JSOutboundTextItem> PendingUIText { get; private set; }
        public static SafeQueue<IScriptingCallback> PendingScriptingCallbacks { get; private set; }
        public static SafeQueue<JSUIPopupCallback> PendingPopupCallbacks { get; private set; }
        public static List<CustomJSMenuOption> UserListMenuOptions { get; private set; }
        public static List<CustomJSMenuOption> RoomMenuOptions { get; private set; }

        private static List<String> auto_loaded_scripts = new List<String>();

        public static void InstallScript(String filename)
        {
            IntPtr ptr = Process.GetCurrentProcess().MainWindowHandle;
            DwmForm control = (DwmForm)DwmForm.FromHandle(ptr);
            InstallScript(control, filename);
        }

        private delegate void InstallScriptHandler(DwmForm form, String filename);
        private static void InstallScript(DwmForm form, String filename)
        {
            if (form.InvokeRequired)
                form.BeginInvoke(new InstallScriptHandler(InstallScript), form, filename);
            else
            {
                if (!form.Visible)
                    form.Show();

                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;

                form.Activate();

                if (Settings.IsAway)
                    if (form.OverlayIcon != null)
                        form.OverlayIcon.Show();

                DialogResult result = MessageBox.Show(form,
                                                      "Confirm you would like to install the script: " + filename,
                                                      "cb0t script installer",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool install_success = false;

                    try
                    {
                        String url = "http://chatrooms.marsproject.net/cb0t/" + filename;
                        byte[] data = null;
                        WebRequest request = WebRequest.Create(url);

                        using (WebResponse response = request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        {
                            List<byte> list = new List<byte>();
                            int size = 0;
                            byte[] buffer = new byte[1024];

                            while ((size = stream.Read(buffer, 0, 1024)) > 0)
                                list.AddRange(buffer.Take(size));

                            if (list.Count > 0)
                                data = list.ToArray();
                        }

                        if (data != null)
                        {
                            data = Zip.Decompress(data);

                            if (data != null)
                                if (data.Length > 0)
                                {
                                    TCPPacketReader reader = new TCPPacketReader(data);
                                    String dir_path = Path.Combine(Settings.ScriptPath, filename);

                                    if (!Directory.Exists(dir_path))
                                        Directory.CreateDirectory(dir_path);

                                    String data_path = Path.Combine(dir_path, "data");

                                    if (!Directory.Exists(data_path))
                                        Directory.CreateDirectory(data_path);

                                    while (reader.Remaining > 0)
                                    {
                                        String name = reader.ReadString();
                                        byte type = reader;
                                        uint size = reader;
                                        byte[] buffer = reader.ReadBytes((int)size);

                                        if (type == 1)
                                            File.WriteAllBytes(Path.Combine(data_path, name), buffer);
                                        else if (type == 0)
                                            File.WriteAllBytes(Path.Combine(dir_path, name), buffer);
                                    }

                                    AddToAutoLoad(filename);
                                    install_success = true;
                                }
                        }
                    }
                    catch { }

                    if (install_success)
                        MessageBox.Show(form,
                                        filename + " was installed successfully.  Please complete the installation by restarting cb0t now.",
                                        "cb0t script installer",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    else
                        MessageBox.Show(form,
                                        filename + " was unable to be installed at this time.",
                                        "cb0t script installer",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                }
            }
        }

        public static bool AddToAutoLoad(String name)
        {
            String[] avail = AvailableScripts;

            if (avail.Contains(name))
                if (!auto_loaded_scripts.Contains(name))
                {
                    auto_loaded_scripts.Add(name);

                    try
                    {
                        File.WriteAllLines(Path.Combine(Settings.ScriptPath, "autoload"), auto_loaded_scripts.ToArray());
                    }
                    catch { }

                    return true;
                }

            return false;
        }

        public static bool RemoveFromAutoLoad(String name)
        {
            int count = auto_loaded_scripts.RemoveAll(x => x == name);

            if (count > 0)
            {
                try
                {
                    File.WriteAllLines(Path.Combine(Settings.ScriptPath, "autoload"), auto_loaded_scripts.ToArray());
                }
                catch { }

                return true;
            }

            return false;
        }

        public static String[] AutoLoadedScripts
        {
            get
            {
                return auto_loaded_scripts.ToArray();
            }
        }

        public static String[] AvailableScripts
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Settings.ScriptPath);
                List<String> results = new List<String>();

                foreach (DirectoryInfo d in dir.GetDirectories().Where(x => x.Name.EndsWith(".js")))
                    results.Add(d.Name);

                return results.ToArray();
            }
        }

        public static void AddRoom(IPEndPoint ep, FavouritesListItem creds)
        {
            foreach (JSScript script in Scripts)
                script.Rooms.Add(new Objects.JSRoom(script.JS.Object.InstancePrototype, ep, creds));

            ScriptEvents.OnRoomOpened(ep);
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

        public static void EventCycle(ulong time)
        {
            JSTimers.ServiceTimers(time);

            if (PendingTerminators.Pending)
            {
                IPEndPoint ep = null;

                while (PendingTerminators.TryDequeue(out ep))
                {
                    ClearUsers(ep);
                    ScriptEvents.OnRoomClosed(ep);

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
                        else if (item.Type == JSOutboundTextItemType.Link)
                        {
                            if (ScriptEvents.OnLinkClicked(r, item.Text))
                            {
                                try
                                {
                                    Process.Start(item.Text);
                                }
                                catch { }
                            }
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
                    else if (item.EventType == JSUIEventType.UISelected)
                        ScriptEvents.OnUISelected((String)item.Arg);
                    else if (item.EventType == JSUIEventType.RoomOpened)
                        ScriptEvents.OnRoomOpened((IPEndPoint)item.Arg);
                    else if (item.EventType == JSUIEventType.RoomClosed)
                        ScriptEvents.OnRoomClosed((IPEndPoint)item.Arg);
                }
            }

            if (PendingPopupCallbacks.Pending)
            {
                JSUIPopupCallback item = null;

                while (PendingPopupCallbacks.TryDequeue(out item))
                {
                    JSScript script = Scripts.Find(x => x.ScriptName == item.Callback.Engine.ScriptName);

                    if (script != null)
                    {
                        Objects.JSRoom room = script.Rooms.Find(x => x.EndPoint.Equals(item.Room));

                        if (room != null)
                            try { item.Callback.Call(script.JS.Global, room); }
                            catch (Jurassic.JavaScriptException je)
                            {
                                ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                            }
                            catch { }
                    }

                    item.Callback = null;
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
                                    catch (Jurassic.JavaScriptException je)
                                    {
                                        ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                                    }
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
                                catch (Jurassic.JavaScriptException je)
                                {
                                    ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                                }
                                catch { }
                        }
                    }
                    else if (item is Objects.JSHttpRequestResult)
                    {
                        Objects.JSHttpRequestResult cb = (Objects.JSHttpRequestResult)item;
                        JSScript script = Scripts.Find(x => x.ScriptName == cb.ScriptName);

                        if (script != null)
                            if (cb.Callback != null)
                                try { cb.Callback.Call(cb, !String.IsNullOrEmpty(cb.Data)); }
                                catch (Jurassic.JavaScriptException je)
                                {
                                    ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                                }
                                catch { }
                    }
                    else if (item is Objects.JSScribbleImage)
                    {
                        Objects.JSScribbleImage cb = (Objects.JSScribbleImage)item;
                        JSScript script = Scripts.Find(x => x.ScriptName == cb.ScriptName);

                        if (script != null)
                            if (cb.Callback != null)
                                try { cb.Callback.Call(cb, cb.Data != null); }
                                catch (Jurassic.JavaScriptException je)
                                {
                                    ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                                }
                                catch { }
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
            PendingPopupCallbacks = new SafeQueue<JSUIPopupCallback>();

            try
            {
                String[] lines = File.ReadAllLines(Path.Combine(Settings.ScriptPath, "autoload"));

                if (lines.Length > 0)
                    foreach (String str in lines)
                        if (!String.IsNullOrEmpty(str))
                            auto_loaded_scripts.Add(str);
            }
            catch { }

            DirectoryInfo dir = new DirectoryInfo(Settings.ScriptPath);

            foreach (DirectoryInfo d in dir.GetDirectories().Where(x => x.Name.EndsWith(".js")))
                if (auto_loaded_scripts.Contains(d.Name))
                    LoadScript(Path.Combine(d.FullName, d.Name));

            auto_loaded_scripts.Clear();

            foreach (JSScript script in Scripts)
                if (script.ScriptName.EndsWith(".js"))
                    auto_loaded_scripts.Add(script.ScriptName);

            try
            {
                File.WriteAllLines(Path.Combine(Settings.ScriptPath, "autoload"), auto_loaded_scripts.ToArray());
            }
            catch { }
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
                    catch (Jurassic.JavaScriptException je)
                    {
                        ScriptManager.ErrorHandler(script.ScriptName, je.LineNumber, je.Message);
                    }
                    catch { }

                script.UI.CanCreate = false;
                script.UI.CanAddControls = false;

                JSScript sscript = new JSScript(Path.GetFileNameWithoutExtension(file.Name), true);
                Scripts.Add(sscript);
            }
        }

        public static void ErrorHandler(String script_name, int line, String message)
        {
            foreach (JSScript script in Scripts)
                if (script.EVENT_ONERROR != null)
                {
                    try
                    {
                        script.EVENT_ONERROR.Call(script.JS.Global, script_name, line, message);
                    }
                    catch { }
                }
        }
    }
}
