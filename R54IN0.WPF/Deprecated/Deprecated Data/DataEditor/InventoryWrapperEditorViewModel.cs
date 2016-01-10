using System;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0
{
    public class InventoryWrapperEditorViewModel : InventoryWrapperProperties
    {
        private InventoryWrapper _target;
        private InventoryWrapperViewModel _viewModel;

        /// <summary>
        /// 새로운 Inventory 데이터를 추가하고자 할 생성자
        /// </summary>
        /// <param name="viewModel"></param>
        public InventoryWrapperEditorViewModel(InventoryWrapperViewModel viewModel) : base()
        {
            _viewModel = viewModel;
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();

            //아직 Inventory 데이터가 없는 Item(With Specification) 데이터의 리스트를 구한다.
            IEnumerable<SpecificationWrapper> specifications = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
            List<ItemWrapper> itemWrapperList = new List<ItemWrapper>();
            foreach (SpecificationWrapper specification in specifications)
            {
                if (iwd.SearchAsSpecificationKey(specification.ID) == null)
                {
                    ItemWrapper itemw = fwd.BinSearch<Item, ItemWrapper>(specification.Field.ItemID);
                    if (itemw != null && !itemw.IsDeleted)
                        itemWrapperList.Add(itemw);
                }
            }
            ItemList = itemWrapperList.Distinct(); //겹치는 데이터를 제거하여 ItemList에 대입

            WarehouseList = fwd.CreateCollection<Warehouse, Observable<Warehouse>>().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// 기존의 Inventory 데이터를 수정하고자 할 생성자
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="inventoryWrapper">수정하고자할 inventory 래핑 클래스</param>
        public InventoryWrapperEditorViewModel(InventoryWrapperViewModel viewModel, InventoryWrapper inventoryWrapper) : base(inventoryWrapper)
        {
            if (inventoryWrapper == null)
                throw new ArgumentNullException();

            _viewModel = viewModel;
            _target = inventoryWrapper;

            Specification = inventoryWrapper.Specification;
            Item = inventoryWrapper.Item;
            Warehouse = inventoryWrapper.Warehouse;

            ItemList = new ItemWrapper[] { Item };
            SpecificationList = new SpecificationWrapper[] { Specification };
            WarehouseList = FieldWrapperDirector.GetInstance().CreateCollection<Warehouse, Observable<Warehouse>>().Where(x => !x.IsDeleted);
        }

        public IEnumerable<ItemWrapper> ItemList
        {
            get;
            set;
        }

        public IEnumerable<Observable<Warehouse>> WarehouseList
        {
            get;
            set;
        }

        public IEnumerable<SpecificationWrapper> SpecificationList
        {
            get;
            set;
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
                if (_target == null)
                {
                    //Inventory 데이터가 없거나, 아직 Inventory에 등록되지 않은 Specification을 모아 SpecificationList에 할당한다.
                    var fwd = FieldWrapperDirector.GetInstance();
                    var iwd = InventoryWrapperDirector.GetInstance();
                    IEnumerable<SpecificationWrapper> specws = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
                    specws = specws.Where(x => x.Field.ItemID == base.Item.ID);
                    List<SpecificationWrapper> list = new List<SpecificationWrapper>();
                    List<InventoryWrapper> invenws = iwd.SearchAsItemKey(base.Item.ID);
                    foreach (SpecificationWrapper specw in specws)
                    {
                        if (invenws == null || invenws.All(x => x.Specification != specw))
                            list.Add(specw);
                    }
                    SpecificationList = list;
                    OnPropertyChanged("SpecificationList");
                }
            }
        }

        public InventoryWrapper Update()
        {
            if (Item == null)
                throw new Exception("리스트박스에서 품목을 선택하세요.");
            if (Specification == null)
                throw new Exception("리스트박스에서 규격을 선택하세요.");

            Inventory inven = Stock as Inventory;
            InventoryWrapper invenw = null;
            if (_target != null) //기존 데이터를 수정하고자 할 경우
            {
                _target.Product = inven;
                invenw = _target;
            }
            else //새로운 데이터를 추가하고자 할 경우
            {
                invenw = new InventoryWrapper(inven);
                _viewModel.Add(invenw);
            }
            invenw.Product.Save<Inventory>();
            return invenw;
        }
    }
}