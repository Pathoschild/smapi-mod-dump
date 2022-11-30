/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(SObject), "performObjectDropInAction")]
    internal class ObjectDropInPatch
    {
        public static bool Prefix(SObject __instance, ref bool __result, Item dropInItem, bool probe, Farmer who)
        {
            if (__instance.Name != "Deconstructor")
                return true;

            if (__instance.isTemporarilyInvisible)
                return true;

            if (__instance.heldObject.Value is not null)
            {
                Game1.showRedMessage("Deconstructor is already in use.");
                return false;
            }

            if (dropInItem is not null)
            {
                SObject deconstructor_output = __instance.GetDeconstructorOutput(dropInItem);
                if (deconstructor_output != null)
                {
                    __instance.heldObject.Value = new SObject(dropInItem.ParentSheetIndex, 1);
                    if (!probe)
                    {
                        __instance.heldObject.Value = deconstructor_output;
                        __instance.MinutesUntilReady = 5;
                        Game1.playSound("furnace");
                        __result = true;
                        return false;
                    }
                    __result = true;
                    return false;
                }
                if (!probe)
                {
                    if (SObject.autoLoadChest == null)
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Deconstructor_fail"));

                    return false;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SObject), "GetDeconstructorOutput")]
    internal class DeconstructorOutputPatch
    {
        public static readonly List<int> InvalidItemIndices = new()
        {
            384,  // Gold Ore
            39,   // Dark Sign
            322   // Wood Fence
        };

        public static bool Prefix(ref SObject? __result, Item item)
        {
            if (InvalidItemIndices.Contains(item.ParentSheetIndex) || item is Pickaxe)
            {
                __result = null;
                return false;
            }

            __result = new(384, 3);

            return false;
        }
    }
}
