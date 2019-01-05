using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static BattleRoyale.NetworkUtility;

namespace BattleRoyale
{
    internal class AutoKicker
	{
		internal static Dictionary<long, Tuple<int, int, byte[]>> playersToVersions = new Dictionary<long, Tuple<int, int, byte[]>>();

		public bool ProcessPlayerJoin(NetFarmerRoot farmer)
		{
			var pl = ModEntry.BRGame.ModHelper.Multiplayer.GetConnectedPlayer(farmer.Value.UniqueMultiplayerID);
			if (!pl.HasSmapi)
			{
				Console.WriteLine("Kicking because does not have SMAPI");

				SendChatMessageToPlayerWithoutMod(farmer.Value.UniqueMultiplayerID, "Read the instruction page before joining.");
				Game1.server.sendMessage(farmer.Value.UniqueMultiplayerID, new OutgoingMessage((byte)19, farmer.Value.UniqueMultiplayerID, new object[0]));
				Game1.server.playerDisconnected(farmer.Value.UniqueMultiplayerID);
				Game1.otherFarmers.Remove(farmer.Value.UniqueMultiplayerID);
				return false;
			}

			Console.WriteLine($"Player joined with mods:");
			foreach (IMultiplayerPeerMod mod in pl.Mods)
			{
				Console.WriteLine($" - '{mod.ID}' / '{mod.Name}' / '{mod.Version}'");
				if (!AntiCheat.IsLegal(mod))
				{
					Console.WriteLine($"^ found illegal mod");
					SendChatMessageToPlayerWithoutMod(farmer.Value.UniqueMultiplayerID, $"Mod {mod.Name} is not permitted.");
					Game1.server.sendMessage(farmer.Value.UniqueMultiplayerID, new OutgoingMessage((byte)19, farmer.Value.UniqueMultiplayerID, new object[0]));
					Game1.server.playerDisconnected(farmer.Value.UniqueMultiplayerID);
					Game1.otherFarmers.Remove(farmer.Value.UniqueMultiplayerID);
					return false;
				}
			}

			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(ModEntry.Config.TimeInMillisecondsBetweenPlayerJoiningAndServerExpectingTheirVersionNumber);
				if (playersToVersions.TryGetValue(farmer.Value.UniqueMultiplayerID, out var theirVersion))
				{
					//Item1 = major, Item2 = minor, Item3 = sha
                    var tp = GetMyVersion();
					var major = tp.Item1;
					var minor = tp.Item2;
					var sha = tp.Item3;

					if (theirVersion.Item1 != major || theirVersion.Item2 != minor|| !theirVersion.Item3.SequenceEqual(sha))
					{
						//Kick them for wrong version
						playersToVersions.Remove(farmer.Value.UniqueMultiplayerID);
						KickPlayer(farmer.Value, $"Incompatible mod version. You need {major}.{minor}");
						return;
					}
				}
				else
				{
                    //They don't have the mod installed or they have glitched (or really slow internet?)
                    Console.WriteLine("Kicking");
                    //KickPlayer(farmer.Value, $"Could not connect in time");
                    SendChatMessageToPlayerWithoutMod(farmer.Value.UniqueMultiplayerID, "Could not connect in time");
                    Game1.server.sendMessage(farmer.Value.UniqueMultiplayerID, new OutgoingMessage((byte)19, farmer.Value.UniqueMultiplayerID, new object[0]));
                    Game1.server.playerDisconnected(farmer.Value.UniqueMultiplayerID);
                    Game1.otherFarmers.Remove(farmer.Value.UniqueMultiplayerID);
                    return;
				}
			});

			return true;
		}

        
        public void SendMyVersionToTheServer(Client client)
		{
            var m = new OutgoingMessage(uniqueMessageType, Game1.player, 
				(int)MessageTypes.SEND_MY_VERSION_TO_SERVER, (int)GetMyVersion().Item1, (int)GetMyVersion().Item2, GetSHA());

			if (client != null)
				client.sendMessage(m);
			else
				Console.WriteLine("Client null: can't send version info to server");
		}

        private byte[] GetSHA()
        {
            using (FileStream stream = new FileStream(System.Reflection.Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(stream);
            }
        }

        public void AcknowledgeClientVersion(long clientID, int major, int minor, byte[] sha)
		{
			playersToVersions.Remove(clientID);
			playersToVersions.Add(clientID, Tuple.Create(major, minor, sha));
		}

		private Tuple<int, int, byte[]> GetMyVersion()
		{
			var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			return Tuple.Create(v.Major, v.Minor, GetSHA());
		}
	}
}
