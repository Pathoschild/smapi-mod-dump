/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/wrongcoder/UnscheduledEvaluation
**
*************************************************/

using StardewValley;
using xTile.Dimensions;

namespace UnscheduledEvaluation;

// ReSharper disable InconsistentNaming
public class AlwaysActiveShrinePatch
{
    public struct PatchState
    {
        internal bool shrineAction;
        internal int year;
        internal int grandpaScore;
    }

    public static void Prefix(out PatchState __state, Farm __instance, Location tileLocation, Farmer who)
    {
        __state.shrineAction = isShrineAction(__instance, tileLocation);
        __state.year = Game1.year;
        __state.grandpaScore = __instance.grandpaScore.Value;
        if (!__state.shrineAction) return;
        if (doReplaceYear(Game1.year)) Game1.year = 3;
        if (doReplaceGrandpaScore(__instance.grandpaScore.Value)) __instance.grandpaScore.Value = 1;
    }

    public static void Postfix(PatchState __state, Farm __instance, Farmer who)
    {
        if (!__state.shrineAction) return;
        if (doReplaceYear(__state.year)) Game1.year = __state.year;
        if (doReplaceGrandpaScore(__state.grandpaScore)) __instance.grandpaScore.Value = __state.grandpaScore;
    }

    private static bool isShrineAction(Farm farm, Location tileLocation)
    {
        var grandpaShrinePosition = farm.GetGrandpaShrinePosition();
        return tileLocation.X >= grandpaShrinePosition.X - 1
               && tileLocation.X <= grandpaShrinePosition.X + 1
               && tileLocation.Y == grandpaShrinePosition.Y;
    }

    private static bool doReplaceYear(int oldYear)
    {
        return oldYear < 3;
    }

    private static bool doReplaceGrandpaScore(int oldGrandpaScore)
    {
        return oldGrandpaScore < 1 | oldGrandpaScore >= 4;
    }
}