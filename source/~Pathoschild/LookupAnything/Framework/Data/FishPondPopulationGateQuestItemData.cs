/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>An item required to unlock a fish pond population gate.</summary>
    /// <param name="ItemID">The item ID.</param>
    /// <param name="MinCount">The minimum number of the item that may be requested.</param>
    /// <param name="MaxCount">The maximum number of the item that may be requested.</param>
    internal record FishPondPopulationGateQuestItemData(int ItemID, int MinCount, int MaxCount);
}
