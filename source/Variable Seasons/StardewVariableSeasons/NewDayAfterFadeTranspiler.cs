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
using StardewModdingAPI;
using StardewValley;

namespace StardewVariableSeasons
{
    [HarmonyPatch(typeof(Game1), "_newDayAfterFade")]
    public static class NewDayAfterFadeTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            
            foreach (var code in codes.Where(code =>
                         code.opcode == OpCodes.Call &&
                         code.operand.ToString().Contains("StardewValley.Game1::newSeason()")))
            {
                code.opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }
    }
}