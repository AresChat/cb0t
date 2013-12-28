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

        public JSUser(ObjectInstance prototype, User u)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Room"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = u;
        }

        protected override string InternalClassName
        {
            get { return "Room"; }
        }

        // begin

        private User parent { get; set; }
        
        public void SetToNull()
        {
            this.parent = null;
        }

        [JSProperty(Name = "name")]
        public String U_Name
        {
            get { return this.parent.Name; }
            set { }
        }
    }
}
