using System.Threading;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.Interfaces
{
    public interface IMainThread
    {
        SynchronizationContext SynchronizationContext { get; }
        Dispatcher Dispatcher { get; }
    }
}
