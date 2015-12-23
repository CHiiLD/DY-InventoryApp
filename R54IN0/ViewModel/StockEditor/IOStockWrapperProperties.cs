using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class IOStockWrapperProperties : InventoryWrapperProperties, IIOStockInfoProperties
    {
        IOStockWrapper _ioStockWrapper;
        IOStockWrapper _target;

        public IOStockWrapperProperties() : base(null)
        {
            InOutStock stock = new InOutStock();
            recordWrapper = _ioStockWrapper = new IOStockWrapper(stock);
            Date = DateTime.Now;
        }

        public IOStockWrapperProperties(IOStockWrapper ioStockWrapper) : base(null)
        {
            var clone = ioStockWrapper.Record.Clone() as InOutStock;
            recordWrapper = _ioStockWrapper = new IOStockWrapper(clone);
            Date = DateTime.Now;
            _target = ioStockWrapper;
        }

        public AccountWrapper Account
        {
            get
            {
                return _ioStockWrapper.Account;
            }
            set
            {
                _ioStockWrapper.Account = value;
                OnPropertyChanged("Account");
            }
        }

        public DateTime Date
        {
            get
            {
                return _ioStockWrapper.Date;
            }
            set
            {
                _ioStockWrapper.Date = value;
                OnPropertyChanged("Date");
            }
        }

        public FieldWrapper<Employee> Employee
        {
            get
            {
                return _ioStockWrapper.Employee;
            }
            set
            {
                _ioStockWrapper.Employee = value;
                OnPropertyChanged("Employee");
            }
        }

        public override int ItemCount
        {
            get
            {
                return base.ItemCount;
            }
            set
            {
                base.ItemCount = value;
                OnPropertyChanged("InventoryItemCount");
            }
        }

        public int InventoryItemCount
        {
            get
            {
                if (StockType == StockType.NONE || StockType == StockType.ALL)
                    throw new Exception();
                if (Specification == null)
                    return ItemCount;
                var iwd = InventoryWrapperDirector.GetInstance();
                var invenw = iwd.CreateCollection().Where(x => x.Specification.UUID == Specification.UUID).SingleOrDefault();
                if (invenw == null)
                    return ItemCount;
                if (_target == null)
                    return StockType == StockType.IN ? invenw.ItemCount + ItemCount : invenw.ItemCount - ItemCount;
                else
                    return StockType == StockType.IN ? invenw.ItemCount - _target.ItemCount + ItemCount : invenw.ItemCount + _target.ItemCount - ItemCount;
            }
        }

        public string Remark
        {
            get
            {
                return _ioStockWrapper.Remark;
            }
            set
            {
                _ioStockWrapper.Remark = value;
                OnPropertyChanged("Remark");
            }
        }

        public StockType StockType
        {
            get
            {
                return _ioStockWrapper.StockType;
            }
            set
            {
                _ioStockWrapper.StockType = value;
                OnPropertyChanged("StockType");
            }
        }
    }
}
