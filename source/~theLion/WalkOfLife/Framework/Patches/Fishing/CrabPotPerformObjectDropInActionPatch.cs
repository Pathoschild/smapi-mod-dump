/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class CrabPotPerformObjectDropInActionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.performObjectDropInAction)),
				transpiler: new HarmonyMethod(GetType(), nameof(CrabPotPerformObjectDropInActionTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to allow Conservationist to place bait.</summary>
		private static IEnumerable<CodeInstruction> CrabPotPerformObjectDropInActionTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(CrabPot)}::{nameof(CrabPot.performObjectDropInAction)}");

			/// Removed: ... && (owner_farmer == null || !owner_farmer.professions.Contains(11)

			try
			{
				Helper
					.FindProfessionCheck(Utility.ProfessionMap.Forward["Conservationist"])
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Brtrue)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing Conservationist bait restriction.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}