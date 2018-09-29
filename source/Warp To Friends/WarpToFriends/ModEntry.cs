using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using WarpToFriends.Helpers;

namespace WarpToFriends
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		public static IModHelper Helper { get; private set; }
		public static IMonitor Monitor { get; private set; }
		public static ModConfig config;

		public override void Entry(IModHelper helper)
		{
			Helper = helper;
			Monitor = base.Monitor;
			config = helper.ReadConfig<ModConfig>();
			InputEvents.ButtonPressed += InputEvents_ButtonPressed;

			// Debug logs
			//PlayerEvents.Warped += Warped;
		}

		private void Warped(object sender, EventArgsPlayerWarped e)
		{
			Monitor.Log(e.NewLocation.name);
			Monitor.Log(e.NewLocation.uniqueName);
			Monitor.Log(e.NewLocation.Name);
		}

		private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
		{
			if (Context.IsWorldReady && e.Button.ToString() == config.OpenMenuKey)
			{
				if (!Context.IsPlayerFree)
				{
					if(Game1.activeClickableMenu is WarpMenu)
					{
						Game1.activeClickableMenu.exitThisMenu(true);
					}
				}
				else
				{
					Game1.activeClickableMenu = new WarpMenu();
				}
				
			}
		}
	}
}