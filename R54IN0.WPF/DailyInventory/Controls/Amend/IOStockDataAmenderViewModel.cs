using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : ObservableIOStock, ICollectionViewModelObserver
    {
        private IOStockStatusViewModel _ioStockStatusViewModel;
        private IObservableIOStockProperties _originSource;
        private Mode _mode;
        private bool _isOpenFlyout;
        private bool _isReadOnlyProductTextBox;
        private IEnumerable<IObservableInventoryProperties> _inventoryList;

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

        private ObservableCollection<IObservableField> _clientList;
        private ObservableCollection<Observable<Maker>> _makerList;
        private ObservableCollection<Observable<Project>> _projectList;
        private ObservableCollection<Observable<Employee>> _employeeList;
        private ObservableCollection<Observable<Warehouse>> _warehouseList;
        private ObservableCollection<Observable<Measure>> _measureList;

        public const string TITLE_TEXT_ADD = "새로운 입출고 기록 추가하기";
        public const string TITLE_TEXT_MODIFY = "입출고 기록 수정하기";
        public const string SUPPLIER = "구입처";
        public const string CUSTOMER = "출고처";
        public const string PROJECT_PREPIX = "DY";

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

            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        /// <summary>
        /// TEST용 생성자
        /// </summary>
        /// <param name="ioStock"></param>
        public IOStockDataAmenderViewModel(IObservableIOStockProperties ioStock) : base(new IOStockFormat(ioStock.Format))
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

            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

#if false
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
#endif

        ~IOStockDataAmenderViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        /// <summary>
        /// 제품 탐색기 뷰모델
        /// </summary>
        public MultiSelectTreeViewModelView TreeViewViewModel { get; set; }

#region Command

        /// <summary>
        /// 제품 탐색기 열기 버튼의 명령어
        /// </summary>
        public RelayCommand ProductSearchCommand { get; set; }

        /// <summary>
        /// 저장 버튼 명령어
        /// </summary>
        public RelayCommand RecordCommand { get; set; }

        /// <summary>
        /// 제품 탐색기에서 제품을 선택한 뒤, 확인 버튼의 Command 객체
        /// </summary>
        public RelayCommand ProductSelectCommand { get; set; }

        /// <summary>
        /// 최근 저장된 데이터를 불러온다.
        /// </summary>
        public RelayCommand LoadLastRecordCommand { get; set; }

        public RelayCommand<RoutedEventArgs> ProjectComboBoxGotFocusEventCommand { get; set; }

        public RelayCommand WindowCloseCommand { get; set; }

        public RelayCommand<KeyEventArgs> ComboBoxKeyUpEventCommand { get; set; }

        /// <summary>
        /// 콤보박스의 아이템들을 삭제
        /// </summary>
        public RelayCommand<object> ComboBoxItemDeleteCommand { get; set; }

#endregion Command

        public bool IsEditableSpecification
        {
            get
            {
                return _isEditableSpecification;
            }
            set
            {
                _isEditableSpecification = value;
                NotifyPropertyChanged("IsEditableSpecification");
            }
        }

        /// <summary>
        /// 제품 탐색기 플라이아웃의 가시 여부 바인딩 프로퍼티
        /// </summary>
        public bool IsOpenFlyout
        {
            get
            {
                return _isOpenFlyout;
            }
            set
            {
                _isOpenFlyout = value;
                NotifyPropertyChanged("IsOpenFlyout");
            }
        }

        /// <summary>
        /// 단가의 합계 (입출된 개수 * 가격)
        /// </summary>
        public decimal Amount
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        /// <summary>
        /// 재고 수량
        /// Inventory.Quantity는 입출고 수량에 의한 변동된 재고수량을 계산하기 위해 가만히 둔다.
        /// </summary>
        public int InventoryQuantity
        {
            get
            {
                return _inventoryQuantity;
            }
            private set
            {
                _inventoryQuantity = value;
                NotifyPropertyChanged("InventoryQuantity");
            }
        }

        public string AccountTypeText
        {
            get
            {
                return _accountTypeText;
            }
            set
            {
                _accountTypeText = value;
                NotifyPropertyChanged("AccountTypeText");
            }
        }

        public string TitleText
        {
            get; set;
        }

        public Visibility LoadLastRecordVisibility
        {
            get;
            set;
        }

        public override IOStockType StockType
        {
            get
            {
                return base.StockType;
            }
            set
            {
                base.StockType = value;
                var ofd = DataDirector.GetInstance();

                switch (value)
                {
                    case IOStockType.INCOMING:
                        Project = null;
                        ProjectText = null;
                        ClientList = new ObservableCollection<IObservableField>(ofd.CopyFields<Supplier>());
                        IsEditableSpecification = true;
                        IsReadOnlyProductTextBox = false;
                        IsEnabledWarehouseComboBox = true;
                        IsEnabledProjectComboBox = false;
                        AccountTypeText = SUPPLIER;
                        break;

                    case IOStockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;
                        ClientList = new ObservableCollection<IObservableField>(ofd.CopyFields<Customer>());
                        IsEditableSpecification = false;
                        IsReadOnlyProductTextBox = true;
                        IsEnabledWarehouseComboBox = false;
                        IsEnabledProjectComboBox = true;
                        AccountTypeText = CUSTOMER;

                        if (Product == null)
                            ProductText = null;
                        if (Inventory == null)
                        {
                            SpecificationText = null;
                            SpecificationMemo = null;
                            MakerText = null;
                            MeasureText = null;
                            Maker = null;
                            Measure = null;
                        }
                        break;
                }
                CalcInventoryQuantityProperty(Quantity);
            }
        }

        public override int Quantity
        {
            get
            {
                return base.Quantity;
            }
            set
            {
                base.Quantity = value;
                CalcInventoryQuantityProperty(value);
            }
        }

#region IsEnabled Property

        public bool IsEnabledDatePicker
        {
            get
            {
                return _isEnabledDatePicker;
            }
            set
            {
                _isEnabledDatePicker = value;
                NotifyPropertyChanged("IsEnabledDatePicker");
            }
        }

        public bool IsEnabledWarehouseComboBox
        {
            get
            {
                return _isEnabledWarehouseComboBox;
            }
            set
            {
                _isEnabledWarehouseComboBox = value;
                NotifyPropertyChanged("IsEnabledWarehouseComboBox");
            }
        }

        public bool IsEnabledProjectComboBox
        {
            get
            {
                return _isEnabledProjectComboBox;
            }
            set
            {
                _isEnabledProjectComboBox = value;
                NotifyPropertyChanged("IsEnabledProjectComboBox");
            }
        }

        public bool IsEnabledInComingRadioButton
        {
            get
            {
                return _isEnabledInComingRadioButton;
            }
            set
            {
                _isEnabledInComingRadioButton = value;
                NotifyPropertyChanged("IsEnabledInComingRadioButton");
            }
        }

        public bool IsEnabledOutGoingRadioButton
        {
            get
            {
                return _isEnabledInOutGoingRadioButton;
            }
            set
            {
                _isEnabledInOutGoingRadioButton = value;
                NotifyPropertyChanged("IsEnabledOutGoingRadioButton");
            }
        }

        public bool IsEnabledSpecificationComboBox
        {
            get
            {
                return _isEnabledSpecificationComboBox;
            }
            set
            {
                _isEnabledSpecificationComboBox = value;
                NotifyPropertyChanged("IsEnabledSpecificationComboBox");
            }
        }

#endregion IsEnabled Property

#region IsReadOnly Property

        /// <summary>
        /// 제품 텍스트 박스의 텍스트 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnlyProductTextBox
        {
            get
            {
                return _isReadOnlyProductTextBox;
            }
            set
            {
                _isReadOnlyProductTextBox = value;
                NotifyPropertyChanged("IsReadOnlyProductTextBox");
            }
        }

#endregion IsReadOnly Property

#region ComboBox ItemsSource Property

        public IEnumerable<IObservableInventoryProperties> InventoryList
        {
            get
            {
                return _inventoryList;
            }
            set
            {
                _inventoryList = value;
                NotifyPropertyChanged("InventoryList");
            }
        }

        public ObservableCollection<Observable<Maker>> MakerList
        {
            get
            {
                return _makerList;
            }
            set
            {
                _makerList = value;
                NotifyPropertyChanged("MakerList");
            }
        }

        public ObservableCollection<Observable<Measure>> MeasureList
        {
            get
            {
                return _measureList;
            }
            set
            {
                _measureList = value;
                NotifyPropertyChanged("MeasureList");
            }
        }

        public ObservableCollection<IObservableField> ClientList
        {
            get
            {
                return _clientList;
            }
            set
            {
                _clientList = value;
                NotifyPropertyChanged("ClientList");
            }
        }

        public ObservableCollection<Observable<Warehouse>> WarehouseList
        {
            get
            {
                return _warehouseList;
            }
            set
            {
                _warehouseList = value;
                NotifyPropertyChanged("WarehouseList");
            }
        }

        public ObservableCollection<Observable<Employee>> EmployeeList
        {
            get
            {
                return _employeeList;
            }
            set
            {
                _employeeList = value;
                NotifyPropertyChanged("EmployeeList");
            }
        }

        public ObservableCollection<Observable<Project>> ProjectList
        {
            get
            {
                return _projectList;
            }
            set
            {
                _projectList = value;
                NotifyPropertyChanged("ProjectList");
            }
        }

#endregion ComboBox ItemsSource Property

#region ComboBox SelectedItem Property

        /// <summary>
        /// Product를 변경할 시, ProductText, InventoryList 프로퍼티를 업데이트
        /// </summary>
        public Observable<Product> Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
                if (_product != null)
                {
                    ProductText = _product.Name;
                    var inventoryList = DataDirector.GetInstance().SearchInventories(_product.ID);
                    InventoryList = inventoryList.Select(x => new NonSaveObservableInventory(new InventoryFormat(x.Format))).ToList();
                    if (InventoryList.Count() == 1)
                        Inventory = InventoryList.Single();
                }
                if (RecordCommand != null)
                    RecordCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("Product");
            }
        }

        public override IObservableInventoryProperties Inventory
        {
            get
            {
                return base.Inventory;
            }
            set
            {
                base.Inventory = value;
                if (value == null)
                {
                    SpecificationText = null;
                    SpecificationMemo = null;
                    Maker = null;
                    Measure = null;
                    MakerText = null;
                    MeasureText = null;
                }
                if (RecordCommand != null)
                    RecordCommand.RaiseCanExecuteChanged();
                if (LoadLastRecordCommand != null)
                    LoadLastRecordCommand.RaiseCanExecuteChanged();

                CalcInventoryQuantityProperty(Quantity);

                NotifyPropertyChanged("Maker");
                NotifyPropertyChanged("Measure");
                NotifyPropertyChanged("SpecificationMemo");
            }
        }

        public IObservableField Client
        {
            get
            {
                return StockType == IOStockType.INCOMING ? (IObservableField)Supplier : (IObservableField)Customer;
            }
            set
            {
                if (StockType == IOStockType.INCOMING)
                    Supplier = value as Observable<Supplier>;
                else if (StockType == IOStockType.OUTGOING)
                    Customer = value as Observable<Customer>;
                NotifyPropertyChanged("Client");
            }
        }

        public Observable<Maker> Maker
        {
            get
            {
                return Inventory != null ? Inventory.Maker : _maker;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Maker = value;
                else
                    _maker = value;
                NotifyPropertyChanged("Maker");
            }
        }

        public Observable<Measure> Measure
        {
            get
            {
                return Inventory != null ? Inventory.Measure : _measure;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Measure = value;
                else
                    _measure = value;
                NotifyPropertyChanged("Measure");
            }
        }

