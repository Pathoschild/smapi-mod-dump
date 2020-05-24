using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DeluxeHats.Hats
{
    public static class PartyHat
    {
        public const string Name = "Party Hat";
        public const string Description = "Gifts given on a birthdays give even more friendship.";
        public static void Activate()
        {
            HatService.Harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                transpiler: new HarmonyMethod(typeof(PartyHat), nameof(PartyHat.SetUp_Transpiler)));
        }

        public static void Disable()
        {
            HatService.Harmony.Unpatch(
                AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                HarmonyPatchType.Transpiler,
                HatService.HarmonyId);
        }

        public static IEnumerable<CodeInstruction> SetUp_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);
                var found = false;
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4)
                    {
                        if (codes[i].operand.ToString() == "8")
                        {
                            codes[i].opcode = OpCodes.Ldc_I4;
                            codes[i].operand = 10f;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    throw new Exception("Could not find opcode Ldc_R4 with operand of 8");
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                HatService.Monitor.Log($"Failed in {nameof(SetUp_Transpiler)}:\n{ex}");
                return instructions;
            }
        }
    }
}
