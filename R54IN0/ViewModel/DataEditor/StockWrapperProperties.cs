using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class StockWrapperProperties : InventoryWrapperProperties, IStockEditorViewModelProperties
    {
        StockWrapper _ioStockWrapper;
        StockWrapper _target;

        public StockWrapperProperties() : base(null)
        {
            InOutStock stock = new InOutStock();
            recordWrapper = _ioStockWrapper = new StockWrapper(stock);
            Date = DateTime.Now;
            Quantity = 1;
        }

        public StockWrapperProperties(StockWrapper ioStockWrapper) : base(null)
        {
            var clone = ioStockWrapper.Record.Clone() as InOutStock;
            recordWrapper = _ioStockWrapper = new StockWrapper(clone);
            Date = DateTime.Now;
            _target = ioStockWrapper;
        }

        public ClientWrapper Client
        {
            get
            {
                return _ioStockWrapper.Client;
            }
            set
            {
                _ioStockWrapper.Client = value;
                OnPropertyChanged("Client");
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

        public override int Quantity
        {
            get
            {
                return base.Quantity;
            }
            set
            {
                base.Quantity = value;
                OnPropertyChanged("InventoryQuantity");
            }
        }

        public int InventoryQuantity
        {
            get
            {
                if (StockType == StockType.NONE || StockType == StockType.ALL)
                    throw new Exception();
                if (Specification == null)
                    return Quantity;
                var iwd = InventoryWrapperDirector.GetInstance();
                var invenw = iwd.CreateCollection().Where(x => x.Specification.UUID == Specification.UUID).SingleOrDefault();
                if (invenw == null)
                    return Quantity;
                if (_target == null)
                    return StockType == StockType.INCOMING ? invenw.Quantity + Quantity : invenw.Quantity - Quantity;
                else
                    return StockType == StockType.INCOMING ? invenw.Quantity - _target.Quantity + Quantity : invenw.Quantity + _target.Quantity - Quantity;
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
