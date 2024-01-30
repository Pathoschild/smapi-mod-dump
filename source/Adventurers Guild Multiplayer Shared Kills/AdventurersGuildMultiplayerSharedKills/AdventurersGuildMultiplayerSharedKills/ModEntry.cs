/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Veniamin-Arefev/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;

namespace AdventurersGuildMultiplayerSharedKills
{
    public struct MobKilledData
    {
        public string mobName;
        public long count;

        public MobKilledData(string mob_name, long count = 1)
        {
            this.mobName = mob_name;
            this.count = count;
        }
    }

    public struct MobSyncData
    {
        public SerializableDictionary<string, int> mobData;
        public bool isReplyable;

        public MobSyncData(SerializableDictionary<string, int> mobData, bool isReplyable = false)
        {
            this.mobData = mobData;
            this.isReplyable = isReplyable;
        }
    }

    internal sealed class ModEntry : Mod
    {
        internal static ModEntry Instance;
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            helper.ConsoleCommands.Add("add_kill", "Perform kill of specific monster", this.AddKill);
            helper.ConsoleCommands.Add("print_player_kills", "Display all kills for all monsters", this.PrintPlayerKills);
            helper.ConsoleCommands.Add("send_kill_message", "Send mob kill message to all players", this.SendKillMessage);
            helper.ConsoleCommands.Add("send_sync_message", "Send sync mob kill message to player", this.SendSyncMessage);

            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            ObjectPatches.Initialize(this.Monitor, this.Helper.Multiplayer);

