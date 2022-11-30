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

namespace BattleRoyale
{
    class BombFixer : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(TemporaryAnimatedSprite), "update");

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var a = instr.ToArray();

            for (int i = 5; i <= 13; i++)
            {
                a[i].opcode = OpCodes.Nop;
            }

            return a;
        }
    }
}
