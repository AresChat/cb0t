using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WMPLib;
using System.Drawing;

namespace MediaIPC.WMP
{
    class WMPListener
    {
        private IWMPPlayer4 IPC { get; set; }
        private bool unavailable { get; set; } // true if WMP isn't installed

        public WMPListener(Form container, bool can_use)
        {
            if (!can_use)
                this.unavailable = true;
            else
            {
                this.unavailable = false;

                try
                {
                    WMPInterOp wmp = new WMPInterOp();
                    wmp.Size = new Size(1, 1);
                    container.Controls.Add(wmp);
                    wmp.Visible = false;
                    this.IPC = (IWMPPlayer4)wmp.GetOcx();
                }
                catch
                {
                    this.unavailable = true;
                }
            }
        }

        public String Song
        {
            get
            {
                if (this.unavailable)
                    return String.Empty;

                try
                {
                    if (this.IPC.currentMedia != null)
                    {
                        IWMPMedia media = this.IPC.currentMedia;

                        if (media.name.Length > 0)
                        {
                            StringBuilder sb = new StringBuilder();

                            for (int i = 0; i < media.attributeCount; i++)
                            {
                                String x = media.getAttributeName(i);
                                String y = media.getItemInfo(x).Trim();

                                if (y.Length > 0)
                                {
                                    switch (x.ToUpper())
                                    {
                                        case "AUTHOR":
                                            sb.Append(y);
                                            break;

                                        case "WM/ALBUMARTIST":
                                            if (sb.Length == 0)
                                                sb.Append(y);
                                            break;

                                        case "WM/COMPOSER":
                                            if (sb.Length == 0)
                                                sb.Append(y);
                                            break;
                                    }
                                }
                            }

                            if (sb.Length == 0)
                                sb.Append("Unknown Artist");

                            sb.Append(" - ");
                            sb.Append(media.name);
                            return sb.ToString();
                        }
                    }
                }
                catch
                {
                    this.unavailable = true;
                }

                return String.Empty;
            }
        }
    }
}
