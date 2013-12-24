using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSRedirect : ObjectInstance
    {
        internal JSRedirect(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSRedirect(ObjectInstance prototype, Redirect r)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Redirect"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.Parent = r;
        }

        protected override string InternalClassName
        {
            get { return "Redirect"; }
        }

        // begin

        private Redirect Parent { get; set; }

        [JSProperty(Name = "name")]
        public String R_Name
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.Name;

                return null;
            }
            set { }
        }

        [JSProperty(Name = "ip")]
        public String R_IP
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.IP.ToString();

                return null;
            }
            set { }
        }

        [JSProperty(Name = "hashlink")]
        public String R_Hashlink
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.Hashlink;

                return null;
            }
            set { }
        }

        [JSProperty(Name = "port")]
        public int R_Port
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.Port;

                return 0;
            }
            set { }
        }
    }
}
