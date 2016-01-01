using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace R54IN0
{
    public class StockWrapperDirector
    {
        private static StockWrapperDirector _thiz;
        private List<StockWrapper> _list;
        private MultiSortedDictionary<string, StockWrapper> _itemKeyDic;
        private MultiSortedDictionary<string, StockWrapper> _invenKeyDic;

        public StockWrapperDirector()
        {
            InitCollection();
        }

        public static StockWrapperDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new StockWrapperDirector();
            return _thiz;
        }

        public static void Distory()
        {
            if (_thiz != null)
            {
                if (_thiz._list != null)
                {
                    _thiz._list = null;
                    _thiz._itemKeyDic = null;
                    _thiz._invenKeyDic = null;
                }
                _thiz = null;
            }
        }

        public ObservableCollection<StockWrapper> CreateCollection(StockType type)
        {
            if (type == StockType.NONE)
                throw new ArgumentOutOfRangeException();
            if (type == StockType.ALL)
                return new ObservableCollection<StockWrapper>(_list);
            else
                return new ObservableCollection<StockWrapper>(_list.Where(x => x.StockType == type));
        }

        private void InitCollection()
        {
            _list = new List<StockWrapper>();
            _itemKeyDic = new MultiSortedDictionary<string, StockWrapper>();
            _invenKeyDic = new MultiSortedDictionary<string, StockWrapper>();

            InOutStock[] stocks = null;
            using (var db = LexDb.GetDbInstance())
            {
                stocks = db.LoadAll<InOutStock>();
            }
#if DEBUG
            foreach (var stock in stocks)
            {
                Debug.Assert(stock.ID != null);
                Debug.Assert(stock.ItemID != null);
                Debug.Assert(stock.SpecificationID != null);
                Debug.Assert(stock.StockType != StockType.NONE);
            }
#endif
            _list.AddRange(stocks.Select(x => new StockWrapper(x)));
            _itemKeyDic = new MultiSortedDictionary<string, StockWrapper>();
            foreach (var item in _list)
            {
                _itemKeyDic.Add(item.Item.ID, item);
                _invenKeyDic.Add(item.Inventory.ID, item);
            }
        }

        public bool Contains(StockWrapper item)
        {
#if DEBUG
            Debug.Assert(_list.Contains(item) == _itemKeyDic[item.Item.ID].Contains(item));
#endif
            return _list.Contains(item);
        }

        public void Add(StockWrapper item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                _itemKeyDic.Add(item.Item.ID, item);
                _invenKeyDic.Add(item.Inventory.ID, item);
            }
        }

        public void Remove(StockWrapper item)
        {
            if (Contains(item))
            {
                _list.Remove(item);
                _itemKeyDic.Remove(item.Item.ID, item);
                _invenKeyDic.Remove(item.Inventory.ID, item);
            }
        }

        public int Count(StockType type = StockType.ALL)
        {
            if (type == StockType.NONE)
                throw new ArgumentException();
            if (StockType.ALL == type)
                return _list.Count;
            else
                return _list.Where(x => x.StockType == type).Count();
        }

        public List<StockWrapper> SearchAsItemkey(string ID)
        {
            if (ID == null)
                return null;
            if (!_itemKeyDic.ContainsKey(ID))
                return null;
            return _itemKeyDic[ID];
        }

        public List<StockWrapper> SearchAsInventoryKey(string ID)
        {
            if (ID == null)
                return null;
            if (!_invenKeyDic.ContainsKey(ID))
                return null;
            return _invenKeyDic[ID];
        }
    }
}