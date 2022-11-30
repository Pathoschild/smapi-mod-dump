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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Crop), "harvest")]
    internal class DisableCropQualityPatch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new(instructions);

            int whereToInsert = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ble_Un_S)
                {
                    whereToInsert = i;
                    break;
                }
            }

            if (whereToInsert > 0)
            {
                codes.Insert(whereToInsert, new(OpCodes.Ldc_I4_0));
                codes.Insert(whereToInsert, new(OpCodes.Stloc_S, 7));
            }

            return codes.AsEnumerable();
        }
    }
}
