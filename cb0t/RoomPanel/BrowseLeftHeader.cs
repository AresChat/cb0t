using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace cb0t
{
    public partial class BrowseLeftHeader : UserControl
    {
        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);

        public String HeaderText { get; set; }

        public BrowseLeftHeader()
        {
            this.InitializeComponent();
            this.HeaderText = "Loading...";
        }

        private void BrowseLeftHeader_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            using (Pen pen = new Pen(Color.Gray, 1))
                e.Graphics.DrawRectangle(pen, new Rectangle(r.X, r.Y, r.Width - 1, r.Height));

            e.Graphics.DrawString(this.HeaderText, this.Font, this.column_text_brush, new PointF(3, 6));
        }

        public void ReleaseResources()
        {
            this.Paint -= this.BrowseLeftHeader_Paint;
            this.column_text_brush.Dispose();
        }
    }
}
