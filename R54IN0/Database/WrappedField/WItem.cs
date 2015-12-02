using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class WItem : WField<Item>
    {
        WMeasure _measure;
        WCurrency _currency;
        WMaker _maker;

        public WMeasure SelectedMeasure
        {
            get
            {
                if (_measure == null)
                {
                    using (var db = DatabaseDirector.GetFieldDb())
                    {
                        _measure = db.EnableSortedMeasureList.BinarySearchAsUUID(Field.MeasureUUID);
                    }
                }
                return _measure;
            }
            set
            {
                _measure = value;
                if (_measure != null)
                    Field.MeasureUUID = _measure.Field.UUID;
                else
                    Field.MeasureUUID = null;
                Field.Save<Item>();
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public WMaker SelectedMaker
        {
            get
            {
                if (_maker == null)
                {
                    using (var db = DatabaseDirector.GetFieldDb())
                    {
                        _maker = db.EnableSortedMakerList.BinarySearchAsUUID(Field.MakerUUID);
                    }
                }
                return _maker;
            }
            set
            {
                _maker = value;
                if (_maker != null)
                    Field.MakerUUID = _maker.Field.UUID;
                else
                    Field.MakerUUID = null;
                Field.Save<Item>();
                OnPropertyChanged("SelectedMaker");
            }
        }

        public WCurrency SelectedCurrency
        {
            get
            {
                if (_currency == null)
                {
                    using (var db = DatabaseDirector.GetFieldDb())
                    {
                        _currency = db.EnableSortedCurrencyList.BinarySearchAsUUID(Field.CurrencyUUID);
                    }
                }
                return _currency;
            }
            set
            {
                _currency = value;
                if (_currency != null)
                    Field.CurrencyUUID = _currency.Field.UUID;
                else
                    Field.CurrencyUUID = null;
                Field.Save<Item>();
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public IEnumerable<WMaker> AllMaker
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedMakerList;
                }
            }
        }

        public IEnumerable<WMeasure> AllMeasure
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedMeasureList;
                }
            }
        }

        public IEnumerable<WCurrency> AllCurrency
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedCurrencyList;
                }
            }
        }

        public IEnumerable<WSpecification> AllSpecification
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedSpecList.Where(x => x.Field.ItemUUID == Field.UUID);
                }
            }
        }

        public WItem(Item item)
            : base(item)
        {
        }
    }
}