using System.Collections.ObjectModel;

namespace R54IN0.WPF
{
    public interface ICollectionViewModel<T> where T : class
    {
        ObservableCollection<T> Items { get; set; }
        T SelectedItem { get; set; }
    }
}