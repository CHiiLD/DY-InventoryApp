using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0
{
    public class StockWrapperEditorViewModel : StockWrapperProperties, IFinderViewModelCallback
    {
        StockWrapperViewModel _viewModel;
        StockWrapper _target;
        IEnumerable<ItemWrapper> _itemList;
        IEnumerable<SpecificationWrapper> _specificationList;
        IEnumerable<FieldWrapper<Warehouse>> _warehouseList;

        public StockWrapperEditorViewModel(StockWrapperViewModel viewModel) : base()
        {
            Initialize(viewModel);
        }

        public StockWrapperEditorViewModel(StockWrapperViewModel viewModel, StockWrapper ioStockWrapper) : base(ioStockWrapper)
        {
            if (ioStockWrapper == null)
                throw new ArgumentNullException();
            _target = ioStockWrapper;
            Initialize(viewModel);
            
            ItemList = new ItemWrapper[] { _target.Item };
            SpecificationList = new SpecificationWrapper[] { _target.Specification };
            WarehouseList = new FieldWrapper<Warehouse>[] { _target.Warehouse };

            Item = ItemList.First();
            Specification = SpecificationList.First();
            Warehouse = WarehouseList.First();
        }

        public void Initialize(StockWrapperViewModel viewModel)
        {
            _viewModel = viewModel;
            var fwd = FieldWrapperDirector.GetInstance();
            StockTypeList = viewModel.StockType == StockType.ALL ?
                new StockType[] { StockType.INCOMING, StockType.OUTGOING } : new StockType[] { viewModel.StockType };
            ClientList = fwd.CreateCollection<Client, ClientWrapper>().Where(x => !x.IsDeleted);
            EmployeeList = fwd.CreateCollection<Employee, FieldWrapper<Employee>>().Where(x => !x.IsDeleted);
            StockType = viewModel.StockType == StockType.ALL ? StockType.INCOMING : viewModel.StockType;
        }

        public override ItemWrapper Item
        {
            get
            {
                return base.Item;
            }
            set
            {
                base.Item = value;
                if (_target == null && base.Item != null)
                {
                    Specification = null;
                    var fwd = FieldWrapperDirector.GetInstance();
                    var specws = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
                    SpecificationList = specws.Where(x => x.Field.ItemUUID == base.Item.UUID);
                }
            }
        }

        public override SpecificationWrapper Specification
        {
            get
            {
                return base.Specification;
            }
            set
            {
                base.Specification = value;
                if (_target == null && base.Specification != null)
                {
                    InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
                    var inven = iwd.SearchAsSpecificationKey(base.Specification.UUID);
                    if (inven != null)
                    {
                        WarehouseList = new FieldWrapper<Warehouse>[] { inven.Warehouse };
                        Warehouse = inven.Warehouse;
                    }
                    else
                    {
                        var fwd = FieldWrapperDirector.GetInstance();
                        WarehouseList = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();
                        Warehouse = null;
                    }
                }
            }
        }

        public IEnumerable<ItemWrapper> ItemList
        {
            get
            {
                return _itemList;
            }
            set
            {
                _itemList = value;
                OnPropertyChanged("ItemList");
            }
        }

        public IEnumerable<SpecificationWrapper> SpecificationList
        {
            get
            {
                return _specificationList;
            }
            set
            {
                _specificationList = value;
                OnPropertyChanged("SpecificationList");
            }
        }

        public IEnumerable<ClientWrapper> ClientList { get; private set; }

        public IEnumerable<FieldWrapper<Employee>> EmployeeList { get; private set; }

        public IEnumerable<StockType> StockTypeList { get; private set; }

        public IEnumerable<FieldWrapper<Warehouse>> WarehouseList
        {
            get
            {
                return _warehouseList;
            }
            set
            {
                _warehouseList = value;
                OnPropertyChanged("WarehouseList");
            }
        }

        public StockWrapper Update()
        {
            if (Item == null)
                throw new Exception("리스트박스에서 품목을 선택하세요.");
            if (Specification == null)
                throw new Exception("리스트박스에서 규격을 선택하세요.");

            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            InventoryWrapper invenw = iwd.SearchAsSpecificationKey(Specification.UUID);
            if (invenw != null) //재고의 개수를 수정
            {
                invenw.Quantity = InventoryQuantity;
            }
            else //새로운 재고를 등록
            {
                Inventory inven = new Inventory();
                InventoryWrapper newInvenw = new InventoryWrapper(inven)
                {
                    Item = this.Item,
                    Specification = this.Specification,
                    Quantity = this.InventoryQuantity,
                    Warehouse = this.Warehouse,
                };
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(newInvenw); //순서의 주의 
                iwd.Add(newInvenw); //순서의 주의 
            }

            StockWrapper result;
            if (_target != null) //변경
            {
                _target.Product = IOStock;
                result = _target;
            }
            else //추가
            {
                result = new StockWrapper(IOStock);
                _viewModel.Add(result);
            }
            return result;
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            FinderViewModel fvm = sender as FinderViewModel;
            if (fvm != null && _target == null)
            {
                List<ItemWrapper> items = new List<ItemWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var fwd = FieldWrapperDirector.GetInstance();
                foreach (var itemNode in itemNodes)
                    items.Add(fwd.BinSearch<Item, ItemWrapper>(itemNode.ItemUUID));
                Item = null;
                Specification = null;
                SpecificationList = null;
                ItemList = items; //이 코드를 Item = null; 아래에 두어야 함
            }
        }

        public FinderViewModel CreateFinderViewModel(TreeViewEx treeView)
        {
            var fd = FinderDirector.GetInstance();
            var collection = _target == null ? fd.Collection : new ObservableCollection<FinderNode>();
            FinderViewModel fvm = new FinderViewModel(treeView, new ObservableCollection<FinderNode>(collection));
            fvm.SelectItemsChanged += OnFinderViewSelectItemChanged;
            return fvm;
        }
    }
}