#endregion ComboBox SelectedItem Property

#region TextBox Property

        /// <summary>
        /// ProductText를 변경할 시, Product, Inventory, InventoryList 속성을 변경
        /// </summary>
        public string ProductText
        {
            get
            {
                return _productText;
            }
            set
            {
                _productText = value;
                if (_productText != null)
                {
                    //제품을 선택하였지만, 이후 다른 제품명을 입력한 경우 저장된 제품 객체를 null로 대입
                    if (Product != null && Product.Name != _productText)
                        Product = null;
                    if (Product == null && Inventory != null)
                        Inventory = null;
                    if (InventoryList != null)
                        InventoryList = null;
                }
                NotifyPropertyChanged("ProductText");
                if (RecordCommand != null)
                    RecordCommand.RaiseCanExecuteChanged();
            }
        }

        public string SpecificationText
        {
            get
            {
                return _specificationText;
            }
            set
            {
                _specificationText = value;
                NotifyPropertyChanged("SpecificationText");
                RecordCommand.RaiseCanExecuteChanged();
            }
        }

        public string SpecificationMemo
        {
            get
            {
                return Inventory != null ? Inventory.Memo : _specificationMemo;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Memo = value;
                else
                    _specificationMemo = value;
                NotifyPropertyChanged("SpecificationMemo");
            }
        }

        public string MakerText
        {
            get
            {
                return _makerText;
            }
            set
            {
                _makerText = value;
                NotifyPropertyChanged("MakerText");
            }
        }

        public string MeasureText
        {
            get
            {
                return _measureText;
            }
            set
            {
                _measureText = value;
                NotifyPropertyChanged("MeasureText");
            }
        }

        public string ClientText
        {
            get
            {
                return _clientText;
            }
            set
            {
                _clientText = value;
                NotifyPropertyChanged("ClientText");
            }
        }

        public string WarehouseText
        {
            get
            {
                return _warehouseText;
            }
            set
            {
                _warehouseText = value;
                NotifyPropertyChanged("WarehouseText");
            }
        }

        public string ProjectText
        {
            get
            {
                return _projectText;
            }
            set
            {
                _projectText = value;
                NotifyPropertyChanged("ProjectText");
            }
        }

        public string EmployeeText
        {
            get
            {
                return _employeeText;
            }
            set
            {
                _employeeText = value;
                NotifyPropertyChanged("EmployeeText");
            }
        }

