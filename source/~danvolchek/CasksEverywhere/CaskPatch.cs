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
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CasksEverywhere
{
    [HarmonyPatch]
    internal class CaskPatch
    {
        private static MethodBase TargetMethod()
        {
            return typeof(Cask).GetMethod(nameof(Cask.performObjectDropInAction));
        }

        /// <summary>Change the first Cellar operand to be a GameLocation instead - allowing casks everywhere.</summary>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool changed = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!changed && instruction.opcode == OpCodes.Isinst &&
                    (Type)instruction.operand == typeof(Cellar))
                {
                    changed = true;
                    instruction.operand = typeof(GameLocation);
                }

                yield return instruction;
            }
        }
    }
}
