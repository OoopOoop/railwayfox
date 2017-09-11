using System;
using System.Globalization;
using System.Windows.Data;
using ToyTrainProject.Models;

namespace ToyTrainProject.Converters
{
    public class DeviceInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var filterInfo = value as DeviceInfo;
                if (filterInfo != null)
                {
                    return filterInfo.UsbString;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