            harmony.Patch(
               original: AccessTools.Method(typeof(Stats), nameof(Stats.monsterKilled)), // because it is private method
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.monsterKilled_Prefix))
            );
        }


        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            this.Monitor.Log($"Connected player {GetPrettyPlayerName(e.Peer.PlayerID)}.", LogLevel.Debug);
            this.PrintPlayerKills("", new string[1] { GetPlayerName(e.Peer.PlayerID) });
            var mobDictData = new MobSyncData(Game1.stats.specificMonstersKilled, true);
            this.Helper.Multiplayer.SendMessage<MobSyncData>(mobDictData, nameof(MobSyncData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            //this.Monitor.Log($"msg type {e.Type} and self is {nameof(MobKilledData)} equals is {e.Type == nameof(MobKilledData)}.", LogLevel.Debug);
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == nameof(MobKilledData))
            {
                MobKilledData message = e.ReadAs<MobKilledData>();
                this.Monitor.Log($"Player {GetPrettyPlayerName(e.FromPlayerID)} killed mob named \"{message.mobName}\".", LogLevel.Debug);
                ObjectPatches.isSendingMessage = false;
                Game1.stats.monsterKilled(message.mobName);
                ObjectPatches.isSendingMessage = true;
            }
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == nameof(MobSyncData))
            {
                MobSyncData message = e.ReadAs<MobSyncData>();
                this.Monitor.Log($"Received sync message from player {GetPrettyPlayerName(e.FromPlayerID)}.", LogLevel.Debug);
                ObjectPatches.isSendingMessage = false;
                bool isDetectedNotRegisteredKills = false; // for host all new kills is detected, for farmhands not counted on host
                if (!Game1.IsMasterGame && Game1.stats.specificMonstersKilled.Any(item => !message.mobData.ContainsKey(item.Key))) // check farmhand kill list contain not regitered entities
                {
                    isDetectedNotRegisteredKills = true;
                }
                foreach (var item in message.mobData)
                {
                    int already_killed;
                    Game1.stats.specificMonstersKilled.TryGetValue(item.Key, out already_killed);
                    if (!Game1.IsMasterGame && already_killed > item.Value) { isDetectedNotRegisteredKills = true; } // detected farmhand kill not counted on host
                    for (int i = already_killed; i < item.Value; ++i)
                    {
                        if (Game1.IsMasterGame) { isDetectedNotRegisteredKills = true; } // detected new kill for host
                        Game1.stats.monsterKilled(item.Key);
                    }
                }
                this.Monitor.Log($"Synced kills with {GetPrettyPlayerName(e.FromPlayerID)}", LogLevel.Debug);
                if (isDetectedNotRegisteredKills)
                {
                    if (Game1.IsMasterGame)
                    {
                        this.Monitor.Log($"Received not registered kills, send sync message to all players", LogLevel.Debug);
                        this.Helper.Multiplayer.SendMessage<MobSyncData>(
                            message: new MobSyncData(Game1.stats.specificMonstersKilled, false),
                            nameof(MobSyncData),
                            modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID },
                            playerIDs: Game1.getAllFarmhands().Where(player => player.UniqueMultiplayerID != e.FromPlayerID).Select(player => player.UniqueMultiplayerID).ToArray()
                        );
                    }
                    else if (message.isReplyable) // farmhand part reply to host
                    {
                        this.Monitor.Log($"Detected not registered kills on host, reply with sync message to host", LogLevel.Debug);
                        this.Helper.Multiplayer.SendMessage<MobSyncData>(
                            message: new MobSyncData(Game1.stats.specificMonstersKilled, false),
                            nameof(MobSyncData),
                            modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID },
                            playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID }
                        );
                    }
                }
                ObjectPatches.isSendingMessage = true;
            }
        }

        private static string GetPlayerName(long uniqueMultiplayerID)
        {
            return Game1.getFarmer(uniqueMultiplayerID).Name;
        }

        private static string GetPrettyPlayerName(long uniqueMultiplayerID)
        {
            return $"{GetPlayerName(uniqueMultiplayerID)}({uniqueMultiplayerID})";
        }

        private void AddKill(string command = "", string[] arguments = null)
        {
            if (arguments != null && arguments.Length > 0)
            {
                if (Game1.stats.specificMonstersKilled.ContainsKey(arguments[0]))
                {
                    Game1.stats.specificMonstersKilled[arguments[0]]++;
                }
                else
                {
                    Game1.stats.specificMonstersKilled.Add(arguments[0], 1);
                }
            }
            else
            {
                this.Monitor.Log("Mob name cant be empty", LogLevel.Debug);
            }
        }

        private void PrintPlayerKills(string command = "", string[] arguments = null)
        {
            Farmer cur_farmer = Game1.player;
            if (arguments != null && arguments.Length > 0)
            {
                cur_farmer = Game1.getOnlineFarmers().ToList().Find(farmer => farmer.Name == arguments[0]);
                if (cur_farmer == null)
                {
                    this.Monitor.Log("Player not found", LogLevel.Debug);
                    return;
                }
            }

            if (cur_farmer.stats.specificMonstersKilled.Count > 0)
            {
                this.Monitor.Log($"{GetPrettyPlayerName(cur_farmer.UniqueMultiplayerID)} kill list:", LogLevel.Debug);
                foreach (var item in cur_farmer.stats.specificMonstersKilled)
                    this.Monitor.Log($"\t{item.Key}'s killed: {item.Value}", LogLevel.Debug);
            }
            else
            {
                this.Monitor.Log($"{GetPrettyPlayerName(cur_farmer.UniqueMultiplayerID)}'s kill list is empty right now", LogLevel.Debug);
            }
        }

        private void SendKillMessage(string command = "", string[] arguments = null)
        {
            if (!Game1.IsMultiplayer)
            {
                this.Monitor.Log("Game is in single player mode right now", LogLevel.Debug);
                return;
            }
            if (arguments != null && arguments.Length > 0)
            {
                long count = 1;
                if (arguments.Length > 1)
                    long.TryParse(arguments[1], out count);
                count = count > 1 ? count : 1;
                this.Helper.Multiplayer.SendMessage<MobKilledData>(new MobKilledData(arguments[0]), nameof(MobKilledData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }
            else
            {
                this.Monitor.Log("Mob name cant be empty", LogLevel.Debug);
            }
        }
        private void SendSyncMessage(string command = "", string[] arguments = null)
        {
            if (!Game1.IsMultiplayer)
            {
                this.Monitor.Log("Game is in single player mode right now", LogLevel.Debug);
                return;
            }
            if (arguments != null && arguments.Length > 0)
            {
                Farmer cur_farmer = Game1.getOnlineFarmers().ToList().Find(farmer => farmer.Name == arguments[0]);
                if (cur_farmer is null)
                {
                    this.Monitor.Log("Player not found", LogLevel.Debug);
                    return;
                }
                MobSyncData mobDictData = new MobSyncData(Game1.stats.specificMonstersKilled, false);
                this.Helper.Multiplayer.SendMessage<MobSyncData>(mobDictData, nameof(MobSyncData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID }, playerIDs: new[] { cur_farmer.UniqueMultiplayerID });
            }
            else
            {
                this.Monitor.Log("Player name cant be empty", LogLevel.Debug);
            }
        }
    }

    internal class ObjectPatches
    {
        private static IMonitor Monitor;
        private static IMultiplayerHelper MultiplayerHelper;
        public static bool isSendingMessage = true;

        internal static void Initialize(IMonitor monitor, IMultiplayerHelper multiplayerHelper)
        {
            Monitor = monitor;
            MultiplayerHelper = multiplayerHelper;
        }

        internal static bool monsterKilled_Prefix(Stats __instance, string name)
        {
            try
            {
                //Monitor.Log($"Performing Killing of mob named {name}.", LogLevel.Debug);
                if (isSendingMessage && Game1.IsMultiplayer)
                {
                    // send message to all players
                    MobKilledData mobKilledData = new MobKilledData(name);
                    MultiplayerHelper.SendMessage<MobKilledData>(mobKilledData, nameof(MobKilledData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
                    Monitor.Log($"Send message to all other players.", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(monsterKilled_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true; // always run original logic
        }
    }
}