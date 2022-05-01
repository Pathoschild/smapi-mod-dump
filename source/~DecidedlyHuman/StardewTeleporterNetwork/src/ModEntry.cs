/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewTeleporterNetwork.Utilities;
using StardewValley;
using SObject = StardewValley.Object;

namespace StardewTeleporterNetwork
{
	public class ModEntry : Mod
	{
		private IModHelper helper;
		private IMonitor monitor;
		private Logger logger;

		private TeleporterList teleporters;

		public override void Entry(IModHelper helper)
		{
			this.helper = helper;
			monitor = Monitor;
			logger = new Logger(monitor);
			teleporters = new TeleporterList(logger);

			HarmonyPatches.Patches.Initialise(logger, teleporters);
			Harmony harmony = new Harmony(ModManifest.UniqueID);
			
			// harmony.Patch(
			// 	original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
			// 	prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.SObject_CheckForAction_Prefix)));
			
			// This is where we check for interactions with our teleporter.
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.GameLocation_CheckAction_Prefix)));
			
			// This is where we detect a teleporter has been placed.
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.SObject_PlacementAction_Prefix)));
			
			this.helper.Events.GameLoop.OneSecondUpdateTicked += OneSecondUpdateTicked;
			this.helper.Events.World.ObjectListChanged += WorldOnObjectListChanged;
			this.helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			this.helper.Events.Input.ButtonsChanged += ButtonsChanged;
			
			logger.Log("Stardew Teleporter Network initialised.");
		}

		private void ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
		{
			if (e.Pressed.Contains(SButton.OemSemicolon))
			{
				foreach (Teleporter t in teleporters.Teleporters)
				{
					logger.Log($"Teleporter in {t.Location.Name} at {t.Tile} with key {t.Key}.");
				}
			}

            if (e.Pressed.Contains(SButton.OemTilde))
            {
                teleporters.CreateTeleporterPairings();
            }
		}

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			foreach (GameLocation l in Game1.locations)
			{
				teleporters.AddTeleportersInLocation(l);
			}

			foreach (Teleporter t in teleporters.Teleporters)
			{
				t.UpdateKey();
			}
			
			teleporters.CreateTeleporterPairings();

		logger.Log($"Found {teleporters.Teleporters.Count} teleporters.");
		}

		private void WorldOnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
		{
			// If there are no SObjects removed, we don't need to do anything here.
			if (!e.Removed.Any())
				return;
			
			foreach (var o in e.Removed)
			{
				if (o.Value.Name.Equals("STN Teleporter"))
				{
					logger.Log($"Object in {e.Location} at {o.Key} named STN Teleporter removed.");
					
					Teleporter teleporter = teleporters.FindTeleporter(e.Location, o.Key);

					logger.Log("Teleporters prior to removal: ");
					foreach (Teleporter t in teleporters.Teleporters)
					{
						logger.Log($"Tile: {t.Tile}; Key: {t.Key}");
					}

					if (teleporter != null)
						teleporters.Teleporters.Remove(teleporter);
					
					logger.Log("Teleporters after to removal: ");
					foreach (Teleporter t in teleporters.Teleporters)
					{
						logger.Log($"Tile: {t.Tile}; Key: {t.Key}");
					}
				}
			}
		}

		private void OneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
		{
			//_teleporters.UpdateAllTeleporters();
		}
	}
}