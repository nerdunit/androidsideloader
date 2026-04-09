using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AndroidSideloader.Utilities
{
    public class ExtractionException : Exception
    {
        public ExtractionException(string message) : base(message) { }
    }

    internal class Zip
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;

        // Progress callback: (percent, eta)
        public static Action<float, TimeSpan?> ExtractionProgressCallback { get; set; }
        public static Action<string> ExtractionStatusCallback { get; set; }

        public static void ExtractFile(string sourceArchive, string destination)
        {
            string args = $"x \"{sourceArchive}\" -y -o\"{destination}\" -bsp1";
            DoExtract(args);
        }

        public static void ExtractFile(string sourceArchive, string destination, string password)
        {
            string args = $"x \"{sourceArchive}\" -y -o\"{destination}\" -p\"{password}\" -bsp1";
            DoExtract(args);
        }

        private static string extractionError = null;
        private static bool errorMessageShown = false;

        private static void DoExtract(string args)
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "7z.exe")) || !File.Exists(Path.Combine(Environment.CurrentDirectory, "7z.dll")))
            {
                _ = Logger.Log("Begin download 7-zip");
                string architecture = Environment.Is64BitOperatingSystem ? "64" : "";
                try
                {
                    // Use DNS fallback download method from GetDependencies
                    GetDependencies.DownloadFileWithDnsFallback($"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/7z{architecture}.exe", "7z.exe");
                    GetDependencies.DownloadFileWithDnsFallback($"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/7z{architecture}.dll", "7z.dll");
                }
                catch (Exception ex)
                {
                    _ = FlexibleMessageBox.Show(Program.form, $"You are unable to access the GitHub page with the Exception: {ex.Message}\nSome files may be missing (7z)");
                    _ = FlexibleMessageBox.Show(Program.form, "7z was unable to be downloaded\nRookie will now close");
                    Application.Exit();
                }
                _ = Logger.Log("Complete download 7-zip");
            }

            ProcessStartInfo pro = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "7z.exe",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            _ = Logger.Log($"Extract: 7z {string.Join(" ", args.Split(' ').Where(a => !a.StartsWith("-p")))}");

            // Throttle percent reports
            float lastReportedPercent = -1;

            // ETA engine (percent units)
            var etaEstimator = new EtaEstimator(alpha: 0.10, reanchorThreshold: 0.20, minSampleSeconds: 0.10);

            // Smooth progress (sub-percent) interpolation (because 7z -bsp1 is integer-only)
            System.Threading.Timer smoothTimer = null;
            int extractingFlag = 1; // 1 = extracting, 0 = stop
            float smoothLastTickPercent = 0f;
            DateTime smoothLastTickTime = DateTime.UtcNow;
            float smoothLastReported = -1f;
            const int SmoothIntervalMs = 80;        // ~12.5 updates/sec
            const float SmoothReportDelta = 0.10f;  // report only if change >= 0.10%

            using (Process x = new Process())
            {
                x.StartInfo = pro;

                if (MainForm.isInDownloadExtract && x != null)
                {
                    // Smooth sub-percent UI, while keeping ETA ticking
                    smoothTimer = new System.Threading.Timer(_ =>
                    {
                        if (System.Threading.Volatile.Read(ref extractingFlag) == 0) return;
                        if (smoothLastTickPercent <= 0) return; // need at least one 7z tick

                        // Use current ETA to approximate seconds-per-percent
                        TimeSpan? displayEta = etaEstimator.GetDisplayEta();
                        if (!displayEta.HasValue) return; // Skip until ETA exists

                        var now = DateTime.UtcNow;
                        var elapsed = (now - smoothLastTickTime).TotalSeconds;

                        // Approx seconds-per-percent from remaining ETA / remaining percent
                        double remainingPercent = Math.Max(1.0, 100.0 - smoothLastTickPercent);
                        double spp = Math.Max(0.05, displayEta.Value.TotalSeconds / remainingPercent);

                        float candidate = smoothLastTickPercent + (float)(elapsed / spp);

                        // Clamp
                        float floorTick = (float)Math.Floor(smoothLastTickPercent);
                        float ceiling = Math.Min(99.99f, floorTick + 0.999f);

                        if (candidate > ceiling) candidate = ceiling;
                        if (candidate < smoothLastTickPercent) candidate = smoothLastTickPercent;

                        if (smoothLastReported >= 0 && Math.Abs(candidate - smoothLastReported) < SmoothReportDelta) return;
                        smoothLastReported = candidate;

                        try
                        {
                            MainForm mainForm = (MainForm)Application.OpenForms[0];
                            if (mainForm != null && !mainForm.IsDisposed)
                            {
                                mainForm.BeginInvoke((Action)(() => mainForm.SetProgress(candidate)));
                            }
                        }
                        catch { }

                        // ETA countdown ticks even if 7z percent is unchanged
                        ExtractionProgressCallback?.Invoke(candidate, etaEstimator.GetDisplayEta());

                    }, null, SmoothIntervalMs, SmoothIntervalMs);

                    x.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            var match = Regex.Match(e.Data, @"^\s*(\d+)%");
                            if (match.Success && float.TryParse(match.Groups[1].Value, out float percent))
                            {
                                // Update ETA from integer percent
                                if (percent <= 0.0f) etaEstimator.Reset();
                                else if (percent < 100.0f) etaEstimator.Update(totalUnits: 100, doneUnits: (long)Math.Round(percent));

                                // Reset smoothing baseline on each integer tick
                                smoothLastTickPercent = percent;
                                smoothLastTickTime = DateTime.UtcNow;
                                smoothLastReported = percent;

                                if (Math.Abs(percent - lastReportedPercent) >= 0.1f)
                                {
                                    lastReportedPercent = percent;

                                    MainForm mainForm = (MainForm)Application.OpenForms[0];
                                    if (mainForm != null)
                                    {
                                        mainForm.Invoke((Action)(() => mainForm.SetProgress(percent)));
                                    }

                                    ExtractionProgressCallback?.Invoke(percent, etaEstimator.GetDisplayEta());
                                }
                            }
                        }
                    };
                }

                x.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        var error = e.Data;
                        if (error.Contains("There is not enough space on the disk") && !errorMessageShown)
                        {
                            errorMessageShown = true;
                            Program.form.Invoke(new Action(() =>
                            {
                                _ = FlexibleMessageBox.Show(Program.form, $"Not enough space to extract archive.\r\nMake sure your {Path.GetPathRoot(settings.DownloadDir)} drive has at least double the space of the game, then try again.",
                                   "NOT ENOUGH SPACE",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                                return;
                            }));
                        }
                        _ = Logger.Log(error, LogLevel.ERROR);
                        extractionError = $"Extracting failed: {error}"; // Store the error message directly
                        return;
                    }
                };

                x.Start();
                x.BeginOutputReadLine();
                x.BeginErrorReadLine();
                x.WaitForExit();

                // Stop smoother
                System.Threading.Interlocked.Exchange(ref extractingFlag, 0);
                smoothTimer?.Dispose();
                smoothTimer = null;

                // Clear callbacks
                ExtractionProgressCallback?.Invoke(100, null);
                ExtractionStatusCallback?.Invoke("");

                errorMessageShown = false;

                if (!string.IsNullOrEmpty(extractionError))
                {
                    string errorMessage = extractionError;
                    extractionError = null; // Reset the error message
                    throw new ExtractionException(errorMessage);
                }
            }
        }
    }
}