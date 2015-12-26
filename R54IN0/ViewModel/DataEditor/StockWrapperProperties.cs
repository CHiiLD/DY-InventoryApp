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

        public InOutStock InOutStock
        {
            get; set;
        }

        public override IStock Stock
        {
            get
            {
                return InOutStock;
            }
            set
            {
                InOutStock = value as InOutStock;
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
                InOutStock = new InOutStock();
                Quantity = 1;
                Date = DateTime.Now;
            }
            else
            {
                InOutStock = product.Product.Clone() as InOutStock;

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
                InOutStock.EnterpriseUUID = _client.UUID;
                OnPropertyChanged("Client");
            }
        }

        public DateTime Date
        {
            get
            {
                return InOutStock.Date;
            }
            set
            {
                InOutStock.Date = value;
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
                InOutStock.EmployeeUUID = _employee != null ? _employee.UUID : null;
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
                InventoryWrapper invenw = iwd.SearchAsSpecificationKey(Specification.UUID);
                if (invenw == null)
                    return Quantity;
                if (_target == null)
                    return StockType == StockType.INCOMING ? invenw.Quantity + Quantity : invenw.Quantity - Quantity;
                else
                    return StockType == StockType.INCOMING ? invenw.Quantity - _target.Quantity + Quantity : invenw.Quantity - _target.Quantity - Quantity;
            }
        }

        public string Remark
        {
            get
            {
                return InOutStock.Remark;
            }
            set
            {
                InOutStock.Remark = value;
                OnPropertyChanged("Remark");
            }
        }

        public StockType StockType
        {
            get
            {
                return InOutStock.StockType;
            }
            set
            {
                InOutStock.StockType = value;
                OnPropertyChanged("StockType");
                OnPropertyChanged("InventoryQuantity");
            }
        }
    }
}
