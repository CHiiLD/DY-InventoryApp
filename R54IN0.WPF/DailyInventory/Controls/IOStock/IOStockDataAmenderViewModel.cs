using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : IObservableIOStockProperties
    {
        private IOStockFormat _fmt;
        private Observable<Customer> _customer;
        private Observable<Supplier> _supplier;
        private Observable<Project> _project;
        private Observable<Employee> _employee;
        private Observable<Warehouse> _warehouse;
        private IObservableInventoryProperties _inventory;

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
        private bool _isReadOnlySpecificationMemoTextBox;
        private bool _isEnabledMakerComboBox;
        private bool _isEnabledMeasureComboBox;
        private bool _isEnabledInComingRadioButton;
        private bool _isEnabledInOutGoingRadioButton;
        private int _inventoryQuantity;
        private bool _isEnabledDatePicker;

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
        /// TEST용 생성자
        /// </summary>
        public IOStockDataAmenderViewModel()
        {
            Initialize();
            _fmt = new IOStockFormat();
            _mode = Mode.ADD;
            StockType = IOStockType.INCOMING;
            Date = DateTime.Now;

            IsReadOnlySpecificationMemoTextBox = false;
            IsEnabledMakerComboBox = true;
            IsEnabledMeasureComboBox = true;
            IsEnabledSpecificationComboBox = true;
            IsEnabledOutGoingRadioButton = true;
            IsEnabledInComingRadioButton = true;
            IsEnabledDatePicker = true;
        }

        /// <summary>
        /// TEST용 생성자
        /// </summary>
        /// <param name="observableInoutStock"></param>
        public IOStockDataAmenderViewModel(IObservableIOStockProperties observableInoutStock)
        {
            Initialize();
            _fmt = new IOStockFormat(observableInoutStock.Format);
            InitializeProperties(_fmt);
            _mode = Mode.MODIFY;
            _originSource = observableInoutStock;
            StockType = observableInoutStock.StockType;

            IsReadOnlyProductTextBox = true;
            IsReadOnlySpecificationMemoTextBox = false;
            IsEnabledMakerComboBox = true;
            IsEnabledMeasureComboBox = true;
            IsEnabledSpecificationComboBox = false;
            IsEnabledOutGoingRadioButton = false;
            IsEnabledInComingRadioButton = false;
            IsEnabledDatePicker = false;
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
        /// <param name="observableInoutStock"></param>
        public IOStockDataAmenderViewModel(IOStockStatusViewModel ioStockStatusViewModel, IObservableIOStockProperties observableInoutStock) :
            this(observableInoutStock)
        {
            _ioStockStatusViewModel = ioStockStatusViewModel;
        }

        /// <summary>
        /// IOStockProperties 속성 초기화
        /// </summary>
        /// <param name="iosFmt"></param>
        protected void InitializeProperties(IOStockFormat iosFmt)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            _customer = ofd.Search<Customer>(iosFmt.CustomerID);
            _supplier = ofd.Search<Supplier>(iosFmt.SupplierID);
            _project = ofd.Search<Project>(iosFmt.ProjectID);
            _employee = ofd.Search<Employee>(iosFmt.EmployeeID);
            _warehouse = ofd.Search<Warehouse>(iosFmt.WarehouseID);

            Product = oid.Search(iosFmt.InventoryID).Product;
            Inventory = InventoryList.Where(x => x.ID == iosFmt.InventoryID).SingleOrDefault();
            InventoryQuantity = Inventory.Quantity;
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
        }

        private void UpdateQuantityProperties()
        {
            int value = Quantity;
            switch (_mode)
            {
                case Mode.ADD:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            InventoryQuantity = (Product == null || Inventory == null) ? value : Inventory.Quantity + value;
                            break;
                        case IOStockType.OUTGOING:
                            InventoryQuantity = (Product == null || Inventory == null) ? -value : Inventory.Quantity - value;
                            break;
                    }
                    break;
                case Mode.MODIFY:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            InventoryQuantity = _originSource.Inventory.Quantity + value - _originSource.Quantity;
                            break;
                        case IOStockType.OUTGOING:
                            InventoryQuantity = _originSource.Inventory.Quantity + _originSource.Quantity - value;
                            break;
                    }
                    break;
            }
        }

        private void CalculateRemainingQuantity(IOStockFormat near = null)
        {
            int qty = Quantity;
            switch (_mode)
            {
                case Mode.ADD:
                    if (Product == null || Inventory == null)
                    {
                        switch (StockType)
                        {
                            case IOStockType.INCOMING: RemainingQuantity = qty; break;
                            case IOStockType.OUTGOING: RemainingQuantity = -qty; break;
                        }
                    }
                    else
                    {
                        switch (StockType)
                        {
                            case IOStockType.INCOMING: RemainingQuantity = (near == null) ? qty : near.RemainingQuantity + Quantity; break;
                            case IOStockType.OUTGOING: RemainingQuantity = (near == null) ? -qty : near.RemainingQuantity - Quantity; break;
                        }
                    }
                    break;
                case Mode.MODIFY:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            RemainingQuantity = _originSource.RemainingQuantity + qty - _originSource.Quantity;
                            break;
                        case IOStockType.OUTGOING:
                            RemainingQuantity = _originSource.RemainingQuantity + _originSource.Quantity - qty;
                            break;
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
                var product = ofd.Search<Product>(node.ProductID);
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

        public void NotifyPropertyChanged(string propertyName)
        {
            var eventHandler = _propertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}