/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }
		protected static ModConfig Config => ModConfig.Instance;

		public static readonly List<int> Crystals = new () { 319, 320, 321 };

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("detonate_flower_bombs",
				"Detonates all Flower Bombs currently placed in the world.",
				cmdDetonateFlowerBombs);

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.GameLoop.Saving += onSaving;
			Helper.Events.GameLoop.Saved += onSaved;
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
			Helper.Events.Display.MenuChanged += onMenuChanged;

			// Set up asset loaders/editors.
			Helper.Content.AssetEditors.Add (new EventsEditor ());
			Helper.Content.AssetEditors.Add (new MailEditor ());
		}

		private void cmdDetonateFlowerBombs (string _command, string[] _args)
		{
			try
			{
				if (!Context.IsMainPlayer)
					throw new Exception ("Cannot detonate Flower Bombs without an active save as host.");
				FlowerBomb.DetonateAll ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"detonate_flower_bombs failed: {e.Message}", LogLevel.Error);
			}
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();

			// Register PyTK custom objects.
			FlowerBomb.Register ();

			// Hook into Save Anywhere for compatibility with it.
			var SaveAnywhere = Helper.ModRegistry.GetApi
				<Omegasis.SaveAnywhere.Framework.ISaveAnywhereAPI>
				("Omegasis.SaveAnywhere");
			if (SaveAnywhere != null)
			{
				SaveAnywhere.BeforeSave += onSaving;
				SaveAnywhere.AfterSave += onSaved;
				SaveAnywhere.AfterLoad += onSaveLoaded;
			}
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			if (!Game1.player.knowsRecipe ("Flower Bomb"))
			{
				if (!Config.LeahRecipe)
				{
					// Unlock the recipe directly.
					Game1.player.craftingRecipes.Add ("Flower Bomb", 0);
				}
				else if (Game1.player.getSpouse ()?.Name == "Leah")
				{
					// Ensure that the correct version of the event is loaded.
					Helper.Content.InvalidateCache ("Data\\Events\\FarmHouse");
				}
				else
				{
					// Deliver the letter if Leah befriended and not winter.
					if (!Game1.player.hasOrWillReceiveMail (MailEditor.RecipeKey) &&
							Game1.player.getFriendshipHeartLevelForNPC ("Leah") >= 2 &&
							Game1.currentSeason != "winter")
						Game1.player.mailbox.Add (MailEditor.RecipeKey);
				}
			}

			if (Context.IsMainPlayer)
			{
				// Melt any crystals left over from winter.
				foreach (GameLocation location in Game1.locations)
				{
					if (location is MineShaft ||
							Game1.GetSeasonForLocation (location) == "winter")
						continue;
					foreach (var kvp in location.objects.Pairs.ToArray ())
					{
						if (Crystals.Contains (kvp.Value.ParentSheetIndex))
							location.objects.Remove (kvp.Key);
					}
				}

				// Detonate the Flower Bombs.
				FlowerBomb.DetonateAll ();
			}
		}

		private void onSaving (object _sender, EventArgs _e)
		{
			Flower.RevertAll ();
		}

		private void onSaved (object _sender, EventArgs _e)
		{
			Flower.ConvertAll ();
		}

		private void onSaveLoaded (object _sender, EventArgs _e)
		{
			Flower.ConvertAll ();
		}

		private void onUpdateTicked (object _sender, UpdateTickedEventArgs e)
		{
			if (!e.IsMultipleOf (5) || !Context.IsMainPlayer ||
					Game1.currentLocation == null)
				return;

			foreach (var projectile in Game1.currentLocation.projectiles
					.OfType<BasicProjectile> ())
				FlowerBomb.FixProjectile (projectile);
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			// Only respond if the world is ready and the player can interact.
			if (!Context.IsWorldReady || Game1.dialogueUp ||
					(Game1.eventUp && !Game1.isFestival ()) ||
					Game1.player.UsingTool || Game1.IsChatting)
				return;

			// Check for a Flower Bomb that can handle the action.
			if (FlowerBomb.HandleButtonPress (e))
				Helper.Input.Suppress (e.Button);
		}

		private static readonly int MudstoneIndex = 574;
		private static readonly int MudstoneLimit = 5;

		private void onMenuChanged (object _sender, MenuChangedEventArgs e)
		{
			// Include some Mudstone in Clint's inventory if so configured.
			if (Config.ClintMudstone &&
				e.NewMenu is ShopMenu menu &&
				menu.portraitPerson?.Name == "Clint")
			{
				var forSale = Helper.Reflection.GetField
					<List<ISalable>> (menu, "forSale").GetValue ();
				var itemPriceAndStock = Helper.Reflection.GetField
					<Dictionary<ISalable, int[]>> (menu, "itemPriceAndStock")
					.GetValue ();

				Item item = new SObject (Vector2.Zero, MudstoneIndex,
					MudstoneLimit);
				forSale.Add (item);
				itemPriceAndStock.Add (item, new int[] { item.salePrice (),
					MudstoneLimit });
			}
		}
	}
}
