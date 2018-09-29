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