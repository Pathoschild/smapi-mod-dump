/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using System.ComponentModel;
using System.Reflection;

namespace ProductionStats;

internal static class EnumHelper
{
    /// <summary>
    /// Get the description for a given value of the enum.
    /// </summary>
    /// <param name="value">Value of the enum</param>
    /// <returns>
    ///     Description of the given enum. If description is not defined
    ///     returns empty string.
    /// </returns>
    public static string GetDescription(this Enum value)
    {
        string? name = Enum.GetName(value.GetType(), value);
        if (name is null)
        {
            return string.Empty;
        }

        FieldInfo? field = value.GetType().GetField(name);
        if (field is null)
        {
            return string.Empty;
        }

        DescriptionAttribute? attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attr?.Description ?? string.Empty;
    }
}
