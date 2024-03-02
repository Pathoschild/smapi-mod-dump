/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Objects;

namespace DynamicDialogues;

/// <summary>
/// Custom queries.
/// </summary>
/// <see cref="GameStateQuery.DefaultResolvers"/>
internal static class Queries
{
    internal static bool EventLocked(string[] query, GameStateQueryContext context)
    {
        if (Game1.weddingToday || Game1.weddingsToday?.Count != 0)
            return true;

        return ModEntry.EventLock;
    }

    public static bool Wearing(string[] query, GameStateQueryContext context)
    {
        //<q> <player> <type> <item> [only worn]

        var p = !ArgUtility.TryGet(query, 1, out var playerKey, out var error);
        var t = !ArgUtility.TryGet(query, 2, out var type, out error);
        var i = !ArgUtility.TryGet(query, 3, out var item, out error);
        ArgUtility.TryGetOptionalBool(query, 4, out var onlyWorn, out _);
        var invalid = p || t || i;

        type = type.Replace("(", "");
        
        var hat = type.StartsWith("H", StringComparison.OrdinalIgnoreCase);
        var shirt = type.StartsWith("S", StringComparison.OrdinalIgnoreCase);
        var ring = type.StartsWith("R", StringComparison.OrdinalIgnoreCase);
        var pants = type.StartsWith("P", StringComparison.OrdinalIgnoreCase);
        var boots = type.StartsWith("B", StringComparison.OrdinalIgnoreCase);
        
        return invalid ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, target =>
        {
            if(hat)
                return target.hat.Value.ItemId == item;
            if (shirt)
                return onlyWorn ? target.shirtItem.Value.ItemId == item : target.GetShirtId() == item;
            if (ring)
                return target.isWearingRing(item);
            if (pants)
                return onlyWorn ? target.pantsItem.Value.ItemId == item : target.GetPantsId() == item;
            if (boots)
                return target.boots.Value.ItemId == item;
            
            return false;
        });
    }
}