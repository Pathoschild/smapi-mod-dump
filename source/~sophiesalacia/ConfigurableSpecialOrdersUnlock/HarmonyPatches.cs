/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;

namespace ConfigurableSpecialOrdersUnlock;

public class HarmonyPatches
{
    private static readonly Harmony Harmony = new(Globals.Manifest.UniqueID);

    ///<summary>
    ///Attempts to Harmony patch the following:
    ///<br />SpecialOrder.IsSpecialOrdersBoardUnlocked &#8594; HarmonyPatches.IsSpecialOrdersBoardUnlocked_Prefix
    ///</summary>
    /// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
    public static bool ApplyHarmonyPatches()
    {
        try
        {

            Harmony.Patch(
                original: typeof(SpecialOrder).GetMethod("IsSpecialOrdersBoardUnlocked"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod("IsSpecialOrdersBoardUnlocked_Prefix"))
            );

            return true;
        }
        catch (Exception e)
        {
            Log.Error(e);
            return false;
        }
    }

    /// <summary>
    /// Harmony patch for <c>SpecialOrder.IsSpecialOrdersBoardUnlocked</c>. Overwrites the standard check with a configurable custom check.
    /// </summary>
    /// <param name="__result"> The modified output to pass to the caller. <c>True</c> if the configured settings are met, <c>False</c> otherwise.</param>
    /// <returns>
    /// <c>True</c> if unable to patch, so that the original method runs instead.
    /// <c>False</c> if successfully patched, in order to skip the original method.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public static bool IsSpecialOrdersBoardUnlocked_Prefix(ref bool __result)
    {
        try
        {
            __result = Game1.stats.DaysPlayed >= Globals.Config.GetUnlockDaysPlayed();
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_Prefix)}:\n{ex}");
            return true; // run original logic
        }
    }
}
