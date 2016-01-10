using System;
using System.Globalization;
using System.Windows.Data;

namespace R54IN0.WPF
{
    public class StockTypeToStringConverter : IValueConverter
    {
        private const string IN_STOCK = "입고";
        private const string OUT_STOCK = "출고";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            IOStockType type = (IOStockType)value;

            switch (type)
            {
                case IOStockType.INCOMING:
                    result = IN_STOCK;
                    break;

                case IOStockType.OUTGOING:
                    result = OUT_STOCK;
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IOStockType type = IOStockType.NONE;
            string str = value as string;
            if (str.CompareTo(IN_STOCK) == 0)
                type = IOStockType.INCOMING;
            else if (str.CompareTo(OUT_STOCK) == 0)
                type = IOStockType.OUTGOING;
            return type;
        }
    }
}