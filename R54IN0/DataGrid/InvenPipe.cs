using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public class InvenPipe<T> : INotifyPropertyChanged where T : class, IInventory, new()
    {
        T _inven;
        Specification _specification;
        Warehouse _warehouse;
        Item _item;
        Measure _measure;
        Currency _currency;
        Maker _maker;

        public Measure Measure
        {
            get
            {
                return _measure;
            }
        }

        public Maker Maker
        {
            get
            {
                return _maker;
            }
        }

        public Currency Currency
        {
            get
            {
                return _currency;
            }
        }

        public decimal SalesUnitPrice
        {
            get
            {
                return _specification.SalesUnitPrice;
            }
        }

        public decimal SelesPriceAmount
        {
            get
            {
                return _specification.SalesUnitPrice * ItemCount;
            }
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return _specification.PurchaseUnitPrice;
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                return _specification.PurchaseUnitPrice * ItemCount;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Inven
        {
            get
            {
                return _inven;
            }
        }

        public Item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                IEnumerable<Specification> result = null;
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    result = db.Table<Specification>().IndexQueryByKey("ItemUUID", _item.UUID).ToList().Where(x => !x.IsDeleted);
                    _measure = db.LoadByKey<Measure>(_item.MeasureUUID);
                    _currency = db.LoadByKey<Currency>(_item.CurrencyUUID);
                    _maker = db.LoadByKey<Maker>(_item.MakerUUID);
                }
                Specification = result.FirstOrDefault();
                OnPropertyChanged("Item");
            }
        }

        public Specification Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                _inven.SpecificationUUID = _specification.UUID;
                OnPropertyChanged("Specification");
            }
        }

        public Warehouse Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                _inven.WarehouseUUID = _warehouse.UUID;
                OnPropertyChanged("Warehouse");
            }
        }

        public int ItemCount
        {
            get
            {
                return _inven.ItemCount;
            }
            set
            {
                _inven.ItemCount = value;
                OnPropertyChanged("ItemCount");
            }
        }

        public virtual string Remark
        {
            get
            {
                return _inven.Remark;
            }
            set
            {
                _inven.Remark = value;
                OnPropertyChanged("Remark");
            }
        }

        public InvenPipe(T iinven)
        {
            _inven = iinven;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Specification spec = db.LoadByKey<Specification>(_inven.SpecificationUUID);
                if (spec != null)
                {
                    Item = db.LoadByKey<Item>(spec.ItemUUID);
                    Specification = spec;
                }
                _warehouse = db.LoadByKey<Warehouse>(_inven.WarehouseUUID);
            }
        }

        //public InvenPipe()
        //{
            
        //}

        public InvenPipe(InvenPipe<T> thiz)
        {
            _inven = thiz._inven.Clone() as T;
            _specification = new Specification(thiz._specification);
            _warehouse = new Warehouse(thiz._warehouse);
            _item = new Item(thiz._item);
            _measure = new Measure(thiz._measure);
            _currency = new Currency(thiz._currency);
            _maker = new Maker(thiz._maker);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}