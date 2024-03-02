/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace BirbCore.Extensions;
public static class CustomFieldsExtensions
{
    public static int? TryGetInt(this Dictionary<string, string>? customFields, string key)
    {
        if (customFields == null)
        {
            return null;
        }
        if (!customFields.TryGetValue(key, out string value))
        {
            return null;
        }
        if (!int.TryParse(value, out int result))
        {
            return null;
        }
        return result;
    }
}
