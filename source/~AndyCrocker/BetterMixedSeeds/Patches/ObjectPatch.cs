/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BetterMixedSeeds.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="StardewValley.Object"/> class.</summary>
    internal static class ObjectPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpiler for the <see cref="StardewValley.Object.cutWeed(Farmer, GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>Used to change the drop chance of mixed seeds from weeds.</remarks>
        internal static IEnumerable<CodeInstruction> CutWeedTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var patchApplied = false;

            foreach (var instruction in instructions)
            {
                // check if this instruction is the one responsible for the mixed seeds drop chance
                if (!patchApplied && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 0.05)
                {
                    patchApplied = true;

                    // change the mixed seeds drop chance
                    instruction.operand = Math.Max(Math.Min(1, ModEntry.Instance.Config.PercentDropChanceForMixedSeedsWhenNotFiber / 100d), 0);
                    yield return instruction;
                    continue;
                }

                yield return instruction;
            }
        }
    }
}
