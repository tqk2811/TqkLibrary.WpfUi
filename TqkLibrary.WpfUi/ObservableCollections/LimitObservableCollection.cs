using System;
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

        public async Task AppendFileAsync(T item)
        {
            string? data = item?.ToString();
            if (!string.IsNullOrWhiteSpace(data))
            {
                string path = this.LogPath.Invoke();
                if (!string.IsNullOrEmpty(path))
                {
                    await this.Dispatcher.InvokeAsync(async () =>
                    {
                        try
                        {
                            using StreamWriter sw = new(path, true, Encoding.UTF8);
                            await sw.WriteLineAsync(data);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error writing to log file: {ex.Message}");
                        }
                    });
                }
            }
        }
    }
}