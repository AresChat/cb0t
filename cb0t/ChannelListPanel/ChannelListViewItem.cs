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
        public Bitmap CountImg { get; set; }

        public String XName { get; private set; }
        public ushort XCount { get; private set; }

        public void Dispose()
        {
            this.NameImg.Dispose();
            this.NameImg = null;
            this.TopicImg.Dispose();
            this.TopicImg = null;
            this.CountImg.Dispose();
            this.CountImg = null;
        }

        public ChannelListViewItem(String xn, ushort xc)
        {
            while (this.SubItems.Count < 3)
                this.SubItems.Add(String.Empty);

            this.XName = xn;
            this.XCount = xc;
        }
    }
}
