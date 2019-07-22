
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using WarpToFriends.Helpers;

namespace WarpToFriends
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		public static IModHelper Helper { get; private set; }
		public static IMonitor Monitor { get; private set; }
		public static ModConfig config;
		public ChatReader chatReader;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Helper = helper;
			Monitor = base.Monitor;
			config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			helper.Events.GameLoop.ReturnedToTitle += this.OnReturnToTitle;

			//chatReader = new ChatReader(Helper);
			//ChatReader.NewChatMessageEvent += CheckChatCommand;
		}
		
		private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
		{
			Helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;
			
			// Debug Logs
			//Helper.Events.Player.Warped += Warped;
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			//chatReader.OnSaveLoaded();
		}

		private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			//chatReader.OnReturnToTitle();
		}

		// Testing
		private void CheckChatCommand(object sender, NewChatMessageEvent e)
		{
			if (!config.canConsoleWarp) return;

			string[] msgArray = e.message.Split(' ');

			if (msgArray[0].ToLower().Equals("!warp"))
			{
				if (msgArray.Length < 3) return;
				Farmer warpedFarmer = Game1.getOnlineFarmers().First(f => f.Name == msgArray[1]);
				Farmer warpToFarmer = Game1.getOnlineFarmers().First(f => f.Name == msgArray[2]);

				Monitor.Log(warpedFarmer.Name);
				Monitor.Log(warpToFarmer.Name);

				PlayerHelper.warpFarmerToPlayer(warpToFarmer, warpedFarmer);
			}
		}

		/// <summary>Raised after a player warps to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void Warped(object sender, WarpedEventArgs e)
		{
			Monitor.Log(e.NewLocation.Name);
			Monitor.Log(e.NewLocation.uniqueName);
			Monitor.Log(e.NewLocation.Name);
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// Open Warp Menu
			if (Context.IsWorldReady && e.Button == config.OpenMenuKey)
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