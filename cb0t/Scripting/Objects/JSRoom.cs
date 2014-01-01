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

        public JSRoom(ObjectInstance prototype, IPEndPoint ep)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Room"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.EndPoint = ep;
            this.UserList = new List<JSUser>();
        }

        protected override string InternalClassName
        {
            get { return "Room"; }
        }

        // begin

        public IPEndPoint EndPoint { get; private set; }
        public List<JSUser> UserList { get; private set; }

        [JSProperty(Name = "name")]
        public String R_Name
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return r.Credentials.Name;
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
                else
                    return null;
            }
            set { }
        }

        [JSProperty(Name = "hashlink")]
        public String R_Hashlink
        {
            get
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    return Hashlink.EncodeHashlink(r.Credentials);
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
                String data = a.ToString();

                if (!String.IsNullOrEmpty(ident) && !String.IsNullOrEmpty(data))
                {
                    Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                    if (r != null)
                        r.SendCustomData(ident, data);
                }
            }
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
                    catch { }
                });
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
    }
}
