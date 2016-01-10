using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        public ProductSelectorViewModel TreeViewViewModel { get; set; }

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

        public override IOStockType StockType
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
                    case IOStockType.INCOMING:
                        Project = null;
                        ProjectText = null;

                        ClientList = ofd.CreateList<Supplier>();

                        IsEditableSpecification = true;
                        IsReadOnlyProductTextBox = false;
                        IsEnabledWarehouseComboBox = true;
                        IsEnabledProjectComboBox = false;
                        break;

                    case IOStockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;

                        ClientList = ofd.CreateList<Customer>();

                        IsEditableSpecification = false;
                        IsReadOnlyProductTextBox = true;
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
                        switch (StockType)
                        {
                            case IOStockType.INCOMING:
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

                            case IOStockType.OUTGOING: //출고할 경우 기존의 데이터만 사용하기에
                                RemainingQuantity = Inventory.Quantity - value;
                                InventoryQuantity = Inventory.Quantity - value;
                                break;
                        }
                        break;

                    case Mode.MODIFY:
                        switch (StockType)
                        {
                            case IOStockType.INCOMING:
                                RemainingQuantity = _origin.RemainingQuantity + value - _origin.Quantity;
                                InventoryQuantity = _origin.Inventory.Quantity + value - _origin.Quantity;
                                break;

                            case IOStockType.OUTGOING:
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
                    var searchList = ObservableInventoryDirector.GetInstance().SearchAsProductID(_product.ID);
                    InventoryList = searchList.Select(x => new NonSaveObservableInventory(x.Format)).ToList();
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
                if (value == null)
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

        public bool IsEnabledMeasureComboBox
        {
            get
            {
                return _isEnabledMeasureComboBox;
            }
            set
            {
                _isEnabledMeasureComboBox = value;
                NotifyPropertyChanged("IsEnabledMeasureComboBox");
            }
        }

        public bool IsEnabledMakerComboBox
        {
            get
            {
                return _isEnabledMakerComboBox;
            }
            set
            {
                _isEnabledMakerComboBox = value;
                NotifyPropertyChanged("IsEnabledMakerComboBox");
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

        public bool IsReadOnlySpecificationMemoTextBox
        {
            get
            {
                return _isReadOnlySpecificationMemoTextBox;
            }
            set
            {
                _isReadOnlySpecificationMemoTextBox = value;
                NotifyPropertyChanged("IsReadOnlySpecificationMemoTextBox");
            }
        }

        #endregion IsReadOnly Property
    }
}