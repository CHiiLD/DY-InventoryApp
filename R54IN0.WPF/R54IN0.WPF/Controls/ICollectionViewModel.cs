using System.Collections.ObjectModel;

namespace R54IN0
{
    public interface ICollectionViewModel<T> where T : class
    {
        ObservableCollection<T> Items { get; set; }
        T SelectedItem { get; set; }
    }
}