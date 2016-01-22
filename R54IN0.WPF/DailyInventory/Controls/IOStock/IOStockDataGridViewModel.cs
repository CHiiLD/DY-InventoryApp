using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public partial class IOStockDataGridViewModel : ICollectionViewModel<IOStockDataGridItem>, INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler _propertyChanged;

        private ObservableCollection<IOStockDataGridItem> _items;
        private IOStockDataGridItem _selectedItem;
        private Visibility _specificationMemoColumnVisibility;
        private Visibility _makerColumnVisibility;
        private Visibility _secondStockTypeColumnVisibility;
        private Visibility _remainQtyColumnVisibility;
        private bool _isReadOnly;

        public IOStockDataGridViewModel()
        {
            Items = new ObservableCollection<IOStockDataGridItem>();
            PreviewTextInputEventCommand = new RelayCommand<TextCompositionEventArgs>(ExecutePreviewTextInputEventCommand);
            CellEditEndingEventCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(ExecuteCellEditEndingEventCommand);

            SearchAsIOStockRecordCommand = new RelayCommand(ExecuteSearchAsIOStockRecordCommand, IsSelected);
            NewIOStockFormatAdditionCommand = new RelayCommand(ExecuteNewIOStockFormatAdditionCommand);
            IOStockFormatModificationCommand = new RelayCommand(ExecuteIOStockFormatModificationCommand, IsSelected);
            IOStockFormatDeletionCommand = new RelayCommand(ExecuteIOStockFormatDeletionCommand, IsSelected);
            ChekcedIOStockFormatsDeletionCommand = new RelayCommand(ExecuteChekcedIOStockFormatsDeletionCommand);
            CheckedIOStockFormatsCopyCommand = new RelayCommand(ExecuteCheckedIOStockFormatsCopyCommand);
            SearchAsInventoryRecordCommand = new RelayCommand(ExecuteSearchAsInventoryRecordCommand, IsSelected);
            ContextMenuOpeningEventCommand = new RelayCommand(ExecuteContextMenuOpeningEventCommand);
        }
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

        public RelayCommand<TextCompositionEventArgs> PreviewTextInputEventCommand { get; set; }

        public RelayCommand<DataGridCellEditEndingEventArgs> CellEditEndingEventCommand { get; set; }

        public RelayCommand ContextMenuOpeningEventCommand { get; set; }

        private void ExecutePreviewTextInputEventCommand(TextCompositionEventArgs e)
        {
            var datagrid = e.Source as DataGrid;
            if (datagrid != null)
            {
                IOStockDataGridItem item = datagrid.CurrentItem as IOStockDataGridItem;
                DataGridColumn column = datagrid.CurrentColumn;
                string sortMemberPath = column.SortMemberPath;
                IOStockType iosType = item.StockType;

                if (iosType == IOStockType.INCOMING && (sortMemberPath.Contains("Customer") || sortMemberPath.Contains("Project")) ||
                iosType == IOStockType.OUTGOING && (sortMemberPath.Contains("Supplier") || sortMemberPath.Contains("Warehouse")))
                    e.Handled = true;
            }
        }

        private void ExecuteCellEditEndingEventCommand(DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn column = e.Column;
            DataGridRow row = e.Row;
            TextBox textBox = e.EditingElement as TextBox;
            string sortMemberPath = column.SortMemberPath;
            string text = textBox.Text;
            IOStockDataGridItem item = row.Item as IOStockDataGridItem;
            IOStockType iosType = item.StockType;

            if (!sortMemberPath.Contains("Name") || item == null || textBox == null)
                return;
            if (iosType == IOStockType.INCOMING && (sortMemberPath.Contains("Customer") || sortMemberPath.Contains("Project")) ||
                iosType == IOStockType.OUTGOING && (sortMemberPath.Contains("Supplier") || sortMemberPath.Contains("Warehouse")))
                return;
            
            string[] paths = column.SortMemberPath.Replace(".Name", "").Split('.');
            object property = item;
            foreach (var path in paths)
                property = property.GetType().GetProperty(path).GetValue(property, null);
            if (property == null)
            {
                string propertyName = paths.Last();
                var ofd = ObservableFieldDirector.GetInstance();
                switch (propertyName)
                {
                    case "Maker":
                        item.Inventory.Maker = new Observable<Maker>(text);
                        ofd.AddObservableField<Maker>(item.Inventory.Maker);
                        break;
                    case "Measure":
                        item.Inventory.Measure = new Observable<Measure>(text);
                        ofd.AddObservableField<Measure>(item.Inventory.Measure);
                        break;
                    case "Warehouse":
                        item.Warehouse = new Observable<Warehouse>(text);
                        ofd.AddObservableField<Warehouse>(item.Warehouse);
                        break;
                    case "Project":
                        item.Project = new Observable<Project>(text);
                        ofd.AddObservableField<Project>(item.Project);
                        break;
                    case "Customer":
                        item.Customer = new Observable<Customer>(text);
                        ofd.AddObservableField<Customer>(item.Customer);
                        break;
                    case "Supplier":
                        item.Supplier = new Observable<Supplier>(text);
                        ofd.AddObservableField<Supplier>(item.Supplier);
                        break;
                    case "Employee":
                        item.Employee = new Observable<Employee>(text);
                        ofd.AddObservableField<Employee>(item.Employee);
                        break;
                }
            }
        }

        private void ExecuteContextMenuOpeningEventCommand()
        {
            SearchAsIOStockRecordCommand.RaiseCanExecuteChanged();
            IOStockFormatModificationCommand.RaiseCanExecuteChanged();
            IOStockFormatDeletionCommand.RaiseCanExecuteChanged();
            SearchAsInventoryRecordCommand.RaiseCanExecuteChanged();
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}