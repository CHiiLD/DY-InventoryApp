using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface ICollectionViewModel<T> where T : class
    {
        ObservableCollection<T> Items { get; set; }
        T SelectedItem { get; set; }
    }
}