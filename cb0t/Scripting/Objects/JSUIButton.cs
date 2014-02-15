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
    class JSUIButton : ObjectInstance, ICustomUI
    {
        internal JSUIButton(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIButton(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIButton"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UIButton = new Button();
            this.UIButton.UseVisualStyleBackColor = true;
            this.UIButton.Width = 75;
            this.UIButton.Height = 25;
            this.UIButton.Text = String.Empty;
            this.UIButton.Location = new Point(32, 32);
            this._x = 0;
            this._y = 0;
            this._value = String.Empty;
            this._visible = true;
            this._enabled = true;
            this._width = 75;
            this._height = 25;
            this.ID = String.Empty;
            this.UIButton.MouseClick += this.UIButtonMouseClick;
            parent.UIPanel.Controls.Add(this.UIButton);
        }

        protected override string InternalClassName
        {
            get { return "UIButton"; }
        }

        // begin

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        private void UIButtonMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ScriptManager.PendingUIEvents.Enqueue(new JSUIEventItem
                {
                    Arg = null,
                    Element = this,
                    EventType = JSUIEventType.Click
                });
        }

        [JSProperty(Name = "onclick")]
        public UserDefinedFunction OnClickFunction { get; set; }
        public void ClickCallback()
        {
            if (this.OnClickFunction != null)
                try
                {
                    this.OnClickFunction.Call(this);
                }
                catch (Jurassic.JavaScriptException je)
                {
                    ScriptManager.ErrorHandler(this.OnClickFunction.Engine.ScriptName, je.LineNumber, je.Message);
                }
                catch { }
        }

        private Button UIButton { get; set; }

        public void KeyPressCallback(int k) { }
        public void ValueChangedCallback() { }

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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Location = new Point((value + 32), this.UIButton.Location.Y)));
                else
                    this.UIButton.Location = new Point((value + 32), this.UIButton.Location.Y);
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Location = new Point(this.UIButton.Location.X, (value + 32))));
                else
                    this.UIButton.Location = new Point(this.UIButton.Location.X, (value + 32));
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Text = value));
                else
                    this.UIButton.Text = value;
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Visible = value));
                else
                    this.UIButton.Visible = value;
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Enabled = value));
                else
                    this.UIButton.Enabled = value;
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Width = value));
                else
                    this.UIButton.Width = value;
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

                if (this.UIButton.InvokeRequired)
                    this.UIButton.BeginInvoke((Action)(() => this.UIButton.Height = value));
                else
                    this.UIButton.Height = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIButton.InvokeRequired)
                this.UIButton.BeginInvoke((Action)(() => this.UIButton.BringToFront()));
            else
                this.UIButton.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIButton.InvokeRequired)
                this.UIButton.BeginInvoke((Action)(() => this.UIButton.SendToBack()));
            else
                this.UIButton.SendToBack();
        }
    }
}
