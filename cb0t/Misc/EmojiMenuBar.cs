using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class EmojiMenuBar : ToolStripRenderer
    {
        private Pen bg_pen = new Pen(Color.Gray, 1);
        private SolidBrush bg_brush = new SolidBrush(Color.White);

        public EmojiMenuBarSelectedItem SelectedItem { get; set; }

        public EmojiMenuBar()
        {
            this.SelectedItem = EmojiMenuBarSelectedItem.People;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle r = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
            e.Graphics.FillRectangle(this.bg_brush, r);
            e.Graphics.DrawLine(this.bg_pen, new Point(0, 0), new Point(e.ToolStrip.Width, 0));
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is ToolStripButton)
            {
                EmojiMenuBarSelectedItem i = (EmojiMenuBarSelectedItem)e.Item.Tag;

                if (this.SelectedItem == i)
                {
                    Rectangle rec = new Rectangle(0, 1, e.Item.Bounds.Width - 1, e.Item.Bounds.Height - 2);

                    using (GraphicsPath path = rec.Rounded(3))
                    {
                        using (SolidBrush sb = new SolidBrush(Color.Gainsboro))
                            e.Graphics.FillPath(sb, path);

                        using (Pen pen = new Pen(Color.Gray, 1))
                            e.Graphics.DrawPath(pen, path);
                    }
                }
                else if (e.Item.Selected)
                {
                    Rectangle rec = new Rectangle(0, 1, e.Item.Bounds.Width - 1, e.Item.Bounds.Height - 2);

                    using (GraphicsPath path = rec.Rounded(3))
                    using (Pen pen = new Pen(Color.Gray, 1))
                        e.Graphics.DrawPath(pen, path);
                }
            }
        }
    }

    enum EmojiMenuBarSelectedItem
    {
        People,
        Nature,
        Objects,
        Places,
        Symbols
    }
}
