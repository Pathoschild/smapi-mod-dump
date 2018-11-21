using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ClimatesOfFerngillRebuild.Patches
{
    public static class SGamePatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            bool OpNotFound = true, OpNotFoundB = true;

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldelema && codes[i].operand.ToString().Contains("RainDrop"))
                {
                    var startIndex = i + 1;

                    for (int j = startIndex; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Call && OpNotFound)
                        {
                            if (codes[j].operand.ToString().Contains("get_White()"))
                            {
                                codes[j].operand = AccessTools.Method(typeof(ClimatesOfFerngill), "GetRainColor", new Type[] { });
                                OpNotFound = false;
                            }
                        }
                    }
                }

                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_Blue") && OpNotFoundB)
                {
                    codes[i].operand = AccessTools.Method(typeof(ClimatesOfFerngill), "GetRainBackColor", new Type[] { });
                    OpNotFoundB = false;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
