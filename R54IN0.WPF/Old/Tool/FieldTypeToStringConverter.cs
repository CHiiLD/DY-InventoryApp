using System;
using System.Globalization;
using System.Windows.Data;

namespace R54IN0.WPF
{
    public class FieldTypeToStringConverter : IValueConverter
    {
        private const string ITEM = "품목";
        private const string SPECIFICATION = "규격";
        private const string MAKER = "제조사";
        private const string WAREHOUSE = "보관장소";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = null;
            if (value is Type)
            {
                Type type = value as Type;
                if (type == typeof(Item))
                    name = "품목";
                if (type == typeof(Specification))
                    name = "규격";
                if (type == typeof(Maker))
                    name = "제조사";
                if (type == typeof(Warehouse))
                    name = "보관장소";
            }
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = null;
            if (value is string)
            {
                string name = value as string;
                if (name.CompareTo(ITEM) == 0)
                    type = typeof(Item);
                else if (name.CompareTo(SPECIFICATION) == 0)
                    type = typeof(Specification);
                else if (name.CompareTo(WAREHOUSE) == 0)
                    type = typeof(Warehouse);
                else if (name.CompareTo(MAKER) == 0)
                    type = typeof(Maker);
            }
            return type;
        }
    }
}