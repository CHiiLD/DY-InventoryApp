using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;

namespace R54IN0
{
    public class IOStockWrapperViewModel : ItemSourceViewModel<IOStockWrapper>, INotifyPropertyChanged, IFinderViewModelEvent
    {
        StockType _stockType;
        ObservableCollection<IOStockWrapper> _items;
        IOStockWrapperDirector _director;
        IOStockWrapper _selectedItem;

        public EventHandler<EventArgs> SelectedItemModifyHandler;
        public EventHandler<EventArgs> NewItemAddHandler;

        public IOStockWrapperViewModel(StockType type, ViewModelObserverSubject subject) : base(subject)
        {
            _stockType = type;
            _director = IOStockWrapperDirector.GetInstance();
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

        public override ObservableCollection<IOStockWrapper> Items
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

        public override IOStockWrapper SelectedItem
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

        public override void Add(IOStockWrapper item)
        {
            base.Add(item);
            _director.Add(item);
        }

        public override void Remove(IOStockWrapper item)
        {
            base.Remove(item);
            _director.Remove(item);
        }

        public override void UpdateNewItem(object item)
        {
            if (item is IOStockWrapper)
            {
                var ioStockw = item as IOStockWrapper;
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
                List<IOStockWrapper> temp = new List<IOStockWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var ioStockws = _director.CreateCollection(_stockType);
                foreach (var itemNode in itemNodes)
                    temp.AddRange(ioStockws.Where(x => x.Item.UUID == itemNode.ItemUUID));
                Items = new ObservableCollection<IOStockWrapper>(temp);
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