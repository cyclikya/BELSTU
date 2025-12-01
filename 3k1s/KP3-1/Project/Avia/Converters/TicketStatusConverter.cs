using Avia.Data.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Avia.Converters;

public class TicketStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Active => "Активен",
                TicketStatus.Cancelled => "Отменён",
                _ => status.ToString()
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
                "Активен" => TicketStatus.Active,
                "Отменён" => TicketStatus.Cancelled,
                _ => Enum.Parse<TicketStatus>(str, true)
            };
        }
        throw new NotImplementedException();
    }
}

