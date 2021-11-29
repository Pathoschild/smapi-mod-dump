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
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationBreakStonePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationBreakStonePatch()
		{
			Original = RequireMethod<GameLocation>("breakStone");
			Transpiler = new(GetType(), nameof(GameLocationBreakStoneTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Skipped: if (who.professions.Contains(<geologist_id> && r.NextDouble() < 0.5) switch(indexOfStone) ...

			try
			{
				helper
					.FindProfessionCheck(Farmer.geologist) // find index of geologist check
					.Retreat()
					.StripLabels(out var labels) // backup and remove branch labels
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the false case branch
					)
					.GetOperand(out var isNotGeologist) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br, (Label) isNotGeologist)
					)
					.Retreat()
					.AddLabels(labels); // restore backed-up labels to inserted branch
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing vanilla Geologist paired gems.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Skipped: if (who.professions.Contains(<prospector_id>)) ...

			try
			{
				helper
					.FindProfessionCheck(Farmer.burrower) // find index of prospector check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
					)
					.GetOperand(out var isNotProspector) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label) isNotProspector)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}