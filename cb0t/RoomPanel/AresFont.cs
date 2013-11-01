using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public class AresFont
    {
        public String NameColor { get; set; }
        public String TextColor { get; set; }
        public String FontName { get; set; }
        public int Size { get; set; }

        public AresFont Copy()
        {
            AresFont f = new AresFont();
            f.NameColor = "#" + this.NameColor;
            f.TextColor = "#" + this.TextColor;
            f.FontName = this.FontName;
            f.Size = this.Size;
            return f;
        }
    }
}
