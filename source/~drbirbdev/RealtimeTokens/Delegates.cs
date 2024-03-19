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
using StardewValley.Delegates;
using static StardewValley.GameStateQuery;

namespace RealtimeFramework;

[SDelegate]
internal class Delegates
{
    [SDelegate.GameStateQuery]
    public static bool IsHoliday(string[] query, GameStateQueryContext _)
    {
        return !ArgUtility.TryGet(query, 1, out string value, out string error)
            ? Helpers.ErrorResult(query, error)
            : ModEntry.Api.IsHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsComingHoliday(string[] query, GameStateQueryContext _)
    {
        return !ArgUtility.TryGet(query, 1, out string value, out string error)
            ? Helpers.ErrorResult(query, error)
            : ModEntry.Api.IsComingHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsCurrentHoliday(string[] query, GameStateQueryContext _)
    {
        return !ArgUtility.TryGet(query, 1, out string value, out string error)
            ? Helpers.ErrorResult(query, error)
            : ModEntry.Api.IsCurrentHoliday(value);
    }

    [SDelegate.GameStateQuery]
    public static bool IsPassingHoliday(string[] query, GameStateQueryContext _)
    {
        return !ArgUtility.TryGet(query, 1, out string value, out string error)
            ? Helpers.ErrorResult(query, error)
            : ModEntry.Api.IsPassingHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsHoliday(GameLocation _, string _1, string[] args)
    {
        return !ArgUtility.TryGet(args, 1, out string value, out string error)
            ? Helpers.ErrorResult(args, error)
            : ModEntry.Api.IsHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsComingHoliday(GameLocation _, string _1, string[] args)
    {
        return !ArgUtility.TryGet(args, 1, out string value, out string error)
            ? Helpers.ErrorResult(args, error)
            : ModEntry.Api.IsComingHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsCurrentHoliday(GameLocation _, string _1, string[] args)
    {
        return !ArgUtility.TryGet(args, 1, out string value, out string error)
            ? Helpers.ErrorResult(args, error)
            : ModEntry.Api.IsCurrentHoliday(value);
    }

    [SDelegate.EventPrecondition]
    public static bool IsPassingHoliday(GameLocation _, string _1, string[] args)
    {
        return !ArgUtility.TryGet(args, 1, out string value, out string error)
            ? Helpers.ErrorResult(args, error)
            : ModEntry.Api.IsPassingHoliday(value);
    }

    [SDelegate.TokenParser]
    public static bool HolidayName(string[] query, out string replacement, Random _, Farmer _1)
    {
        if (!ArgUtility.TryGet(query, 1, out string value, out string error))
        {
            return StardewValley.TokenizableStrings.TokenParser.LogTokenError(query, error, out replacement);
        }

        replacement = ModEntry.Api.GetLocalName(value);
        return ModEntry.Api.IsHoliday(value);
    }
}
