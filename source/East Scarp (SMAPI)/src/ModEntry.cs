/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarp
**
*************************************************/

using ExpandedPreconditionsUtility;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.IO;

namespace EastScarp
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		public ModData data { get; private set; }
		internal Harmony harmony { get; private set; }
		internal IConditionsChecker conditionsChecker { get; private set; }

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			data = Helper.Data.ReadJsonFile<ModData>
				(Path.Combine ("assets", "data.json")) ?? new ModData ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("es_reset_fruit_trees",
				"Resets fruit trees spawned for East Scarp.",
				cmdResetFruitTrees);

			// Handle game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Player.Warped += onWarped;
			Helper.Events.GameLoop.Saving += onSaving;
			Helper.Events.GameLoop.Saved += onSaved;
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.Display.MenuChanged += onMenuChanged;

			// Apply Harmony patches.
			harmony = new Harmony (ModManifest.UniqueID);
			FishingAreas.Patch ();
			Obelisks.Patch ();
			WinterGrasses.Patch ();
		}

		private void onMenuChanged (object sender, MenuChangedEventArgs e)
		{
			Obelisks.UpdateMenu (e.NewMenu);
		}

		private void onSaveLoaded (object sender, SaveLoadedEventArgs e)
		{
			Obelisks.RestoreAll ();
		}

		private void onSaved (object sender, SavedEventArgs e)
		{
			Obelisks.RestoreAll ();
		}

		private void onSaving (object sender, SavingEventArgs e)
		{
			Obelisks.SanitizeAll ();
		}

		private void onGameLaunched (object _sender, EventArgs _e)
		{
            // Make Expanded Preconditions Utility available for checking.
            conditionsChecker = Helper.ModRegistry.GetApi<IConditionsChecker>
				("Cherry.ExpandedPreconditionsUtility");
            conditionsChecker.Initialize (false, ModManifest.UniqueID);
		}

		private void onDayStarted (object _sender, EventArgs _e)
		{
			if (Context.IsMainPlayer)
			{
				CrabPotCatches.DayUpdate ();
				FruitTrees.DayUpdate ();
				Obelisks.RestoreAll ();
				RainWatering.DayUpdate ();
			}
		}

		private void onUpdateTicked (object _sender, EventArgs _e)
		{
			if (Context.IsWorldReady)
			{
				AmbientSounds.Play ();
				Critters.CheckSpawns (onEntry: false);
				SeaMonster.CheckSpawns ();
			}
		}

		private void onWarped (object _sender, EventArgs _e)
		{
			Critters.CheckSpawns (onEntry: true);
			WaterColors.Apply ();
			WaterEffects.Apply ();
			WinterGrasses.Apply ();
		}

		private void cmdResetFruitTrees (string _command, string[] _args)
		{
			try
			{
				FruitTrees.Reset ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"es_reset_fruit_trees failed: {e.Message}", LogLevel.Error);
			}
		}
	}
}
