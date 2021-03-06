/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace LetMeRest
{
    class ModEntry : Mod
    {
        //DATA VARIABLES
        public Data data;
        public Dictionary<string, float> ItemDataBase;

        //CONFIG VARIABLES
        public ModConfig config;

        //CONTROL VARIABLES
        public int resetMovingTimer = 2;
        private int movingTimer;
        public static bool playingSound;

        //GENERAL VARIABLES
        public int radius = 6;

        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            ItemDataBase = DataBase.GetDataBase();

            helper.ConsoleCommands.Add("rest_change_multiplier", "Changes the stamina multiplier", cm_OnChangeMultiplier);

            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.onUpdate;
        }

        private void onUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            //Reset moving timer if using tool
            if (Game1.player.UsingTool)
            {
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }

            // Singleplayer Pause Check
            if (!Context.IsMultiplayer && !Context.IsPlayerFree && !Context.IsInDrawLoop) return;
            // Multiplayer Pause Check
            if (Context.IsMultiplayer && !Context.IsPlayerFree && !Context.IsInDrawLoop) return;

            // Check sitting
            if (Game1.player.IsSitting())
            {
                increaseStamina(1f);
            }
            // Check riding
            if (Game1.player.isRidingHorse())
            {
                increaseStamina(0.25f);
            }
            // Check movement
            if (!Game1.player.isMoving() &&
                !Game1.player.IsSitting() &&
                !Game1.player.isRidingHorse() &&
                !Game1.player.UsingTool)
            {
                if (movingTimer > 0) movingTimer--;
                else increaseStamina(0.25f);
            }
            else
            {
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }

        }

        private void increaseStamina(float value)
        {
            if (Game1.player.Stamina < Game1.player.MaxStamina)
            {
                // Convert PerSecond multiplier value to PerTick
                value /= 60;

                // Get multipliers
                float decorationMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[0];
                float waterMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[1];
                float paisageMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[2];

                // Increases stamina in multiplayer
                if (Context.IsMultiplayer)
                {
                    Game1.player.Stamina += (value * data.staminaMultiplier) * 
                        ((decorationMultiplier * 1.25f) * data.staminaMultiplier) * 
                        (waterMultiplier * data.staminaMultiplier) *
                        (paisageMultiplier * data.staminaMultiplier);
                }
                // Increases stamina in singleplayer
                else
                {
                    Game1.player.Stamina += (value * config.Multiplier) *
                        ((decorationMultiplier * 1.25f) * config.Multiplier) *
                        (waterMultiplier * config.Multiplier) *
                        (paisageMultiplier * config.Multiplier);
                }
            }
        }

        private void cm_OnChangeMultiplier(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.staminaMultiplier = config.Multiplier = float.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Stamina multiplier changed to: {data.staminaMultiplier}x", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.Multiplier = float.Parse(args[0]);
                this.Monitor.Log($"Stamina multiplier changed to: {config.Multiplier}x", LogLevel.Info);
            }
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Save host information
            if (Context.IsMainPlayer && Context.IsMultiplayer)
            {
                data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();
                data.staminaMultiplier = config.Multiplier;
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
            }
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            // Send data to a specific farmhand when connecting
            SendMessageToSpecificPlayer(e.Peer.PlayerID);
        }

        private void SendMessageToSpecificPlayer(long player_id)
        {
            // Send data to a specific farmhand
            if (Context.IsMainPlayer)
            {
                Data _data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                this.Monitor.Log($"Sending important data to farmhand {player_id}.", LogLevel.Trace);
                this.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { this.ModManifest.UniqueID },
                    playerIDs: new[] { player_id }
                );
            }
        }

        private void SendMessageToAllPlayers()
        {
            // Send data to all farmhands
            if (Context.IsMainPlayer)
            {
                Data _data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                this.Monitor.Log($"Sending important data to all farmhands.", LogLevel.Trace);
                this.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { this.ModManifest.UniqueID }
                );
            }
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // Receive data from host
            if (!Context.IsMainPlayer && e.FromModID == this.ModManifest.UniqueID && e.Type == "SaveDataFromHost")
            {
                data = e.ReadAs<Data>();
                this.Monitor.Log("Received important data from host.", LogLevel.Trace);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
            }
        }
    }
}
