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
using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Debris"/> class.</summary>
    internal class DebrisPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpiler for the <see cref="Debris(int, int, Vector2, Vector2, float)"/> constructor.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to change the final debris type (in the base game passing an id of '68', for example, doesn't necessarily mean the actual debris chunks will have that same id. They can be offset by 1, this is to allow some debris to have varients, not all items have multiple varients though so this should be removed. This does have the side effect of removing the feature for all items, including ones with multiple varients, but it'll be quite unnoticable anyway).</remarks>
        internal static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                // check if the instruction is the beginning of the instruction 'group' to be patched
                var instruction = instructions.ElementAt(i);
                if (instruction.opcode != OpCodes.Ldarg_1)
                {
                    yield return instruction;
                    continue;
                }

                // get the next instruction (to check if it should be patched)
                var nextInstruction = instructions.ElementAt(i + 1);
                if (nextInstruction.opcode == OpCodes.Ldc_I4_2)
                {
                    // edit the instruction so debris can't have a different id to what was specified
                    nextInstruction.opcode = OpCodes.Ldc_I4_1;
                    yield return instruction;
                    yield return nextInstruction;

                    i++; // increment as the next instruction has been handled
                    continue;
                }

                yield return instruction;
            }
        }
    }
}
