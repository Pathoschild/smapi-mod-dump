/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

using Galaxy.Api;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerBrowser
{
	internal static class ServerMetaData
	{
		public static void InnitServer(GalaxyID id)
		{
			Console.WriteLine("Initialising server metadata");
			var mm = GalaxyInstance.Matchmaking();

			string serverBrowserVersion = ModEntry.ModHelper.ModRegistry.Get("Ilyaki.ServerBrowser").Manifest.Version.ToString();
			mm.SetLobbyData(id, "serverBrowserVersion", serverBrowserVersion);

			string serverMessage = ModEntry.Config.ServerMessage;
			mm.SetLobbyData(id, "serverMessage", serverMessage);

			StringBuilder sb = new StringBuilder();
			foreach (var modID in ModEntry.Config.RequiredMods)
			{
				sb.Append(modID);
				sb.Append(',');
			}
			mm.SetLobbyData(id, "requiredMods", sb.ToString());

			sb.Clear();
			foreach (var modID in ModEntry.ModHelper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID))
			{
				sb.Append(modID);
				sb.Append(',');
			}
			mm.SetLobbyData(id, "serverMods", sb.ToString());

			string password = ModEntry.Config.Password;
			mm.SetLobbyData(id, "password", password);
		}

		public static void SetTemporaryPassword(GalaxyID id, string password)
		{
			GalaxyInstance.Matchmaking().SetLobbyData(id, "password", password);
		}

		public static void UpdateServerTick(GalaxyID id)
		{
			try
			{
				var mm = GalaxyInstance.Matchmaking();
				int numberOfPlayers = Game1.getOnlineFarmers().Count;

				int numberOfPlayerSlots = -1;
				try
				{
					Multiplayer mp = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
					numberOfPlayerSlots = mp.playerLimit;
				}
				catch (Exception) { }

				int freeCabins = GetCabinsInsides().Where(x => x.owner == null || x.owner.Name.Length == 0).Count();

				mm.SetLobbyData(id, "numberOfPlayers", numberOfPlayers.ToString());
				mm.SetLobbyData(id, "numberOfPlayerSlots", numberOfPlayerSlots.ToString());
				mm.SetLobbyData(id, "freeCabins", freeCabins.ToString());
			}
			catch(Exception)
			{
				Console.WriteLine("Could not update server metadata - likely because the world is not yet initialised");
			}
		}

		private static IEnumerable<Cabin> GetCabinsInsides()
		{
			foreach (Building building in GetCabinsOutsides())
			{
				yield return building.indoors.Value as Cabin;
			}
		}

		private static IEnumerable<Building> GetCabinsOutsides()
		{
			if (Game1.getFarm() != null)
			{
				foreach (Building building in Game1.getFarm().buildings)
				{
					if (building.daysOfConstructionLeft.Value <= 0 && building.indoors.Value is Cabin)
					{
						yield return building;
					}
				}
			}
		}
	}
}
