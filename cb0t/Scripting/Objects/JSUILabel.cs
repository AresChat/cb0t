using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t.Scripting.Objects
{
    class JSUILabel : ObjectInstance, ICustomUI
    {
        internal JSUILabel(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUILabel(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UILabel"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.UILabel = new Label();
            this.UILabel.Font = new Font("Tahoma", (float)9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.UILabel.AutoSize = true;
            this.UILabel.Location = new Point(32, 32);
            this._x = 0;
            this._y = 0;
            this.UILabel.Text = String.Empty;
            this._value = String.Empty;
            this.ID = String.Empty;
            this._visible = true;
            this._enabled = true;
            this._link = false;
            this._size = 9;
            this.UILabel.ForeColor = Color.Black;
            this.UILabel.MouseClick += this.UILabelMouseClick;
            parent.UIPanel.Controls.Add(this.UILabel);
        }

        protected override string InternalClassName
        {
            get { return "UILabel"; }
        }

        // begin

        private Label UILabel { get; set; }

        public String Group { get { return String.Empty; } set { } }

        public void KeyPressCallback(int k) { }
        public void ValueChangedCallback() { }
        public void SelectCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        private void UILabelMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
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

                if (this.UILabel.IsHandleCreated)
                    this.UILabel.BeginInvoke((Action)(() => this.UILabel.Location = new Point((value + 32), this.UILabel.Location.Y)));
                else
                    this.UILabel.Location = new Point((value + 32), this.UILabel.Location.Y);
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

                if (this.UILabel.IsHandleCreated)
                    this.UILabel.BeginInvoke((Action)(() => this.UILabel.Location = new Point(this.UILabel.Location.X, (value + 32))));
                else
                    this.UILabel.Location = new Point(this.UILabel.Location.X, (value + 32));
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

                if (this.UILabel.IsHandleCreated)
                    this.UILabel.BeginInvoke((Action)(() => this.UILabel.Text = value));
                else
                    this.UILabel.Text = value;
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

                if (this.UILabel.IsHandleCreated)
                    this.UILabel.BeginInvoke((Action)(() => this.UILabel.Visible = value));
                else
                    this.UILabel.Visible = value;
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

                if (this.UILabel.IsHandleCreated)
                    this.UILabel.BeginInvoke((Action)(() => this.UILabel.Enabled = value));
                else
                    this.UILabel.Enabled = value;
            }
        }

        private bool _link;
        [JSProperty(Name = "link")]
        public bool Link
        {
            get { return this._link; }
            set
            {
                this._link = value;

                if (this.UILabel.IsHandleCreated)
                {
                    this.UILabel.BeginInvoke((Action)(() =>
                    {
                        this.UILabel.ForeColor = value ? Color.Blue : Color.Black;
                        this.UILabel.Font.Dispose();
                        this.UILabel.Font = new Font("Tahoma", (float)this._size, value ? FontStyle.Underline : FontStyle.Regular, GraphicsUnit.Point, ((byte)0));
                        this.UILabel.Cursor = value ? Cursors.Hand : Cursors.Arrow;
                    }));
                }
                else
                {
                    this.UILabel.ForeColor = value ? Color.Blue : Color.Black;
                    this.UILabel.Font.Dispose();
                    this.UILabel.Font = new Font("Tahoma", (float)this._size, value ? FontStyle.Underline : FontStyle.Regular, GraphicsUnit.Point, ((byte)0));
                    this.UILabel.Cursor = value ? Cursors.Hand : Cursors.Arrow;
                }
            }
        }

        private int _size;
        [JSProperty(Name = "fontSize")]
        public int Size
        {
            get { return this._size; }
            set
            {
                this._size = value;

                if (this._size < 6)
                    this._size = 6;

                if (this._size > 24)
                    this._size = 24;

                if (this.UILabel.IsHandleCreated)
                {
                    this.UILabel.BeginInvoke((Action)(() =>
                    {
                        this.UILabel.Font.Dispose();
                        this.UILabel.Font = new Font("Tahoma", (float)this._size, this._link ? FontStyle.Underline : FontStyle.Regular, GraphicsUnit.Point, ((byte)0));
                    }));
                }
                else
                {
                    this.UILabel.Font.Dispose();
                    this.UILabel.Font = new Font("Tahoma", (float)this._size, this._link ? FontStyle.Underline : FontStyle.Regular, GraphicsUnit.Point, ((byte)0));
                }
            }
        }
    }
}
