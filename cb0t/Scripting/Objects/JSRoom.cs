using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSRoom : ObjectInstance
    {
        internal JSRoom(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSRoom(ObjectInstance prototype, IPEndPoint ep, FavouritesListItem creds)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Room"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.EndPoint = ep;
            this.UserList = new List<JSUser>();
            this.Creds = creds.Copy();
        }

        protected override string InternalClassName
        {
            get { return "Room"; }
        }

        // begin

        private FavouritesListItem Creds { get; set; }
        public IPEndPoint EndPoint { get; private set; }
        public List<JSUser> UserList { get; private set; }

        [JSFunction(Name = "equals", IsEnumerable = true, IsWritable = false)]
        public bool R_IsEqual(object a)
        {
            if (!(a is Undefined))
                if (a is JSRoom)
                {
                    JSRoom compare = (JSRoom)a;

                    if (compare.EndPoint != null)
                        if (this.EndPoint != null)
                            return this.EndPoint.Equals(compare.EndPoint);
                }

            return false;
        }

        [JSFunction(Name = "injectHTML", IsEnumerable = true, IsWritable = false)]
        public void R_InjectHTML(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    r.Panel.InjectHTMLFromScript(str);
            }
        }

        [JSFunction(Name = "injectJS", IsEnumerable = true, IsWritable = false)]
        public void R_InjectJS(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    r.Panel.InjectJSFromScript(str);
            }
        }

        [JSFunction(Name = "popup", IsEnumerable = true, IsWritable = false)]
        public void R_Popup(object a, object b, object c)
        {
            if (!(a is Undefined))
                if (!(b is Undefined))
                {
                    String caption = "cb0t :: " + a.ToString();
                    String text = b.ToString();
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        if (c is Undefined)
                            r.ShowPopup(caption, text, PopupSound.Notify);
                        else if (c is UserDefinedFunction)
                        {
                            JSUIPopupCallback cb = new JSUIPopupCallback
                            {
                                Callback = (UserDefinedFunction)c,
                                Room = this.EndPoint
                            };

                            r.ShowPopup(caption, text, cb);
                        }
                }
        }

        [JSFunction(Name = "print", IsEnumerable = true, IsWritable = false)]
        public void R_Print(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    r.ShowAnnounceText(str);
            }
        }

        [JSFunction(Name = "scribble", IsWritable = false, IsEnumerable = true)]
        public void Scribble(object a)
        {
            if (a is Objects.JSScribbleImage)
            {
                Objects.JSScribbleImage cb = (Objects.JSScribbleImage)a;

                if (cb.Data != null)
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.ScriptScribble(cb.Data, null);
                }
            }
        }

        [JSFunction(Name = "sendCommand", IsEnumerable = true, IsWritable = false)]
        public void R_SendCommand(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.SendCommand(str);
                }
            }
        }

        [JSFunction(Name = "sendCustomData", IsEnumerable = true, IsWritable = false)]
        public void R_SendCustomData(object a, object b)
        {
            if (!(a is Undefined) && !(b is Undefined))
            {
                String ident = a.ToString();
                String data = b.ToString();

                if (!String.IsNullOrEmpty(ident) && !String.IsNullOrEmpty(data))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.SendCustomData(ident, data);
                }
            }
        }

        [JSFunction(Name = "sendEmote", IsEnumerable = true, IsWritable = false)]
        public void R_SendEmote(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.SendEmote(str);
                }
            }
        }

        [JSFunction(Name = "sendText", IsEnumerable = true, IsWritable = false)]
        public void R_SendText(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.SendText(str);
                }
            }
        }

        [JSFunction(Name = "sendPersonalMessage", IsEnumerable = true, IsWritable = false)]
        public void R_SendPersonalMessage(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    if (!String.IsNullOrEmpty(str))
                        r.SendPersonalMessage(str);
                    else
                        r.SendPersonalMessage();
            }
        }

        [JSFunction(Name = "user", IsEnumerable = true, IsWritable = false)]
        public Objects.JSUser R_GetUser(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();

                if (!String.IsNullOrEmpty(str))
                {
                    foreach (Objects.JSUser u in this.UserList)
                        if (u.U_Name == str)
                            return u;

                    foreach (Objects.JSUser u in this.UserList)
                        if (u.U_Name.StartsWith(str))
                            return u;
                }
            }

            return null;
        }

        [JSFunction(Name = "users", IsEnumerable = true, IsWritable = false)]
        public void R_Users(object a)
        {
            if (a is UserDefinedFunction)
            {
                UserDefinedFunction f = (UserDefinedFunction)a;

                this.UserList.ForEach(x =>
                {
                    try { f.Call(this.Engine.Global, x); }
                    catch (Jurassic.JavaScriptException je)
                    {
                        ScriptManager.ErrorHandler(f.Engine.ScriptName, je.LineNumber, je.Message);
                    }
                    catch { }
                });
            }
        }

        [JSProperty(Name = "hashlink")]
        public String R_Hashlink
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return Hashlink.EncodeHashlink(r.Credentials);
                else if (this.Creds != null)
                    return Hashlink.EncodeHashlink(this.Creds);
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "name")]
        public String R_Name
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.Credentials.Name;
                else if (this.Creds != null)
                    return this.Creds.Name;
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "selected")]
        public bool R_Selected
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.RoomIsVisible;
                else
                    return false;
            }
            set { }
        }

        [JSProperty(Name = "server")]
        public String R_Server
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.Credentials.Server;
                else if (this.Creds != null)
                    return this.Creds.Server;
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "topic")]
        public String R_Topic
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.Credentials.Topic;
                else if (this.Creds != null)
                    return this.Creds.Topic;
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "userName")]
        public String R_UserName
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.MyName;
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "userLevel")]
        public int R_UserLevel
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.Panel.Userlist.MyLevel;

                return 0;
            }
            set { }
        }
    }
}
