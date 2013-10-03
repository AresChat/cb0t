using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class Aero
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        [DllImport("uxtheme.dll")]
        private static extern int SetWindowThemeAttribute(IntPtr hWnd, WINDOWTHEMEATTRIBUTETYPE wtype, ref WTA_OPTIONS attributes, uint size);

        [DllImport("dwmapi.dll")]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        [StructLayout(LayoutKind.Sequential)]
        private struct WTA_OPTIONS
        {
            public WTNCA dwFlags;
            public WTNCA dwMask;
        }

        [Flags]
        private enum WTNCA : uint
        {
            NODRAWCAPTION = 1,
            NODRAWICON = 2,
            NOSYSMENU = 4,
            NOMIRRORHELP = 8,
            VALIDBITS = NODRAWCAPTION | NODRAWICON | NOSYSMENU | NOMIRRORHELP
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int left, right, top, bottom;
        }

        private enum WINDOWTHEMEATTRIBUTETYPE : uint
        {
            WTA_NONCLIENT = 1
        }

        public static bool CanAero
        {
            get
            {
                bool result = Environment.OSVersion.Version.Major >= 6;

                if (result)
                    result = DwmIsCompositionEnabled();

                return result;
            }
        }

        public static void HideIconAndText(Form form)
        {
            if (!CanAero)
                return;

            WTA_OPTIONS options = new WTA_OPTIONS();
            options.dwFlags = WTNCA.NODRAWICON | WTNCA.NODRAWCAPTION;
            options.dwMask = WTNCA.VALIDBITS;

            SetWindowThemeAttribute(form.Handle,
                WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT,
                ref options,
                (uint)Marshal.SizeOf(typeof(WTA_OPTIONS)));
        }

        public static void ExtendTop(Form form, int pixels)
        {
            if (!CanAero)
                return;

            MARGINS m = new MARGINS();
            m.left = 0;
            m.right = 0;
            m.top = pixels;
            m.bottom = 0;

            DwmExtendFrameIntoClientArea(form.Handle, ref m);
        }
    }
}
