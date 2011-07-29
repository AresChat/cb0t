using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace cb0t_chat_client_v2
{
    class RoundedRectangle
    {
        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2,
            BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public static GraphicsPath Create(int x, int y, int width, int height, int radius, RectangleCorners corners)
        {
            int xw = x + width;
            int yh = y + height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = x + radius;
            int yr = y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();

            p.StartFigure();

            if ((RectangleCorners.TopLeft & corners)
                == RectangleCorners.TopLeft)
            {
                p.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            p.AddLine(xr, y, xwr, y);

            if ((RectangleCorners.TopRight & corners)
                == RectangleCorners.TopRight)
            {
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            p.AddLine(xw, yr, xw, yhr);

            if ((RectangleCorners.BottomRight & corners)
                == RectangleCorners.BottomRight)
            {
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            p.AddLine(xwr, yh, xr, yh);

            if ((RectangleCorners.BottomLeft & corners)
                == RectangleCorners.BottomLeft)
            {
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        public static GraphicsPath Create(Rectangle rect, int radius, RectangleCorners corners)
        {
            return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, corners);
        }

        public static GraphicsPath Create(int x, int y, int width, int height, int radius)
        {
            return Create(x, y, width, height, radius, RectangleCorners.All);
        }

        public static GraphicsPath Create(Rectangle rect, int radius)
        {
            return Create(rect.X, rect.Y, rect.Width, rect.Height, radius);
        }

        public static GraphicsPath Create(int x, int y, int width, int height)
        {
            return Create(x, y, width, height, 5);
        }

        public static GraphicsPath Create(Rectangle rect)
        {
            return Create(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
