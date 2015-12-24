using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class ItemWrapper : FieldWrapper<Item>, IViewModelObserver
    {
        FieldWrapper<Measure> _measure;
        FieldWrapper<Currency> _currency;
        FieldWrapper<Maker> _maker;

        ViewModelObserverSubject _subject;

        public ItemWrapper() : base()
        {
            LoadEnumerableProperies();
            _subject = ViewModelObserverSubject.GetInstance();
            _subject.Attach(this);
        }

        public ItemWrapper(Item item)
                    : base(item)
        {
            LoadEnumerableProperies();
            LoadProperties(item);
            _subject = ViewModelObserverSubject.GetInstance();
            _subject.Attach(this);
        }

        public ItemWrapper(Item item, ViewModelObserverSubject subject)
                    : base(item)
        {
            LoadEnumerableProperies();
            LoadProperties(item);
            _subject = subject;
            _subject.Attach(this);
        }

        ~ItemWrapper()
        {
            _subject.Detach(this);
        }

        public override Item Field
        {
            get
            {
                return base.Field;
            }
            set
            {
                LoadProperties(value);
                base.Field = value;
            }
        }

        public FieldWrapper<Measure> SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                Field.MeasureUUID = (_measure != null ? _measure.UUID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public FieldWrapper<Maker> SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                Field.MakerUUID = (_maker != null ? _maker.UUID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedMaker");
            }
        }

        public FieldWrapper<Currency> SelectedCurrency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
                Field.CurrencyUUID = (_currency != null ? _currency.UUID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public ObservableCollection<FieldWrapper<Maker>> AllMaker
        {
            get;
            private set;
        }

        public ObservableCollection<FieldWrapper<Measure>> AllMeasure
        {
            get;
            private set;
        }

        public ObservableCollection<FieldWrapper<Currency>> AllCurrency
        {
            get;
            private set;
        }

        void LoadEnumerableProperies()
        {
            var fwd = FieldWrapperDirector.GetInstance();
            AllMaker = new ObservableCollection<FieldWrapper<Maker>>(
                fwd.CreateCollection<Maker, FieldWrapper<Maker>>().Where(x => !x.IsDeleted));
            AllMeasure = new ObservableCollection<FieldWrapper<Measure>>(
                fwd.CreateCollection<Measure, FieldWrapper<Measure>>().Where(x => !x.IsDeleted));
            AllCurrency = new ObservableCollection<FieldWrapper<Currency>>(
                fwd.CreateCollection<Currency, FieldWrapper<Currency>>().Where(x => !x.IsDeleted));
        }

        void LoadProperties(Item item)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _measure = AllMeasure.Where(x => x.UUID == item.MeasureUUID).SingleOrDefault();
            _currency = AllCurrency.Where(x => x.UUID == item.CurrencyUUID).SingleOrDefault();
            _maker = AllMaker.Where(x => x.UUID == item.MakerUUID).SingleOrDefault();
        }

        public void UpdateNewItem(object item)
        {
            Type type = item.GetType();

            if (type == typeof(FieldWrapper<Measure>))
            {
                AllMeasure.Add(item as FieldWrapper<Measure>);
            }
            else if (type == typeof(FieldWrapper<Currency>))
            {
                AllCurrency.Add(item as FieldWrapper<Currency>);
            }
            else if (type == typeof(FieldWrapper<Maker>))
            {
                AllMaker.Add(item as FieldWrapper<Maker>);
            }
        }

        public void UpdateDelItem(object item)
        {
            Type type = item.GetType();

            if (type == typeof(FieldWrapper<Measure>))
            {
                AllMeasure.Remove(item as FieldWrapper<Measure>);
                if (SelectedMeasure == item)
                    SelectedMeasure = null;
            }
            else if (type == typeof(FieldWrapper<Currency>))
            {
                AllCurrency.Remove(item as FieldWrapper<Currency>);
                if (SelectedCurrency == item)
                    SelectedCurrency = null;
            }
            else if (type == typeof(FieldWrapper<Maker>))
            {
                AllMaker.Remove(item as FieldWrapper<Maker>);
                if (SelectedMaker == item)
                    SelectedMaker = null;
            }
        }
    }
}