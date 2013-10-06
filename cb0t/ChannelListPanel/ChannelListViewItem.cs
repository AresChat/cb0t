using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t
{
    class ChannelListViewItem : ListViewItem, IDisposable
    {
        public Bitmap NameImg { get; set; }
        public Bitmap TopicImg { get; set; }

        public void Dispose()
        {
            this.NameImg.Dispose();
            this.TopicImg.Dispose();
        }

        public ChannelListViewItem()
        {
            while (this.SubItems.Count < 2)
                this.SubItems.Add(String.Empty);
        }
    }
}
