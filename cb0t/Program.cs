using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace cb0t
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            Application.Run(new Form1());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            String path = Path.Combine(Settings.DataPath, "log1.txt");

            try
            {
                String str = e.Exception.Message + "\r\n\r\n" + e.Exception.StackTrace;
                File.WriteAllText(path, str);
                Process.Start("notepad.exe", path);
            }
            catch { }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            String path = Path.Combine(Settings.DataPath, "log2.txt");

            try
            {
                if (e.ExceptionObject != null)
                {
                    Exception ex = (Exception)e.ExceptionObject;
                    String str = ex.Message + "\r\n\r\n" + ex.StackTrace;
                    File.WriteAllText(path, str);
                    Process.Start("nodepad.exe", path);
                }
            }
            catch { }
        }
    }
}
