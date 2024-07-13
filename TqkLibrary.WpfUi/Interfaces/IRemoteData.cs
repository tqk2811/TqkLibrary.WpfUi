using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WpfUi.Interfaces
{
    /// <summary>
    /// Database/Api, or custom to json file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRemoteData<T> : IRemoteDataReadonly<T>
    {
        Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default);
        /// <summary>
        /// return data after add (with id)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> AddAsync(T item, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default);
        Task<bool> ClearAsync(CancellationToken cancellationToken = default);
    }
}
