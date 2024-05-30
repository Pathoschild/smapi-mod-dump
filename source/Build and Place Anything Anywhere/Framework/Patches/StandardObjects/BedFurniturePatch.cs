/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal sealed class BedFurniturePatch : PatchHelper
    {
        internal BedFurniturePatch() : base(typeof(BedFurniture)) { }
        internal void Apply()
        {
            Patch(PatchType.Postfix, nameof(BedFurniture.CanModifyBed), nameof(CanModifyBedPostfix), [typeof(Farmer)]);
            Patch(PatchType.Transpiler, nameof(BedFurniture.placementAction), nameof(PlacementActionTranspiler));
        }

        // Enable modifying other players beds/placing inside of other players homes
        private static void CanModifyBedPostfix(BedFurniture __instance, Farmer who, ref bool __result)
        {
            if (!ModEntry.Config.EnableFreePlace)
                __result = true;
        }

        // Enable all beds indoors
        private static IEnumerable<CodeInstruction> PlacementActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var placementActionTranspiler = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(placementActionTranspiler, generator);

                matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Property(typeof(FarmHouse), nameof(FarmHouse.upgradeLevel)).GetGetMethod()),
                    new CodeMatch(OpCodes.Ldc_I4_2))
                    .Set(OpCodes.Ldc_I4_0, null)
                    .MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Property(typeof(FarmHouse), nameof(FarmHouse.upgradeLevel)).GetGetMethod()),
                    new CodeMatch(OpCodes.Ldc_I4_1))
                    .Set(OpCodes.Ldc_I4_0, null)
                    .ThrowIfNotMatch("Could not find get_upgradeLevel()");

                return matcher.InstructionEnumeration();
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"There was an issue modifying the instructions for {typeof(BedFurniture)}.{original.Name}: {e}", LogLevel.Error);
                return placementActionTranspiler;
            }
        }
    }
}