#endregion TextBox Property

        /// <summary>
        /// IOStockProperties 속성 초기화
        /// </summary>
        /// <param name="iosFmt"></param>
        protected override void InitializeProperties(IOStockFormat iosFmt)
        {
            var ofd = DataDirector.GetInstance();
            var oid = DataDirector.GetInstance();
            customer = ofd.SearchField<Customer>(iosFmt.CustomerID);
            supplier = ofd.SearchField<Supplier>(iosFmt.SupplierID);
            project = ofd.SearchField<Project>(iosFmt.ProjectID);
            employee = ofd.SearchField<Employee>(iosFmt.EmployeeID);
            warehouse = ofd.SearchField<Warehouse>(iosFmt.WarehouseID);

            Product = oid.SearchInventory(iosFmt.InventoryID).Product;
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
            WindowCloseCommand = new RelayCommand(ExecuteWindowCloseCommand);
            ComboBoxKeyUpEventCommand = new RelayCommand<KeyEventArgs>(ExecuteComboBoxKeyUpEventCommand);

            var ofd = DataDirector.GetInstance();
            _makerList = new ObservableCollection<Observable<Maker>>(ofd.CopyFields<Maker>());
            _measureList = new ObservableCollection<Observable<Measure>>(ofd.CopyFields<Measure>());
            _projectList = new ObservableCollection<Observable<Project>>(ofd.CopyFields<Project>());
            _employeeList = new ObservableCollection<Observable<Employee>>(ofd.CopyFields<Employee>());
            _warehouseList = new ObservableCollection<Observable<Warehouse>>(ofd.CopyFields<Warehouse>());
        }

        /// <summary>
        /// windows7에서 한글 ime 사용 시, update 되지 아니한 버그 현상 회피
        /// </summary>
        /// <param name="e"></param>
        private void ExecuteComboBoxKeyUpEventCommand(KeyEventArgs e)
        {
            TextBox textbox = e.OriginalSource as TextBox;
            ComboBox combobox = e.Source as ComboBox;
            if (textbox != null && combobox != null)
            {
                string text = textbox.Text;

                BindingExpression be = BindingOperations.GetBindingExpression(combobox, ComboBox.TextProperty);
                string Name = be.ParentBinding.Path.Path;

                Binding bi = BindingOperations.GetBinding(combobox, ComboBox.TextProperty);
                GetType().GetProperty(bi.Path.Path).SetValue(this, text);
            }
        }

        private void ExecuteWindowCloseCommand()
        {
            var window = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            if (window != null)
                window.Close();
        }

        private void ExecuteComboBoxItemDeleteCommand(object obj)
        {
            IObservableField observableField = obj as IObservableField;
            DataDirector.GetInstance().RemoveField(observableField);
        }

        private void ExecuteLoadLastRecordCommand()
        {
            if (Inventory == null)
                return;

            if (StockType == IOStockType.OUTGOING)
            {
                var qresult = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} = '{4}' order by {5} desc limit 1;",
                    typeof(IOStockFormat).Name, "InventoryID", Inventory.ID, "StockType", (int)IOStockType.INCOMING, "Date");
                if (qresult.Count() == 1)
                    UnitPrice = qresult.Single().UnitPrice;
            }

            var query = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} = '{4}' order by {5} desc limit 1;",
                    typeof(IOStockFormat).Name, "InventoryID", Inventory.ID, "StockType", (int)StockType, "Date");

            if (query.Count() != 1)
                return;
            var item = query.Single();
            Quantity = item.Quantity;
            Employee = DataDirector.GetInstance().SearchField<Employee>(item.EmployeeID);
            if (StockType == IOStockType.INCOMING)
            {
                Client = DataDirector.GetInstance().SearchField<Supplier>(item.SupplierID);
                Warehouse = DataDirector.GetInstance().SearchField<Warehouse>(item.WarehouseID);
                UnitPrice = item.UnitPrice;
            }
            else if (StockType == IOStockType.OUTGOING)
            {
                Client = DataDirector.GetInstance().SearchField<Customer>(item.CustomerID);
                Project = DataDirector.GetInstance().SearchField<Project>(item.ProjectID);
            }
        }

        private void ExecuteProjectComboBoxGotFocusEventCommand(RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox)
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
                var ofd = DataDirector.GetInstance();
                var product = ofd.SearchField<Product>(node.ObservableObjectID);
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

        private void ExecuteRecordCommand()
        {
            Record();
            ExecuteWindowCloseCommand();
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

        public void UpdateNewItem(object item)
        {
            if (item is Observable<Measure>)
                MeasureList.Add(item as Observable<Measure>);
            else if (item is Observable<Maker>)
                MakerList.Add(item as Observable<Maker>);
            else if (item is Observable<Customer>)
                ClientList.Add(item as Observable<Customer>);
            else if (item is Observable<Supplier>)
                ClientList.Add(item as Observable<Supplier>);
            else if (item is Observable<Project>)
                ProjectList.Add(item as Observable<Project>);
            else if (item is Observable<Warehouse>)
                WarehouseList.Add(item as Observable<Warehouse>);
            else if (item is Observable<Employee>)
                EmployeeList.Add(item as Observable<Employee>);
        }

        public void UpdateDelItem(object item)
        {
            if (item is Observable<Measure>)
                MeasureList.Remove(item as Observable<Measure>);
            else if (item is Observable<Maker>)
                MakerList.Remove(item as Observable<Maker>);
            else if (item is Observable<Customer>)
                ClientList.Remove(item as Observable<Customer>);
            else if (item is Observable<Supplier>)
                ClientList.Remove(item as Observable<Supplier>);
            else if (item is Observable<Project>)
                ProjectList.Remove(item as Observable<Project>);
            else if (item is Observable<Warehouse>)
                WarehouseList.Remove(item as Observable<Warehouse>);
            else if (item is Observable<Employee>)
                EmployeeList.Remove(item as Observable<Employee>);
        }

        public IObservableIOStockProperties Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");

            IObservableIOStockProperties result = null;

            switch (_mode)
            {
                case Mode.ADD:
                    CreateIOStockNewProperies();
                    ApplyModifiedInventoryProperties();
                    DataDirector.GetInstance().DB.Insert(Format);
                    result = new ObservableIOStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    ApplyModifiedIOStockProperties();
                    ApplyModifiedInventoryProperties();
                    DataDirector.GetInstance().DB.Update(Format);
                    _originSource.Format = Format;
                    result = _originSource;
                    break;
            }
            //RefreshDataGridItems();
            return result;
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </sumary>
        private void CreateIOStockNewProperies()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                    {
                        var supplier = new Observable<Supplier>(ClientText);
                        DataDirector.GetInstance().AddField(supplier);
                        Supplier = supplier;
                    }
                    if (Warehouse == null && !string.IsNullOrEmpty(WarehouseText))
                    {
                        warehouse = new Observable<Warehouse>(WarehouseText);
                        DataDirector.GetInstance().AddField(warehouse);
                        Warehouse = warehouse;
                    }
                    break;

                case IOStockType.OUTGOING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                    {
                        var customer = new Observable<Customer>(ClientText);
                        DataDirector.GetInstance().AddField(customer);
                        Customer = customer;
                    }
                    if (Project == null && !string.IsNullOrEmpty(ProjectText))
                    {
                        var project = new Observable<Project>(ProjectText);
                        DataDirector.GetInstance().AddField(project);
                        Project = project;
                    }
                    break;
            }
            if (Employee == null && !string.IsNullOrEmpty(EmployeeText))
            {
                var employee = new Observable<Employee>(EmployeeText);
                DataDirector.GetInstance().AddField(employee);
                Employee = employee;
            }
            if (Maker == null && !string.IsNullOrEmpty(MakerText))
            {
                var maker = new Observable<Maker>(MakerText);
                DataDirector.GetInstance().AddField(maker);
                Maker = maker;
            }
            if (Measure == null && !string.IsNullOrEmpty(MeasureText))
            {
                var measure = new Observable<Measure>(MeasureText);
                DataDirector.GetInstance().AddField(measure);
                Measure = measure;
            }
        }

        /// <summary>
        /// 이름이 변경된 객체를 수정
        /// </summary>
        private void ApplyModifiedIOStockProperties()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (_originSource.Supplier != null && Client == null)
                    {
                        _originSource.Supplier.Name = ClientText;
                        Client = _originSource.Supplier;
                    }
                    if (_originSource.Warehouse != null && Warehouse == null)
                    {
                        _originSource.Warehouse.Name = WarehouseText;
                        Warehouse = _originSource.Warehouse;
                    }
                    break;

                case IOStockType.OUTGOING:
                    if (_originSource.Customer != null && Client == null)
                    {
                        _originSource.Customer.Name = ClientText;
                        Client = _originSource.Customer;
                    }
                    if (_originSource.Project != null && Project == null)
                    {
                        _originSource.Project.Name = ProjectText;
                        Project = _originSource.Project;
                    }
                    break;
            }
            if (_originSource.Employee != null && Employee == null)
            {
                _originSource.Employee.Name = EmployeeText;
                Employee = _originSource.Employee;
            }
            if (_originSource.Inventory.Maker != null && Maker == null)
            {
                _originSource.Inventory.Maker.Name = MakerText;
                Maker = _originSource.Inventory.Maker;
            }
            if (_originSource.Inventory.Measure != null && Measure == null)
            {
                _originSource.Inventory.Measure.Name = MeasureText;
                Measure = _originSource.Inventory.Measure;
            }
            CreateIOStockNewProperies();
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private void ApplyModifiedInventoryProperties()
        {
            ObservableInventory inventory = null;
            if (Inventory == null)
            {
                if (Product == null)
                {
                    Observable<Product> product = new Observable<Product>(ProductText);
                    DataDirector.GetInstance().AddField(product);
                    Product = product;
                }
                inventory = new ObservableInventory(Product, SpecificationText, InventoryQuantity, SpecificationMemo, Maker, Measure);
                DataDirector.GetInstance().AddInventory(inventory);
            }
            else
            {
                inventory = DataDirector.GetInstance().SearchInventory(Inventory.ID);
                inventory.Format = Inventory.Format;
                inventory.Quantity = InventoryQuantity;
            }
            Inventory = inventory;
        }

#if false
        /// <summary>
        /// 입출고 데이터를 새로 추가하는 경우 또는 과거의 데이터를 수정할 경우 입출고 수량에 변화가 있다면
        /// 관련 IOStock 데이터들의 잔여수량 및 재고수량을 다시 계산하여 전부 업데이트하고 Owner의 DataGridItems 역시 변화된 값들을 반영하게 한다.
        /// TODO
        /// </summary>
        private void RefreshDataGridItems()
        {
            if (_ioStockStatusViewModel != null && _ioStockStatusViewModel.DataGridItemSources != null)
            {
                var backupSource = _ioStockStatusViewModel.DataGridItemSources;
                foreach (var src in backupSource)
                {
                    if (src.Inventory.ID == Inventory.ID && src.Date > Date)
                        src.Refresh();
                }
            }
        }
#endif
    }
}