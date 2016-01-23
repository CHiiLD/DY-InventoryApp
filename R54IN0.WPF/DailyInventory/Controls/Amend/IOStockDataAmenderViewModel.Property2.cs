using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        public override IOStockType StockType
        {
            get
            {
                return base.StockType;
            }
            set
            {
                base.StockType = value;
                var ofd = InventoryDataCommander.GetInstance();

                switch (value)
                {
                    case IOStockType.INCOMING:
                        Project = null;
                        ProjectText = null;
                        ClientList = new ObservableCollection<IObservableField>(ofd.CopyObservableFields<Supplier>());
                        IsEditableSpecification = true;
                        IsReadOnlyProductTextBox = false;
                        IsEnabledWarehouseComboBox = true;
                        IsEnabledProjectComboBox = false;
                        AccountTypeText = SUPPLIER;
                        break;

                    case IOStockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;
                        ClientList = new ObservableCollection<IObservableField>(ofd.CopyObservableFields<Customer>());
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
                    var inventoryList = InventoryDataCommander.GetInstance().SearchObservableInventoryAsProductID(_product.ID);
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

                Console.WriteLine("setted clinet text property: {0}", value);
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
    }
}