using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
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
        protected virtual void NotifyPropertyChange([CallerMemberName] string name = "") => OnPropertyChanged(new PropertyChangedEventArgs(name));

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
            foreach (int keyPage in _pages.Keys)
            {
                foreach (int keyIndex in _pages[keyPage].Items.Keys)
                {
                    _pages[keyPage].Items[keyIndex].ViewModel = default(TViewModel);
                    _pages[keyPage].Items[keyIndex].DisableFetchCount = 0;
                };
            }
#if DEBUG
            Debug.WriteLine($"==================Reset================");
#endif
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
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

                int pageIndex = index / PageSize;
                int pageOffset = index % PageSize;

                // remove stale pages
                CleanUpPages();

                bool isShouldRequestPage = !_pages.ContainsKey(pageIndex);//should request if was not request
                if (!isShouldRequestPage)
                    isShouldRequestPage = _pages[pageIndex].Items.All(x => x.Value.ViewModel is null);//page was reset
                if (!isShouldRequestPage && _pages[pageIndex].Items[pageOffset].DisableFetchCount > 0)//disable RequestPage by noti CollectionChanged
                {
                    _pages[pageIndex].Items[pageOffset].DisableFetchCount--;
                    isShouldRequestPage = false;
#if DEBUG
                    Debug.WriteLine($"Skip RequestPage at {index}");
#endif
                }
                if (isShouldRequestPage)
                {
                    RequestPage(pageIndex);

                    // if accessing upper 50% then request next page
                    if (pageOffset > PageSize / 2 && pageIndex < Count / PageSize)
                        RequestPage(pageIndex + 1);

                    // if accessing lower 50% then request prev page
                    if (pageOffset < PageSize / 2 && pageIndex > 0)
                        RequestPage(pageIndex - 1);
                }
                var result = _pages[pageIndex].Items[pageOffset].ViewModel;
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
                if (_count == -1)
                {
                    Task.Run(() => FetchCountAsync(false));
                    return 0;
                }
                return _count;
            }
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => this;


        [Obsolete] public void Add(TViewModel item) => throw new NotSupportedException();
        [Obsolete] public void Clear() => throw new NotSupportedException();
        public bool Contains(TViewModel item) => _pages.ViewModels.Contains(item);
        [Obsolete] public void CopyTo(TViewModel[] array, int arrayIndex) => throw new NotSupportedException();
        [Obsolete] public int IndexOf(TViewModel item) => -1;
        [Obsolete] public void Insert(int index, TViewModel item) => throw new NotSupportedException();
        [Obsolete] public bool Remove(TViewModel item) => throw new NotSupportedException();
        [Obsolete] public void RemoveAt(int index) => throw new NotSupportedException();

        [Obsolete] int IList.Add(object? value) => throw new NotSupportedException();
        bool IList.Contains(object? value) => Contains((TViewModel)value!);
        [Obsolete] int IList.IndexOf(object? value) => IndexOf((TViewModel)value!);
        [Obsolete] void IList.Insert(int index, object? value) => Insert(index, (TViewModel)value!);
        [Obsolete] void IList.Remove(object? value) => throw new NotSupportedException();
        [Obsolete] void ICollection.CopyTo(Array array, int index) => throw new NotSupportedException();


        public IEnumerator<TViewModel> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i]!;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
                Items = new();
                for (int i = 0; i < pageSize; i++)
                {
                    Items[i] = new ItemData() { ViewModel = default(TViewModel) };
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

            if (!_pages.ContainsKey(pageIndex))
                _pages[pageIndex] = new PageData(PageSize);

            bool isShouldFetch = _pages[pageIndex].TaskFetchPage is null;//was not fetch
            if (!isShouldFetch && (_pages[pageIndex].TaskFetchPage!.IsCanceled || _pages[pageIndex].TaskFetchPage!.IsFaulted))//fetch error
                isShouldFetch = true;


            if (_pages[pageIndex].TaskFetchPage is null ||
                (_pages[pageIndex].TaskFetchPage!.IsCompleted &&
                    ((DateTime.Now - _pages[pageIndex].TouchTime).TotalMilliseconds > PageTimeout || _pages[pageIndex].Items.All(x => x.Value.ViewModel is null))
                )
                )
            {
                //make new fetch
                _pages[pageIndex].TaskFetchPage = Task.Run<Task>(() => FetchPageAsync(pageIndex)).Unwrap();
            }
        }
        protected virtual void CleanUpPages()
        {
            this.Dispatcher.VerifyAccess();

            foreach (int keyPage in _pages.Keys.ToList())
            {
                if (
                    _pages[keyPage].Items.Values.Any(x => x.ViewModel is not null) &&
                    (DateTime.Now - _pages[keyPage].TouchTime).TotalMilliseconds > PageTimeout
                    )
                {
                    foreach (int keyIndex in _pages[keyPage].Items.Keys)
                    {
                        TViewModel? oldItem = _pages[keyPage].Items[keyIndex].ViewModel;
                        TViewModel? newItem = default(TViewModel);
                        _pages[keyPage].Items[keyIndex].ViewModel = newItem;
                        FireItemReplace(_pages[keyPage].Items[keyIndex], keyPage * PageSize + keyIndex, newItem, oldItem);
                    };
                }
            }
        }

        #region Fetch
        protected virtual async Task FetchCountAsync(bool force = false)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(FetchTimeout);
            int count = await _asyncData.CountAsync(timeout.Token);
            if (count != _count || force)
            {
                _count = count;
                _ = this.Dispatcher.InvokeAsync(FireCollectionReset);
            }
        }
        protected virtual async Task FetchPageAsync(int pageIndex)
        {
            Debug.WriteLine($"Start Fetch page {pageIndex}");
            using CancellationTokenSource timeout = new CancellationTokenSource(FetchTimeout);
            IEnumerable<TData> datas = await _asyncData.GetsAsync(pageIndex, PageSize, timeout.Token);
            await Dispatcher.TrueThreadInvokeAsync(() =>
            {
                int indexInPage = 0;
                _pages[pageIndex].TouchTime = DateTime.Now;
                foreach (var data in datas)
                {
                    TViewModel newItem = _func(data);
                    TViewModel? oldItem = _pages[pageIndex].Items[indexInPage].ViewModel;
                    _pages[pageIndex].Items[indexInPage].ViewModel = newItem;
                    FireItemReplace(_pages[pageIndex].Items[indexInPage], pageIndex * PageSize + indexInPage, newItem, oldItem);
                    indexInPage++;
                }
            });
            Debug.WriteLine($"End Fetch page {pageIndex}");
        }
        #endregion

        #endregion
    }
}