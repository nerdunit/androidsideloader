using System;
using System.Collections.Generic;

namespace AndroidSideloader
{
    public enum TransferState { Queued, Active, Done, Failed }

    // A single file transfer surfaced in the integrated transfer strip.
    public class TransferItem
    {
        public string Name;
        public string Kind;          // "SFTP", "Push", "Pull"
        public long TotalBytes;
        public long TransferredBytes;
        public double SpeedMBps;
        public TransferState State = TransferState.Queued;
        public DateTime? CompletedUtc; // set when the item reaches Done/Failed, for auto-clear
        public Action Retry;           // re-attempts this single file; set when it fails
    }

    // Thread-safe registry of in-flight file transfers. Producers are the transfer
    // workers (background threads); the consumer is the TransferStrip UI timer, which
    // polls Snapshot() and auto-removes finished rows via PurgeCompleted().
    public static class TransferQueue
    {
        private static readonly object _lock = new object();
        private static readonly List<TransferItem> _items = new List<TransferItem>();

        public static TransferItem Add(string name, string kind, long totalBytes)
        {
            TransferItem item = new TransferItem { Name = name, Kind = kind, TotalBytes = totalBytes };
            lock (_lock)
            {
                _items.Add(item);
            }
            return item;
        }

        public static void SetState(TransferItem item, TransferState state)
        {
            if (item == null) return;
            lock (_lock)
            {
                item.State = state;
                if (state == TransferState.Done || state == TransferState.Failed)
                {
                    item.CompletedUtc = DateTime.UtcNow;
                }
            }
        }

        public static void Report(TransferItem item, long transferred, double speedMBps)
        {
            if (item == null) return;
            lock (_lock)
            {
                item.TransferredBytes = transferred;
                item.SpeedMBps = speedMBps;
                if (item.State == TransferState.Queued)
                {
                    item.State = TransferState.Active;
                }
            }
        }

        public static List<TransferItem> Snapshot()
        {
            lock (_lock)
            {
                return new List<TransferItem>(_items);
            }
        }

        // Auto-clears *successful* rows a short while after they finish, so a "Done" row is
        // visible briefly then disappears. Failed rows are kept so the user can retry them.
        public static void PurgeCompleted(double doneSeconds)
        {
            DateTime now = DateTime.UtcNow;
            lock (_lock)
            {
                _items.RemoveAll(i =>
                    i.State == TransferState.Done &&
                    i.CompletedUtc.HasValue &&
                    (now - i.CompletedUtc.Value).TotalSeconds >= doneSeconds);
            }
        }

        public static void Remove(TransferItem item)
        {
            if (item == null) return;
            lock (_lock)
            {
                _items.Remove(item);
            }
        }

        // Clears finished rows on demand (both Done and Failed).
        public static void ClearFinished()
        {
            lock (_lock)
            {
                _items.RemoveAll(i => i.State == TransferState.Done || i.State == TransferState.Failed);
            }
        }
    }
}
