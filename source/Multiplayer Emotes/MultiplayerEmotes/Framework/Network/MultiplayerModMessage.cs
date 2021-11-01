/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System;
using System.Linq;
using MultiplayerEmotes.Extensions;
using MultiplayerEmotes.Framework.Patches;
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
      helper.Multiplayer.SendMessage(message, messageType.ToString(), new[] { helper.ModRegistry.ModID });
    }

    public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {

#if DEBUG
      ModEntry.ModMonitor.Log($"Received message.\n\tFromPlayer: \"{e.FromPlayerID}\"\n\tFromMod: \"{e.FromModID}\"\n\tType: \"{e.Type}\"", LogLevel.Trace);
#endif

      Type messageType = typeof(EmoteMessage);
      if (e.FromModID != helper.ModRegistry.ModID || e.Type != messageType.ToString()) {
        return;
      }

      EmoteMessage message = e.ReadAs<EmoteMessage>();

      switch (message.EmoteSourceType) {
        case CharacterType.Farmer:
          if (long.TryParse(message.EmoteSourceId, out long farmerId)) {
            Farmer farmer = Game1.getFarmer(farmerId);
            if (farmer != null) {
              FarmerPatch.DoEmotePatch.Instance.PostfixEnabled = false;
              farmer.PlayEmote(message.EmoteIndex);
              FarmerPatch.DoEmotePatch.Instance.PostfixEnabled = true;
            }
          }
          break;
        case CharacterType.NPC:
          NPC npc = Game1.getCharacterFromName(message.EmoteSourceId, mustBeVillager: false);
          if (npc != null && !npc.IsEmoting) {
            CharacterPatch.DoEmotePatch.Instance.PostfixEnabled = false;
            npc.PlayEmote(message.EmoteIndex);
            CharacterPatch.DoEmotePatch.Instance.PostfixEnabled = true;
          }
          break;
        case CharacterType.FarmAnimal:
          if (long.TryParse(message.EmoteSourceId, out long farmAnimalId)) {
            FarmAnimal farmAnimal = Game1.getFarm().getAllFarmAnimals().FirstOrDefault(x => x?.myID.Value == farmAnimalId);
            if (farmAnimal != null && !farmAnimal.IsEmoting) {
              CharacterPatch.DoEmotePatch.Instance.PostfixEnabled = false;
              farmAnimal.PlayEmote(message.EmoteIndex);
              CharacterPatch.DoEmotePatch.Instance.PostfixEnabled = true;
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
