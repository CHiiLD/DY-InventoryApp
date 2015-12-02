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
                return _measure;
            }
            set
            {
                _measure = value;
                Field.MeasureUUID = _measure.Field.UUID;
                Field.Save<Item>();
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public WMaker SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                Field.MakerUUID = _maker.Field.UUID;
                Field.Save<Item>();
                OnPropertyChanged("SelectedMaker");
            }
        }

        public WCurrency SelectedCurrency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
                Field.CurrencyUUID = _currency.Field.UUID;
                Field.Save<Item>();
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public SortedObservableCollection<WMaker> AllMaker
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedMakerList;
                }
            }
        }

        public SortedObservableCollection<WMeasure> AllMeasure
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedMeasureList;
                }
            }
        }

        public SortedObservableCollection<WCurrency> AllCurrency
        {
            get
            {
                using (var db = DatabaseDirector.GetFieldDb())
                {
                    return db.EnableSortedCurrencyList;
                }
            }
        }

        public WItem(Item item)
            : base(item)
        {
            using (var db = DatabaseDirector.GetFieldDb())
            {
                _measure = db.EnableSortedMeasureList.BinarySearchAsUUID(Field.MeasureUUID);
                _currency = db.EnableSortedCurrencyList.BinarySearchAsUUID(Field.CurrencyUUID);
                _maker = db.EnableSortedMakerList.BinarySearchAsUUID(Field.MakerUUID);
            }
        }
    }
}