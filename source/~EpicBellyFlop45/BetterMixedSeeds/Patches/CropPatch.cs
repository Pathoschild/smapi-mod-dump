using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterMixedSeeds.Patches
{
    [HarmonyPatch]
    internal class CropPatch
    {
        private static MethodBase TargetMethod()
        {
            return ModEntry.GetSDVType("Crop").GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) });
        }

        /// <summary>
        /// Change the condition for the seed index from 473 to 1 (this was preventing Green Beans from being planted as it would decrement the number).
        /// </summary>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool changed = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!changed && instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 473)
                {
                    // Changes the condition of the if to check for id of 1 instead of 473 (an Id that will never be returned by the method patch)
                    changed = true;
                    instruction.operand = 1;
                }

                yield return instruction;
            }
        }
    }
}
