using System.Linq;
using System.ComponentModel;

namespace R54IN0
{
    public abstract class ProductWrapper<ProductT> : IProductWrapper, INotifyPropertyChanged where ProductT : class, IStock, IUUID
    {
        ProductT _product;
        SpecificationWrapper _specification;
        ItemWrapper _item;

        public ProductWrapper()
        {
        }

        public ProductWrapper(ProductT record)
        {
            _product = record;
            LoadProperies(record);
        }

        public FieldWrapper<Measure> Measure
        {
            get
            {
                return Item.SelectedMeasure;
            }
        }

        public FieldWrapper<Maker> Maker
        {
            get
            {
                return Item.SelectedMaker;
            }
        }

        public FieldWrapper<Currency> Currency
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ProductT Record
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
                LoadProperies(_product);
                OnPropertyChanged("");
            }
        }

        IStock IProductWrapper.Product
        {
            get
            {
                return Record;
            }
            set
            {
                Record = value as ProductT;
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
                _product.ItemUUID = (_item != null ? _item.UUID : null);
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
                _product.SpecificationUUID = (_specification != null ? _specification.UUID : null);
                _product.Save<ProductT>();
                OnPropertyChanged("Specification");
            }
        }

        public abstract FieldWrapper<Warehouse> Warehouse { get; set; }

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

        public string UUID
        {
            get
            {
                return Record.UUID;
            }
        }

        protected virtual void LoadProperies(ProductT record)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            Item = fwd.CreateCollection<Item, ItemWrapper>().
                Where(x => x.UUID == record.ItemUUID).SingleOrDefault();
            Specification = fwd.CreateCollection<Specification, SpecificationWrapper>().
                Where(x => x.UUID == record.SpecificationUUID).SingleOrDefault();
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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