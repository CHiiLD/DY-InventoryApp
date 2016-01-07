using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            public override void NotifyPropertyChanged(string propertyName)
            {
                if (propertyChanged != null)
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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

        public InoutStockDataAmenderWindowViewModel() : base()
        {
            _mode = Mode.ADD;
            IsCheckedOutGoing = false;
            IsCheckedInComing = true;
            Initialize();
        }

        public InoutStockDataAmenderWindowViewModel(InoutStockFormat stockFormat) : base(stockFormat)
        {
            _mode = Mode.MODIFY;
            Initialize();
        }

        public ICommand ProductSearchCommand { get; set; }

        public ICommand RecordCommand { get; set; }

        public override StockType StockType
        {
            get
            {
                return base.StockType;
            }
            set
            {
                base.StockType = value;
                bool isInComing = StockType == StockType.INCOMING ? true : false;
                IsReadOnlyProductName = !isInComing;
                IsEnabledWarehouseComboBox = isInComing;
                IsEnabledProjectComboBox = !isInComing;
                var ofd = ObservableFieldDirector.GetInstance();
                switch (StockType)
                {
                    case StockType.INCOMING:
                        Project = null;
                        ProjectText = null;
                        ClientList = ofd.CreateList<Supplier>();
                        break;

                    case StockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;
                        if (ProductText != null)
                            ProductText = null;
                        ClientList = ofd.CreateList<Customer>();
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
        public bool IsReadOnlyProductName
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
                    InventoryList = ObservableInvenDirector.GetInstance().SearchAsProductID(_product.ID);
                NotifyPropertyChanged("Product");
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

        public string ProductText
        {
            get
            {
                return _productText;
            }
            set
            {
                _productText = value;
                if (Product == null)
                    Product = null;
                NotifyPropertyChanged("ProductText");
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

        public ObservableInoutStock Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");
            var ofd = ObservableFieldDirector.GetInstance();
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
                Inventory = new ObservableInventory(new InventoryFormat().Save<InventoryFormat>());
                Inventory.Product = Product != null ? Product : new Observable<Product>() { Name = ProductText };
                Inventory.Specification = SpecificationText;
                Inventory.Memo = _specificationMemo;
                if (!string.IsNullOrEmpty(MakerText))
                    Inventory.Maker = new Observable<Maker>() { Name = MakerText };
                if (!string.IsNullOrEmpty(MeasureText))
                    Inventory.Measure = new Observable<Measure>() { Name = MeasureText };
                Inventory.Quantity = Quantity;
                ObservableInvenDirector.GetInstance().Add(Inventory as ObservableInventory);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(Inventory);
            }
            else
            {
                Inventory.Product = Product;
            }
            ObservableInoutStock observableInoutStock = new ObservableInoutStock(this);
            observableInoutStock.Format.Save<InoutStockFormat>();
            CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(observableInoutStock);
            return observableInoutStock;
        }

        private void Initialize()
        {
            ProductSearchCommand = new CommandHandler(ExecuteProductSearchCommand, CanSearch);
            RecordCommand = new CommandHandler(ExecuteRecordCommand, CanRecord);
            Date = DateTime.Now;
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