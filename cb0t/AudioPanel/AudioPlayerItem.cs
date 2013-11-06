using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class AudioPlayerItem : ListViewItem
    {
        public AudioPlayerItem()
        {
            while (this.SubItems.Count < 4)
                this.SubItems.Add(new ListViewSubItem());

            this.ImageIndex = 0;
        }

        public bool Playing
        {
            get { return this.ImageIndex == 1; }
            set { this.ImageIndex = value ? 1 : 0; }
        }

        public bool Exists
        {
            get
            {
                bool result = false;

                try { result = File.Exists(this.Path); }
                catch { }

                return result;
            }
        }

        public int Duration { get; set; }

        public void SetDurationText(int d)
        {
            TimeSpan ts = new TimeSpan(0, 0, d);
            this.SubItems[3].Text = String.Format("{0:0}:{1:00}", Math.Floor(ts.TotalMinutes), ts.Seconds);
        }

        public String Title
        {
            get { return this.SubItems[0].Text; }
            set { this.SubItems[0].Text = value; }
        }

        public String Path { get; set; }

        public String Artist
        {
            get { return this.SubItems[1].Text; }
            set { this.SubItems[1].Text = value; }
        }

        public String Author { get; set; }

        public String Album
        {
            get { return this.SubItems[2].Text; }
            set { this.SubItems[2].Text = value; }
        }

        public String ToAudioTextString()
        {
            if (!String.IsNullOrEmpty(this.Artist))
                return this.Artist + " - " + this.Title;

            if (!String.IsNullOrEmpty(this.Author))
                return this.Author + " - " + this.Title;

            return this.Title;
        }
    }
}
