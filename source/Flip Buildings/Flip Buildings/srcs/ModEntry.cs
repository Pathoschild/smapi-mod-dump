/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using FlipBuildings.Patches;
using FlipBuildings.Managers;
using FlipBuildings.Utilities;

namespace FlipBuildings
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

			// Load Mod assets
			AssetManager.Apply();

			// Load Harmony patches
			try
			{
				var harmony = new Harmony(ModManifest.UniqueID);

				// Apply menu patches
				IClickableMenuPatch.Apply(harmony);
				CarpenterMenuPatch.Apply(harmony);

				// Apply building patches
				BuildingPatch.Apply(harmony);
				CoopPatch.Apply(harmony);
				GreenhouseBuildingPatch.Apply(harmony);
				StablePatch.Apply(harmony);
				JunimoHutPatch.Apply(harmony);
				FishPondPatch.Apply(harmony);
				MillPatch.Apply(harmony);
				BarnPatch.Apply(harmony);

				// Apply location patches
				FarmPatch.Apply(harmony);
				FarmHousePatch.Apply(harmony);

				// Apply character patches
				NPCPatch.Apply(harmony);

				// Apply AlternativeTextures patches
				if (CompatibilityHelper.IsAlternativeTexturesLoaded)
				{
					Patches.AT.FarmPatch.Apply(harmony);
					Patches.AT.StablePatch.Apply(harmony);
				}

				// Apply SolidFoundations patches
				if (CompatibilityHelper.IsSolidFoundationsLoaded)
				{
					Patches.SF.GenericBuildingPatch.Apply(harmony);
				}
			}
			catch (Exception e)
			{
				Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
				return;
			}

			// Hook into the required events
			Helper.Events.GameLoop.SaveLoaded += Hooks.SaveLoaded.Apply;
			Helper.Events.GameLoop.DayStarted += Hooks.DayStarted.Apply;
			Helper.Events.GameLoop.DayEnding += Hooks.DayEnding.Apply;
			Helper.Events.Multiplayer.ModMessageReceived += Hooks.ModMessageReceived.Apply;
		}
	}
}
