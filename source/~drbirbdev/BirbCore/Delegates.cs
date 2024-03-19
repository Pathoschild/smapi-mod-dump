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
using Object = StardewValley.Object;

namespace BirbCore;

[SDelegate]
internal class Delegates
{

    [SDelegate.ResolveItemQuery]
    public static IList<ItemQueryResult> CustomFlavoredItem(string key, string arguments, ItemQueryContext _, bool _1, HashSet<string> _2, Action<string, string> logError)
    {
        string[] splitArgs = ItemQueryResolver.Helpers.SplitArguments(arguments);
        if (!ArgUtility.TryGet(splitArgs, 0, out string flavoredItemId, out string error) ||
            !ArgUtility.TryGet(splitArgs, 1, out string ingredientItemId, out error) ||
            !ArgUtility.TryGetOptional(splitArgs, 2, out string ingredientPreservedItemId, out error))
        {
            return ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, error);
        }

        Object ingredient = ItemRegistry.Create(ingredientPreservedItemId ?? ingredientItemId) as Object;

        if (ItemRegistry.Create(flavoredItemId) is not Object flavored)
        {
            return null;
        }

        if (ingredient == null)
        {
            return new ItemQueryResult[]
            {
                new(flavored)
            };
        }

        flavored.Name += ingredient.Name;
        // flavored.preserve.Value = ...
        flavored.preservedParentSheetIndex.Value = ingredient.ItemId;

        return new ItemQueryResult[]
        {
            new(flavored)
        };

    }
}
