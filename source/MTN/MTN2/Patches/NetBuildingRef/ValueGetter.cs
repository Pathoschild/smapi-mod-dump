using Harmony;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.NetBuildingRefPatches
{
    public class ValueGetterPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            codes[6].opcode = OpCodes.Nop;
            codes[10].opcode = OpCodes.Call;
            codes[10].operand = AccessTools.Method(typeof(BuildableGameLocation), "getBuildingFromFarmsByName", new Type[] { typeof(string) });
            return codes.AsEnumerable();
        }
    }
}
