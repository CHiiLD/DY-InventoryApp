using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InoutStockDataGridVIewModel : ICollectionViewModel<InoutStockDataGridItem>, INotifyPropertyChanged
    {
        public InoutStockDataGridVIewModel()
        {
            Items = new ObservableCollection<InoutStockDataGridItem>();
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

        private ObservableCollection<InoutStockDataGridItem> _items;
        private InoutStockDataGridItem _selectedItem;

        public ObservableCollection<InoutStockDataGridItem> Items
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

        public InoutStockDataGridItem SelectedItem
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
