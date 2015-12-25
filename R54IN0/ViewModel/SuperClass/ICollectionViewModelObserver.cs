using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace R54IN0
{
    public interface ICollectionViewModelObserver
    {
        void UpdateNewItem(object item);
        void UpdateDelItem(object item);
    }
}