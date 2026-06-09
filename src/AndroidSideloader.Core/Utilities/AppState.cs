using AndroidSideloader.Models;
using System;

namespace AndroidSideloader.Utilities
{
    public static class AppState
    {
        public static bool IsOffline { get; set; } = false;
        public static bool DebugMode { get; set; } = false;
        public static bool IsManualMirrorSwitch { get; set; } = false;
        public static string CurrentRemote { get; set; } = string.Empty;
        public static PublicConfig? PublicConfigFile { get; set; }
        public static string PublicMirrorExtraArgs { get; set; } = string.Empty;
        public static bool NoRcloneUpdating { get; set; } = false;

        // Events for UI-layer callbacks
        public static event Action<string, string>? OnShowMessageBox;
        public static event Action<string>? OnShowError;
        public static event Action? OnSwitchMirrors;
        public static event Action<string>? OnChangeTitle;
        public static event Action<string, float, TimeSpan?>? OnDownloadProgress;
        public static event Action<string>? OnStatusUpdate;

        public static void ShowMessageBox(string title, string message)
        {
            OnShowMessageBox?.Invoke(title, message);
        }

        public static void ShowError(string message)
        {
            OnShowError?.Invoke(message);
        }

        public static void ChangeTitle(string title)
        {
            OnChangeTitle?.Invoke(title);
        }

        public static void ReportProgress(string context, float percent, TimeSpan? eta)
        {
            OnDownloadProgress?.Invoke(context, percent, eta);
        }

        public static void UpdateStatus(string status)
        {
            OnStatusUpdate?.Invoke(status);
        }
    }
}
