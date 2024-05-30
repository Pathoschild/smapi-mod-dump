/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/SVMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace HopperPlus;

public class Mod: StardewModdingAPI.Mod
{
    
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.minutesElapsed)),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.MinutesElapsedPostfix))
        );
    }

    private class Game1Patcher
    {
        private static readonly Dictionary<int, bool> HopperMessageCache = [];

        public static void MinutesElapsedPostfix(Object __instance)
        {
            if (!Game1.player.IsMainPlayer) {
                return;
            }
            if (__instance.Location is null || __instance.GetMachineData() is null) {
                return;
            }
            __instance.Location.objects.TryGetValue(new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f), out var fromObj);
            if (fromObj is not StardewValley.Objects.Chest hopper || hopper.specialChestType.Value != StardewValley.Objects.Chest.SpecialChestTypes.AutoLoader) {
                return;
            }
            var objectThatWasHeld = __instance.heldObject.Value;
            if (__instance.readyForHarvest.Value) {
                __instance.heldObject.Value = null;
                if (hopper.addItem(objectThatWasHeld) is null) {
                    __instance.playNearbySoundAll("coin");
                    MachineDataUtility.UpdateStats(__instance.GetMachineData()?.StatsToIncrementWhenHarvested, objectThatWasHeld, objectThatWasHeld.Stack);
                    __instance.heldObject.Value = null;
                    __instance.readyForHarvest.Value = false;
                    __instance.showNextIndex.Value = false;
                    __instance.ResetParentSheetIndex();
                    __instance.AttemptAutoLoad(Game1.player);
                    HopperMessageCache.Remove(__instance.GetHashCode());
                } else {
                    __instance.heldObject.Value = objectThatWasHeld;
                    if (HopperMessageCache.ContainsKey(__instance.GetHashCode())) {
                        return;
                    }
                    HopperMessageCache.Add(__instance.GetHashCode(), true);
                    Game1.showRedMessage("Hopper is full,can not collect more items!");
                }
            } else {
                if (objectThatWasHeld is null) {
                    __instance.AttemptAutoLoad(Game1.player);
                }
            }
        }
    }
}