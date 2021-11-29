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
using StardewValley.Tools;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class FishingRodStartMinigameEndFunctionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FishingRodStartMinigameEndFunctionPatch()
		{
			Original = RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
			Transpiler = new(GetType(), nameof(FishingRodStartMinigameEndFunctionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remove Pirate bonus treasure chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FishingRodStartMinigameEndFunctionTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

			try
			{
				helper // find index of pirate check
					.FindProfessionCheck(Farmer.pirate)
					.Retreat(2)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Add) // remove this check
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}