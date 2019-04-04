using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerEmotes.Framework.Network {

	public class MultiplayerModMessage {

		private readonly IModHelper helper;

		public MultiplayerModMessage(IModHelper helper) {
			this.helper = helper;
			SubscribeEvents();
		}

		public void SubscribeEvents() {
			this.helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
		}

		public void UnsubscribeEvents() {
			this.helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
		}

		public void Send(EmoteMessage message) {

			Type messageType = typeof(EmoteMessage);

#if DEBUG
			ModEntry.ModMonitor.Log($"Sending message.\n\tFromPlayer: \"{Game1.player.UniqueMultiplayerID}\"\n\tFromMod: \"{helper.ModRegistry.ModID}\"\n\tType: \"{messageType}\"", LogLevel.Trace);
#endif
			helper.Multiplayer.SendMessage(message, messageType.ToString(), modIDs: new[] { helper.ModRegistry.ModID });

		}

		public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {

#if DEBUG
			ModEntry.ModMonitor.Log($"Received message.\n\tFromPlayer: \"{e.FromPlayerID}\"\n\tFromMod: \"{e.FromModID}\"\n\tType: \"{e.Type}\"", LogLevel.Trace);
#endif
			Type messageType = typeof(EmoteMessage);
			if(e.FromModID == helper.ModRegistry.ModID && e.Type == messageType.ToString()) {

				EmoteMessage message = e.ReadAs<EmoteMessage>();

				switch(message.EmoteSourceType) {
					case CharacterType.Farmer:
						if(long.TryParse(message.EmoteSourceId, out long farmerId)) {
							Farmer farmer = Game1.getFarmer(farmerId);
							farmer.doEmote(message.EmoteIndex);
						}
						break;
					case CharacterType.NPC:
						NPC npc = Game1.getCharacterFromName(message.EmoteSourceId);
						if(npc != null && !npc.IsEmoting) {
							npc.doEmote(message.EmoteIndex);
						}
						break;
					case CharacterType.FarmAnimal:
						if(long.TryParse(message.EmoteSourceId, out long farmAnimalId)) {
							FarmAnimal farmAnimal = Game1.getFarm().getAllFarmAnimals().FirstOrDefault(x => x?.myID.Value == farmAnimalId);
							if(farmAnimal != null && !farmAnimal.IsEmoting) {
								farmAnimal.doEmote(message.EmoteIndex);
							}
						}
						break;
					case CharacterType.Unknown:
					default:
						break;
				}

			}

		}

	}

}
