using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryWrapperViewModel : ViewModelObserver<InventoryWrapper>, INotifyPropertyChanged, IFinderViewModelEvent
    {
        ObservableCollection<InventoryWrapper> _items;
        InventoryWrapperDirector _inventoryWrapperDirector;

        public event PropertyChangedEventHandler PropertyChanged;

        public InventoryWrapperViewModel(ViewModelObserverSubject subject) : base(subject)
        {
            _inventoryWrapperDirector = InventoryWrapperDirector.GetInstance();
            Items = _inventoryWrapperDirector.CreateCollection();
            SelectedItem = Items.FirstOrDefault();
        }

        public override ObservableCollection<InventoryWrapper> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }

        public InventoryWrapper SelectedItem { get; set; }

        public override void Add(InventoryWrapper item)
        {
            var coll = _inventoryWrapperDirector.CreateCollection();
            var origin = coll.Where(x => x.UUID == item.UUID && x != item).FirstOrDefault();
            if (origin != null) //EIDT인 경우
                Remove(origin); //원본을 삭제하고 
            base.Add(item); //새로운 아이템으로 대체
            _inventoryWrapperDirector.Add(item);
        }

        public override void Remove(InventoryWrapper item)
        {
            base.Remove(item);
            _inventoryWrapperDirector.Remove(item);
        }

        public override void UpdateNewItem(object item)
        {
            //만약에 Finder가 일정한 포맷에 묶인 상태에서 새로운 아이템을 추가한다면 그 포맷이 같아야 추가한다.
            if (item is InventoryWrapper)
            {
                InventoryWrapper inventoryWrapper = item as InventoryWrapper;
                if (_inventoryWrapperDirector.Count() == Items.Count || Items.Any(x => x.Item.UUID == inventoryWrapper.Item.UUID))
                    base.UpdateNewItem(item);
            }
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            FinderViewModel fvm = sender as FinderViewModel;
            if (fvm != null)
            {
                List<InventoryWrapper> temp = new List<InventoryWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var invens = _inventoryWrapperDirector.CreateCollection();
                foreach (var itemNode in itemNodes)
                    temp.AddRange(invens.Where(x => x.Item.UUID == itemNode.ItemUUID));
                Items = new ObservableCollection<InventoryWrapper>(temp);
            }
        }
    }
}