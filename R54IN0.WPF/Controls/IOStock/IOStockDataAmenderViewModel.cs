using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : ObservableIOStock
    {
        private enum Mode
        {
            ADD,
            MODIFY
        }

        public class NonSaveObservableInventory : ObservableInventory
        {
            public NonSaveObservableInventory(InventoryFormat inventory) : base(inventory)
            {
            }

            public override void NotifyPropertyChanged(string propertyName)
            {
                if (propertyChanged != null)
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private IObservableIOStockProperties _origin;
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
        }

        public IOStockDataAmenderViewModel(IObservableIOStockProperties observableInoutStock) : base(
            new IOStockFormat(observableInoutStock.Format))
        {
            _mode = Mode.MODIFY;
            StockType = observableInoutStock.StockType;
            Initialize();
            _origin = observableInoutStock;

            IsReadOnlyProductTextBox = true;
            IsReadOnlySpecificationMemoTextBox = false;

            IsEnabledMakerComboBox = true;
            IsEnabledMeasureComboBox = true;
            IsEnabledSpecificationComboBox = false;

            IsEnabledOutGoingRadioButton = false;
            IsEnabledInComingRadioButton = false;
        }

        private void Initialize()
        {
            TreeViewViewModel = new ProductSelectorViewModel();
            TreeViewViewModel.PropertyChanged += OnTreeViewModelPropertyChanged;
            ProductSearchCommand = new CommandHandler(ExecuteProductSearchCommand, CanSearch);
            RecordCommand = new CommandHandler(ExecuteRecordCommand, CanRecord);
            ProductSelectCommand = new CommandHandler(ExecuteProductSelectCommand, CanSelectProduct);
        }

        protected override void InitializeProperties(IOStockFormat inoutStockFormat)
        {
            base.InitializeProperties(inoutStockFormat);
            var oid = ObservableInventoryDirector.GetInstance();
            Product = Inventory.Product;
            Inventory = InventoryList.Where(x => x.ID == Inventory.ID).Single();
            //Debug.Assert(InventoryList.Contains(Inventory));
        }

        private void UpdateRecordCommand()
        {
            var cmd = RecordCommand as CommandHandler;
            if (cmd != null)
                cmd.UpdateCanExecute();
        }

        public ObservableIOStock Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            if (Client == null && !string.IsNullOrEmpty(ClientText)) //거래처
            {
                switch (StockType)
                {
                    case IOStockType.INCOMING:
                        Supplier = new Observable<Supplier>() { Name = ClientText };
                        ofd.Add<Supplier>(Supplier);
                        break;

                    case IOStockType.OUTGOING:
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
                ObservableInventoryDirector.GetInstance().Add(inven as ObservableInventory);
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

            ObservableIOStock result = null;
            switch (_mode)
            {
                case Mode.ADD:
                    result = new ObservableIOStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    result = _origin as ObservableIOStock;
                    result.Format = Format;
                    break;
            }
            result.Format.Save<IOStockFormat>();
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