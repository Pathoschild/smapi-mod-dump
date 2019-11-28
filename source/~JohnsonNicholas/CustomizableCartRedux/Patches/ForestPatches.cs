using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomizableTravelingCart.Patches
{
    public static class ForestPatches
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static IEnumerable<CodeInstruction> CheckActionTranspiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("timeOfDay"))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomizableCartRedux), "IsValidHours", new Type[] { }));
                    codes[i + 1].opcode = OpCodes.Nop;
                    codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Brfalse;
                }
            }
            return codes.AsEnumerable();
        }

        public static IEnumerable<CodeInstruction> DrawTranspiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("timeOfDay"))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomizableCartRedux), "IsValidHours", new Type[] { }));
                    codes[i + 1].opcode = OpCodes.Nop;
                    codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Brtrue;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
