/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;
using AtraBase.Models.Result;
using AtraBase.Models.WeightedRandom;

using AtraCore.Framework.ItemManagement;

using AtraShared.ItemManagement;

using StardewModdingAPI.Events;

namespace CatGiftsRedux.Framework;

/// <summary>
/// Manages assets for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName path = null!;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
        => path = parser.ParseAssetName("Mods/atravita/CatGiftsRedux/Data");

    /// <summary>
    /// Applies the asset edits.
    /// </summary>
    /// <param name="e">Asset requested event args.</param>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(path))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, WeightedItemData>, AssetLoadPriority.Exclusive);
        }
    }

    /// <summary>
    /// Picks an item from the mod-added list.
    /// </summary>
    /// <param name="random">The seeded random to use.</param>
    /// <returns>An item, or null to skip.</returns>
    internal static Item? Pick(Random random)
    {
        ModEntry.ModMonitor.Log("Picking from mod added data.");

        Dictionary<string, WeightedItemData>.ValueCollection? data = Game1.temporaryContent.Load<Dictionary<string, WeightedItemData>>(path.BaseName).Values;

        if (data.Count == 0)
        {
            return null;
        }

        WeightedManager<ItemRecord?> manager = new(data.Select(item => new WeightedItem<ItemRecord?>(item.Weight, item.Item)));

        if (!manager.GetValueUncached(random).TryGetValue(out ItemRecord? entry) || entry is null)
        {
            return null;
        }

        if (!int.TryParse(entry.Identifier, out int id))
        {
            id = DataToItemMap.GetID(entry.Type, entry.Identifier);
        }

        if (id > 0)
        {
            return ItemUtils.GetItemFromIdentifier(entry.Type, id);
        }
        return null;
    }
}
