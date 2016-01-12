using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private Visibility _specificationMemoColumnVisibility;
        private Visibility _makerColumnVisibility;
        private Visibility _secondStockTypeColumnVisibility;
        private Visibility _remainQtyColumnVisibility;
        private bool _isReadOnly;

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

        public Visibility SpecificationMemoColumnVisibility
        {
            get
            {
                return _specificationMemoColumnVisibility;
            }
            set
            {
                _specificationMemoColumnVisibility = value;
                NotifyPropertyChanged("SpecificationMemoColumnVisibility");
            }
        }

        public Visibility MakerColumnVisibility
        {
            get
            {
                return _makerColumnVisibility;
            }
            set
            {
                _makerColumnVisibility = value;
                NotifyPropertyChanged("MakerColumnVisibility");
            }
        }

        public Visibility SecondStockTypeColumnVisibility
        {
            get
            {
                return _secondStockTypeColumnVisibility;
            }
            set
            {
                _secondStockTypeColumnVisibility = value;
                NotifyPropertyChanged("SecondStockTypeColumnVisibility");
            }
        }

        public Visibility RemainQtyColumnVisibility
        {
            get
            {
                return _remainQtyColumnVisibility;
            }
            set
            {
                _remainQtyColumnVisibility = value;
                NotifyPropertyChanged("RemainQtyColumnVisibility");
            }
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

        public void OnPreviewTextInputted(object sender, TextCompositionEventArgs e)
        {
            var datagrid = sender as DataGrid;
            if (datagrid != null)
            {
                IOStockDataGridItem item = datagrid.CurrentItem as IOStockDataGridItem;
                DataGridColumn column = datagrid.CurrentColumn;
                if (column.SortMemberPath.Contains("Name"))
                {
                    string propertyPath = column.SortMemberPath.Replace(".Name", "");
                    string[] paths = propertyPath.Split('.');
                    object property = item;
                    foreach (var path in paths)
                    {
                        property = property.GetType().GetProperty(path).GetValue(property, null);
                    }
                    if (property == null)
                        e.Handled = true;
                }
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}