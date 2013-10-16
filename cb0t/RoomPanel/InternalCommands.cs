using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t
{
    class InternalCommands
    {
        public static String CMD_TIME
        {
            get
            {
                return "Time: " + DateTime.Now.ToString();
            }
        }

        public static String CMD_UPTIME
        {
            get
            {
                String str = "Uptime: ";

                try
                {
                    using (ManagementObject mo = new ManagementObject(@"\\.\root\cimv2:Win32_OperatingSystem=@"))
                    {
                        DateTime lastBootUp = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
                        TimeSpan ts = DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime();
                        str += ts.Days + " days, " + ts.Hours + " hours, " + ts.Minutes + " minutes";
                    }
                }
                catch
                {
                    using (PerformanceCounter uptime = new PerformanceCounter("System", "System Up Time"))
                    {
                        uptime.NextValue();
                        TimeSpan ts = TimeSpan.FromSeconds(uptime.NextValue());
                        str += ts.Days + " days, " + ts.Hours + " hours, " + ts.Minutes + " minutes";
                    }
                }

                return str;
            }
        }

        public static String CMD_GFX
        {
            get
            {
                String result = String.Empty;

                using (ManagementObjectSearcher WmiSelect = new ManagementObjectSearcher("\\root\\CIMV2", "SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject WmiResults in WmiSelect.Get())
                    {
                        result = WmiResults.GetPropertyValue("Name").ToString();

                        if (!String.IsNullOrEmpty(result))
                            break;
                    }
                }

                return "Graphics: " + result;
            }
        }

        public static String CMD_HDD
        {
            get
            {
                long available = 0;
                long total = 0;


                foreach (DriveInfo disk in DriveInfo.GetDrives())
                    if (disk.DriveType == DriveType.Fixed)
                    {
                        available += disk.AvailableFreeSpace;
                        total += disk.TotalSize;
                    }

                double d_av = available;
                d_av = Math.Round((double)(available / 1024 / 1024 / 1024), 2);
                double d_to = total;
                d_to = Math.Round((double)(total / 1024 / 1024 / 1024));
                double load = 100 - Math.Round((double)(d_av / d_to) * 100);
                return "Disk Space: " + d_av + "GB out of " + d_to + "GB available (" + load + "% usage)";
            }
        }

        public static String CMD_OS
        {
            get
            {
                ObjectQuery query = new WqlObjectQuery("SELECT * FROM Win32_OperatingSystem");
                StringBuilder sb = new StringBuilder();
                sb.Append("Operating System: ");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        object tmp = obj["Caption"];

                        if (tmp != null)
                            if (!String.IsNullOrEmpty(tmp.ToString()))
                            {
                                sb.Append(tmp.ToString());
                                break;
                            }
                    }

                Version v = Environment.OSVersion.Version;
                sb.Append(" (" + v.Major + "." + v.Minor + "." + v.Build + ")");
                return sb.ToString();
            }
        }

        public static String CMD_CPU
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                RegistryKey key = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");

                if (key != null)
                {
                    sb.Append(key.GetValue("ProcessorNameString").ToString().Trim());
                    key.Close();
                }

                String result = sb.ToString();

                while (result.Contains("  "))
                    result = result.Replace("  ", " ");

                return "Processor: " + result;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern void GlobalMemoryStatus(out MemoryStatus stat);

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatusEx(out MemoryStatusEx stat);

        private struct MemoryStatus
        {
            public uint Length;
            public uint MemoryLoad;
            public uint TotalPhysical;
            public uint AvailablePhysical;
            public uint TotalPageFile;
            public uint AvailablePageFile;
            public uint TotalVirtual;
            public uint AvailableVirtual;
        }

        public struct MemoryStatusEx
        {
            public uint Length;
            public uint MemoryLoad;
            public ulong TotalPhysical;
            public ulong AvailablePhysical;
            public ulong TotalPageFile;
            public ulong AvailablePageFile;
            public ulong TotalVirtual;
            public ulong AvailableVirtual;
            public ulong AvailableExtendedVirtual;
        }

        public static String CMD_RAM
        {
            get
            {
                MemoryStatusEx mem;
                mem.Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
                GlobalMemoryStatusEx(out mem);

                if (mem.AvailablePhysical == 0)
                {
                    MemoryStatus ms;
                    GlobalMemoryStatus(out ms);
                    double av = (ms.AvailablePhysical / 1024 / 1024);
                    double tot = (ms.TotalPhysical / 1024 / 1024);
                    double load = 100 - Math.Round((double)(av / tot) * 100);
                    return "Memory: " + av + "MB out of " + tot + "MB available (" + load + "% usage)";
                }
                else
                {
                    double av = (mem.AvailablePhysical / 1024 / 1024);
                    double tot = (mem.TotalPhysical / 1024 / 1024);
                    double load = 100 - Math.Round((double)(av / tot) * 100);
                    return "Memory: " + av + "MB out of " + tot + "MB available (" + load + "% usage)";
                }
            }
        }
    }
}
