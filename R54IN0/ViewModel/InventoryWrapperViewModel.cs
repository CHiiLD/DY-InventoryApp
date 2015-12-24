﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryWrapperViewModel : ItemSourceViewModel<InventoryWrapper>, INotifyPropertyChanged, IFinderViewModelEvent
    {
        ObservableCollection<InventoryWrapper> _items;
        InventoryWrapperDirector _director;

        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler<EventArgs> SelectedItemModifyHandler;
        public EventHandler<EventArgs> NewItemAddHandler;

        public InventoryWrapperViewModel(ViewModelObserverSubject subject) : base(subject)
        {
            _director = InventoryWrapperDirector.GetInstance();
            Items = _director.CreateCollection();
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

        public override void Add(InventoryWrapper item)
        {
            base.Add(item);
            _director.Add(item);
        }

        public override void Remove(InventoryWrapper item)
        {
            base.Remove(item);
            _director.Remove(item);
        }

        public override void UpdateNewItem(object item)
        {
            //만약에 Finder가 일정한 포맷에 묶인 상태에서 새로운 아이템을 추가한다면 그 포맷이 같아야 추가한다.
            if (item is InventoryWrapper)
            {
                InventoryWrapper inventoryWrapper = item as InventoryWrapper;
                if (_director.Count() == Items.Count || Items.Any(x => x.Item.UUID == inventoryWrapper.Item.UUID))
                    base.UpdateNewItem(item);
            }
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            ItemFinderViewModel fvm = sender as ItemFinderViewModel;
            if (fvm != null)
            {
                List<InventoryWrapper> temp = new List<InventoryWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var invens = _director.CreateCollection();
                foreach (var itemNode in itemNodes)
                    temp.AddRange(invens.Where(x => x.Item.UUID == itemNode.ItemUUID));
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
    }
}