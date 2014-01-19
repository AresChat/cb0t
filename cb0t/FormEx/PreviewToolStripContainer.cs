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

        public void CreateItem(Bitmap image)
        {
            this.CreateItem(image, String.Empty, null);
        }

        public void CreateItem(Bitmap image, object tag)
        {
            this.CreateItem(image, String.Empty, tag);
        }

        public void CreateItem(Bitmap image, String tiptext)
        {
            this.CreateItem(image, tiptext, null);
        }

        public void CreateItem(Bitmap image, String tiptext, object tag)
        {
            PreviewToolStripItem item = new PreviewToolStripItem(image, tiptext);
            item.Tag = tag;
            item.ButtonClicked += this.ButtonClicked;
            this.Items.Add(item);
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Owner.Handle, item.Button);
        }

        private void ButtonClicked(object sender, PreviewToolStripItemClickedEventArgs e)
        {
            this.ItemClicked(this, e);
        }
    }
}
