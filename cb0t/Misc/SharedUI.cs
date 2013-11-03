using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class SharedUI
    {
        public static CustomColorPicker ColorPicker { get; set; }
        public static ScribbleEditor ScribbleEditor { get; set; }
        public static SaveFileDialog SaveFile { get; set; }

        public static void Init()
        {
            ColorPicker = new CustomColorPicker();
            ScribbleEditor = new ScribbleEditor();
            SaveFile = new SaveFileDialog();
        }
    }
}
