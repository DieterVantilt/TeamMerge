﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace TeamMergeBase.Converters
{
    public class NullableToBooleanConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}