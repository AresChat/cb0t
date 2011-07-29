using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;

namespace cb0t_chat_client_v2
{
    class ClientCommands
    {
        [DllImport("kernel32.dll")]
        private static extern void GlobalMemoryStatus(out MemoryStatus stat);

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatusEx(out MemoryStatusEx stat);

        private struct MemoryStatus
        {

            public uint Length; //Length of struct 
            public uint MemoryLoad; //Value from 0-100 represents memory usage 
            public uint TotalPhysical;
            public uint AvailablePhysical;
            public uint TotalPageFile;
            public uint AvailablePageFile;
            public uint TotalVirtual;
            public uint AvailableVirtual;

        }

        public struct MemoryStatusEx
        {

            public uint Length; //Length of struct
            public uint MemoryLoad; //Value from 0-100 represents memory usage
            public ulong TotalPhysical;
            public ulong AvailablePhysical;
            public ulong TotalPageFile;
            public ulong AvailablePageFile;
            public ulong TotalVirtual;
            public ulong AvailableVirtual;
            public ulong AvailableExtendedVirtual;

        }


        public static String GetMemory()
        {
            MemoryStatusEx mem;
            mem.Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
            GlobalMemoryStatusEx(out mem);

            if (mem.TotalPhysical == 0)
            {
                MemoryStatus ms;
                GlobalMemoryStatus(out ms);
                return "Memory: " + (ms.TotalPhysical / 1024 / 1024) + "MB RAM";
            }
            else
            {
                return "Memory: " + (mem.TotalPhysical / 1024 / 1024) + "MB RAM";
            }
        }

        public static String GetTime()
        {
            DateTime getDate = DateTime.Now;
            return "Timestamp: " + getDate.ToString();
        }

        public static String GetCPU()
        {
            RegistryKey RegKey = Registry.LocalMachine;
            RegKey = RegKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            Object cpuSpeed = RegKey.GetValue("~MHz");
            Object cpuType = RegKey.GetValue("ProcessorNameString");
            RegKey.Close();
            return "Processor: " + cpuType.ToString() + " - " + cpuSpeed.ToString() + "Mhz";
        }

        public static String GetUptime()
        {
            String str = "Uptime: ";

            try
            {
                using (ManagementObject mo = new ManagementObject(@"\\.\root\cimv2:Win32_OperatingSystem=@"))
                {
                    DateTime lastBootUp = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
                    TimeSpan ts = DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime();
                    str += (ts.Days + " days " + ts.Hours + " hours " + ts.Minutes + " minutes");
                }
            }
            catch
            {
                using (PerformanceCounter uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();
                    TimeSpan ts = TimeSpan.FromSeconds(uptime.NextValue());
                    str += (ts.Days + " days " + ts.Hours + " hours " + ts.Minutes + " minutes");
                }
            }

            return str;
        }

        public static String OSVer()
        {
            return "Operating System: " + Environment.OSVersion.ToString();
        }

        public static String GetGraphicsCardName()
        {
            String GraphicsCardName = String.Empty;

            try
            {
                using (ManagementObjectSearcher WmiSelect = new ManagementObjectSearcher("\\root\\CIMV2", "SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject WmiResults in WmiSelect.Get())
                    {
                        GraphicsCardName = WmiResults.GetPropertyValue("Name").ToString();

                        if (!String.IsNullOrEmpty(GraphicsCardName))
                            break;
                    }
                }
            }
            catch { }

            return GraphicsCardName;
        }
    }
}
