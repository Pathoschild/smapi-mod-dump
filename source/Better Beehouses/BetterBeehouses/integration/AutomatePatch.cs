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
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace BetterBeehouses.integration
{
	class AutomatePatch
	{
		private static readonly Lazy<ILHelper> getStatePatch = new(statePatch);
		private static readonly Lazy<ILHelper> getOutputPatch = new(outputPatch);
		private static readonly Lazy<ILHelper> resetPatch = new(getResetPatch);
		private static bool isPatched = false;
		private static MethodInfo locationGetter;
		private static MethodInfo machineGetter;
		public static bool Setup()
		{

			if (!ModEntry.helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
				return false;

			ModEntry.monitor.Log("Automate Integration " + (ModEntry.config.PatchPFM ? "Enabling" : "Disabling"));

			var targetClass = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine");
			machineGetter ??= targetClass.PropertyGetter("Machine");
			locationGetter ??= targetClass.PropertyGetter("Location");

			if (!isPatched && ModEntry.config.PatchAutomate)
			{
				isPatched = false;
				ModEntry.harmony.Patch(targetClass.MethodNamed("GetState"), transpiler: new(typeof(AutomatePatch), "PatchState"));
				ModEntry.harmony.Patch(targetClass.MethodNamed("GetOutput"), transpiler: new(typeof(AutomatePatch), "PatchOutput"));
				ModEntry.harmony.Patch(targetClass.MethodNamed("Reset"), transpiler: new(typeof(AutomatePatch), "PatchReset"));
				isPatched = true;
			} else if (isPatched && !ModEntry.config.PatchAutomate)
			{
				ModEntry.harmony.Unpatch(targetClass.MethodNamed("GetState"), HarmonyPatchType.Transpiler, ModEntry.ModID);
				ModEntry.harmony.Unpatch(targetClass.MethodNamed("GetOutput"), HarmonyPatchType.Transpiler, ModEntry.ModID);
				ModEntry.harmony.Unpatch(targetClass.MethodNamed("Reset"), HarmonyPatchType.Transpiler, ModEntry.ModID);
				isPatched = false;
			}

			return true;
		}

		public static IEnumerable<CodeInstruction> PatchState(IEnumerable<CodeInstruction> instructions) => getStatePatch.Value.Run(instructions);
		public static IEnumerable<CodeInstruction> PatchOutput(IEnumerable<CodeInstruction> instructions) => getOutputPatch.Value.Run(instructions);
		public static IEnumerable<CodeInstruction> PatchReset(IEnumerable<CodeInstruction> instructions) => resetPatch.Value.Run(instructions);

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
				.Remove(new CodeInstruction[]
				{
					new(OpCodes.Ldc_I4_5)
				})
				.Add(new CodeInstruction[]
				{
					new(OpCodes.Call, typeof(ObjectPatch).MethodNamed(nameof(ObjectPatch.GetSearchRange)))
				})
				.SkipTo(new CodeInstruction[]
				{
					new(OpCodes.Dup),
					new(OpCodes.Ldloc_0),
					new(OpCodes.Callvirt,typeof(SObject).PropertyGetter(nameof(SObject.Price)))
				})
				.InsertBefore(new CodeInstruction[]
				{
					new(OpCodes.Ldloc_S, 4),
					new(OpCodes.Ldarg_0),
					new(OpCodes.Callvirt, AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine").MethodNamed("GetOwner")),
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, typeof(AutomatePatch).MethodNamed(nameof(ManipulateObject)))
				}, new CodeInstruction[]
				{
					new(OpCodes.Ldnull),
					new(OpCodes.Ldarg_0)
				})
				.Finish();
		}
		private static void ManipulateObject(SObject obj, Farmer owner, object machine)
		{
			var beehouse = machineGetter.Invoke(machine, null) as SObject;
			ObjectPatch.ManipulateObject(obj, owner, locationGetter.Invoke(machine, null) as GameLocation,
				beehouse?.TileLocation ?? default);
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
			=> ObjectPatch.CantProduceToday(loc.GetSeasonForLocation() == "winter", loc);

		private static Vector2 PointToVec(Point pt)
			=> new(pt.X, pt.Y);
	}
}
