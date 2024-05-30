/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using StardewValley;
using Object = StardewValley.Object;
using SpaceCore;

namespace BuffProfessions.Core
{
    [HarmonyPatch(typeof(StardewValley.Object), nameof(Object.OutputMachine))]
    class BlackSmithOutput_patch
    {
        [HarmonyLib.HarmonyPostfix]
        private static void Postfix(
        StardewValley.Object __instance, MachineData machine, MachineOutputRule outputRule, Item inputItem, Farmer who, GameLocation location, bool probe)
        {
            bool furnace = __instance.QualifiedItemId.Equals("(BC)13") || __instance.QualifiedItemId.Equals("(BC)HeavyFurnace");
            if (furnace &&
                __instance.heldObject.Value != null &&
                who.professions.Contains(20))
            {
                __instance.heldObject.Value.Stack *= 2;
            }
        }
    }
}
