/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Framework
{
    /// <summary>
    /// A tool for moving items in bulk from the player's inventory
    /// into a given chest according to that chest's settings.
    /// </summary>
    public interface IChestFiller
    {
        void DumpItemsToChest(Chest chest);
    }
}