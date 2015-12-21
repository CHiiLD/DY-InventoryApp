using System;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0
{
    public class InventoryWrapperViewModelHelper : RecordWrapperViewModelHelper<InventoryWrapper, Inventory>
    {
        InventoryWrapper _target;

        ItemWrapper _selectedItem;

        public InventoryWrapperViewModelHelper(InventoryWrapperViewModel viewModel)
                            : base(viewModel)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            var iwd = InventoryWrapperDirector.GetInstance();

            var itemws = fwd.CreateFieldWrapperCollection<Item, ItemWrapper>().Where(x => !x.IsDeleted);
            var specws = fwd.CreateFieldWrapperCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
            var invenws = iwd.CreateInventoryWrapperCollection();

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
            AllItem = list.Distinct();
        }

        public InventoryWrapperViewModelHelper(InventoryWrapperViewModel viewModel, InventoryWrapper target)
            : base(viewModel, target)
        {
            _target = target;
        }
        public override ItemWrapper SelectedItem
        {
            get
            {
                return _target != null ? base.SelectedItem : _selectedItem;
            }
            set
            {
                if (_target != null)
                    base.SelectedItem = value;
                else if (_target == null && _selectedItem != value)
                {
                    _selectedItem = value;
                    record.ItemUUID = value.UUID;
                    var fwd = FieldWrapperDirector.GetInstance();
                    var iwd = InventoryWrapperDirector.GetInstance();
                    var specws = fwd.CreateFieldWrapperCollection<Specification, SpecificationWrapper>().Where(x => !x.IsDeleted);
                    specws = specws.Where(x => x.Field.ItemUUID == _selectedItem.UUID);
                    var invenws = iwd.CreateInventoryWrapperCollection();
                    List<SpecificationWrapper> list = new List<SpecificationWrapper>();
                    foreach (SpecificationWrapper specw in specws)
                    {
                        if (!invenws.Any(x => x.Specification == specw))
                            list.Add(specw);
                    }
                    AllSpecification = list;
                }
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}