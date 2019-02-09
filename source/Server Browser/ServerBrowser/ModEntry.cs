using Galaxy.Api;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace ServerBrowser
{
	class ModEntry : Mod
	{
		public static SearchOptions SearchOptions { get; private set; } = new SearchOptions()
		{
			ShowFullServers = true, ShowFullCabinServers = true, ShowPasswordProtectedSerers = false, SearchQuery = ""
		};

		internal static GalaxyID CurrentCreatedLobby = null;
		internal static IModHelper ModHelper { get; private set; }
		protected static CallResult<LobbyMatchList_t> callr;

		internal static ModConfig Config { get; private set; }

		private CSteamID lastLobbyJoined = new CSteamID(0);

		public static Texture2D RefreshTexture { get; private set; }
				
		public override void Entry(IModHelper helper)
		{
			ModHelper = helper;

			LoadConfig();
			CoopMenuHolder.PublicCheckBox.IsChecked = Config.PublicCheckedByDefault;

			RefreshTexture = helper.Content.Load<Texture2D>("refresh.png");

			Patch.PatchAll("Ilyaki.ServerBrowser");

			callr = CallResult<LobbyMatchList_t>.Create(OnReceiveSteamServers);

			helper.ConsoleCommands.Add("serverbrowser_setTemporaryPassword", "Set a temporary password for the current server. Usage: serverbrowser_setTemporaryPassword <password>",
				(cmd, args) => {
					if (args.Length != 1)
						Monitor.Log("Need a password to set", LogLevel.Error);
					else if (!Game1.IsServer)
						Monitor.Log("You need to be the server host", LogLevel.Error);
					else if (CurrentCreatedLobby == null)
						Monitor.Log("Can't find a connected server", LogLevel.Error);
					else
					{
						ServerMetaData.SetTemporaryPassword(CurrentCreatedLobby, args[0]);
						Monitor.Log("Set the temporary password", LogLevel.Info);
					}
				});

			helper.ConsoleCommands.Add("serverbrowser_reloadConfig", "Reload the config.json",
				(cmd, args) => {
					if (args.Length != 0)
						Monitor.Log("This command required no parameters", LogLevel.Error);
					else
					{
						LoadConfig();
						Monitor.Log("Reloaded config", LogLevel.Info);

						if (Game1.IsServer && CurrentCreatedLobby != null)
						{
							ServerMetaData.InnitServer(CurrentCreatedLobby);
						}
					}
				});

			helper.Events.GameLoop.UpdateTicked += (o, e) =>
			{
				if (CurrentCreatedLobby != null && Game1.IsServer && e.IsMultipleOf(60 * 5))
				{
					ServerMetaData.UpdateServerTick(CurrentCreatedLobby);
				}
			};
		}

		private void LoadConfig()
		{
			Config = Helper.ReadConfig<ModConfig>();
			if (Config == null)
			{
				Config = new ModConfig();
				Helper.WriteConfig(Config);
			}
		}

		internal static void OpenServerBrowser()
		{
			SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
			callr.Set(handle);
		}
		
		private GalaxyID GalaxyIDFromSteamID(CSteamID steamID)
		{
			GalaxyID parseConnectionString(string connectionString)
			{
				if (connectionString.StartsWith("-connect-lobby-"))
				{
					return new GalaxyID(Convert.ToUInt64(connectionString.Substring("-connect-lobby-".Length)));
				}
				if (connectionString.StartsWith("+connect_lobby "))
				{
					return new GalaxyID(Convert.ToUInt64(connectionString.Substring("+connect_lobby".Length + 1)));
				}
				return null;
			}

			return parseConnectionString(SteamMatchmaking.GetLobbyData(steamID, "connect"));
		}

		private void OnReceiveSteamServers(LobbyMatchList_t x, bool bIOFailure)
		{
			if (bIOFailure)
			{
				Monitor.Log("IO Failure!", LogLevel.Error);
				return;
			}

			Console.WriteLine($"STEAM RECEIVE SERVER LIST, COUNT={x.m_nLobbiesMatching}");

			if (lastLobbyJoined.m_SteamID != 0)
			{
				Console.WriteLine("Disconnecting from last joined lobby");

				try
				{
					GalaxyInstance.Matchmaking().LeaveLobby(GalaxyIDFromSteamID(lastLobbyJoined));
				}
				catch (Exception)
				{
					Console.WriteLine("  was not connected to any lobby");
				}
			}

			List<CSteamID> servers = new List<CSteamID>();

			BrowserMenu browser = null;

			int serverI = 0;
			while (true)
			{
				CSteamID steamID = SteamMatchmaking.GetLobbyByIndex(serverI);
				if (!steamID.IsValid() || steamID.m_SteamID == 0)
					break;
				
				Console.WriteLine($"DISCOVERED SERVERID={steamID.m_SteamID}");

				var galaxyID = GalaxyIDFromSteamID(steamID);
				
				Console.WriteLine($"Received galaxy ID = {galaxyID?.ToString() ?? "NULL"}");

				if (galaxyID != null)
				{
					servers.Add(steamID);

					var mm = GalaxyInstance.Matchmaking();

					var unkown = mm.RequestLobbyData(galaxyID);
					Console.WriteLine($"Request lobby data output = {unkown}");

					Task task = DelayForLobbyData(steamID, galaxyID, serverI, id => browser?.GetSlot(id), id => browser?.RemoveSlot(id));
				}

				serverI++;
			}
			if (servers.Count == 0)
			{
				Monitor.Log("Couldn't find any servers!", LogLevel.Info);
			}

			var blankTitle = new TitleMenu();
			blankTitle.skipToTitleButtons();
			TitleMenu.subMenu = new CoopMenu();

			//Game1.viewport.Height - 50 * 2
			browser = new BrowserMenu(25, 25, Game1.viewport.Width - 25 * 2, Game1.viewport.Height, servers, blankTitle);
			Game1.activeClickableMenu = browser;
		}

		async Task DelayForLobbyData(CSteamID steamID, GalaxyID galaxyID, int serverI, Func<CSteamID, BrowserSlot> getServerSlot, Action<CSteamID> removeSlot)
		{
			var mm = GalaxyInstance.Matchmaking();

			BrowserSlot slot = null;

			for (int i = 0; i < 20; i++)
			{
				slot = getServerSlot(steamID);
				if (slot == null)
					await Task.Delay(500);
				else
				{
					Console.WriteLine("Got slot ID");
					break;
				}
			}

			if (slot == null)
			{
				Console.WriteLine($"Failed to find slot for {steamID}");
				return;
			}

			for (int i = 0; i < 10; i++)
			{
				string getData(string key) => mm.GetLobbyData(galaxyID, key);

				string farmName = getData("farmName");

				if (string.IsNullOrEmpty(farmName))
				{
					Console.WriteLine($"Get lobby data fail number {i}");
					await Task.Delay(1000);
					continue;
				}

				string serverMessage = getData("serverMessage");
				string numberOfPlayers = getData("numberOfPlayers");
				string numberOfPlayerSlots = getData("numberOfPlayerSlots");
				string freeCabins = getData("freeCabins");
				string requiredMods = getData("requiredMods");
				string serversInstalledMods = getData("serverMods");
				string password = getData("password");

				string passwordYesNo = string.IsNullOrEmpty(password) ? "No" : "Yes";
				Monitor.Log($"Server {serverI} - '{farmName}'\nPlayers online = {numberOfPlayers}. Total slots = {numberOfPlayerSlots}. Empty cabins = {freeCabins}.\nPassword protected = {passwordYesNo}.\nRequired mods = {requiredMods}\nServer's installed mods = {serversInstalledMods}\n\nServer message:\n{serverMessage}\n", LogLevel.Info);

				slot.FarmName = farmName;
				slot.ServerDescription = serverMessage;
				int intPlayersOnline = int.TryParse(numberOfPlayers, out int x) ? x : -1;
				slot.PlayersOnline = intPlayersOnline;
				int intPlayerSlots = int.TryParse(numberOfPlayerSlots, out int y) ? y : -1;
				slot.PlayerSlots = intPlayerSlots;
				slot.CabinCountText = freeCabins;
				slot.ShowPasswordLockIcon = !string.IsNullOrEmpty(password);

				var sq = SearchOptions.SearchQuery.ToLower();

				if (
				(!SearchOptions.ShowPasswordProtectedSerers && !string.IsNullOrEmpty(password))
				||
				 (!SearchOptions.ShowFullServers && intPlayersOnline == intPlayerSlots)
				||
				 (!SearchOptions.ShowFullCabinServers && freeCabins == "0" || freeCabins == "-1")
				||
				(!string.IsNullOrWhiteSpace(sq) && !farmName.ToLower().Contains(sq) && !serverMessage.Contains(sq))
				){
					removeSlot(steamID);
					return;
				}

				slot.CallBack = delegate
				{
					var browswer = Game1.activeClickableMenu;

					Game1.activeClickableMenu = new ServerPage(25, 25, Game1.viewport.Width - 25 * 2, Game1.viewport.Height - 25 * 2, requiredMods, serversInstalledMods, delegate
					{
						var browser = Game1.activeClickableMenu;

						void connect()
						{
							Console.WriteLine($"Connecting to steam server {steamID}");

							//JoinLobby only works if you are at the title screen
							var title = new TitleMenu();
							title.skipToTitleButtons();
							Game1.activeClickableMenu = title;

							try
							{
								SteamMatchmaking.JoinLobby(steamID);
								lastLobbyJoined = steamID;
							}
							catch (Exception e)
							{
								Monitor.Log("Error while connecting to server: " + e.Message, LogLevel.Error);
							}
						}

						if (!string.IsNullOrEmpty(password))
						{
							Console.WriteLine("Showing password box");
							Game1.activeClickableMenu = new TextMenu("Please enter this server's password", true, (passwordInput) =>
							{
								if (password != passwordInput)
								{
									Console.WriteLine("Entered wrong password");
									Game1.activeClickableMenu = browser;
									return;
								}
								else
									connect();
							}, () => Game1.activeClickableMenu = browser);
						}
						else
							connect();
					}, 
					delegate
					{
						Game1.activeClickableMenu = browswer;
					}
					){
						PlayersOnline = numberOfPlayers,
						PlayerSlots = numberOfPlayerSlots,
						CabinCountText = freeCabins,
						ServerDescription = serverMessage,
						FarmName = farmName,
						ShowPasswordLockIcon = !string.IsNullOrEmpty(password)
					};
				};
				

				break;
			}
		}
	}
}
