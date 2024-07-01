/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace AchtuurCore.Patches;

internal class WateringPatcher : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        // Prefix patch
        harmony.Patch(
            original: GetOriginalMethod<HoeDirt>(nameof(HoeDirt.performToolAction)),
            prefix: GetHarmonyMethod(nameof(this.prefix_performToolAction))
        );

        // Postfix patch
        harmony.Patch(
            original: GetOriginalMethod<HoeDirt>(nameof(HoeDirt.performToolAction)),
            postfix: GetHarmonyMethod(nameof(this.postfix_performToolAction))
        );
    }
    private static void prefix_performToolAction(Tool t, HoeDirt __instance, out WateringInfo __state)
    {
        __state = new WateringInfo();
        try
        {
            __state.soilStateBefore = __instance.state.Value;
            __state.toolUsed = t;
            __state.toolHeld = Game1.player.CurrentTool;
            __state.location = __instance.Location;
        }
        catch (Exception e)
        {
            ModEntry.Instance.Monitor.Log($"Something went wrong when prefix patching performToolAction (WateringPatcher):\n{e}", LogLevel.Error);
        }
    }

    private static void postfix_performToolAction(ref HoeDirt __instance, WateringInfo __state)
    {
        try
        {
            // If tool was not a watering can, return
            if (__state.toolUsed is null || !__state.toolUsed.Name.ToLower().Contains("watering can"))
                return;

            // If some other mod tries to fake watering by acting as if a watering can was used, return
            if (__state.toolHeld is not null && __state.toolHeld.Name != __state.toolUsed.Name ||
                __state.location != Game1.player.currentLocation)
                return;


            // Tile has been watered -> call watering soil event
            if (__state.soilStateBefore != 1 && __instance.state.Value == 1)
            {
                Farmer lastUser = __state.toolUsed.getLastFarmerToUse();
                WateringFinishedArgs args = new WateringFinishedArgs(lastUser, __instance);
                ModEntry.EventManager.FinishedWateringSoil.Invoke(null, args);
            }
        }
        catch (Exception e)
        {
            Logger.ErrorLog(ModEntry.Instance.Monitor, $"Something went wrong when postfix patching performToolAction (WateringPatcher):\n{e}");
        }
    }
}

struct WateringInfo
{
    /// <summary>
    /// Tracks whether soil state changes to 1 when calling <see cref="HoeDirt.performToolAction"/> 
    /// </summary>
    internal int soilStateBefore;

    internal GameLocation location;

    /// <summary>
    /// Tool used according to the performToolAction method
    /// </summary>
    internal Tool toolUsed;

    /// <summary>
    /// Tool currently held by the player
    /// </summary>
    internal Tool toolHeld;
}
