namespace R54IN0.WPF
{
    public class IOStockDataGridItem : ObservableIOStock
    {
        public IOStockDataGridItem(IOStockFormat inoutStockFormat) : base(inoutStockFormat)
        {
            IsChecked = false;
        }

        public bool? IsChecked { get; set; }

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