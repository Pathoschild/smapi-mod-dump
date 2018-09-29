using Harmony;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.Game1Patch {
    class rain {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            var codes = new List<CodeInstruction>(instructions);

            //Replace Color.White with a MTN specific function: Utility.randomColor()
            codes[2027].opcode = OpCodes.Call;
            codes[2027].operand = AccessTools.Method(typeof(Utilities), "randomColor", new Type[] { });
            codes[2462].opcode = OpCodes.Call;
            codes[2462].operand = AccessTools.Method(typeof(Utilities), "randomColor", new Type[] { });

            //for (int i = 2020; i < 2465; i++) {
            //    Memory.instance.Monitor.Log($"Line {i}: {codes[i].opcode} - {codes[i].operand}", (codes[i].operand != null && codes[i].operand.ToString().Contains("Rain") ? StardewModdingAPI.LogLevel.Error : StardewModdingAPI.LogLevel.Debug));
            //}

            return codes.AsEnumerable();
        }
    }
}
