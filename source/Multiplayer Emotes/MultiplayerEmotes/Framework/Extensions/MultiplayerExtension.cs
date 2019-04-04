
using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerEmotes.Framework.Network;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;

namespace MultiplayerEmotes.Extensions {

	public static class MultiplayerExtension {

		public static void BroadcastEmote(this Multiplayer multiplayer, int emoteIndex, Character character = null) {

			if(Game1.IsMultiplayer) {

				EmoteMessage message = new EmoteMessage {
					EmoteIndex = emoteIndex
				};

				if(character is Farmer farmer) {
					message.EmoteSourceId = farmer.UniqueMultiplayerID.ToString();
					message.EmoteSourceType = CharacterType.Farmer;
				} else if(character is NPC npc) {
					message.EmoteSourceId = npc.Name;
					message.EmoteSourceType = CharacterType.NPC;
				} else if(character is FarmAnimal farmAnimal) {
					message.EmoteSourceId = farmAnimal.myID.Value.ToString();
					message.EmoteSourceType = CharacterType.FarmAnimal;
				}

				ModEntry.MultiplayerMessage.Send(message);

			}

		}

#if DEBUG
		public static void TestFunc(string name, bool mustBeVillager = false) {

			if(Game1.currentLocation != null) {
				ModEntry.ModMonitor.Log($"- Loop1");
				foreach(NPC character in (IEnumerable<NPC>)Game1.currentLocation.getCharacters()) {
					ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
					if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
						ModEntry.ModMonitor.Log($"### Found Character: {character}");
				}
			}
			ModEntry.ModMonitor.Log($"- Loop2");
			for(int index = 0; index < Game1.locations.Count; ++index) {
				foreach(NPC character in (IEnumerable<NPC>)Game1.locations[index].getCharacters()) {
					ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
					if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
						ModEntry.ModMonitor.Log($"### Found Character: {character.Name}");
				}
			}
			if(Game1.getFarm() != null) {
				ModEntry.ModMonitor.Log($"- Loop3");
				foreach(Building building in Game1.getFarm().buildings) {
					if(building.indoors.Value != null) {
						foreach(NPC character in building.indoors.Value.characters) {
							ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
							if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
								ModEntry.ModMonitor.Log($"### Found Character: {character.Name}");
						}
					}
				}
			}
		}
#endif

		[Obsolete("This method removal is planned. Is not longer in use.", true)]
		public static void ReceiveEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length > 0) {
				int emoteIndex = msg.Reader.ReadInt32();
				msg.SourceFarmer.IsEmoting = false;
				msg.SourceFarmer.doEmote(emoteIndex);

#if DEBUG
				ModEntry.ModMonitor.Log($"Received player emote broadcast. (Name: \"{msg.SourceFarmer.Name}\", Emote: {emoteIndex})");
#endif

			}
		}

		[Obsolete("This method removal is planned. Is not longer in use.", true)]
		public static void ReceiveCharacterEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Context.IsPlayerFree && msg.Data.Length > 0) {

				int emoteIndex = msg.Reader.ReadInt32();
				string characterId = msg.Reader.ReadString();

				Character sourceCharacter = null;

				if(long.TryParse(characterId, out long id)) {
					sourceCharacter = (Game1.currentLocation as AnimalHouse).animals.Values.FirstOrDefault(x => x.myID.Value == id);
				} else {
					sourceCharacter = Game1.getCharacterFromName(characterId);
				}

#if DEBUG
				TestFunc(characterId);
				ModEntry.ModMonitor.Log($"Received character emote broadcast. (Name: \"{sourceCharacter.Name}\", Emote: {emoteIndex})");
#endif

				if(sourceCharacter != null && !sourceCharacter.IsEmoting) {
					sourceCharacter.doEmote(emoteIndex, true);
				}

			}
		}

	}

}
