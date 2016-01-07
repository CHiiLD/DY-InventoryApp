namespace R54IN0.WPF
{
    public class InoutStockDataGridItem : ObservableInoutStock
    {
        public InoutStockDataGridItem(InoutStockFormat inoutStockFormat) : base(inoutStockFormat)
        {
            IsChecked = false;
        }

        public bool? IsChecked { get; set; }

        public int? InComingQuantity
        {
            get
            {
                return StockType == StockType.INCOMING ? (int?)Quantity : null;
            }
        }

        public int? OutGoingQuantity
        {
            get
            {
                return StockType == StockType.OUTGOING ? (int?)Quantity : null;
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