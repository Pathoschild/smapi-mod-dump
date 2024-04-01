/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using StardewValley;

namespace ProductionStats.Containers;

internal record ManagedStorage(IStorageContainer container)
{
    public IEnumerable<Item> GetItemsForCurrentPlayer()
    {
        return container.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
    }
}