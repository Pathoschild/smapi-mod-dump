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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class CropHarvestPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CropHarvestPatch()
		{
			Original = RequireMethod<Crop>(nameof(Crop.harvest));
			Transpiler = new(GetType(), nameof(CropHarvestTranspiler));
		}

		#region harmony patches

		/// <summary>
		///     Patch to nerf Ecologist spring onion quality and increment forage counter + always allow iridium-quality crops
		///     for Agriculturist + Harvester bonus crop yield.
		/// </summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			var mb = original.GetMethodBody() ??
			         throw new ArgumentNullException($"{original.Name} method body returned null.");

			/// From: @object.Quality = 4
			/// To: @object.Quality = GetEcologistForageQuality()

			try
			{
				helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // start of @object.Quality = 4
					)
					.ReplaceWith( // replace with custom quality
						new(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(
								nameof(Utility.Professions.GetEcologistForageQuality)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
			///		Data.IncrementField("ItemsForaged", amount: @object.Stack)
			///	After: Game1.stats.ItemsForaged += @object.Stack;

			var dontIncreaseEcologistCounter = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Stats).PropertySetter(nameof(Stats.ItemsForaged)))
					)
					.Advance()
					.InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Ecologist"),
						dontIncreaseEcologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "ItemsForaged"),
						new CodeInstruction(OpCodes.Ldloc_1), // loc 1 = @object
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Item).PropertyGetter(nameof(Item.Stack))),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).GetMethods().First(m => m.Name == "Increment")
								.MakeGenericMethod(typeof(int)))
					)
					.AddLabels(dontIncreaseEcologistCounter);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Ecologist counter increment.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			/// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
			/// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

			var fertilizerQualityLevel = mb.LocalVariables[8];
			var random2 = mb.LocalVariables[9];
			var isAgriculturist = iLGenerator.DefineLabel();
			try
			{
				helper.AdvanceUntil( // find index of Crop.fertilizerQualityLevel >= 3
						new CodeInstruction(OpCodes.Ldloc_S, fertilizerQualityLevel),
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Blt_S)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Agriculturist"), isAgriculturist,
						true)
					.AdvanceUntil( // find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, random2)
					)
					.AddLabels(isAgriculturist); // branch here if player is agriculturist
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: if (junimoHarvester is null && Game1.player.professions.Contains(<harvester_id>) && r.NextDouble() < 0.1) numToHarvest++

			var numToHarvest = mb.LocalVariables[6];
			var dontIncreaseNumToHarvest = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, numToHarvest) // find index of numToHarvest++
					)
					.ToBufferUntil( // copy this segment
						true,
						false,
						new CodeInstruction(OpCodes.Stloc_S, numToHarvest)
					)
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, random2) // find an instance of accessing the rng
					)
					.GetOperand(out var r2) // copy operand object
					.FindLast( // find end of chanceForExtraCrops while loop
						new CodeInstruction(OpCodes.Ldfld,
							typeof(Crop).Field(nameof(Crop.chanceForExtraCrops)))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // beginning of the next segment
					)
					.GetLabels(out var labels) // copy existing labels
					.SetLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
					.Insert( // insert check if junimoHarvester is null
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 4),
						new CodeInstruction(OpCodes.Brtrue_S, dontIncreaseNumToHarvest)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Harvester"),
						dontIncreaseNumToHarvest)
					.Insert( // insert dice roll
						new CodeInstruction(OpCodes.Ldloc_S, r2),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Random).MethodNamed(nameof(Random.NextDouble))),
						new CodeInstruction(OpCodes.Ldc_R8, 0.1),
						new CodeInstruction(OpCodes.Bge_Un_S, dontIncreaseNumToHarvest)
					)
					.InsertBuffer() // insert numToHarvest++
					.Return(3) // return to first inserted instruction
					.SetLabels(labels); // restore original labels to this segment
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding modded Harvester extra crop yield.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}