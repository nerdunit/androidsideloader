using System;
using System.IO;

namespace AndroidSideloader
{
    internal class Logger
    {
        public string logfile = Properties.Settings.Default.CurrentLogPath;
        public static bool Log(string text, bool ret = true)
        {

            string time = DateTime.Now.ToString("hh:mmtt(UTC): ");
            if (text.Length > 5)
            {

                string newline = "\n";
                if (text.Length > 40 && text.Contains("\n"))
                {
                    newline += "\n\n";
                }

                try { File.AppendAllText(Properties.Settings.Default.CurrentLogPath, time + text + newline); } catch { }
                return ret;
            }
            return ret;
        }
    }
}
