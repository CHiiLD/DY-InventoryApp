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

        ObservableCollection<FieldWrapper<Measure>> _measures;
        ObservableCollection<FieldWrapper<Currency>> _currencies;
        ObservableCollection<FieldWrapper<Maker>> _makers;

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
                Field.MeasureUUID = value.Field.UUID;
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
                Field.MakerUUID = value.Field.UUID;
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
                Field.CurrencyUUID = value.Field.UUID;
                Field.Save<Item>();
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public ObservableCollection<FieldWrapper<Maker>> AllMaker
        {
            get
            {
                return _makers;
            }
        }

        public ObservableCollection<FieldWrapper<Measure>> AllMeasure
        {
            get
            {
                return _measures;
            }
        }

        public ObservableCollection<FieldWrapper<Currency>> AllCurrency
        {
            get
            {
                return _currencies;
            }
        }

        void LoadEnumerableProperies()
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _makers = new ObservableCollection<FieldWrapper<Maker>>(
                fwd.CreateCollection<Maker, FieldWrapper<Maker>>().Where(x => !x.IsDeleted));
            _measures = new ObservableCollection<FieldWrapper<Measure>>(
                fwd.CreateCollection<Measure, FieldWrapper<Measure>>().Where(x => !x.IsDeleted));
            _currencies = new ObservableCollection<FieldWrapper<Currency>>(
                fwd.CreateCollection<Currency, FieldWrapper<Currency>>().Where(x => !x.IsDeleted));
        }

        void LoadProperties(Item item)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _measure = _measures.Where(x => x.UUID == Field.MeasureUUID).SingleOrDefault();
            _currency = _currencies.Where(x => x.UUID == Field.CurrencyUUID).SingleOrDefault();
            _maker = _makers.Where(x => x.UUID == Field.MakerUUID).SingleOrDefault();
        }

        public void UpdateNewItem(object item)
        {
            Type type = item.GetType();

            if (type == typeof(FieldWrapper<Measure>))
                _measures.Add(item as FieldWrapper<Measure>);
            else if (type == typeof(FieldWrapper<Currency>))
                _currencies.Add(item as FieldWrapper<Currency>);
            else if (type == typeof(FieldWrapper<Maker>))
                _makers.Add(item as FieldWrapper<Maker>);
        }

        public void UpdateDelItem(object item)
        {
            Type type = item.GetType();

            if (type == typeof(FieldWrapper<Measure>))
                _measures.Remove(item as FieldWrapper<Measure>);
            else if (type == typeof(FieldWrapper<Currency>))
                _currencies.Remove(item as FieldWrapper<Currency>);
            else if (type == typeof(FieldWrapper<Maker>))
                _makers.Remove(item as FieldWrapper<Maker>);
        }
    }
}