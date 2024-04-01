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
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.GetRadiusForScarecrow))]
    public static class ScarecrowPatchesV2
    {
        public static double GetScarecrowRadiusMultiplier(this SObject Obj)
        {
            if (Obj.IsScarecrow() && Obj.IsCombinedMachine())
            {
                double TilesMultiplier = Obj.GetProcessingPower();
                double RadiusMultiplier = Math.Sqrt(TilesMultiplier);
                return RadiusMultiplier;
            }
            else
                return 1.0;
        }

        public static void Postfix(SObject __instance, ref int __result)
        {
            try
            {
                if (__instance.TryGetCombinedQuantity(out int Qty))
                {
                    int BaseRadius = __result;
                    double RadiusMultiplier = __instance.GetScarecrowRadiusMultiplier();
                    double Result = BaseRadius * RadiusMultiplier;
                    __result = (int)Math.Round(Result, MidpointRounding.AwayFromZero);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log($"Unhandled Error in {nameof(ScarecrowPatchesV2)}.{nameof(Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
