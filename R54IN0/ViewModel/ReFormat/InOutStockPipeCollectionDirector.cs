using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace R54IN0
{
    public class InOutStockPipeCollectionDirector
    {
        static InOutStockPipeCollectionDirector _thiz;
        Dictionary<StockType, ObservableCollection<InOutStockPipe>> _ioDic;

        InOutStockPipeCollectionDirector()
        {
            _ioDic = new Dictionary<StockType, ObservableCollection<InOutStockPipe>>();
            _ioDic[StockType.ALL] = new ObservableCollection<InOutStockPipe>();
            _ioDic[StockType.IN] = new ObservableCollection<InOutStockPipe>();
            _ioDic[StockType.OUT] = new ObservableCollection<InOutStockPipe>();

            using (var db = DatabaseDirector.GetDbInstance())
            {
                IEnumerable<InOutStock> stocks = db.LoadAll<InOutStock>();
                foreach (InOutStock item in stocks)
                    _ioDic[StockType.ALL].Add(new InOutStockPipe(item));
                foreach (InOutStockPipe item in _ioDic[StockType.ALL])
                {
                    if (item.StockType == StockType.IN)
                        _ioDic[StockType.IN].Add(item);
                    else if (item.StockType == StockType.OUT)
                        _ioDic[StockType.OUT].Add(item);
                    else
                        throw new NotSupportedException();
                }
            }
        }

        public void AddNewItem(InOutStockPipe newPipe)
        {
            if (!_ioDic[StockType.ALL].Contains(newPipe))
                _ioDic[StockType.ALL].Add(newPipe);
            if (!_ioDic[newPipe.StockType].Contains(newPipe))
                _ioDic[newPipe.StockType].Add(newPipe);
        }

        public void RemoveItem(InOutStockPipe pipe)
        {
            if (_ioDic[StockType.ALL].Contains(pipe))
                _ioDic[StockType.ALL].Remove(pipe);
            if (_ioDic[pipe.StockType].Contains(pipe))
                _ioDic[pipe.StockType].Remove(pipe);
        }

        public static InOutStockPipeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new InOutStockPipeCollectionDirector();
            return _thiz;
        }

        public ObservableCollection<InOutStockPipe> LoadPipe(StockType type)
        {
            return _ioDic[type];
        }
    }
}