using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AndroidSideloader.Utilities
{
    public static class PlatformHelpers
    {
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string ExeExtension => IsWindows ? ".exe" : "";

        public static string ShellExecutable => IsWindows ? "cmd.exe" : "/bin/sh";

        public static string ShellArgumentsPrefix => IsWindows ? "/c " : "-c ";

        public static string AdbBinaryName => $"adb{ExeExtension}";
        public static string RcloneBinaryName => $"rclone{ExeExtension}";

        public static string GetRcloneDownloadUrl(string version, bool fallback = false)
        {
            string arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => "arm64",
                Architecture.X64 => "amd64",
                Architecture.X86 => "386",
                _ => "amd64"
            };

            string os = IsWindows ? "windows" : IsMacOS ? "osx" : "linux";
            string baseUrl = fallback
                ? $"https://raw.githubusercontent.com/{RepoDefaults.Repo}/{RepoDefaults.Branch}/dep/rclone-v{version}-{os}-{arch}.zip"
                : $"https://downloads.rclone.org/v{version}/rclone-v{version}-{os}-{arch}.zip";

            return baseUrl;
        }

        public static string GetAdbDownloadUrl()
        {
            // Google publishes platform-tools per OS
            if (IsWindows)
                return "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
            if (IsMacOS)
                return "https://dl.google.com/android/repository/platform-tools-latest-darwin.zip";
            return "https://dl.google.com/android/repository/platform-tools-latest-linux.zip";
        }

        public static string GetConfigBasePath()
        {
            if (IsWindows)
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rookie.AndroidSideloader");
            if (IsMacOS)
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Rookie.AndroidSideloader");
            // Linux
            string xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (!string.IsNullOrEmpty(xdg))
                return Path.Combine(xdg, "Rookie.AndroidSideloader");
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "Rookie.AndroidSideloader");
        }
    }

    public static class RepoDefaults
    {
        public const string Repo = "nerdunit/androidsideloader";
        public const string Branch = "main";
    }
}
