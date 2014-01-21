using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSUser : ObjectInstance
    {
        internal JSUser(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUser(ObjectInstance prototype, User u, IPEndPoint ep)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["User"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = u;
            this.EndPoint = ep;
        }

        private IPEndPoint EndPoint { get; set; }

        protected override string InternalClassName
        {
            get { return "User"; }
        }

        // begin

        private User parent { get; set; }

        public void SetToNull()
        {
            this.parent = null;
        }

        [JSProperty(Name = "age")]
        public int Age
        {
            get { return this.parent.Age; }
            set { }
        }

        [JSProperty(Name = "away")]
        public bool Away
        {
            get { return this.parent.IsAway; }
            set { }
        }

        [JSProperty(Name = "browsable")]
        public bool Browsable
        {
            get { return this.parent.HasFiles; }
            set { }
        }

        [JSProperty(Name = "country")]
        public String Country
        {
            get { return this.parent.Country; }
            set { }
        }

        [JSFunction(Name = "deleteAvatar", IsEnumerable = true, IsWritable = false)]
        public bool DeleteAvatar()
        {
            Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

            if (r != null)
                if (this.parent != null)
                {
                    r.DeleteAvatarFromScripting(this.parent);
                    return true;
                }

            return false;
        }

        [JSFunction(Name = "deleteFont", IsEnumerable = true, IsWritable = false)]
        public bool DeleteFont()
        {
            if (this.parent != null)
            {
                this.parent.Font = null;
                return true;
            }

            return false;
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIP
        {
            get { return this.parent.ExternalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "friend")]
        public bool Friend
        {
            get { return this.parent.IsFriend; }
            set
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    if (this.parent.IsFriend != value)
                        Friends.FriendStatusChanged(this.parent.Name);
            }
        }

        [JSProperty(Name = "fontName")]
        public String F_FontName
        {
            get
            {
                if (this.parent.Font != null)
                    return this.parent.Font.FontName;

                return null;
            }
            set { }
        }

        [JSProperty(Name = "fontSize")]
        public int F_Size
        {
            get
            {
                if (this.parent.Font != null)
                    return this.parent.Font.Size;

                return 10;
            }
            set { }
        }

        [JSProperty(Name = "gender")]
        public int Gender
        {
            get { return this.parent.Gender; }
            set { }
        }

        [JSProperty(Name = "ignored")]
        public bool Ignored
        {
            get { return this.parent.Ignored; }
            set
            {
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    if (this.parent.Ignored != value)
                    {
                        this.parent.Ignored = value;
                        r.UpdateIgnoreFromScripting(this.parent);
                    }
            }
        }

        [JSProperty(Name = "level")]
        public int Level
        {
            get { return this.parent.Level; }
            set { }
        }

        [JSProperty(Name = "localIp")]
        public String LocalIP
        {
            get { return this.parent.LocalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "name")]
        public String U_Name
        {
            get { return this.parent.Name; }
            set { }
        }

        [JSProperty(Name = "nameColor")]
        public String F_NameColor
        {
            get
            {
                if (this.parent.Font != null)
                    return this.parent.Font.NameColor;

                return null;
            }
            set { }
        }

        [JSFunction(Name = "nudge", IsWritable = false, IsEnumerable = true)]
        public void Nudge()
        {
            Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

            if (r != null)
                r.NudgeUser(this.parent);
        }

        [JSProperty(Name = "personalMessage")]
        public String PersonalMessage
        {
            get { return this.parent.PersonalMessage; }
            set { }
        }

        [JSFunction(Name = "pm", IsEnumerable = true, IsWritable = false)]
        public void SendPM(object a)
        {
            if (!(a is Undefined))
            {
                String text = a.ToString();
                Room r = RoomPool.Rooms.Find(x => x.EndPoint.Equals(this.EndPoint));

                if (r != null)
                    r.SendJSPM(this.parent.Name, text);
            }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.Port; }
            set { }
        }

        [JSProperty(Name = "region")]
        public String Region
        {
            get { return this.parent.Region; }
            set { }
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
                        r.ScriptScribble(cb.Data, this.U_Name);
                }
            }
        }

        [JSProperty(Name = "textColor")]
        public String F_TextColor
        {
            get
            {
                if (this.parent.Font != null)
                    return this.parent.Font.TextColor;

                return null;
            }
            set { }
        }

        [JSProperty(Name = "writing")]
        public bool Writing
        {
            get { return this.parent.Writing; }
            set { }
        }
    }
}
