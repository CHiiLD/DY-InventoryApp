namespace R54IN0
{
    public interface ICollectionViewModelObserver
    {
        void UpdateNewItem(object item);

        void UpdateDelItem(object item);
    }
}