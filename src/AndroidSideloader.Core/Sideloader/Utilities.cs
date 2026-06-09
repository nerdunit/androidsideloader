using AndroidSideloader.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AndroidSideloader
{
    public class SideloaderUtilities
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;

        public static bool CheckFolderIsObb(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (file.EndsWith(".obb") || (Path.GetDirectoryName(file)?.Contains(".") == true && !Path.GetDirectoryName(file)?.Contains("_data") == true))
                {
                    return true;
                }
            }

            return false;
        }

        private static string? uuid = null;
        public static string UUID()
        {
            uuid = settings.UUID;
            if (!string.IsNullOrEmpty(uuid))
            {
                return uuid;
            }

            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            uuid = BitConverter.ToString(bytes).Replace("-", "");

            settings.UUID = uuid;
            settings.Save();

            return uuid;
        }
    }
}
