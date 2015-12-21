using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0
{
    public class IOStockWrapperViewModel : ViewModelObserver<IOStockWrapper>, INotifyPropertyChanged, IFinderViewModelEvent
    {
        StockType _stockType;
        ObservableCollection<IOStockWrapper> _items;
        IOStockWrapperDirector _director;

        public IOStockWrapperViewModel(StockType type, ViewModelObserverSubject subject) : base(subject)
        {
            _stockType = type;
            _director = IOStockWrapperDirector.GetInstance();
            _items = _director.CreateCollection(type);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override ObservableCollection<IOStockWrapper> Items
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

        public IOStockWrapper SelectedItem { get; set; }

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
                    if (_director.Count() == Items.Count || Items.Any(x => x.Item.UUID == ioStockw.Item.UUID))
                        base.UpdateNewItem(item);
                }
            }
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            FinderViewModel fvm = sender as FinderViewModel;
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
    }
}