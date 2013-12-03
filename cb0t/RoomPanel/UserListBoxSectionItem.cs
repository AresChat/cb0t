using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class UserListBoxSectionItem
    {
        public UserListBoxSectionType Section { get; private set; }

        public UserListBoxSectionItem(UserListBoxSectionType type)
        {
            this.Section = type;
        }

        public void Draw(DrawItemEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.DarkGray))
                e.Graphics.FillRectangle(brush, e.Bounds);

            using (SolidBrush brush = new SolidBrush(Color.WhiteSmoke))
            using (Font font = new Font(e.Font, FontStyle.Bold))
            {
                switch (this.Section)
                {
                    case UserListBoxSectionType.Friends:
                        e.Graphics.DrawString(StringTemplate.Get(STType.UserList, 18), font, brush, new PointF(e.Bounds.X, e.Bounds.Y + 1));
                        break;

                    case UserListBoxSectionType.Admins:
                        e.Graphics.DrawString(StringTemplate.Get(STType.UserList, 19), font, brush, new PointF(e.Bounds.X, e.Bounds.Y + 1));
                        break;

                    case UserListBoxSectionType.Users:
                        e.Graphics.DrawString(StringTemplate.Get(STType.UserList, 15), font, brush, new PointF(e.Bounds.X, e.Bounds.Y + 1));
                        break;
                }
            }
        }
    }
}
