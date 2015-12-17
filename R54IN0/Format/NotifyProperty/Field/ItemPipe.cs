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
        IFieldPipe _measure;
        IFieldPipe _currency;
        IFieldPipe _maker;

        public IFieldPipe SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                Field.MeasureUUID = _measure.Field.UUID;
                OnPropertyChanged("SelectedMeasure");
            }
        }

        public IFieldPipe SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                Field.MakerUUID = _maker.Field.UUID;
                OnPropertyChanged("SelectedMaker");
            }
        }

        public IFieldPipe SelectedCurrency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
                Field.CurrencyUUID = _currency.Field.UUID;
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public IEnumerable<IFieldPipe> AllMaker
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Maker>();
            }
        }

        public IEnumerable<IFieldPipe> AllMeasure
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Measure>();
            }
        }

        public IEnumerable<IFieldPipe> AllCurrency
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Currency>();
            }
        }

        public ItemPipe(Item item)
            : base(item)
        {
            FieldPipeCollectionDirector fcd = FieldPipeCollectionDirector.GetInstance();
            _measure = fcd.LoadEnablePipe<Measure>().Where(x => x.Field.UUID == Field.MeasureUUID).SingleOrDefault();
            _currency = fcd.LoadEnablePipe<Currency>().Where(x => x.Field.UUID == Field.CurrencyUUID).SingleOrDefault();
            _maker = fcd.LoadEnablePipe<Maker>().Where(x => x.Field.UUID == Field.MakerUUID).SingleOrDefault();
        }
    }
}
