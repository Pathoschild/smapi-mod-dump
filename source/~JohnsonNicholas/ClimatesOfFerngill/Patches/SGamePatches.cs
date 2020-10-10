/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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
#pragma warning disable IDE0060 // Remove unused parameter
        public static IEnumerable<CodeInstruction> DrawImplTranspiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {

                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("isRaining") && codes[i+1].opcode == OpCodes.Brtrue_S)
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = AccessTools.Method(typeof(ClimatesOfFerngill), "ShouldDarken", new Type[] { });
                } 
            }

            return codes.AsEnumerable();
        }
    }
}
