using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class StockWrapperProperties : InventoryWrapperProperties, IStockViewModelProperties
    {
        StockWrapper _target;
        FieldWrapper<Employee> _employee;
        ClientWrapper _client;

        public InOutStock IOStock
        {
            get; set;
        }

        public override IStock Stock
        {
            get
            {
                return IOStock;
            }
            set
            {
                IOStock = value as InOutStock;
            }
        }

        public StockWrapperProperties() : base()
        {
        }

        public StockWrapperProperties(StockWrapper stockWrapper) : base(stockWrapper)
        {
            _target = stockWrapper;
        }

        protected override void Initialize(IProductWrapper product = null)
        {
            if (product == null)
            {
                IOStock = new InOutStock();
                Quantity = 1;
                Date = DateTime.Now;
            }
            else
            {
                IOStock = product.Product.Clone() as InOutStock;

                var stock = product as StockWrapper;
                Item = stock.Item;
                Specification = stock.Specification;
                Warehouse = stock.Warehouse;
                Quantity = stock.Quantity;
                Client = stock.Client;
                Employee = stock.Employee;
            }
        }

        public ClientWrapper Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                IOStock.EnterpriseUUID = _client.UUID;
                OnPropertyChanged("Client");
            }
        }

        public DateTime Date
        {
            get
            {
                return IOStock.Date;
            }
            set
            {
                IOStock.Date = value;
                OnPropertyChanged("Date");
            }
        }
        public FieldWrapper<Employee> Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                IOStock.EmployeeUUID = _employee != null ? _employee.UUID : null;
                OnPropertyChanged("Employee");
            }
        }

        public override FieldWrapper<Warehouse> Warehouse
        {
            get
            {
                return base.Warehouse;
            }
            set
            {
                base.Warehouse = value;
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
                InventoryWrapper invenw = iwd.CreateCollection().Where(x => x.Specification.UUID == Specification.UUID).SingleOrDefault();
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
                return IOStock.Remark;
            }
            set
            {
                IOStock.Remark = value;
                OnPropertyChanged("Remark");
            }
        }

        public StockType StockType
        {
            get
            {
                return IOStock.StockType;
            }
            set
            {
                IOStock.StockType = value;
                OnPropertyChanged("StockType");
            }
        }
    }
}
