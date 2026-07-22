using AndroidSideloader.Utilities;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    // SFTP fast path for large transfers (OBBs) to rooted headsets running an SSH server.
    // adb push tops out well below what the Quest's WiFi/USB link can do; a direct SFTP
    // session to an on-device sshd is typically several times faster for multi-GB OBBs.
    // Everything here degrades gracefully: if no SSH server is reachable, transfers fall
    // back to the regular adb push path automatically.
    internal static class SFTP
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;

        private static readonly object _detectLock = new object();
        private static Task<bool> _detectionTask;
        private static string _detectedDeviceId;

        // Results of the last detection run
        public static bool RootChecked { get; private set; }
        public static bool IsRootAvailable { get; private set; }
        public static bool IsSftpAvailable { get; private set; }
        // An SSH server answered but no configured credential worked - the UI uses
        // this to ask the user for a key file or password
        public static bool AuthFailedButServerPresent { get; private set; }
        public static string Host { get; private set; }
        public static int Port { get; private set; }
        public static string User { get; private set; }

        private static readonly int[] DefaultPorts = { 22, 8022, 2222 };
        // /storage/emulated/0 is the canonical path; /sdcard is a symlink that some
        // sshd mount namespaces cannot resolve, so it is only the fallback.
        private static readonly string[] ObbRootCandidates =
        {
            "/storage/emulated/0/Android/obb",
            "/sdcard/Android/obb"
        };
        private static string _obbRoot = ObbRootCandidates[0];
        // Upper bound only: SSH.NET clamps each write to the server's negotiated max
        private const uint TransferBufferSize = 1048576;

        // Short status for the titlebar, e.g. " | Root ✓ | SFTP ✓ (root@192.168.1.35:22)"
        public static string TitleSuffix
        {
            get
            {
                string suffix = "";
                if (RootChecked)
                {
                    suffix += IsRootAvailable ? " | Root ✓" : " | Root ✗";
                    suffix += IsSftpAvailable ? $" | SFTP ✓ ({User}@{Host}:{Port})" : " | SFTP ✗";
                }
                return suffix;
            }
        }

        public static void InvalidateCache()
        {
            lock (_detectLock)
            {
                _detectionTask = null;
                _detectedDeviceId = null;
                RootChecked = false;
                IsRootAvailable = false;
                IsSftpAvailable = false;
            }
        }

        // Runs root + SFTP detection once per connected device; safe to call repeatedly.
        public static Task<bool> DetectAsync(bool force = false)
        {
            lock (_detectLock)
            {
                if (force || _detectionTask == null || _detectedDeviceId != ADB.DeviceID)
                {
                    _detectedDeviceId = ADB.DeviceID;
                    _detectionTask = Task.Run(() => Detect());
                }
                return _detectionTask;
            }
        }

        private static bool Detect()
        {
            try
            {
                IsRootAvailable = DetectRoot();
                RootChecked = true;
                Logger.Log($"SFTP: root access {(IsRootAvailable ? "detected (su grants uid=0)" : "not detected")}");

                IsSftpAvailable = DetectSftp();
                Logger.Log(IsSftpAvailable
                    ? $"SFTP: server available at {User}@{Host}:{Port} - OBB transfers will use SFTP"
                    : "SFTP: no usable server found - OBB transfers will use adb push");
                return IsSftpAvailable;
            }
            catch (Exception ex)
            {
                Logger.Log($"SFTP: detection failed: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        // 'timeout' guards against Magisk/KernelSU showing a grant prompt and blocking forever
        private static bool DetectRoot()
        {
            ProcessOutput result = ADB.RunAdbCommandToString("shell \"timeout 3 su -c id\"", true);
            if (result.Output.Contains("uid=0"))
            {
                return true;
            }

            ProcessOutput which = ADB.RunAdbCommandToString("shell which su", true);
            if (!string.IsNullOrWhiteSpace(which.Output))
            {
                Logger.Log("SFTP: su binary present but a root shell was not granted", LogLevel.WARNING);
            }
            return false;
        }

        // Resolves the headset's IP: explicit setting > wireless ADB serial > ip route > wlan0
        public static string GetDeviceIp()
        {
            if (!string.IsNullOrEmpty(settings.SftpHost))
            {
                return settings.SftpHost;
            }

            string serial = ADB.DeviceID;
            if (!string.IsNullOrEmpty(serial))
            {
                Match m = Regex.Match(serial, @"^(\d{1,3}(?:\.\d{1,3}){3}):\d+$");
                if (m.Success)
                {
                    return m.Groups[1].Value;
                }
            }

            ProcessOutput route = ADB.RunAdbCommandToString("shell ip route", true);
            Match src = Regex.Match(route.Output, @"\bsrc\s+(\d{1,3}(?:\.\d{1,3}){3})");
            if (src.Success)
            {
                return src.Groups[1].Value;
            }

            ProcessOutput wlan = ADB.RunAdbCommandToString("shell ip -f inet addr show wlan0", true);
            Match inet = Regex.Match(wlan.Output, @"inet\s+(\d{1,3}(?:\.\d{1,3}){3})");
            return inet.Success ? inet.Groups[1].Value : null;
        }

        private static bool DetectSftp()
        {
            AuthFailedButServerPresent = false;

            string host = GetDeviceIp();
            if (string.IsNullOrEmpty(host))
            {
                Logger.Log("SFTP: could not determine headset IP address", LogLevel.WARNING);
                return false;
            }

            bool sawServer = false;
            int[] ports = settings.SftpPort > 0 ? new[] { settings.SftpPort } : DefaultPorts;
            foreach (int port in ports)
            {
                if (!HasSshBanner(host, port))
                {
                    continue;
                }
                sawServer = true;

                if (TryAuthenticate(host, port))
                {
                    Host = host;
                    Port = port;
                    return true;
                }
            }

            AuthFailedButServerPresent = sawServer;
            return false;
        }

        // SSH servers send their version banner immediately after accept, so a quick
        // connect+read cheaply filters out closed ports and non-SSH services.
        private static bool HasSshBanner(string host, int port)
        {
            try
            {
                using (TcpClient tcp = new TcpClient())
                {
                    IAsyncResult ar = tcp.BeginConnect(host, port, null, null);
                    if (!ar.AsyncWaitHandle.WaitOne(1000) )
                    {
                        return false;
                    }
                    tcp.EndConnect(ar);

                    NetworkStream stream = tcp.GetStream();
                    stream.ReadTimeout = 2000;
                    byte[] buffer = new byte[255];
                    int read = stream.Read(buffer, 0, buffer.Length);
                    bool isSsh = read >= 4 && System.Text.Encoding.ASCII.GetString(buffer, 0, read).StartsWith("SSH-");
                    if (isSsh)
                    {
                        Logger.Log($"SFTP: SSH server found on {host}:{port}");
                    }
                    return isSsh;
                }
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<string> CandidateKeyPaths()
        {
            if (!string.IsNullOrEmpty(settings.SftpPrivateKeyPath))
            {
                yield return settings.SftpPrivateKeyPath;
            }

            string sshDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
            yield return Path.Combine(sshDir, "quest_root");
            yield return Path.Combine(sshDir, "id_ed25519");
            yield return Path.Combine(sshDir, "id_rsa");
        }

        private static ConnectionInfo BuildConnectionInfo(string host, int port)
        {
            string user = string.IsNullOrEmpty(settings.SftpUsername) ? "root" : settings.SftpUsername;

            List<AuthenticationMethod> methods = new List<AuthenticationMethod>();
            List<PrivateKeyFile> keys = new List<PrivateKeyFile>();
            foreach (string keyPath in CandidateKeyPaths().Distinct().Where(File.Exists))
            {
                try
                {
                    keys.Add(new PrivateKeyFile(keyPath));
                }
                catch (Exception ex)
                {
                    Logger.Log($"SFTP: could not load key '{keyPath}': {ex.Message}", LogLevel.WARNING);
                }
            }
            if (keys.Count > 0)
            {
                methods.Add(new PrivateKeyAuthenticationMethod(user, keys.ToArray()));
            }
            if (!string.IsNullOrEmpty(settings.SftpPassword))
            {
                methods.Add(new PasswordAuthenticationMethod(user, settings.SftpPassword));

                // Many sshd builds are configured for keyboard-interactive instead of
                // plain password auth; answer every prompt with the stored password
                KeyboardInteractiveAuthenticationMethod interactive = new KeyboardInteractiveAuthenticationMethod(user);
                interactive.AuthenticationPrompt += (sender, e) =>
                {
                    foreach (Renci.SshNet.Common.AuthenticationPrompt prompt in e.Prompts)
                    {
                        prompt.Response = settings.SftpPassword;
                    }
                };
                methods.Add(interactive);
            }
            // Some rootful sshd builds accept root with no credentials at all
            methods.Add(new NoneAuthenticationMethod(user));

            ConnectionInfo info = new ConnectionInfo(host, port, user, methods.ToArray())
            {
                Timeout = TimeSpan.FromSeconds(6)
            };
            User = user;
            return info;
        }

        private static bool TryAuthenticate(string host, int port)
        {
            try
            {
                using (SftpClient sftp = new SftpClient(BuildConnectionInfo(host, port)))
                {
                    sftp.Connect();
                    string obbRoot = ObbRootCandidates.FirstOrDefault(sftp.Exists);
                    sftp.Disconnect();

                    if (obbRoot == null)
                    {
                        // An sshd whose mount namespace lacks the shared storage would
                        // silently strand files where no game can read them, so treat as unusable
                        Logger.Log($"SFTP: authenticated on {host}:{port} but {ObbRootCandidates[0]} is not visible - ignoring this server", LogLevel.WARNING);
                        return false;
                    }
                    _obbRoot = obbRoot;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"SFTP: authentication to {host}:{port} failed: {ex.Message}", LogLevel.WARNING);
                return false;
            }
        }

        // Attempts the OBB copy over SFTP. Returns null when SFTP is disabled, undetected
        // or fails, which tells the caller to fall back to the adb push path.
        public static async Task<ProcessOutput> TryCopyObbWithProgressAsync(
            string localPath,
            Action<float, TimeSpan?> progressCallback = null,
            Action<string> statusCallback = null,
            string gameName = "")
        {
            if (!settings.EnableSftpTransfers)
            {
                return null;
            }

            bool available = await DetectAsync();
            if (!available)
            {
                return null;
            }

            try
            {
                return await Task.Run(() => CopyObbCore(localPath, progressCallback, statusCallback, gameName));
            }
            catch (Exception ex)
            {
                Logger.Log($"SFTP: OBB transfer failed, falling back to adb push: {ex.Message}", LogLevel.ERROR);
                InvalidateCache();
                return null;
            }
        }

        private static ProcessOutput CopyObbCore(
            string localPath,
            Action<float, TimeSpan?> progressCallback,
            Action<string> statusCallback,
            string gameName)
        {
            string folderName = Path.GetFileName(localPath);
            string remotePath = $"{_obbRoot}/{folderName}";
            string quotedRemotePath = "'" + remotePath.Replace("'", "'\\''") + "'";

            statusCallback?.Invoke($"Preparing: {folderName} (SFTP)");
            progressCallback?.Invoke(0, null);

            string[] files = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);
            long totalBytes = files.Sum(f => new FileInfo(f).Length);
            long transferredBytes = 0; // shared across worker threads (Interlocked)

            DateTime startTime = DateTime.UtcNow;
            DateTime lastProgressUpdate = DateTime.MinValue;
            float lastReportedPercent = -1;
            const int ThrottleMs = 100;
            EtaEstimator eta = new EtaEstimator(alpha: 0.10, reanchorThreshold: 0.20);
            object progressLock = new object();

            // Recreate the OBB folder fresh, matching the adb push behaviour
            using (SshClient ssh = new SshClient(BuildConnectionInfo(Host, Port)))
            {
                ssh.Connect();
                using (SshCommand cmd = ssh.CreateCommand($"rm -rf {quotedRemotePath} && mkdir -p {quotedRemotePath}"))
                {
                    cmd.CommandTimeout = TimeSpan.FromSeconds(60);
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        throw new Exception($"remote mkdir failed: {cmd.Error}");
                    }
                }
                ssh.Disconnect();
            }

            // Map every local file to its remote path, and pre-create all remote
            // subdirectories up front on a single connection. Doing this before the
            // parallel phase avoids races on directory creation between workers.
            var remoteFiles = new Dictionary<string, string>(files.Length, StringComparer.Ordinal);
            var remoteDirs = new HashSet<string>(StringComparer.Ordinal);
            foreach (string file in files)
            {
                string relativePath = file.Substring(localPath.Length)
                                          .TrimStart('\\', '/')
                                          .Replace('\\', '/');
                string remoteFilePath = $"{remotePath}/{relativePath}";
                remoteFiles[file] = remoteFilePath;
                int slash = remoteFilePath.LastIndexOf('/');
                if (slash > 0)
                {
                    remoteDirs.Add(remoteFilePath.Substring(0, slash));
                }
            }
            if (remoteDirs.Any(d => d != remotePath))
            {
                using (SftpClient dirClient = new SftpClient(BuildConnectionInfo(Host, Port)))
                {
                    dirClient.Connect();
                    HashSet<string> created = new HashSet<string> { remotePath };
                    foreach (string dir in remoteDirs)
                    {
                        EnsureRemoteDirectory(dirClient, dir, created);
                    }
                    dirClient.Disconnect();
                }
            }

            // How many files to push at once. Each worker gets its own SFTP connection
            // (a single SSH.NET client is not safe for concurrent uploads) and pulls the
            // next file from a shared queue, FileZilla-style, so no more than `degree`
            // files transfer at any moment regardless of how many OBB files there are.
            int degree = settings.SingleThreadMode ? 1 : Math.Max(1, settings.SftpThreads);
            degree = Math.Min(degree, Math.Max(1, files.Length));

            statusCallback?.Invoke($"Copying: {folderName} (SFTP ×{degree})");

            // Register every file in the transfer window (largest first) and build the work queue.
            string[] ordered = files.OrderByDescending(f => new FileInfo(f).Length).ToArray();
            Dictionary<string, TransferItem> transferItems = new Dictionary<string, TransferItem>(StringComparer.Ordinal);
            foreach (string file in ordered)
            {
                transferItems[file] = TransferQueue.Add(Path.GetFileName(file), "SFTP", new FileInfo(file).Length);
            }
            System.Collections.Concurrent.ConcurrentQueue<string> workQueue =
                new System.Collections.Concurrent.ConcurrentQueue<string>(ordered);

            // Reports overall progress + throughput; called from every worker thread.
            Action<string> report = (fileName) =>
            {
                DateTime now = DateTime.UtcNow;
                long done = Interlocked.Read(ref transferredBytes);
                lock (progressLock)
                {
                    float overallPercent = totalBytes > 0
                        ? (float)(done * 100.0 / totalBytes)
                        : 0f;
                    overallPercent = Math.Max(0, Math.Min(100, overallPercent));

                    if (totalBytes > 0 && done > 0 && overallPercent < 100)
                    {
                        eta.Update(totalUnits: totalBytes, doneUnits: done);
                    }
                    TimeSpan? displayEta = eta.GetDisplayEta();

                    bool shouldUpdate = (now - lastProgressUpdate).TotalMilliseconds >= ThrottleMs
                                        || Math.Abs(overallPercent - lastReportedPercent) >= 0.1f;
                    if (shouldUpdate)
                    {
                        lastProgressUpdate = now;
                        lastReportedPercent = overallPercent;
                        double elapsed = (now - startTime).TotalSeconds;
                        double speedMBps = elapsed > 0.1 ? (done / 1048576.0) / elapsed : 0;
                        progressCallback?.Invoke(overallPercent, displayEta);
                        statusCallback?.Invoke($"{fileName} · {speedMBps:0.0} MB/s · SFTP");
                    }
                }
            };

            int failedCount = 0;
            Exception lastError = null;
            List<Task> workers = new List<Task>();
            for (int w = 0; w < degree; w++)
            {
                workers.Add(Task.Run(() =>
                {
                    SftpClient sftp = null;
                    try
                    {
                        while (workQueue.TryDequeue(out string file))
                        {
                            string remoteFilePath = remoteFiles[file];
                            string fileName = Path.GetFileName(file);
                            TransferItem item = transferItems[file];
                            long fileTotal = new FileInfo(file).Length;

                            bool ok = false;
                            Exception fileError = null;
                            for (int attempt = 0; attempt < 3 && !ok; attempt++)
                            {
                                long fileCounted = 0;
                                long lastUploaded = 0;
                                DateTime fileStart = DateTime.UtcNow;
                                try
                                {
                                    if (sftp == null || !sftp.IsConnected)
                                    {
                                        try { sftp?.Dispose(); } catch { }
                                        sftp = new SftpClient(BuildConnectionInfo(Host, Port));
                                        // A wedged channel makes each SFTP write request time out and
                                        // throw instead of blocking UploadFile forever (the softlock).
                                        sftp.OperationTimeout = TimeSpan.FromSeconds(60);
                                        sftp.Connect();
                                        sftp.BufferSize = TransferBufferSize;
                                    }

                                    TransferQueue.SetState(item, TransferState.Active);
                                    TransferQueue.Report(item, 0, 0);

                                    using (FileStream stream = File.OpenRead(file))
                                    {
                                        sftp.UploadFile(stream, remoteFilePath, true, uploaded =>
                                        {
                                            long u = (long)uploaded;
                                            long delta = u - lastUploaded;
                                            lastUploaded = u;
                                            if (delta != 0)
                                            {
                                                Interlocked.Add(ref transferredBytes, delta);
                                                fileCounted += delta;
                                            }
                                            double fe = (DateTime.UtcNow - fileStart).TotalSeconds;
                                            double fileMBps = fe > 0.1 ? (u / 1048576.0) / fe : 0;
                                            TransferQueue.Report(item, u, fileMBps);
                                            report(fileName);
                                        });
                                    }

                                    // Verify the file actually landed at the target path with the
                                    // exact size. A "completed" upload that isn't really there (or is
                                    // truncated) is a failure and must be retried, not reported as done.
                                    var attrs = sftp.GetAttributes(remoteFilePath);
                                    if (attrs == null || attrs.Size != fileTotal)
                                    {
                                        throw new Exception($"verification failed for {fileName}: remote size {(attrs != null ? attrs.Size : -1)} != {fileTotal}");
                                    }

                                    TransferQueue.Report(item, fileTotal, 0);
                                    TransferQueue.SetState(item, TransferState.Done);
                                    ok = true;
                                }
                                catch (Exception ex)
                                {
                                    fileError = ex;
                                    // undo this attempt's partial contribution to the overall total
                                    if (fileCounted != 0) Interlocked.Add(ref transferredBytes, -fileCounted);
                                    try { sftp?.Dispose(); } catch { }
                                    sftp = null; // force a fresh connection next attempt
                                    Logger.Log($"SFTP: '{fileName}' attempt {attempt + 1}/3 failed: {ex.Message}", LogLevel.WARNING);
                                    if (attempt < 2) System.Threading.Thread.Sleep(500 * (attempt + 1));
                                }
                            }

                            if (!ok)
                            {
                                string capturedFile = file;
                                string capturedRemote = remoteFilePath;
                                TransferItem capturedItem = item;
                                capturedItem.Retry = () => RetryUpload(capturedItem, capturedFile, capturedRemote);
                                TransferQueue.SetState(item, TransferState.Failed);
                                Interlocked.Increment(ref failedCount);
                                lock (progressLock) { if (lastError == null) lastError = fileError; }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (progressLock) { if (lastError == null) lastError = ex; }
                    }
                    finally
                    {
                        try { sftp?.Dispose(); } catch { }
                    }
                }));
            }
            Task.WaitAll(workers.ToArray());

            // If every file failed, the SFTP path is unusable (server gone / auth lost) - throw so
            // the caller falls back to adb push. If only some failed, leave them in the transfer
            // strip (marked Failed, each with a Retry action) instead of re-pushing the whole set.
            if (failedCount >= files.Length && lastError != null)
            {
                throw lastError;
            }

            // Make the freshly-written OBB tree readable by the game. adb push goes through
            // the adb daemon which lands app-readable perms; an sshd running as root can
            // instead write root-owned/restrictive files. On FUSE-emulated storage these
            // calls are effectively no-ops (perms/SELinux labels are synthesized), but where
            // the sshd writes through to a real filesystem this is what lets the app open its
            // OBB. Best-effort — failures are ignored.
            try
            {
                using (SshClient ssh = new SshClient(BuildConnectionInfo(Host, Port)))
                {
                    ssh.Connect();
                    using (SshCommand cmd = ssh.CreateCommand(
                        $"chmod -R 0777 {quotedRemotePath} 2>/dev/null; restorecon -R {quotedRemotePath} 2>/dev/null; true"))
                    {
                        cmd.CommandTimeout = TimeSpan.FromSeconds(30);
                        cmd.Execute();
                    }
                    ssh.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"SFTP: post-transfer chmod/restorecon skipped: {ex.Message}", LogLevel.WARNING);
            }

            progressCallback?.Invoke(100, null);
            statusCallback?.Invoke("");

            double totalSecs = Math.Max(0.1, (DateTime.UtcNow - startTime).TotalSeconds);
            Logger.Log($"SFTP: OBB '{folderName}' transferred ({totalBytes / 1048576.0:0.0} MB in {totalSecs:0.0}s = {(totalBytes / 1048576.0) / totalSecs:0.0} MB/s via {degree}× {User}@{Host}:{Port})");
            return new ProcessOutput($"{gameName}: OBB transfer (SFTP): Success\n", "");
        }

        // Re-uploads a single OBB file that previously failed. Runs on its own connection and
        // updates the transfer item in place. Wired to the transfer strip's "Retry" action.
        public static void RetryUpload(TransferItem item, string localFile, string remoteFilePath)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(localFile))
                    {
                        Logger.Log($"SFTP retry: local file missing: {localFile}", LogLevel.ERROR);
                        TransferQueue.SetState(item, TransferState.Failed);
                        return;
                    }

                    TransferQueue.SetState(item, TransferState.Queued);
                    TransferQueue.Report(item, 0, 0);

                    using (SftpClient sftp = new SftpClient(BuildConnectionInfo(Host, Port)))
                    {
                        sftp.OperationTimeout = TimeSpan.FromSeconds(60);
                        sftp.Connect();
                        sftp.BufferSize = TransferBufferSize;

                        int slash = remoteFilePath.LastIndexOf('/');
                        if (slash > 0)
                        {
                            EnsureRemoteDirectory(sftp, remoteFilePath.Substring(0, slash), new HashSet<string>());
                        }

                        long total = new FileInfo(localFile).Length;
                        DateTime start = DateTime.UtcNow;
                        using (FileStream stream = File.OpenRead(localFile))
                        {
                            sftp.UploadFile(stream, remoteFilePath, true, uploaded =>
                            {
                                long u = (long)uploaded;
                                double e = (DateTime.UtcNow - start).TotalSeconds;
                                double mbps = e > 0.1 ? (u / 1048576.0) / e : 0;
                                TransferQueue.Report(item, u, mbps);
                            });
                        }

                        var vattrs = sftp.GetAttributes(remoteFilePath);
                        if (vattrs == null || vattrs.Size != total)
                        {
                            throw new Exception($"verification failed: remote size {(vattrs != null ? vattrs.Size : -1)} != {total}");
                        }

                        TransferQueue.Report(item, total, 0);
                        TransferQueue.SetState(item, TransferState.Done);
                        sftp.Disconnect();
                    }

                    // best-effort perms fix on the retried file
                    try
                    {
                        string q = "'" + remoteFilePath.Replace("'", "'\\''") + "'";
                        using (SshClient ssh = new SshClient(BuildConnectionInfo(Host, Port)))
                        {
                            ssh.Connect();
                            using (SshCommand cmd = ssh.CreateCommand($"chmod 0777 {q} 2>/dev/null; restorecon {q} 2>/dev/null; true"))
                            {
                                cmd.CommandTimeout = TimeSpan.FromSeconds(15);
                                cmd.Execute();
                            }
                            ssh.Disconnect();
                        }
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    Logger.Log($"SFTP retry failed for {Path.GetFileName(localFile)}: {ex.Message}", LogLevel.ERROR);
                    TransferQueue.SetState(item, TransferState.Failed);
                }
            });
        }

        private static void EnsureRemoteDirectory(SftpClient sftp, string remoteDir, HashSet<string> created)
        {
            if (created.Contains(remoteDir))
            {
                return;
            }

            string[] segments = remoteDir.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string current = "";
            foreach (string segment in segments)
            {
                current += "/" + segment;
                if (created.Contains(current))
                {
                    continue;
                }
                if (!sftp.Exists(current))
                {
                    sftp.CreateDirectory(current);
                }
                created.Add(current);
            }
        }
    }
}
