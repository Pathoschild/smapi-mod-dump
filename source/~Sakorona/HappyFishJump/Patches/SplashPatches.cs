using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using StardewValley.Buildings;

namespace HappyFishJump
{
    [HarmonyPatch(typeof(JumpingFish), nameof(JumpingFish.Splash))]
    class SplashPatches
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0060
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("playSound"))
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = AccessTools.Method( typeof( HappyFishJump ), nameof(HappyFishJump.PlaySound) );
                }
            }
            return codes.AsEnumerable();
        }
    }
}