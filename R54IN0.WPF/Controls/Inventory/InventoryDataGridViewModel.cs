using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace R54IN0.WPF
{
    public class InventoryDataGridViewModel : ICollectionViewModel<ObservableInventory>, INotifyPropertyChanged
    {
        private ObservableCollection<ObservableInventory> _items;
        private ObservableInventory _selectedItem;
        private bool _isReadOnly;
        private Visibility _productVisibility;
        private Visibility _makerVisibility;
        private Visibility _measureVisibility;

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

        /// <summary>
        /// 제품열 보기/숨기기
        /// </summary>
        public Visibility ProductVisibility
        {
            get
            {
                return _productVisibility;
            }
            set
            {
                _productVisibility = value;
                NotifyPropertyChanged("ProductVisibility");
            }
        }

        /// <summary>
        /// 제조사열 보기/숨기기
        /// </summary>
        public Visibility MakerVisibility
        {
            get
            {
                return _makerVisibility;
            }
            set
            {
                _makerVisibility = value;
                NotifyPropertyChanged("MakerVisibility");
            }
        }

        /// <summary>
        /// 단위열 보기/숨기기
        /// </summary>
        public Visibility MeasureVisibility
        {
            get
            {
                return _measureVisibility;
            }
            set
            {
                _measureVisibility = value;
                NotifyPropertyChanged("MeasureVisibility");
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public InventoryDataGridViewModel()
        {
            _items = new ObservableCollection<ObservableInventory>();
        }
        /// <summary>
        /// 수량을 제외한 모든열에 대한 IsReadOnly 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged("IsReadOnly");
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