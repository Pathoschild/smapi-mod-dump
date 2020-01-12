

using StardewModdingAPI;
using StardewValley;
using System;

using Steamworks;

namespace ShipmentTrackerSMAPI {

    public class MainModClass : Mod {
        private static MainModClass instance;

        public override void Entry(IModHelper helper) {
           instance = this;
            helper.Events.GameLoop.SaveLoaded += OnLoad;

            Log("RetroActive Achievement Loader by Iceburg333 => Initialized");
        }

        private void OnLoad(object sender, EventArgs e) {            
            Log("Player has " + Game1.player.achievements.Count + " achievements");
            Log("Steam API is running? " + SteamAPI.IsSteamRunning());

            foreach (int achievement in Game1.player.achievements)
            {
                string achievementName = Game1.achievements[achievement];
                Log("Player has achievement " + achievement + ": " + achievementName);
                Program.sdk.GetAchievement(string.Concat((object)achievement));
            }

            if (SteamAPI.IsSteamRunning()) { 
                Game1.playSound("achievement");
            }
        }

        public static void Log(string message) {
            instance.Monitor.Log(message, LogLevel.Debug);
        }
    }
}
