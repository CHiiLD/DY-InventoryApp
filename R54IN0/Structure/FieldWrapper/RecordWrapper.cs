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
        IRecord Record { get; set; }
        ItemWrapper Item { get; set; }
        SpecificationWrapper Specification { get; set; }
        FieldWrapper<Warehouse> Warehouse { get; set; }
        int ItemCount { get; set; }
    }

    public class RecordWrapper<RecordT> : IRecordWrapper, INotifyPropertyChanged where RecordT : class, IRecord, IUUID
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
                return Specification.SalesUnitPrice * ItemCount;
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
                return Specification.PurchaseUnitPrice * ItemCount;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RecordT Record
        {
            get
            {
                return _record;
            }
        }

        IRecord IRecordWrapper.Record
        {
            get
            {
                return _record;
            }
            set
            {
                _record = value as RecordT;
                LoadProperies(_record);
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

        public int ItemCount
        {
            get
            {
                return _record.ItemCount;
            }
            set
            {
                _record.ItemCount = value;
                _record.Save<RecordT>();
                OnPropertyChanged("ItemCount");
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