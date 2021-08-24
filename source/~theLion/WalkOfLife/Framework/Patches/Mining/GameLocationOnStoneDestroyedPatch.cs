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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationOnStoneDestroyedPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationOnStoneDestroyedPatch()
		{
			Original = typeof(GameLocation).MethodNamed(nameof(GameLocation.OnStoneDestroyed));
			Transpiler = new HarmonyMethod(GetType(), nameof(GameLocationOnStoneDestroyedTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remove Prospector double coal chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationOnStoneDestroyedTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: random.NextDouble() < 0.035 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2)
			/// To: random.NextDouble() < 0.035

			try
			{
				Helper
					.FindProfessionCheck(Farmer.burrower) // find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul) // remove this check
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}