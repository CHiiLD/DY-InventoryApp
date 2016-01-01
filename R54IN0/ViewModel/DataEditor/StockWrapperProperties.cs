using System;

namespace R54IN0
{
    public class StockWrapperProperties : InventoryWrapperProperties, IStockViewModelProperties
    {
        private StockWrapper _target;
        private Observable<Employee> _employee;
        private ClientWrapper _client;

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
                InOutStock.EnterpriseID = _client.ID;
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

        public Observable<Employee> Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                InOutStock.EmployeeID = _employee != null ? _employee.ID : null;
                OnPropertyChanged("Employee");
            }
        }

        public override Observable<Warehouse> Warehouse
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
                InventoryWrapper invenw = iwd.SearchAsSpecificationKey(Specification.ID);
                if (invenw == null)
                    return StockType == StockType.INCOMING ? Quantity : -Quantity; //Inventory 데이터가 없는 경우 입출고 수량으로 표현(출고는 물건이 나가는 것임으로 마이너스 연산으로 반환)

                if (_target == null) //새로운 데이터를 추가할 경우, Inventory의 수량 데이터와 가감연산을 하여 반환한다. (간단)
                    return StockType == StockType.INCOMING ? invenw.Quantity + Quantity : invenw.Quantity - Quantity;
                else //기존의 데이터를 수정하고자 할 경우, 이 데이터를 추가하기 전 수량과, 수정 후 변동될 값을 계산하여 반환한다. (복잡)
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