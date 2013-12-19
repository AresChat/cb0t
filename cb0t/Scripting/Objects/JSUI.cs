using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSUI : ObjectInstance
    {
        internal JSUI(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUI(ObjectInstance prototype)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UI"]).InstancePrototype)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "UI"; }
        }

        // begin

        public CustomScriptSettings UIPanel { get; private set; }
        public bool CanCreate { get; set; }
        public bool CanAddControls { get; set; }

        [JSFunction(Name = "createTextBox", IsWritable = false, IsEnumerable = true)]
        public JSUITextBox CreateTextBox()
        {
            if (this.CanAddControls)
                return new JSUITextBox(this.Engine.Object.InstancePrototype, this);
            else
                return null;
        }

        [JSFunction(Name = "createTextArea", IsWritable = false, IsEnumerable = true)]
        public JSUITextArea CreateTextArea()
        {
            if (this.CanAddControls)
                return new JSUITextArea(this.Engine.Object.InstancePrototype, this);
            else
                return null;
        }

        [JSFunction(Name = "create", IsWritable = false, IsEnumerable = true)]
        public bool Create()
        {
            if (this.CanCreate)
            {
                this.UIPanel = Form1.SettingsContent.CreateCustomScriptSettings(this.Engine.ScriptName);
                this.CanCreate = false;
                this.CanAddControls = true;
                return true;
            }

            return false;
        }


    }
}
