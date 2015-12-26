using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace R54IN0
{
    public class StockWrapperDirector
    {
        static StockWrapperDirector _thiz;
        List<StockWrapper> _list;
        MultiSortedDictionary<string, StockWrapper> _itemKeyDic;
        MultiSortedDictionary<string, StockWrapper> _invenKeyDic;

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
            using (var db = DatabaseDirector.GetDbInstance())
            {
                stocks = db.LoadAll<InOutStock>();
            }
#if DEBUG
            foreach (var stock in stocks)
            {
                Debug.Assert(stock.UUID != null);
                Debug.Assert(stock.ItemUUID != null);
                Debug.Assert(stock.SpecificationUUID != null);
                Debug.Assert(stock.StockType != StockType.NONE);
            }
#endif
            _list.AddRange(stocks.Select(x => new StockWrapper(x)));
            _itemKeyDic = new MultiSortedDictionary<string, StockWrapper>();
            foreach (var item in _list)
            {
                _itemKeyDic.Add(item.Item.UUID, item);
                _invenKeyDic.Add(item.Inventory.UUID, item);
            }
        }

        public bool Contains(StockWrapper item)
        {
#if DEBUG
            Debug.Assert(_list.Contains(item) == _itemKeyDic[item.Item.UUID].Contains(item));
#endif
            return _list.Contains(item);
        }

        public void Add(StockWrapper item)
        {
            if (!Contains(item))
            {
                _list.Add(item);
                _itemKeyDic.Add(item.Item.UUID, item);
                _invenKeyDic.Add(item.Inventory.UUID, item);
            }
        }

        public void Remove(StockWrapper item)
        {
            if (Contains(item))
            {
                _list.Remove(item);
                _itemKeyDic.Remove(item.Item.UUID, item);
                _invenKeyDic.Remove(item.Inventory.UUID, item);
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

        public List<StockWrapper> SearchAsItemkey(string uuid)
        {
            if (uuid == null)
                return null;
            if (!_itemKeyDic.ContainsKey(uuid))
                return null;
            return _itemKeyDic[uuid];
        }

        public List<StockWrapper> SearchAsInventoryKey(string uuid)
        {
            if (uuid == null)
                return null;
            if (!_invenKeyDic.ContainsKey(uuid))
                return null;
            return _invenKeyDic[uuid];
        }
    }
}