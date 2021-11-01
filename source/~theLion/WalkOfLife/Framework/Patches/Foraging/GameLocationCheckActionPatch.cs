/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationCheckActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationCheckActionPatch()
		{
			Original = typeof(GameLocation).MethodNamed(nameof(GameLocation.checkAction));
			Transpiler = new(GetType(), nameof(GameLocationCheckActionTranspiler));
		}

		#region harmony patches

		/// <summary>
		///     Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective
		///     mod data fields.
		/// </summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationCheckActionTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: if (who.professions.Contains(<botanist_id>) && objects[key].isForage()) objects[key].Quality = 4
			/// To: if (who.professions.Contains(<ecologist_id>) && objects[key].isForage() && !IsForagedMineral(objects[key]) objects[key].Quality = GetEcologistForageQuality()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // start of objects[key].isForage() check
					)
					.ToBufferUntil( // copy objects[key]
						new CodeInstruction(OpCodes.Callvirt,
							typeof(OverlaidDictionary).PropertyGetter("Item"))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // end of check
					)
					.GetOperand(out var shouldntSetCustomQuality) // copy failed check branch destination
					.Advance()
					.InsertBuffer() // insert objects[key]
					.Insert( // check if is foraged mineral and branch if true
						new CodeInstruction(OpCodes.Call,
							typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.IsForagedMineral))),
						new CodeInstruction(OpCodes.Brtrue_S, (Label) shouldntSetCustomQuality)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // start of objects[key].Quality = 4
					)
					.ReplaceWith( // replace with custom quality
						new(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetEcologistForageQuality)))
					);
			}
			catch (Exception ex)
			{
				Log($"Failed while patching modded Ecologist forage quality.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			/// Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) objects[key].Quality = GetMineralQualityForGemologist()

			var gemologistCheck = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // return to botanist check
					.Retreat() // retreat to start of check
					.ToBufferUntil( // copy entire section until done setting quality
						true,
						false,
						new CodeInstruction(OpCodes.Br_S)
					)
					.AdvanceUntil( // change previous section branch destinations to injected section
						new CodeInstruction(OpCodes.Brfalse_S)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brtrue_S)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Br_S)
					)
					.Advance()
					.InsertBuffer() // insert copy
					.Return()
					.AddLabels(gemologistCheck) // add destination label for branches from previous section
					.AdvanceUntil( // find repeated botanist check
						new CodeInstruction(OpCodes.Ldc_I4_S, 16)
					)
					.SetOperand(Util.Professions.IndexOf("Gemologist")) // replace with gemologist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S)
					)
					.GetOperand(out var shouldntSetCustomQuality) // copy next section branch destination
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // start of call to isForage()
					)
					.RemoveUntil( // right before call to IsForagedMineral()
						new CodeInstruction(OpCodes.Callvirt,
							typeof(OverlaidDictionary).PropertyGetter("Item"))
					)
					.Advance()
					.ReplaceWith( // remove 'not' and set correct branch destination
						new(OpCodes.Brfalse_S, (Label) shouldntSetCustomQuality)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetEcologistForageQuality)))
					)
					.SetOperand(
						typeof(Util.Professions).MethodNamed(nameof(Util.Professions
							.GetGemologistMineralQuality))); // set correct custom quality method call
			}
			catch (Exception ex)
			{
				Log($"Failed while adding modded Gemologist foraged mineral quality.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			/// Injected: IncrementModData(objects[key], this, who)

			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Stats).PropertySetter(nameof(Stats.ItemsForaged)))
					)
					.Advance()
					.InsertBuffer(5, 4) // SObject objects[key]
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldarg_3)
					)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(GameLocationCheckActionPatch).MethodNamed(nameof(CheckActionSubroutine)))
					);
			}
			catch (Exception ex)
			{
				Log($"Failed while adding Ecologist and Gemologist counter increment.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void CheckActionSubroutine(SObject obj, GameLocation location, Farmer who)
		{
			if (who.HasProfession("Ecologist") && obj.isForage(location) && !obj.IsForagedMineral())
				ModEntry.Data.IncrementField<uint>("ItemsForaged");
			else if (who.HasProfession("Gemologist") && obj.IsForagedMineral())
				ModEntry.Data.IncrementField<uint>("MineralsCollected");
		}

		#endregion private methods
	}
}