using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Super_Tour
{
    public class StringWithCommasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                decimal number;
                if (decimal.TryParse(stringValue, out number))
                {
                    string formattedValue = number.ToString("#,##0");
                    return formattedValue;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                string formattedValue = stringValue.Replace(",", "");
                decimal number;
                if (decimal.TryParse(formattedValue, out number))
                {
                    return formattedValue;
                }
            }

            return value;
        }
    }
}
