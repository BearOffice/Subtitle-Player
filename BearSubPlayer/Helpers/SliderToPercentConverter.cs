using System.Globalization;
using System.Windows.Data;

namespace BearSubPlayer.Helpers;

public class SliderToPercentConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 &&
            values[0] is double value &&
            values[1] is double maximum &&
            maximum != 0)
        {
            double percent = (value / maximum) * 100;
            return $"{Math.Round(percent)}%";
        }
        return "0%";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}