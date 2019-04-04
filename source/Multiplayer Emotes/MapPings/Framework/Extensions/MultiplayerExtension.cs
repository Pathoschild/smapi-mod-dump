
using StardewValley;
using StardewValley.Network;
using System;
using MapPings.Framework.Events;
using MapPings.Framework.Constants;
using Netcode;
using Microsoft.Xna.Framework;

namespace MapPings.Framework.Extensions {

	public static class MultiplayerExtension {

		public static event EventHandler<PlayerConnectedEventArgs> OnPlayerConnected = delegate { };
		public static event EventHandler<PlayerDisconnectedEventArgs> OnPlayerDisconnected = delegate { };

		public static event EventHandler<ChatMessageEventArgs> OnReceiveChatMessage = delegate { };
		public static event EventHandler<ChatMessageEventArgs> OnSendChatMessage = delegate { };

		public static event EventHandler<ReceivedMapPingEventArgs> OnReceiveMapPing = delegate { };

		public static void SendMapPing(this Multiplayer multiplayer, long peerId, Vector2 mapCoords) {
			if(Game1.IsMultiplayer) {
				object[] objArray = new object[2] {
					Message.Action.SendMapPing.ToString(),
					mapCoords
				};

				OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else if(Game1.IsServer) {
					Game1.server.sendMessage(peerId, message);
				}
			}
		}

		public static void ReceiveMapPing(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Game1.IsMultiplayer && msg.Data.Length > 0) {
				ReceivedMapPingEventArgs args = new ReceivedMapPingEventArgs {
					SourceFarmer = msg.SourceFarmer,
					MapCoords = msg.Reader.ReadVector2()
				};
				OnReceiveMapPing(null, args);
			}
		}

		public static void BroadcastMapPing(this Multiplayer multiplayer, Vector2 mapCoords) {

			if(Game1.IsMultiplayer) {

				object[] objArray = new object[2] {
					Message.Action.BroadcastMapPing.ToString(),
					mapCoords
				};
				OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);

				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else if(Game1.IsClient) {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
						}
					}
				}

			}

		}

		public static void ReceiveMapPingBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Game1.IsMultiplayer && msg.Data.Length > 0) {
				ReceivedMapPingEventArgs args = new ReceivedMapPingEventArgs {
					SourceFarmer = msg.SourceFarmer,
					MapCoords = msg.Reader.ReadVector2()
				};
				OnReceiveMapPing(null, args);
			}
		}

		#region Vanilla MessageTypes

		//TODO: Not in use, remove if not needed
		public static void PlayerConnected(this Multiplayer multiplayer, IncomingMessage msg) {
			PlayerConnectedEventArgs args = new PlayerConnectedEventArgs {
				Player = multiplayer.readFarmer(msg.Reader).Value
			};
			ModEntry.ModMonitor.Log("PlayerConnected(this Multiplayer multiplayer, IncomingMessage msg)");
			OnPlayerConnected(null, args);
		}

		public static void PlayerConnected(this Multiplayer multiplayer, Farmer farmer) {
			PlayerConnectedEventArgs args = new PlayerConnectedEventArgs {
				Player = farmer
			};
			OnPlayerConnected(null, args);
		}

		//TODO: Not in use, remove if not needed
		public static void PlayerDisconnected(this Multiplayer multiplayer, IncomingMessage msg) {
			PlayerDisconnectedEventArgs args = new PlayerDisconnectedEventArgs {
				Player = msg.SourceFarmer
			};
			ModEntry.ModMonitor.Log("PlayerDisconnected(this Multiplayer multiplayer, IncomingMessage msg)");
			OnPlayerDisconnected(null, args);
		}

		public static void PlayerDisconnected(this Multiplayer multiplayer, Farmer farmer) {
			PlayerDisconnectedEventArgs args = new PlayerDisconnectedEventArgs {
				Player = farmer
			};
			OnPlayerDisconnected(null, args);
		}

		#endregion

		#region Chat Messages

		public static void ReceivedChatMessage(this Multiplayer multiplayer, Farmer farmer, LocalizedContentManager.LanguageCode language, string message) {
			ChatMessageEventArgs args = new ChatMessageEventArgs {
				SourcePlayer = farmer,
				Language = language,
				Message = message
			};
#if DEBUG
			ModEntry.ModLogger.LogTrace();
#endif
			OnReceiveChatMessage(null, args);
		}

		public static void SendedChatMessage(this Multiplayer multiplayer, Farmer farmer, LocalizedContentManager.LanguageCode language, string message) {
			ChatMessageEventArgs args = new ChatMessageEventArgs {
				SourcePlayer = farmer,
				Language = language,
				Message = message
			};
#if DEBUG
			ModEntry.ModLogger.LogTrace();
#endif
			OnSendChatMessage(null, args);
		}

		#endregion

	}

}
