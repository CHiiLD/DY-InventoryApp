using System.ComponentModel;

namespace R54IN0
{
    public abstract class ProductWrapper<ProductT> : IProductWrapper, INotifyPropertyChanged where ProductT : class, IStock, IID
    {
        private ProductT _product;
        private SpecificationWrapper _specification;
        private ItemWrapper _item;
        private PropertyChangedEventHandler _propertyChanged;

        public ProductWrapper()
        {
        }

        public ProductWrapper(ProductT record)
        {
            _product = record;
            SetProperies(record);
        }

        public Observable<Measure> Measure
        {
            get
            {
                return Item.SelectedMeasure;
            }
        }

        public Observable<Maker> Maker
        {
            get
            {
                return Item.SelectedMaker;
            }
        }

        public Observable<Currency> Currency
        {
            get
            {
                return Item.SelectedCurrency;
            }
        }

        public decimal SalesUnitPrice
        {
            get
            {
                return Specification.SalesUnitPrice;
            }
        }

        public decimal SelesPriceAmount
        {
            get
            {
                return Specification.SalesUnitPrice * Quantity;
            }
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return Specification.PurchaseUnitPrice;
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                return Specification.PurchaseUnitPrice * Quantity;
            }
        }

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

        public ProductT Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
                SetProperies(_product);
                OnPropertyChanged("");
            }
        }

        IStock IProductWrapper.Product
        {
            get
            {
                return Product;
            }
            set
            {
                Product = value as ProductT;
            }
        }

        public ItemWrapper Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                if (_item != null)
                    _item.PropertyChanged += OnItemWrapperPropertyChanged;
                _product.ItemID = (_item != null ? _item.ID : null);
                _product.Save<ProductT>();
                OnPropertyChanged("Item");
            }
        }

        public SpecificationWrapper Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                if (_specification != null)
                    _specification.PropertyChanged += OnSpecificationWrapperPropertyChanged;
                _product.SpecificationID = (_specification != null ? _specification.ID : null);
                _product.Save<ProductT>();
                OnPropertyChanged("Specification");
            }
        }

        public abstract Observable<Warehouse> Warehouse { get; set; }

        public int Quantity
        {
            get
            {
                return _product.Quantity;
            }
            set
            {
                _product.Quantity = value;
                _product.Save<ProductT>();
                OnPropertyChanged("Quantity");
                OnPropertyChanged("SelesPriceAmount");
                OnPropertyChanged("PurchasePriceAmount");
            }
        }

        public virtual string Remark
        {
            get
            {
                return _product.Remark;
            }
            set
            {
                _product.Remark = value;
                _product.Save<ProductT>();
                OnPropertyChanged("Remark");
            }
        }

        public string ID
        {
            get
            {
                return Product.ID;
            }
        }

        protected virtual void SetProperies(ProductT product)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _item = fwd.BinSearch<Item, ItemWrapper>(product.ItemID); //fwd.CreateCollection<Item, ItemWrapper>().Where(x => x.ID == record.ItemID).SingleOrDefault();
            if (_item != null)
                _item.PropertyChanged += OnItemWrapperPropertyChanged;
            _specification = fwd.BinSearch<Specification, SpecificationWrapper>(product.SpecificationID); //fwd.CreateCollection<Specification, SpecificationWrapper>().Where(x => x.ID == product.SpecificationID).SingleOrDefault();
            if (_specification != null)
                _specification.PropertyChanged += OnSpecificationWrapperPropertyChanged;
        }

        protected void OnPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void OnItemWrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != Item)
                return;
            if (e.PropertyName == "SelectedMeasure")
                OnPropertyChanged("Measure");
            else if (e.PropertyName == "SelectedMaker")
                OnPropertyChanged("Maker");
            else if (e.PropertyName == "SelectedCurrency")
                OnPropertyChanged("Currency");
        }

        protected void OnSpecificationWrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != Specification)
                return;
            if (e.PropertyName == "SalesUnitPrice")
            {
                OnPropertyChanged("SalesUnitPrice");
                OnPropertyChanged("SelesPriceAmount");
            }
            else if (e.PropertyName == "PurchaseUnitPrice")
            {
                OnPropertyChanged("PurchaseUnitPrice");
                OnPropertyChanged("PurchasePriceAmount");
            }
            else if (e.PropertyName == "Remark")
            {
                OnPropertyChanged("Remark");
            }
        }
    }
}