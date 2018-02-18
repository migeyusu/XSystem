using System;
using System.Globalization;
using System.Windows.Data;
using XSystem.Core.Domain;

namespace XSystem.Reader
{
    public class RecommendLevelConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((RecommendLevel) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RecommendLevel) value;
        }
    }
}