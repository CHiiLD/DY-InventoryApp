using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InoutStockDataGridItem : ObservableInoutStock
    {
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
