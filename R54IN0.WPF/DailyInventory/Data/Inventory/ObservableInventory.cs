using System.ComponentModel;

namespace R54IN0
{
    public class ObservableInventory : IObservableInventoryProperties
    {
        private InventoryFormat _fmt;
        protected Observable<Product> product;
        protected Observable<Maker> maker;
        protected Observable<Measure> measure;
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

        public ObservableInventory(InventoryFormat inventoryFormat)
        {
            _fmt = inventoryFormat;
            InitializeProperties(inventoryFormat);
        }

        protected void InitializeProperties(InventoryFormat fmt)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            product = ofd.Search<Product>(fmt.ProductID);
            measure = ofd.Search<Measure>(fmt.MeasureID);
            maker = ofd.Search<Maker>(fmt.MakerID);
        }

        public virtual InventoryFormat Format
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
        /// 제품의 규격 이름
        /// </summary>
        public virtual string Specification
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
        public virtual string Remark
        {
            get
            {
                return _fmt.Remark;
            }
            set
            {
                _fmt.Remark = value;
                NotifyPropertyChanged("Remark");
            }
        }

        /// <summary>
        /// 제품
        /// </summary>
        public virtual Observable<Product> Product
        {
            get
            {
                return product;
            }
            set
            {
                _fmt.ProductID = value != null ? value.ID : null;
                product = value;
                NotifyPropertyChanged("Product");
            }
        }

        /// <summary>
        /// 단위
        /// </summary>
        public virtual Observable<Measure> Measure
        {
            get
            {
                return measure;
            }
            set
            {
                _fmt.MeasureID = value != null ? value.ID : null;
                measure = value;
                NotifyPropertyChanged("Measure");
            }
        }

        /// <summary>
        /// 제조사
        /// </summary>
        public virtual Observable<Maker> Maker
        {
            get
            {
                return maker;
            }
            set
            {
                _fmt.MakerID = value != null ? value.ID : null;
                maker = value;
                NotifyPropertyChanged("Maker");
            }
        }

        public virtual string ID
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

        public virtual async void NotifyPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));

            if (ID == null)
            {
                await DbAdapter.GetInstance().InsertAsync(Format);
                ObservableInventoryDirector.GetInstance().Add(this);
            }
            else
            {
                await DbAdapter.GetInstance().UpdateAsync(Format, name);
            }
        }
    }
}