using System.Runtime.CompilerServices;
using TqkLibrary.WpfUi.ObservableCollections.AsyncCollections;

namespace TestWpfApp.TestUC.TestAsyncCollection
{
    class AsyncData : IAsyncData<ItemData>
    {
        readonly HashSet<ItemData> _items = new HashSet<ItemData>();
        public AsyncData()
        {
            for (int i = 0; i < 1000; i++)
            {
                _items.Add(new ItemData() { Id = i });
            }
        }
        public async Task<bool> AddAsync(ItemData item, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Add(item);
        }
        public async Task AddsAsync(IEnumerable<ItemData> items, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            foreach (ItemData item in items)
                _items.Add(item);
        }
        public async Task<bool> ClearAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            _items.Clear();
            return true;
        }
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Count;
        }
        public async Task<bool> DeleteAsync(ItemData item, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Remove(item);
        }
        public async Task DeletesAsync(IEnumerable<ItemData> items, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            foreach (ItemData item in items)
                _items.Remove(item);
        }
        public async Task<IEnumerable<ItemData>> GetsAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.ToList();
        }
        public async Task<IEnumerable<ItemData>> GetsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Skip(page * pageSize).Take(pageSize).ToList();
        }
        public async Task<bool> UpdateAsync(ItemData item, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Contains(item);
        }
    }
}
