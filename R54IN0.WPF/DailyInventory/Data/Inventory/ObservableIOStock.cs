using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace R54IN0
{
    public class ObservableIOStock : IObservableIOStockProperties
    {
        private IOStockFormat _fmt;
        private IObservableInventoryProperties _inventory;

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
                _fmt.StockType = value;
                NotifyPropertyChanged("StockType");
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
                _fmt.Date = value;
                NotifyPropertyChanged("Date");
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
                _fmt.UnitPrice = value;
                NotifyPropertyChanged("UnitPrice");
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
                return customer;
            }
            set
            {
                _fmt.CustomerID = value != null ? value.ID : null;
                customer = value;
                NotifyPropertyChanged("Customer");
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
                _fmt.SupplierID = value != null ? value.ID : null;
                supplier = value;
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
                return project;
            }
            set
            {
                _fmt.ProjectID = value != null ? value.ID : null;
                project = value;
                NotifyPropertyChanged("Project");
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
                return employee;
            }
            set
            {
                _fmt.EmployeeID = value != null ? value.ID : null;
                employee = value;
                NotifyPropertyChanged("Employee");
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
                _fmt.WarehouseID = value != null ? value.ID : null;
                warehouse = value;
                NotifyPropertyChanged("Warehouse");
            }
        }

        public int RemainingQuantity
        {
            get
            {
                return _fmt.RemainingQuantity;
            }
            set
            {
                _fmt.RemainingQuantity = value;
                NotifyPropertyChanged("RemainingQuantity");
            }
        }

        protected virtual void InitializeProperties(IOStockFormat iosfmt)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            customer = ofd.SearchObservableField<Customer>(iosfmt.CustomerID);
            supplier = ofd.SearchObservableField<Supplier>(iosfmt.SupplierID);
            project = ofd.SearchObservableField<Project>(iosfmt.ProjectID);
            employee = ofd.SearchObservableField<Employee>(iosfmt.EmployeeID);
            warehouse = ofd.SearchObservableField<Warehouse>(iosfmt.WarehouseID);

            var oid = ObservableInventoryDirector.GetInstance();
            _inventory = oid.SearchObservableInventory(iosfmt.InventoryID);
        }

        public virtual async void NotifyPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));

            if (ID == null)
                await DbAdapter.GetInstance().InsertAsync(Format);
            else
                await DbAdapter.GetInstance().UpdateAsync(Format, name);
        }

        public async Task SyncDataFromServer()
        {
            IOStockFormat fmt = await DbAdapter.GetInstance().SelectAsync<IOStockFormat>(ID);
            if (fmt != null)
                Format = fmt;
        }
    }
}