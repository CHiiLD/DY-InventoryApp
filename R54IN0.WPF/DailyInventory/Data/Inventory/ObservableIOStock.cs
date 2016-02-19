using System;
using System.ComponentModel;
using System.Linq;

namespace R54IN0.WPF
{
    public class ObservableIOStock : IIOStockFormat, IObservableIOStockProperties, IUpdateLock
    {
        private IOStockFormat _fmt;
        private IObservableInventoryProperties _inventory;
        private bool _canUpdate = true;
        protected Observable<Customer> customer;
        protected Observable<Supplier> supplier;
        protected Observable<Project> project;
        protected Observable<Employee> employee;
        protected Observable<Warehouse> warehouse;
        protected PropertyChangedEventHandler propertyChanged;

        public ObservableIOStock()
        {
            _fmt = new IOStockFormat();
        }

        public ObservableIOStock(IOStockFormat inoutStockFormat)
        {
            _fmt = inoutStockFormat;
            InitializeProperties(inoutStockFormat);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged -= value;
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        public IOStockFormat Format
        {
            get
            {
                return _fmt;
            }
            set
            {
                _fmt = value;
                InitializeProperties(_fmt);
                NotifyPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// 입출고 종류
        /// </summary>
        public virtual IOStockType StockType
        {
            get
            {
                return _fmt.StockType;
            }
            set
            {
                if (_fmt.StockType != value)
                {
                    _fmt.StockType = value;
                    NotifyPropertyChanged("StockType");
                }
            }
        }

        /// <summary>
        /// 기록된 날짜
        /// </summary>
        public virtual DateTime Date
        {
            get
            {
                return _fmt.Date;
            }
            set
            {
                if (_fmt.Date != value)
                {
                    _fmt.Date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        /// <summary>
        /// 제품의 개별적 입고가, 출고가
        /// </summary>
        public virtual decimal UnitPrice
        {
            get
            {
                return _fmt.UnitPrice;
            }
            set
            {
                if (_fmt.UnitPrice != value)
                {
                    _fmt.UnitPrice = value;
                    NotifyPropertyChanged("UnitPrice");
                }
            }
        }

        /// <summary>
        /// 입고 또는 출고 수량
        /// </summary>
        public virtual int Quantity
        {
            get
            {
                return _fmt.Quantity;
            }
            set
            {
                if (_fmt.Quantity != value)
                {
                    _fmt.Quantity = value;
                    NotifyPropertyChanged("Quantity");
                }
            }
        }

        /// <summary>
        /// 비고
        /// </summary>
        public string Memo
        {
            get
            {
                return _fmt.Memo;
            }
            set
            {
                if (_fmt.Memo != value)
                {
                    _fmt.Memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }

        /// <summary>
        /// 거래처
        /// </summary>
        public Observable<Customer> Customer
        {
            get
            {
                return customer;
            }
            set
            {
                if (customer != value)
                {
                    _fmt.CustomerID = value != null ? value.ID : null;
                    customer = value;
                    NotifyPropertyChanged("Customer");
                }
            }
        }

        public Observable<Supplier> Supplier
        {
            get
            {
                return supplier;
            }
            set
            {
                if (supplier != value)
                {
                    _fmt.SupplierID = value != null ? value.ID : null;
                    supplier = value;
                    NotifyPropertyChanged("Supplier");
                }
            }
        }

        /// <summary>
        /// 프로젝트
        /// </summary>
        public Observable<Project> Project
        {
            get
            {
                return project;
            }
            set
            {
                if (project != value)
                {
                    _fmt.ProjectID = value != null ? value.ID : null;
                    project = value;
                    NotifyPropertyChanged("Project");
                }
            }
        }

        public virtual IObservableInventoryProperties Inventory
        {
            get
            {
                return _inventory;
            }
            set
            {
                if (_inventory != value)
                {
                    _fmt.InventoryID = value != null ? value.ID : null;
                    _inventory = value;
                    NotifyPropertyChanged("InventoryID");
                }
            }
        }

        public string ID
        {
            get
            {
                return _fmt.ID;
            }
            set
            {
                _fmt.ID = value;
                throw new NotSupportedException();
            }
        }

        public Observable<Employee> Employee
        {
            get
            {
                return employee;
            }
            set
            {
                if (employee != value)
                {
                    _fmt.EmployeeID = value != null ? value.ID : null;
                    employee = value;
                    NotifyPropertyChanged("Employee");
                }
            }
        }

        public Observable<Warehouse> Warehouse
        {
            get
            {
                return warehouse;
            }
            set
            {
                if (warehouse != value)
                {
                    _fmt.WarehouseID = value != null ? value.ID : null;
                    warehouse = value;
                    NotifyPropertyChanged("Warehouse");
                }
            }
        }

        public string CustomerID
        {
            get
            {
                return _fmt.CustomerID;
            }
            set
            {
                if (_fmt.CustomerID != value)
                    Customer = DataDirector.GetInstance().SearchField<Customer>(value);
            }
        }

        public string EmployeeID
        {
            get
            {
                return _fmt.EmployeeID;
            }
            set
            {
                if (_fmt.EmployeeID != value)
                    Employee = DataDirector.GetInstance().SearchField<Employee>(value);
            }
        }

        public string InventoryID
        {
            get
            {
                return _fmt.InventoryID;
            }
            set
            {
                if (_fmt.InventoryID != value)
                    Inventory = DataDirector.GetInstance().SearchInventory(value);
            }
        }

        public string ProjectID
        {
            get
            {
                return _fmt.ProjectID;
            }
            set
            {
                if (_fmt.ProjectID != value)
                    Project = DataDirector.GetInstance().SearchField<Project>(value);
            }
        }

        public string SupplierID
        {
            get
            {
                return _fmt.SupplierID;
            }
            set
            {
                if (_fmt.SupplierID != value)
                    Supplier = DataDirector.GetInstance().SearchField<Supplier>(value);
            }
        }

        public string WarehouseID
        {
            get
            {
                return _fmt.WarehouseID;
            }
            set
            {
                if (_fmt.WarehouseID != value)
                    Warehouse = DataDirector.GetInstance().SearchField<Warehouse>(value);
            }
        }

        public bool UpdateLock
        {
            get
            {
                return _canUpdate;
            }
            set
            {
                _canUpdate = value;
            }
        }

        protected virtual void InitializeProperties(IOStockFormat iosfmt)
        {
            var ofd = DataDirector.GetInstance();
            customer = ofd.SearchField<Customer>(iosfmt.CustomerID);
            supplier = ofd.SearchField<Supplier>(iosfmt.SupplierID);
            project = ofd.SearchField<Project>(iosfmt.ProjectID);
            employee = ofd.SearchField<Employee>(iosfmt.EmployeeID);
            warehouse = ofd.SearchField<Warehouse>(iosfmt.WarehouseID);

            var oid = DataDirector.GetInstance();
            _inventory = oid.SearchInventory(iosfmt.InventoryID);
        }

        public virtual void NotifyPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));

            if (string.IsNullOrEmpty(name))
                return;

            if (name == nameof(ID))
                throw new Exception();

            string[] fieldNames = new string[] { nameof(Customer), nameof(Supplier), nameof(Project),
                nameof(Inventory), nameof(Employee), nameof(Warehouse) };
            if (fieldNames.Any(x => x == name))
                name = name.Insert(name.Length, "ID");

            if (!typeof(IOStockFormat).GetProperties().Any(x => x.Name == name))
                return;

            if (ID == null)
                DataDirector.GetInstance().DB.Insert(Format);
            else if (UpdateLock)
                DataDirector.GetInstance().DB.Update<IOStockFormat>(ID, name, GetType().GetProperty(name).GetValue(this));
        }
    }
}