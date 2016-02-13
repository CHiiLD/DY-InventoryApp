using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class IOStockDataGridViewModel : ICollectionViewModel<IOStockDataGridItem>, INotifyPropertyChanged, ICollectionViewModelObserver
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

            SearchAsIOStockRecordCommand = new RelayCommand(ExecuteSearchAsIOStockRecordCommand, IsSelected);
            NewIOStockFormatAdditionCommand = new RelayCommand(ExecuteNewIOStockFormatAdditionCommand);
            IOStockFormatModificationCommand = new RelayCommand(ExecuteIOStockFormatModificationCommand, IsSelected);
            IOStockFormatDeletionCommand = new RelayCommand(ExecuteIOStockFormatDeletionCommand, IsSelected);
            ChekcedIOStockFormatsDeletionCommand = new RelayCommand(ExecuteChekcedIOStockFormatsDeletionCommand);
            CheckedIOStockFormatsCopyCommand = new RelayCommand(ExecuteCheckedIOStockFormatsCopyCommand);
            SearchAsInventoryRecordCommand = new RelayCommand(ExecuteSearchAsInventoryRecordCommand, IsSelected);
            ContextMenuOpeningEventCommand = new RelayCommand(ExecuteContextMenuOpeningEventCommand);
            BeginningEditEventCommand = new RelayCommand<DataGridBeginningEditEventArgs>(CancelCellEditEvent, CanCancelCellEdit);

            var makers = InventoryDataCommander.GetInstance().CopyFields<Maker>();
            Makers = new ObservableCollection<Observable<Maker>>(makers);
            var measures = InventoryDataCommander.GetInstance().CopyFields<Measure>();
            Measures = new ObservableCollection<Observable<Measure>>(measures);
            var warehouses = InventoryDataCommander.GetInstance().CopyFields<Warehouse>();
            Warehouses = new ObservableCollection<Observable<Warehouse>>(warehouses);

            var projects = InventoryDataCommander.GetInstance().CopyFields<Project>();
            Projects = new ObservableCollection<Observable<Project>>(projects);
            var employees = InventoryDataCommander.GetInstance().CopyFields<Employee>();
            Employees = new ObservableCollection<Observable<Employee>>(employees);
            var suppliers = InventoryDataCommander.GetInstance().CopyFields<Supplier>();
            Suppliers = new ObservableCollection<Observable<Supplier>>(suppliers);
            var customers = InventoryDataCommander.GetInstance().CopyFields<Customer>();
            Customers = new ObservableCollection<Observable<Customer>>(customers);

            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        ~IOStockDataGridViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
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

        /// <summary>
        /// 유저가 데이터그리드의 셀을 수정하고자 할 경우
        /// </summary>
        public RelayCommand<DataGridBeginningEditEventArgs> BeginningEditEventCommand { get; private set; }

        /// <summary>
        /// 데이터그리드 셀에 컨덱스트 메뉴를 오픈할 경우
        /// </summary>
        public RelayCommand ContextMenuOpeningEventCommand { get; set; }

        #region DataGridComboBoxColumn ItemSources

        public ObservableCollection<Observable<Warehouse>> Warehouses { get; private set; }
        public ObservableCollection<Observable<Maker>> Makers { get; private set; }
        public ObservableCollection<Observable<Measure>> Measures { get; private set; }
        public ObservableCollection<Observable<Project>> Projects { get; private set; }
        public ObservableCollection<Observable<Supplier>> Suppliers { get; private set; }
        public ObservableCollection<Observable<Customer>> Customers { get; private set; }
        public ObservableCollection<Observable<Employee>> Employees { get; private set; }

        #endregion DataGridComboBoxColumn ItemSources

        /// <summary>
        /// 데이터그리드 셀의 수정 이벤트를 취소할 지 질의하기
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanCancelCellEdit(DataGridBeginningEditEventArgs arg)
        {
            string path = arg.Column.SortMemberPath;
            IOStockDataGridItem item = arg.Row.Item as IOStockDataGridItem;
            if (item.StockType == IOStockType.OUTGOING && (path.Contains("Warehouse") || path.Contains("Supplier")))
                return true;
            else if (item.StockType == IOStockType.INCOMING && (path.Contains("Project") || path.Contains("Customer")))
                return true;
            return false;
        }

        /// <summary>
        /// 데이터그리드 셀의 수정 이벤트를 캔슬하기
        /// </summary>
        /// <param name="obj"></param>
        private void CancelCellEditEvent(DataGridBeginningEditEventArgs obj)
        {
            obj.Cancel = true;
        }

        /// <summary>
        /// 메뉴컨텍스트를 열었을 때 메뉴아이템 커맨드 실행상태 업데이트하기
        /// </summary>
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

        public void UpdateNewItem(object item)
        {
            if (item.GetType() == typeof(Observable<Maker>))
                Makers.Add(item as Observable<Maker>);
            else if (item.GetType() == typeof(Observable<Measure>))
                Measures.Add(item as Observable<Measure>);
            else if (item.GetType() == typeof(Observable<Warehouse>))
                Warehouses.Add(item as Observable<Warehouse>);
            else if (item.GetType() == typeof(Observable<Project>))
                Projects.Add(item as Observable<Project>);
            else if (item.GetType() == typeof(Observable<Supplier>))
                Suppliers.Add(item as Observable<Supplier>);
            else if (item.GetType() == typeof(Observable<Customer>))
                Customers.Add(item as Observable<Customer>);
            else if (item.GetType() == typeof(Observable<Employee>))
                Employees.Add(item as Observable<Employee>);
        }

        public void UpdateDelItem(object item)
        {
            if (item.GetType() == typeof(Observable<Maker>))
                Makers.Remove(item as Observable<Maker>);
            else if (item.GetType() == typeof(Observable<Measure>))
                Measures.Remove(item as Observable<Measure>);
            else if (item.GetType() == typeof(Observable<Warehouse>))
                Warehouses.Remove(item as Observable<Warehouse>);
            else if (item.GetType() == typeof(Observable<Project>))
                Projects.Remove(item as Observable<Project>);
            else if (item.GetType() == typeof(Observable<Supplier>))
                Suppliers.Remove(item as Observable<Supplier>);
            else if (item.GetType() == typeof(Observable<Customer>))
                Customers.Remove(item as Observable<Customer>);
            else if (item.GetType() == typeof(Observable<Employee>))
                Employees.Remove(item as Observable<Employee>);
        }
    }
}