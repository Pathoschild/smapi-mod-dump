
using MultiplayerEmotes.Framework.Constants;
using System.Linq;
using StardewValley;
using StardewValley.Network;
using System.Collections.Generic;
using StardewModdingAPI;

namespace MultiplayerEmotes.Extensions {

	public static class MultiplayerExtension {

		public static void BroadcastEmote(this Multiplayer multiplayer, int emoteIndex, Character character = null) {

			if(Game1.IsMultiplayer) {

				ModNetwork.MessageAction messageAction = ModNetwork.MessageAction.EmoteBroadcast;

				string characterId = "";
				if(character != null && !(character is Farmer)) {
					messageAction = ModNetwork.MessageAction.CharacterEmoteBroadcast;
					if(character is NPC npc) {
						characterId = npc.Name;
					} else if(character is FarmAnimal farmAnimal) {
						characterId = farmAnimal.myID.Value.ToString();
					}
				}

				object[] objArray = new object[3] {
					messageAction.ToString(),
					emoteIndex,
					characterId
				};

				OutgoingMessage message = new OutgoingMessage(ModNetwork.MessageTypeID, Game1.player, objArray);

				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
						}
					}

				}

			}

		}

		public static void ReceiveEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length >= 0) {
				int emoteIndex = msg.Reader.ReadInt32();
				msg.SourceFarmer.IsEmoting = false;
				msg.SourceFarmer.doEmote(emoteIndex);
			}
		}

		public static void ReceiveCharacterEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Context.IsPlayerFree && msg.Data.Length >= 0) {

				int emoteIndex = msg.Reader.ReadInt32();
				string characterId = msg.Reader.ReadString();

				Character sourceCharacter = null;

				if(int.TryParse(characterId, out int id)) {
					sourceCharacter = (Game1.currentLocation as AnimalHouse).animals.Values.FirstOrDefault(x => x.myID.Value == id);
				} else {
					sourceCharacter = Game1.getCharacterFromName(characterId);
				}
		
#if DEBUG
				ModEntry.ModMonitor.Log($"Received character emote broadcast. (Name: \"{sourceCharacter.Name}\", Emote: {emoteIndex})");
#endif

				if(sourceCharacter != null && !sourceCharacter.IsEmoting) {
					sourceCharacter.doEmote(emoteIndex, true);
				}

			}
		}

	}

}
