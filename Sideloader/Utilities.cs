using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace AndroidSideloader
{
    internal class SideloaderUtilities
    {
        public static bool CheckFolderIsObb(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (file.EndsWith(".obb") || (Path.GetDirectoryName(file).Contains(".") && !Path.GetDirectoryName(file).Contains("_data")))
                {
                    return true;
                }
            }

            return false;
        }

        private static string uuid = null;
        public static string UUID()
        {
            if (uuid != null)
            {
                return uuid;
            }

            StringBuilder sb = new StringBuilder();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Processor");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                _ = sb.Append(queryObj["NumberOfCores"]);
                _ = sb.Append(queryObj["ProcessorId"]);
                _ = sb.Append(queryObj["Name"]);
                _ = sb.Append(queryObj["SocketDesignation"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                _ = sb.Append(queryObj["Manufacturer"]);
                _ = sb.Append(queryObj["Name"]);
                _ = sb.Append(queryObj["Version"]);

            }

            searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                _ = sb.Append(queryObj["Product"]);
            }

            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            SHA256Managed sha = new SHA256Managed();

            byte[] hash = sha.ComputeHash(bytes);

            uuid = BitConverter.ToString(hash).Replace("-", "");
            return uuid;
        }

    }
}
