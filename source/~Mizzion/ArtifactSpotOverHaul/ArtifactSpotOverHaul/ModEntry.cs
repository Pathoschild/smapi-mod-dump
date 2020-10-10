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
using System.Reflection;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace ArtifactSpotOverHaul
{
    public class ModEntry : Mod
    {
        internal static ModEntry instance;
        public ModConfig config;
        //private LocationConfig lconfig;
        public List<int> needArtifacts;
        public bool debugging = false;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            this.config = Helper.ReadConfig<ModConfig>();
            //this.lconfig = this.config.lconfig;
            debugging = true;
            needArtifacts = new List<int>();
            //Events
            ControlEvents.KeyPressed += KeyPressed;
            GameEvents.HalfSecondTick += HalfSecondTick;
            //Set Up Harmony
            var harmony = HarmonyInstance.Create("mizzion.artifactspotoverhaul");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        //Keypressed Method
        private void KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.KeyPressed == Keys.F5)
            {
                this.config = this.Helper.ReadConfig<ModConfig>();
                this.Monitor.Log($"ModConfig has been reloaded.", LogLevel.Info);
            }
            if(e.KeyPressed == Keys.RightAlt)
            {
                this.Monitor.Log($"Artifact Luck: {artifactLuck()}", LogLevel.Warn);
            }
            if(e.KeyPressed == Keys.RightControl)
            {
                doArtifactSpots();
            }
            if(e.KeyPressed == Keys.RightShift)
            {
                foreach(int i in needArtifacts)
                {
                    this.Monitor.Log($"Needed Artifact: {i}.", LogLevel.Info);
                }
            }
        }
        //HalfSecondTick Method
        private void HalfSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            needArtifacts.Clear();
            foreach(KeyValuePair<int, string>objectInfo in Game1.objectInformation)
            {
                string[] oInfo = objectInfo.Value.Split('/');
                if (oInfo[3].Contains("Arch"))
                {
                    if (!Game1.player.archaeologyFound.ContainsKey(objectInfo.Key))
                    {
                        needArtifacts.Add(objectInfo.Key);
                    }
                }
            }
        }

        //Method to spawn artifact spots. This is used for testing.
        private void doArtifactSpots()
        {
            int count = 0;            
            if (!Context.IsWorldReady)
                return;
            foreach(GameLocation location in Game1.locations)
            {
               if (location.isOutdoors)
                {
                    for(int i = 0; i < 100; i++)
                    {
                        int x = Game1.random.Next(location.map.DisplayWidth / Game1.tileSize);
                        int y = Game1.random.Next(location.map.DisplayHeight / Game1.tileSize);
                        Vector2 vec = new Vector2((float)x, (float)y);
                        if(location.isTileLocationTotallyClearAndPlaceable(vec) && location.getTileIndexAt(x, y, "AlwaysFront") == -1 && location.getTileIndexAt(x, y, "Front") == -1 && !location.isBehindBush(vec) && (location.doesTileHaveProperty(x, y, "Diggable", "Back") != null || (Game1.currentSeason.Equals("winter") && location.doesTileHaveProperty(x, y, "Type", "Back") != null && location.doesTileHaveProperty(x, y, "Type", "Back").Equals("Grass"))) && (!location.name.Equals("Forest") || x < 93 || y > 22))
                        {
                            location.objects.Add(vec, new SObject(vec, 590, 1));
                            count++;
                        }
                    }
                }
            }
            Game1.showGlobalMessage($"Spawned {count} Artifact Spots.");
            count = 0;
        }

        //Method to add bonus to artifact luck
        public double artifactLuck()
        {
            int TotalArtifacts = 0;
            int multiplier = this.config.ArtifactMultiplier > 1 ? this.config.ArtifactMultiplier : 1000;
            foreach(KeyValuePair<int, int[]> artifacts in Game1.player.archaeologyFound)
            {
                if (artifacts.Value[0] > 1)
                    TotalArtifacts += artifacts.Value[0];
            }
            return (double)TotalArtifacts / multiplier;
        }
        //Method that process Map Odds
        public static int getDropId(GameLocation location, double chance)
        {
            int id = 0;
            string name = MapName(location);
            int inum;
            

            return id;
        }
        public static string MapName(GameLocation location)
        {
            return location.uniqueName ?? location.Name;
        }
    }
}