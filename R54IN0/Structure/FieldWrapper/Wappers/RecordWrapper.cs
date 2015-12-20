using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public interface IRecordWrapper
    {
        IRecord Inven { get; set; }
    }

    public class RecordWrapper<T> : IRecordWrapper, INotifyPropertyChanged where T : class, IRecord, IUUID
    {
        T _inven;
        SpecificationWrapper _specification;
        FieldWrapper<Warehouse> _warehouse;
        ItemWrapper _item;
        FieldWrapper<Measure> _measure;
        FieldWrapper<Currency> _currency;
        FieldWrapper<Maker> _maker;

        public RecordWrapper(T iinven)
        {
            _inven = iinven;
            LoadProperies();
        }

        public FieldWrapper<Measure> Measure
        {
            get
            {
                return _measure;
            }
        }

        public FieldWrapper<Maker> Maker
        {
            get
            {
                return _maker;
            }
        }

        public FieldWrapper<Currency> Currency
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

        IRecord IRecordWrapper.Inven
        {
            get
            {
                return _inven;
            }
            set
            {
                _inven = value as T;
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
                _inven.ItemUUID = value.Field.UUID;
                _inven.Save<T>();
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
                _inven.SpecificationUUID = value.Field.UUID;
                _inven.Save<T>();
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
                _inven.WarehouseUUID = value.Field.UUID;
                _inven.Save<T>();
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
                _inven.Save<T>();
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
                _inven.Save<T>();
                OnPropertyChanged("Remark");
            }
        }

        void LoadProperies()
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _specification = fwd.CreateFieldWrapperCollection<Specification, SpecificationWrapper>().
                Where(x => x.UUID == _inven.SpecificationUUID).SingleOrDefault();
            _item = fwd.CreateFieldWrapperCollection<Item, ItemWrapper>().
                Where(x => x.UUID == _inven.ItemUUID).SingleOrDefault();
            _warehouse = fwd.CreateFieldWrapperCollection<Warehouse, FieldWrapper<Warehouse>>().
                Where(x => x.UUID == _inven.WarehouseUUID).SingleOrDefault();

            if (_item != null)
            {
                _measure = _item.SelectedMeasure;
                _currency = _item.SelectedCurrency;
                _maker = _item.SelectedMaker;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}