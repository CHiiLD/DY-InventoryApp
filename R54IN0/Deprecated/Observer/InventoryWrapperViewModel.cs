using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0
{
    public class InventoryWrapperViewModel : ItemSourceViewModel<InventoryWrapper>, INotifyPropertyChanged, IFinderViewModelCreatation
    {
        private ObservableCollection<InventoryWrapper> _items;
        private InventoryWrapperDirector _inventoryDirector;

        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler<EventArgs> SelectedItemModifyHandler { get; set; }
        public EventHandler<EventArgs> NewItemAddHandler { get; set; }

        public InventoryWrapperViewModel(CollectionViewModelObserverSubject subject) : base(subject)
        {
            _inventoryDirector = InventoryWrapperDirector.GetInstance();
            Items = _inventoryDirector.CreateCollection();
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
                OnPropertyChanged("Items");
            }
        }

        public override InventoryWrapper SelectedItem
        {
            get
            {
                return base.SelectedItem;
            }
            set
            {
                base.SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public FinderViewModel FinderViewModel
        {
            get; set;
        }

        public override void Add(InventoryWrapper item)
        {
            base.Add(item);
            _inventoryDirector.Add(item);
        }

        public override void Remove(InventoryWrapper item)
        {
            base.Remove(item);
            _inventoryDirector.Remove(item);
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            ItemFinderViewModel fvm = sender as ItemFinderViewModel;
            if (fvm != null)
            {
                List<InventoryWrapper> temp = new List<InventoryWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT));
                foreach (var itemNode in itemNodes)
                {
                    var inventories = _inventoryDirector.SearchAsItemKey(itemNode.ItemID);
                    if (inventories != null)
                        temp.AddRange(inventories);
                }
                Items = new ObservableCollection<InventoryWrapper>(temp);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public override void ExecuteAddCommand(object parameter)
        {
            if (NewItemAddHandler != null)
                NewItemAddHandler(this, EventArgs.Empty);
        }

        public override void ExecuteModifyCommand(object parameter)
        {
            if (SelectedItemModifyHandler != null)
                SelectedItemModifyHandler(this, EventArgs.Empty);
        }

        public override void ExecuteRemoveCommand(object parameter)
        {
            Remove(SelectedItem);
            SelectedItem = null;
        }

        public FinderViewModel CreateFinderViewModel(TreeViewEx treeView)
        {
            FinderViewModel = new ItemFinderViewModel(treeView);
            FinderViewModel.SelectItemsChanged += OnFinderViewSelectItemChanged;
            return FinderViewModel;
        }

#if DEBUG

        /// <summary>
        /// 이전에 작성한 UnitTest 코드들의 호환성을 위해 사용한다. release에서는 사용하지 아니한다.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateNewItemStub(object item)
        {
            //만약에 Finder가 일정한 포맷에 묶인 상태에서 새로운 아이템을 추가한다면 그 포맷이 같아야 추가한다.
            if (item is InventoryWrapper)
            {
                InventoryWrapper inventoryWrapper = item as InventoryWrapper;
                if (_inventoryDirector.Count() == Items.Count || Items.Any(x => x.Item.ID == inventoryWrapper.Item.ID))
                    base.UpdateNewItem(item);
            }
        }

#endif

        public override void UpdateNewItem(object item)
        {
            if (!(item is InventoryWrapper))
                return;
#if DEBUG
            if (FinderViewModel == null)
            {
                UpdateNewItemStub(item);
                return;
            }
#endif
            InventoryWrapper invenw = item as InventoryWrapper;
            //Items에 모든 데이터가 들어있으며 데이터를 추가한다.
            if (FinderViewModel.SelectedNodes.Count() == 0 && _inventoryDirector.Count() == Items.Count())
            {
                base.UpdateNewItem(item);
                return;
            }
            //Finder의 노드와 관계가 있는 경우 데이터를 추가한다.
            var itemNodes = FinderViewModel.SelectedNodes.SelectMany(rn => rn.Descendants().Where(x => x.Type == NodeType.PRODUCT));
            if (itemNodes.Any(n => n.ItemID == invenw.Item.ID))
                base.UpdateNewItem(item);
        }
    }
}