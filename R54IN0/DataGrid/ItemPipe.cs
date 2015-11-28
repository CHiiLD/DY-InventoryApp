using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class ItemPipe : FieldPipe
    {
        Item _item;
        Measure _measure;
        Currency _currency;

        public ItemPipe(Item item) : base()
        {
            _item = item;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Measure measure = db.LoadByKey<Measure>(_item.MeasureUUID);
                if (measure != null)
                    SelectedMeasure = measure;
                Currency currency = db.LoadByKey<Currency>(_item.CurrencyUUID);
                if (currency != null)
                    SelectedCurrency = currency;
            }
        }

        public Item Item
        {
            get
            {
                return _item;
            }
        }

        public override string Name
        {
            get
            {
                return _item.Name;
            }
            set
            {
                if (_item.Name != value)
                {
                    _item.Name = value;
                    _item.Save<Item>();
                }
                OnPropertyChanged("Name");
            }
        }

        public Measure SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                if (_item.MeasureUUID != _measure.UUID)
                {
                    _item.MeasureUUID = _measure.UUID;
                    _item.Save<Item>();
                }
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public Currency SelectedCurrency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
                if (_item.CurrencyUUID != _currency.UUID)
                {
                    _item.CurrencyUUID = _currency.UUID;
                    _item.Save<Item>();
                }
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public IEnumerable<Specification> GetSpecifications()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.Table<Specification>().IndexQueryByKey("ItemUUID", _item.UUID).ToList().Where(x => !x.IsDeleted);
            }
        }
    }
}
