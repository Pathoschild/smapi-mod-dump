/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ExtendedReach
**
*************************************************/

using Harmony;
using SpaceShared;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtendedReach
{
    public class TileRadiusFix
    {
        public static IEnumerable<CodeInstruction> IncreaseRadiusChecks(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            // TODO: Learn how to use ILGenerator

            var newInsns = new List<CodeInstruction>();
            foreach (var insn in insns)
            {
                if (insn.opcode == OpCodes.Call && insn.operand is MethodInfo meth)
                {
                    if (meth.Name == "withinRadiusOfPlayer" || meth.Name == "tileWithinRadiusOfPlayer")
                    {
                        var newInsn = new CodeInstruction(OpCodes.Ldc_I4, 100);
                        Log.trace($"Found {meth.Name}, replacing {newInsns[newInsns.Count - 2]} with {newInsn}");
                        newInsns[newInsns.Count - 2] = newInsn;
                    }
                }
                newInsns.Add(insn);
            }

            return newInsns;
        }
    }
}
