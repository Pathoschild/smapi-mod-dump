using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

class ModConfig
{
    public string Motd_text { get; set; } = "Message of the Day";
    public int Motd_delay { get; set; } = 2;
}

namespace MOTDMod
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        Dictionary<long, DateTime> newPlayerConnectTime =
            new Dictionary<long, DateTime>();

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            // chat message gets truncated at 411 characters in my testing
            if (this.Config.Motd_text.Length >= 411)
                this.Monitor.Log("WARNING: MOTD text exceeds 411 characters and may be truncated");
            helper.Events.Multiplayer.PeerContextReceived += this.OnPeerContextReceived;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        }

        private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            newPlayerConnectTime.Add(e.Peer.PlayerID, DateTime.Now);
            this.Monitor.Log($"New Player: {e.Peer.PlayerID}");
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            List<long> toRemove = new List<long>();

            foreach( KeyValuePair<long, DateTime> kvp in newPlayerConnectTime )
            {
                TimeSpan elapsed = (DateTime.Now - kvp.Value);
              
                if (elapsed.Seconds > this.Config.Motd_delay)
                {
                    // send MOTD to player
                    Game1.server?.sendMessage(kvp.Key, 
                        new OutgoingMessage(10, Game1.player, LocalizedContentManager.LanguageCode.en, 
                        this.Config.Motd_text));
                    // alternatively (see github.com/funny-snek/anticheat-and-servercode):
                    // Game1.server.sendMessage(kvp.Key, Multiplayer.chatMessage, Game1.player, 
                    //      this.Helper.Content.CurrentLocaleConstant, Motd_message);

                    // mark player for removal from the newPlayerConnectTime dictionary
                    toRemove.Add(kvp.Key);
                }
            }

            // remove all marked player entries from the dictionary
            foreach (long p in toRemove)
            {
                newPlayerConnectTime.Remove(p);
                this.Monitor.Log($"removed player {p.ToString()} from the list of new players");
            }
            // empty the toRemove list now that we're done with cleanup
            toRemove.Clear(); // may not be needed - the list may disappear at the end of the method anyway
        }

        // TODO: add method to update MOTD and send it to all online players

    }
}
