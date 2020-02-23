using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BetterMixedSeeds.Patches
{
    [HarmonyPatch]
    internal class ObjectPatch
    {
        private static MethodBase TargetMethod()
        {
            return ModEntry.GetSDVType("Object").GetMethod("cutWeed", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Change the condition for the seed index from 473 to 1 (this was preventing Green Beans from being planted as it would decrement the number).
        /// </summary>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool changed = false;

            ModConfig modConfig = ModEntry.GetConfig();

            // Look in config and / 100 (Needs to be reduced from 0 - 100 (Percent) to 0 - 1 (Double))
            double mixedSeedDropChance = modConfig.MixedSeedDropChanceFromWeedsWhenNotFiber;
            mixedSeedDropChance = Math.Max(0, Math.Min(1, mixedSeedDropChance / 100));

            foreach (CodeInstruction instruction in instructions)
            {
                if (!changed && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 0.05)
                {
                    // Changes the mixed seed drop rate when cutting weeds to the config
                    changed = true;
                    instruction.operand = mixedSeedDropChance;
                }

                yield return instruction;
            }
        }
    }
}
