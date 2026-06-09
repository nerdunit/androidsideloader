using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;

namespace AndroidSideloader.Utilities
{
    public class Zip
    {
        public static bool ExtractFile(string sourceArchive, string destination, bool overwrite = true)
        {
            try
            {
                Logger.Log($"Extracting {sourceArchive} to {destination}");
                Directory.CreateDirectory(destination);

                using (var archive = ArchiveFactory.Open(sourceArchive))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            string outPath = Path.Combine(destination, entry.Key);
                            string? dir = Path.GetDirectoryName(outPath);
                            if (!string.IsNullOrEmpty(dir))
                                Directory.CreateDirectory(dir);

                            entry.WriteToFile(outPath, new ExtractionOptions()
                            {
                                Overwrite = overwrite
                            });
                        }
                    }
                }

                Logger.Log($"Extraction complete: {sourceArchive}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Extraction failed: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        public static bool ExtractFile(string sourceArchive, string destination, string password, bool overwrite = true)
        {
            try
            {
                Logger.Log($"Extracting {sourceArchive} to {destination} (password-protected)");
                Directory.CreateDirectory(destination);

                var options = new ReaderOptions { Password = password };
                using (var archive = ArchiveFactory.Open(sourceArchive, options))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            string outPath = Path.Combine(destination, entry.Key);
                            string? dir = Path.GetDirectoryName(outPath);
                            if (!string.IsNullOrEmpty(dir))
                                Directory.CreateDirectory(dir);

                            entry.WriteToFile(outPath, new ExtractionOptions()
                            {
                                Overwrite = overwrite
                            });
                        }
                    }
                }

                Logger.Log($"Extraction complete: {sourceArchive}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Extraction failed: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        public static void ExtractZip(string zipPath, string extractPath)
        {
            ExtractFile(zipPath, extractPath);
        }

        public static bool ZipDirectory(string sourceDirectory, string destinationArchive)
        {
            try
            {
                using (var archive = SharpCompress.Archives.Zip.ZipArchive.Create())
                {
                    archive.AddAllFromDirectory(sourceDirectory);
                    archive.SaveTo(destinationArchive, CompressionType.Deflate);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Zip creation failed: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        public static long GetUncompressedSize(string zipPath)
        {
            try
            {
                using var archive = ArchiveFactory.Open(zipPath);
                return archive.Entries.Sum(e => e.Size);
            }
            catch
            {
                return -1;
            }
        }

        public static bool IsZip(string path)
        {
            try
            {
                using var archive = ArchiveFactory.Open(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
