using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace R54IN0
{
    public class StockWrapperViewModel : ItemSourceViewModel<StockWrapper>, INotifyPropertyChanged, IFinderViewModelOnSelectingCallback
    {
        private ObservableCollection<StockWrapper> _items;
        private StockType _stockType;

        protected StockWrapperDirector stockDirector;

        public EventHandler<EventArgs> SelectedItemModifyHandler { get; set; }
        public EventHandler<EventArgs> NewItemAddHandler { get; set; }

        public StockWrapperViewModel(StockType type, CollectionViewModelObserverSubject subject) : base(subject)
        {
            _stockType = type;
            stockDirector = StockWrapperDirector.GetInstance();
            _items = stockDirector.CreateCollection(type);
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
            stockDirector.Add(item);
        }

        public override void Remove(StockWrapper item)
        {
            base.Remove(item);
            stockDirector.Remove(item);
        }

#if true

        /// <summary>
        /// 기존의 TDD 코드들과 연동을 위한 메서드
        /// 실제로는 사용하지 아니한다.
        /// </summary>
        /// <param name="item"></param>
        public override void UpdateNewItem(object item)
        {
            if (item is StockWrapper)
            {
                var ioStockw = item as StockWrapper;
                if (_stockType.HasFlag(ioStockw.StockType))
                {
                    //특정 목록이 파인더에 체크되어 있으면서 그 파인더 목록의 데이터가 아무것도 없으면 추가가 되지 아니하는 에러가 있다.
                    if (stockDirector.Count(_stockType) == Items.Count || //모든 목록 모드 이거나
                        Items.Any(x => x.Item.ID == ioStockw.Item.ID))  //특정 모록 모드 이거나
                        base.UpdateNewItem(item);
                }
            }
        }

#endif

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            ItemFinderViewModel fvm = sender as ItemFinderViewModel;
            if (fvm != null)
            {
                //Finder에서 품목 클릭 시, 해당하는 품목들과 관련된 InOutStock 데이터들을 Items에 업데이트한다.
                List<StockWrapper> temp = new List<StockWrapper>();
                var itemNodes = fvm.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                foreach (var itemNode in itemNodes)
                {
                    var stockList = stockDirector.SearchAsItemkey(itemNode.ItemID);
                    if (stockList != null)
                    {
                        if (StockType == StockType.ALL)
                            temp.AddRange(stockList);
                        else if (StockType == StockType.INCOMING || StockType == StockType.OUTGOING)
                            temp.AddRange(stockList.Where(x => x.StockType == StockType));
                    }
                }
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