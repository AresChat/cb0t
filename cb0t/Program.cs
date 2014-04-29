using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.IO.Pipes;
using System.Text;

namespace cb0t
{
    static class Program
    {
        static Mutex mutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            String hashlink = null;
            
            if (args != null)
                if (args.Length > 0)
                    hashlink = String.Join(String.Empty, args);

            if (String.IsNullOrEmpty(hashlink))
                hashlink = null;

            bool first;
            mutex = new Mutex(true, "cb0t_v3_mtx", out first);

            if (!first)
            {
                if (!String.IsNullOrEmpty(hashlink))
                    SendIPC(hashlink);

                Environment.Exit(-1);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            Application.Run(new Form1(hashlink));
        }

        static void SendIPC(String str)
        {
            using (NamedPipeClientStream c = new NamedPipeClientStream("cb0t_v3_pipe"))
            {
                byte[] buf = Encoding.UTF8.GetBytes(str);

                try
                {
                    c.Connect(2000);
                    c.Write(buf, 0, buf.Length);
                    c.WaitForPipeDrain();
                }
                catch { }
            }
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
                    Process.Start("notepad.exe", path);
                }
            }
            catch { }
        }
    }
}
