using System;
using System.ComponentModel;

namespace R54IN0
{
    public class ObservableInoutStock : IObservableInoutStockProperties
    {
        private InoutStockFormat _fmt;
        private Observable<Customer> _customer;
        private Observable<Supplier> _supplier;
        private Observable<Project> _project;
        private Observable<Employee> _employee;
        private Observable<Warehouse> _warehouse;

        private IObservableInventoryProperties _inventory;
        protected PropertyChangedEventHandler propertyChanged;

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

        public ObservableInoutStock()
        {
            _fmt = new InoutStockFormat();
        }

        public ObservableInoutStock(InoutStockFormat inoutStockFormat)
        {
            InitializeProperties(inoutStockFormat);
            _fmt = inoutStockFormat;
        }

        public ObservableInoutStock(ObservableInoutStock thiz) : this(thiz._fmt)
        {
        }

        protected void InitializeProperties(InoutStockFormat inoutStockFormat)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            _customer = ofd.Search<Customer>(inoutStockFormat.CustomerID);
            _supplier = ofd.Search<Supplier>(inoutStockFormat.SupplierID);
            _project = ofd.Search<Project>(inoutStockFormat.ProjectID);
            _employee = ofd.Search<Employee>(inoutStockFormat.EmployeeID);
            _warehouse = ofd.Search<Warehouse>(inoutStockFormat.WarehouseID);

            var oid = ObservableInvenDirector.GetInstance();
            _inventory = oid.Search(inoutStockFormat.InventoryID);
        }

        public InoutStockFormat Format
        {
            get
            {
                return _fmt;
            }
        }

        /// <summary>
        /// 입출고 종류
        /// </summary>
        public virtual StockType StockType
        {
            get
            {
                return _fmt.StockType;
            }
            set
            {
                _fmt.StockType = value;
                NotifyPropertyChanged("StockType");
            }
        }

        /// <summary>
        /// 기록된 날짜
        /// </summary>
        public DateTime Date
        {
            get
            {
                return _fmt.Date;
            }
            set
            {
                _fmt.Date = value;
                NotifyPropertyChanged("Date");
            }
        }

        /// <summary>
        /// 제품의 개별적 입고가, 출고가
        /// </summary>
        public decimal UnitPrice
        {
            get
            {
                return _fmt.UnitPrice;
            }
            set
            {
                _fmt.UnitPrice = value;
                NotifyPropertyChanged("UnitPrice");
            }
        }

        /// <summary>
        /// 입고 또는 출고 수량
        /// </summary>
        public int Quantity
        {
            get
            {
                return _fmt.Quantity;
            }
            set
            {
                _fmt.Quantity = value;
                NotifyPropertyChanged("Quantity");
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
                _fmt.Memo = value;
                NotifyPropertyChanged("Memo");
            }
        }

        /// <summary>
        /// 거래처
        /// </summary>
        public Observable<Customer> Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _fmt.CustomerID = value != null ? value.ID : null;
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        public Observable<Supplier> Supplier
        {
            get
            {
                return _supplier;
            }
            set
            {
                _fmt.SupplierID = value != null ? value.ID : null;
                _supplier = value;
                NotifyPropertyChanged("Supplier");
            }
        }

        /// <summary>
        /// 프로젝트
        /// </summary>
        public Observable<Project> Project
        {
            get
            {
                return _project;
            }
            set
            {
                _fmt.ProjectID = value != null ? value.ID : null;
                _project = value;
                NotifyPropertyChanged("Project");
            }
        }

        public IObservableInventoryProperties Inventory
        {
            get
            {
                return _inventory;
            }
            set
            {
                _fmt.InventoryID = value != null ? value.ID : null;
                _inventory = value;
                NotifyPropertyChanged("Inventory");
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
                _fmt.EmployeeID = value != null ? value.ID : null;
                _employee = value;
                NotifyPropertyChanged("Employee");
            }
        }

        public Observable<Warehouse> Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _fmt.WarehouseID = value != null ? value.ID : null;
                _warehouse = value;
                NotifyPropertyChanged("Warehouse");
            }
        }

        public virtual void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!string.IsNullOrEmpty(propertyName))
                _fmt.Save<InoutStockFormat>();
        }
    }
}