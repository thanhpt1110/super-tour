using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Super_Tour
{
    public class DecimalToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                string formattedValue = decimalValue.ToString("#,##0");
                return formattedValue;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Xóa dấu phẩy trong chuỗi
                string decimalString = stringValue.Replace(".", string.Empty);

                if (decimal.TryParse(decimalString, out decimal decimalValue))
                {
                    return decimalValue;
                }
            }

            return DependencyProperty.UnsetValue; // Giá trị không hợp lệ
        }
    }
}
