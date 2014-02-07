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
    class JSUIRadioButton : ObjectInstance, ICustomUI
    {
        internal JSUIRadioButton(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIRadioButton(ObjectInstance prototype, JSUI parent, String group)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIRadioButton"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UIRadioButton = new RadioButton();
            this.UIRadioButton.BackColor = Color.Transparent;
            this.UIRadioButton.UseVisualStyleBackColor = true;
            this.UIRadioButton.AutoSize = true;
            this.UIRadioButton.Location = new Point(32, 32);
            this.ID = String.Empty;
            this._group = group;
            this._x = 0;
            this._y = 0;
            this.UIRadioButton.Text = String.Empty;
            this._value = String.Empty;
            this._visible = true;
            this._enabled = true;

            int count = 0;

            if (script != null)
                foreach (ICustomUI ctrl in script.Elements)
                    if (!string.IsNullOrEmpty(ctrl.Group))
                        if (ctrl.Group == group)
                            count++;

            this._checked = count == 1;
            this.UIRadioButton.Checked = count == 1;
            this.UIRadioButton.CheckedChanged += this.UIRadioButtonCheckedChanged;
            parent.UIPanel.Controls.Add(this.UIRadioButton);
        }

        protected override string InternalClassName
        {
            get { return "UIRadioButton"; }
        }

        // begin

        public RadioButton UIRadioButton { get; set; }

        public void KeyPressCallback(int k) { }
        public void ClickCallback() { }
        public void ValueChangedCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        [JSProperty(Name = "onselect")]
        public UserDefinedFunction OnSelectFunction { get; set; }
        public void SelectCallback()
        {
            if (this.OnSelectFunction != null)
                try
                {
                    this.OnSelectFunction.Call(this);
                }
                catch { }
        }

        public void ForceUnselect()
        {
            this._checked = false;

            if (this.UIRadioButton.IsHandleCreated)
                this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Checked = false));
            else
                this.UIRadioButton.Checked = false;
        }

        private void UIRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (this.UIRadioButton.Checked)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

                if (script != null)
                    foreach (ICustomUI ctrl in script.Elements)
                        if (!string.IsNullOrEmpty(ctrl.Group))
                            if (ctrl.Group == this._group)
                            {
                                JSUIRadioButton r = (JSUIRadioButton)ctrl;

                                if (!r.UIRadioButton.Equals(this.UIRadioButton))
                                    r.ForceUnselect();
                            }

                this._checked = true;

                if (!this._manual_select)
                    ScriptManager.PendingUIEvents.Enqueue(new JSUIEventItem
                    {
                        Arg = null,
                        Element = this,
                        EventType = JSUIEventType.Select
                    });

                this._manual_select = false;
            }
        }

        [JSProperty(Name = "id")]
        public String ID { get; set; }

        private String _group;
        [JSProperty(Name = "group")]
        public String Group
        {
            get { return this._group; }
            set { }
        }

        private bool _checked;
        private bool _manual_select = false;
        [JSProperty(Name = "selected")]
        public bool Checked
        {
            get { return this._checked; }
            set
            {
                if (value && !this._checked)
                {
                    this._manual_select = true;

                    if (this.UIRadioButton.IsHandleCreated)
                        this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Checked = true));
                    else
                        this.UIRadioButton.Checked = true;
                }
            }
        }

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

                if (this.UIRadioButton.IsHandleCreated)
                    this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Location = new Point((value + 32), this.UIRadioButton.Location.Y)));
                else
                    this.UIRadioButton.Location = new Point((value + 32), this.UIRadioButton.Location.Y);
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

                if (this.UIRadioButton.IsHandleCreated)
                    this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Location = new Point(this.UIRadioButton.Location.X, (value + 32))));
                else
                    this.UIRadioButton.Location = new Point(this.UIRadioButton.Location.X, (value + 32));
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

                if (this.UIRadioButton.IsHandleCreated)
                    this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Text = value));
                else
                    this.UIRadioButton.Text = value;
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

                if (this.UIRadioButton.IsHandleCreated)
                    this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Visible = value));
                else
                    this.UIRadioButton.Visible = value;
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

                if (this.UIRadioButton.IsHandleCreated)
                    this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.Enabled = value));
                else
                    this.UIRadioButton.Enabled = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIRadioButton.IsHandleCreated)
                this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.BringToFront()));
            else
                this.UIRadioButton.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIRadioButton.IsHandleCreated)
                this.UIRadioButton.BeginInvoke((Action)(() => this.UIRadioButton.SendToBack()));
            else
                this.UIRadioButton.SendToBack();
        }
    }
}
