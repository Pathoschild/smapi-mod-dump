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
using BirbCore.Attributes;
using StardewValley;
using static StardewValley.GameStateQuery;

namespace RealtimeFramework;

[SDelegate]
internal class Delegates
{
    [SDelegate.GameStateQuery]
    public static bool IsHoliday(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(query, error);
        }

        return ModEntry.API.IsHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsComingHoliday(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(query, error);
        }

        return ModEntry.API.IsComingHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsCurrentHoliday(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(query, error);
        }

        return ModEntry.API.IsCurrentHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsPassingHoliday(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(query, error);
        }

        return ModEntry.API.IsPassingHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsHoliday(GameLocation location, string eventId, string[] args)
    {
        if (!ArgUtility.TryGet(args, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(args, error);
        }

        return ModEntry.API.IsHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsComingHoliday(GameLocation location, string eventId, string[] args)
    {
        if (!ArgUtility.TryGet(args, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(args, error);
        }

        return ModEntry.API.IsComingHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsCurrentHoliday(GameLocation location, string eventId, string[] args)
    {
        if (!ArgUtility.TryGet(args, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(args, error);
        }

        return ModEntry.API.IsCurrentHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsPassingHoliday(GameLocation location, string eventId, string[] args)
    {
        if (!ArgUtility.TryGet(args, 1, out string value, out string error))
        {
            return Helpers.ErrorResult(args, error);
        }

        return ModEntry.API.IsPassingHoliday(value);
    }

    [SDelegate.TokenParser]
    public static bool HolidayName(string[] query, out string replacement, Random random, Farmer player)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return TokenParser.LogTokenError(query, error, out replacement);
        }

        replacement = ModEntry.API.GetLocalName(value);
        return ModEntry.API.IsHoliday(value);
    }

}
