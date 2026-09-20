using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitObservableCollection<T> : DispatcherObservableCollection<T>
    {
        /// <summary>
        ///
        /// </summary>
        public LimitObservableCollection()
        {
            this.LogPath = new Func<string>(() => $"{Directory.GetCurrentDirectory()}\\{DateTime.Now:yyyy-MM-dd}.log");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delegatePath"></param>
        /// <param name="Limit"></param>
        /// <param name="IsInsertTop"></param>
        public LimitObservableCollection(Func<string> delegatePath, int Limit = 500, bool IsInsertTop = true)
        {
            this.LogPath = delegatePath;
            this.Limit = Limit;
            this.IsInsertTop = IsInsertTop;
        }
        /// <summary>
        ///
        /// </summary>
        public int Limit { get; set; } = 100;

        /// <summary>
        ///
        /// </summary>
        public bool IsInsertTop { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public bool IsExportToFile { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public Func<string> LogPath { get; set; }

        /// <summary>
        /// Upper bound on how many queued lines go into a single file write, so a burst cannot grow
        /// one buffer without limit.
        /// </summary>
        const int MaxLinesPerWrite = 512;

        readonly ConcurrentQueue<string> _pendingLines = new ConcurrentQueue<string>();
        /// <summary>
        /// 0 when no drain is running. Guarantees a single writer, which both serialises access to
        /// the file and stops every queued line from scheduling its own task.
        /// </summary>
        int _isDraining = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            if (this.Count == this.Limit) base.RemoveAt(this.IsInsertTop ? this.Count - 1 : 0);
            if (this.LogPath != null && this.IsExportToFile)
            {
                _ = AppendFileAsync(item);
            }
            base.InsertItem(this.IsInsertTop ? 0 : this.Count, item);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();// base.MoveItem(oldIndex, newIndex);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();// base.SetItem(IsInsertTop ? 0 : this.Count, item);
        }


        public async Task AddAsync(T item, bool isInsertcollection, CancellationToken cancellationToken = default)
        {
            if (isInsertcollection)
            {
                await this.AddAsync(item, cancellationToken);
            }
            else
            {
                await AppendFileAsync(item);
            }
        }

        /// <summary>
        /// Queues the item for the background writer and returns.
        /// <para>The write itself must not run on the dispatcher: InsertItem is called on the UI
        /// thread, and opening, appending to and closing the file once per line froze the UI as soon
        /// as lines arrived faster than a few per second.</para>
        /// <para>The returned task completes when the queue has been drained by the writer this call
        /// started. If another drain was already running it completes immediately, the line still
        /// being written by that one.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task AppendFileAsync(T item)
        {
            string? data = item?.ToString();
            if (string.IsNullOrWhiteSpace(data)) return Task.CompletedTask;

            _pendingLines.Enqueue(data!);
            return ScheduleDrain();
        }

        /// <summary>
        /// Starts a drain unless one is already running. Task.Run matters: without it the drain
        /// would run synchronously on the caller - the UI thread - up to its first real await.
        /// </summary>
        Task ScheduleDrain()
        {
            if (Interlocked.CompareExchange(ref _isDraining, 1, 0) != 0) return Task.CompletedTask;
            return Task.Run(DrainAsync);
        }

        async Task DrainAsync()
        {
            try
            {
                while (!_pendingLines.IsEmpty)
                {
                    string path = this.LogPath?.Invoke() ?? string.Empty;
                    if (string.IsNullOrEmpty(path))
                    {
                        //nowhere to write, so do not let the queue grow forever
                        while (_pendingLines.TryDequeue(out _)) { }
                        return;
                    }

                    StringBuilder builder = new StringBuilder();
                    int lines = 0;
                    while (lines < MaxLinesPerWrite && _pendingLines.TryDequeue(out string? line))
                    {
                        builder.AppendLine(line);
                        lines++;
                    }
                    if (lines == 0) return;

                    try
                    {
                        //one open/close per batch instead of per line
                        using StreamWriter sw = new(path, true, Encoding.UTF8);
                        await sw.WriteAsync(builder.ToString()).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error writing to log file: {ex.Message}");
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isDraining, 0);
            }

            //lines queued between the last emptiness check and releasing the flag would otherwise
            //sit there until something else is logged
            if (!_pendingLines.IsEmpty) _ = ScheduleDrain();
        }
    }
}
