using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace R54IN0
{
    public class InOutStockPipeCollectionDirector : IDirectorAction
    {
        static InOutStockPipeCollectionDirector _thiz;
        Dictionary<StockType, ObservableCollection<IOStockWrapper>> _ioDic;

        InOutStockPipeCollectionDirector()
        {
            _ioDic = new Dictionary<StockType, ObservableCollection<IOStockWrapper>>();
            _ioDic[StockType.ALL] = new ObservableCollection<IOStockWrapper>();
            _ioDic[StockType.IN] = new ObservableCollection<IOStockWrapper>();
            _ioDic[StockType.OUT] = new ObservableCollection<IOStockWrapper>();

            using (var db = DatabaseDirector.GetDbInstance())
            {
                IEnumerable<InOutStock> stocks = db.LoadAll<InOutStock>();
                foreach (InOutStock item in stocks)
                    _ioDic[StockType.ALL].Add(new IOStockWrapper(item));
                foreach (IOStockWrapper item in _ioDic[StockType.ALL])
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

        public void Add(object pipe)
        {
            if (!(pipe is IOStockWrapper))
                return;

            IOStockWrapper newPipe = pipe as IOStockWrapper;

            if (!_ioDic[StockType.ALL].Contains(newPipe))
                _ioDic[StockType.ALL].Add(newPipe);

            if (!_ioDic[newPipe.StockType].Contains(newPipe))
                _ioDic[newPipe.StockType].Add(newPipe);
        }

        public void Remove(object pipe)
        {
            if (!(pipe is IOStockWrapper))
                return;

            IOStockWrapper oldPipe = pipe as IOStockWrapper;

            if (_ioDic[StockType.ALL].Contains(oldPipe))
                _ioDic[StockType.ALL].Remove(oldPipe);

            if (_ioDic[oldPipe.StockType].Contains(oldPipe))
                _ioDic[oldPipe.StockType].Remove(oldPipe);
        }

        public static InOutStockPipeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new InOutStockPipeCollectionDirector();
            return _thiz;
        }

        public ObservableCollection<IOStockWrapper> NewPipe(StockType type)
        {
            return new ObservableCollection<IOStockWrapper>(_ioDic[type]);
        }
    }
}