namespace R54IN0.WPF
{
    public class IOStockDataGridItem : ObservableIOStock
    {
        public IOStockDataGridItem(IOStockFormat inoutStockFormat) : base(inoutStockFormat)
        {
            IsChecked = false;
        }

        bool? _isChecked;

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

        public int? InComingQuantity
        {
            get
            {
                return StockType == IOStockType.INCOMING ? (int?)Quantity : null;
            }
        }

        public int? OutGoingQuantity
        {
            get
            {
                return StockType == IOStockType.OUTGOING ? (int?)Quantity : null;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
    }
}