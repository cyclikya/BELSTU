using Avia.Data.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Avia.Data.Converters;

public static class EnumConverters
{
    public static ValueConverter<RoleType, string> RoleTypeConverter => new(
        v => v.ToString().ToLowerInvariant(),
        v => Enum.Parse<RoleType>(v, true));

    public static ValueConverter<ClassType, string> ClassTypeConverter => new(
        v => v.ToString().ToLowerInvariant(),
        v => Enum.Parse<ClassType>(v, true));

    public static ValueConverter<TicketStatus, string> TicketStatusConverter => new(
        v => v.ToString().ToLowerInvariant(),
        v => Enum.Parse<TicketStatus>(v, true));
}

