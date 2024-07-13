using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WpfUi.Interfaces
{
    public interface IRemoteDataBatch<T> : IRemoteData<T>
    {
        Task<IEnumerable<T>> AddsAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> DeletesAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    }
}