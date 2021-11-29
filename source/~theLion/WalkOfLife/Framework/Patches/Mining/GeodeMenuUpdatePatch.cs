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
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GeodeMenuUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GeodeMenuUpdatePatch()
		{
			Original = RequireMethod<GeodeMenu>(nameof(GeodeMenu.update));
			Transpiler = new(GetType(), nameof(GeodeMenuUpdateTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for geodes cracked at Clint's.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GeodeMenuUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
			///		Data.IncrementField<uint>("MineralsCollected")
			///	After: Game1.stats.GeodesCracked++;

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Stats).PropertySetter(nameof(Stats.GeodesCracked)))
					)
					.Advance()
					.InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Gemologist"),
						dontIncreaseGemologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "MineralsCollected"),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).MethodNamed(nameof(ModData.Increment), new[] {typeof(string)})
								.MakeGenericMethod(typeof(uint)))
					)
					.AddLabels(dontIncreaseGemologistCounter);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Gemologist counter increment.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}