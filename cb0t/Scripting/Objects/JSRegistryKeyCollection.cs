using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSRegistryKeyCollection : ObjectInstance
    {
        private int count { get; set; }

        public JSRegistryKeyCollection(ObjectInstance prototype, String[] keys, String scriptName)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["RegistryKeyCollection"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            foreach (String str in keys)
                this.SetPropertyValue((uint)this.count++, str, throwOnError: true);
        }

        internal JSRegistryKeyCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "RegistryKeyCollection"; }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
