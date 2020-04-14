using Harmony;
using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		internal HarmonyInstance harmony { get; private set; }

		protected ModConfig Config => ModConfig.Instance;

		internal int parentSheetIndex = -1;
		public bool IsScryingOrb (Item item)
		{
			if (item?.GetType () != typeof (SObject))
				return false;
			SObject obj = item as SObject;
			return obj.bigCraftable.Value &&
				obj.ParentSheetIndex == parentSheetIndex;
		}

		private bool orbHovered;
		internal bool OrbHovered
		{
			get => orbHovered;
			set
			{
				bool oldValue = orbHovered;
				orbHovered = value;

				// Let the cursor editor know to do its thing.
				if (cursorEditor != null && oldValue != value)
					cursorEditor.apply ();
			}
		}

		private uint orbsIlluminated;
		internal uint OrbsIlluminated
		{
			get => orbsIlluminated;
			set
			{
				uint oldValue = orbsIlluminated;
				orbsIlluminated = value;

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
			Helper.ConsoleCommands.Add ("reset_scrying_orbs",
				"Resets the state of Scrying Orbs to default values.",
				cmdResetScryingOrbs);
			Helper.ConsoleCommands.Add ("test_scrying_orb",
				"Puts a Scrying Orb and all types of offering into inventory.",
				cmdTestScryingOrb);
			Helper.ConsoleCommands.Add ("test_date_picker",
				"Runs a DatePicker dialog for testing use.",
				cmdTestDatePicker);

			// Listen for game events.
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.Input.CursorMoved += onCursorMoved;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
		}

		private void onSaveLoaded (object _sender, SaveLoadedEventArgs _e)
		{
			// Discover the object ID of the "Scrying Orb" inventory object.
			var JsonAssets = Helper.ModRegistry.GetApi<JsonAssets.IApi>
				("spacechase0.JsonAssets");
			if (JsonAssets != null)
			{
				parentSheetIndex = JsonAssets.GetBigCraftableId ("Scrying Orb");
				if (parentSheetIndex == -1)
				{
					Monitor.Log ("Could not find the ID of the Scrying Orb big craftable. The Scrying Orb content pack for Json Assets may not be installed.",
						LogLevel.Error);
				}
			}
			else
			{
				Monitor.LogOnce ("Could not connect to Json Assets. It may not be installed or working properly.",
					LogLevel.Error);
			}

			// Migrate data from older formats.
			UnlimitedExperience.MigrateData ();
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			// If the recipe is already given, nothing else to do.
			if (Game1.player.craftingRecipes.ContainsKey ("Scrying Orb"))
				return;

			// If the instant recipe cheat is enabled, add the recipe now.
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
				OrbHovered = false;
				return;
			}

			// Only hovering when a Scrying Orb is pointed at.
			SObject obj = Game1.currentLocation.getObjectAtTile
				((int) e.NewPosition.Tile.X, (int) e.NewPosition.Tile.Y);
			OrbHovered = IsScryingOrb (obj);
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			// Only respond if the world is ready and the player is free to
			// interact with an orb.
			if (!Context.IsWorldReady || !Context.IsPlayerFree)
				return;

			// Only respond when a Scrying Orb is interacted with.
			SObject orb = Game1.currentLocation.getObjectAtTile
				((int) e.Cursor.GrabTile.X, (int) e.Cursor.GrabTile.Y);
			if (!IsScryingOrb (orb))
				return;

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

			// Suppress the button so it won't cause any other effects.
			Helper.Input.Suppress (e.Button);

			// If the unlimited use cheat is enabled, skip to the menu.
			if (Config.UnlimitedUse)
			{
				Experience.Run<UnlimitedExperience> (orb);
				return;
			}

			if (Experience.Check<UnlimitedExperience> (orb) ||
				Experience.Check<NothingExperience> (orb) ||
				Experience.Check<LuckyPurpleExperience> (orb) ||
				Experience.Check<MetaExperience> (orb) ||
				Experience.Check<MiningExperience> (orb) ||
				Experience.Check<GeodesExperience> (orb) ||
				Experience.Check<NightEventsExperience> (orb) ||
				// TODO: Experience.Try<ShoppingExperience> (orb) ||
				Experience.Check<GarbageExperience> (orb) ||
				// TODO: Experience.Try<ItemFinderExperience> (orb) ||
				Experience.Check<FallbackExperience> (orb))
			{} // (if-block used to allow boolean fallback)
		}

		private void cmdResetScryingOrbs (string _command, string[] _args)
		{
			try
			{
				Utilities.CheckWorldReady ();
				UnlimitedExperience.Reset ();
				LuckyPurpleExperience.Reset ();
				MetaExperience.Reset ();
				Monitor.Log ("Scrying Orb state reset to defaults.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not reset Scrying Orbs: {e.Message}", LogLevel.Error);
			}
		}

		private void cmdTestScryingOrb (string _command, string[] _args)
		{
			try
			{
				IDictionary<int, string> bci = Game1.bigCraftablesInformation;
				int orbID = bci.First ((kp) => kp.Value.StartsWith ("Scrying Orb/",
					StringComparison.Ordinal)).Key;

				Game1.player.addItemsByMenuIfNecessary (new List<Item>
				{
					new SObject (74, 50), // Prismatic Shard for UnlimitedExperience
					new SObject (789, 1), // Lucky Purple Shorts for LuckyPurpleExperience
					// TODO: item for ItemFinderExperience
					new SObject (168, 150), // 3 Trash for GarbageExperience
					// TODO: item for ShoppingExperience
					new SObject (767, 150), // 3 Bat Wing for NightEventsExperience
					new SObject (541, 50), // Aerinite for GeodesExperience
					new SObject (382, 100), // 2 Coal for MiningExperience
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID) // Scrying Orb
				});

				Monitor.Log ("Scrying Orb test kit placed in inventory.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not create test kit: {e.Message}", LogLevel.Error);
			}
		}

		private void cmdTestDatePicker (string _command, string[] _args)
		{
			try
			{
				WorldDate initialDate = new WorldDate (2, "spring", 15);
				string prompt = "Where on the wheel of the year do you seek?";
				if (Context.IsWorldReady)
					++OrbsIlluminated; // use the special cursor in the dialog
				Game1.activeClickableMenu = new DatePicker (initialDate, prompt,
					(date) =>
					{
						if (Context.IsWorldReady)
							--OrbsIlluminated;
						Monitor.Log ($"DatePicker chose {date}", LogLevel.Info);
					});
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not test date picker: {e.Message}", LogLevel.Error);
			}
		}
	}
}
