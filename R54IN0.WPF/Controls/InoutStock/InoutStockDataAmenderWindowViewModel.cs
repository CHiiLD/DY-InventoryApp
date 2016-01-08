using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InoutStockDataAmenderWindowViewModel : ObservableInoutStock
    {
        private enum Mode
        {
            ADD,
            MODIFY
        }

        private class NonSaveObservableInventory : ObservableInventory
        {
            public NonSaveObservableInventory() : base()
            {
            }

            public NonSaveObservableInventory(InventoryFormat inventory) : base(inventory)
            {
            }

            public override void NotifyPropertyChanged(string propertyName)
            {
                if (propertyChanged != null)
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private IObservableInoutStockProperties _origin;
        private Mode _mode;
        private bool _isOpenFlyout;
        private bool _isReadOnlyProductName;
        private IEnumerable<ObservableInventory> _inventoryList;
        private IEnumerable<IObservableField> _clientList;
        private Observable<Product> _product;
        private string _specificationMemo;
        private Observable<Measure> _measure;
        private Observable<Maker> _maker;
        private bool? _isCheckedInComing;
        private bool? _isCheckedOutGoing;
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
        private bool _isEnabledProductText;
        private bool _isEnabledSpecification;
        private bool _isEnabledSpecificationMemo;
        private bool _isEnabledMaker;
        private bool _isEnabledMeasure;
        private int _inventoryQuantity;

        public InoutStockDataAmenderWindowViewModel() : base()
        {
            _mode = Mode.ADD;
            IsCheckedOutGoing = false;
            IsCheckedInComing = true;
            Date = DateTime.Now;
            Initialize();

            IsEnabledMaker = true;
            IsEnabledMeasure = true;
            IsEnabledSpecificationMemo = true;
            IsEnabledSpecification = true;
            IsEnabledProductText = true;
        }

        public InoutStockDataAmenderWindowViewModel(IObservableInoutStockProperties observableInoutStock) : base(
            new InoutStockFormat(observableInoutStock.Format))
        {
            switch(StockType)
            {
                case StockType.INCOMING:
                    IsCheckedInComing = true;
                    IsCheckedOutGoing = false;
                    break;
                case StockType.OUTGOING:
                    IsCheckedInComing = false;
                    IsCheckedOutGoing = true;
                    break;
            }

            _origin = observableInoutStock;
            _mode = Mode.MODIFY;
            Initialize();

            IsEnabledMaker = true;
            IsEnabledMeasure = true;
            IsEnabledSpecificationMemo = true;
            IsEnabledSpecification = false;
            IsEnabledProductText = false;
        }
        public ProductSelectorViewModel TreeViewViewModel
        {
            get; set;
        }

        public ICommand ProductSearchCommand { get; set; }

        public ICommand RecordCommand { get; set; }

        /// <summary>
        /// 제품 탐색기에서 제품을 선택한 뒤, 확인 버튼의 Command 객체
        /// </summary>
        public ICommand ProductSelectCommand { get; set; }

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

        public override StockType StockType
        {
            get
            {
                return base.StockType;
            }
            set
            {
                base.StockType = value;
                var ofd = ObservableFieldDirector.GetInstance();
                switch (base.StockType)
                {
                    case StockType.INCOMING:
                        Project = null;
                        ProjectText = null;

                        ClientList = ofd.CreateList<Supplier>();

                        IsEditableSpecification = true;
                        IsReadOnlyProductText = false;
                        IsEnabledWarehouseComboBox = true;
                        IsEnabledProjectComboBox = false;
                        break;

                    case StockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;

                        ClientList = ofd.CreateList<Customer>();

                        IsEditableSpecification = false;
                        IsReadOnlyProductText = true;
                        IsEnabledWarehouseComboBox = false;
                        IsEnabledProjectComboBox = true;

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
            }
        }

        /// <summary>
        /// 입고 라디오박스 체크 바인딩 프로퍼티
        /// </summary>
        public bool? IsCheckedInComing
        {
            get
            {
                return _isCheckedInComing;
            }
            set
            {
                _isCheckedInComing = value;
                if (value == true)
                    StockType = StockType.INCOMING;
                NotifyPropertyChanged("IsCheckedInComing");
            }
        }

        /// <summary>
        /// 출고 라디오박스 체크 바인딩 프로퍼티
        /// </summary>
        public bool? IsCheckedOutGoing
        {
            get
            {
                return _isCheckedOutGoing;
            }
            set
            {
                _isCheckedOutGoing = value;
                if (value == true)
                    StockType = StockType.OUTGOING;
                NotifyPropertyChanged("IsCheckedOutGoing");
            }
        }

        /// <summary>
        /// 제품 텍스트 박스의 텍스트 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnlyProductText
        {
            get
            {
                return _isReadOnlyProductName;
            }
            set
            {
                _isReadOnlyProductName = value;
                NotifyPropertyChanged("IsReadOnlyProductName");
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
        /// 제품의 개별적 입고가, 출고가
        /// </summary>
        public override decimal UnitPrice
        {
            get
            {
                return base.UnitPrice;
            }
            set
            {
                base.UnitPrice = value;
                NotifyPropertyChanged("Amount");
            }
        }

        public decimal Amount
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        /// <summary>
        /// 입고 또는 출고 수량
        /// </summary>
        public override int Quantity
        {
            get
            {
                return base.Quantity;
            }
            set
            {
                base.Quantity = value;
                switch (_mode)
                {
                    case Mode.ADD:
                        switch(StockType)
                        {
                            case StockType.INCOMING:
                                if (Product == null || Inventory == null) //새로운 제품의 규격을 등록하는 경우
                                {
                                    RemainingQuantity = value;
                                    InventoryQuantity = value;
                                }
                                else //기존 제품의 규격에 입고 하는 경우 
                                {
                                    RemainingQuantity = Inventory.Quantity + value;
                                    InventoryQuantity = Inventory.Quantity + value;
                                }
                                break;
                            case StockType.OUTGOING: //출고할 경우 기존의 데이터만 사용하기에
                                RemainingQuantity = Inventory.Quantity - value;
                                InventoryQuantity = Inventory.Quantity - value;
                                break;
                        }
                        break;
                    case Mode.MODIFY:
                        switch (StockType)
                        {
                            case StockType.INCOMING:
                                RemainingQuantity = _origin.RemainingQuantity + value - _origin.Quantity;
                                InventoryQuantity = _origin.Inventory.Quantity + value - _origin.Quantity;
                                break;
                            case StockType.OUTGOING: 
                                RemainingQuantity = _origin.RemainingQuantity + _origin.Quantity - value;
                                InventoryQuantity = _origin.Inventory.Quantity + _origin.Quantity - value;
                                break;
                        }
                        break;
                }
                NotifyPropertyChanged("Amount");
            }
        }

        /// <summary>
        /// 재고 수량
        /// </summary>
        public int InventoryQuantity
        {
            get
            {
                return _inventoryQuantity;
            }
            set
            {
                _inventoryQuantity = value;
                NotifyPropertyChanged("InventoryQuantity");
            }
        }

        #region ComboBox ItemsSource Property

        public IEnumerable<ObservableInventory> InventoryList
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

        public IEnumerable<Observable<Maker>> MakerList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Maker>();
            }
        }

        public IEnumerable<Observable<Measure>> MeasureList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Measure>();
            }
        }

        public IEnumerable<IObservableField> ClientList
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

        public IEnumerable<Observable<Warehouse>> WarehouseList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Warehouse>();
            }
        }

        public IEnumerable<Observable<Employee>> EmployeeList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Employee>();
            }
        }

        public IEnumerable<Observable<Project>> ProjectList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Project>();
            }
        }

        #endregion ComboBox ItemsSource Property

        #region ComboBox SelectedItem Property

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
                    var list = ObservableInvenDirector.GetInstance().SearchAsProductID(_product.ID);
                    InventoryList = list.Select(x => new NonSaveObservableInventory(x.Format));
                }
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
                if (base.Inventory == null)
                {
                    SpecificationText = null;
                    SpecificationMemo = null;
                    Maker = null;
                    Measure = null;
                    MakerText = null;
                    MeasureText = null;
                }
                NotifyPropertyChanged("Maker");
                NotifyPropertyChanged("Measure");
                NotifyPropertyChanged("SpecificationMemo");
                UpdateRecordCommand();
            }
        }
        public IObservableField Client
        {
            get
            {
                return StockType == StockType.INCOMING ? (IObservableField)Supplier : (IObservableField)Customer;
            }
            set
            {
                if (StockType == StockType.INCOMING)
                    Supplier = value as Observable<Supplier>;
                else if (StockType == StockType.OUTGOING)
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
                    if (Inventory != null && Product == null)
                        Inventory = null;
                    if (InventoryList != null)
                        InventoryList = null;
                }
                NotifyPropertyChanged("ProductText");
                UpdateRecordCommand();
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
                UpdateRecordCommand();
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



        #region IsEnabled Property

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

        public bool IsEnabledProductText
        {
            get
            {
                return _isEnabledProductText;
            }
            set
            {
                _isEnabledProductText = value;
                NotifyPropertyChanged("IsEnabledProductText");
            }
        }

        public bool IsEnabledSpecification
        {
            get
            {
                return _isEnabledSpecification;
            }
            set
            {
                _isEnabledSpecification = value;
                NotifyPropertyChanged("IsEnabledSpecification");
            }
        }

        public bool IsEnabledSpecificationMemo
        {
            get
            {
                return _isEnabledSpecificationMemo;
            }
            set
            {
                _isEnabledSpecificationMemo = value;
                NotifyPropertyChanged("IsEnabledSpecificationMemo");
            }
        }

        public bool IsEnabledMaker
        {
            get
            {
                return _isEnabledMaker;
            }
            set
            {
                _isEnabledMaker = value;
                NotifyPropertyChanged("IsEnabledMaker");
            }
        }

        public bool IsEnabledMeasure
        {
            get
            {
                return _isEnabledMeasure;
            }
            set
            {
                _isEnabledMeasure = value;
                NotifyPropertyChanged("IsEnabledMeasure");
            }
        }

        #endregion IsEnabled Property

        private void Initialize()
        {
            TreeViewViewModel = new ProductSelectorViewModel();
            TreeViewViewModel.PropertyChanged += OnTreeViewModelPropertyChanged;
            ProductSearchCommand = new CommandHandler(ExecuteProductSearchCommand, CanSearch);
            RecordCommand = new CommandHandler(ExecuteRecordCommand, CanRecord);
            ProductSelectCommand = new CommandHandler(ExecuteProductSelectCommand, CanSelectProduct);
        }

        protected override void InitializeProperties(InoutStockFormat inoutStockFormat)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            Customer = ofd.Search<Customer>(inoutStockFormat.CustomerID);
            Supplier = ofd.Search<Supplier>(inoutStockFormat.SupplierID);
            Project = ofd.Search<Project>(inoutStockFormat.ProjectID);
            Employee = ofd.Search<Employee>(inoutStockFormat.EmployeeID);
            Warehouse = ofd.Search<Warehouse>(inoutStockFormat.WarehouseID);

            var oid = ObservableInvenDirector.GetInstance();
            Inventory = new NonSaveObservableInventory(oid.Search(inoutStockFormat.InventoryID).Format);
            Product = Inventory.Product;
        }

        void UpdateRecordCommand()
        {
            var cmd = RecordCommand as CommandHandler;
            if (cmd != null)
                cmd.UpdateCanExecute();
        }

        public ObservableInoutStock Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInvenDirector.GetInstance();
            if (Client == null && !string.IsNullOrEmpty(ClientText)) //거래처
            {
                switch (StockType)
                {
                    case StockType.INCOMING:
                        Supplier = new Observable<Supplier>() { Name = ClientText };
                        ofd.Add<Supplier>(Supplier);
                        break;

                    case StockType.OUTGOING:
                        Customer = new Observable<Customer>() { Name = ClientText };
                        ofd.Add<Customer>(Customer);
                        break;
                }
            }
            if (Warehouse == null && !string.IsNullOrEmpty(WarehouseText))
            {
                Warehouse = new Observable<Warehouse>() { Name = WarehouseText };
                ofd.Add<Warehouse>(Warehouse);
            }
            if (Project == null && !string.IsNullOrEmpty(ProjectText))
            {
                Project = new Observable<Project>() { Name = ProjectText };
                ofd.Add<Project>(Project);
            }
            if (Employee == null && !string.IsNullOrEmpty(EmployeeText))
            {
                Employee = new Observable<Employee>() { Name = EmployeeText };
                ofd.Add<Employee>(Employee);
            }
            if (Inventory == null)
            {
                ObservableInventory inven = new ObservableInventory(new InventoryFormat().Save<InventoryFormat>());
                inven.Product = Product != null ? Product : new Observable<Product>() { Name = ProductText };
                inven.Specification = SpecificationText;
                inven.Memo = _specificationMemo;
                if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                    inven.Maker = new Observable<Maker>() { Name = MakerText };
                else if (Maker != null)
                    inven.Maker = Maker;
                if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                    inven.Measure = new Observable<Measure>() { Name = MeasureText };
                else if (Measure != null)
                    inven.Measure = Measure;
                inven.Quantity = InventoryQuantity;
                ObservableInvenDirector.GetInstance().Add(inven as ObservableInventory);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(inven);
                Inventory = inven;
            }
            else
            {
                ObservableInventory origin = oid.Search(Inventory.ID);
                if (origin != null)
                {
                    if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                        origin.Maker = new Observable<Maker>() { Name = MakerText };
                    else if (origin.Maker != Maker)
                        origin.Maker = Maker;
                    if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                        origin.Measure = new Observable<Measure>() { Name = MeasureText };
                    else if (origin.Measure != Measure)
                        origin.Measure = Measure;
                    if (origin.Memo != SpecificationMemo)
                        origin.Memo = SpecificationMemo;
                    origin.Quantity = InventoryQuantity;
                }
            }

            ObservableInoutStock result = null;
            switch (_mode)
            {
                case Mode.ADD:
                    result = new ObservableInoutStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    result = _origin as ObservableInoutStock;
                    result.Format = Format;
                    break;
            }
            result.Format.Save<InoutStockFormat>();
            return result;
        }

        private void OnTreeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
            {
                var cmd = ProductSelectCommand as CommandHandler;
                cmd.UpdateCanExecute();
            }
        }

        private void ExecuteProductSelectCommand(object obj)
        {
            var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(n => n.Type == NodeType.PRODUCT));
            var node = nodes.FirstOrDefault();
            if (node != null)
            {
                var ofd = ObservableFieldDirector.GetInstance();
                var product = ofd.Search<Product>(node.ProductID);
                if (product != null)
                    Product = product;
            }
            IsOpenFlyout = false;
        }

        private bool CanSelectProduct(object arg)
        {
            return TreeViewViewModel.SelectedNodes.Count == 1 && TreeViewViewModel.SelectedNodes.Single().Type == NodeType.PRODUCT;
        }

        private bool CanRecord(object arg)
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                return false;
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                return false;
            return true;
        }

        private void ExecuteRecordCommand(object obj)
        {
            Record();
            var currentWindow = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            if (currentWindow != null)
                currentWindow.Close();
        }

        /// <summary>
        /// 제품 텍스트박스의 검색 버튼의 활성화 여부
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanSearch(object arg)
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
        private void ExecuteProductSearchCommand(object obj)
        {
            IsOpenFlyout = true;
        }

        public override void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}