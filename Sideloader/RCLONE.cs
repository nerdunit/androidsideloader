using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    internal class SideloaderRCLONE
    {
        public static List<string> RemotesList = new List<string>();

        public static string RcloneGamesFolder = "Quest Games";

        public static int GameNameIndex = 0;
        public static int ReleaseNameIndex = 1;
        public static int PackageNameIndex = 2;
        public static int VersionCodeIndex = 3;
        public static int ReleaseAPKPathIndex = 4;
        public static int VersionNameIndex = 5;
        public static int DownloadsIndex = 6;
        public static int InstalledVersion = 7;

        public static List<string> gameProperties = new List<string>();
        /* Game Name
         * Release Name
         * Release APK Path
         * Package Name
         * Version Code
         * Version Name
         */
        public static List<string[]> games = new List<string[]>();

        public static string Nouns = Path.Combine(Environment.CurrentDirectory, "nouns");
        public static string ThumbnailsFolder = Path.Combine(Environment.CurrentDirectory, "thumbnails");
        public static string NotesFolder = Path.Combine(Environment.CurrentDirectory, "notes");

        public static void UpdateNouns(string remote)
        {
            _ = Logger.Log($"Updating Nouns");
            _ = RCLONE.runRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/nouns\" \"{Nouns}\"");
        }

        public static void UpdateGamePhotos(string remote)
        {
            _ = Logger.Log($"Updating Thumbnails");
            _ = RCLONE.runRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/thumbnails\" \"{ThumbnailsFolder}\" --transfers 10");
        }

        public static void UpdateGameNotes(string remote)
        {
            _ = Logger.Log($"Updating Game Notes");
            _ = RCLONE.runRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/notes\" \"{NotesFolder}\"");
        }

        public static void UpdateMetadataFromPublic()
        {
            _ = Logger.Log($"Downloading Metadata");
            string rclonecommand =
                $"sync \":http:/meta.7z\" \"{Environment.CurrentDirectory}\"";
            _ = RCLONE.runRcloneCommand_PublicConfig(rclonecommand);
        }

        public static void ProcessMetadataFromPublic()
        {
            try
            {
                var sw = Stopwatch.StartNew();

                string currentDir = Environment.CurrentDirectory;
                string metaRoot = Path.Combine(currentDir, "meta");
                string metaArchive = Path.Combine(currentDir, "meta.7z");
                string metaDotMeta = Path.Combine(metaRoot, ".meta");

                // Check if archive exists and is newer than existing metadata
                if (!File.Exists(metaArchive))
                {
                    Logger.Log("meta.7z not found, skipping extraction", LogLevel.WARNING);
                    return;
                }

                // Skip extraction if metadata is already up-to-date (based on file timestamps)
                string gameListPath = FindGameListFile(metaRoot);
                if (gameListPath != null)
                {
                    var archiveTime = File.GetLastWriteTimeUtc(metaArchive);
                    var gameListTime = File.GetLastWriteTimeUtc(gameListPath);

                    // If game list is newer than archive, skip extraction
                    if (gameListTime > archiveTime && games.Count > 0)
                    {
                        Logger.Log($"Metadata already up-to-date, skipping extraction");
                        return;
                    }
                }

                _ = Logger.Log($"Extracting Metadata");
                Zip.ExtractFile(metaArchive, metaRoot, MainForm.PublicConfigFile.Password);
                Logger.Log($"Extraction completed in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                _ = Logger.Log($"Updating Metadata");

                // Use Parallel.Invoke for independent directory operations
                System.Threading.Tasks.Parallel.Invoke(
                    () => SafeDeleteDirectory(Nouns),
                    () => SafeDeleteDirectory(ThumbnailsFolder),
                    () => SafeDeleteDirectory(NotesFolder)
                );
                Logger.Log($"Directory cleanup in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                // Move directories
                MoveIfExists(Path.Combine(metaDotMeta, "nouns"), Nouns);
                MoveIfExists(Path.Combine(metaDotMeta, "thumbnails"), ThumbnailsFolder);
                MoveIfExists(Path.Combine(metaDotMeta, "notes"), NotesFolder);

                // Deploy upload.config if bundled in meta.7z
                string uploadConfigSource = Path.Combine(metaDotMeta, "upload.config");
                string uploadConfigDest = Path.Combine(currentDir, "rclone", "upload.config");
                if (File.Exists(uploadConfigSource))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(uploadConfigDest));
                    File.Copy(uploadConfigSource, uploadConfigDest, true);
                }

                Logger.Log($"Directory moves in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                _ = Logger.Log($"Initializing Games List");

                gameListPath = FindGameListFile(metaRoot);
                if (gameListPath != null)
                {
                    // Read all lines at once - faster for files that fit in memory
                    var lines = File.ReadAllLines(gameListPath);
                    var newGames = new List<string[]>(lines.Length);

                    for (int i = 1; i < lines.Length; i++) // Skip header
                    {
                        var line = lines[i];
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var splitGame = line.Split(';');
                        if (splitGame.Length > 1)
                        {
                            newGames.Add(splitGame);
                        }
                    }

                    // Atomic swap
                    games.Clear();
                    games.AddRange(newGames);
                    Logger.Log($"Parsed {games.Count} games in {sw.ElapsedMilliseconds}ms");
                }
                else
                {
                    _ = Logger.Log("Game list file not found in extracted metadata.", LogLevel.WARNING);
                }

                SafeDeleteDirectory(metaRoot);
            }
            catch (Exception e)
            {
                _ = Logger.Log(e.Message);
                _ = Logger.Log(e.StackTrace);
            }
        }

        public static void initGames(string remote)
        {
            _ = Logger.Log($"Initializing Games List");

            gameProperties.Clear();
            games.Clear();

            // Fetch whichever *ameList.txt exists on the remote
            string tempGameList = RCLONE.runRcloneCommand_DownloadConfig(
                $"cat \"{remote}:{RcloneGamesFolder}\" --include \"*ameList.txt\" --max-depth 1").Output;
            if (MainForm.debugMode)
            {
                // Avoid redundant disk I/O: write only if non-empty
                if (!string.IsNullOrEmpty(tempGameList))
                {
                    File.WriteAllText("gameList.txt", tempGameList);
                }
            }

            if (!string.IsNullOrEmpty(tempGameList))
            {
                bool isFirstLine = true;
                foreach (var line in SplitLines(tempGameList))
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false; // skip header
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var splitGame = line.Split(new[] { ';' }, StringSplitOptions.None);
                    if (splitGame.Length > 1)
                    {
                        games.Add(splitGame);
                    }
                }
            }
        }

        // Scan cache model

        private const string ScanCacheFileName = ".rookie_scan_cache.json";

        private class ScanCacheEntry
        {
            [JsonProperty("f")] public string Fingerprint;
            [JsonProperty("g")] public string GameName;
            [JsonProperty("p")] public string PackageName;
            [JsonProperty("v")] public string VersionCode;
            [JsonProperty("m")] public string LastModified;
            [JsonProperty("s")] public string SizeMB;
            [JsonProperty("a")] public string ApkFileName;
        }

        private class FolderToScan
        {
            public string Dir;
            public string FolderName;
            public string ApkFile;
            public string Fingerprint;
        }

        private class ScannedGame
        {
            public string FolderName;
            public string[] Entry;
            public ScanCacheEntry CacheEntry;
        }

        // Cache helpers

        private static string ComputeApkFingerprint(string apkFile)
        {
            try
            {
                var fi = new FileInfo(apkFile);
                return $"{fi.Name}|{fi.LastWriteTimeUtc.Ticks}|{fi.Length}";
            }
            catch
            {
                return null;
            }
        }

        private static Dictionary<string, ScanCacheEntry> LoadScanCache(string downloadDir)
        {
            string cacheFile = Path.Combine(downloadDir, ScanCacheFileName);
            if (!File.Exists(cacheFile))
                return new Dictionary<string, ScanCacheEntry>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string json = File.ReadAllText(cacheFile);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, ScanCacheEntry>>(json);
                if (dict != null)
                {
                    // Rebuild with case-insensitive comparer
                    return new Dictionary<string, ScanCacheEntry>(dict, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to load scan cache: {ex.Message}", LogLevel.WARNING);
            }

            return new Dictionary<string, ScanCacheEntry>(StringComparer.OrdinalIgnoreCase);
        }

        private static void SaveScanCache(string downloadDir, Dictionary<string, ScanCacheEntry> cache)
        {
            string cacheFile = Path.Combine(downloadDir, ScanCacheFileName);
            try
            {
                string json = JsonConvert.SerializeObject(cache, Formatting.None);
                File.WriteAllText(cacheFile, json);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to save scan cache: {ex.Message}", LogLevel.WARNING);
            }
        }

        private static ScannedGame ScanSingleFolder(FolderToScan info, string aaptExe, bool hasAapt)
        {
            string packageName = "";
            string versionCode = "0";

            if (hasAapt)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = aaptExe,
                        Arguments = $"dump badging \"{info.ApkFile}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var proc = Process.Start(psi))
                    {
                        string aaptOutput = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit(10000);

                        var nameMatch = Regex.Match(aaptOutput, @"package:\s+name='([^']+)'");
                        if (nameMatch.Success)
                            packageName = nameMatch.Groups[1].Value;

                        var vcMatch = Regex.Match(aaptOutput, @"versionCode='(\d+)'");
                        if (vcMatch.Success)
                            versionCode = vcMatch.Groups[1].Value;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"aapt failed for {info.ApkFile}: {ex.Message}", LogLevel.WARNING);
                }
            }

            // Fallback: derive package name from OBB subfolder or APK filename
            if (string.IsNullOrEmpty(packageName))
            {
                try
                {
                    var subDirs = Directory.GetDirectories(info.Dir);
                    foreach (var sub in subDirs)
                    {
                        string subName = Path.GetFileName(sub);
                        if (subName.Contains(".") && !subName.StartsWith("."))
                        {
                            packageName = subName;
                            break;
                        }
                    }
                }
                catch { }

                if (string.IsNullOrEmpty(packageName))
                    packageName = Path.GetFileNameWithoutExtension(info.ApkFile);
            }

            string releaseName = info.FolderName;
            string gameName = CleanFolderNameToGameName(info.FolderName);

            // Calculate content size (APK + OBB only)
            double sizeMB = 0;
            try
            {
                long totalBytes = 0;
                var dirInfo = new DirectoryInfo(info.Dir);

                foreach (var f in dirInfo.EnumerateFiles("*.apk", SearchOption.TopDirectoryOnly))
                    totalBytes += f.Length;

                foreach (var subDir in dirInfo.GetDirectories())
                {
                    foreach (var f in subDir.EnumerateFiles("*.obb", SearchOption.AllDirectories))
                        totalBytes += f.Length;
                }

                sizeMB = Math.Round(totalBytes / (1024.0 * 1024.0), 1);
            }
            catch { }

            string lastModified = "";
            try
            {
                lastModified = new FileInfo(info.ApkFile).LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm");
            }
            catch { }

            string sizeMBText = sizeMB.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);

            var entry = new string[]
            {
                gameName,
                releaseName,
                packageName,
                versionCode,
                lastModified,
                sizeMBText,
                "0"
            };

            var cacheEntry = new ScanCacheEntry
            {
                Fingerprint = info.Fingerprint,
                GameName = gameName,
                PackageName = packageName,
                VersionCode = versionCode,
                LastModified = lastModified,
                SizeMB = sizeMBText,
                ApkFileName = Path.GetFileName(info.ApkFile)
            };

            return new ScannedGame
            {
                FolderName = info.FolderName,
                Entry = entry,
                CacheEntry = cacheEntry
            };
        }

        // Main scan method

        /// <summary>
        /// Scans the local download directory for game folders containing APK files.
        /// Uses a per-library cache (<c>.rookie_scan_cache.json</c>) stored in the
        /// download directory so switching between libraries is near-instant.
        /// Only folders whose APK has changed (different file size, date, or name)
        /// are re-scanned with aapt; everything else is loaded from cache.
        /// </summary>
        public static void ScanLocalGames(string downloadDir)
        {
            _ = Logger.Log("Scanning local games directory: " + downloadDir);
            var sw = Stopwatch.StartNew();

            if (!Directory.Exists(downloadDir))
            {
                _ = Logger.Log("Download directory does not exist: " + downloadDir, LogLevel.WARNING);
                return;
            }

            string aaptExe = Path.Combine(Environment.CurrentDirectory, "platform-tools", "aapt.exe");
            bool hasAapt = File.Exists(aaptExe);
            if (!hasAapt)
            {
                _ = Logger.Log("aapt.exe not found, will use basic file info only", LogLevel.WARNING);
            }

            // Track packages already present (from online metadata) to avoid duplicates
            var existingPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in games)
            {
                if (g.Length > PackageNameIndex)
                    existingPackages.Add(g[PackageNameIndex]);
            }

            string[] dirs;
            try
            {
                dirs = Directory.GetDirectories(downloadDir);
            }
            catch (Exception ex)
            {
                _ = Logger.Log($"Error enumerating download directory: {ex.Message}", LogLevel.ERROR);
                return;
            }

            // Phase 1: Load cache and categorize folders
            var cache = LoadScanCache(downloadDir);
            var newCache = new Dictionary<string, ScanCacheEntry>(dirs.Length, StringComparer.OrdinalIgnoreCase);

            var cachedResults = new List<string[]>();
            var foldersToScan = new List<FolderToScan>();
            int skippedFolders = 0;

            foreach (string dir in dirs)
            {
                string folderName = Path.GetFileName(dir);

                // Skip metadata / system folders
                if (folderName.StartsWith(".") || folderName.StartsWith("_"))
                    continue;

                // Fast path: if cache has this folder AND has an APK filename,
                // check with a single File.Exists() instead of enumerating the directory.
                string apkFile = null;
                ScanCacheEntry cached = null;

                if (cache.TryGetValue(folderName, out cached) &&
                    !string.IsNullOrEmpty(cached.ApkFileName))
                {
                    string candidatePath = Path.Combine(dir, cached.ApkFileName);
                    if (File.Exists(candidatePath))
                    {
                        apkFile = candidatePath;
                    }
                    else
                    {
                        // Cached APK filename no longer exists — invalidate cache entry
                        cached = null;
                    }
                }

                // Slow path fallback: enumerate directory for APK files
                if (apkFile == null)
                {
                    string[] apkFiles;
                    try
                    {
                        apkFiles = Directory.GetFiles(dir, "*.apk", SearchOption.TopDirectoryOnly);
                    }
                    catch
                    {
                        continue;
                    }

                    if (apkFiles.Length == 0)
                        continue;

                    apkFile = apkFiles[0];
                }

                string fingerprint = ComputeApkFingerprint(apkFile);

                // Check if we have a valid cached entry with matching fingerprint
                if (fingerprint != null &&
                    cached != null &&
                    cached.Fingerprint == fingerprint &&
                    !string.IsNullOrEmpty(cached.PackageName))
                {
                    // Skip if this package is already in the online games list
                    if (existingPackages.Contains(cached.PackageName))
                    {
                        skippedFolders++;
                        continue;
                    }

                    // Reuse cached data — no aapt or regex needed
                    string gameName = cached.GameName ?? CleanFolderNameToGameName(folderName);
                    var entry = new string[]
                    {
                        gameName,
                        folderName,
                        cached.PackageName,
                        cached.VersionCode ?? "0",
                        cached.LastModified ?? "",
                        cached.SizeMB ?? "0",
                        "0"
                    };

                    cachedResults.Add(entry);
                    existingPackages.Add(cached.PackageName);
                    newCache[folderName] = cached;
                }
                else
                {
                    // Needs scanning
                    foldersToScan.Add(new FolderToScan
                    {
                        Dir = dir,
                        FolderName = folderName,
                        ApkFile = apkFile,
                        Fingerprint = fingerprint ?? ""
                    });
                }
            }

            _ = Logger.Log($"Cache categorized in {sw.ElapsedMilliseconds}ms: " +
                           $"{cachedResults.Count} cached, {foldersToScan.Count} to scan, {skippedFolders} skipped (online duplicates)");

            // Phase 2: Parallel scan of uncached folders
            var scannedResults = new List<ScannedGame>(foldersToScan.Count);
            var scanLock = new object();

            if (foldersToScan.Count > 0)
            {
                var scanSw = Stopwatch.StartNew();

                Parallel.ForEach(
                    foldersToScan,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount) },
                    info =>
                    {
                        var result = ScanSingleFolder(info, aaptExe, hasAapt);
                        if (result != null)
                        {
                            lock (scanLock)
                            {
                                scannedResults.Add(result);
                            }
                        }
                    });

                _ = Logger.Log($"Parallel scan of {foldersToScan.Count} folders completed in {scanSw.ElapsedMilliseconds}ms");
            }

            // Phase 3: Merge results
            var localGames = new List<string[]>(cachedResults.Count + scannedResults.Count);
            localGames.AddRange(cachedResults);

            foreach (var scanned in scannedResults)
            {
                // Skip online duplicates
                if (existingPackages.Contains(scanned.Entry[PackageNameIndex]))
                    continue;

                localGames.Add(scanned.Entry);
                existingPackages.Add(scanned.Entry[PackageNameIndex]);
                newCache[scanned.FolderName] = scanned.CacheEntry;
            }

            if (localGames.Count > 0)
            {
                games.AddRange(localGames);
            }

            // Phase 4: Save updated cache
            SaveScanCache(downloadDir, newCache);

            _ = Logger.Log($"Local scan complete: {localGames.Count} games " +
                           $"({cachedResults.Count} from cache, {scannedResults.Count} freshly scanned) " +
                           $"in {sw.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// Extracts the app name from a folder name formatted like "SomeAppName v123"
        /// by taking everything before the first " v" followed by a digit.
        /// </summary>
        private static string CleanFolderNameToGameName(string folderName)
        {
            // Find the first " v" followed by a digit — everything before it is the game name.
            int idx = folderName.IndexOf(" v", StringComparison.Ordinal);
            while (idx >= 0)
            {
                // Ensure the character after "v" is a digit
                int afterV = idx + 2;
                if (afterV < folderName.Length && char.IsDigit(folderName[afterV]))
                {
                    string cleaned = folderName.Substring(0, idx).Trim(' ', '-', '_');
                    return string.IsNullOrWhiteSpace(cleaned) ? folderName : cleaned;
                }
                // Not a version marker (e.g. "VR"), keep searching
                idx = folderName.IndexOf(" v", idx + 1, StringComparison.Ordinal);
            }

            // No version pattern found — strip common suffixes as fallback
            string result = folderName.Trim(' ', '-', '_');
            return string.IsNullOrWhiteSpace(result) ? folderName : result;
        }

        // Fast directory delete using Windows cmd - faster than .NET's Directory.Delete
        // for large directories with many files (e.g., thumbnails folder with 1000+ images)
        private static void SafeDeleteDirectory(string path)
        {
            // Avoid exceptions when directory is missing
            if (!Directory.Exists(path))
                return;

            try
            {
                // Use Windows rd command which is ~10x faster than .NET's recursive delete
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c rd /s /q \"{path}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(psi))
                {
                    // Wait with timeout to prevent hanging
                    if (!process.WaitForExit(30000)) // 30 second timeout
                    {
                        try { process.Kill(); } catch { }
                        Logger.Log($"Directory delete timed out for: {path}", LogLevel.WARNING);
                        // Fallback to .NET delete
                        FallbackDelete(path);
                    }
                    else if (process.ExitCode != 0 && Directory.Exists(path))
                    {
                        // Command failed, try fallback
                        FallbackDelete(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Fast delete failed for {path}: {ex.Message}", LogLevel.WARNING);
                // Fallback to standard .NET delete
                FallbackDelete(path);
            }
        }

        // Fallback delete method using standard .NET
        private static void FallbackDelete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    FileSystemUtilities.TryDeleteDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Fallback delete also failed for {path}: {ex.Message}", LogLevel.ERROR);
            }
        }

        // Move directory only if source exists
        private static void MoveIfExists(string sourceDir, string destDir)
        {
            if (Directory.Exists(sourceDir))
            {
                // Ensure destination does not exist to prevent IOException
                // Use fast delete method
                SafeDeleteDirectory(destDir);
                Directory.Move(sourceDir, destDir);
            }
            else
            {
                _ = Logger.Log($"Source directory not found: {sourceDir}", LogLevel.WARNING);
            }
        }

        // Returns the path of the first file matching *ameList.txt found in the
        // given directory, or null if none exists.
        private static string FindGameListFile(string directory)
        {
            try
            {
                var matches = Directory.GetFiles(directory, "*ameList.txt", SearchOption.TopDirectoryOnly);
                if (matches.Length > 0)
                    return matches[0];
            }
            catch { }
            return null;
        }

        // Efficient, cross-platform line splitting for string buffers
        private static IEnumerable<string> SplitLines(string s)
        {
            // Handle both \r\n and \n without allocating intermediate arrays
            using (var reader = new StringReader(s))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}