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
        public static EmoticonMenu EMenu { get; set; }
        public static ColorMenu CMenu { get; set; }
        public static OpenFileDialog OpenFile { get; set; }
        public static FolderBrowserDialog OpenFolder { get; set; }
        public static ScribbleDownloader ScribbleDownloader { get; set; }

        public static void Init()
        {
            ColorPicker = new CustomColorPicker();
            ScribbleEditor = new ScribbleEditor();
            SaveFile = new SaveFileDialog();
            EMenu = new EmoticonMenu();
            CMenu = new ColorMenu();
            OpenFile = new OpenFileDialog();
            OpenFolder = new FolderBrowserDialog();
            ScribbleDownloader = new ScribbleDownloader();
            ScribbleDownloader.PrepareAnimation();
        }

        public static void UpdateTemplate()
        {
            ScribbleEditor.UpdateTemplate();
            ColorPicker.UpdateTemplate();
            EMenu.UpdateTemplate();
        }
    }
}
