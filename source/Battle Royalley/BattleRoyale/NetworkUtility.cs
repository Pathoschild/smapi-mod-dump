using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace BattleRoyale
{
	internal class NetworkUtility
	{
		public const byte uniqueMessageType = 39;//Some random number that the game doesn't use (Check Multiplayer.processIncomingMessage)

		public enum MessageTypes
		{
			KICK_PLAYER = 1,
			SEND_MY_VERSION_TO_SERVER = 2,
			TAKE_DAMAGE,
			SERVER_BROADCAST_GAME_START,
			SERVER_BROADCAST_GAME_END,
			RELAY_MESSAGE_TO_ANOTHER_PLAYER, //first 4 bytes: MessageTypes, next 8 bytes is player ID then next is the message
			TELL_SERVER_I_DIED,
			TELL_PLAYER_HIT_SHAKE_TIMER,//technically invincibility frames
			SERVER_BROADCAST_CHAT_MESSAGE,
			BROADCAST_ALIVE_COUNT,
			WARP,
			SEND_PHASE_DATA,
			TELL_NEW_PLAYER_TO_SPECTATE,
			GIVE_HAT
		}

		public static void RelayMessageFromClientToAnotherPlayer(long targetID, params object[] message)
		{
			var objectsList = new List<object>() { (int)MessageTypes.RELAY_MESSAGE_TO_ANOTHER_PLAYER, targetID };
			objectsList.AddRange(message);

			var relayMessage = new StardewValley.Network.OutgoingMessage(uniqueMessageType, Game1.player, objectsList.ToArray());
			
			if (Game1.IsServer)
				throw new Exception("Server should not be sending relay instructions");
			else
				Game1.client.sendMessage(relayMessage);
		}


        public static void SendDamageToPlayerServerOnly(int damage, long targetID)
        {
            object[] objects = new object[] { (int)MessageTypes.TAKE_DAMAGE, damage };

            if (Game1.IsServer)
                Game1.server.sendMessage(targetID, new StardewValley.Network.OutgoingMessage(uniqueMessageType, Game1.player, objects));
            //else
            //    RelayMessageFromClientToAnotherPlayer(target, objects);
        }

        public static void SendDamageToPlayer(int damage, Farmer target, long? damagerID = null) => SendDamageToPlayer(damage, target.UniqueMultiplayerID, damagerID);
        public static void SendDamageToPlayer(int damage, long targetID, long? damagerID = null)
		{
            object[] objects;
            if (damagerID.HasValue)
                objects = new object[] { (int)MessageTypes.TAKE_DAMAGE, damage, damagerID.Value };
            else
                objects = new object[] { (int)MessageTypes.TAKE_DAMAGE, damage };

            if (Game1.IsServer)
				Game1.server.sendMessage(targetID, new OutgoingMessage(uniqueMessageType, Game1.player, objects));
			else
				RelayMessageFromClientToAnotherPlayer(targetID, objects);
		}

		public static void BroadcastGameStartToClient(Farmer target, int numberOfPlayers, bool isHostParticipating, int stormIndex)
		{
			if (!Game1.IsServer)
			{
				Console.WriteLine("Can't broadcast game start: not the server");
				return;
			}

			var message = new OutgoingMessage(uniqueMessageType, Game1.player, (int)MessageTypes.SERVER_BROADCAST_GAME_START, (int)numberOfPlayers, (int)stormIndex, isHostParticipating);
			
			Game1.server.sendMessage(target.UniqueMultiplayerID, message);
		}

		public static void BroadcastGameEndToClient(Farmer target)
		{
			if (!Game1.IsServer)
			{
				Console.WriteLine("Can't broadcast game end: not the server");
				return;
			}

            var message = new OutgoingMessage(uniqueMessageType, Game1.player, (int)MessageTypes.SERVER_BROADCAST_GAME_END);

			if (target != null)
				Game1.server?.sendMessage(target.UniqueMultiplayerID, message);
		}

		public static void SendChatMessageToAllPlayers(string message)
		{
			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player != Game1.player && player != null)
				{
					Game1.server?.sendMessage(player.UniqueMultiplayerID,
						new OutgoingMessage(uniqueMessageType, Game1.player, (int)MessageTypes.SERVER_BROADCAST_CHAT_MESSAGE, message));
				}
			}

            string colorName = "white";
            try
            {
                colorName = message.Substring(1, message.Substring(1).IndexOf(']'));
                Console.WriteLine($"color = {colorName}");
            }
            catch (Exception) { }

            var color = StardewValley.Menus.ChatMessage.getColorFromName(colorName);
            if (color == Microsoft.Xna.Framework.Color.White)
                color = Microsoft.Xna.Framework.Color.Gold;

            Game1.chatBox.addMessage(message, color);
		}

        public static void SendChatMessageToPlayerWithoutMod(long playerID, string message)
        {
            Game1.server?.sendMessage(playerID, new OutgoingMessage(10, Game1.player, LocalizedContentManager.LanguageCode.en, message));
        }

        public static void WarpFarmer(Farmer target, TileLocation targetLocation)
		{
			if (!Game1.IsServer)
				return;

			if (target == Game1.player)
			{
				//Face downwards & Warp
				Game1.player.FacingDirection = 2;
				Game1.player.warpFarmer(targetLocation.CreateWarp());

			}
			else
			{
				var message = new OutgoingMessage(uniqueMessageType, Game1.player, (int)MessageTypes.WARP,
					 targetLocation.tileX, targetLocation.tileY, targetLocation.locationName);

				Game1.server.sendMessage(target.UniqueMultiplayerID, message);
			}
		}

		public static void KickPlayer(Farmer target, string message)
		{
			if (!Game1.IsServer)
				return;

			Console.WriteLine($"Kicking {target.UniqueMultiplayerID}={target.Name}");
			var m = new OutgoingMessage(uniqueMessageType, Game1.player, (int)MessageTypes.KICK_PLAYER, message);

			Game1.server.sendMessage(target.UniqueMultiplayerID, m);
		}
	}
}
