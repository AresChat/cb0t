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
    class JSUIListBox : ObjectInstance, ICustomUI
    {
        internal JSUIListBox(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIListBox(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIListBox"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
                script.Elements.Add(this);

            this.Shadow = new List<String>();
            this.UIListBox = new ListBox();
            this.UIListBox.FormattingEnabled = true;
            this.UIListBox.Width = 100;
            this.UIListBox.Height = 100;
            this.UIListBox.ScrollAlwaysVisible = true;
            this.UIListBox.HorizontalScrollbar = false;
            this.UIListBox.Location = new Point(32, 32);
            this._x = 0;
            this._y = 0;
            this.ID = String.Empty;
            this._visible = true;
            this._enabled = true;
            this._width = 100;
            this._height = 100;
            this.UIListBox.SelectedIndexChanged += this.UIListBoxSelectedIndexChanged;
            this.UIListBox.MouseDoubleClick += this.UIListBoxMouseDoubleClick;
            parent.UIPanel.Controls.Add(this.UIListBox);
        }

        protected override string InternalClassName
        {
            get { return "UIListBox"; }
        }

        // begin

        private int _selectedindex = -1;
        [JSProperty(Name = "selectedIndex")]
        public int GetOrSetSelectedIndex
        {
            get { return this._selectedindex; }
            set
            {
                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() =>
                    {
                        int i = value;

                        if (i >= 0 && i < this.Shadow.Count)
                            this.UIListBox.SelectedIndex = i;
                    }));
                else
                {
                    int i = value;
                    
                    if (i >= 0 && i < this.Shadow.Count)
                        this.UIListBox.SelectedIndex = i;
                }
            }
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

        private void UIListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this._selectedindex = this.UIListBox.SelectedIndex;
            
            ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
            {
                Arg = null,
                Element = this,
                EventType = JSUIEventType.SelectedItemChanged
            });
        }

        [JSProperty(Name = "ondoubleclick")]
        public UserDefinedFunction ODCFunction { get; set; }
        public void ItemDoubleClickCallback()
        {
            if (this.ODCFunction != null)
                try
                {
                    this.ODCFunction.Call(this);
                }
                catch { }
        }

        private void UIListBoxMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (this.UIListBox.SelectedIndex > -1)
                    ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
                    {
                        Arg = null,
                        Element = this,
                        EventType = JSUIEventType.ItemDoubleClick
                    });
        }

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void ClickCallback() { }
        public void KeyPressCallback(int k) { }
        public void ValueChangedCallback() { }

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

            if (this.UIListBox.IsHandleCreated)
                this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Items.Add(str)));
            else
                this.UIListBox.Items.Add(str);
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

                    if (this.UIListBox.IsHandleCreated)
                        this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Items.Insert(i, str)));
                    else
                        this.UIListBox.Items.Insert(i, str);
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

                    if (this.UIListBox.IsHandleCreated)
                        this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Items.RemoveAt(i)));
                    else
                        this.UIListBox.Items.RemoveAt(i);
                }
        }

        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public void ClearAllItems()
        {
            this.Shadow.Clear();

            if (this.UIListBox.IsHandleCreated)
                this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Items.Clear()));
            else
                this.UIListBox.Items.Clear();
        }

        [JSProperty(Name = "length")]
        public int ItemCount
        {
            get { return this.Shadow.Count; }
            set { }
        }

        [JSProperty(Name = "id")]
        public String ID { get; set; }

        private ListBox UIListBox { get; set; }
        private List<String> Shadow { get; set; }

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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Location = new Point((value + 32), this.UIListBox.Location.Y)));
                else
                    this.UIListBox.Location = new Point((value + 32), this.UIListBox.Location.Y);
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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Location = new Point(this.UIListBox.Location.X, (value + 32))));
                else
                    this.UIListBox.Location = new Point(this.UIListBox.Location.X, (value + 32));
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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Visible = value));
                else
                    this.UIListBox.Visible = value;
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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Enabled = value));
                else
                    this.UIListBox.Enabled = value;
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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Width = value));
                else
                    this.UIListBox.Width = value;
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

                if (this.UIListBox.IsHandleCreated)
                    this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.Height = value));
                else
                    this.UIListBox.Height = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIListBox.IsHandleCreated)
                this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.BringToFront()));
            else
                this.UIListBox.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIListBox.IsHandleCreated)
                this.UIListBox.BeginInvoke((Action)(() => this.UIListBox.SendToBack()));
            else
                this.UIListBox.SendToBack();
        }
    }
}
