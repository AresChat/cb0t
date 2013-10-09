using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class UserListBoxEmptyItem
    {
        public UserListBoxSectionType Section { get; private set; }

        public UserListBoxEmptyItem(UserListBoxSectionType type)
        {
            this.Section = type;
        }

        public void Draw(DrawItemEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(brush, e.Bounds);

            using (SolidBrush brush = new SolidBrush(Color.Black))
            using (Font font = new Font(e.Font, FontStyle.Italic))
                e.Graphics.DrawString("(empty)", font, brush, new PointF(e.Bounds.X, e.Bounds.Y + 1));
        }
    }
}
