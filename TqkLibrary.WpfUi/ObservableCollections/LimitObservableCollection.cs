﻿using System;
using System.IO;

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
                string path = this.LogPath.Invoke();
                if (!string.IsNullOrEmpty(path))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            using StreamWriter sw = new(path, true);
                            sw.WriteLine(item?.ToString());
                        }
                        catch { }
                    });
                }
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
    }
}