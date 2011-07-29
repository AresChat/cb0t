using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace cb0t_chat_client_v2
{
    class ScribbleScreen : UserControl
    {
        public enum ScribbleType : int
        {
            Thin = 0,
            Medium = 1,
            Thick = 2
        };

        private bool painting = false;
        private ImageFormat save_type = ImageFormat.Jpeg;

        private Bitmap canvas;
        private Graphics graphics;

        private int last_x = 0;
        private int last_y = 0;

        private int min_x = 0;
        private int min_y = 0;
        private int max_x = 0;
        private int max_y = 0;

        public Color pen = Color.Red;
        public ScribbleType brush = ScribbleType.Thin;

        public ScribbleScreen()
        {
            this.DoubleBuffered = true;
            this.AllowDrop = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.White;
            this.canvas = new Bitmap(this.Width, this.Height);
        }

        public void Init()
        {
            this.canvas = new Bitmap(this.Width, this.Height);
            this.graphics = Graphics.FromImage(this.canvas);
            this.graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));
            this.min_x = this.canvas.Width;
            this.min_y = this.canvas.Height;
            this.max_x = 0;
            this.max_y = 0;
            this.Invalidate();
        }

        public ScribbleObject ExportPicture()
        {
            if (this.max_x < this.min_x || this.max_y < this.min_y)
                return null;

            Bitmap b = new Bitmap((this.max_x - this.min_x) + 20, (this.max_y - this.min_y) + 20);
            Graphics g = Graphics.FromImage(b);

            int x1 = this.min_x - 10;
            int y1 = this.min_y - 10;
            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            int x2 = b.Width;
            int y2 = b.Height;

            if ((x1 + x2) > this.canvas.Width) x2 -= ((x1 + x2) - this.canvas.Width);
            if ((y1 + y2) > this.canvas.Height) y2 -= ((y1 + y2) - this.canvas.Height);

            g.DrawImage(this.canvas, new Rectangle(0, 0, b.Width, b.Height),
                new Rectangle(x1, y1, x2, y2), GraphicsUnit.Pixel);

            // target max width 256

            MemoryStream ms = new MemoryStream();

            if (b.Width > 256)
            {
                int fix3 = b.Height - (int)Math.Floor(Math.Floor((double)b.Height / 100) * Math.Floor(((double)(b.Width - 256) / b.Width) * 100));

                Bitmap b2 = new Bitmap(256, fix3);
                Graphics g2 = Graphics.FromImage(b2);
                g2.DrawImage(b, new RectangleF(0, 0, b2.Width, b2.Height));
                b2.Save(ms, this.save_type);
                byte[] data = ms.ToArray();
                data = ZLib.Zlib.Compress(data);
                return new ScribbleObject(b2, data);
            }
            else
            {
                b.Save(ms, this.save_type);
                byte[] data = ms.ToArray();
                data = ZLib.Zlib.Compress(data);
                return new ScribbleObject(b, data);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
         //   e.Graphics.DrawImage(this.canvas, new Point(e.ClipRectangle.X, e.ClipRectangle.Y));
            e.Graphics.DrawImage(this.canvas, new Point(0, 0));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
         //   e.Graphics.DrawImage(this.canvas, new Point(e.ClipRectangle.X, e.ClipRectangle.Y));
            e.Graphics.DrawImage(this.canvas, new Point(0, 0));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.painting = true;
                this.last_x = e.X;
                this.last_y = e.Y;
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.painting = false;
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMove(EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                drgevent.Effect = DragDropEffects.All;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            String path = ((string[])drgevent.Data.GetData(DataFormats.FileDrop))[0];
            String check = path.ToUpper();

            if (check.EndsWith(".JPG") || check.EndsWith(".BMP") || check.EndsWith(".PNG"))
            {
                try
                {
                    byte[] buf1 = File.ReadAllBytes(path);
                    Bitmap img = new Bitmap(new MemoryStream(buf1));

                    int img_x = img.Width;
                    int img_y = img.Height;

                    if (img_x > this.Height)
                    {
                        img_x = this.Height;
                        img_y = img.Height - (int)Math.Floor(Math.Floor((double)img.Height / 100) * Math.Floor(((double)(img.Width - this.Height) / img.Width) * 100));
                    }

                    if (img_y > this.Height)
                    {
                        img_x -= (int)Math.Floor(Math.Floor((double)img_x / 100) * Math.Floor(((double)(img_y - this.Height) / img_y) * 100));
                        img_y = this.Height;
                    }

                    Graphics g = this.CreateGraphics();
                    g.DrawImage(img, new RectangleF(0, 0, img_x, img_y));
                    this.graphics.DrawImage(img, new RectangleF(0, 0, img_x, img_y));

                    this.save_type = ImageFormat.Jpeg;
                    this.min_x = 0;
                    this.min_y = 0;

                    if (this.max_x < img_x)
                        this.max_x = img_x;

                    if (this.max_y < img_y)
                        this.max_y = img_y;
                }
                catch { }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();

            if (this.painting)
            {
                if (e.X < this.min_x && e.X < this.canvas.Width) this.min_x = e.X;
                if (e.X > this.max_x && e.X > 0) this.max_x = e.X;
                if (e.Y < this.min_y && e.Y < this.canvas.Height) this.min_y = e.Y;
                if (e.Y > this.max_y && e.Y > 0) this.max_y = e.Y;

                Pen myPen;

                switch (this.brush)
                {
                    case ScribbleType.Thin:
                        myPen = new Pen(this.pen, 2);
                        g.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        this.graphics.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        break;

                    case ScribbleType.Medium:
                        myPen = new Pen(this.pen, 10);
                        g.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        g.FillEllipse(new SolidBrush(this.pen), new Rectangle(e.X - 5, e.Y - 5, 10, 10));
                        this.graphics.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        this.graphics.FillEllipse(new SolidBrush(this.pen), new Rectangle(e.X - 5, e.Y - 5, 10, 10));
                        break;

                    case ScribbleType.Thick:
                        myPen = new Pen(this.pen, 20);
                        g.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        g.FillEllipse(new SolidBrush(this.pen), new Rectangle(e.X - 10, e.Y - 10, 20, 20));
                        this.graphics.DrawLine(myPen, this.last_x, this.last_y, e.X, e.Y);
                        this.graphics.FillEllipse(new SolidBrush(this.pen), new Rectangle(e.X - 10, e.Y - 10, 20, 20));
                        break;
                }

                g.Dispose();

                this.last_x = e.X;
                this.last_y = e.Y;
            }
        }

    }

    class ScribbleObject
    {
        public Bitmap image;
        public byte[] data;

        public ScribbleObject(Bitmap image, byte[] data)
        {
            this.image = image;
            this.data = data;
        }
    }
}
