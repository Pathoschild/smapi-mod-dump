/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using static PlacementPlus.ModState;

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(Chest), nameof(Chest.performObjectDropInAction))]
    internal class ChestPatches
    {
        private static Color originalColor = Color.White;

        private static bool Prefix(Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result, Chest __instance)
        {
            originalColor = __instance.playerChoiceColor.Value;

            // We do nothing when probing as we do not want to show the invalid placement square when trying to swap a
            // chest of the same type!
            if (probe || dropInItem == null) return true;

            __result = !dropInItem.ItemId.Equals(__instance.ItemId);
            return __result; // Skip original logic if the chests are the same.

        }


        private static void Postfix(Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result, Chest __instance)
        {
            try
            {
                if (!__result || probe || dropInItem == null) return;

                // Do nothing if chests are not swappable.
                if (!(dropInItem.HasContextTag("swappable_chest")
                      && __instance.HasContextTag("swappable_chest")
                      && dropInItem.Name.Contains(nameof(Chest))
                      && (dropInItem.Name.Contains("Big") || !__instance.ItemId.Contains("Big"))))
                {
                    return;
                }
                ((Chest) __instance.Location.Objects[__instance.TileLocation]).playerChoiceColor.Value = originalColor;
            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(ChestPatches)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
