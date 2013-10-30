using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Net;

namespace cb0t
{
    class ChannelBar : ToolStripRenderer
    {
        public enum ModeOption
        {
            Settings,
            Audio,
            ChannelList,
            Channel
        }

        public ModeOption Mode { get; set; }
        public IPEndPoint SelectedButton { get; set; }

        public ChannelBar()
        {
            this.Mode = ModeOption.ChannelList;
            this.SelectedButton = null;
            this.IsFocused = true;
        }

        private SolidBrush text_brush = new SolidBrush(Color.Black);
        private Pen border_pen = new Pen(Color.FromArgb(109, 115, 123), 1);

        private Color regular_col1 = Color.FromArgb(233, 239, 247);
        private Color regular_col2 = Color.FromArgb(152, 161, 175);
        private Color regular_col1_nf = Color.WhiteSmoke;
        private Color regular_col2_nf = Color.Gainsboro;

        public bool IsFocused { get; set; }

        private Color selected_col1 = Color.FromArgb(255, 255, 255);
        private Color selected_col2 = Color.FromArgb(241, 249, 255);
        private Color hover_col1 = Color.FromArgb(234, 240, 247);
        private Color hover_col2 = Color.FromArgb(208, 213, 222);

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {

        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.Clear(Aero.CanAero ? Color.Transparent : Color.Gainsboro);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is SettingsButton)
            {
                Rectangle bounds = new Rectangle(0, 0, e.Item.Bounds.Width - 1, e.Item.Bounds.Height - 1);

                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.FillPath(brush, path);

                if (this.Mode == ModeOption.Settings)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.selected_col1, this.selected_col2, LinearGradientMode.Vertical))
                    using (GraphicsPath path = bounds.Rounded(3))
                        e.Graphics.FillPath(brush, path);
                }
                else if (e.Item.Selected)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.hover_col1, this.hover_col2, LinearGradientMode.Vertical))
                    using (GraphicsPath path = bounds.Rounded(3))
                        e.Graphics.FillPath(brush, path);
                }

                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.DrawPath(this.border_pen, path);
            }
            else if (e.Item is AudioButton)
            {
                Rectangle bounds = new Rectangle(0, 0, e.Item.Bounds.Width - 1, e.Item.Bounds.Height - 1);

                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.FillPath(brush, path);

                if (this.Mode == ModeOption.Audio)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.selected_col1, this.selected_col2, LinearGradientMode.Vertical))
                    using (GraphicsPath path = bounds.Rounded(3))
                        e.Graphics.FillPath(brush, path);
                }
                else if (e.Item.Selected)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.hover_col1, this.hover_col2, LinearGradientMode.Vertical))
                    using (GraphicsPath path = bounds.Rounded(3))
                        e.Graphics.FillPath(brush, path);
                }

                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.DrawPath(this.border_pen, path);
            }
            else if (e.Item is ChannelListButton)
            {
                bool text_drawn = false;
                Rectangle bounds = new Rectangle(0, 0, e.Item.Bounds.Width - 4, e.Item.Bounds.Height - 1);

                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.FillPath(brush, path);

                if (this.Mode == ModeOption.ChannelList)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.selected_col1, this.selected_col2, LinearGradientMode.Vertical))
                    {
                        using (GraphicsPath path = bounds.Rounded(3))
                            e.Graphics.FillPath(brush, path);

                        using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                        using (Graphics g = Graphics.FromImage(text_bmp))
                        {
                            Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                            g.FillRectangle(brush, text_bounds);
                            g.DrawString(e.Item.Text, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                            e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                        }

                        text_drawn = true;
                    }
                }
                else if (e.Item.Selected)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.hover_col1, this.hover_col2, LinearGradientMode.Vertical))
                    {
                        using (GraphicsPath path = bounds.Rounded(3))
                            e.Graphics.FillPath(brush, path);

                        using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                        using (Graphics g = Graphics.FromImage(text_bmp))
                        {
                            Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                            g.FillRectangle(brush, text_bounds);
                            g.DrawString(e.Item.Text, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                            e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                        }

                        text_drawn = true;
                    }
                }

                if (!text_drawn)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                    using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                    using (Graphics g = Graphics.FromImage(text_bmp))
                    {
                        Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                        g.FillRectangle(brush, text_bounds);
                        g.DrawString(e.Item.Text, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                        e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                    }
                }

                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.DrawPath(this.border_pen, path);
            }
            else if (e.Item is ChannelButton)
            {
                bool text_drawn = false;
                Rectangle bounds = new Rectangle(0, 0, e.Item.Bounds.Width - 4, e.Item.Bounds.Height - 1);
                ChannelButton button = (ChannelButton)e.Item;

                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.FillPath(brush, path);

                if (this.Mode == ModeOption.Channel)
                {
                    if (button.EndPoint.Equals(this.SelectedButton))
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.selected_col1, this.selected_col2, LinearGradientMode.Vertical))
                        {
                            using (GraphicsPath path = bounds.Rounded(3))
                                e.Graphics.FillPath(brush, path);

                            using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                            using (Graphics g = Graphics.FromImage(text_bmp))
                            {
                                Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                                g.FillRectangle(brush, text_bounds);
                                g.DrawString(button.RoomName, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                                e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                            }

                            text_drawn = true;
                        }
                    }
                    else if (e.Item.Selected)
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.hover_col1, this.hover_col2, LinearGradientMode.Vertical))
                        {
                            using (GraphicsPath path = bounds.Rounded(3))
                                e.Graphics.FillPath(brush, path);

                            using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                            using (Graphics g = Graphics.FromImage(text_bmp))
                            {
                                Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                                g.FillRectangle(brush, text_bounds);
                                g.DrawString(button.RoomName, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                                e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                            }

                            text_drawn = true;
                        }
                    }
                }
                else if (e.Item.Selected)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.hover_col1, this.hover_col2, LinearGradientMode.Vertical))
                    {
                        using (GraphicsPath path = bounds.Rounded(3))
                            e.Graphics.FillPath(brush, path);

                        using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                        using (Graphics g = Graphics.FromImage(text_bmp))
                        {
                            Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                            g.FillRectangle(brush, text_bounds);
                            g.DrawString(button.RoomName, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                            e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                        }

                        text_drawn = true;
                    }
                }

                if (!text_drawn)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.IsFocused ? this.regular_col1 : this.regular_col1_nf, this.IsFocused ? this.regular_col2 : this.regular_col2_nf, LinearGradientMode.Vertical))
                    using (Bitmap text_bmp = new Bitmap(bounds.Width - 28, bounds.Height))
                    using (Graphics g = Graphics.FromImage(text_bmp))
                    {
                        Rectangle text_bounds = new Rectangle(0, 0, text_bmp.Width, text_bmp.Height);
                        g.FillRectangle(brush, text_bounds);
                        g.DrawString(button.RoomName, e.ToolStrip.Font, this.text_brush, new PointF(text_bounds.X, text_bounds.Y + 6));
                        e.Graphics.DrawImage(text_bmp, new Point(20, 0));
                    }
                }

                using (GraphicsPath path = bounds.Rounded(3))
                    e.Graphics.DrawPath(this.border_pen, path);
            }
        }
    }
}
