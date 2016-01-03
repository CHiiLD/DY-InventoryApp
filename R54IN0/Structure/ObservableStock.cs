using System;
using System.ComponentModel;

namespace R54IN0
{
    public class ObservableStock : IObservableStockProperties
    {
        private StockFormat _fmt;
        private Observable<Customer> _customer;
        private Observable<Supplier> _supplier;
        private Observable<Project> _project;

        private IObservableInventoryProperties _inven;
        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public ObservableStock()
        {
            _fmt = new StockFormat();
        }

        public ObservableStock(StockFormat stock)
        {
            InitializeProperties(stock);
            _fmt = stock;
        }

        protected void InitializeProperties(StockFormat stock)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            _customer = ofd.Search<Customer>(stock.CustomerID);
            _supplier = ofd.Search<Supplier>(stock.SupplierID);
            _project = ofd.Search<Project>(stock.ProjectID);
            var oid = ObservableInvenDirector.GetInstance();
            _inven = oid.Search(stock.InventoryItemID);
        }

        /// <summary>
        /// 입출고 종류
        /// </summary>
        public StockType StockType
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
                return _inven;
            }
            set
            {
                _fmt.InventoryItemID = value != null ? value.ID : null;
                _inven = value;
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

        public void NotifyPropertyChanged(string propertyName)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!string.IsNullOrEmpty(propertyName))
                _fmt.Save<StockFormat>();
        }
    }
}