using System.ComponentModel;

namespace R54IN0
{
    public class ObservableInventory : IObservableInventoryProperties
    {
        private InventoryFormat _fmt;
        private Observable<Product> _product;
        private Observable<Maker> _maker;
        private Observable<Measure> _measure;
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

        public ObservableInventory()
        {
            _fmt = new InventoryFormat();
        }

        public ObservableInventory(InventoryFormat inventory)
        {
            InitializeProperties(inventory);
            _fmt = inventory;
        }

        protected void InitializeProperties(InventoryFormat inventory)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            _product = ofd.Search<Product>(inventory.ProductID);
            _measure = ofd.Search<Measure>(inventory.MeasureID);
            _maker = ofd.Search<Maker>(inventory.MakerID);
        }

        public InventoryFormat Format
        {
            get
            {
                return _fmt;
            }
        }

        /// <summary>
        /// 제품의 규격 이름
        /// </summary>
        public string Specification
        {
            get
            {
                return _fmt.Specification;
            }
            set
            {
                _fmt.Specification = value;
                NotifyPropertyChanged("Specification");
            }
        }

        /// <summary>
        /// 적재 수량
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
        /// 제품
        /// </summary>
        public Observable<Product> Product
        {
            get
            {
                return _product;
            }
            set
            {
                _fmt.ProductID = value != null ? value.ID : null;
                _product = value;
                NotifyPropertyChanged("Product");
            }
        }

        /// <summary>
        /// 단위
        /// </summary>
        public Observable<Measure> Measure
        {
            get
            {
                return _measure;
            }
            set
            {
                _fmt.MeasureID = value != null ? value.ID : null;
                _measure = value;
                NotifyPropertyChanged("Measure");
            }
        }

        /// <summary>
        /// 제조사
        /// </summary>
        public Observable<Maker> Maker
        {
            get
            {
                return _maker;
            }
            set
            {
                _fmt.MakerID = value != null ? value.ID : null;
                _maker = value;
                NotifyPropertyChanged("Maker");
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

        public virtual void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!string.IsNullOrEmpty(propertyName))
                _fmt.Save<InventoryFormat>();
        }
    }
}