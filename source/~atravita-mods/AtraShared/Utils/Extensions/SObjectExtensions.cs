/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions for SObject.
/// </summary>
public static class SObjectExtensions
{
    /// <summary>
    /// Gets whether or not an SObject is a trash item.
    /// </summary>
    /// <param name="obj">SObject to check.</param>
    /// <returns>true if it's a trash item, false otherwise.</returns>
    public static bool IsTrashItem(this SObject obj)
        => obj is not null && !obj.bigCraftable.Value && (obj.ParentSheetIndex >= 168 && obj.ParentSheetIndex < 173);

    /// <summary>
    /// Gets the public name of a bigcraftable.
    /// </summary>
    /// <param name="bigCraftableIndex">Bigcraftable.</param>
    /// <returns>public name if found.</returns>
    public static string GetBigCraftableName(this int bigCraftableIndex)
    {
        if (Game1.bigCraftablesInformation.TryGetValue(bigCraftableIndex, out string? value))
        {
            int index = value.IndexOf('/');
            if (index >= 0)
            {
                return value[..index];
            }
        }
        return "ERROR - big craftable not found!";
    }

    /// <summary>
    /// Gets the translated name of a bigcraftable.
    /// </summary>
    /// <param name="bigCraftableIndex">Index of the bigcraftable.</param>
    /// <returns>Name of the bigcraftable.</returns>
    public static string GetBigCraftableTranslatedName(this int bigCraftableIndex)
    {
        if (Game1.bigCraftablesInformation.TryGetValue(bigCraftableIndex, out string? value))
        {
            int index = value.LastIndexOf('/');
            if (index >= 0)
            {
                return value[(index + 1)..];
            }
        }
        return "ERROR - big craftable not found!";
    }
}