using System;
using System.IO;
using System.Threading;

namespace AndroidSideloader.Utilities
{
    internal static class FileSystemUtilities
    {
        public static bool MoveToRecycleBin(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (!Directory.Exists(path) && !File.Exists(path)) return false;

            if (PlatformHelpers.IsMacOS)
            {
                try
                {
                    // macOS: use osascript to move to trash
                    var psi = new System.Diagnostics.ProcessStartInfo("osascript", $"-e 'tell application \"Finder\" to delete POSIX file \"{path}\"'")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var p = System.Diagnostics.Process.Start(psi);
                    p?.WaitForExit();
                    return true;
                }
                catch { /* fall through to direct delete */ }
            }
            else if (PlatformHelpers.IsLinux)
            {
                // Try trash-put if available
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo("trash-put", $"\"{path}\"")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var p = System.Diagnostics.Process.Start(psi);
                    p?.WaitForExit();
                    if (p?.ExitCode == 0) return true;
                }
                catch { /* fall through */ }
            }

            // Windows or fallback: just delete directly
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                else if (Directory.Exists(path))
                    Directory.Delete(path, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryDeleteDirectory(string directoryPath, int maxRetries = 3, int delayMs = 150)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return true;

            if (!Directory.Exists(directoryPath))
                return true;

            Exception? lastError = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    StripReadOnlyAttributes(directoryPath);
                    Directory.Delete(directoryPath, true);
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return true;
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
                {
                    lastError = ex;
                    if (attempt < maxRetries)
                    {
                        Thread.Sleep(delayMs);
                        continue;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    break;
                }
            }

            try
            {
                string renamedPath = directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    + ".deleting." + DateTime.UtcNow.Ticks;

                Directory.Move(directoryPath, renamedPath);
                StripReadOnlyAttributes(renamedPath);
                Directory.Delete(renamedPath, true);
                return true;
            }
            catch (Exception ex)
            {
                lastError = ex;
            }

            Logger.Log($"Failed to delete directory: {directoryPath}. Error: {lastError}", LogLevel.WARNING);
            return false;
        }

        private static void StripReadOnlyAttributes(string directoryPath)
        {
            var root = new DirectoryInfo(directoryPath);
            if (!root.Exists) return;

            root.Attributes &= ~FileAttributes.ReadOnly;

            foreach (var dir in root.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                dir.Attributes &= ~FileAttributes.ReadOnly;
            }

            foreach (var file in root.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                file.Attributes &= ~FileAttributes.ReadOnly;
            }
        }
    }
}
