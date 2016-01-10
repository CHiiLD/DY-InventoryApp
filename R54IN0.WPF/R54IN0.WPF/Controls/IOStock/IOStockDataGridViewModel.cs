using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0.WPF
{
    public class IOStockDataGridViewModel : ICollectionViewModel<IOStockDataGridItem>, INotifyPropertyChanged
    {
        public IOStockDataGridViewModel()
        {
            Items = new ObservableCollection<IOStockDataGridItem>();
        }

        private event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        private ObservableCollection<IOStockDataGridItem> _items;
        private IOStockDataGridItem _selectedItem;

        public ObservableCollection<IOStockDataGridItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public IOStockDataGridItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}