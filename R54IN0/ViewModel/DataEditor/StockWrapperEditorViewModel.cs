using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0
{
    public class StockWrapperEditorViewModel : StockWrapperProperties, IFinderViewModelCreatation
    {
        StockWrapperViewModel _viewModel;
        StockWrapper _target;
        IEnumerable<ItemWrapper> _itemList;
        IEnumerable<SpecificationWrapper> _specificationList;
        IEnumerable<Observable<Warehouse>> _warehouseList;

        /// <summary>
        /// 새로운 InOutStock 데이터를 추가하고자 할 때 쓰이는 생성자입니다.
        /// </summary>
        /// <param name="viewModel"></param>
        public StockWrapperEditorViewModel(StockWrapperViewModel viewModel) : base()
        {
            Initialize(viewModel);
        }

        /// <summary>
        /// 기존의 InOutStock 데이터를 수정할 때 쓰이는 생성자입니다.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="ioStockWrapper"></param>
        public StockWrapperEditorViewModel(StockWrapperViewModel viewModel, StockWrapper ioStockWrapper) : base(ioStockWrapper)
        {
            if (ioStockWrapper == null)
                throw new ArgumentNullException();
            _target = ioStockWrapper;
            Initialize(viewModel);

            //수정할 데이터를 변경할 수 없도록 고정
            ItemList = new ItemWrapper[] { _target.Item };
            SpecificationList = new SpecificationWrapper[] { _target.Specification };
            WarehouseList = new Observable<Warehouse>[] { _target.Warehouse };

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
            EmployeeList = fwd.CreateCollection<Employee, Observable<Employee>>().Where(x => !x.IsDeleted);
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
                    SpecificationList = specws.Where(x => x.Field.ItemID == base.Item.ID);
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
                //새로운 InOutStock 데이터를 추가하고자할 때 규격데이터를 선택하였다면 
                if (_target == null && base.Specification != null)
                {
                    //기존의 Inventory 데이터가 존재할 경우 Inventory의 Warehouse 데이터를 고정하고 
                    //없으면 Warehouse 리스트에서 선택하게 한다.
                    InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
                    var inven = iwd.SearchAsSpecificationKey(base.Specification.ID);
                    if (inven != null) //리스트 고정
                    {
                        WarehouseList = new Observable<Warehouse>[] { inven.Warehouse };
                        Warehouse = inven.Warehouse;
                    }
                    else //리스트 업데이트
                    {
                        var fwd = FieldWrapperDirector.GetInstance();
                        WarehouseList = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();
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

        public IEnumerable<Observable<Employee>> EmployeeList { get; private set; }

        public IEnumerable<StockType> StockTypeList { get; private set; }

        public IEnumerable<Observable<Warehouse>> WarehouseList
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

        public FinderViewModel FinderViewModel
        {
            get;
            set;
        }

        /// <summary>
        /// 수정 또는 새로 추가할 데이터를 데이터베이스와 ViewModel의 Items 속성에 각각 추가 및 적용한다.
        /// </summary>
        /// <returns></returns>
        public StockWrapper Update()
        {
            //추가 및 적용의 최소 조건들
            if (Item == null)
                throw new Exception("리스트박스에서 품목을 선택하세요.");
            if (Specification == null)
                throw new Exception("리스트박스에서 규격을 선택하세요.");

            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            InventoryWrapper invenw = iwd.SearchAsSpecificationKey(Specification.ID);
            StockWrapper stockw = null;
            
            if (invenw != null) 
            {
                //재고의 개수를 수정
                invenw.Quantity = InventoryQuantity;
            }
            else 
            {
                //새로운 재고를 등록
                Inventory inven = new Inventory();
                inven.ItemID = this.Item.ID;
                inven.SpecificationID = this.Specification.ID;
                inven.Quantity = this.InventoryQuantity;
                inven.WarehouseID = this.Warehouse != null ? this.Warehouse.ID : null;
                invenw = new InventoryWrapper(inven);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(invenw); //순서의 주의 
                iwd.Add(invenw); //순서의 주의 
            }

            if (_target != null) //InOutStock 데이터의 수정 코드
            {
                _target.Product = InOutStock;
                stockw = _target;
            }
            else //새로운 InOutStock 데이터의 추가 코드
            {
                InOutStock.InventoryID = invenw.ID;
                stockw = new StockWrapper(InOutStock);
                _viewModel.Add(stockw);
            }
            stockw.Product.Save<InOutStock>();
            return stockw;
        }

        /// <summary>
        /// FinderViewModel에서 OnSelecting이벤트의 콜백 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            //기존 Property를 Null로 초기화 하고 해당 품목 데이터의 고유식별자를 사용하여 ItemList를 초기화한다.
            FinderViewModel fvm = sender as FinderViewModel;
            if (fvm != null && _target == null)
            {
                List<ItemWrapper> items = new List<ItemWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var fwd = FieldWrapperDirector.GetInstance();
                foreach (var itemNode in itemNodes)
                    items.Add(fwd.BinSearch<Item, ItemWrapper>(itemNode.ItemID));
                Item = null;
                Specification = null;
                SpecificationList = null;
                ItemList = items; //이 코드를 Item = null; 아래에 두어야 함
            }
            OnPropertyChanged("InventoryQuantity");
        }

        public FinderViewModel CreateFinderViewModel(TreeViewEx treeView)
        {
            var fd = FinderDirector.GetInstance();
            var collection = _target == null ? fd.Collection : new ObservableCollection<FinderNode>();
            FinderViewModel = new FinderViewModel(treeView, new ObservableCollection<FinderNode>(collection));
            FinderViewModel.SelectItemsChanged += OnFinderViewSelectItemChanged;
            return FinderViewModel;
        }
    }
}