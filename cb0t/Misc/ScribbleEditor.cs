using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class ScribbleEditor : Form
    {
        private ScribbleBar ts = new ScribbleBar();
        private Color s_col = Color.Black;
        private Bitmap undo = null;
        private Bitmap redo = null;
        private Bitmap current = null;
        private Graphics g_current = null;
        private ScribbleArea current_rect = new ScribbleArea();
        private ScribbleArea undo_rect = new ScribbleArea();
        private ScribbleArea redo_rect = new ScribbleArea();

        public ScribbleEditor()
        {
            this.InitializeComponent();
            this.toolStrip1.Renderer = this.ts;
            this.toolStripLabel1.BackColor = this.s_col;
        }

        public void ResetCanvas()
        {
            if (this.undo != null)
            {
                this.undo.Dispose();
                this.undo = null;
            }

            if (this.redo != null)
            {
                this.redo.Dispose();
                this.redo = null;
            }

            if (this.g_current != null)
            {
                this.g_current.Dispose();
                this.g_current = null;
            }

            if (this.current != null)
            {
                this.current.Dispose();
                this.current = null;
            }

            this.toolStripLabel1.BackColor = this.s_col;
            this.toolStripButton6.Enabled = false;
            this.toolStripButton7.Enabled = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.ts.BrushWeight = ScribbleBrush.Thin;
            this.toolStrip1.Invalidate();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public byte[] GetScribble()
        {
            this.current = new Bitmap(396, 396);
            this.g_current = Graphics.FromImage(this.current);
            this.g_current.Clear(Color.White);
            this.toolStripButton6.Enabled = false;
            this.toolStripButton7.Enabled = false;
            this.current_rect = new ScribbleArea();
            this.undo_rect = new ScribbleArea();
            this.redo_rect = new ScribbleArea();

            byte[] result = null;

            if (this.ShowDialog() == DialogResult.OK)
                if (this.current_rect.max_x > this.current_rect.min_x && this.current_rect.max_y > this.current_rect.min_y)
                    using (Bitmap sized = new Bitmap((this.current_rect.max_x - this.current_rect.min_x) + 20, (this.current_rect.max_y - this.current_rect.min_y) + 20))
                    using (Graphics g = Graphics.FromImage(sized))
                    {
                        int x1 = this.current_rect.min_x - 10;
                        int y1 = this.current_rect.min_y - 10;

                        if (x1 < 0)
                            x1 = 0;

                        if (y1 < 0)
                            y1 = 0;

                        int x2 = sized.Width;
                        int y2 = sized.Height;

                        if ((x1 + x2) > 396)
                            x2 -= ((x1 + x2) - 396);

                        if ((y1 + y2) > 396)
                            y2 -= ((y1 + y2) - 396);

                        using (SolidBrush sb = new SolidBrush(Color.White))
                            g.FillRectangle(sb, new Rectangle(0, 0, sized.Width, sized.Height));

                        g.DrawImage(this.current, new Rectangle(0, 0, sized.Width, sized.Height), new Rectangle(x1, y1, x2, y2), GraphicsUnit.Pixel);
                        ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType == "image/jpeg");
                        EncoderParameters encoding = new EncoderParameters();
                        encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            sized.Save(ms, info, encoding);
                            result = ms.ToArray();
                        }
                    }

            this.g_current.Dispose();
            this.g_current = null;
            this.current.Dispose();
            this.current = null;

            if (this.undo != null)
            {
                this.undo.Dispose();
                this.undo = null;
            }

            if (this.redo != null)
            {
                this.redo.Dispose();
                this.redo = null;
            }

            return result;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            SharedUI.ColorPicker.StartPosition = FormStartPosition.CenterParent;
            SharedUI.ColorPicker.SelectedColor = Color.Black;

            if (SharedUI.ColorPicker.ShowDialog() == DialogResult.OK)
            {
                this.s_col = SharedUI.ColorPicker.SelectedColor;
                this.toolStripLabel1.BackColor = this.s_col;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.ts.BrushWeight = ScribbleBrush.Medium;
            this.toolStrip1.Invalidate();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.ts.BrushWeight = ScribbleBrush.Thick;
            this.toolStrip1.Invalidate();
        }

        private bool painting = false;
        private Point last_point = Point.Empty;

        private void SetUndo(Point p)
        {
            if (this.undo == null)
                this.undo = new Bitmap(396, 396);

            using (Graphics g = Graphics.FromImage(this.undo))
                g.DrawImage(this.current, new Point(0, 0));

            this.painting = true;
            this.undo_rect = this.current_rect.Clone();
            this.last_point = p;
            this.toolStripButton6.Enabled = true;
            this.toolStripButton7.Enabled = false;
        }

        private void scribbleCanvas1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.SetUndo(e.Location);
        }

        private void scribbleCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.painting && e.X <= 396 && e.Y <= 396)
                using (Graphics g = this.scribbleCanvas1.CreateGraphics())
                {
                    switch (this.ts.BrushWeight)
                    {
                        case ScribbleBrush.Thin:
                            using (SolidBrush sb = new SolidBrush(this.s_col))
                            using (Pen pen = new Pen(sb, 2))
                            {
                                g.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                this.g_current.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                this.SetCurrentRectangle(e.Location);
                            }
                            break;

                        case ScribbleBrush.Medium:
                            using (SolidBrush sb = new SolidBrush(this.s_col))
                            using (Pen pen = new Pen(sb, 10))
                            {
                                g.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                g.FillEllipse(sb, new Rectangle(e.X - 5, e.Y - 5, 10, 10));
                                this.g_current.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                this.g_current.FillEllipse(sb, new Rectangle(e.X - 5, e.Y - 5, 10, 10));
                                this.SetCurrentRectangle(e.Location);
                            }
                            break;

                        case ScribbleBrush.Thick:
                            using (SolidBrush sb = new SolidBrush(this.s_col))
                            using (Pen pen = new Pen(sb, 20))
                            {
                                g.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                g.FillEllipse(sb, new Rectangle(e.X - 10, e.Y - 10, 20, 20));
                                this.g_current.DrawLine(pen, this.last_point.X, this.last_point.Y, e.X, e.Y);
                                this.g_current.FillEllipse(sb, new Rectangle(e.X - 10, e.Y - 10, 20, 20));
                                this.SetCurrentRectangle(e.Location);
                            }
                            break;
                    }

                    this.last_point = e.Location;
                }
        }

        private void SetCurrentRectangle(Point e)
        {
            if (e.X < this.current_rect.min_x && e.X < 396)
                this.current_rect.min_x = e.X;

            if (e.X > this.current_rect.max_x && e.X > 0)
                this.current_rect.max_x = e.X;

            if (e.Y < this.current_rect.min_y && e.Y < 396)
                this.current_rect.min_y = e.Y;

            if (e.Y > this.current_rect.max_y && e.Y > 0)
                this.current_rect.max_y = e.Y;
        }

        private void scribbleCanvas1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (this.painting)
                    this.painting = false;
        }

        private void scribbleCanvas1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.current, new Point(0, 0));
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            this.toolStripButton6.Enabled = false;
            this.toolStripButton7.Enabled = true;

            if (this.redo == null)
                this.redo = new Bitmap(396, 396);

            using (Graphics g = Graphics.FromImage(this.redo))
                g.DrawImage(this.current, new Point(0, 0));

            this.g_current.DrawImage(this.undo, new Point(0, 0));
            this.redo_rect = this.current_rect.Clone();
            this.current_rect = this.undo_rect.Clone();
            this.scribbleCanvas1.Invalidate();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            this.toolStripButton6.Enabled = true;
            this.toolStripButton7.Enabled = false;

            using (Graphics g = Graphics.FromImage(this.undo))
                g.DrawImage(this.current, new Point(0, 0));

            this.g_current.DrawImage(this.redo, new Point(0, 0));
            this.undo_rect = this.current_rect.Clone();
            this.current_rect = this.redo_rect.Clone();
            this.scribbleCanvas1.Invalidate();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            this.toolStripButton6.Enabled = false;
            this.toolStripButton7.Enabled = false;
            this.g_current.Clear(Color.White);
            this.scribbleCanvas1.Invalidate();
            this.current_rect = new ScribbleArea();
            this.undo_rect = new ScribbleArea();
            this.redo_rect = new ScribbleArea();
        }

        private void scribbleCanvas1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void scribbleCanvas1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] raw_paths = (String[])e.Data.GetData(DataFormats.FileDrop);

                if (raw_paths.Length > 0)
                {
                    String path = raw_paths[0];

                    try
                    {
                        using (Bitmap avatar_raw = new Bitmap(new MemoryStream(File.ReadAllBytes(path))))
                            this.ImportExternalImage(avatar_raw);
                    }
                    catch { }
                }
            }
        }

        private void ImportExternalImage(Bitmap avatar_raw)
        {
            int img_x = avatar_raw.Width;
            int img_y = avatar_raw.Height;

            if (img_x > 396)
            {
                img_x = 396;
                img_y = avatar_raw.Height - (int)Math.Floor(Math.Floor((double)avatar_raw.Height / 100) * Math.Floor(((double)(avatar_raw.Width - 396) / avatar_raw.Width) * 100));
            }

            if (img_y > 396)
            {
                img_x -= (int)Math.Floor(Math.Floor((double)img_x / 100) * Math.Floor(((double)(img_y - 396) / img_y) * 100));
                img_y = 396;
            }

            using (Bitmap avatar_sized = new Bitmap(img_x, img_y))
            {
                using (Graphics g = Graphics.FromImage(avatar_sized))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(avatar_raw, new RectangleF(0, 0, img_x, img_y));
                    this.SetUndo(Point.Empty);
                    this.g_current.DrawImage(avatar_sized, new Point(0, 0));
                    this.scribbleCanvas1.Invalidate();
                    this.painting = false;
                    this.SetCurrentRectangle(new Point(0, 0));
                    this.SetCurrentRectangle(new Point(img_x, img_y));
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsImage())
                    using (Bitmap bmp = (Bitmap)Clipboard.GetImage())
                        this.ImportExternalImage(bmp);
            }
            catch { }
        }
    }

    class ScribbleArea
    {
        public int min_x { get; set; }
        public int max_x { get; set; }
        public int min_y { get; set; }
        public int max_y { get; set; }

        public ScribbleArea()
        {
            this.min_x = 396;
            this.min_y = 396;
            this.max_x = 0;
            this.max_y = 0;
        }

        public ScribbleArea Clone()
        {
            ScribbleArea scr = new ScribbleArea();
            scr.min_x = this.min_x;
            scr.min_y = this.min_y;
            scr.max_x = this.max_x;
            scr.max_y = this.max_y;
            return scr;
        }
    }
}
