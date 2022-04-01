/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterLightningRods
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        public static IEnumerable<CodeInstruction> Utility_performLightningUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Utility.performLightningUpdate");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            bool rodsFound = false;
            bool derandomFound = false;
            bool shuffleFound = false;
            bool chanceFound = false;
            CodeInstruction indexCode = null;
            for (int i = 0; i < codes.Count; i++)
            {
                if (!rodsFound && i < codes.Count - 2 && codes[i + 2].opcode == OpCodes.Blt && codes[i + 1].opcode == OpCodes.Ldc_I4_2 && codes[i].opcode == OpCodes.Ldloc_S)
                {
                    SMonitor.Log($"Overriding number of lightning rods to check");
                    codes[i + 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetRodsToCheck)));
                    rodsFound = true;
                }
                else if (!chanceFound && codes[i].opcode == OpCodes.Ldc_R8 && (double)codes[i].operand == 0.125)
                {
                    SMonitor.Log($"Overriding lightning chance");
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetLightningChance));
                    chanceFound = true;
                }
                else if (!shuffleFound && Config.UniqueCheck && i < codes.Count - 2 && codes[i + 2].opcode == OpCodes.Ble && codes[i + 1].opcode == OpCodes.Ldc_I4_0 && codes[i].opcode == OpCodes.Callvirt && codes[i - 1].opcode == OpCodes.Ldloc_3)
                {
                    SMonitor.Log($"Shuffling lightning rod list");
                    indexCode = new CodeInstruction(OpCodes.Ldloc_S, codes[i + 4].operand);
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.ShuffleRodList))));
                    shuffleFound = true;
                }
                else if (!derandomFound && Config.UniqueCheck && indexCode != null && i < codes.Count - 4 && codes[i + 4].opcode == OpCodes.Callvirt && (MethodInfo)codes[i + 4].operand == AccessTools.Method(typeof(Random), nameof(Random.Next), new Type[] { typeof(int) }) && codes[i + 2].opcode == OpCodes.Ldloc_3 && codes[i + 1].opcode == OpCodes.Ldloc_0 && codes[i].opcode == OpCodes.Ldloc_3)
                {
                    SMonitor.Log($"Setting check to indexed rod");
                    newCodes.Add(codes[i]);
                    newCodes.Add(codes[i]);
                    newCodes.Add(indexCode);
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetLightningRod))));
                    i += 5;
                    derandomFound = true;
                }
                newCodes.Add(codes[i]);
            }

            return newCodes.AsEnumerable();
        }

        private static double GetLightningChance()
        {
            return Config.EnableMod ? (double)(Config.LightningChance / 100f) : 0.125;
        }

        private static int GetRodsToCheck()
        {
            return Config.EnableMod ? Config.RodsToCheck : 2;
        }
    }
}