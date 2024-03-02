/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using BirbCore.Attributes;
using StardewValley;
using StardewValley.Internal;

namespace BirbCore;

[SDelegate]
internal class Delegates
{

    [SDelegate.ResolveItemQuery]
    public static IList<ItemQueryResult> CustomFlavoredItem(string key, string arguments, ItemQueryContext context, bool avoidRepeat, HashSet<string> avoidItemIds, Action<string, string> logError)
    {
        string[] splitArgs = ItemQueryResolver.Helpers.SplitArguments(arguments);
        if (!ArgUtility.TryGet(splitArgs, 0, out string flavoredItemId, out string error) ||
            !ArgUtility.TryGet(splitArgs, 1, out string ingredientItemId, out error) ||
            !ArgUtility.TryGetOptional(splitArgs, 2, out string ingredientPreservedItemId, out error))
        {
            return ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, error);
        }

        StardewValley.Object ingredient = ItemRegistry.Create(ingredientPreservedItemId ?? ingredientItemId) as StardewValley.Object;

        StardewValley.Object flavored = ItemRegistry.Create(flavoredItemId, 1) as StardewValley.Object;
        flavored.Name += ingredient.Name;
        // flavored.preserve.Value = ...
        flavored.preservedParentSheetIndex.Value = ingredient.ItemId;

        return new ItemQueryResult[1]
        {
            new ItemQueryResult(flavored)
        };
    }
}
