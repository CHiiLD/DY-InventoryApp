using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : ObservableIOStock
    {
        private IOStockStatusViewModel _ioStockStatusViewModel;
        private IObservableIOStockProperties _originSource;
        private Mode _mode;
        private bool _isOpenFlyout;
        private bool _isReadOnlyProductTextBox;
        private IEnumerable<IObservableInventoryProperties> _inventoryList;
        private IEnumerable<IObservableField> _clientList;
        private Observable<Product> _product;
        private string _specificationMemo;
        private Observable<Measure> _measure;
        private Observable<Maker> _maker;
        private bool _isEnabledWarehouseComboBox;
        private bool _isEnabledProjectComboBox;
        private string _productText;
        private string _specificationText;
        private string _makerText;
        private string _measureText;
        private string _clientText;
        private string _warehouseText;
        private string _projectText;
        private string _employeeText;
        private bool _isEditableSpecification;
        private bool _isEnabledSpecificationComboBox;
        private bool _isEnabledInComingRadioButton;
        private bool _isEnabledInOutGoingRadioButton;
        private int _inventoryQuantity;
        private bool _isEnabledDatePicker;
        private string _accountTypeText;

        public const string TITLE_TEXT_ADD = "새로운 입출고 기록 추가하기";
        public const string TITLE_TEXT_MODIFY = "입출고 기록 수정하기";

        /// <summary>
        /// TEST용 생성자
        /// </summary> 
        public IOStockDataAmenderViewModel() : base()
        {
            Initialize();
            Quantity = 1;
            _mode = Mode.ADD;
            StockType = IOStockType.INCOMING;
            Date = DateTime.Now;

            IsEnabledSpecificationComboBox = true;
            IsEnabledOutGoingRadioButton = true;
            IsEnabledInComingRadioButton = true;
            IsEnabledDatePicker = true;
            TitleText = TITLE_TEXT_ADD;
            LoadLastRecordVisibility = Visibility.Visible;
        }

        /// <summary>
        /// TEST용 생성자
        /// </summary>
        /// <param name="ioStock"></param>
        public IOStockDataAmenderViewModel(IObservableIOStockProperties ioStock) : base(
            new IOStockFormat(ioStock.Format))
        {
            Initialize();
            _mode = Mode.MODIFY;
            _originSource = ioStock;
            StockType = ioStock.StockType;

            IsReadOnlyProductTextBox = true;
            IsEnabledSpecificationComboBox = false;
            IsEnabledOutGoingRadioButton = false;
            IsEnabledInComingRadioButton = false;
            IsEnabledDatePicker = false;
            TitleText = TITLE_TEXT_MODIFY;
            LoadLastRecordVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// 새로운 IOStockFormat을 추가하고자 할 경우
        /// </summary>
        /// <param name="ioStockStatusViewModel"></param>
        public IOStockDataAmenderViewModel(IOStockStatusViewModel ioStockStatusViewModel) : this()
        {
            _ioStockStatusViewModel = ioStockStatusViewModel;
        }

        /// <summary>
        /// 기존의 IOStockFormat을 수정하고자 할 경우
        /// </summary>
        /// <param name="ioStockStatusViewModel"></param>
        /// <param name="ioStock"></param>
        public IOStockDataAmenderViewModel(IOStockStatusViewModel ioStockStatusViewModel, IObservableIOStockProperties ioStock) :
            this(ioStock)
        {
            _ioStockStatusViewModel = ioStockStatusViewModel;
        }

        /// <summary>
        /// IOStockProperties 속성 초기화
        /// </summary>
        /// <param name="iosFmt"></param>
        protected override void InitializeProperties(IOStockFormat iosFmt)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            customer = ofd.SearchObservableField<Customer>(iosFmt.CustomerID);
            supplier = ofd.SearchObservableField<Supplier>(iosFmt.SupplierID);
            project = ofd.SearchObservableField<Project>(iosFmt.ProjectID);
            employee = ofd.SearchObservableField<Employee>(iosFmt.EmployeeID);
            warehouse = ofd.SearchObservableField<Warehouse>(iosFmt.WarehouseID);

            Product = oid.SearchObservableInventory(iosFmt.InventoryID).Product;
            Inventory = InventoryList.Where(x => x.ID == iosFmt.InventoryID).SingleOrDefault();
            CalcInventoryQuantityProperty(Quantity);
        }

        /// <summary>
        /// 명령어와 뷰모델 초기화
        /// </summary>
        private void Initialize()
        {
            TreeViewViewModel = new MultiSelectTreeViewModelView();
            TreeViewViewModel.PropertyChanged += OnTreeViewModelPropertyChanged;
            TreeViewViewModel.ContextMenuVisibility = Visibility.Collapsed;

            ProductSearchCommand = new RelayCommand(ExecuteProductSearchCommand, CanSearch);
            RecordCommand = new RelayCommand(ExecuteRecordCommand, CanRecord);
            ProductSelectCommand = new RelayCommand(ExecuteProductSelectCommand, CanSelectProduct);
            ProjectComboBoxGotFocusEventCommand = new RelayCommand<RoutedEventArgs>(ExecuteProjectComboBoxGotFocusEventCommand);
            LoadLastRecordCommand = new RelayCommand(ExecuteLoadLastRecordCommand, CanLoadLastRecord);
            ComboBoxItemDeleteCommand = new RelayCommand<object>(ExecuteComboBoxItemDeleteCommand);
        }

        private async void ExecuteComboBoxItemDeleteCommand(object obj)
        {
            IObservableField observableField = obj as IObservableField;
            var field = observableField.Field;
            Type type = field.GetType();

            CollectionViewModelObserverSubject.GetInstance().NotifyItemDeleted(observableField);
            if (type == typeof(Customer))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Customer);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Customer>(observableField);
            }
            else if (type == typeof(Supplier))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Supplier);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Supplier>(observableField);
            }
            else if (type == typeof(Maker))
            {
                await DbAdapter.GetInstance().DeleteAsync<Maker>(field.ID);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Maker>(observableField);
                ObservableInventoryDirector.GetInstance().CopyObservableInventories().ForEach(x => 
                {
                    if (x.Maker.ID == field.ID)
                        x.Maker = null;
                });
            }
            else if (type == typeof(Measure))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Measure);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Measure>(observableField);
                ObservableInventoryDirector.GetInstance().CopyObservableInventories().ForEach(x =>
                {
                    if (x.Measure.ID == field.ID)
                        x.Measure = null;
                });
            }
            else if (type == typeof(Project))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Project);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Project>(observableField);
            }
            else if (type == typeof(Warehouse))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Warehouse);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Warehouse>(observableField);
            }
            else if (type == typeof(Employee))
            {
                await DbAdapter.GetInstance().DeleteAsync(field as Employee);
                ObservableFieldDirector.GetInstance().RemoveObservableField<Employee>(observableField);
            }
            if(_ioStockStatusViewModel != null && _ioStockStatusViewModel.BackupSource != null)
                _ioStockStatusViewModel.BackupSource.ForEach(async x => await x.SyncDataFromServer());
        }

        private async void ExecuteLoadLastRecordCommand()
        {
            if (Inventory == null)
                return;
            var query = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(
                DbCommand.WHERE, "InventoryID", Inventory.ID,
                DbCommand.WHERE, "StockType", StockType,
                DbCommand.DESCENDING, "Date",
                DbCommand.LIMIT, 1);
            if (query.Count() != 1)
                return;
            var item = query.Single();
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
            Employee = ObservableFieldDirector.GetInstance().SearchObservableField<Employee>(item.EmployeeID);
            if (StockType == IOStockType.INCOMING)
            {
                Client = ObservableFieldDirector.GetInstance().SearchObservableField<Supplier>(item.SupplierID);
                Warehouse = ObservableFieldDirector.GetInstance().SearchObservableField<Warehouse>(item.WarehouseID);
            }
            else if (StockType == IOStockType.OUTGOING)
            {
                Client = ObservableFieldDirector.GetInstance().SearchObservableField<Customer>(item.CustomerID);
                Project = ObservableFieldDirector.GetInstance().SearchObservableField<Project>(item.ProjectID);
            }
        }

        private void ExecuteProjectComboBoxGotFocusEventCommand(RoutedEventArgs e)
        {
            if(e.OriginalSource is TextBox)
            {
                TextBox tb = e.OriginalSource as TextBox;
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = PROJECT_PREPIX;
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }

        private void CalcInventoryQuantityProperty(int iosQty)
        {
            switch (_mode)
            {
                case Mode.ADD:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            InventoryQuantity = (Inventory == null) ? iosQty : Inventory.Quantity + iosQty; break;
                        case IOStockType.OUTGOING:
                            InventoryQuantity = (Inventory == null) ? -iosQty : Inventory.Quantity - iosQty; break;
                    }
                    break;
                case Mode.MODIFY:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            InventoryQuantity = _originSource.Inventory.Quantity + iosQty - _originSource.Quantity; break;
                        case IOStockType.OUTGOING:
                            InventoryQuantity = _originSource.Inventory.Quantity + _originSource.Quantity - iosQty; break;
                    }
                    break;
            }
        }

        private void OnTreeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
                ProductSelectCommand.RaiseCanExecuteChanged();
        }

        private bool CanSelectProduct()
        {
            return TreeViewViewModel.SelectedNodes.Count == 1 && TreeViewViewModel.SelectedNodes.Single().Type == NodeType.PRODUCT;
        }

        private void ExecuteProductSelectCommand()
        {
            var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(n => n.Type == NodeType.PRODUCT));
            var node = nodes.FirstOrDefault();
            if (node != null)
            {
                var ofd = ObservableFieldDirector.GetInstance();
                var product = ofd.SearchObservableField<Product>(node.ProductID);
                if (product != null)
                    Product = product;
            }
            IsOpenFlyout = false;
        }

        private bool CanRecord()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                return false;
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                return false;
            return true;
        }

        private bool CanLoadLastRecord()
        {
            return Product != null && Inventory != null;
        }

        private async void ExecuteRecordCommand()
        {
            await RecordAsync();
            var window = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            if (window != null)
                window.Close();
        }

        /// <summary>
        /// 제품 텍스트박스의 검색 버튼의 활성화 여부
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanSearch()
        {
            bool can = false;
            switch (_mode)
            {
                case Mode.ADD:
                    can = true;
                    break;

                case Mode.MODIFY:
                    can = false;
                    break;
            }
            return can;
        }

        /// <summary>
        /// 제품 탐색기 열기 명령
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteProductSearchCommand()
        {
            IsOpenFlyout = true;
        }

        public override void NotifyPropertyChanged(string propertyName)
        {
            var eventHandler = propertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "Quantity" || propertyName == "UnitPrice")
                NotifyPropertyChanged("Amount");
        }
    }
}