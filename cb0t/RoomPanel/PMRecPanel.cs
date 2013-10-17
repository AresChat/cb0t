using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class PMRecPanel : UserControl
    {
        private Bitmap rec { get; set; }

        public int RecordingTime { get; set; }
        public bool Recording { get; set; }

        public void Destroy()
        {
            this.rec.Dispose();
            this.rec = null;
            this.Paint -= this.PaintRec;
        }

        public PMRecPanel()
        {
            this.InitializeComponent();
            this.rec = (Bitmap)Properties.Resources.mrec.Clone();
            this.ResizeRedraw = true;
            this.RecordingTime = 0;
            this.Paint += this.PaintRec;
        }

        private void PaintRec(object sender, PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.Height));

            if (this.Recording)
            {
                e.Graphics.DrawImage(this.rec, new Point(1, 0));

                using (SolidBrush brush = new SolidBrush(Color.Black))
                    e.Graphics.DrawString("RECORDING [" + (15 - this.RecordingTime) + " seconds remaining]", this.Font, brush, new PointF(16, 1));
            }
        }
    }
}
