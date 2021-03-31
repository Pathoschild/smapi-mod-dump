/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LetMeRest.Framework.Multiplayer
{
    public class NetController
    {
        public static void OnSaveLoaded()
        {
            // Save host information
            if (Context.IsMainPlayer && Context.IsMultiplayer)
            {
                ModEntry.data = ModEntry.instance.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();
                ModEntry.data.Multiplier = ModEntry.config.Multiplier;
                ModEntry.data.SittingVerification = ModEntry.config.SittingVerification;
                ModEntry.data.RidingVerification = ModEntry.config.RidingVerification;
                ModEntry.data.StandingVerification = ModEntry.config.StandingVerification;
                ModEntry.data.EnableSecrets = ModEntry.config.EnableSecrets;
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
            }
        }

        public static void SyncSpecificPlayer(long player_id)
        {
            // Send data to a specific farmhand
            if (Context.IsMainPlayer)
            {
                Data _data = ModEntry.instance.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                ModEntry.instance.Monitor.Log($"Sending important data to farmhand {player_id}.", LogLevel.Trace);
                ModEntry.instance.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { ModEntry.instance.ModManifest.UniqueID },
                    playerIDs: new[] { player_id }
                );
            }
        }

        public static void SyncAllPlayers()
        {
            // Send data to all farmhands
            if (Context.IsMainPlayer)
            {
                Data _data = ModEntry.instance.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                ModEntry.instance.Monitor.Log($"Sending important data to all farmhands.", LogLevel.Trace);
                ModEntry.instance.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { ModEntry.instance.ModManifest.UniqueID }
                );
            }
        }

        public static void OnMessageReceived(ModMessageReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer && e.FromModID == ModEntry.instance.ModManifest.UniqueID && e.Type == "SaveDataFromHost")
            {
                ModEntry.data = e.ReadAs<Data>();
                ModEntry.instance.Monitor.Log("Received important data from host.", LogLevel.Trace);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
            }
        }
    }
}
