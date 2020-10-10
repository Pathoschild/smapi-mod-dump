/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Harmony;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomTransparency.Patches
{
    [HarmonyPatch]
    internal class TreePatch
    {
        private static MethodBase TargetMethod()
        {
            return typeof(Tree).GetMethod(nameof(Tree.tickUpdate));
        }

        /// <summary>Change the first 0.4 in the method to the specified transparency in the config.</summary>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool changed = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!changed && instruction.opcode == OpCodes.Ldc_R4 &&
                    (float)instruction.operand <= 0.41f && (float)instruction.operand >= 0.39f)
                {
                    changed = true;
                    instruction.operand = ModEntry.Config.MinimumTreeTransparency;
                }

                yield return instruction;
            }
        }
    }
}
