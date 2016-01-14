namespace R54IN0.WPF
{
    public class IOStockDataGridItem : ObservableIOStock
    {
        public IOStockDataGridItem(IOStockFormat inoutStockFormat) : base(inoutStockFormat)
        {
            IsChecked = false;
        }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="thiz"></param>
        public IOStockDataGridItem(IOStockDataGridItem thiz) : base(new IOStockFormat(thiz.Format) { ID = null })
        {
            IsChecked = thiz.IsChecked;
        }

        private bool? _isChecked;

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