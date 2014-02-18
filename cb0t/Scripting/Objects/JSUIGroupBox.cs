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
    class JSUIGroupBox : ObjectInstance, ICustomUI
    {
        internal JSUIGroupBox(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIGroupBox(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIGroupBox"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UIGroupBox = new GroupBox();
            this.UIGroupBox.Text = String.Empty;
            this._value = String.Empty;
            this.UIGroupBox.Location = new Point(32, 32);
            this._x = 0;
            this._y = 0;
            this.UIGroupBox.Width = 100;
            this._width = 100;
            this.UIGroupBox.Height = 100;
            this._height = 100;
            this.ID = String.Empty;
            parent.UIPanel.Controls.Add(this.UIGroupBox);
        }

        protected override string InternalClassName
        {
            get { return "UIGroupBox"; }
        }

        private GroupBox UIGroupBox { get; set; }

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }
        public void ValueChangedCallback() { }
        public void KeyPressCallback(int k) { }
        public void ClickCallback() { }

        [JSProperty(Name = "id")]
        public String ID { get; set; }

        private String _value;
        [JSProperty(Name = "value")]
        public String Value
        {
            get { return this._value; }
            set
            {
                this._value = value;

                if (this.UIGroupBox.InvokeRequired)
                    this.UIGroupBox.BeginInvoke((Action)(() =>
                    {
                        this.UIGroupBox.Text = value;
                    }));
                else
                {
                    this.UIGroupBox.Text = value;
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

                if (this.UIGroupBox.InvokeRequired)
                    this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.Location = new Point((value + 32), this.UIGroupBox.Location.Y)));
                else
                    this.UIGroupBox.Location = new Point((value + 32), this.UIGroupBox.Location.Y);
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

                if (this.UIGroupBox.InvokeRequired)
                    this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.Location = new Point(this.UIGroupBox.Location.X, (value + 32))));
                else
                    this.UIGroupBox.Location = new Point(this.UIGroupBox.Location.X, (value + 32));
            }
        }

        private int _width;
        [JSProperty(Name = "width")]
        public int Width
        {
            get { return this._width; }
            set
            {
                if (value < 10)
                    return;

                this._width = value;

                if (this.UIGroupBox.InvokeRequired)
                    this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.Width = value));
                else
                    this.UIGroupBox.Width = value;
            }
        }

        private int _height;
        [JSProperty(Name = "height")]
        public int Height
        {
            get { return this._height; }
            set
            {
                if (value < 10)
                    return;

                this._height = value;

                if (this.UIGroupBox.InvokeRequired)
                    this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.Height = value));
                else
                    this.UIGroupBox.Height = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIGroupBox.InvokeRequired)
                this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.BringToFront()));
            else
                this.UIGroupBox.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIGroupBox.InvokeRequired)
                this.UIGroupBox.BeginInvoke((Action)(() => this.UIGroupBox.SendToBack()));
            else
                this.UIGroupBox.SendToBack();
        }
    }
}
