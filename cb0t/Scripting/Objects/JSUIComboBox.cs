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
    class JSUIComboBox : ObjectInstance, ICustomUI
    {
        internal JSUIComboBox(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIComboBox(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIComboBox"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.Shadow = new List<String>();
            this.UIComboBox = new ComboBox();
            this.UIComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.UIComboBox.Width = 120;
            this.UIComboBox.Location = new Point(32, 32);
            this.ID = String.Empty;
            this._x = 0;
            this._y = 0;
            this._visible = true;
            this._enabled = true;
            this._width = 100;
            this.UIComboBox.SelectedIndexChanged += this.UIComboBoxSelectedIndexChanged;
            parent.UIPanel.Controls.Add(this.UIComboBox);
        }

        protected override string InternalClassName
        {
            get { return "UIComboBox"; }
        }

        // begin

        private ComboBox UIComboBox { get; set; }
        private List<String> Shadow { get; set; }

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void ClickCallback() { }
        public void KeyPressCallback(int k) { }
        public void ValueChangedCallback() { }
        public void ItemDoubleClickCallback() { }
        
        [JSProperty(Name = "id")]
        public String ID { get; set; }

        private void UIComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this._selectedindex = this.UIComboBox.SelectedIndex;

            ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
            {
                Arg = null,
                Element = this,
                EventType = JSUIEventType.SelectedItemChanged
            });
        }

        [JSProperty(Name = "onselectedindexchanged")]
        public UserDefinedFunction OSICFunction { get; set; }
        public void SelectedItemChangedCallback()
        {
            if (this.OSICFunction != null)
                try
                {
                    this.OSICFunction.Call(this);
                }
                catch { }
        }

        private int _selectedindex = -1;
        [JSProperty(Name = "selectedIndex")]
        public int GetOrSetSelectedIndex
        {
            get { return this._selectedindex; }
            set
            {
                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() =>
                    {
                        int i = value;

                        if (i >= 0 && i < this.Shadow.Count)
                            this.UIComboBox.SelectedIndex = i;
                    }));
                else
                {
                    int i = value;

                    if (i >= 0 && i < this.Shadow.Count)
                        this.UIComboBox.SelectedIndex = i;
                }
            }
        }

        [JSFunction(Name = "itemAt", IsEnumerable = true, IsWritable = false)]
        public String GetItemAt(object a)
        {
            if (!(a is Undefined))
            {
                String str = a.ToString();
                int i;

                if (int.TryParse(str, out i))
                    if (i >= 0 && i < this.Shadow.Count)
                        return this.Shadow[i];
            }

            return null;
        }

        [JSFunction(Name = "add", IsEnumerable = true, IsWritable = false)]
        public void AddItem(object a)
        {
            if (a is Undefined)
                return;

            String str = a.ToString();
            this.Shadow.Add(str);

            if (this.UIComboBox.IsHandleCreated)
                this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Items.Add(str)));
            else
                this.UIComboBox.Items.Add(str);
        }

        [JSFunction(Name = "insertAt", IsEnumerable = true, IsWritable = false)]
        public void InsertItemAt(object a, object b)
        {
            if (a is Undefined || b is Undefined)
                return;

            String _index = a.ToString();
            int i;

            if (int.TryParse(a.ToString(), out i))
                if (i >= 0 && i <= this.Shadow.Count)
                {
                    String str = b.ToString();
                    this.Shadow.Insert(i, str);

                    if (this.UIComboBox.IsHandleCreated)
                        this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Items.Insert(i, str)));
                    else
                        this.UIComboBox.Items.Insert(i, str);
                }
        }

        [JSFunction(Name = "removeAt", IsEnumerable = true, IsWritable = false)]
        public void RemoveItemAt(object a)
        {
            if (a is Undefined)
                return;

            String str = a.ToString();
            int i;

            if (int.TryParse(str, out i))
                if (i >= 0 && i < this.Shadow.Count)
                {
                    this.Shadow.RemoveAt(i);

                    if (this.UIComboBox.IsHandleCreated)
                        this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Items.RemoveAt(i)));
                    else
                        this.UIComboBox.Items.RemoveAt(i);
                }
        }

        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public void ClearAllItems()
        {
            this.Shadow.Clear();

            if (this.UIComboBox.IsHandleCreated)
                this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Items.Clear()));
            else
                this.UIComboBox.Items.Clear();
        }

        [JSProperty(Name = "length")]
        public int ItemCount
        {
            get { return this.Shadow.Count; }
            set { }
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

                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Location = new Point((value + 32), this.UIComboBox.Location.Y)));
                else
                    this.UIComboBox.Location = new Point((value + 32), this.UIComboBox.Location.Y);
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

                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Location = new Point(this.UIComboBox.Location.X, (value + 32))));
                else
                    this.UIComboBox.Location = new Point(this.UIComboBox.Location.X, (value + 32));
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

                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Visible = value));
                else
                    this.UIComboBox.Visible = value;
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

                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Enabled = value));
                else
                    this.UIComboBox.Enabled = value;
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

                if (this.UIComboBox.IsHandleCreated)
                    this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.Width = value));
                else
                    this.UIComboBox.Width = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIComboBox.IsHandleCreated)
                this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.BringToFront()));
            else
                this.UIComboBox.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIComboBox.IsHandleCreated)
                this.UIComboBox.BeginInvoke((Action)(() => this.UIComboBox.SendToBack()));
            else
                this.UIComboBox.SendToBack();
        }
    }
}
