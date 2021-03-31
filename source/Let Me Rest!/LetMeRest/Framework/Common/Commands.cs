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
using StardewValley;
using LetMeRest.Framework.Multiplayer;

namespace LetMeRest.Framework.Common
{
    public class Commands
    {
        public static void cm_OnChangeMultiplier(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data.Multiplier = ModEntry.config.Multiplier = float.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Stamina multiplier changed to: {ModEntry.data.Multiplier}x", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.Multiplier = float.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Stamina multiplier changed to: {ModEntry.config.Multiplier}x", LogLevel.Info);
            }
        }

        public static void cm_OnChangeSitting(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data.SittingVerification = ModEntry.config.SittingVerification = bool.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Sitting verification changed to: {args[0]}", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.SittingVerification = bool.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Sitting verification changed to: {args[0]}", LogLevel.Info);
            }
        }

        public static void cm_OnChangeRiding(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                if (!Context.IsWorldReady) return;
                ModEntry.data.RidingVerification = ModEntry.config.RidingVerification = bool.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Riding verification changed to: {args[0]}", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.RidingVerification = bool.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Riding verification changed to: {args[0]}", LogLevel.Info);
            }
        }

        public static void cm_OnChangeStanding(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data.StandingVerification = ModEntry.config.StandingVerification = bool.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Standing verification changed to: {args[0]}", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.StandingVerification = bool.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Standing verification changed to: {args[0]}", LogLevel.Info);
            }
        }

        public static void cm_OnChangeSecrets(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data.EnableSecrets = ModEntry.config.EnableSecrets = bool.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Secrets verification changed to: {args[0]}", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.EnableSecrets = bool.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Secrets verification changed to: {args[0]}", LogLevel.Info);
            }
        }

        public static void cm_OnChangeBuffs(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data.EnableBuffs = ModEntry.config.EnableSecrets = bool.Parse(args[0]);
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);
                NetController.SyncAllPlayers();
                ModEntry.instance.Monitor.Log($"Buff status changed to: {args[0]}", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config.EnableBuffs = bool.Parse(args[0]);
                ModEntry.instance.Monitor.Log($"Buff status changed to: {args[0]}", LogLevel.Info);
            }
        }

        public static void cm_ResetData(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                ModEntry.data = new Data();
                ModEntry.instance.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", ModEntry.data);

                ModEntry.instance.Monitor.Log($"Reseting all farmhands data", LogLevel.Trace);
                ModEntry.instance.Helper.Multiplayer.SendMessage(
                    message: ModEntry.data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { ModEntry.instance.ModManifest.UniqueID }
                );
                ModEntry.instance.Monitor.Log($"Data reseted", LogLevel.Info);
            }
            if (!Context.IsMultiplayer)
            {
                ModEntry.config = new ModConfig();
                ModEntry.instance.Monitor.Log($"Config.json reseted", LogLevel.Info);
            }
        }
    }
}
