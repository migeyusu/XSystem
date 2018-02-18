using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using XSystem.Core.Domain;

namespace XSystem.Reader
{
    public class FilmImgConverter : IValueConverter
    {
        static readonly BitmapImage NullBitmapImage =
            new BitmapImage(new Uri("pack://application:,,,/Resource/noimg.jpeg"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) {
                return NullBitmapImage;
            }
            return new BitmapImage(new Uri($"pack://siteoforigin:,,,/{Film.ShotDirPath}/{value}.jpeg")) {
                CacheOption = BitmapCacheOption.None
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}