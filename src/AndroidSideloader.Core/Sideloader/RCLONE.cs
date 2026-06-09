using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public class SideloaderRCLONE
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
        public static List<string[]> games = new List<string[]>();

        public static string Nouns = Path.Combine(AppContext.BaseDirectory, "nouns");
        public static string ThumbnailsFolder = Path.Combine(AppContext.BaseDirectory, "thumbnails");
        public static string NotesFolder = Path.Combine(AppContext.BaseDirectory, "notes");

        public static void UpdateNouns(string remote)
        {
            _ = Logger.Log($"Updating Nouns");
            _ = RCLONE.RunRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/nouns\" \"{Nouns}\"");
        }

        public static void UpdateGamePhotos(string remote)
        {
            _ = Logger.Log($"Updating Thumbnails");
            _ = RCLONE.RunRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/thumbnails\" \"{ThumbnailsFolder}\" --transfers 10");
        }

        public static void UpdateGameNotes(string remote)
        {
            _ = Logger.Log($"Updating Game Notes");
            _ = RCLONE.RunRcloneCommand_DownloadConfig($"sync \"{remote}:{RcloneGamesFolder}/.meta/notes\" \"{NotesFolder}\"");
        }

        public static void UpdateMetadataFromPublic()
        {
            _ = Logger.Log($"Downloading Metadata");
            string rclonecommand = $"sync \":http:/meta.7z\" \"{AppContext.BaseDirectory}\"";
            _ = RCLONE.RunRcloneCommand_PublicConfig(rclonecommand);
        }

        public static void ProcessMetadataFromPublic()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                string currentDir = AppContext.BaseDirectory;
                string metaRoot = Path.Combine(currentDir, "meta");
                string metaArchive = Path.Combine(currentDir, "meta.7z");
                string metaDotMeta = Path.Combine(metaRoot, ".meta");

                if (!File.Exists(metaArchive))
                {
                    Logger.Log("meta.7z not found, skipping extraction", LogLevel.WARNING);
                    return;
                }

                string? gameListPath = FindGameListFile(metaRoot);
                if (gameListPath != null)
                {
                    var archiveTime = File.GetLastWriteTimeUtc(metaArchive);
                    var gameListTime = File.GetLastWriteTimeUtc(gameListPath);

                    if (gameListTime > archiveTime && games.Count > 0)
                    {
                        Logger.Log("Metadata already up-to-date, skipping extraction");
                        return;
                    }
                }

                _ = Logger.Log($"Extracting Metadata");
                string? password = AppState.PublicConfigFile?.Password;
                if (!string.IsNullOrEmpty(password))
                {
                    Utilities.Zip.ExtractFile(metaArchive, metaRoot, password);
                }
                else
                {
                    Utilities.Zip.ExtractFile(metaArchive, metaRoot);
                }
                Logger.Log($"Extraction completed in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                _ = Logger.Log($"Updating Metadata");

                Parallel.Invoke(
                    () => SafeDeleteDirectory(Nouns),
                    () => SafeDeleteDirectory(ThumbnailsFolder),
                    () => SafeDeleteDirectory(NotesFolder)
                );
                Logger.Log($"Directory cleanup in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                MoveIfExists(Path.Combine(metaDotMeta, "nouns"), Nouns);
                MoveIfExists(Path.Combine(metaDotMeta, "thumbnails"), ThumbnailsFolder);
                MoveIfExists(Path.Combine(metaDotMeta, "notes"), NotesFolder);

                string uploadConfigSource = Path.Combine(metaDotMeta, "upload.config");
                string uploadConfigDest = Path.Combine(currentDir, "rclone", "upload.config");
                if (File.Exists(uploadConfigSource))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(uploadConfigDest)!);
                    File.Copy(uploadConfigSource, uploadConfigDest, true);
                }

                Logger.Log($"Directory moves in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                _ = Logger.Log($"Initializing Games List");

                gameListPath = FindGameListFile(metaRoot);
                if (gameListPath != null)
                {
                    var lines = File.ReadAllLines(gameListPath);
                    var newGames = new List<string[]>(lines.Length);

                    for (int i = 1; i < lines.Length; i++)
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
                _ = Logger.Log(e.StackTrace ?? "");
            }
        }

        public static void initGames(string remote)
        {
            _ = Logger.Log($"Initializing Games List");

            gameProperties.Clear();
            games.Clear();

            string tempGameList = RCLONE.RunRcloneCommand_DownloadConfig(
                $"cat \"{remote}:{RcloneGamesFolder}\" --include \"*ameList.txt\" --max-depth 1").Output;

            if (AppState.DebugMode)
            {
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
                        isFirstLine = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var splitGame = line.Split(new[] { ';' }, StringSplitOptions.None);
                    if (splitGame.Length > 1)
                    {
                        games.Add(splitGame);
                    }
                }
            }
        }

        private const string ScanCacheFileName = ".rookie_scan_cache.json";

        private class ScanCacheEntry
        {
            [JsonProperty("f")] public string Fingerprint = "";
            [JsonProperty("g")] public string GameName = "";
            [JsonProperty("p")] public string PackageName = "";
            [JsonProperty("v")] public string VersionCode = "";
            [JsonProperty("m")] public string LastModified = "";
            [JsonProperty("s")] public string SizeMB = "";
            [JsonProperty("a")] public string ApkFileName = "";
        }

        private class FolderToScan
        {
            public string Dir = "";
            public string FolderName = "";
            public string ApkFile = "";
            public string Fingerprint = "";
        }

        private class ScannedGame
        {
            public string FolderName = "";
            public string[] Entry = Array.Empty<string>();
            public ScanCacheEntry CacheEntry = new ScanCacheEntry();
        }

        private static string? ComputeApkFingerprint(string apkFile)
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

        private static ScannedGame? ScanSingleFolder(FolderToScan info, string aaptExe, bool hasAapt)
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
                        if (proc == null) return null;
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

        public static void ScanLocalGames(string downloadDir)
        {
            _ = Logger.Log("Scanning local games directory: " + downloadDir);
            var sw = Stopwatch.StartNew();

            if (!Directory.Exists(downloadDir))
            {
                _ = Logger.Log("Download directory does not exist: " + downloadDir, LogLevel.WARNING);
                return;
            }

            string aaptExe = Path.Combine(AppContext.BaseDirectory, "platform-tools", "aapt" + (PlatformHelpers.IsWindows ? ".exe" : ""));
            bool hasAapt = File.Exists(aaptExe);
            if (!hasAapt)
            {
                _ = Logger.Log("aapt not found, will use basic file info only", LogLevel.WARNING);
            }

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

            var cache = LoadScanCache(downloadDir);
            var newCache = new Dictionary<string, ScanCacheEntry>(dirs.Length, StringComparer.OrdinalIgnoreCase);

            var cachedResults = new List<string[]>();
            var foldersToScan = new List<FolderToScan>();
            int skippedFolders = 0;

            foreach (string dir in dirs)
            {
                string folderName = Path.GetFileName(dir);

                if (folderName.StartsWith(".") || folderName.StartsWith("_"))
                    continue;

                string? apkFile = null;
                ScanCacheEntry? cached = null;

                if (cache.TryGetValue(folderName, out cached) &&
                    !string.IsNullOrEmpty(cached?.ApkFileName))
                {
                    string candidatePath = Path.Combine(dir, cached.ApkFileName);
                    if (File.Exists(candidatePath))
                    {
                        apkFile = candidatePath;
                    }
                    else
                    {
                        cached = null;
                    }
                }

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

                string? fingerprint = ComputeApkFingerprint(apkFile);

                if (fingerprint != null &&
                    cached != null &&
                    cached.Fingerprint == fingerprint &&
                    !string.IsNullOrEmpty(cached.PackageName))
                {
                    if (existingPackages.Contains(cached.PackageName))
                    {
                        skippedFolders++;
                        continue;
                    }

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

            var localGames = new List<string[]>(cachedResults.Count + scannedResults.Count);
            localGames.AddRange(cachedResults);

            foreach (var scanned in scannedResults)
            {
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

            SaveScanCache(downloadDir, newCache);

            _ = Logger.Log($"Local scan complete: {localGames.Count} games " +
                           $"({cachedResults.Count} from cache, {scannedResults.Count} freshly scanned) " +
                           $"in {sw.ElapsedMilliseconds}ms");
        }

        private static string CleanFolderNameToGameName(string folderName)
        {
            int idx = folderName.IndexOf(" v", StringComparison.Ordinal);
            while (idx >= 0)
            {
                int afterV = idx + 2;
                if (afterV < folderName.Length && char.IsDigit(folderName[afterV]))
                {
                    string cleaned = folderName.Substring(0, idx).Trim(' ', '-', '_');
                    return string.IsNullOrWhiteSpace(cleaned) ? folderName : cleaned;
                }
                idx = folderName.IndexOf(" v", idx + 1, StringComparison.Ordinal);
            }

            string result = folderName.Trim(' ', '-', '_');
            return string.IsNullOrWhiteSpace(result) ? folderName : result;
        }

        private static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            try
            {
                FileSystemUtilities.TryDeleteDirectory(path);
            }
            catch (Exception ex)
            {
                Logger.Log($"Delete failed for {path}: {ex.Message}", LogLevel.WARNING);
            }
        }

        private static void MoveIfExists(string sourceDir, string destDir)
        {
            if (Directory.Exists(sourceDir))
            {
                SafeDeleteDirectory(destDir);
                Directory.Move(sourceDir, destDir);
            }
            else
            {
                _ = Logger.Log($"Source directory not found: {sourceDir}", LogLevel.WARNING);
            }
        }

        private static string? FindGameListFile(string directory)
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

        private static IEnumerable<string> SplitLines(string s)
        {
            using (var reader = new StringReader(s))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
