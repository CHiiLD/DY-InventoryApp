using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class ItemPipe : FieldPipe<Item>
    {
        Measure _measure;
        Currency _currency;
        Maker _maker;

        public Measure SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                Field.MeasureUUID = _measure.UUID;
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public Maker SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                Field.MakerUUID = _maker.UUID;
                OnPropertyChanged("SelectedMaker");
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
                Field.CurrencyUUID = _currency.UUID;
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public IEnumerable<Maker> AllMaker
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Maker>().Where(x => !x.IsDeleted);
                }
            }
        }

        public IEnumerable<Measure> AllMeasure
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Measure>().Where(x => !x.IsDeleted);
                }
            }
        }

        public IEnumerable<Currency> AllCurrency
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Currency>().Where(x => !x.IsDeleted);
                }
            }
        }

        public ItemPipe(Item item)
            : base(item)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                _measure = db.LoadByKey<Measure>(Field.MeasureUUID);
                _currency = db.LoadByKey<Currency>(Field.CurrencyUUID);
                _maker = db.LoadByKey<Maker>(Field.MakerUUID);
            }
        }
    }
}
