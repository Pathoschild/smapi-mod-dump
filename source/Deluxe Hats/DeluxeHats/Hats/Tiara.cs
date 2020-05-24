using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using Harmony;
using StardewValley;
using StardewValley.Events;

namespace DeluxeHats.Hats
{
    public static class Tiara
    {
        public const string Name = "Tiara";
        public const string Description = "When sleeping increase the chance for the fairy farm event.";
        public static void Activate()
        {
            HatService.Harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.pickFarmEvent)),
                postfix: new HarmonyMethod(typeof(Tiara), nameof(Tiara.PickFarmEvent_Postfix)));

            HatService.Harmony.Patch(
                original: AccessTools.Method(typeof(FairyEvent), nameof(FairyEvent.setUp)),
                transpiler: new HarmonyMethod(typeof(Tiara), nameof(Tiara.SetUp_Transpiler)));
        }

        public static void Disable()
        {
            HatService.Harmony.Unpatch(
                AccessTools.Method(typeof(Utility), nameof(Utility.pickFarmEvent)),
                HarmonyPatchType.Postfix,
                HatService.HarmonyId);

            HatService.Harmony.Unpatch(
                AccessTools.Method(typeof(FairyEvent), nameof(FairyEvent.setUp)),
                HarmonyPatchType.Transpiler,
                HatService.HarmonyId);
        }

        public static void PickFarmEvent_Postfix(ref FarmEvent __result)
        {
            try
            {
                if (__result == null)
                {
                    if (Game1.random.NextDouble() < 0.45)
                    { 
                        __result = new FairyEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                HatService.Monitor.Log($"Failed in {nameof(PickFarmEvent_Postfix)}:\n{ex}");
            }
        }

        public static IEnumerable<CodeInstruction> SetUp_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);
                var found = false;
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S)
                    {
                        if (codes[i].operand.ToString() == "100")
                        {
                            codes[i].opcode = OpCodes.Ldc_I4;
                            codes[i].operand = 300;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    throw new Exception("Could not find opcode Ldc_I4_S with operand of 100");
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
