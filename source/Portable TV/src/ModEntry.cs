using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortableTV
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		public PortableTV tv { get; private set; }
		public int parentSheetIndex { get; private set; } = -1;

		private WeakReference<Toolbar> toolbar;
		private IReflectedField<List<ClickableComponent>> toolbarButtons;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("portable_tv",
				"Starts a Portable TV, regardless of whether one is in inventory.",
				cmdPortableTV);
			Helper.ConsoleCommands.Add ("reset_portable_tv",
				"Resets a Portable TV in case it gets stuck.",
				cmdResetPortableTV);

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.GameLoop.DayStarted += onDayStarted;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
			Helper.Events.Display.RenderedWorld += onRenderedWorld;
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();
		}

		private void onSaveLoaded (object _sender, SaveLoadedEventArgs _e)
		{
			// Discover the object ID of the "Portable TV" inventory object.
			var JsonAssets = Helper.ModRegistry.GetApi<JsonAssets.IApi>
				("spacechase0.JsonAssets");
			if (JsonAssets != null)
			{
				parentSheetIndex = JsonAssets.GetObjectId ("Portable TV");
				if (parentSheetIndex == -1)
				{
					Monitor.Log ("Could not find the ID of the Portable TV inventory object. The Portable TV content pack for Json Assets may not be installed.",
						LogLevel.Error);
				}
			}
			else
			{
				Monitor.LogOnce ("Could not connect to Json Assets. It may not be installed or working properly.",
					LogLevel.Error);
			}
		}

		private void onDayStarted (object _sender, DayStartedEventArgs _e)
		{
			// Set up the PortableTV. This furniture item isn't actually made or
			// handled by the player; it only handles running the TV on screen
			// when the "Portable TV" inventory object is interacted with.
			tv = new PortableTV ();

			// Have the the inventory toolbar and its buttons ready for checking
			// in onButtonPressed.
			var toolbar = Game1.onScreenMenus.First ((m) => m is Toolbar) as Toolbar;
			this.toolbar = new WeakReference<Toolbar> (toolbar);
			toolbarButtons = Helper.Reflection.GetField<List<ClickableComponent>>
				(toolbar, "buttons");
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			// Only respond if the TV and the world are ready and the player is
			// free to interact with a TV.
			if (tv == null || !Context.IsWorldReady || !Context.IsPlayerFree ||
					Game1.player.UsingTool || Game1.IsChatting)
				return;

			// On any platform, accept the configured keybinding if a Portable
			// TV is in inventory.
			if (e.Button == Config.ActivateKey)
			{
				if (!Game1.player.hasItemInInventory (parentSheetIndex, 1))
					return;
			}
			// On Android, must be tapping in the world while the Portable TV
			// is the current item.
			else if (Constants.TargetPlatform == GamePlatform.Android)
			{
				if (e.Button != SButton.MouseLeft)
					return;

				int x = (int) e.Cursor.ScreenPixels.X;
				int y = (int) e.Cursor.ScreenPixels.Y;

				// Allow a fixed area for the right-edge HUD elements.
				if (x > Game1.viewport.Width - 80)
					return;

				this.toolbar.TryGetTarget (out Toolbar toolbar);
				if (toolbar?.isWithinBounds (x, y) ?? false)
					return;

				if (!Utility.IsNormalObjectAtParentSheetIndex
						(Game1.player.CurrentItem, parentSheetIndex))
					return;
			}
			else
			{
				// On Linux/Mac/Windows, only respond to the use button.
				if (!e.Button.IsActionButton ())
					return;

				// If clicking on the toolbar, must be clicking on a Portable TV.
				Item toolbarItem = null;
				foreach (ClickableComponent button in toolbarButtons.GetValue ())
				{
					if (button.containsPoint ((int) e.Cursor.ScreenPixels.X,
						(int) e.Cursor.ScreenPixels.Y))
					{
						toolbarItem = Game1.player.Items[Convert.ToInt32 (button.name)];
						break;
					}
				}
				if (toolbarItem != null &&
						!Utility.IsNormalObjectAtParentSheetIndex
							(toolbarItem, parentSheetIndex))
					return;

				// If not clicking on a toolbar button, must be holding the TV
				// and have no interaction at the grab tile.
				if (toolbarItem == null)
				{
					if (!Utility.IsNormalObjectAtParentSheetIndex
							(Game1.player.CurrentItem, parentSheetIndex))
						return;
					if (Game1.currentLocation.isActionableTile
							((int) e.Cursor.GrabTile.X, (int) e.Cursor.GrabTile.Y,
								Game1.player))
						return;
				}
			}
			

			// Activate the TV. Don't do anything else with this click.
			tv.turnOnTV ();
			Helper.Input.Suppress (e.Button);
		}

		private void onRenderedWorld (object sender, RenderedWorldEventArgs e)
		{
			if (tv != null)
				tv.draw (e.SpriteBatch, 0, 0);
		}

		private void cmdPortableTV (string _command, string[] _args)
		{
			try
			{
				tv.turnOnTV ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"Couldn't activate Portable TV: {e.Message}", LogLevel.Error);
			}
		}

		private void cmdResetPortableTV (string _command, string[] _args)
		{
			try
			{
				tv = new PortableTV ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"Couldn't reset Portable TV: {e.Message}", LogLevel.Error);
			}
		}
	}
}
