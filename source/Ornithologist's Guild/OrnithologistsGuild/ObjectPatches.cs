/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using HarmonyLib;
using OrnithologistsGuild.Models;
using StardewModdingAPI;
using StardewValley;

namespace OrnithologistsGuild
{
	public partial class ObjectPatches
	{
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;

            // Shared
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.maximumStackSize)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(maximumStackSize_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performUseAction)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(performUseAction_Postfix))
            );

            // Life List
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.canBeShipped)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(canBeGenericFalse_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.canBeTrashed)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(canBeGenericFalse_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.canBeGivenAsGift)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(canBeGenericFalse_Postfix))
            );
            // Note: this applies to `Item`, not `Object`
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Item), nameof(StardewValley.Object.canBeDropped)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(canBeGenericFalse_Postfix))
            );

            // Binoculars
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.actionWhenBeingHeld)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(actionWhenBeingHeld_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.actionWhenStopBeingHeld)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(actionWhenStopBeingHeld_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawWhenHeld)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(drawWhenHeld_Postfix))
            );
        }

        public static void performUseAction_Postfix(StardewValley.Object __instance, GameLocation location, ref bool __result)
        {
            try
            {
                bool removeFromInventory = false;

                if (__instance.IsBinoculars()) UseBinoculars(__instance, location, out removeFromInventory);
                else if (__instance.QualifiedItemId == ID_LIFE_LIST) UseLifeList();

                if (removeFromInventory)
                {
                    __result = true;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(performUseAction_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void maximumStackSize_Postfix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (__instance.IsBinoculars() || __instance.QualifiedItemId == ID_LIFE_LIST)
                {
                    __result = 1;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(maximumStackSize_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
