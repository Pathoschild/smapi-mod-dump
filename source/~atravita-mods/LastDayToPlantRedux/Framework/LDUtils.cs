/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;
using AtraShared.Wrappers;

namespace LastDayToPlantRedux.Framework;
internal static class LDUtils
{
    /// <summary>
    /// Returns the id and type of an SObject, or null if not found.
    /// </summary>
    /// <param name="identifier">string identifier.</param>
    /// <returns>id/type tuple, or null for not found.</returns>
    internal static (int id, int type)? ResolveIDAndType(string identifier)
    {
        if (!int.TryParse(identifier, out int id))
        {
            id = DataToItemMap.GetID(ItemTypeEnum.SObject, identifier);
        }

        if (id < -1 || !Game1Wrappers.ObjectInfo.TryGetValue(id, out string? data))
        {
            ModEntry.ModMonitor.Log($"{identifier} could not be resolved, skipping");
            return null;
        }

        ReadOnlySpan<char> cat = data.GetNthChunk('/', SObject.objectInfoTypeIndex);
        int index = cat.GetIndexOfWhiteSpace();
        if (index < 0 || !int.TryParse(cat[(index + 1)..], out int type) || type is not SObject.fertilizerCategory or SObject.SeedsCategory)
        {
            ModEntry.ModMonitor.Log($"{identifier} with {id} does not appear to be a seed or fertilizer, skipping.");
            return null;
        }

        return (id, type);
    }
}
