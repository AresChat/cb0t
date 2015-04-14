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

        private object callback = null;
        private bool bg = false;
        private String pname = null;

        public void SetCallback(RoomPanel cb, bool bg)
        {
            this.callback = cb;
            this.bg = bg;
        }

        public void SetCallback(GlobalSettings cb, String pn)
        {
            this.callback = cb;
            this.pname = pn;
        }

        private void ColorClicked(object sender, EventArgs e)
        {
            String text = ((ColorMenuItem)sender).Tag.ToString();

            if (this.callback != null)
            {
                if (this.callback is RoomPanel)
                {
                    RoomPanel rp = (RoomPanel)this.callback;
                    if (this.bg)
                        rp.ColorCallback("\x00025" + text);
                    else
                        rp.ColorCallback("\x00023" + text);
                }
                else if (this.callback is GlobalSettings)
                    ((GlobalSettings)this.callback).ColorChangeCallback(this.pname, int.Parse(text));

                this.callback = null;
            }

            this.Hide();
        }
    }
}
