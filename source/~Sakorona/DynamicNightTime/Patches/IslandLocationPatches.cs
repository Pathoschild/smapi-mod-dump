/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;


namespace DynamicNightTime.Patches
{
    class IslandLocationPatches
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static IEnumerable<CodeInstruction> DrawParallaxHorizonTranspiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var codes = new List<CodeInstruction>(instructions);
                       
            var startIndex = -1;
            var endIndex = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_2 && codes[i+1].opcode == OpCodes.Ldsfld)
                {
                    startIndex = i+1;
                }

                if (codes[i].opcode == OpCodes.Stloc_S && (codes[i].operand as LocalBuilder).LocalIndex == 6 && i+1 < codes.Count && codes[i+1].opcode == OpCodes.Ldloc_S && (codes[i+1].operand as LocalBuilder).LocalIndex == 6)
                {
                    endIndex= i;
                }
            }
            //now that we've found the insert ranges
            codes[startIndex].opcode = OpCodes.Call;
            codes[startIndex].operand = AccessTools.Method(typeof(DynamicNightTime), "CalculateDayNightRatio", new Type[] { });

            startIndex++;
            for (int i = startIndex; i < endIndex; i++)
            {
                codes[i].opcode = OpCodes.Nop;
                codes[i].operand = null;
            }

            return codes.AsEnumerable();
        }
    }
}
