using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public interface IRecordPipe
    {
        IInventory Inven { get; set; }
    }

    public class RecordPipe<T> : IRecordPipe, INotifyPropertyChanged where T : class, IInventory, IUUID
    {
        T _inven;
        IFieldPipe _specification;
        IFieldPipe _warehouse;
        IFieldPipe _item;
        IFieldPipe _measure;
        IFieldPipe _currency;
        IFieldPipe _maker;

        public IFieldPipe Measure
        {
            get
            {
                return _measure;
            }
        }

        public IFieldPipe Maker
        {
            get
            {
                return _maker;
            }
        }

        public IFieldPipe Currency
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
                return ((SpecificationPipe)_specification).SalesUnitPrice;
            }
        }

        public decimal SelesPriceAmount
        {
            get
            {
                return ((SpecificationPipe)_specification).SalesUnitPrice * ItemCount;
            }
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return ((SpecificationPipe)_specification).PurchaseUnitPrice;
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                return ((SpecificationPipe)_specification).PurchaseUnitPrice * ItemCount;
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

        IInventory IRecordPipe.Inven
        {
            get
            {
                return _inven;
            }
            set
            {

            }
        }

        public IFieldPipe Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                _inven.Save<T>();
                OnPropertyChanged("Item");
            }
        }

        public IFieldPipe Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                _inven.Save<T>();
                OnPropertyChanged("Specification");
            }
        }

        public IFieldPipe Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
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

        public RecordPipe(T iinven)
        {
            _inven = iinven;
            FieldPipeCollectionDirector fcd = FieldPipeCollectionDirector.GetInstance();
            _item = fcd.LoadPipe<Item>().Where(x => x.Field.UUID == _inven.ItemUUID).SingleOrDefault();
            _specification = fcd.LoadPipe<Specification>().Where(x => x.Field.UUID == _inven.SpecificationUUID).SingleOrDefault();
            _warehouse = fcd.LoadPipe<Warehouse>().Where(x => x.Field.UUID == _inven.WarehouseUUID).SingleOrDefault();

            if (_item != null)
            {
                ItemPipe itemPipe = _item as ItemPipe;
                _measure = fcd.LoadPipe<Measure>().Where(x => x.Field.UUID == itemPipe.Field.MeasureUUID).SingleOrDefault();
                _currency = fcd.LoadPipe<Currency>().Where(x => x.Field.UUID == itemPipe.Field.CurrencyUUID).SingleOrDefault();
                _maker = fcd.LoadPipe<Maker>().Where(x => x.Field.UUID == itemPipe.Field.MakerUUID).SingleOrDefault();
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}