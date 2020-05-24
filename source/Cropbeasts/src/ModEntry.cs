using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cropbeasts
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		internal JsonAssets.IApi jsonAssets { get; private set; }
		internal object jsonAssetsMod { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		public Chooser chooser { get; private set; }

		private List<FarmMonster> currentMonsters;
		private IEnumerable<Cropbeast> currentBeasts =>
			currentMonsters?.OfType<Cropbeast> ();
		public int currentBeastCount => currentBeasts?.Count () ?? 0;

		public void registerMonster (FarmMonster monster)
		{
			if (!currentMonsters.Contains (monster))
				currentMonsters.Add (monster);
		}

		public override void Entry (IModHelper helper)
		{
			// Check for unsupported platforms.
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				Monitor.Log ("This mod does not yet support Android.",
					LogLevel.Error);
				return;
			}

			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Add console commands.
			ConsoleCommands.Initialize ();

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.SaveCreated += (_s, _e) => prepareSave ();
			Helper.Events.GameLoop.SaveLoaded += (_s, _e) => prepareSave ();
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.GameLoop.DayEnding += onDayEnding;
			Helper.Events.GameLoop.TimeChanged += onTimeChanged;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Display.RenderedWorld += onRenderedWorld;
			Helper.Events.Display.RenderedHud += onRenderedHUD;
			Helper.Events.Display.MenuChanged += onMenuChanged;
			Helper.Events.Multiplayer.ModMessageReceived += onModMessageReceived;

			// Set up asset loaders/editors.
			Helper.Content.AssetLoaders.Add (new Assets.CropbeastLoader ());
			Helper.Content.AssetLoaders.Add (new Assets.SpriteLoader ());
			Helper.Content.AssetEditors.Add (new Assets.AchievementEditor ());
			Helper.Content.AssetEditors.Add (new Assets.BuffsEditor ());
			Helper.Content.AssetEditors.Add (new Assets.MonsterEditor ());
		}

		internal bool spawnCropbeast (bool console = false, bool force = false,
			string filter = null)
		{
			// Check preconditions.
			if (!Context.IsWorldReady)
				throw new Exception ("The world is not ready.");
			if (!Context.IsMainPlayer)
				throw new InvalidOperationException ("Only the host can do that.");

			// Choose a crop tile.
			CropTile tile = chooser.choose (console: console, force: force,
				filter: filter);
			if (tile == null)
				return false;

			// Spawn the cropbeast.
			Spawner.Spawn (tile);
			return true;
		}

		internal void revertCropbeasts (bool console = false)
		{
			// Check preconditions.
			if (!Context.IsWorldReady)
				throw new Exception ("The world is not ready.");
			if (!Context.IsMainPlayer)
				throw new InvalidOperationException ("Only the host can do that.");

			// Clean up dead beasts and check if any remain.
			cleanUpMonsters ();
			int count = currentBeastCount;
			if (count == 0)
			{
				if (console)
				{
					Monitor.Log ("There are no cropbeasts active to be reverted.",
						LogLevel.Warn);
				}
				return;
			}

			// Revert the remaining beasts and clear the list.
			foreach (Cropbeast beast in currentBeasts)
				beast.revert ();
			cleanUpMonsters ();

			Monitor.Log ($"Reverted {count} cropbeast(s) to their original crops.",
				console ? LogLevel.Info : LogLevel.Debug);
		}

		internal void cleanUpMonsters ()
		{
			if (currentMonsters == null)
				return;

			for (int i = currentMonsters.Count - 1; i >= 0; --i)
			{
				FarmMonster monster = currentMonsters[i];
				if (monster.Health <= 0 || (monster.currentLocation != null &&
					!monster.currentLocation.characters.Contains (monster)))
				{
					monster.processDeath ();
					currentMonsters.RemoveAt (i);
				}
			}
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();

			// Access the Json Assets API and mod instance, if they are available.
			jsonAssets = Helper.ModRegistry.GetApi<JsonAssets.IApi>
				("spacechase0.JsonAssets");
			Type jaModType = jsonAssets?.GetType ()?.Assembly
				?.GetType ("JsonAssets.Mod");
			jsonAssetsMod = jaModType?.GetField ("instance")
				?.GetValue (jaModType);

			// Load the Json Assets content pack, if JA is available.
			jsonAssets?.LoadAssets (Path.Combine (Helper.DirectoryPath,
				"assets", "JA"));
		}

		private void prepareSave ()
		{
			// Find the hat's ID.
			Assets.HatEditor.PrepareSave ();
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			Mappings.Load (true);
			currentMonsters = new List<FarmMonster> ();
			if (Context.IsMainPlayer)
				chooser = new Chooser ();

			// Check whether the achievement was attained previously but missed.
			Assets.AchievementEditor.CheckAchievement ();
		}

		private void onDayEnding (object _sender, DayEndingEventArgs _e)
		{
			if (Context.IsMainPlayer)
				revertCropbeasts ();
			currentMonsters = null;
			chooser = null;
		}

		private void onTimeChanged (object _sender, TimeChangedEventArgs _e)
		{
			if (chooser != null && currentMonsters != null)
			{
				cleanUpMonsters ();
				spawnCropbeast ();
			}
		}

		private void onUpdateTicked (object _sender, UpdateTickedEventArgs _e)
		{
			cleanUpMonsters ();

			if (Context.IsWorldReady && Game1.player.swimming.Value)
				SandblastDebuff.Duration -= SandblastDebuff.HealIncrement;
		}

		private void onRenderedWorld (object _sender, RenderedWorldEventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

#if DEBUG
			if (Config.BoundingBoxes)
			{
				// Draw the bounding boxes, compliments of spacechase0.
				foreach (NPC character in Game1.currentLocation.characters)
				{
					if (!(character is FarmMonster)) continue;
					Rectangle box = Game1.GlobalToLocal (Game1.viewport,
						character.GetBoundingBox ());
					e.SpriteBatch.Draw (Game1.staminaRect, new Rectangle
						(box.X, box.Y, box.Width, 1), Color.Red);
					e.SpriteBatch.Draw (Game1.staminaRect, new Rectangle
						(box.X, box.Y, 1, box.Height), Color.Red);
					e.SpriteBatch.Draw (Game1.staminaRect, new Rectangle
						(box.X + box.Width, box.Y, 1, box.Height), Color.Red);
					e.SpriteBatch.Draw (Game1.staminaRect, new Rectangle
						(box.X, box.Y + box.Height, box.Width, 1), Color.Red);
				}
			}
#endif

			if (Config.CactusbeastSandblast)
				SandblastDebuff.Draw (e.SpriteBatch);
		}

		private void onRenderedHUD (object _sender, RenderedHudEventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			if (Config.TrackingArrows && !Game1.eventUp)
			{
				foreach (NPC character in Game1.currentLocation.characters)
				{
					if (character is FarmMonster monster)
						monster.drawTrackingArrow (e.SpriteBatch);
				}
			}
		}

		private void onMenuChanged (object _sender, MenuChangedEventArgs e)
		{
			// Check for the Hat Mouse and fix their inventory.
			if (e.NewMenu is ShopMenu menu && menu.potraitPersonDialogue ==
				Game1.parseText (Game1.content.LoadString ("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"),
					Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4))
            {
				Assets.HatEditor.FixShopMenu (menu);
			}
		}

		private void onModMessageReceived (object _sender,
			ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModManifest.UniqueID)
				return;

			switch (e.Type)
			{
			case "CreateSpawner":
				if (Context.IsWorldReady && !Context.IsMainPlayer)
					Spawner.Create (e.ReadAs<Spawner.Message> ());
				break;
			}
		}
	}
}
