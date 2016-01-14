using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : ObservableIOStock
    {
        private IOStockStatusViewModel _ioStockStatusViewModel;
        private IObservableIOStockProperties _originObservableIOStock;
        private Mode _mode;
        private bool _isOpenFlyout;
        private bool _isReadOnlyProductTextBox;
        private IEnumerable<IObservableInventoryProperties> _inventoryList;
        private IEnumerable<IObservableField> _clientList;
        private Observable<Product> _product;
        private string _specificationMemo;
        private Observable<Measure> _measure;
        private Observable<Maker> _maker;
        private IOStockFormat _nearIOStockFormat;
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

        /// <summary>
        /// TEST용 생성자
        /// </summary>
        public IOStockDataAmenderViewModel() : base()
        {
            _mode = Mode.ADD;
            StockType = IOStockType.INCOMING;
            Initialize();
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
        public IOStockDataAmenderViewModel(IObservableIOStockProperties observableInoutStock) :
            base(new IOStockFormat(observableInoutStock.Format))
        {
            _originObservableIOStock = observableInoutStock;
            _mode = Mode.MODIFY;
            StockType = observableInoutStock.StockType;
            Initialize();

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

        private void Initialize()
        {
            TreeViewViewModel = new MultiSelectTreeViewModelView();
            TreeViewViewModel.PropertyChanged += OnTreeViewModelPropertyChanged;
            TreeViewViewModel.ContextMenuVisibility = Visibility.Collapsed;

            ProductSearchCommand = new RelayCommand(ExecuteProductSearchCommand, CanSearch);
            RecordCommand = new RelayCommand(ExecuteRecordCommand, CanRecord);
            ProductSelectCommand = new RelayCommand(ExecuteProductSelectCommand, CanSelectProduct);
        }

        private void UpdateQuantityProperties(int value)
        {
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
                                RemainingQuantity = (_nearIOStockFormat == null) ? value : _nearIOStockFormat.RemainingQuantity + Quantity;
                                InventoryQuantity = Inventory.Quantity + value;
                            }
                            break;

                        case IOStockType.OUTGOING: //출고할 경우 기존의 데이터만 사용하기에
                            if (Product == null || Inventory == null) //새로운 제품의 규격을 등록하는 경우
                            {
                                RemainingQuantity = -value;
                                InventoryQuantity = -value;
                            }
                            else //기존 제품의 규격에 입고 하는 경우
                            {
                                RemainingQuantity = (_nearIOStockFormat == null) ? -value : _nearIOStockFormat.RemainingQuantity - Quantity;
                                InventoryQuantity = Inventory.Quantity - value;
                            }
                            break;
                    }
                    break;

                case Mode.MODIFY:
                    switch (StockType)
                    {
                        case IOStockType.INCOMING:
                            RemainingQuantity = _originObservableIOStock.RemainingQuantity + value - _originObservableIOStock.Quantity;
                            InventoryQuantity = _originObservableIOStock.Inventory.Quantity + value - _originObservableIOStock.Quantity;
                            break;

                        case IOStockType.OUTGOING:
                            RemainingQuantity = _originObservableIOStock.RemainingQuantity + _originObservableIOStock.Quantity - value;
                            InventoryQuantity = _originObservableIOStock.Inventory.Quantity + _originObservableIOStock.Quantity - value;
                            break;
                    }
                    break;
            }
        }

        protected override void InitializeProperties(IOStockFormat inoutStockFormat)
        {
            base.InitializeProperties(inoutStockFormat);
            var oid = ObservableInventoryDirector.GetInstance();
            Product = Inventory.Product;
            Inventory = InventoryList.Where(x => x.ID == Inventory.ID).Single();
            InventoryQuantity = Inventory.Quantity;
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

        private void ExecuteRecordCommand()
        {
            Record();
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
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}