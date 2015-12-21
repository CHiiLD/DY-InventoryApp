using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace R54IN0
{
    public class IOStockWrapperDirector
    {
        static IOStockWrapperDirector _thiz;
        List<IOStockWrapper> _list;

        public IOStockWrapperDirector()
        {
        }

        public static IOStockWrapperDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new IOStockWrapperDirector();
            return _thiz;
        }

        public ObservableCollection<IOStockWrapper> CreateCollection(StockType type)
        {
            if (type == StockType.NONE)
                throw new ArgumentOutOfRangeException();

            if (_list == null)
            {
                _list = new List<IOStockWrapper>();
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    InOutStock[] stocks = db.LoadAll<InOutStock>();
                    _list.AddRange(stocks.Select(x => new IOStockWrapper(x)));
                }
            }
            if (type == StockType.ALL)
                return new ObservableCollection<IOStockWrapper>(_list);
            else
                return new ObservableCollection<IOStockWrapper>(_list.Where(x => x.StockType == type));
        }

        public bool Contains(IOStockWrapper item)
        {
            return _list.Contains(item);
        }

        public void Add(IOStockWrapper item)
        {
            if (!Contains(item))
                _list.Add(item);
        }

        public void Remove(IOStockWrapper item)
        {
            if (Contains(item))
                _list.Remove(item);
        }

        public static void Distory()
        {
            if (_thiz != null)
            {
                if (_thiz._list != null)
                    _thiz._list = null;
                _thiz = null;
            }
        }

        public int Count()
        {
            return _list.Count;
        }
    }
}