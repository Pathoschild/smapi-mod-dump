/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BetterBeehouses.integration
{
    class AutomatePatch
    {
        private static ILHelper getStatePatch;
        private static ILHelper getOutputPatch;
        private static ILHelper resetPatch;
        public static bool Setup()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
                return false;

            ModEntry.monitor.Log(ModEntry.helper.Translation.Get("general.automateWarning"), LogLevel.Warn);

            var targetClass = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine");
            getStatePatch = statePatch();
            getOutputPatch = outputPatch();
            resetPatch = getResetPatch();

            ModEntry.harmony.Patch(targetClass.MethodNamed("GetState"),transpiler: new(typeof(AutomatePatch),"PatchState"));
            ModEntry.harmony.Patch(targetClass.MethodNamed("GetOutput"), transpiler: new(typeof(AutomatePatch), "PatchOutput"));
            ModEntry.harmony.Patch(targetClass.MethodNamed("Reset"), transpiler: new(typeof(AutomatePatch), "PatchReset"));

            return true;
        }

        public static IEnumerable<CodeInstruction> PatchState(IEnumerable<CodeInstruction> instructions)
        {
            return getStatePatch.Run(instructions);
        }
        public static IEnumerable<CodeInstruction> PatchOutput(IEnumerable<CodeInstruction> instructions)
        {
            return getOutputPatch.Run(instructions);
        }
        public static IEnumerable<CodeInstruction> PatchReset(IEnumerable<CodeInstruction> instructions)
        {
            return resetPatch.Run(instructions);
        }

        private static ILHelper statePatch()
        {
            return new ILHelper("Automate:GetState")
                .Remove(new CodeInstruction[]
                {
                    new(OpCodes.Callvirt,typeof(GameLocation).MethodNamed("GetSeasonForLocation")),
                    new(OpCodes.Ldstr,"winter")
                })
                .Remove()
                .Add(new CodeInstruction[]
                {
                    new(OpCodes.Call,typeof(AutomatePatch).MethodNamed("CantWorkHere"))
                })
                .Finish();
        }
        private static ILHelper outputPatch()
        {
            return new ILHelper("Automate:GetOutput")
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Dup),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Callvirt,typeof(Object).MethodNamed("get_Price"))
                })
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Callvirt, typeof(Object).MethodNamed("set_Price"))
                })
                .Add(new CodeInstruction[]
                {
                    new(OpCodes.Dup),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call,AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine").MethodNamed("GetOwner")),
                    new(OpCodes.Call,typeof(ObjectPatch).MethodNamed(nameof(ObjectPatch.ManipulateObject)))
                })
                .Finish();
        }
        private static ILHelper getResetPatch()
        {
            return new ILHelper("Automate:Reset")
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldsfld,typeof(Game1).FieldNamed("timeOfDay"))
                })
                .Transform(new CodeInstruction[]{
                    new(OpCodes.Call,typeof(Utility).MethodNamed("CalculateMinutesUntilMorning",new[]{typeof(int),typeof(int)}))
                }, ObjectPatch.ChangeDays)
                .Finish();
        }
        public static bool CantWorkHere(GameLocation loc)
        {
            return ObjectPatch.CantProduceToday(loc.GetSeasonForLocation() == "winter", loc);
        }
    }
}
