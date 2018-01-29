using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using XSystem.Core.Domain;

namespace XSystem.Reader
{
    public class ActorImgConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return new BitmapImage(new Uri($"pack://siteoforigin:,,,/{Actor.ShotDirName}/{value}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}