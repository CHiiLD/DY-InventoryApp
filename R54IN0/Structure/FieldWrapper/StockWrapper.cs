using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;
using System.ComponentModel;

namespace R54IN0
{
    public class StockWrapper : ProductWrapper<InOutStock>
    {
        ClientWrapper _client;
        FieldWrapper<Employee> _eeployee;
        InventoryWrapper _inventory;

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
                return Product.ItemUUID.Substring(0, 6).ToUpper();
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
                Product.EnterpriseUUID = (_client != null ? _client.UUID : null);
                Product.Save<InOutStock>();
                OnPropertyChanged("Client");
            }
        }

        public FieldWrapper<Employee> Employee
        {
            get
            {
                return _eeployee;
            }
            set
            {
                _eeployee = value;
                Product.EmployeeUUID = (_eeployee != null ? _eeployee.UUID : null);
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

        public override FieldWrapper<Warehouse> Warehouse
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
                Product.InventoryUUID = (_inventory != null ? _inventory.UUID : null);
                Product.Save<InOutStock>();
                if (_inventory != null)
                    _inventory.PropertyChanged += OnInventoryPropertyChanged;
                OnPropertyChanged("Inventory");
                OnPropertyChanged("Warehouse");
            }
        }

        void OnInventoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == Inventory && e.PropertyName == "Warehouse")
                OnPropertyChanged("Warehouse");
        }

        protected override void SetProperies(InOutStock stock)
        {
            base.SetProperies(stock);
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            _client = fwd.BinSearch<Client, ClientWrapper>(stock.EnterpriseUUID);
            _eeployee = fwd.BinSearch<Employee, FieldWrapper<Employee>>(stock.EmployeeUUID);
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            _inventory = iwd.BinSearch(Product.InventoryUUID);
            if (_inventory != null)
                _inventory.PropertyChanged += OnInventoryPropertyChanged;
        }
    }
}