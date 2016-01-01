using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0
{
    public class ItemWrapper : Observable<Item>, ICollectionViewModelObserver
    {
        private Observable<Measure> _measure;
        private Observable<Currency> _currency;
        private Observable<Maker> _maker;

        private CollectionViewModelObserverSubject _subject;

        public ItemWrapper() : base()
        {
            LoadEnumerableProperies();
            _subject = CollectionViewModelObserverSubject.GetInstance();
            _subject.Attach(this);
        }

        public ItemWrapper(Item item)
                    : base(item)
        {
            LoadEnumerableProperies();
            LoadProperties(item);
            _subject = CollectionViewModelObserverSubject.GetInstance();
            _subject.Attach(this);
        }

        public ItemWrapper(Item item, CollectionViewModelObserverSubject subject)
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

        public Observable<Measure> SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                Field.MeasureID = (_measure != null ? _measure.ID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public Observable<Maker> SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                Field.MakerID = (_maker != null ? _maker.ID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedMaker");
            }
        }

        public Observable<Currency> SelectedCurrency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
                Field.CurrencyID = (_currency != null ? _currency.ID : null);
                Field.Save<Item>();
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public ObservableCollection<Observable<Maker>> AllMaker
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Measure>> AllMeasure
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Currency>> AllCurrency
        {
            get;
            private set;
        }

        private void LoadEnumerableProperies()
        {
            var fwd = FieldWrapperDirector.GetInstance();
            AllMaker = new ObservableCollection<Observable<Maker>>(
                fwd.CreateCollection<Maker, Observable<Maker>>().Where(x => !x.IsDeleted));
            AllMeasure = new ObservableCollection<Observable<Measure>>(
                fwd.CreateCollection<Measure, Observable<Measure>>().Where(x => !x.IsDeleted));
            AllCurrency = new ObservableCollection<Observable<Currency>>(
                fwd.CreateCollection<Currency, Observable<Currency>>().Where(x => !x.IsDeleted));
        }

        private void LoadProperties(Item item)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _measure = AllMeasure.Where(x => x.ID == item.MeasureID).SingleOrDefault();
            _currency = AllCurrency.Where(x => x.ID == item.CurrencyID).SingleOrDefault();
            _maker = AllMaker.Where(x => x.ID == item.MakerID).SingleOrDefault();
        }

        /// <summary>
        /// 새로운 Measure, Currency, Maker 랩핑 클래스가 추가될 경우,
        /// 각각의 리스트에 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateNewItem(object item)
        {
            Type type = item.GetType();
            if (type == typeof(Observable<Measure>))
                AllMeasure.Add(item as Observable<Measure>);
            else if (type == typeof(Observable<Currency>))
                AllCurrency.Add(item as Observable<Currency>);
            else if (type == typeof(Observable<Maker>))
                AllMaker.Add(item as Observable<Maker>);
        }

        public void UpdateDelItem(object item)
        {
            Type type = item.GetType();

            if (type == typeof(Observable<Measure>))
            {
                AllMeasure.Remove(item as Observable<Measure>);
                if (SelectedMeasure == item)
                    SelectedMeasure = null;
            }
            else if (type == typeof(Observable<Currency>))
            {
                AllCurrency.Remove(item as Observable<Currency>);
                if (SelectedCurrency == item)
                    SelectedCurrency = null;
            }
            else if (type == typeof(Observable<Maker>))
            {
                AllMaker.Remove(item as Observable<Maker>);
                if (SelectedMaker == item)
                    SelectedMaker = null;
            }
        }
    }
}