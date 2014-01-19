using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t
{
    class PMTab : TabPage
    {
        private Panel container { get; set; }
        private PMRecPanel rec { get; set; }
        private RtfScreen rtf { get; set; }

        public event EventHandler HashlinkClicked;
        public event EventHandler EditScribbleClicked;

        public bool AutoReplySent { get; set; }
        public bool First { get; set; }

        public PMTab(String name)
        {
            this.AutoReplySent = false;
            this.rec = new PMRecPanel();
            this.rec.BackColor = Color.White;
            this.rec.Dock = DockStyle.Bottom;
            this.rec.Location = new Point(0, 233);
            this.rec.Recording = false;
            this.rec.RecordingTime = 0;
            this.rec.Size = new Size(540, 16);

            this.rtf = new RtfScreen();
            this.rtf.BackColor = Color.White;
            this.rtf.BorderStyle = BorderStyle.None;
            this.rtf.Dock = DockStyle.Fill;
            this.rtf.HideSelection = false;
            this.rtf.Location = new Point(0, 0);
            this.rtf.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.rtf.Size = new Size(540, 233);
            this.rtf.HashlinkClicked += this.LinkHashlinkClicked;
            this.rtf.EditScribbleClicked += this.rtf_EditScribbleClicked;

            this.container = new Panel();
            this.container.BackColor = Color.White;
            this.container.BorderStyle = BorderStyle.FixedSingle;
            this.container.Controls.Add(this.rtf);
            this.container.Controls.Add(this.rec);
            this.container.Dock = DockStyle.Fill;
            this.container.Location = new Point(0, 0);
            this.container.Size = new Size(542, 251);

            this.Location = new Point(4, 22);
            this.Size = new Size(542, 251);
            this.Text = name;
            this.ImageIndex = 2;
            this.UseVisualStyleBackColor = true;
            this.Controls.Add(this.container);
        }

        private void rtf_EditScribbleClicked(object sender, EventArgs e)
        {
            this.EditScribbleClicked(sender, e);
        }

        public void UpdateTemplate()
        {
            this.rtf.UpdateTemplate();
        }

        public void SetToBlack()
        {
            this.rec.IsBlack = true;
            this.rec.Invalidate();
            this.rtf.IsBlack = true;
            this.rtf.BackColor = Color.Black;
        }

        public void UpdateVoiceTime(int seconds)
        {
            if (seconds == -1)
            {
                this.rec.Recording = false;
                this.rec.RecordingTime = 0;
            }
            else
            {
                this.rec.Recording = true;
                this.rec.RecordingTime = seconds;
            }

            this.rec.Invalidate();
        }

        private void LinkHashlinkClicked(object sender, EventArgs e)
        {
            this.HashlinkClicked(sender, e);
        }

        public void SetRead(bool read)
        {
            this.BeginInvoke((Action)(() => this.ImageIndex = read ? 1 : 2));
        }

        public void PM(String name, String text, AresFont font)
        {
            this.rtf.ShowPMText(name, text, font);
        }

        public void Announce(String text)
        {
            this.rtf.ShowAnnounceText(text);
        }

        public void Scribble(byte[] data)
        {
            this.rtf.Scribble(data);
        }

        public void Free()
        {
            while (this.Controls.Count > 0)
                this.Controls.RemoveAt(0);

            while (this.container.Controls.Count > 0)
                this.container.Controls.RemoveAt(0);

            this.container.Dispose();
            this.container = null;
            this.rec.Destroy();
            this.rec.Dispose();
            this.rec = null;
            this.rtf.HashlinkClicked -= this.LinkHashlinkClicked;
            this.rtf.EditScribbleClicked -= this.rtf_EditScribbleClicked;
            this.rtf.Free();
            this.rtf.Dispose();
            this.rtf = null;
        }
    }
}
