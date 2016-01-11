using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        IOStockStatusViewModel _ioStockStatusViewModel;
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
        }

        /// <summary>
        /// TEST용 생성자
        /// </summary>
        /// <param name="observableInoutStock"></param>
        public IOStockDataAmenderViewModel(IObservableIOStockProperties observableInoutStock) :
            base(new IOStockFormat(observableInoutStock.Format))
        {
            _mode = Mode.MODIFY;
            StockType = observableInoutStock.StockType;
            Initialize();
            _originObservableIOStock = observableInoutStock;

            IsReadOnlyProductTextBox = true;
            IsReadOnlySpecificationMemoTextBox = false;

            IsEnabledMakerComboBox = true;
            IsEnabledMeasureComboBox = true;
            IsEnabledSpecificationComboBox = false;

            IsEnabledOutGoingRadioButton = false;
            IsEnabledInComingRadioButton = false;
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
            InventoryQuantity = Inventory.Quantity;
        }

        private void UpdateRecordCommand()
        {
            var cmd = RecordCommand as CommandHandler;
            if (cmd != null)
                cmd.UpdateCanExecute();
        }

        public IObservableIOStockProperties Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");

            CreateObservableFields();
            ApplyModifiedInventoryProperties();
            UpdateModifiedStockQuantity();

            ObservableIOStock result = null;
            switch (_mode)
            {
                case Mode.ADD:
                    result = new ObservableIOStock(Format.Save<IOStockFormat>());
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    result = _originObservableIOStock as ObservableIOStock;
                    result.Format = Format;
                    break;
            }
            result.Format.Save<IOStockFormat>();
            return result;
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private void ApplyModifiedInventoryProperties()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            if (Inventory == null)
            {
                ObservableInventory obInventory = new ObservableInventory(new InventoryFormat().Save<InventoryFormat>());
                obInventory.Product = Product != null ? Product : new Observable<Product>() { Name = ProductText };
                obInventory.Specification = SpecificationText;
                obInventory.Memo = _specificationMemo;
                if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                {
                    obInventory.Maker = new Observable<Maker>() { Name = MakerText };
                    ofd.Add<Maker>(obInventory.Maker);
                }
                else if (Maker != null)
                {
                    obInventory.Maker = Maker;
                }

                if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                {
                    obInventory.Measure = new Observable<Measure>() { Name = MeasureText };
                    ofd.Add<Measure>(obInventory.Measure);
                }
                else if (Measure != null)
                {
                    obInventory.Measure = Measure;
                }
                obInventory.Quantity = InventoryQuantity;
                oid.Add(obInventory as ObservableInventory);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(obInventory);
                Inventory = obInventory;
            }
            else
            {
                ObservableInventory originInventory = oid.Search(Inventory.ID);
                if (originInventory != null)
                {
                    if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                    {
                        originInventory.Maker = new Observable<Maker>() { Name = MakerText };
                        ofd.Add<Maker>(originInventory.Maker);
                    }
                    else if (originInventory.Maker != Maker)
                    {
                        originInventory.Maker = Maker;
                    }
                    if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                    {
                        originInventory.Measure = new Observable<Measure>() { Name = MeasureText };
                        ofd.Add<Measure>(originInventory.Measure);
                    }
                    else if (originInventory.Measure != Measure)
                    {
                        originInventory.Measure = Measure;
                    }
                    if (originInventory.Memo != SpecificationMemo)
                        originInventory.Memo = SpecificationMemo;
                    originInventory.Quantity = InventoryQuantity;
                }
                originInventory.Quantity = InventoryQuantity;
            }
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </summary>
        private void CreateObservableFields()
        {
            var ofd = ObservableFieldDirector.GetInstance();
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
        }

        /// <summary>
        /// 입출고 데이터를 새로 추가하는 경우 또는 과거의 데이터를 수정할 경우 입출고 수량에 변화가 있다면 
        /// 관련 IOStock 데이터들의 잔여수량 및 재고수량을 다시 계산하여 전부 업데이트하고 Owner의 DataGridItems 역시 변화된 값들을 반영하게 한다.
        /// TODO
        /// </summary>
        private void UpdateModifiedStockQuantity()
        {
            //using (var db = LexDb.GetDbInstance())
            //{
            //    var formats = db.Table<IOStockFormat>().IndexQueryByKey("InventoryID", Inventory.ID).ToList();
            //    formats.Where(x => x.Date > Date);

            //}
        }

        private void OnTreeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
            {
                var cmd = ProductSelectCommand as CommandHandler;
                cmd.UpdateCanExecute();
            }
        }

        private bool CanSelectProduct(object arg)
        {
            return TreeViewViewModel.SelectedNodes.Count == 1 && TreeViewViewModel.SelectedNodes.Single().Type == NodeType.PRODUCT;
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