using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace cb0t_chat_client_v2
{
    public partial class EmoticonDesigner : UserControl
    {
        public EmoticonDesigner()
        {
            this.ClearCanvas(false);

            InitializeComponent();

            this.BorderStyle = BorderStyle.None;
            this.DoubleBuffered = true;
            this.Size = new Size(255, 255);
            this.ForeColor = Color.Black;
            this.SizeChanged += new EventHandler(this.EmoticonDesigner_SizeChanged);
        }

        private void EmoticonDesigner_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width != 225)
                this.Size = new Size(225, 225);
        }

        private EmoteDesignMode mode = EmoteDesignMode.Size16x16;

        private Color[,] grid16 = new Color[16, 16];
        private Color[,] grid32 = new Color[32, 32];

        public byte[] GetEmoticon
        {
            get
            {
                byte[] results = null;

                using (Bitmap bmp = this.mode == EmoteDesignMode.Size16x16 ? new Bitmap(16, 16) : new Bitmap(32, 32))
                {
                    switch (this.mode)
                    {
                        case EmoteDesignMode.Size16x16:
                            for (int x = 0; x < 16; x++)
                                for (int y = 0; y < 16; y++)
                                    bmp.SetPixel(x, y, this.grid16[x, y]);
                            break;

                        case EmoteDesignMode.Size32x32:
                            for (int x = 0; x < 32; x++)
                                for (int y = 0; y < 32; y++)
                                    bmp.SetPixel(x, y, this.grid32[x, y]);
                            break;
                    }

                    ImageCodecInfo info = new List<ImageCodecInfo>(ImageCodecInfo.GetImageEncoders()).Find(
                        delegate(ImageCodecInfo i)
                        {
                            return i.MimeType == "image/jpeg";
                        });

                    EncoderParameters encoding = new EncoderParameters();
                    encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, info, encoding);
                        results = ms.ToArray();
                    }
                }

                return results;
            }
        }

        public EmoteDesignMode Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                this.ClearCanvas(true);
            }
        }

        public void ClearCanvas(bool redraw)
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (x < 16 && y < 16)
                        this.grid16[x, y] = Color.White;

                    this.grid32[x, y] = Color.White;
                }
            }

            if (redraw)
                this.Invalidate();
        }

        private bool painting = false;
        private Point last_paint = Point.Empty;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.painting)
            {
                int grid_width, x, y;

                switch (this.mode)
                {
                    case EmoteDesignMode.Size16x16:
                        grid_width = 224 / 16;
                        x = (e.X / grid_width);
                        y = (e.Y / grid_width);

                        if (!this.last_paint.Equals(new Point(x, y)))
                        {
                            if (x >= 0 && x <= 15 && y >= 0 && y <= 15)
                            {
                                this.last_paint = new Point(x, y);
                                this.grid16[x, y] = this.ForeColor;
                                this.Invalidate(new Rectangle(x * grid_width, y * grid_width, grid_width, grid_width));
                            }
                        }
                        
                        break;

                    case EmoteDesignMode.Size32x32:
                        grid_width = 224 / 32;
                        x = (e.X / grid_width);
                        y = (e.Y / grid_width);

                        if (!this.last_paint.Equals(new Point(x, y)))
                        {
                            if (x >= 0 && x <= 31 && y >= 0 && y <= 31)
                            {
                                this.last_paint = new Point(x, y);
                                this.grid32[x, y] = this.ForeColor;
                                this.Invalidate(new Rectangle(x * grid_width, y * grid_width, grid_width, grid_width));
                            }
                        }

                        break;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int grid_width, x, y;

            switch (this.mode)
            {
                case EmoteDesignMode.Size16x16:
                    grid_width = 224 / 16;
                    x = (e.X / grid_width);
                    y = (e.Y / grid_width);

                    if (x >= 0 && x <= 15 && y >= 0 && y <= 15)
                    {
                        this.painting = true;
                        this.last_paint = new Point(x, y);
                        this.grid16[x, y] = this.ForeColor;
                        this.Invalidate(new Rectangle(x * grid_width, y * grid_width, grid_width, grid_width));
                    }

                    break;

                case EmoteDesignMode.Size32x32:
                    grid_width = 224 / 32;
                    x = (e.X / grid_width);
                    y = (e.Y / grid_width);

                    if (x >= 0 && x <= 31 && y >= 0 && y <= 31)
                    {
                        this.painting = true;
                        this.last_paint = new Point(x, y);
                        this.grid32[x, y] = this.ForeColor;
                        this.Invalidate(new Rectangle(x * grid_width, y * grid_width, grid_width, grid_width));
                    }

                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.painting)
                this.painting = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int grid_width = 0;
            e.Graphics.Clear(Color.Gainsboro);

            switch (this.mode)
            {
                case EmoteDesignMode.Size16x16:
                    grid_width = 224 / 16;

                    for (int x = 0; x < 16; x++)
                        for (int y = 0; y < 16; y++)
                            using (SolidBrush sb = new SolidBrush(this.grid16[x, y]))
                                e.Graphics.FillRectangle(sb, new Rectangle(x * grid_width, y * grid_width, grid_width - 1, grid_width - 1));
                    break;

                case EmoteDesignMode.Size32x32:
                    grid_width = 224 / 32;

                    for (int x = 0; x < 32; x++)
                        for (int y = 0; y < 32; y++)
                            using (SolidBrush sb = new SolidBrush(this.grid32[x, y]))
                                e.Graphics.FillRectangle(sb, new Rectangle(x * grid_width, y * grid_width, grid_width - 1, grid_width - 1));
                    break;
            }
        }
    }

    public enum EmoteDesignMode
    {
        Size16x16,
        Size32x32
    }
}
