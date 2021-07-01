/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/portabletv
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PortableTV
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		internal JsonAssets.IApi jsonAssets { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		public int parentSheetIndex { get; private set; } = -1;

		private readonly PerScreen<PortableTV> tv_ = new (() => new ());
		public PortableTV tv => tv_.Value;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("portable_tv",
				"Starts a Portable TV, regardless of whether one is in inventory.",
				(string _command, string[] _args) => show ());
			Helper.ConsoleCommands.Add ("reset_portable_tv",
				"Resets a Portable TV in case it gets stuck.",
				(string _command, string[] _args) => reset ());

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
			Helper.Events.Display.RenderedWorld += onRenderedWorld;

			// Set up asset loaders/editors.
			Helper.Content.AssetEditors.Add (new ObjectEditor ());
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
					LogLevel.Error);
			}

			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();
		}

		private void onIdsAssigned (object _sender, EventArgs _e)
		{
			// Discover the object ID of the "Portable TV" inventory object.
			parentSheetIndex = jsonAssets.GetObjectId ("Portable TV");
			if (parentSheetIndex == -1)
			{
				Monitor.Log ("Could not find the ID of the Portable TV inventory object. The Json Assets content pack may not have loaded correctly.",
					LogLevel.Error);
			}
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			// Reset the Portable TV furniture item to avoid possible issues.
			reset ();
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			// Only respond if the player is free to interact with a TV.
			if (!Context.IsWorldReady || Game1.player.IsBusyDoingSomething () ||
					Game1.IsChatting)
				return;

			// Only respond if the button press is relevant to a Portable TV.
			if (!isRelevantPress (e))
				return;

			Helper.Input.Suppress (e.Button);
			show ();
		}

		private bool isRelevantPress (ButtonPressedEventArgs e)
		{
			// On any platform, accept the configured keybinding iff a Portable
			// TV is in inventory.
			if (e.Button == Config.ActivateKey)
			{
				return Game1.player.hasItemInInventory (parentSheetIndex, 1);
			}

			// Get the cursor coordinates on screen in UI scaling.
			var coords = Utility.ModifyCoordinatesForUIScale (e.Cursor.ScreenPixels);
			int x = (int) coords.X;
			int y = (int) coords.Y;

			// Get relevant UI elements.
			var toolbar = Game1.onScreenMenus.First ((m) => m is Toolbar) as Toolbar;
			var toolbarButtons = (toolbar != null)
				? Helper.Reflection.GetField<List<ClickableComponent>> (toolbar, "buttons")
				: null;

			// On Android, must be tapping in the world while the Portable TV
			// is the current item.
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				if (e.Button != SButton.MouseLeft)
					return false;

				// Allow a fixed area for the right-edge HUD elements.
				if (x > Game1.viewport.Width - 80)
					return false;

				if (toolbar?.isWithinBounds (x, y) ?? false)
					return false;

				return Utility.IsNormalObjectAtParentSheetIndex (Game1.player.CurrentItem,
					parentSheetIndex);
			}

			// On other platforms, must be clicking the use button.
			if (!e.Button.IsActionButton ())
				return false;

			// If clicking on a toolbar button, must be a Portable TV item.
			Item toolbarItem = null;
			foreach (ClickableComponent button in toolbarButtons?.GetValue () ?? new ())
			{
				if (button.containsPoint (x, y))
				{
					toolbarItem = Game1.player.Items[Convert.ToInt32 (button.name)];
					break;
				}
			}
			if (toolbarItem != null)
			{
				return Utility.IsNormalObjectAtParentSheetIndex (toolbarItem,
					parentSheetIndex);
			}

			// Otherwise, must have no interaction at the grab tile.
			if (Game1.currentLocation.isActionableTile
					((int) e.Cursor.GrabTile.X, (int) e.Cursor.GrabTile.Y,
						Game1.player))
				return false;

			// And finally must be holding a Portable TV.
			return Utility.IsNormalObjectAtParentSheetIndex (Game1.player.CurrentItem,
				parentSheetIndex);
		}

		private void onRenderedWorld (object sender, RenderedWorldEventArgs e)
		{
			tv?.draw (e.SpriteBatch, 0, 0);
		}

		private void show ()
		{
			try
			{
				if (tv == null || !Context.IsWorldReady ||
						Game1.player.IsBusyDoingSomething () || Game1.IsChatting)
					throw new Exception ("Not ready to show Portable TV right now.");
				tv.turnOnTV ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"Couldn't activate Portable TV: {e.Message}", LogLevel.Error);
			}
		}

		private void reset ()
		{
			try
			{
				if (!Context.IsWorldReady)
					throw new Exception ("Cannot reset Portable TV without an active save.");
				tv_.Value = new PortableTV ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"Couldn't reset Portable TV: {e.Message}", LogLevel.Error);
			}
		}
	}
}
