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
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class CrabPotMachineGetStatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CrabPotMachineGetStatePatch()
		{
			try
			{
				Original = "CrabPotMachine".ToType().MethodNamed("GetState");
			}
			catch
			{
				// ignored
			}

			Transpiler = new(GetType(), nameof(CrabPotMachineGetStateTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> CrabPotMachineGetStateTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Removed: || !this.PlayerNeedsBait()

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Brtrue_S)
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Call, "CrabPotMachine".ToType().MethodNamed("PlayerNeedsBait"))
					)
					.SetOpCode(OpCodes.Brfalse_S);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching bait conditions for automated Crab Pots.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}