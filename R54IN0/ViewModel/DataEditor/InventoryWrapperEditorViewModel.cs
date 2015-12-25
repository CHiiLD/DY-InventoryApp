﻿using System;
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

            IEnumerable<ItemWrapper> itemws = fwd.CreateCollection<Item, ItemWrapper>().Where(x => !x.IsDeleted);
            IEnumerable<SpecificationWrapper> specws = fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
            IEnumerable<InventoryWrapper> invenws = iwd.CreateCollection();

            List<ItemWrapper> list = new List<ItemWrapper>();
            //아직 등록하지 아니한 Itemw - Specw 넣기 
            foreach (var specw in specws)
            {
                if (!invenws.Any(x => x.Specification.UUID == specw.UUID))
                {
                    var itemUuid = specw.Field.ItemUUID;
                    var itemw = itemws.Where(x => x.UUID == itemUuid).Single();
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

            ItemList = new ItemWrapper[] { inventoryWrapper.Item };
            SpecificationList = new SpecificationWrapper[] { inventoryWrapper.Specification };

            Item = inventoryWrapper.Item;
            Specification = inventoryWrapper.Specification;
            Warehouse = inventoryWrapper.Warehouse;
            var fwd = FieldWrapperDirector.GetInstance();
            WarehouseList = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>().Where(x => !x.IsDeleted);
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
                    var invenws = iwd.CreateCollection();
                    List<SpecificationWrapper> list = new List<SpecificationWrapper>();
                    foreach (SpecificationWrapper specw in specws)
                    {
                        if (!invenws.Any(x => x.Specification == specw)) //아직 재고현황으로 등록하지 않은 규격 데이터만 추출
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

            if (_target != null) //EIDT인 경우 Inventory만 변경
            {
                if(_target.Warehouse != Warehouse)
                {
                    //Warehouse 동기화
                    var swd = StockWrapperDirector.GetInstance();
                    var ioStockws = swd.CreateCollection(StockType.ALL).Where(x => x.Specification.UUID == Specification.UUID);
                    foreach (var ioStockw in ioStockws)
                        ioStockw.Warehouse = Warehouse;
                }

                var inventory = recordWrapper.Record as Inventory;
                inventory.Delete<Inventory>(); //Clone UUID의 기록을 제거 
                inventory.UUID = _target.Record.UUID; //기존의 UUID를 덮씌운 다음에
                inventory.Save<Inventory>(); //새로 저장
                _target.Record = inventory; //inventory를 변경해버림

                return _target;
            }
            else
            {
                //ADD 인 경우 그냥 추가 
                var target = recordWrapper as InventoryWrapper;
                _viewModel.Add(target);
                return target;
            }
        }
    }
}