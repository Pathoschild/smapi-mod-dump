using System;
using System.Collections.Generic;
using System.Linq;
using FunnySnek.AntiCheat.Server.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

namespace FunnySnek.AntiCheat.Server
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The name of the blacklist file on the server.</summary>
        private readonly string BlacklistFileName = "mod-blacklist.json";

        /// <summary>The number of seconds to wait until kicking a player (to make sure they receive the chat the message).</summary>
        private readonly int SecondsUntilKick = 5;

        /// <summary>The connected players.</summary>
        private readonly List<PlayerSlot> PlayersToKick = new List<PlayerSlot>();

        /// <summary>The mod names to prohibit indexed by mod ID.</summary>
        private readonly IDictionary<string, string> ProhibitedMods = new Dictionary<string, string>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // apply patches
            Patch.PatchAll(this.ModManifest.UniqueID);

            // hook events
            helper.Events.Multiplayer.PeerContextReceived += this.OnPeerContextReceived;
            helper.Events.Multiplayer.PeerDisconnected += this.OnPeerDisconnected;
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            // read mod blacklist
            var blacklist = this.Helper.Data.ReadJsonFile<Dictionary<string, string[]>>(this.BlacklistFileName);
            if (blacklist == null || !blacklist.Any())
            {
                this.Monitor.Log($"The {this.BlacklistFileName} file is missing or empty; please reinstall the mod.", LogLevel.Error);
                return;
            }
            foreach (var entry in blacklist)
            {
                foreach (string id in entry.Value)
                    this.ProhibitedMods[id.Trim()] = entry.Key;
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.PlayersToKick.Clear();
            if (Context.IsMainPlayer)
            {
                if (!this.ProhibitedMods.Any())
                    this.SendPublicChat($"Anti-Cheat's {this.BlacklistFileName} file is missing or empty; please reinstall the mod.", error: true);
                else
                    this.SendPublicChat("Anti-Cheat activated.");
            }
        }

        /// <summary>Raised after the mod context for a peer is received. This happens before the game approves the connection, so the player doesn't yet exist in the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            // log join
            this.Monitor.Log($"Player joined: {e.Peer.PlayerID}");

            // kick: blocked mods found
            if (e.Peer.HasSmapi)
            {
                string[] blockedModNames = this
                    .GetBlockedMods(e.Peer)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .OrderBy(p => p)
                    .ToArray();
                if (blockedModNames.Any())
                {
                    this.Monitor.Log($"   Will kick in {this.SecondsUntilKick} seconds: found prohibited mods {string.Join(", ", blockedModNames)}.");
                    this.PlayersToKick.Add(new PlayerSlot
                    {
                        Peer = e.Peer,
                        CountDownSeconds = this.SecondsUntilKick,
                        BlockedModNames = blockedModNames
                    });
                    return;
                }
            }

            // no issues found
            this.Monitor.Log("   No issues found.");
        }

        /// <summary>Raised after the connection with a peer is severed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            this.Monitor.Log($"Player quit: {e.Peer.PlayerID}");
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsMainPlayer || !Context.IsWorldReady || !e.IsOneSecond)
                return;

            // kick players whose countdowns expired
            foreach (PlayerSlot slot in this.PlayersToKick)
            {
                slot.CountDownSeconds--;
                if (slot.CountDownSeconds < 0)
                {
                    // get player info
                    long playerID = slot.Peer.PlayerID;
                    string name = Game1.getOnlineFarmers().FirstOrDefault(p => p.UniqueMultiplayerID == slot.Peer.PlayerID)?.Name ?? slot.Peer.PlayerID.ToString();

                    // send chat messages
                    int count = slot.BlockedModNames.Length;
                    this.SendPublicChat($"{name}: you're being kicked by Anti-Cheat. You have {(count == 1 ? "a blocked mod" : $"{count} blocked mods")} installed.", error: true);
                    this.SendDirectMessage(playerID, $"Please remove these mods: {string.Join(", ", slot.BlockedModNames)}.");

                    // kick player
                    this.KickPlayer(playerID);
                }
            }
            this.PlayersToKick.RemoveAll(p => p.CountDownSeconds < 0);
        }

        /// <summary>Get a list of blocked mod names the player has installed.</summary>
        /// <param name="peer">The peer whose mods to checks.</param>
        private IEnumerable<string> GetBlockedMods(IMultiplayerPeer peer)
        {
            foreach (var pair in this.ProhibitedMods)
            {
                if (peer.GetMod(pair.Key) != null)
                    yield return pair.Value;
            }
        }

        /// <summary>Send a chat message to all players.</summary>
        /// <param name="text">The chat text to send.</param>
        /// <param name="error">Whether to format the text as an error.</param>
        private void SendPublicChat(string text, bool error = false)
        {
            // format text
            if (error)
            {
                Game1.chatBox.activate();
                Game1.chatBox.setText("/color red");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
            }

            // send chat message
            // (Bypass Game1.chatBox.setText which doesn't handle long text well)
            Game1.chatBox.activate();
            Game1.chatBox.chatBox.reset();
            Game1.chatBox.chatBox.finalText.Add(new ChatSnippet(text, LocalizedContentManager.LanguageCode.en));
            Game1.chatBox.chatBox.updateWidth();
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }

        /// <summary>Send a private message to a specified player.</summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="text">The text to send.</param>
        private void SendDirectMessage(long playerID, string text)
        {
            Game1.server.sendMessage(playerID, Multiplayer.chatMessage, Game1.player, this.Helper.Content.CurrentLocaleConstant, text);
        }

        /// <summary>Kick a player from the server.</summary>
        /// <param name="playerID">The unique player ID.</param>
        private void KickPlayer(long playerID)
        {
            // kick player
            try
            {
                Game1.server.sendMessage(playerID, new OutgoingMessage(Multiplayer.disconnecting, playerID));
            }
            catch { /* ignore error if we can't connect to the player */ }
            Game1.server.playerDisconnected(playerID);
            Game1.otherFarmers.Remove(playerID);
        }
    }
}
