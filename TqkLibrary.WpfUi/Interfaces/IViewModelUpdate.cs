namespace TqkLibrary.WpfUi.Interfaces
{
    public interface IViewModelUpdate<T> : IViewModel<T>
    {
        void Update(T data);
    }
}