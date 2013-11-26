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
    public partial class VolumeControl : Form
    {
        public event EventHandler<VolumeControlValueChangedEventArgs> VolumeChanged;

        public VolumeControl()
        {
            this.InitializeComponent();
            this.ClientSize = new Size(128, 40);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Deactivate += this.VolumeControl_Deactivate;
        }

        private void VolumeControl_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void SetVolume(int vol)
        {
            this.trackBar1.Value = (vol / 10);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (this.VolumeChanged != null)
                this.VolumeChanged(null, new VolumeControlValueChangedEventArgs { Volume = (this.trackBar1.Value * 10) });
        }
    }

    public class VolumeControlValueChangedEventArgs : EventArgs
    {
        public int Volume { get; set; }
    }
}
