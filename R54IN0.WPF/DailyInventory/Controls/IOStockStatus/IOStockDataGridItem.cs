namespace R54IN0.WPF
{
    public class IOStockDataGridItem : ObservableIOStock
    {
        private bool? _isChecked;
        private int _remainingQuantity;

        public IOStockDataGridItem(IOStockFormat inoutStockFormat) : base(inoutStockFormat)
        {
            IsChecked = false;
        }

        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public int InComingQuantity
        {
            get
            {
                return StockType == IOStockType.INCOMING ? Quantity : 0;
            }
        }

        public int OutGoingQuantity
        {
            get
            {
                return StockType == IOStockType.OUTGOING ? Quantity : 0;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }

        public int RemainingQuantity
        {
            get
            {
                return _remainingQuantity;
            }
            set
            {
                _remainingQuantity = value;
                NotifyPropertyChanged("RemainingQuantity");
            }
        }

        public override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);
            if (name == nameof(Quantity))
            {
                NotifyPropertyChanged(nameof(InComingQuantity));
                NotifyPropertyChanged(nameof(OutGoingQuantity));
                NotifyPropertyChanged(nameof(RemainingQuantity));
                NotifyPropertyChanged(nameof(TotalPrice));
            }
            else if (name == nameof(UnitPrice))
            {
                NotifyPropertyChanged(nameof(TotalPrice));
            }
        }
    }
}