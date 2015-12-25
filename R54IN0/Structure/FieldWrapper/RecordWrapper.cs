using System.Linq;
using System.ComponentModel;

namespace R54IN0
{
    public class RecordWrapper<RecordT> : IStockWrapper, INotifyPropertyChanged where RecordT : class, IStock, IUUID
    {
        RecordT _record;
        SpecificationWrapper _specification;
        FieldWrapper<Warehouse> _warehouse;
        ItemWrapper _item;

        public RecordWrapper()
        {
        }

        public RecordWrapper(RecordT record)
        {
            _record = record;
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

        public RecordT Record
        {
            get
            {
                return _record;
            }
            set
            {
                _record = value;
                LoadProperies(_record);
                OnPropertyChanged("");
            }
        }

        IStock IStockWrapper.Record
        {
            get
            {
                return Record;
            }
            set
            {
                Record = value as RecordT;
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
                _record.ItemUUID = (_item != null ? _item.UUID : null);
                _record.Save<RecordT>();
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
                _record.SpecificationUUID = (_specification != null ? _specification.UUID : null);
                _record.Save<RecordT>();
                OnPropertyChanged("Specification");
            }
        }

        public FieldWrapper<Warehouse> Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                _record.WarehouseUUID = (_warehouse != null ? _warehouse.UUID : null);
                _record.Save<RecordT>();
                OnPropertyChanged("Warehouse");
            }
        }

        public int Quantity
        {
            get
            {
                return _record.Quantity;
            }
            set
            {
                _record.Quantity = value;
                _record.Save<RecordT>();
                OnPropertyChanged("Quantity");
                OnPropertyChanged("SelesPriceAmount");
                OnPropertyChanged("PurchasePriceAmount");
            }
        }

        public virtual string Remark
        {
            get
            {
                return _record.Remark;
            }
            set
            {
                _record.Remark = value;
                _record.Save<RecordT>();
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

        protected virtual void LoadProperies(RecordT record)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            Item = fwd.CreateCollection<Item, ItemWrapper>().
                Where(x => x.UUID == record.ItemUUID).SingleOrDefault();
            Specification = fwd.CreateCollection<Specification, SpecificationWrapper>().
                Where(x => x.UUID == record.SpecificationUUID).SingleOrDefault();
            Warehouse = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>().
                Where(x => x.UUID == record.WarehouseUUID).SingleOrDefault();
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