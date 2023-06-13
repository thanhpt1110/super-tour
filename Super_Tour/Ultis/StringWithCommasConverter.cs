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
                
                decimal number = decimal.Parse(stringValue.Trim(','));
                if (decimal.TryParse(stringValue, out number))
                {
                    string[] parts = number.ToString("N").Split('.');
                    string formattedValue = parts[0];
                    return formattedValue;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
