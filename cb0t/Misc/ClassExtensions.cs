using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace cb0t
{
    static class ClassExtensions
    {
        public static bool RGBEquals(this Color c1, Color c2)
        {
            return c1.R == c2.R &&
                   c1.G == c2.G &&
                   c1.B == c2.B;
        }

        public static GraphicsPath Rounded(this Rectangle rec)
        {
            int xw = rec.X + rec.Width;
            int yh = rec.Y + rec.Height;
            int xwr = xw - 5;
            int yhr = yh - 5;
            int xr = rec.X + 5;
            int yr = rec.Y + 5;
            int r2 = 10;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();

            p.StartFigure();
            p.AddArc(rec.X, rec.Y, r2, r2, 180, 90);
            p.AddLine(xr, rec.Y, xwr, rec.Y);
            p.AddArc(xwr2, rec.Y, r2, r2, 270, 90);
            p.AddLine(xw, yr, xw, yhr);
            p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            p.AddLine(xwr, yh, xr, yh);
            p.AddArc(rec.X, yhr2, r2, r2, 90, 90);
            p.AddLine(rec.X, yhr, rec.X, yr);
            p.CloseFigure();

            return p;
        }

        public static GraphicsPath Rounded(this Rectangle rec, int radius)
        {
            int xw = rec.X + rec.Width;
            int yh = rec.Y + rec.Height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = rec.X + radius;
            int yr = rec.Y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();

            p.StartFigure();
            p.AddArc(rec.X, rec.Y, r2, r2, 180, 90);
            p.AddLine(xr, rec.Y, xwr, rec.Y);
            p.AddArc(xwr2, rec.Y, r2, r2, 270, 90);
            p.AddLine(xw, yr, xw, yhr);
            p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            p.AddLine(xwr, yh, xr, yh);
            p.AddArc(rec.X, yhr2, r2, r2, 90, 90);
            p.AddLine(rec.X, yhr, rec.X, yr);
            p.CloseFigure();

            return p;
        }
    }
}
