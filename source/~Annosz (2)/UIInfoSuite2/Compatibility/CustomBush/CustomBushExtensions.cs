/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using UIInfoSuite2.Infrastructure.Helpers;

namespace UIInfoSuite2.Compatibility.CustomBush;

internal static class CustomBushExtensions
{
  private const string ShakeOffItem = $"{ModCompat.CustomBush}/ShakeOff";

  public static bool GetShakeOffItemIfReady(
    this ICustomBush customBush,
    Bush bush,
    [NotNullWhen(true)] out ParsedItemData? item
  )
  {
    item = null;
    if (bush.size.Value != Bush.greenTeaBush)
    {
      return false;
    }

    if (!bush.modData.TryGetValue(ShakeOffItem, out string itemId))
    {
      return false;
    }

    item = ItemRegistry.GetData(itemId);
    return true;
  }

  public static List<PossibleDroppedItem> GetCustomBushDropItems(
    this ICustomBushApi api,
    ICustomBush bush,
    string? id,
    bool includeToday = false
  )
  {
    if (id == null || string.IsNullOrEmpty(id))
    {
      return new List<PossibleDroppedItem>();
    }

    api.TryGetDrops(id, out IList<ICustomBushDrop>? drops);
    return drops == null
      ? new List<PossibleDroppedItem>()
      : DropsHelper.GetGenericDropItems(drops, id, includeToday, bush.DisplayName, BushDropConverter);

    DropInfo BushDropConverter(ICustomBushDrop input)
    {
      return new DropInfo(input.Condition, input.Chance, input.ItemId);
    }
  }
}
