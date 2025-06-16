using System.Collections.Generic;

namespace TqkLibrary.WpfUi.Interfaces
{
    public interface IMainThreadList<T> : IMainThread, IList<T>, IMainThreadCollection<T>
    {

    }
}
