#if NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WpfUi.ObservableCollections.AsyncCollections
{
    /// <summary>
    /// Database/Api, or custom to json file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncData<T>
    {
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default);
        Task<bool> AddAsync(T item, CancellationToken cancellationToken = default);
        Task AddsAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default);
        Task DeletesAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
        Task<bool> ClearAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
    public interface IAsyncData<T, TId> : IAsyncData<T>
    {
        Task<T> GetAsync(TId id, CancellationToken cancellationToken = default);
    }
}
#endif
