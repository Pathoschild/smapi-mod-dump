/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace StardewVariableSeasons
{
    public static class NewDayAfterFadeTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ble_S &&
                    codes[i - 2].opcode == OpCodes.Ldsfld &&
                    codes[i - 2].operand.ToString()!.Contains("dayOfMonth"))
                {
                    codes[i].opcode = OpCodes.Jmp;
                }
            }

            return codes.AsEnumerable();
        }
    }
}