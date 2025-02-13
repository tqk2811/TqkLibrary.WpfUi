using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TqkLibrary.WpfUi.Interfaces;

namespace TqkLibrary.WpfUi.ObservableCollections.AsyncCollections
{
    public class AsyncCollection<TData, TViewModel> : IList<TViewModel>, IList, INotifyPropertyChanged, INotifyCollectionChanged
        where TViewModel : IViewModel<TData>
    {
        #region INotifyPropertyChanged
        public virtual event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        protected virtual void NotifyPropertyChange([CallerMemberName] string name = "") => this.OnPropertyChanged(new PropertyChangedEventArgs(name));

        #endregion

        #region INotifyCollectionChanged
        public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
            //Dispatcher.InvokeAsync(() => CollectionChanged?.Invoke(this, e));
        }
        protected virtual void FireCollectionReset()
        {
            this.Dispatcher.VerifyAccess();
            foreach (int keyPage in this._pages.Keys)
            {
                foreach (int keyIndex in this._pages[keyPage].Items.Keys)
                {
                    this._pages[keyPage].Items[keyIndex].ViewModel = default;
                    this._pages[keyPage].Items[keyIndex].DisableFetchCount = 0;
                };
            }
#if DEBUG
            Debug.WriteLine($"==================Reset================");
#endif
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        void FireItemReplace(PageData.ItemData itemData, int index, TViewModel? newItem, TViewModel? oldItem
#if DEBUG
            , [CallerMemberName] string callFrom = ""
#endif
            )
        {
            this.Dispatcher.VerifyAccess();
            if (!ReferenceEquals(newItem, oldItem))
            {
#if DEBUG
                Debug.WriteLine($"[{callFrom}] Replace index {index} from {(oldItem is null ? "null" : nameof(oldItem))} to {(newItem is null ? "null" : nameof(newItem))}");
#endif
                if (oldItem is null && newItem is not null)
                    itemData.DisableFetchCount = 1;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            }
        }
        #endregion



        readonly IRemoteDataReadonly<TData> _asyncData;
        readonly Func<TData, TViewModel> _func;


        public SynchronizationContext SynchronizationContext { get; }
        public Dispatcher Dispatcher { get; }



        public virtual int PageSize { get; } = 50;
        public virtual int FetchTimeout { get; set; } = 30000;
        public virtual int PageTimeout { get; set; }
#if DEBUG
        = 10000;
#else
        = 30000;
#endif
        public virtual bool IsLoading { get; protected set; } = false;


        public AsyncCollection(
           IRemoteDataReadonly<TData> asyncData,
           Func<TData, TViewModel> func,
           int pageSize = 50
           ) :
            this(
                asyncData,
                func,
                pageSize,
                Application.Current.Dispatcher,
                SynchronizationContext.Current ?? throw new InvalidOperationException("Must create in main thread")
            )
        {
        }

        public AsyncCollection(
            IRemoteDataReadonly<TData> asyncData,
            Func<TData, TViewModel> func,
            int pageSize,
            Dispatcher dispatcher,
            SynchronizationContext synchronizationContext
            )
        {
            this._asyncData = asyncData ?? throw new ArgumentNullException(nameof(asyncData));
            this._func = func ?? throw new ArgumentNullException(nameof(func));
            this.Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.SynchronizationContext = synchronizationContext ?? throw new ArgumentNullException(nameof(synchronizationContext));
            this.PageSize = pageSize;
        }






        #region IList<TViewModel>, IList
        public TViewModel this[int index]//current bropblem loop fetch all item
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                //main thread access only

                int pageIndex = index / this.PageSize;
                int pageOffset = index % this.PageSize;

                // remove stale pages
                this.CleanUpPages();

                bool isShouldRequestPage = !this._pages.ContainsKey(pageIndex);//should request if was not request
                if (!isShouldRequestPage)
                    isShouldRequestPage = this._pages[pageIndex].Items.All(x => x.Value.ViewModel is null);//page was reset
                if (!isShouldRequestPage && this._pages[pageIndex].Items[pageOffset].DisableFetchCount > 0)//disable RequestPage by noti CollectionChanged
                {
                    this._pages[pageIndex].Items[pageOffset].DisableFetchCount--;
                    isShouldRequestPage = false;
#if DEBUG
                    Debug.WriteLine($"Skip RequestPage at {index}");
#endif
                }
                if (isShouldRequestPage)
                {
                    this.RequestPage(pageIndex);

                    // if accessing upper 50% then request next page
                    if (pageOffset > this.PageSize / 2 && pageIndex < this.Count / this.PageSize)
                        this.RequestPage(pageIndex + 1);

                    // if accessing lower 50% then request prev page
                    if (pageOffset < this.PageSize / 2 && pageIndex > 0)
                        this.RequestPage(pageIndex - 1);
                }
                var result = this._pages[pageIndex].Items[pageOffset].ViewModel;
#if DEBUG
                Debug.WriteLine($"Get at {index}, values is {(result is null ? "null" : "not null")}");
#endif
                return result!;
            }
            set => throw new NotSupportedException();
        }
        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }


        int _count = -1;
        public int Count
        {
            get
            {
                if (this._count == -1)
                {
                    Task.Run(() => this.FetchCountAsync(false));
                    return 0;
                }
                return this._count;
            }
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => this;


        [Obsolete] public void Add(TViewModel item) => throw new NotSupportedException();
        [Obsolete] public void Clear() => throw new NotSupportedException();
        public bool Contains(TViewModel item) => this._pages.ViewModels.Contains(item);
        [Obsolete] public void CopyTo(TViewModel[] array, int arrayIndex) => throw new NotSupportedException();
        [Obsolete] public int IndexOf(TViewModel item) => -1;
        [Obsolete] public void Insert(int index, TViewModel item) => throw new NotSupportedException();
        [Obsolete] public bool Remove(TViewModel item) => throw new NotSupportedException();
        [Obsolete] public void RemoveAt(int index) => throw new NotSupportedException();

        [Obsolete] int IList.Add(object? value) => throw new NotSupportedException();
        bool IList.Contains(object? value) => this.Contains((TViewModel)value!);
        [Obsolete] int IList.IndexOf(object? value) => this.IndexOf((TViewModel)value!);
        [Obsolete] void IList.Insert(int index, object? value) => this.Insert(index, (TViewModel)value!);
        [Obsolete] void IList.Remove(object? value) => throw new NotSupportedException();
        [Obsolete] void ICollection.CopyTo(Array array, int index) => throw new NotSupportedException();


        public IEnumerator<TViewModel> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i]!;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion



        #region page
        class PageData
        {
            public class ItemData
            {
                public TViewModel? ViewModel { get; set; }
                public int DisableFetchCount { get; set; } = 0;
            }
            public PageData(int pageSize)
            {
                this.Items = new();
                for (int i = 0; i < pageSize; i++)
                {
                    this.Items[i] = new ItemData() { ViewModel = default };
                }
            }
            public Dictionary<int, ItemData> Items { get; }
            public DateTime TouchTime { get; set; } = DateTime.Now;
            public Task? TaskFetchPage { get; set; }
        }
        class DictPageData : Dictionary<int, PageData>
        {
            public IEnumerable<TViewModel?> ViewModels
            {
                get
                {
                    foreach (PageData pageData in this.Values.ToList())
                    {
                        foreach (PageData.ItemData itemData in pageData.Items.Values)
                        {
                            yield return itemData.ViewModel;
                        }
                    }
                }
            }
        }

        private readonly DictPageData _pages = new();

        protected virtual void RequestPage(int pageIndex)
        {
            this.Dispatcher.VerifyAccess();

            if (!this._pages.ContainsKey(pageIndex))
                this._pages[pageIndex] = new PageData(this.PageSize);

            bool isShouldFetch = this._pages[pageIndex].TaskFetchPage is null;//was not fetch
            if (!isShouldFetch && (this._pages[pageIndex].TaskFetchPage!.IsCanceled || this._pages[pageIndex].TaskFetchPage!.IsFaulted))//fetch error
                isShouldFetch = true;


            if (this._pages[pageIndex].TaskFetchPage is null ||
                (this._pages[pageIndex].TaskFetchPage!.IsCompleted &&
                    ((DateTime.Now - this._pages[pageIndex].TouchTime).TotalMilliseconds > this.PageTimeout || this._pages[pageIndex].Items.All(x => x.Value.ViewModel is null))
                )
                )
            {
                //make new fetch
                this._pages[pageIndex].TaskFetchPage = Task.Run<Task>(() => this.FetchPageAsync(pageIndex)).Unwrap();
            }
        }
        protected virtual void CleanUpPages()
        {
            this.Dispatcher.VerifyAccess();

            foreach (int keyPage in this._pages.Keys.ToList())
            {
                if (
                    this._pages[keyPage].Items.Values.Any(x => x.ViewModel is not null) &&
                    (DateTime.Now - this._pages[keyPage].TouchTime).TotalMilliseconds > this.PageTimeout
                    )
                {
                    foreach (int keyIndex in this._pages[keyPage].Items.Keys)
                    {
                        TViewModel? oldItem = this._pages[keyPage].Items[keyIndex].ViewModel;
                        TViewModel? newItem = default;
                        this._pages[keyPage].Items[keyIndex].ViewModel = newItem;
                        this.FireItemReplace(this._pages[keyPage].Items[keyIndex], keyPage * this.PageSize + keyIndex, newItem, oldItem);
                    };
                }
            }
        }

        #region Fetch
        protected virtual async Task FetchCountAsync(bool force = false)
        {
            using CancellationTokenSource timeout = new(this.FetchTimeout);
            int count = await this._asyncData.CountAsync(timeout.Token);
            if (count != this._count || force)
            {
                this._count = count;
                _ = this.Dispatcher.InvokeAsync(this.FireCollectionReset);
            }
        }
        protected virtual async Task FetchPageAsync(int pageIndex)
        {
            Debug.WriteLine($"Start Fetch page {pageIndex}");
            using CancellationTokenSource timeout = new(this.FetchTimeout);
            IEnumerable<TData> datas = await this._asyncData.GetsAsync(pageIndex, this.PageSize, timeout.Token);
            await this.Dispatcher.TrueThreadInvokeAsync(() =>
            {
                int indexInPage = 0;
                this._pages[pageIndex].TouchTime = DateTime.Now;
                foreach (var data in datas)
                {
                    TViewModel newItem = this._func(data);
                    TViewModel? oldItem = this._pages[pageIndex].Items[indexInPage].ViewModel;
                    this._pages[pageIndex].Items[indexInPage].ViewModel = newItem;
                    this.FireItemReplace(this._pages[pageIndex].Items[indexInPage], pageIndex * this.PageSize + indexInPage, newItem, oldItem);
                    indexInPage++;
                }
            });
            Debug.WriteLine($"End Fetch page {pageIndex}");
        }
        #endregion

        #endregion
    }
}