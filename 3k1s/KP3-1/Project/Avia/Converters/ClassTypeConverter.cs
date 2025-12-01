using Avia.Data.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Avia.Converters;

public class ClassTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ClassType classType)
        {
            return classType switch
            {
                ClassType.Economy => "Эконом",
                ClassType.Business => "Бизнес",
                _ => classType.ToString()
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str switch
            {
                "Эконом" => ClassType.Economy,
                "Бизнес" => ClassType.Business,
                _ => Enum.Parse<ClassType>(str, true)
            };
        }
        throw new NotImplementedException();
    }
}

