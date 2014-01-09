using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSHashlinkResult : ObjectInstance
    {
        internal DecryptedHashlink parent;

        internal JSHashlinkResult(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "HashlinkResult"; }
        }

        public JSHashlinkResult(ObjectInstance prototype, DecryptedHashlink obj)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["HashlinkResult"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = obj;
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
            set { }
        }

        [JSProperty(Name = "ip")]
        public String ExternalIP
        {
            get { return this.parent.IP.ToString(); }
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.Port; }
            set { }
        }
    }
}
