using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using TeamMergeBase.Helpers;

namespace TeamMergeBase.Converters
{
    class EnumConverters
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Resources.NotSelected;

            if (value is Enum enumValue)
            {
                result = enumValue.GetDescription();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}