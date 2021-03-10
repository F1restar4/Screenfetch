using System;
using System.Windows.Forms;
using System.Management;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;

namespace Screenfetch
{
    class Program
    {

        static void writeToConsole(string one, string two)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(one);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(two);
        }


        static string getOS()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem");
            searcher.Get();

            foreach (ManagementObject share in searcher.Get())
            {
                var OsName = share["Caption"];

                if (Environment.Is64BitOperatingSystem)
                {
                    OsName = OsName + "(64-bit)";
                }
                else
                {
                    OsName = OsName + "(32-bit)";
                }

                searcher.Dispose();
                return OsName.ToString();
            }

            return null;
        }

        static string getHostname()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select CSName from Win32_OperatingSystem");
            searcher.Get();

            foreach (ManagementObject share in searcher.Get())
            {
                var HostName = share["CSName"];
                searcher.Dispose();
                return HostName.ToString();
            }

            return null;
        }

        static string getProcessCount()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select NumberOfProcesses from Win32_OperatingSystem");
            searcher.Get();

            foreach (ManagementObject share in searcher.Get())
            {
                var ProcessCount = share["NumberOfProcesses"];
                searcher.Dispose();
                return ProcessCount.ToString();
            }

            return null;

        }

        static TimeSpan GetUpTime()
        {
            var uptime = Environment.TickCount;
            return RoundTimeSpan(TimeSpan.FromMilliseconds(uptime));
        }

        static TimeSpan RoundTimeSpan(TimeSpan input)
        {

            int factor = (int)Math.Pow(10, 7);

            TimeSpan roundedTimeSpan = new TimeSpan(((long)Math.Round((1.0 * input.Ticks / factor)) * factor));

            return roundedTimeSpan;

        }


        static string getScreenRes()
        {
            var x = SystemInformation.VirtualScreen.Width;
            var y = SystemInformation.VirtualScreen.Height;

            string res = x.ToString() + " x " + y.ToString();

            return res;
        }

        static string getCPUInformation()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select CurrentClockSpeed, Name, NumberOfCores, Architecture from Win32_Processor");
            searcher.Get();

            foreach (ManagementObject share in searcher.Get())
            {
                var clockSpeedM = share["CurrentClockSpeed"];
                var Name = share["Name"];
                var CoreCount = share["NumberOfCores"];

                double clockSpeedG = Math.Round(Convert.ToDouble(clockSpeedM) / 1000, 2);

                if (share["Architecture"].ToString() == "9")
                {
                    Name = Name + " (64-bit)";
                }
                else
                {
                    Name = Name + " (32-bit)";
                }

                searcher.Dispose();
                return Name + " " + CoreCount + " cores" + " ~" + clockSpeedG + "GHz";

            }

            return null;

        }

        static string getGPUInformation()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Description from Win32_VideoController");
            searcher.Get();

            foreach (ManagementObject share in searcher.Get())
            {
                var gpuInfo = share["Description"];
                searcher.Dispose();
                return gpuInfo.ToString();
            }

            return null;
        }

        static string getRAMInformation()
        {
            ComputerInfo ComputerI = new ComputerInfo();
            string Totalmem = ComputerI.TotalPhysicalMemory.ToString();
            string Freemem = ComputerI.AvailablePhysicalMemory.ToString();

            double TotalMemGiB = Math.Round(Convert.ToDouble(Totalmem) / 1073741824, 2);
            double FreeMemGiB = Math.Round(Convert.ToDouble(Freemem) / 1073741824, 2);

            double UsedMemGiB = Math.Round(TotalMemGiB - FreeMemGiB, 2);

            return UsedMemGiB + "GiB" + " / " + TotalMemGiB + "GiB";

        }

        static void Main(string[] args)
        {

            Console.Title = "Screenfetch";

            string OS, Hostname, ProcessCount, Res, CPUInfo, GPUInfo, RAMInfo;
            bool verboseMode;
            TimeSpan Uptime;


            var TimeElapsed = Stopwatch.StartNew();
 


            if (args != null && args.Length != 0 && args[0] == "-v")
            {

                verboseMode = true;

                Console.WriteLine(Environment.NewLine + "Gathering data..." + Environment.NewLine);


                OS = getOS();
                Console.WriteLine("Got OS: " + OS); 

                Hostname = getHostname();
                Console.WriteLine("Got Hostname: " + Hostname);

                ProcessCount = getProcessCount();
                Console.WriteLine("Got Process count: " + ProcessCount);

                Uptime = GetUpTime();
                Console.WriteLine("Got Uptime: " + Uptime);

                Res = getScreenRes();
                Console.WriteLine("Got Screen res: " + Res);

                CPUInfo = getCPUInformation();
                Console.WriteLine("Got CPU Info: " + CPUInfo);

                GPUInfo = getGPUInformation();
                Console.WriteLine("Got CPU Info: " + GPUInfo);

                RAMInfo = getRAMInformation();
                Console.WriteLine("Got RAM Info: " + RAMInfo);

                Console.WriteLine(Environment.NewLine + "Output: " + Environment.NewLine);
            }
            else
            {

            verboseMode = false;
            Console.WriteLine("Gathering data...");

            OS = getOS();
            Hostname = getHostname();
            ProcessCount = getProcessCount();
            Uptime = GetUpTime();
            Res = getScreenRes();
            CPUInfo = getCPUInformation();
            GPUInfo = getGPUInformation();
            RAMInfo = getRAMInformation();

            Console.Clear();

            }

            writeToConsole("OS: ", OS);
            writeToConsole("Hostname: ", Hostname);
            writeToConsole("Uptime: ", Uptime.Days.ToString() + " Day(s) " + Uptime.Hours.ToString() + " Hour(s) " + Uptime.Minutes.ToString() + " Minute(s) " + Uptime.Seconds.ToString() + " Second(s)");
            writeToConsole("Resolution: ", Res);
            writeToConsole("Process Count: ", ProcessCount);
            writeToConsole("CPU: ", CPUInfo);
            writeToConsole("GPU: ", GPUInfo);
            writeToConsole("RAM: ", RAMInfo);

            TimeElapsed.Stop();

            if (verboseMode)
            {
                var elapsedTime = TimeElapsed.ElapsedMilliseconds;
                Console.WriteLine(Environment.NewLine + "Operation took " + elapsedTime + "ms" + Environment.NewLine);
            }

            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
