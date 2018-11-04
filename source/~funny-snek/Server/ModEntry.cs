using System;
using System.Collections.Generic;
using System.Linq;
using FunnySnek.AntiCheat.Server.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace FunnySnek.AntiCheat.Server
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The connected players.</summary>
        private readonly IDictionary<long, PlayerSlot> PlayerSlots = new Dictionary<long, PlayerSlot>();

        /// <summary>The number of seconds to wait for a ping from a player's local mod before kicking them.</summary>
        private readonly int SecondsUntilKick = 60;

        /// <summary>The current passcode appended to player ID messages.</summary>
        private readonly string CurrentPassCode = "SCAZ";

        /// <summary>Whether we sent a chat message indicating that anti-cheat is enabled.</summary>
        private bool AntiCheatMessageSent;


        /*********
        ** Accessors
        *********/
        /// <summary>The chat messages received from other players.</summary>
        public static List<string> MessagesReceived { get; } = new List<string>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Patch.PatchAll("anticheatviachat.anticheatviachat");
            GameEvents.OneSecondTick += this.OnOneSecondTick;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>An event handler called once per second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnOneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // send 'anti-cheat activated' chat message
            if (!this.AntiCheatMessageSent)
            {
                this.SendChatMessage("Anti-Cheat activated");
                this.AntiCheatMessageSent = true;
            }

            // get connected player IDs
            HashSet<long> connectedIDs = new HashSet<long>(
                Game1.getOnlineFarmers().Select(p => p.UniqueMultiplayerID).Except(new [] { Game1.player.UniqueMultiplayerID })
            );

            // add new players
            foreach (long playerID in connectedIDs)
            {
                if (!this.PlayerSlots.ContainsKey(playerID))
                {
                    this.PlayerSlots[playerID] = new PlayerSlot
                    {
                        PlayerID = playerID,
                        IsCountingDown = true,
                        CountDownSeconds = SecondsUntilKick
                    };
                    this.Monitor.Log($"Player joined: {playerID}");
                }
            }

            // remove disconnected players
            foreach (long playerID in this.PlayerSlots.Keys.ToArray())
            {
                if (!connectedIDs.Contains(playerID))
                {
                    this.PlayerSlots.Remove(playerID);
                    this.Monitor.Log($"Player quit: {playerID}");
                }
            }

            // handle received local mod pings
            foreach (string message in ModEntry.MessagesReceived)
            {
                if (message.StartsWith(this.CurrentPassCode))
                {
                    // parse received ID
                    if (!long.TryParse(message.Substring(this.CurrentPassCode.Length), out long playerID))
                    {
                        this.Monitor.Log($"Received invalid player ID message: {message}", LogLevel.Warn);
                        continue;
                    }

                    // get player slot
                    if (!this.PlayerSlots.TryGetValue(playerID, out PlayerSlot slot))
                    {
                        this.Monitor.Log($"Received unknown player ID: {playerID}", LogLevel.Warn);
                        continue;
                    }

                    // disable countdown for player
                    this.Monitor.Log($"Player approved: {playerID}");
                    slot.IsCountingDown = false;
                }
            }
            ModEntry.MessagesReceived.Clear();

            // kick players whose countdowns expired
            foreach (long playerID in this.PlayerSlots.Keys.ToArray())
            {
                PlayerSlot slot = this.PlayerSlots[playerID];
                if (slot.IsCountingDown)
                {
                    slot.CountDownSeconds--;
                    if (slot.CountDownSeconds <= 0)
                    {
                        this.Monitor.Log($"Kicking player {playerID}, no code received.");

                        this.SendChatMessage("/color red");
                        this.SendChatMessage("You are being kicked by Anti-Cheat.");
                        this.SendChatMessage("Please install the latest Anti-Cheat client mod.");

                        try
                        {
                            Game1.server.sendMessage(playerID, new OutgoingMessage(Multiplayer.disconnecting, playerID));
                        }
                        catch { /* ignore error if we can't connect to the player */ }
                        Game1.server.playerDisconnected(playerID);
                        Game1.otherFarmers.Remove(playerID);
                        this.PlayerSlots.Remove(playerID);
                    }
                }
            }
        }

        /// <summary>Send a chat message to all players.</summary>
        /// <param name="text">The chat text to send.</param>
        private void SendChatMessage(string text)
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText(text);
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }
    }
}
