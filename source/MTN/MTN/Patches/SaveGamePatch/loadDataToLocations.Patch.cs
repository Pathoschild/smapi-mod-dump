using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.SaveGamePatch
{
    class loadDataToLocationsPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes[90].opcode = OpCodes.Nop;
            codes[91].opcode = OpCodes.Nop;
            codes[92].opcode = OpCodes.Isinst;
            codes[92].operand = typeof(StardewValley.Farm);

            //388 = Nop
            //389 = Nop
            //390 = Isinst
            //390.Oprand = typeof(stardewvalley.Farm);
            //codes[388].opcode = OpCodes.Nop;
            //codes[389].opcode = OpCodes.Nop;
            //codes[390].opcode = OpCodes.Isinst;
            //codes[390].operand = typeof(StardewValley.Farm);
            return codes.AsEnumerable();
        }
    }
}
