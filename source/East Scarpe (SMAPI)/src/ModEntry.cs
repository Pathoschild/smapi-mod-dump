/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using ExpandedPreconditionsUtility;
using Harmony;
using StardewModdingAPI;
using System;
using System.IO;

namespace EastScarpe
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		public ModData data { get; private set; }
		internal HarmonyInstance harmony { get; private set; }
		internal IConditionsChecker conditionsChecker { get; private set; }

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			data = Helper.Data.ReadJsonFile<ModData>
				(Path.Combine ("assets", "data.json")) ?? new ModData ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("es_reset_fruit_trees",
				"Resets fruit trees spawned for East Scarpe.",
				cmdResetFruitTrees);

			// Handle game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Player.Warped += onWarped;

			// Apply Harmony patches.
			harmony = HarmonyInstance.Create (ModManifest.UniqueID);
			FishingAreas.Patch ();
			WinterGrasses.Patch ();
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
