using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ClimatesOfFerngillRebuild.Patches
{
    public static class GameLocationPatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_White"))
                {
                    codes[i].operand = AccessTools.Method(typeof(ClimatesOfFerngill), "GetSnowColor", new Type[] { });
                }
            }
            return codes.AsEnumerable();
        }
    }
}
