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
    class JSUITextArea : ObjectInstance, ICustomUI
    {
        internal JSUITextArea(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUITextArea(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UITextArea"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UITextBox = new TextBox();
            this.UITextBox.Font = new Font("Tahoma", (float)9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.UITextBox.Multiline = true;
            this.UITextBox.ScrollBars = ScrollBars.Vertical;
            this.UITextBox.Text = String.Empty;
            this._value = String.Empty;
            this.UITextBox.Location = new Point(32, 32);
            this._x = 0;
            this._y = 0;
            this.UITextBox.Width = 100;
            this._width = 100;
            this.UITextBox.Height = 100;
            this._height = 100;
            this.ID = String.Empty;
            this._visible = true;
            this._enabled = true;
            this._readonly = false;
            this.UITextBox.KeyPress += this.UITextBoxKeyPress;
            this.UITextBox.TextChanged += this.UITextBoxTextChanged;
            this.UITextBox.MouseClick += this.UITextBoxMouseClick;
            parent.UIPanel.Controls.Add(this.UITextBox);
        }

        protected override string InternalClassName
        {
            get { return "UITextArea"; }
        }

        // begin

        private TextBox UITextBox { get; set; }

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        private bool can_do_change_event = true;
        private void UITextBoxTextChanged(object sender, EventArgs e)
        {
            this._value = this.UITextBox.Text;

            if (this.can_do_change_event)
                ScriptManager.PendingUIEvents.Enqueue(new JSUIEventItem
                {
                    Arg = null,
                    Element = this,
                    EventType = JSUIEventType.ValueChanged
                });
        }

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

        private void UITextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            ScriptManager.PendingUIEvents.Enqueue(new JSUIEventItem
            {
                Arg = (int)e.KeyChar,
                Element = this,
                EventType = JSUIEventType.KeyPressed
            });
        }

        [JSProperty(Name = "onkeypress")]
        public UserDefinedFunction OnKeyPressFunction { get; set; }
        public void KeyPressCallback(int k)
        {
            if (this.OnKeyPressFunction != null)
                try
                {
                    this.OnKeyPressFunction.Call(this, k);
                }
                catch { }
        }

        private void UITextBoxMouseClick(object sender, MouseEventArgs e)
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
                catch { }
        }

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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() =>
                    {
                        this.can_do_change_event = false;
                        this.UITextBox.Text = value;
                        this.can_do_change_event = true;
                    }));
                else
                {
                    this.can_do_change_event = false;
                    this.UITextBox.Text = value;
                    this.can_do_change_event = true;
                }
            }
        }

        private bool _readonly;
        [JSProperty(Name = "readOnly")]
        public bool ReadOnly
        {
            get { return this._readonly; }
            set
            {
                this._readonly = value;

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.ReadOnly = value));
                else
                    this.UITextBox.ReadOnly = value;
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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Location = new Point((value + 32), this.UITextBox.Location.Y)));
                else
                    this.UITextBox.Location = new Point((value + 32), this.UITextBox.Location.Y);
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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Location = new Point(this.UITextBox.Location.X, (value + 32))));
                else
                    this.UITextBox.Location = new Point(this.UITextBox.Location.X, (value + 32));
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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Width = value));
                else
                    this.UITextBox.Width = value;
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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Height = value));
                else
                    this.UITextBox.Height = value;
            }
        }

        [JSFunction(Name = "focus", IsEnumerable = true, IsWritable = false)]
        public void Focus()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Focus()));
            else
                this.UITextBox.Focus();
        }

        [JSFunction(Name = "scrollToTop", IsEnumerable = true, IsWritable = false)]
        public void ScrollToTop()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() =>
                {
                    this.UITextBox.SelectionStart = 0;
                    this.UITextBox.SelectionLength = 0;
                    this.UITextBox.ScrollToCaret();
                }));
            else
            {
                this.UITextBox.SelectionStart = 0;
                this.UITextBox.SelectionLength = 0;
                this.UITextBox.ScrollToCaret();
            }
        }

        [JSFunction(Name = "scrollToBottom", IsEnumerable = true, IsWritable = false)]
        public void ScrollToBottom()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() =>
                {
                    this.UITextBox.SelectionStart = this.UITextBox.Text.Length;
                    this.UITextBox.SelectionLength = 0;
                    this.UITextBox.ScrollToCaret();
                }));
            else
            {
                this.UITextBox.SelectionStart = this.UITextBox.Text.Length;
                this.UITextBox.SelectionLength = 0;
                this.UITextBox.ScrollToCaret();
            }
        }

        [JSFunction(Name = "append", IsEnumerable = true, IsWritable = false)]
        public void Append(object a)
        {
            if (!(a is Undefined))
            {
                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.AppendText(a.ToString())));
                else
                    this.UITextBox.AppendText(a.ToString());
            }
        }

        [JSFunction(Name = "appendLine", IsEnumerable = true, IsWritable = false)]
        public void AppendLine(object a)
        {
            if (!(a is Undefined))
            {
                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.AppendText(a + "\r\n")));
                else
                    this.UITextBox.AppendText(a + "\r\n");
            }
        }

        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public void Clear()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Clear()));
            else
                this.UITextBox.Clear();
        }

        private bool _visible;
        [JSProperty(Name = "visible")]
        public bool Visible
        {
            get { return this._visible; }
            set
            {
                this._visible = value;

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Visible = value));
                else
                    this.UITextBox.Visible = value;
            }
        }

        private bool _mono;
        [JSProperty(Name = "mono")]
        public bool Mono
        {
            get { return this._mono; }
            set
            {
                this._mono = value;

                if (this.UITextBox.IsHandleCreated)
                {
                    this.UITextBox.BeginInvoke((Action)(() =>
                    {
                        this.UITextBox.Font.Dispose();
                        this.UITextBox.Font = new Font(value ? "Courier New" : "Tahoma", (float)9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    }));
                }
                else
                {
                    this.UITextBox.Font.Dispose();
                    this.UITextBox.Font = new Font(value ? "Courier New" : "Tahoma", (float)9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                }
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

                if (this.UITextBox.IsHandleCreated)
                    this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.Enabled = value));
                else
                    this.UITextBox.Enabled = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.BringToFront()));
            else
                this.UITextBox.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UITextBox.IsHandleCreated)
                this.UITextBox.BeginInvoke((Action)(() => this.UITextBox.SendToBack()));
            else
                this.UITextBox.SendToBack();
        }
    }
}
