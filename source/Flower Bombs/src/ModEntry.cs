/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using PlatoTK;
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

		internal HarmonyInstance harmony { get; private set; }
		internal IPlatoHelper platoHelper { get; private set; }

		public static readonly List<int> Crystals = new () { 319, 320, 321 };

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
            harmony = HarmonyInstance.Create (ModManifest.UniqueID);
			platoHelper = HelperExtension.GetPlatoHelper (this);
			ModConfig.Load ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("germinate_flower_bombs",
				"Germinates all Flower Bombs currently placed in the world.\n\nUsage: germinate_flower_bombs [B:force]\n- force: whether to ignore any water requirement (default true)",
				cmdGerminateFlowerBombs);

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
			Helper.Events.Display.MenuChanged += onMenuChanged;

			// Set up asset loaders/editors.
			Helper.Content.AssetEditors.Add (new EventsEditor ());
			Helper.Content.AssetEditors.Add (new MailEditor ());
		}

		private void cmdGerminateFlowerBombs (string _command, string[] args)
		{
			try
			{
				if (!Context.IsWorldReady)
					throw new Exception ("Cannot germinate Flower Bombs without an active save.");
				if (!Context.IsMainPlayer)
					throw new Exception ("Cannot germinate all Flower Bombs as a farmhand.");

				Queue<string> argq = new (args);
				bool force = true;

				if (argq.Count > 0 && bool.TryParse (argq.Peek (), out bool force_))
				{
					force = force_;
					argq.Dequeue ();
				}

				FlowerBomb.GerminateAll (force: force);
			}
			catch (Exception e)
			{
				Monitor.Log ($"germinate_flower_bombs failed: {e.Message}", LogLevel.Error);
			}
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();

			// Register custom objects.
			FlowerBomb.Register ();
			Wildflower.Register ();
		}

		private void onSaveLoaded (object _sender, EventArgs _e)
		{
			if (Context.IsMainPlayer)
			{
				FlowerBomb.MigrateV1 ();
				Wildflower.MigrateV1 ();
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
							Game1.Date.Season != "winter")
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
					foreach (var pair in location.objects.Pairs.ToArray ())
					{
						if (Crystals.Contains (pair.Value.ParentSheetIndex))
							location.objects.Remove (pair.Key);
					}
				}

				// Germinate the Flower Bombs. Delay to allow for PlatoTK linking.
				DelayedAction.functionAfterDelay (() => FlowerBomb.GerminateAll (), 1);
			}
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
