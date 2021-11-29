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
	internal class ProfessionsCheatSetProfessionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProfessionsCheatSetProfessionPatch()
		{
			try
			{
				Original = "ProfessionsCheat".ToType().MethodNamed("SetProfession");
			}
			catch
			{
				// ignored
			}

			Transpiler = new(GetType(), nameof(ProfessionsCheatSetProfessionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ProfessionsCheatSetProfessionTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: case <defender_id>
			/// To: case <brute_id>

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
					)
					.SetOperand(Utility.Professions.IndexOf("Brute"));
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while moving CJB Profession Cheat health bonus from Defender to Brute.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}