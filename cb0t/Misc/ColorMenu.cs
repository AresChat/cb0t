using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class ColorMenu : Form
    {
        public ColorMenu()
        {
            this.InitializeComponent();
            this.ClientSize = new Size(133, 133);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Deactivate += ColorMenu_Deactivate;
        }

        private void ColorMenu_Deactivate(object sender, EventArgs e)
        {
            this.callback = null;
            this.Hide();
        }

        private RoomPanel callback = null;
        private bool bg = false;

        public void SetCallback(RoomPanel cb, bool bg)
        {
            this.callback = cb;
            this.bg = bg;
        }

        private void ColorClicked(object sender, EventArgs e)
        {
            String text = ((ColorMenuItem)sender).Tag.ToString();

            if (this.callback != null)
            {
                if (this.bg)
                    this.callback.ColorCallback("\x00025" + text);
                else
                    this.callback.ColorCallback("\x00023" + text);

                this.callback = null;
            }

            this.Hide();
        }
    }
}
