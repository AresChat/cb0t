using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            String hashlink = String.Join(String.Empty, args);
            String proc_name = Process.GetCurrentProcess().ProcessName;
            int id = Process.GetCurrentProcess().Id;
            IntPtr ptr = IntPtr.Zero;

            if (proc_name != "cb0t")
                Environment.Exit(-1); // exe must be called cb0t

            int instance_count = 0;

            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == proc_name)
                {
                    instance_count++;

                    if (id != p.Id)
                    {
                        ptr = p.MainWindowHandle;
                        instance_count = 10;
                        break;
                    }
                }
            }

            if (instance_count > 1)
            {
                if (hashlink.Length > 0)
                {
                    if (!ptr.Equals(IntPtr.Zero))
                    {
                        Win32.CopyDataStruct cds = new Win32.CopyDataStruct();

                        try
                        {
                            cds.cbData = (hashlink.Length + 1) * 2;
                            cds.lpData = Win32.LocalAlloc(0x40, cds.cbData);
                            Marshal.Copy(hashlink.ToCharArray(), 0, cds.lpData, hashlink.Length);
                            cds.dwData = (IntPtr)1;
                            Win32.SendMessage(ptr, 0x004A, IntPtr.Zero, ref cds);
                        }
                        finally
                        {
                            cds.Dispose();
                        }
                    }
                }
                
                Environment.Exit(-1); // max 1 instance
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1(null));
            try
            {
                Application.Run(new Form1(hashlink.Length == 0 ? null : hashlink));
            }
            catch (Exception e)
            {
                ShowCrash(e);
                throw;
            }
        }

        static void ShowCrash(Exception e)
        {
            String path = Settings.folder_path + "crash.txt";
            File.WriteAllText(path, e.Message + "\r\n\r\n" + e.StackTrace);
            Process.Start("notepad.exe", path);
        }
    }
}
