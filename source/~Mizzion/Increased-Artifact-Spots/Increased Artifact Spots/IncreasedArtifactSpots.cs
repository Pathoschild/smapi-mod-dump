/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace Increased_Artifact_Spots
{
    public class IncreasedArtifactSpots : Mod
    {
        //Number of actual spawned artifact spots
        private int SpawnedSpots;
        //Debug setting
        private bool debugging;
        //The Mods config
        private ModConfig Config;
        //Populate location names
        private List<Tuple<string,Vector2>> locations;
        public override void Entry(IModHelper helper)
        {
            //Initiate the config file
            Config = helper.ReadConfig<ModConfig>();
            //Set whether or not debugging is enabled
            debugging = false;
            //Set up new Console Command
            helper.ConsoleCommands.Add("artifacts", "Shows how many Artifact Spots were spawned per location..\n\nUsage: artifacts <value>\n- value: can be all, or a location name.", this.ShowSpots);
            //Events
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        public void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !debugging)
                return;
            if (e.Button == SButton.F6)
                SpawnSpots();
            if (e.Button == SButton.F5)
                this.Config = this.Helper.ReadConfig<ModConfig>();
        }

        public void DayStarted(object sender, DayStartedEventArgs e)
        {
            SpawnSpots();
        }

        private void SpawnSpots()
        {
            SpawnedSpots = 0;
            locations = new List<Tuple<string, Vector2>>();
            var i18n = Helper.Translation;
            if (Config.ShowSpawnedNumArtifactSpots)
                Game1.showGlobalMessage(i18n.Get("artifact_start"));
            foreach (GameLocation loc in Game1.locations)
            {
                
                if (loc.IsFarm || !loc.IsOutdoors)
                    continue;

                for (int i = 0; i < Config.AverageArtifactSpots; i++)
                {
                    int randomWidth = Game1.random.Next(loc.Map.DisplayWidth / Game1.tileSize);
                    int randomHeight = Game1.random.Next(loc.Map.DisplayHeight / Game1.tileSize);
                    Vector2 newLoc = new Vector2(randomWidth, randomHeight);
                    if (!loc.isTileLocationTotallyClearAndPlaceable(newLoc) ||
                        loc.getTileIndexAt(randomWidth, randomHeight, "AlwaysFront") != -1 ||
                        (loc.getTileIndexAt(randomWidth, randomHeight, "Front") != -1 ||
                         loc.isBehindBush(newLoc)) ||
                        (loc.doesTileHaveProperty(randomWidth, randomHeight, "Diggable", "Back") == null &&
                         (!Game1.currentSeason.Equals("winter") ||
                          loc.doesTileHaveProperty(randomWidth, randomHeight, "Type", "Back") == null ||
                          !loc.doesTileHaveProperty(randomWidth, randomHeight, "Type", "Back").Equals("Grass"))) ||
                        (loc.Name.Equals("Forest") && randomWidth >= 93 && randomHeight <= 22)) continue;
                    loc.objects.Add(newLoc, new Object(newLoc, 590, 1));
                    locations.Add(new Tuple<string, Vector2>(loc.Name, newLoc));
                    //locDictionary.Add(loc.Name, newLoc);
                    SpawnedSpots++;
                }
                if (debugging)
                    this.Monitor.Log($"Location Name: {loc.Name}, IsFarm: {loc.IsFarm}, IsOutDoors: {loc.IsOutdoors}.", LogLevel.Alert);
            }
            if (Config.ShowSpawnedNumArtifactSpots)
                Game1.showGlobalMessage(i18n.Get("artifact_spawned", new { artifact_spawns = SpawnedSpots }));
        }

        private void ShowSpots(string command, string[] args)
        {
            string arg = args[0];
            Dictionary<string, int> spawns = new Dictionary<string, int>();
            if (arg.ToLower() == "all")
            {
                foreach (var i in locations)
                {
                    if(!spawns.ContainsKey(i.Item1))
                        spawns.Add(i.Item1, locations.Count(x => x.Item1 == i.Item1));
                }
                if (spawns.Count != 0)
                {
                    foreach (var spawn in spawns)
                    {
                        this.Monitor.Log($"{spawn.Key}: {spawn.Value}", LogLevel.Info);
                    }
                }
                else
                {
                    this.Monitor.Log("The location was empty. Something may have gone wrong.", LogLevel.Info);
                }
            }
            else if (arg.ToLower() == "debug")
            {
                foreach (var i  in locations)
                {
                    this.Monitor.Log($"{i.Item1}: {i.Item2}", LogLevel.Info);
                }
            }
        }
    }
}
