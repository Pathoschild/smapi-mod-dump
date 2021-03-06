/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Helpers;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.maximumStackSize))]
    public static class MaximumStackSizePatch
    {
        public static bool Prefix(SObject __instance, ref int __result)
        {
            try
            {
                //  Always return maximumStackSize = 1 if the machine has been merged with another machine
                if (__instance.IsCombinedMachine())
                {
                    __result = 1;
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MaximumStackSizePatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.canStackWith))]
    public static class CanStackWithPatch
    {
        public static bool Prefix(SObject __instance, ISalable other, ref bool __result)
        {
            try
            {
                //  Always return false if the machine has been merged with another machine
                if (__instance.IsCombinedMachine())
                {
                    __result = false;
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(CanStackWithPatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.performRemoveAction))]
    public static class PerformRemoveActionPatch
    {
        public static bool Prefix(SObject __instance, Vector2 tileLocation, GameLocation environment)
        {
            try
            {
                //  Example: If you combined 3 furnaces together and placed it on a tile, then when you remove it, the Game will already refund 1 of them, so this patch is intended to
                //  refund the other 2 combined quantity
                if (__instance.IsCombinableObject() && __instance.TryGetCombinedQuantity(out int CombinedQuantity) && CombinedQuantity > 1)
                {
                    SObject CombinedRefund = new SObject(Vector2.Zero, __instance.ParentSheetIndex, false) { Stack = CombinedQuantity - 1 };
                    Game1.createItemDebris(CombinedRefund, tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4, null, -1);
                }
                return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(PerformRemoveActionPatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }
    }
}
