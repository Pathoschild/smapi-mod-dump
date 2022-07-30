/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/SDV_WikiLinker
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace MenuTesting
{
	public class ModEntry : Mod
	{
		ModConfig Config;

		public static bool HideUnmetVillagers { get; set; }

		public override void Entry(IModHelper helper)
		{
			//Sets up Config
			Config = helper.ReadConfig<ModConfig>();
			HideUnmetVillagers = Config.HideUnmetVillagers;

			//Sets up event listener
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.Display.WindowResized += OnWindowResized;
		}

		//The menu location is based on screen size, so just reload the menu with the new screen size when it's resized
		private void OnWindowResized(object sender, WindowResizedEventArgs e)
		{
			if (Game1.activeClickableMenu is WikiWindow)
			{
				Game1.activeClickableMenu = new WikiWindow();
			}
		}

		//Handles input to toggle menu on/off
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			//Only listens if you're in a game; doesn't work on main menu or anything
			if (!Context.IsWorldReady) return;

			if (Config.ButtonToToggleMenu.JustPressed())
			{
				if (Game1.activeClickableMenu == null)
				{
					Game1.playSound("bigSelect");
					Game1.activeClickableMenu = new WikiWindow();
				}
				else if (Game1.activeClickableMenu is WikiWindow)
				{
					Game1.playSound("bigDeSelect");
					Game1.exitActiveMenu();
					Game1.player.canMove = true;
				}
			}
		}
	}
	class ModConfig
	{
		public KeybindList ButtonToToggleMenu { get; set; } = KeybindList.Parse("J");
		public bool HideUnmetVillagers { get; set; } = true;
	}
}
