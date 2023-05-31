/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Shockah.Kokoro;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Shockah.MaximizeFix
{
	public sealed class MaximizeFix : Mod
	{
		private static MaximizeFix Instance = null!;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			var harmony = new Harmony(ModManifest.UniqueID);

			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(Game1), nameof(Game1.SetWindowSize)),
				transpiler: new HarmonyMethod(typeof(MaximizeFix), nameof(Game1_SetWindowSize_Transpiler))
			);
		}

		private static IEnumerable<CodeInstruction> Game1_SetWindowSize_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Find(
						ILMatches.Ldsfld(AccessTools.Field(typeof(Game1), nameof(Game1.graphics))),
						ILMatches.Ldarg(1),
						ILMatches.Call("set_PreferredBackBufferWidth"),
						ILMatches.Ldsfld(AccessTools.Field(typeof(Game1), nameof(Game1.graphics))),
						ILMatches.Ldarg(2),
						ILMatches.Call("set_PreferredBackBufferHeight")
					)
					.Remove()
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}
	}
}