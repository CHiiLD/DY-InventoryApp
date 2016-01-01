using System;
using System.ComponentModel;

namespace R54IN0
{
    public class StockWrapper : ProductWrapper<InOutStock>
    {
        private ClientWrapper _client;
        private Observable<Employee> _eeployee;
        private InventoryWrapper _inventory;

        public StockWrapper()
           : base()
        {
        }

        public StockWrapper(InOutStock ioStock)
            : base(ioStock)
        {
        }

        public string Code
        {
            get
            {
                return Product.ItemID.Substring(0, 6).ToUpper();
            }
        }

        public DateTime Date
        {
            get
            {
                return Product.Date;
            }
            set
            {
                Product.Date = value;
                Product.Save<InOutStock>();
                OnPropertyChanged("Date");
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
                Product.EnterpriseID = (_client != null ? _client.ID : null);
                Product.Save<InOutStock>();
                OnPropertyChanged("Client");
            }
        }

        public Observable<Employee> Employee
        {
            get
            {
                return _eeployee;
            }
            set
            {
                _eeployee = value;
                Product.EmployeeID = (_eeployee != null ? _eeployee.ID : null);
                Product.Save<InOutStock>();
                OnPropertyChanged("Employee");
            }
        }

        public StockType StockType
        {
            get
            {
                return Product.StockType;
            }
            set
            {
                Product.StockType = value;
                Product.Save<InOutStock>();
                OnPropertyChanged("StockType");
            }
        }

        public override Observable<Warehouse> Warehouse
        {
            get
            {
                return Inventory != null ? Inventory.Warehouse : null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public InventoryWrapper Inventory
        {
            get
            {
                return _inventory;
            }
            set
            {
                _inventory = value;
                Product.InventoryID = (_inventory != null ? _inventory.ID : null);
                Product.Save<InOutStock>();
                if (_inventory != null)
                    _inventory.PropertyChanged += OnInventoryPropertyChanged;
                OnPropertyChanged("Inventory");
                OnPropertyChanged("Warehouse");
            }
        }

        private void OnInventoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == Inventory && e.PropertyName == "Warehouse")
                OnPropertyChanged("Warehouse");
        }

        protected override void SetProperies(InOutStock stock)
        {
            base.SetProperies(stock);
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            _client = fwd.BinSearch<Client, ClientWrapper>(stock.EnterpriseID);
            _eeployee = fwd.BinSearch<Employee, Observable<Employee>>(stock.EmployeeID);
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            _inventory = iwd.BinSearch(Product.InventoryID);
            if (_inventory != null)
                _inventory.PropertyChanged += OnInventoryPropertyChanged;
        }
    }
}