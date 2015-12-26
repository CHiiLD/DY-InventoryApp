using System;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0
{
    public class InventoryWrapperEditorViewModel : InventoryWrapperProperties
    {
        InventoryWrapper _target;
        InventoryWrapperViewModel _viewModel;

        public InventoryWrapperEditorViewModel(InventoryWrapperViewModel viewModel) : base()
        {
            _viewModel = viewModel;
            var fwd = FieldWrapperDirector.GetInstance();
            var iwd = InventoryWrapperDirector.GetInstance();

            IEnumerable<SpecificationWrapper> specifications = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
            List<ItemWrapper> list = new List<ItemWrapper>();
            //아직 Inventory 데이터가 없는 Item(With Specification) 데이터의 리스트를 구한다.
            foreach (var specw in specifications)
            {
                if(iwd.SearchAsSpecificationKey(specw.UUID) == null)
                {
                    var itemw = fwd.BinSearch<Item, ItemWrapper>(specw.Field.ItemUUID);
                    if (itemw != null && !itemw.IsDeleted)
                        list.Add(itemw);
                }
            }
            ItemList = list.Distinct();
            WarehouseList = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>().Where(x => !x.IsDeleted);
        }

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
            WarehouseList = FieldWrapperDirector.GetInstance().CreateCollection<Warehouse, FieldWrapper<Warehouse>>().Where(x => !x.IsDeleted);
        }

        public IEnumerable<ItemWrapper> ItemList
        {
            get;
            set;
        }

        public IEnumerable<FieldWrapper<Warehouse>> WarehouseList
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
                    var fwd = FieldWrapperDirector.GetInstance();
                    var iwd = InventoryWrapperDirector.GetInstance();
                    var specws = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
                    specws = specws.Where(x => x.Field.ItemUUID == base.Item.UUID); //품목에 해당하는 규격 데이터를 추출
                    List<SpecificationWrapper> list = new List<SpecificationWrapper>();
                    List<InventoryWrapper> invenws = iwd.SearchAsItemKey(base.Item.UUID);
                    foreach (SpecificationWrapper specw in specws)
                    {
                        if (invenws == null || invenws.All(x => x.Specification != specw)) //아직 재고현황으로 등록하지 않은 규격 데이터만 추출
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

            InventoryWrapper result;

            if (_target != null) //EIDT
            {
                _target.Product = Stock as Inventory;
                result = _target;
            }
            else
            {
                result = new InventoryWrapper(Stock as Inventory);
                _viewModel.Add(result);
            }
            return result;
        }
    }
}