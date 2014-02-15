using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Interop")]
    class JSInterop : ObjectInstance
    {
        public JSInterop(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "AutoLoad"; }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [JSFunction(Name = "findWindow", IsEnumerable = true, IsWritable = false)]
        public static int JS_FindWindow(object a, object b)
        {
            if (a is Undefined || b is Undefined)
                return 0;

            String lpClassName, lpWindowName;

            if (a is Null)
                lpClassName = null;
            else
                lpClassName = a.ToString();

            if (b is Null)
                lpWindowName = null;
            else
                lpWindowName = b.ToString();

            IntPtr ptr = FindWindow(lpClassName, lpWindowName);
            return ptr.ToInt32();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [JSFunction(Name = "findWindowEx", IsEnumerable = true, IsWritable = false)]
        public static int JS_FindWindowEx(object a, object b, object c, object d)
        {
            if (a is Undefined || b is Undefined || c is Undefined || d is Undefined)
                return 0;

            int hwndParent, hwndChildAfter;
            String lpszClass, lpszWindow;

            if (!int.TryParse(a.ToString(), out hwndParent))
                return 0;

            if (!int.TryParse(b.ToString(), out hwndChildAfter))
                return 0;

            if (c is Null)
                lpszClass = null;
            else
                lpszClass = c.ToString();

            if (d is Null)
                lpszWindow = null;
            else
                lpszWindow = d.ToString();

            IntPtr ptr = FindWindowEx(new IntPtr(hwndParent), new IntPtr(hwndChildAfter), lpszClass, lpszWindow);
            return ptr.ToInt32();
        }

        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wparam, int lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [JSFunction(Name = "getWindowText", IsEnumerable = true, IsWritable = false)]
        public static String JS_GetWindowText(object a)
        {
            if (a is Undefined)
                return String.Empty;

            int i;

            if (!int.TryParse(a.ToString(), out i))
                return String.Empty;

            IntPtr ptr = new IntPtr(i);
            int size = GetWindowTextLength(ptr);

            if (size > 0)
            {
                StringBuilder sb = new StringBuilder(size + 1);
                GetWindowText(ptr, sb, sb.Capacity);
                return sb.ToString();
            }
            else
            {
                size = SendMessage(ptr, WM_GETTEXTLENGTH, 0, 0).ToInt32();

                if (size > 0)
                {
                    StringBuilder sb = new StringBuilder(size + 1);
                    SendMessage(ptr, WM_GETTEXT, sb.Capacity, sb);
                    return sb.ToString();
                }
            }

            return String.Empty;
        }
    }
}
