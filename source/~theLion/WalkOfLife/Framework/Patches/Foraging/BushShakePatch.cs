/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BushShakePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BushShakePatch()
		{
			Original = typeof(Bush).MethodNamed(name: "shake");
			Transpiler = new HarmonyMethod(GetType(), nameof(BushShakeTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> BushShakeTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: Game1.player.professions.Contains(16) ? 4 : 0
			/// To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)
					)
					.GetLabels(out var labels)
					.ReplaceWith( // replace with custom quality
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetEcologistForageQuality)))
					)
					.SetLabels(labels);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}");
				return null;
			}

			/// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
			///		Data.IncrementField<uint>("ItemsForaged")

			var dontIncreaseEcologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.InsertProfessionCheckForLocalPlayer(Util.Professions.IndexOf("Ecologist"), dontIncreaseEcologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "ItemsForaged"),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).MethodNamed(nameof(ModData.IncrementField), new[] { typeof(string) }).MakeGenericMethod(typeof(uint)))
					)
					.AddLabels(dontIncreaseEcologistCounter);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}