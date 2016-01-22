using System;
using System.ComponentModel;
using System.Threading.Tasks;

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

        public ObservableInventory(Observable<Product> product, string specification, int quantity, string memo,
            Observable<Maker> maker = null, Observable<Measure> measure = null) : this()
        {
            this.product = product;
            _fmt.ProductID = product.ID;
            _fmt.Specification = specification;
            _fmt.Quantity = quantity;
            _fmt.Memo = memo;
            if (maker != null)
            {
                _fmt.MakerID = maker.ID;
                this.maker = maker;
            }
            if (measure != null)
            {
                _fmt.MeasureID = measure.ID;
                this.measure = measure;
            }
        }

        public ObservableInventory(InventoryFormat fmt)
        {
            _fmt = fmt;
            InitializeProperties(fmt);
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
        public virtual string Memo
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

        protected void InitializeProperties(InventoryFormat fmt)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            product = ofd.SearchObservableField<Product>(fmt.ProductID);
            measure = ofd.SearchObservableField<Measure>(fmt.MeasureID);
            maker = ofd.SearchObservableField<Maker>(fmt.MakerID);
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
            InventoryFormat fmt = await DbAdapter.GetInstance().SelectAsync<InventoryFormat>(ID);
            if (fmt != null)
                Format = fmt;
        }
    }
}