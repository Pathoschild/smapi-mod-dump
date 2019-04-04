
using System.Reflection;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Constants;

namespace CustomEmojis.Patches {

	internal static class MultiplayerPatch {

		internal class ProcessIncomingMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.processIncomingMessage), new Type[] { typeof(IncomingMessage) });
			public override MethodInfo Prefix => AccessTools.Method(this.GetType(), nameof(ProcessIncomingMessagePatch.ProcessIncomingMessage_Prefix));

			//TODO: Checking for ussed MessageTypes ids. Possible?
			private static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {

#if DEBUG
				if(msg.MessageType != 0) {
					ModEntry.ModLogger.Log($"MessageType: {msg.MessageType}");
				}
#endif
				// Incomming vanilla message MessageType
				//switch(msg.MessageType) {
				//	// "playerIntroduction" never reaches to the 'Multiplayer' class 
				//	//case 2:
				//	//	__instance.PlayerConnected(msg);
				//	//	break;

				//	//case 10:
				//	//	__instance.ReceivedChatMessage(msg);
				//	//	break;

				//	// "disconnecting" never reaches to the 'Multiplayer' class when forced to disconnect
				//	//case 19:
				//	//	__instance.PlayerDisconnected(msg);
				//	//	break;
				//}

				if(msg.MessageType == Message.TypeID && msg.Data.Length > 0) {

					String keyword = Message.Action.None.ToString();

					try {
						//Check that this isnt other mods message by trying to read a 'key'
						keyword = msg.Reader.ReadString();
					} catch(EndOfStreamException) {
						// Do nothing. If it does not contain the key, it may be anothers mod custom message or something went wrong
					}

					if(Enum.TryParse(keyword, out Message.Action action)) {
						if(Enum.IsDefined(typeof(Message.Action), action)) {
							switch(action) {
								case Message.Action.RequestEmojiTexture:
									__instance.ReceiveEmojiTextureRequest(msg);
									return false;
								case Message.Action.SendEmojiTexture:
									__instance.ReceiveEmojiTexture(msg);
									return false;
								case Message.Action.BroadcastEmojiTexture:
									__instance.ReceiveEmojiTextureBroadcast(msg);
									return false;
								case Message.Action.SendEmojisTextureDataList:
									__instance.ReceiveEmojisTextureDataList(msg);
									return false;
							}
						}
					}

				}

				// Allow to execute the vanilla method
				return true;
			}

		}

		// Player connected
		internal class AddPlayerPatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.addPlayer), new Type[] { typeof(NetFarmerRoot) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(AddPlayerPatch.AddPlayer_Postfix));

			private static void AddPlayer_Postfix(Multiplayer __instance, ref NetFarmerRoot f) {
				__instance.PlayerConnected(f.Value);
			}

		}

		// Player disconnected
		internal class PlayerDisconnectedPatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.playerDisconnected), new Type[] { typeof(long) });
			public override MethodInfo Prefix => AccessTools.Method(this.GetType(), nameof(PlayerDisconnectedPatch.PlayerDisconnected_Prefix));

			private static void PlayerDisconnected_Prefix(Multiplayer __instance, ref long id) {
				__instance.PlayerDisconnected(Game1.getFarmer(id));
			}

		}

		/*
		// Player connected
		internal class ReceivePlayerIntroductionPatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.receivePlayerIntroduction), new Type[] { typeof(BinaryReader) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ReceivePlayerIntroductionPatch.ReceivePlayerIntroduction_Postfix));

			private static void ReceivePlayerIntroduction_Postfix(Multiplayer __instance, ref BinaryReader reader) {
				__instance.PlayerConnected(__instance.readFarmer(reader).Value);
			}

		}
		*/

		internal class SendChatMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.sendChatMessage), new Type[] { typeof(LocalizedContentManager.LanguageCode), typeof(string) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(SendChatMessagePatch.SendChatMessage_Postfix));

			private static void SendChatMessage_Postfix(Multiplayer __instance, ref LocalizedContentManager.LanguageCode language, ref string message) {
				__instance.SendedChatMessage(Game1.player, language, message);
			}

		}

		internal class ReceiveChatMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.receiveChatMessage), new Type[] { typeof(Farmer), typeof(LocalizedContentManager.LanguageCode), typeof(string) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ReceiveChatMessagePatch.ReceiveChatMessage_Postfix));

			private static void ReceiveChatMessage_Postfix(Multiplayer __instance, ref Farmer sourceFarmer, ref LocalizedContentManager.LanguageCode language, ref string message) {
				__instance.ReceivedChatMessage(sourceFarmer, language, message);
			}

		}

	}

}
