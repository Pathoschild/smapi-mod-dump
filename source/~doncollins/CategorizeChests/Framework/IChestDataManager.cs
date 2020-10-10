/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewValley.Objects;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// An interface for retrieving the mod-specific data associated with a
    /// given Stardew Valley chest object.
    /// </summary>
    interface IChestDataManager
    {
        ChestData GetChestData(Chest chest);
    }
}