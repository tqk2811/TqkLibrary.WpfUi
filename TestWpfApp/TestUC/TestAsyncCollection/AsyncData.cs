using System.Runtime.CompilerServices;
using TqkLibrary.WpfUi.Interfaces;
using TqkLibrary.WpfUi.ObservableCollections.AsyncCollections;

namespace TestWpfApp.TestUC.TestAsyncCollection
{
    class AsyncData : IRemoteDataReadonly<ItemData>
    {
        readonly HashSet<ItemData> _items = new HashSet<ItemData>();
        public AsyncData()
        {
            for (int i = 0; i < 1000; i++)
            {
                _items.Add(new ItemData() { Id = i });
            }
        }
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Count;
        }
        public async IAsyncEnumerable<ItemData> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            foreach (ItemData item in _items)
            {
                yield return item;
            }
        }
        public async Task<ItemData> GetAsync(ItemData item, CancellationToken cancellationToken = default)
        {
            if(_items.Contains(item))
            {
                return item;
            }
            throw new Exception();
        }
        public async Task<IEnumerable<ItemData>> GetsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _items.Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}
