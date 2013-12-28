using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSFont : ObjectInstance
    {
        internal JSFont(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSFont(ObjectInstance prototype, AresFont font)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Font"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this._nc = font.NameColor;
            this._tc = font.TextColor;
            this._fn = font.FontName;
            this._s = font.Size;
        }

        protected override string InternalClassName
        {
            get { return "Font"; }
        }

        // begin

        private String _nc;
        [JSProperty(Name = "nameColor")]
        public String F_NameColor
        {
            get { return this._nc; }
            set { }
        }

        private String _tc;
        [JSProperty(Name = "textColor")]
        public String F_TextColor
        {
            get { return this._tc; }
            set { }
        }

        private String _fn;
        [JSProperty(Name = "fontName")]
        public String F_FontName
        {
            get { return this._fn; }
            set { }
        }

        private int _s;
        [JSProperty(Name = "fontSize")]
        public int F_Size
        {
            get { return this._s; }
            set { }
        }
    }
}
