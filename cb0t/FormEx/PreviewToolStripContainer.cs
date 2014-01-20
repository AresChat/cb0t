using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace FormEx.PreviewToolStripEx
{
    public class PreviewToolStripContainer
    {
        public event EventHandler<PreviewToolStripItemClickedEventArgs> ItemClicked;

        private List<PreviewToolStripItem> Items { get; set; }
        private DwmForm Owner { get; set; }

        public PreviewToolStripItem this[int index]
        {
            get { return this.Items[index]; }
        }

        public PreviewToolStripContainer(DwmForm f)
        {
            this.Items = new List<PreviewToolStripItem>();
            this.Owner = f;
        }

        public void CreateItems(params PreviewToolStripItem[] items)
        {
            List<Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton> list = new List<Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton>();

            foreach (PreviewToolStripItem i in items)
            {
                i.ButtonClicked += this.ButtonClicked;
                this.Items.Add(i);
                list.Add(i.Button);
            }

            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Owner.Handle, list.ToArray());
        }

        private void ButtonClicked(object sender, PreviewToolStripItemClickedEventArgs e)
        {
            this.ItemClicked(this, e);
        }
    }
}
