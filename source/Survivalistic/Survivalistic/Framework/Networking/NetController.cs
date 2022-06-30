/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using Survivalistic.Framework.Common;
using Survivalistic.Framework.Bars;
using Survivalistic.Framework.Databases;

namespace Survivalistic.Framework.Networking
{
    public class NetController
    {
        private static IModHelper Helper = ModEntry.instance.Helper;
        private static IManifest Manifest = ModEntry.instance.ModManifest;

        public static bool firstLoad;

        public static void SyncSpecificPlayer(long player_id)
        {
            if (Context.IsMainPlayer)
            {
                Data _data = Helper.Data.ReadSaveData<Data>($"{player_id}") ?? new Data();
                float[] _multipliers = { ModEntry.config.hunger_multiplier, ModEntry.config.thirst_multiplier };
                SyncBody _toSend = new SyncBody(_data, Foods.foodDatabase, _multipliers);

                Helper.Data.WriteSaveData($"{player_id}", _data);

                Debugger.Log($"Sending important data to farmhand {player_id}.", "Trace");
                Helper.Multiplayer.SendMessage(
                    message: _toSend,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { Manifest.UniqueID },
                    playerIDs: new[] { player_id }
                );
            }
        }

        public static void SyncAllPlayers()
        {
            if (Context.IsMainPlayer)
            {
                Debugger.Log($"Sending important data to all farmhands.", "Trace");
                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    Data _data = Helper.Data.ReadSaveData<Data>($"{farmer.UniqueMultiplayerID}") ?? new Data();
                    float[] _multipliers = { ModEntry.config.hunger_multiplier, ModEntry.config.thirst_multiplier };
                    SyncBody _toSend = new SyncBody(_data, Foods.foodDatabase , _multipliers);

                    Helper.Data.WriteSaveData($"{farmer.UniqueMultiplayerID}", _data);

                    Debugger.Log($"Sending important data to farmhand {farmer.UniqueMultiplayerID}.", "Trace");
                    Helper.Multiplayer.SendMessage(
                        message: _toSend,
                        messageType: "SaveDataFromHost",
                        modIDs: new[] { Manifest.UniqueID },
                        playerIDs: new[] { farmer.UniqueMultiplayerID }
                    );
                }
            }
        }

        public static void Sync()
        {
            if (Context.IsMainPlayer)
            {
                if (Game1.IsMultiplayer) Debugger.Log($"Saving host data.", "Trace");

                Data _data = Helper.Data.ReadSaveData<Data>($"{Game1.player.UniqueMultiplayerID}") ?? new Data();
                if (!firstLoad)
                {
                    ModEntry.data = _data;
                    firstLoad = true;
                }
                Helper.Data.WriteSaveData($"{Game1.player.UniqueMultiplayerID}", ModEntry.data);

                BarsUpdate.CalculatePercentage();
            }
            else
            {
                Debugger.Log($"Sending important data to host.", "Trace");

                Helper.Multiplayer.SendMessage(
                    message: ModEntry.data,
                    messageType: "SaveDataToHost",
                    modIDs: new[] { Manifest.UniqueID },
                    playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID }
                );
            }
        }

        public static void OnMessageReceived(ModMessageReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer && e.FromModID == Manifest.UniqueID && e.Type == "SaveDataFromHost")
            {
                SyncBody _body = e.ReadAs<SyncBody>();
                ModEntry.data = _body.data;
                Foods.foodDatabase = _body.dict;
                BarsDatabase.hunger_velocity = _body.multipliers[0];
                BarsDatabase.thirst_velocity = _body.multipliers[1];

                Debugger.Log("Received important data from host.", "Trace");
                BarsUpdate.CalculatePercentage();
            }

            if (Context.IsMainPlayer && e.FromModID == Manifest.UniqueID && e.Type == "SaveDataToHost")
            {
                Data _data = e.ReadAs<Data>();
                Debugger.Log($"Received important data from player {e.FromPlayerID}.", "Trace");
                Helper.Data.WriteSaveData($"{e.FromPlayerID}", _data);
            }
        }
    }

    public class SyncBody
    {
        public Data data;
        public Dictionary<string, string> dict;
        public float[] multipliers;

        public SyncBody(Data _data, Dictionary<string, string> _dict, float[] _multipliers)
        {
            data = _data;
            dict = _dict;
            multipliers = _multipliers;
        }
    }
}
