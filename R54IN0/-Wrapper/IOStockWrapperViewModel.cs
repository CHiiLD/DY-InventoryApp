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
        IOStockWrapperDirector _ioStockWrapperDirector;

        public IOStockWrapperViewModel(StockType type, ViewModelObserverSubject subject) : base(subject)
        {
            _stockType = type;
            _ioStockWrapperDirector = IOStockWrapperDirector.GetInstance();
            _items = _ioStockWrapperDirector.CreateCollection(type);
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
            _ioStockWrapperDirector.Add(item);
        }

        public override void Remove(IOStockWrapper item)
        {
            base.Remove(item);
            _ioStockWrapperDirector.Remove(item);
        }

        public override void UpdateNewItem(object item)
        {
            if (item is IOStockWrapper)
            {
                var ioStockw = item as IOStockWrapper;
                if (_stockType.HasFlag(ioStockw.StockType))
                {
                    if (_ioStockWrapperDirector.Count() == Items.Count || Items.Any(x => x.Item.UUID == ioStockw.Item.UUID))
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
                var ioStockws = _ioStockWrapperDirector.CreateCollection(_stockType);
                foreach (var itemNode in itemNodes)
                    temp.AddRange(ioStockws.Where(x => x.Item.UUID == itemNode.ItemUUID));
                Items = new ObservableCollection<IOStockWrapper>(temp);
            }
        }
    }
}