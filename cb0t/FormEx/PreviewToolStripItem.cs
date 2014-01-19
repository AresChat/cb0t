using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FormEx.PreviewToolStripEx
{
    public class PreviewToolStripItem
    {
        public event EventHandler<PreviewToolStripItemClickedEventArgs> ButtonClicked;

        public Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton Button { get; set; }
        public object Tag { get; set; }
        
        private Icon Ico { get; set; }

        public PreviewToolStripItem(Bitmap bmp, String tiptext)
        {
            this.Ico = Icon.FromHandle(bmp.GetHicon());
            this.Button = new Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton(this.Ico, tiptext);
            this.Button.Click += this.Clicked;
        }

        private void Clicked(object sender, Microsoft.WindowsAPICodePack.Taskbar.ThumbnailButtonClickedEventArgs e)
        {
            this.ButtonClicked(null, new PreviewToolStripItemClickedEventArgs { Item = this });
        }
    }
}
