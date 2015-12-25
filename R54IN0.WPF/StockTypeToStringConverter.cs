using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace R54IN0.WPF
{
    public class StockTypeToStringConverter : IValueConverter
    {
        public const string IN_STOCK = "입고";
        public const string OUT_STOCK = "출고";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            StockType type = (StockType)value;

            switch (type)
            {
                case StockType.INCOMING:
                    result = IN_STOCK;
                    break;
                case StockType.OUTGOING:
                    result = OUT_STOCK;
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StockType type = StockType.NONE;
            string str = value as string;
            if (str.CompareTo(IN_STOCK) == 0)
                type = StockType.INCOMING;
            else if (str.CompareTo(OUT_STOCK) == 0)
                type = StockType.OUTGOING;
            return type;
        }
    }
}
