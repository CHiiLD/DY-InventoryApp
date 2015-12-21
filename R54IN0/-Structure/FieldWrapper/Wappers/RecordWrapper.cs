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
        FieldWrapper<Measure> _measure;
        FieldWrapper<Currency> _currency;
        FieldWrapper<Maker> _maker;

        public RecordWrapper()
        {
        }

        public RecordWrapper(RecordT record)
        {
            LoadProperies(record);
            _record = record;
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

        public RecordT Inven
        {
            get
            {
                return _record;
            }
        }

        IRecord IRecordWrapper.Inven
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
                _record.ItemUUID = value.Field.UUID;
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
                _record.SpecificationUUID = value.Field.UUID;
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
                _record.WarehouseUUID = value.Field.UUID;
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
                return Inven.UUID;
            }
        }

        void LoadProperies(RecordT record)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _specification = fwd.CreateFieldWrapperCollection<Specification, SpecificationWrapper>().
                Where(x => x.UUID == record.SpecificationUUID).SingleOrDefault();
            _item = fwd.CreateFieldWrapperCollection<Item, ItemWrapper>().
                Where(x => x.UUID == record.ItemUUID).SingleOrDefault();
            _warehouse = fwd.CreateFieldWrapperCollection<Warehouse, FieldWrapper<Warehouse>>().
                Where(x => x.UUID == record.WarehouseUUID).SingleOrDefault();

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