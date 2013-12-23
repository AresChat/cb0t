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

    }
}
