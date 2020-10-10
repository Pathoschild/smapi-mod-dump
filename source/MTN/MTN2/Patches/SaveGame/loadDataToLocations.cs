/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.SaveGamePatches
{
    public class loadDataToLocationsPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            codes[90].opcode = OpCodes.Nop;
            codes[91].opcode = OpCodes.Nop;
            codes[92].opcode = OpCodes.Isinst;
            codes[92].operand = typeof(StardewValley.Farm);
            return codes.AsEnumerable();
        }
    }
}
