/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using ConvenientChests.CategorizeChests.Framework.Persistence;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Framework
{
    /// <summary>
    /// A helper for finding the chest object corresponding to a given chest address.
    /// </summary>
    interface IChestFinder
    {
        Chest GetChestByAddress(ChestAddress address);
    }
}