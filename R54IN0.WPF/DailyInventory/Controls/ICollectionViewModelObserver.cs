namespace R54IN0.WPF
{
    public interface ICollectionViewModelObserver
    {
        void UpdateNewItem(object item);

        void UpdateDelItem(object item);
    }
}