/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace BirbShared
{
    public static class TranspilerExtensions
    {
        public static int FindBCloseToA(this IEnumerable<CodeInstruction> instructions, CodeInstruction A, CodeInstruction B, int maxDistance = 10)
        {
            CodeInstruction[] instrs = instructions.ToArray();

            int distanceRemaining = 0;
            int result = -1;
            for (int i = 0; i < instrs.Length; i++)
            {
                distanceRemaining--;
                if (instrs[i].Is(A.opcode, A.operand))
                {
                    distanceRemaining = maxDistance;
                    continue;
                }

                if (distanceRemaining > 0 && instrs[i].Is(B.opcode, B.operand))
                {
                    if (result >= 0)
                    {
                        Log.Error($"FindBCloseToA found multiple matches within distance {maxDistance}: {A}, {B}");
                        return -1;
                    }
                    result = i;
                }
            }
            if (result < 0)
            {
                Log.Error($"FindBCloseToA found no matches within distance {maxDistance}: {A}, {B}");
            }
            return result;
        }

        public static IEnumerable<CodeInstruction> InsertAfterIndex(this IEnumerable<CodeInstruction> instructions, CodeInstruction[] toInsert, int index)
        {
            CodeInstruction[] instrs = instructions.ToArray();
            for (int i = 0; i < instrs.Length; i++)
            {
                yield return instrs[i];
                if (i == index)
                {
                    for (int j = 0; j < toInsert.Length; j++)
                    {
                        yield return toInsert[j];
                    }
                }
            }
        }
    }
}
