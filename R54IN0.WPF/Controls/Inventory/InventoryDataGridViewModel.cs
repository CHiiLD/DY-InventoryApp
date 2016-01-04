using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InventoryDataGridViewModel : ICollectionViewModel<ObservableInventory>, INotifyPropertyChanged
    {
        private ObservableCollection<ObservableInventory> _items;
        private ObservableInventory _selectedItem;

        /// <summary>
        /// 생성자
        /// </summary>
        public InventoryDataGridViewModel()
        {
            _items = new ObservableCollection<ObservableInventory>();
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

        public ObservableCollection<ObservableInventory> Items
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

        public ObservableInventory SelectedItem
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
