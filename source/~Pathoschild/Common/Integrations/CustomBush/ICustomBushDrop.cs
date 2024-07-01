/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData;

namespace Pathoschild.Stardew.Common.Integrations.CustomBush
{
    /// <summary>An item produced by a Custom Bush bush.</summary>
    public interface ICustomBushDrop : ISpawnItemData
    {
        /// <summary>Gets the specific season when the item can be produced.</summary>
        Season? Season { get; }

        /// <summary>Gets the probability that the item will be produced.</summary>
        float Chance { get; }

        /// <summary>A game state query which indicates whether the item should be added. Defaults to always added.</summary>
        string? Condition { get; }

        /// <summary>An ID for this entry within the current list (not the item itself, which is <see cref="P:StardewValley.GameData.GenericSpawnItemData.ItemId" />). This only needs to be unique within the current list. For a custom entry, you should use a globally unique ID which includes your mod ID like <c>ExampleMod.Id_ItemName</c>.</summary>
        string? Id { get; }
    }
}
