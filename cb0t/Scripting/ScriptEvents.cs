using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class ScriptEvents
    {
        public static void OnDisconnected(Room room)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONDISCONNECTED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONDISCONNECTED.Call(script.JS.Global, r);
                            }
                            catch { }

                            break;
                        }
        }

        public static void OnConnecting(Room room)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONCONNECTING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONCONNECTING.Call(script.JS.Global, r);
                            }
                            catch { }

                            break;
                        }
        }

        public static void OnConnected(Room room)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONCONNECTED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONCONNECTED.Call(script.JS.Global, r);
                            }
                            catch { }

                            break;
                        }
        }

        public static bool OnRedirecting(Room room, Redirect redirect)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONREDIRECTING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (redirect != null)
                        {
                            Scripting.Objects.JSRedirect target = new Scripting.Objects.JSRedirect(script.JS.Object.InstancePrototype, redirect);

                            if (r.EndPoint.Equals(room.EndPoint))
                            {
                                try
                                {
                                    object obj = script.EVENT_ONREDIRECTING.Call(script.JS.Global, r, target);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }

                                break;
                            }
                        }

            return true;
        }

        public static String OnCommandSending(Room room, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONCOMMANDSENDING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONCOMMANDSENDING.Call(script.JS.Global, r, str);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static String OnEmoteSending(Room room, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONEMOTESENDING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONEMOTESENDING.Call(script.JS.Global, r, str);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static String OnTextSending(Room room, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONTEXTSENDING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONTEXTSENDING.Call(script.JS.Global, r, str);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static String OnPmSending(Room room, String name, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONPMSENDING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser user = r.UserList.Find(x => x.U_Name == name);

                            if (user != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONPMSENDING.Call(script.JS.Global, r, user, str);

                                    if (obj != null)
                                    {
                                        str = obj.ToString();

                                        if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                            return null;
                                    }
                                }
                                catch { }
                            }

                            break;
                        }

            return str;
        }

        public static String OnTextReceiving(Room room, String name, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.Text)
                return null;

            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Text, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return null;
                    }
                }

            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONTEXTRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser user = r.UserList.Find(x => x.U_Name == name);

                            if (user != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONTEXTRECEIVING.Call(script.JS.Global, r, user, str);

                                    if (obj != null)
                                    {
                                        str = obj.ToString();

                                        if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                            return null;
                                    }
                                }
                                catch { }
                            }

                            break;
                        }

            return str;
        }

        public static void OnTextReceived(Room room, String name, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Text, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", name).Replace("+t", text));
                        }
                    }
                }

            if (room.RoomIsVisible)
                if (Settings.GetReg<bool>("can_narrate", false))
                    Narrator.Say(text);

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONTEXTRECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser user = r.UserList.Find(x => x.U_Name == name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONTEXTRECEIVED.Call(script.JS.Global, r, user, text);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static String OnEmoteReceiving(Room room, String name, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.Text)
                return null;

            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Emote, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return null;
                    }
                }

            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONEMOTERECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser user = r.UserList.Find(x => x.U_Name == name);

                            if (user != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONEMOTERECEIVING.Call(script.JS.Global, r, user, str);

                                    if (obj != null)
                                    {
                                        str = obj.ToString();

                                        if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                            return null;
                                    }
                                }
                                catch { }
                            }

                            break;
                        }

            return str;
        }

        public static void OnEmoteReceived(Room room, String name, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Emote, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", name).Replace("+t", text));
                        }
                    }
                }

            if (room.RoomIsVisible)
                if (Settings.GetReg<bool>("can_narrate", false))
                    Narrator.Say(text);

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONEMOTERECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser user = r.UserList.Find(x => x.U_Name == name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONEMOTERECEIVED.Call(script.JS.Global, r, user, text);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static String OnAnnounceReceiving(Room room, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONANNOUNCERECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONANNOUNCERECEIVING.Call(script.JS.Global, r, str);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static void OnAnnounceReceived(Room room, String text)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONANNOUNCERECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONANNOUNCERECEIVED.Call(script.JS.Global, r, text);
                            }
                            catch { }

                            break;
                        }
        }

        public static void OnUserlistReceived(Room room)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERLISTRECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONUSERLISTRECEIVED.Call(script.JS.Global, r);
                            }
                            catch { }

                            break;
                        }
        }

        public static bool OnUserJoining(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERJOINING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONUSERJOINING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static void OnUserJoined(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Join, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                        }
                    }
                }

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERJOINED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONUSERJOINED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static bool OnUserParting(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERPARTING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONUSERPARTING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static void OnUserParted(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Part, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                        }
                    }
                }

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERPARTED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONUSERPARTED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static void OnUserLevelChanged(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERLEVELCHANGED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONUSERLEVELCHANGED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static bool OnUserAvatarReceiving(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERAVATARRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONUSERAVATARRECEIVING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static bool OnUserMessageReceiving(Room room, User user, String text)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERMESSAGERECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONUSERMESSAGERECEIVING.Call(script.JS.Global, r, userobj, text);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static String OnTopicReceiving(Room room, String text)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONTOPICRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONTOPICRECEIVING.Call(script.JS.Global, r, str);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static String OnUrlReceiving(Room room, String text, String address)
        {
            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONURLRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                object obj = script.EVENT_ONURLRECEIVING.Call(script.JS.Global, r, str, address);

                                if (obj != null)
                                {
                                    str = obj.ToString();

                                    if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                        return null;
                                }
                            }
                            catch { }

                            break;
                        }

            return str;
        }

        public static bool OnUserFontChanging(Room room, User user, AresFont font)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERFONTCHANGING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                Scripting.Objects.JSFont f = new Scripting.Objects.JSFont(script.JS.Object.InstancePrototype, font);

                                try
                                {
                                    object obj = script.EVENT_ONUSERFONTCHANGING.Call(script.JS.Global, r, userobj, f);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static void OnUserWritingStatusChanged(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERWRITINGSTATUSCHANGED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONUSERWRITINGSTATUSCHANGED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static void OnUserOnlineStatusChanged(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONUSERONLINESTATUSCHANGED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONUSERONLINESTATUSCHANGED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static String OnPmReceiving(Room room, User user, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.PM)
                return null;

            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PM, user.Name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return null;
                    }
                }

            String str = text;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONPMRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONPMRECEIVING.Call(script.JS.Global, r, userobj, str);

                                    if (obj != null)
                                    {
                                        str = obj.ToString();

                                        if (String.IsNullOrEmpty(str) || obj is Jurassic.Null)
                                            return null;
                                    }
                                }
                                catch { }
                            }

                            break;
                        }

            return str;
        }

        public static void OnPmReceived(Room room, User user, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PM, user.Name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name).Replace("+t", text));
                        }
                    }
                }

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONPMRECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONPMRECEIVED.Call(script.JS.Global, r, userobj, text);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static void OnPmFirstReceived(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PMFirst, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name).Replace("+t", room.Credentials.Name));
                        }
                    }
                }
        }

        public static bool OnNudgeReceiving(Room room, User user)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All)
                return false;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONNUDGERECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONNUDGERECEIVING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static bool OnScribbleReceiving(Room room, User user)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All)
                return false;

            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONSCRIBBLERECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONSCRIBBLERECEIVING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static void OnScribbleReceived(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONSCRIBBLERECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONSCRIBBLERECEIVED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static void OnSongChanged(Room room, String song)
        {
            if (Settings.GetReg<bool>("show_song", true))
            {
                if (!String.IsNullOrEmpty(song))
                    room.SendPersonalMessage("\x0007" + song);
                else
                    room.SendPersonalMessage();
            }

            if (room.CanNP)
                if (!String.IsNullOrEmpty(song))
                    foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                        if (script.EVENT_ONSONGCHANGED != null)
                            foreach (Scripting.Objects.JSRoom r in script.Rooms)
                                if (r.EndPoint.Equals(room.EndPoint))
                                {
                                    try
                                    {
                                        script.EVENT_ONSONGCHANGED.Call(script.JS.Global, r, song);
                                    }
                                    catch { }

                                    break;
                                }
        }

        public static void OnTimer(Room room)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONTIMER != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            try
                            {
                                script.EVENT_ONTIMER.Call(script.JS.Global, r);
                            }
                            catch { }

                            break;
                        }
        }

        public static bool OnVoiceClipReceiving(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONVOICECLIPRECEIVING != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (userobj != null)
                            {
                                try
                                {
                                    object obj = script.EVENT_ONVOICECLIPRECEIVING.Call(script.JS.Global, r, userobj);

                                    if (obj != null)
                                        if (obj is bool)
                                            if (!((bool)obj))
                                                return false;
                                }
                                catch { }
                            }

                            break;
                        }

            return true;
        }

        public static void OnVoiceClipReceived(Room room, User user)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONVOICECLIPRECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONVOICECLIPRECEIVED.Call(script.JS.Global, r, userobj);
                                }
                                catch { }
                            }

                            break;
                        }
        }

        public static void OnCustomDataReceived(Room room, User user, String ident, String data)
        {
            foreach (Scripting.JSScript script in Scripting.ScriptManager.Scripts)
                if (script.EVENT_ONCUSTOMDATARECEIVED != null)
                    foreach (Scripting.Objects.JSRoom r in script.Rooms)
                        if (r.EndPoint.Equals(room.EndPoint))
                        {
                            Scripting.Objects.JSUser userobj = r.UserList.Find(x => x.U_Name == user.Name);

                            if (user != null)
                            {
                                try
                                {
                                    script.EVENT_ONCUSTOMDATARECEIVED.Call(script.JS.Global, r, userobj, ident, data);
                                }
                                catch { }
                            }

                            break;
                        }
        }
    }
}
