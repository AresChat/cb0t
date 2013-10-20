using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class PopupDialog : Form
    {
        public PopupDialog()
        {
            this.InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.ResizeRedraw = true;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private bool timing_out = false;
        private int timing_out_counter = 0;
        private Rectangle screen_size = Screen.PrimaryScreen.WorkingArea;

        public event EventHandler PopupClicked;
        internal PopupSettings Settings { get; set; }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.timing_out)
            {
                this.Size = new Size(252, this.Size.Height + 8);
                this.Location = new Point((this.screen_size.Width - 262), (this.screen_size.Height - this.Offset - this.Size.Height));

                if (this.Size.Height == 126)
                {
                    this.timing_out = true;
                    this.MinimumSize = this.Size;
                }
            }
            else if (this.timing_out_counter++ > 200)
            {
                this.timer1.Stop();
                this.Visible = false;
            }
        }

        public int Offset { get; set; }
        public int OffsetBase { get; set; }

        internal void ShowPopup(PopupSettings s, int os)
        {
            this.OffsetBase = os;
            this.Offset = (10 * os) + (126 * (os - 1));
            this.timing_out = false;
            this.timing_out_counter = 0;
            this.Settings = s;
            this.Text = this.Settings.Title;
            this.label1.Text = this.Settings.Title;
            this.label2.Text = String.Join("\r\n", this.Settings.Message.ToArray());
            this.MinimumSize = new Size(0, 0);
            this.TopMost = true;
            this.Size = new Size(252, 38);
            this.Location = new Point((this.screen_size.Width - 262), (this.screen_size.Height - this.Offset));
            this.Visible = true;
            this.timer1.Start();
        }

        public bool Busy { get; set; }

        private void PopupMouseClicked(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.Visible = false;
            this.PopupClicked(this.Settings.Room, null);
        }

        private void PopupDialog_VisibleChanged(object sender, EventArgs e)
        {
            this.Busy = this.Visible;
        }


    }

    class PopupSettings
    {
        public String Title { get; set; }
        public List<String> Message { get; set; }
        public IPEndPoint Room { get; set; }
    }
}
