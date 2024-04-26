/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using OrnithologistsGuild.Models;
using StardewValley;

namespace OrnithologistsGuild
{
    public class SaveDataManager
    {
        private const string KEY = "Ivy_OrnithologistsGuild_1";
        private const string KEY_LEGACY = "OrnithologistsGuild";

        public static SaveData SaveData { get; private set; }

        public static void Initialize()
        {
            ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            ModEntry.Instance.Helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
        }

        private static void Multiplayer_PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                SendSaveDataToPlayer(e.Peer.PlayerID);
            }
        }

        private static void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModEntry.Instance.ModManifest.UniqueID) return;

            if (Game1.IsMasterGame) // Host received message
            {
                if (e.Type == nameof(PlayerSaveDataToMain))
                {
                    // Farmhand sent save data to host
                    ModEntry.Instance.Monitor.Log($"Got {e.Type} from {e.FromPlayerID}");
                    SaveData.Players[e.FromPlayerID] = e.ReadAs<PlayerSaveDataToMain>().PlayerSaveData;
                    Save();
                }
            }
            else // Farmhand received message
            {
                if (e.Type == nameof(PlayerSaveDataFromMain))
                {
                    // Host sent save data to farmhand
                    ModEntry.Instance.Monitor.Log($"Got {e.Type} from {e.FromPlayerID}");
                    SaveData = new SaveData();
                    SaveData.Players[Game1.player.UniqueMultiplayerID] = e.ReadAs<PlayerSaveDataFromMain>().PlayerSaveData;
                }
            }
        }

        public static void Load()
        {
            if (Game1.IsMasterGame)
            {
                MigrateIfRequired();

                SaveData = ModEntry.Instance.Helper.Data.ReadSaveData<SaveData>(KEY) ?? new SaveData();
                ModEntry.Instance.Monitor.Log($"Loaded data for {SaveData.Players.Count} player(s)");
            }
        }

        public static void Save()
        {
            if (Game1.IsMasterGame)
            {
                ModEntry.Instance.Helper.Data.WriteSaveData(KEY, SaveData);
                ModEntry.Instance.Monitor.Log($"Saved data for {SaveData.Players.Count} player(s)");

                SendSaveDataToAllPlayers();
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Sending {nameof(PlayerSaveDataToMain)} to main");
                ModEntry.Instance.Helper.Multiplayer.SendMessage(
                    new PlayerSaveDataToMain(SaveData.ForPlayer(Game1.player.UniqueMultiplayerID)),
                    nameof(PlayerSaveDataToMain),
                    modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }
        }

        /// <summary>
        /// Sends <see cref="PlayerSaveData"/> to specified farmhand.
        /// </summary>
        private static void SendSaveDataToPlayer(long uniquePlayerId)
        {
            ModEntry.Instance.Monitor.Log($"Sending {nameof(PlayerSaveDataFromMain)} to {uniquePlayerId}");
            ModEntry.Instance.Helper.Multiplayer.SendMessage(
                new PlayerSaveDataFromMain(SaveData.ForPlayer(uniquePlayerId)),
                nameof(PlayerSaveDataFromMain),
                modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID },
                playerIDs: new[] { uniquePlayerId });
        }

        /// <summary>
        /// Sends <see cref="PlayerSaveData"/> to all online farmhands.
        /// </summary>
        private static void SendSaveDataToAllPlayers()
        {
            foreach (var player in Game1.getOnlineFarmers())
            {
                if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                {
                    SendSaveDataToPlayer(player.UniqueMultiplayerID);
                }
            }
        }

        /// <summary>
        /// Migrates <see cref="LegacySaveData"/> to <see cref="SaveData"/>.
        /// </summary>
        private static void MigrateIfRequired()
        {
            var legacySaveData = ModEntry.Instance.Helper.Data.ReadSaveData<LegacySaveData>(KEY_LEGACY);
            if (legacySaveData != null)
            {
                ModEntry.Instance.Monitor.Log($"Migrating {legacySaveData.LifeList.Count} Life List entries from legacy save data...");

                // Migration logic
                SaveData = new SaveData();
                SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList = legacySaveData.LifeList;

                Save();
                ModEntry.Instance.Helper.Data.WriteSaveData<LegacySaveData>(KEY_LEGACY, null);
            }
        }
    }
}
