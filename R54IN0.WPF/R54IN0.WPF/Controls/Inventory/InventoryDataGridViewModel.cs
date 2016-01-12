using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InventoryDataGridViewModel : ICollectionViewModel<ObservableInventory>, INotifyPropertyChanged
    {
        private ObservableCollection<ObservableInventory> _items;
        private ObservableInventory _selectedItem;
        private bool _isReadOnly;
        private Visibility _productColumnVisibility;
        private Visibility _makerColumnVisibility;
        private Visibility _measureColumnVisibility;

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
        /// 생성자
        /// </summary>
        public InventoryDataGridViewModel()
        {
            _items = new ObservableCollection<ObservableInventory>();
            InquiryAsIOStockDataCommand = new CommandHandler(ExecuteInquiryAsIOStockDataCommand, IsSelectedItem);
            NewIOStockDataAddCommand = new CommandHandler(ExecuteNewIOStockDataAddCommand, IsSelectedItem);
            InventoryDataDeleteCommand = new CommandHandler(ExecuteInventoryDataDeleteCommand, IsSelectedItem);
        }

        private void ExecuteInventoryDataDeleteCommand(object obj)
        {

        }

        private void ExecuteNewIOStockDataAddCommand(object obj)
        {

        }

        private void ExecuteInquiryAsIOStockDataCommand(object obj)
        {

        }

        private bool IsSelectedItem(object arg)
        {
            return SelectedItem != null;
        }

        public ICommand InquiryAsIOStockDataCommand { get; set;}

        public ICommand NewIOStockDataAddCommand { get; set; }

        public ICommand InventoryDataDeleteCommand { get; set; }

        /// <summary>
        /// 제품열 보기/숨기기
        /// </summary>
        public Visibility ProductColumnVisibility
        {
            get
            {
                return _productColumnVisibility;
            }
            set
            {
                _productColumnVisibility = value;
                NotifyPropertyChanged("ProductColumnVisibility");
            }
        }

        /// <summary>
        /// 제조사열 보기/숨기기
        /// </summary>
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

        /// <summary>
        /// 단위열 보기/숨기기
        /// </summary>
        public Visibility MeasureColumnVisibility
        {
            get
            {
                return _measureColumnVisibility;
            }
            set
            {
                _measureColumnVisibility = value;
                NotifyPropertyChanged("MeasureColumnVisibility");
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