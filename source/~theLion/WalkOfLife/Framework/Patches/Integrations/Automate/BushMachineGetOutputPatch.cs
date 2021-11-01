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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BushMachineGetOutputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BushMachineGetOutputPatch()
		{
			Original = AccessTools.Method(
				"Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine:GetOutput");
			Transpiler = new(GetType(), nameof(BushMachineGetOutputTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for automated Berry Bush quality.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> BushMachineGetOutputTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
			/// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologist : 0);

			try
			{
				Helper
					.FindProfessionCheck(Util.Professions.IndexOf("Ecologist")) // find index of ecologist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
					)
					.GetLabels(out var labels) // backup branch labels
					.ReplaceWith( // replace with custom quality
						new(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetEcologistForageQuality)))
					)
					.AddLabels(labels); // restore backed-up labels
			}
			catch (Exception ex)
			{
				Log($"Failed while patching automated Berry Bush quality.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}