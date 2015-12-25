﻿using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;

namespace R54IN0
{
    public class StockWrapperViewModel : ItemSourceViewModel<StockWrapper>, INotifyPropertyChanged, IFinderViewModelCallback
    {
        StockType _stockType;
        ObservableCollection<StockWrapper> _items;
        StockWrapperDirector _director;
        //StockWrapper _selectedItem;

        public EventHandler<EventArgs> SelectedItemModifyHandler;
        public EventHandler<EventArgs> NewItemAddHandler;

        public StockWrapperViewModel(StockType type, CollectionViewModelObserverSubject subject) : base(subject)
        {
            _stockType = type;
            _director = StockWrapperDirector.GetInstance();
            _items = _director.CreateCollection(type);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StockType StockType
        {
            get
            {
                return _stockType;
            }
        }

        public override ObservableCollection<StockWrapper> Items
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

        public override StockWrapper SelectedItem
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

        public override void Add(StockWrapper item)
        {
            base.Add(item);
            _director.Add(item);
        }

        public override void Remove(StockWrapper item)
        {
            base.Remove(item);
            _director.Remove(item);
        }

        public override void UpdateNewItem(object item)
        {
            if (item is StockWrapper)
            {
                var ioStockw = item as StockWrapper;
                if (_stockType.HasFlag(ioStockw.StockType))
                {
                    if (_director.Count(_stockType) == Items.Count || Items.Any(x => x.Item.UUID == ioStockw.Item.UUID))
                        base.UpdateNewItem(item);
                }
            }
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            ItemFinderViewModel fvm = sender as ItemFinderViewModel;
            if (fvm != null)
            {
                List<StockWrapper> temp = new List<StockWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var ioStockws = _director.CreateCollection(_stockType);
                foreach (var itemNode in itemNodes)
                    temp.AddRange(ioStockws.Where(x => x.Item.UUID == itemNode.ItemUUID));
                Items = new ObservableCollection<StockWrapper>(temp);
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