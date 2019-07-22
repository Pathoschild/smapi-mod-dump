using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace WarpToFriends.Helpers
{
	public static class PlayerHelper
	{

		public static List<Farmer> GetAllCreatedFarmers()
		{
			return Game1.getAllFarmers().Where(f => !string.IsNullOrEmpty(f.Name)).ToList<Farmer>();
		}

		public static void warpFarmerToPlayer(Farmer toFarmer, Farmer player = null)
		{
			if (player == null) player = Game1.player;

			// Collection params
			var toLocation = (string.IsNullOrEmpty(toFarmer.currentLocation.uniqueName.Value)) ? toFarmer.currentLocation.Name : toFarmer.currentLocation.uniqueName.Value;
			bool isStruc = (string.IsNullOrEmpty(toFarmer.currentLocation.uniqueName.Value)) ? false : true;

			//Actual Warp
			player.warpFarmer(new Warp(0, 0, toLocation, (int)(toFarmer.position.X + 16) / Game1.tileSize, (int)(toFarmer.position.Y) / Game1.tileSize, false));

			//Old method
			//Game1.warpFarmer(toLocation, (int)(toFarmer.position.X + 16) / Game1.tileSize, (int)toFarmer.position.Y / Game1.tileSize, false);
		}

		// Testing functino for chat warps
		public static void MultiplayerWarpTesting(Farmer toFarmer, Farmer player = null)
		{
			if (Game1.IsClient) return;

			//object[] warParams = {toFarmer, (short)((player.position.X + 16) / Game1.tileSize), (short)((player.position.Y) / Game1.tileSize), toLocation.ToString(), isStruc };
			//ModEntry.Helper.Reflection.GetMethod(Game1.server, "warpFarmer", false).Invoke(warParams);

			//object[] warParams = { (int)((toFarmer.position.X + 16) / Game1.tileSize), (int)((toFarmer.position.Y) / Game1.tileSize), toLocation.ToString(), isStruc };
			//OutgoingMessage fakeMsg = new OutgoingMessage(5, player, warParams);


			/* Network warping Testing */

			//var toLocation = (string.IsNullOrEmpty(player.currentLocation.uniqueName.Value)) ? player.currentLocation.Name : player.currentLocation.uniqueName.Value;
			//bool isStruc = (string.IsNullOrEmpty(player.currentLocation.uniqueName.Value)) ? false : true;

			//ModEntry.Monitor.Log(toLocation.ToString());

			//object[] warParams = { 5, (int)((player.position.X + 16) / Game1.tileSize), (int)((player.position.Y) / Game1.tileSize), isStruc, toLocation.ToString() };

			//OutgoingMessage fakeMsg = new OutgoingMessage(4, player, warParams);
			//foreach (long key in Game1.otherFarmers.Keys)
			//{
			//	ModEntry.Monitor.Log(key.ToString());
			//	Game1.server.sendMessage(key, fakeMsg);
			//}
		}
	}
}
