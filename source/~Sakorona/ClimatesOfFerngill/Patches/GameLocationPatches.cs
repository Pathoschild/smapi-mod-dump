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
#pragma warning disable IDE0060 // Remove unused parameter
        public static IEnumerable<CodeInstruction> DAAFLTranspiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060 // Remove unused parameter
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
