/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.IO;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		internal HarmonyInstance harmony { get; private set; }
		internal JsonAssets.IApi jsonAssets { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		internal int parentSheetIndex { get; private set; } = -1;

		public bool isScryingOrb (Item item)
		{
			if (item?.GetType () != typeof (SObject))
				return false;
			SObject obj = item as SObject;
			return obj.bigCraftable.Value &&
				obj.ParentSheetIndex == parentSheetIndex;
		}

		private readonly PerScreen<bool> orbHovered_ = new ();
		internal bool orbHovered
		{
			get => orbHovered_.Value;
			set
			{
				bool oldValue = orbHovered_.Value;
				orbHovered_.Value = value;

				// Let the cursor editor know to do its thing.
				if (cursorEditor != null && oldValue != value)
					cursorEditor.apply ();
			}
		}

		private readonly PerScreen<uint> orbsIlluminated_ = new ();
		internal uint orbsIlluminated
		{
			get => orbsIlluminated_.Value;
			set
			{
				uint oldValue = orbsIlluminated_.Value;
				orbsIlluminated_.Value = value;

				// Let the cursor editor know to do its thing.
				if (cursorEditor != null &&
						((oldValue == 0 && value > 0) ||
							(oldValue > 0 && value == 0)))
					cursorEditor.apply ();
			}
		}

		private CursorEditor cursorEditor;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, () => ModConfig.Instance);

			// Apply Harmony patches.
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony = HarmonyInstance.Create (ModManifest.UniqueID);
				SpriteTextPatches.Apply ();
			}

			// Set up asset editors.
			if (Constants.TargetPlatform != GamePlatform.Android)
			{
				cursorEditor = new CursorEditor ();
				Helper.Content.AssetEditors.Add (cursorEditor);
			}
			Helper.Content.AssetEditors.Add (new MailEditor ());

			// Add console commands.
			ConsoleCommands.Initialize ();

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.Input.CursorMoved += onCursorMoved;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Json Assets, if it is available.
			jsonAssets = Helper.ModRegistry.GetApi<JsonAssets.IApi>
				("spacechase0.JsonAssets");
			if (jsonAssets != null)
			{
				jsonAssets.IdsAssigned += onIdsAssigned;
			}
			else
			{
				Monitor.LogOnce ("Could not connect to Json Assets. It may not be installed or working properly.",
					(Config.ActivateKey != SButton.None) ? LogLevel.Warn : LogLevel.Error);
			}

			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();
		}

		private void onIdsAssigned (object _sender, EventArgs _e)
		{
			// Discover the object ID of the "Scrying Orb" inventory object.
			parentSheetIndex = jsonAssets.GetBigCraftableId ("Scrying Orb");
			if (parentSheetIndex == -1)
			{
				Monitor.Log ("Could not find the ID of the Scrying Orb big craftable. The Json Assets content pack may not have loaded correctly.",
					(Config.ActivateKey != SButton.None) ? LogLevel.Warn : LogLevel.Error);
			}
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			checkRecipe ();
		}

		internal void checkRecipe ()
		{
			// If the recipe is already given, nothing else to do.
			if (Game1.player.craftingRecipes.ContainsKey ("Scrying Orb"))
				return;

			// If the instant recipe cheat is active, add the recipe now.
			if (Config.InstantRecipe)
			{
				Game1.player.craftingRecipes.Add ("Scrying Orb", 0);
				return;
			}

			// Otherwise, if the friendship is adequate, deliver the letter.
			if (Game1.player.getFriendshipHeartLevelForNPC ("Wizard") >= 2 &&
					!Game1.player.mailbox.Contains ("kdau.ScryingOrb.welwickInstructions"))
				Game1.player.mailbox.Add ("kdau.ScryingOrb.welwickInstructions");
		}

		private void onCursorMoved (object _sender, CursorMovedEventArgs e)
		{
			// Only hovering if the world is ready and the player is free to
			// interact with an orb.
			if (!Context.IsWorldReady || !Context.IsPlayerFree)
			{
				orbHovered = false;
				return;
			}

			// Only hovering when a Scrying Orb is pointed at.
			orbHovered = getScryingOrbAt (Game1.currentLocation,
				e.NewPosition.Tile) != null;
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			// Only respond if the world is ready and the player is free to
			// interact with an orb.
			if (!Context.IsWorldReady || !Context.IsPlayerFree)
				return;

			// Accept the configured keybinding, if any.
			if (e.Button == Config.ActivateKey)
			{
				Experience.Run<UnlimitedExperience> (null);
				return;
			}

			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				// On Android, respond to any tap on the grab tile itself.
				if (e.Cursor.GrabTile != e.Cursor.Tile)
					return;
			}
			else
			{
				// On other platforms, only respond to the action button.
				if (!e.Button.IsActionButton ())
					return;
			}

			// Only respond when a Scrying Orb is interacted with.
			SObject orb = getScryingOrbAt (Game1.currentLocation,
				e.Cursor.GrabTile);
			if (orb == null)
				return;

			// Suppress the button so it won't cause any other effects.
			Helper.Input.Suppress (e.Button);

			// Try each of the experiences in turn.
			if (!Experience.Check<UnlimitedExperience> (orb) &&
				!Experience.Check<NothingExperience> (orb) &&
				!Experience.Check<LuckyPurpleExperience> (orb) &&
				!Experience.Check<MetaExperience> (orb) &&
				!Experience.Check<MiningExperience> (orb) &&
				!Experience.Check<GeodesExperience> (orb) &&
				!Experience.Check<EnchantmentsExperience> (orb) &&
				!Experience.Check<NightEventsExperience> (orb) &&
				!Experience.Check<GarbageExperience> (orb))
			{
				Experience.Run<FallbackExperience> (orb);
			}
		}

		private SObject getScryingOrbAt (GameLocation location,
			Vector2 tileLocation)
		{
			// Check for the relevant tiles on the Witchy Crystal Farm.
			if (Helper.ModRegistry.IsLoaded ("TerraBubbles.WC") &&
				Game1.whichFarm == 0 && location is Farm)
			{
				Point tilePoint = Utility.Vector2ToPoint (tileLocation);
				if (location.getTileSheetIDAt (tilePoint.X, tilePoint.Y, "Front")
						== "z_untitled tile sheet" &&
					location.getTileIndexAt (tilePoint.X, tilePoint.Y, "Front")
						== 244)
					return new SObject (tileLocation + new Vector2 (0f, 1f), parentSheetIndex);
				if (location.getTileSheetIDAt (tilePoint.X, tilePoint.Y, "Buildings")
						== "z_untitled tile sheet" &&
					location.getTileIndexAt (tilePoint.X, tilePoint.Y, "Buildings")
						== 269)
					return new SObject (tileLocation, parentSheetIndex);
			}

			// Check for a regular Scrying Orb object on the tile.
			SObject @object = location.getObjectAtTile
				((int) tileLocation.X, (int) tileLocation.Y);
			if (isScryingOrb (@object))
				return @object;

			return null;
		}
	}
}
