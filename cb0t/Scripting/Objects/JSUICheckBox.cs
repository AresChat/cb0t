using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t.Scripting.Objects
{
    class JSUICheckBox : ObjectInstance, ICustomUI
    {
        internal JSUICheckBox(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUICheckBox(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UICheckBox"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UICheckBox = new CheckBox();
            this.UICheckBox.Location = new Point(32, 32);
            this.UICheckBox.AutoSize = true;
            this.UICheckBox.UseVisualStyleBackColor = true;
            this._x = 0;
            this._y = 0;
            this.UICheckBox.Text = String.Empty;
            this._value = String.Empty;
            this.ID = String.Empty;
            this._visible = true;
            this._enabled = true;
            this._checked = false;
            this.UICheckBox.CheckedChanged += this.UICheckBoxCheckedChanged;
            parent.UIPanel.Controls.Add(this.UICheckBox);
        }

        protected override string InternalClassName
        {
            get { return "UICheckBox"; }
        }

        // begin

        public String Group { get { return String.Empty; } set { } }

        private bool can_do_change_event = true;
        private CheckBox UICheckBox { get; set; }

        public void ClickCallback() { }
        public void KeyPressCallback(int k) { }
        public void SelectCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        [JSProperty(Name = "onchange")]
        public UserDefinedFunction OnChangeFunction { get; set; }
        public void ValueChangedCallback()
        {
            if (this.OnChangeFunction != null)
                try
                {
                    this.OnChangeFunction.Call(this);
                }
                catch { }
        }

        private void UICheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this._checked = this.UICheckBox.Checked;

            if (this.can_do_change_event)
                ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
                {
                    Arg = null,
                    Element = this,
                    EventType = JSUIEventType.ValueChanged
                });
        }

        [JSProperty(Name = "id")]
        public String ID { get; set; }

        private int _x;
        [JSProperty(Name = "x")]
        public int X
        {
            get { return this._x; }
            set
            {
                if (value < 0)
                    return;

                this._x = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() => this.UICheckBox.Location = new Point((value + 32), this.UICheckBox.Location.Y)));
                else
                    this.UICheckBox.Location = new Point((value + 32), this.UICheckBox.Location.Y);
            }
        }

        private int _y;
        [JSProperty(Name = "y")]
        public int Y
        {
            get { return this._y; }
            set
            {
                if (value < 0)
                    return;

                this._y = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() => this.UICheckBox.Location = new Point(this.UICheckBox.Location.X, (value + 32))));
                else
                    this.UICheckBox.Location = new Point(this.UICheckBox.Location.X, (value + 32));
            }
        }

        private String _value;
        [JSProperty(Name = "value")]
        public String Value
        {
            get { return this._value; }
            set
            {
                this._value = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() => this.UICheckBox.Text = value));
                else
                    this.UICheckBox.Text = value;
            }
        }

        private bool _visible;
        [JSProperty(Name = "visible")]
        public bool Visible
        {
            get { return this._visible; }
            set
            {
                this._visible = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() => this.UICheckBox.Visible = value));
                else
                    this.UICheckBox.Visible = value;
            }
        }

        private bool _enabled;
        [JSProperty(Name = "enabled")]
        public bool Enabled
        {
            get { return this._enabled; }
            set
            {
                this._enabled = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() => this.UICheckBox.Enabled = value));
                else
                    this.UICheckBox.Enabled = value;
            }
        }

        private bool _checked;
        [JSProperty(Name = "checked")]
        public bool Checked
        {
            get { return this._checked; }
            set
            {
                this._checked = value;

                if (this.UICheckBox.IsHandleCreated)
                    this.UICheckBox.BeginInvoke((Action)(() =>
                    {
                        this.can_do_change_event = false;
                        this.UICheckBox.Checked = value;
                        this.can_do_change_event = true;
                    }));
                else
                {
                    this.can_do_change_event = false;
                    this.UICheckBox.Checked = value;
                    this.can_do_change_event = true;
                }
            }
        }
    }
}
