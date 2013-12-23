using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t.Scripting.Objects
{
    class JSUIImage : ObjectInstance, ICustomUI
    {
        private String DataPath { get; set; }

        internal JSUIImage(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSUIImage(ObjectInstance prototype, JSUI parent)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UIImage"]).InstancePrototype)
        {
            this.PopulateFunctions();

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);

            if (script != null)
            {
                script.Elements.Add(this);
                this.DataPath = script.DataPath;
            }

            this.UIImage = new PictureBox();
            this.UIImage.BackColor = Color.Transparent;
            this.UIImage.Width = 200;
            this.UIImage.Height = 200;
            this.UIImage.Location = new Point(32, 32);
            this.UIImage.SizeMode = PictureBoxSizeMode.Normal;
            this.ID = String.Empty;
            this._x = 0;
            this._y = 0;
            this._visible = true;
            this._width = 200;
            this._height = 200;
            this._resize = false;
            this._hand = false;
            this.UIImage.MouseClick += this.UIImageMouseClick;
            parent.UIPanel.Controls.Add(this.UIImage);
        }

        protected override string InternalClassName
        {
            get { return "UIImage"; }
        }

        // begin

        private PictureBox UIImage { get; set; }
        private Bitmap Img { get; set; }

        public String Group { get { return String.Empty; } set { } }
        public void SelectCallback() { }
        public void KeyPressCallback(int k) { }
        public void ValueChangedCallback() { }
        public void ItemDoubleClickCallback() { }
        public void SelectedItemChangedCallback() { }

        private void UIImageMouseClick(object sender, MouseEventArgs e)
        {
            if (!this._hand)
                return;

            if (e.Button == MouseButtons.Left)
                ScriptManager.PendingEvents.Enqueue(new JSUIEventItem
                {
                    Arg = null,
                    Element = this,
                    EventType = JSUIEventType.Click
                });
        }

        [JSProperty(Name = "onclick")]
        public UserDefinedFunction ClickFunction { get; set; }
        public void ClickCallback()
        {
            if (this.ClickFunction != null)
                try
                {
                    this.ClickFunction.Call(this);
                }
                catch { }
        }

        [JSFunction(Name = "load", IsEnumerable = true, IsWritable = false)]
        public void LoadImage(object a)
        {
            if (a is Undefined || String.IsNullOrEmpty(this.DataPath))
                return;

            if (this.UIImage.IsHandleCreated)
                this.UIImage.BeginInvoke((Action)(() =>
                {
                    String path = Path.Combine(this.DataPath, a.ToString());

                    if (new FileInfo(path).Directory.FullName != new DirectoryInfo(this.DataPath).FullName)
                        return;

                    if (this.UIImage.Image != null)
                        this.UIImage.Image = null;

                    if (this.Img != null)
                        this.Img.Dispose();

                    byte[] data = null;

                    try { data = File.ReadAllBytes(path); }
                    catch { }

                    if (data != null)
                    {
                        using (MemoryStream ms = new MemoryStream(data))
                            this.Img = new Bitmap(ms);

                        this.UIImage.Image = this.Img;
                    }
                }));
            else
            {
                String path = Path.Combine(this.DataPath, a.ToString());

                if (new FileInfo(path).Directory.FullName != new DirectoryInfo(this.DataPath).FullName)
                    return;

                if (this.UIImage.Image != null)
                    this.UIImage.Image = null;

                if (this.Img != null)
                    this.Img.Dispose();

                byte[] data = null;

                try { data = File.ReadAllBytes(path); }
                catch { }

                if (data != null)
                {
                    using (MemoryStream ms = new MemoryStream(data))
                        this.Img = new Bitmap(ms);

                    this.UIImage.Image = this.Img;
                }
            }
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

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Location = new Point((value + 32), this.UIImage.Location.Y)));
                else
                    this.UIImage.Location = new Point((value + 32), this.UIImage.Location.Y);
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

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Location = new Point(this.UIImage.Location.X, (value + 32))));
                else
                    this.UIImage.Location = new Point(this.UIImage.Location.X, (value + 32));
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

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Visible = value));
                else
                    this.UIImage.Visible = value;
            }
        }

        private bool _hand;
        [JSProperty(Name = "hand")]
        public bool Hand
        {
            get { return this._hand; }
            set
            {
                this._hand = value;

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Cursor = value ? Cursors.Hand : Cursors.Arrow));
                else
                    this.UIImage.Cursor = value ? Cursors.Hand : Cursors.Arrow;
            }
        }

        private bool _resize;
        [JSProperty(Name = "resize")]
        public bool Resize
        {
            get { return this._resize; }
            set
            {
                this._resize = value;

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() =>
                    {
                        if (this._resize)
                            this.UIImage.SizeMode = PictureBoxSizeMode.StretchImage;
                        else
                            this.UIImage.SizeMode = PictureBoxSizeMode.Normal;
                    }));
                else
                {
                    if (this._resize)
                        this.UIImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    else
                        this.UIImage.SizeMode = PictureBoxSizeMode.Normal;
                }
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

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Width = value));
                else
                    this.UIImage.Width = value;
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

                if (this.UIImage.IsHandleCreated)
                    this.UIImage.BeginInvoke((Action)(() => this.UIImage.Height = value));
                else
                    this.UIImage.Height = value;
            }
        }

        [JSFunction(Name = "promote", IsEnumerable = true, IsWritable = false)]
        public void DoPromote()
        {
            if (this.UIImage.IsHandleCreated)
                this.UIImage.BeginInvoke((Action)(() => this.UIImage.BringToFront()));
            else
                this.UIImage.BringToFront();
        }

        [JSFunction(Name = "demote", IsEnumerable = true, IsWritable = false)]
        public void DoDemote()
        {
            if (this.UIImage.IsHandleCreated)
                this.UIImage.BeginInvoke((Action)(() => this.UIImage.SendToBack()));
            else
                this.UIImage.SendToBack();
        }
    }
}
