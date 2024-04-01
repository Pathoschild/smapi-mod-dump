/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using RelocateFarmAnimals.Patches;

namespace RelocateFarmAnimals
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		// Shared static helpers
		internal static new IModHelper	Helper { get; private set; }
		internal static new IMonitor	Monitor { get; private set; }
		internal static new IManifest	ModManifest { get; private set; }

		public override void Entry(IModHelper helper)
		{
			Helper = base.Helper;
			Monitor = base.Monitor;
			ModManifest = base.ModManifest;

			// Load Harmony patches
			try
			{
				var harmony = new Harmony(ModManifest.UniqueID);

				// Apply menu patches
				AnimalQueryMenuPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
