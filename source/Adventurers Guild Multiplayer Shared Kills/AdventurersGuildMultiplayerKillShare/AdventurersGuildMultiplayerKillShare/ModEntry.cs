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
using System.Linq;
using System.Xml.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;

namespace AdventurersGuildMultiplayerKillShare
{
    public struct MobKilledData
    {
        public string mob_name;
        public long count;

        public MobKilledData(string mob_name, long count = 1)
        {
            this.mob_name = mob_name;
            this.count = count;
        }
    }

    public struct MobDictData
    {
        public SerializableDictionary<string, int> mob_data;

        public MobDictData(SerializableDictionary<string, int> mod_data)
        {
            this.mob_data = mod_data;
        }
    }

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        internal static ModEntry Instance;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            helper.ConsoleCommands.Add("print_player_kills", "Display all kills for all monsters", this.print_player_kills);
            helper.ConsoleCommands.Add("send_kill_message", "Send mob kill message to all players", this.send_kill_message);
            helper.ConsoleCommands.Add("send_sync_message", "Send sync mob kill message to player", this.send_sync_message);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            ObjectPatches.Initialize(this.Monitor, this.Helper.Multiplayer);

            // example patch, you'll need to edit this for your patch
            harmony.Patch(
               original: AccessTools.Method(typeof(Stats), nameof(Stats.monsterKilled)), // because it is private method
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.monsterKillede_Prefix))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            print_player_kills();
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            this.Monitor.Log($"Connected player with id {e.Peer.PlayerID}.", LogLevel.Debug);
            this.print_player_kills("", new string[1] { e.Peer.PlayerID.ToString() });
            MobDictData mobDictData = new MobDictData(Game1.stats.specificMonstersKilled);
            this.Helper.Multiplayer.SendMessage<MobDictData>(mobDictData, nameof(MobDictData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID});
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            //this.Monitor.Log($"msg type {e.Type} and self is {nameof(MobKilledData)} equals is {e.Type == nameof(MobKilledData)}.", LogLevel.Debug);
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == nameof(MobKilledData))
            {
                MobKilledData message = e.ReadAs<MobKilledData>();
                this.Monitor.Log($"Player with id {e.FromPlayerID} and name {Game1.getFarmer(e.FromPlayerID).Name} killed mob named {message.mob_name}.", LogLevel.Debug);
                ObjectPatches.is_sending_message = false;
                Game1.stats.monsterKilled(message.mob_name);
                ObjectPatches.is_sending_message = true;
            }
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == nameof(MobDictData))
            {
                MobDictData message = e.ReadAs<MobDictData>();
                this.Monitor.Log($"Player with id {e.FromPlayerID} send sync message.", LogLevel.Debug);
                ObjectPatches.is_sending_message = false;
                foreach (var item in message.mob_data)
                {
                    int already_killed;
                    Game1.stats.specificMonstersKilled.TryGetValue(item.Key, out already_killed);
                    for (int i = already_killed;i<item.Value; ++i)
                    {
                        Game1.stats.monsterKilled(item.Key);
                    }
                }
                this.Monitor.Log($"Synced kills with player with id {e.FromPlayerID}", LogLevel.Debug);
                ObjectPatches.is_sending_message = true;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void print_player_kills(string command = "", string[] arguments = null)
        {
            Farmer cur_farmer = null;
            if (arguments != null && arguments.Length > 0)
            {
                cur_farmer = Game1.getOnlineFarmers().ToList().Find(farmer => farmer.Name == arguments[0]);
            }
            if (cur_farmer is null)
            {
                cur_farmer = Game1.player;
            }

            if (cur_farmer.stats.specificMonstersKilled.Count > 0)
            {
                this.Monitor.Log(cur_farmer.Name + " kill list:", LogLevel.Debug);
                foreach (var item in cur_farmer.stats.specificMonstersKilled)
                    this.Monitor.Log(item.Key + "'s killed: " + item.Value, LogLevel.Debug);
            }
            else
            {
                this.Monitor.Log(cur_farmer.Name + " kill list is empty right now", LogLevel.Debug);
            }
        }

        private void send_kill_message(string command = "", string[] arguments = null)
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
                this.Monitor.Log($"Count is {count}", LogLevel.Debug);
                MobKilledData mobKilledData = new MobKilledData(arguments[0]);
                this.Helper.Multiplayer.SendMessage<MobKilledData>(mobKilledData, nameof(MobKilledData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }
            else
            {
                this.Monitor.Log("Mob name cant be empty", LogLevel.Debug);
            }
        }
        private void send_sync_message(string command = "", string[] arguments = null)
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
                }
                MobDictData mobDictData = new MobDictData(Game1.stats.specificMonstersKilled);
                this.Helper.Multiplayer.SendMessage<MobDictData>(mobDictData, nameof(MobDictData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID }, playerIDs: new[] { cur_farmer.UniqueMultiplayerID });
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
        public static bool is_sending_message = true;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor, IMultiplayerHelper multiplayerHelper)
        {
            Monitor = monitor;
            MultiplayerHelper = multiplayerHelper;
        }

        // patches need to be static!
        internal static bool monsterKillede_Prefix(Stats __instance, string name)
        {
            try
            {
                Monitor.Log($"Performing Killing of mob named {name}.", LogLevel.Debug);
                if (is_sending_message && Game1.IsMultiplayer)
                {
                    // send message to all players
                    MobKilledData mobKilledData = new MobKilledData(name);
                    MultiplayerHelper.SendMessage<MobKilledData>(mobKilledData, nameof(MobKilledData), modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
                    Monitor.Log($"Send message to all other players.", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(monsterKillede_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true; // always run original logic
        }
    }
}