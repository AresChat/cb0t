using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FormEx.OverlayIconEx
{
    public class OverlayIcon
    {
        private DwmForm Owner { get; set; }
        private Bitmap _Image { get; set; }

        public OverlayIcon(DwmForm f)
        {
            this.Owner = f;
        }

        public Bitmap Image
        {
            get { return this._Image; }
            set
            {
                if (this._Image != null)
                    this._Image.Dispose();

                this._Image = value;
            }
        }

        public void Show()
        {
            if (this._Image != null)
                using (Icon ico = Icon.FromHandle(this._Image.GetHicon()))
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetOverlayIcon(this.Owner.Handle, ico, String.Empty);
        }

        public void Hide()
        {
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetOverlayIcon(this.Owner.Handle, null, null);
        }
    }
}
