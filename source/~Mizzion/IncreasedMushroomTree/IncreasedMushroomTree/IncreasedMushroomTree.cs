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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace IncreasedMushroomTree
{
    public class IncreasedMushroomTree : Mod
    {
        public ModConfig Config;
        public GameLocation[] Locations;
        public bool ModEnabled;
        public int ShroomChance;
        public int MaxToShroom;
        public Random rnd = new Random();

        //Debugging
        public bool Debugging = true;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            ModEnabled = Config.ModEnabled;
            ShroomChance = Config.ShroomChance;
            MaxToShroom = Config.MaxRandomtoShroom;
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
            ControlEvents.KeyReleased += ControlEvents_KeyReleased;
            GraphicsEvents.OnPreRenderHudEvent += GraphicEvents_OnPreRenderHudEvent;
        }
        private void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
        {
            //Codes for my testing,,,,
            if (e.KeyPressed == Keys.Y)
            {
                if (!Context.IsWorldReady)
                    return;
                Farm farm = Game1.getFarm();
                int outty = rnd.Next(0, 100);
                int ToShroom = rnd.Next(1, this.MaxToShroom);
                //Make sure We can proceed.
                this.Monitor.Log($"Result: {outty} Change: {this.Config.ShroomChance}");
                if (outty > this.Config.ShroomChance)
                    return;
                if (Debugging)
                    Monitor.Log("Made it passed line 54", LogLevel.Alert);


                for (int i = 0; i < ToShroom; i++)
                {
                    //We go through each of the chances to create a shroom trr
                    if (farm.terrainFeatures.Count > 0)
                    {
                        if (Debugging)
                            Monitor.Log("Made it passed line 63", LogLevel.Alert);

                        TerrainFeature terrain = farm.terrainFeatures.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(rnd.Next(farm.terrainFeatures.Count)).Value;

                        if (Debugging)
                            Monitor.Log("Made it passed line 67", LogLevel.Alert);
                        if (terrain is Tree && (terrain as Tree).growthStage >= 5 && !(terrain as Tree).tapped)
                        {
                            if (Debugging)
                                Monitor.Log("Made it passed line 71", LogLevel.Alert);
                            (terrain as Tree).treeType.Value = 7;
                            (terrain as Tree).loadSprite();

                            if (Debugging)
                                Monitor.Log("Made it passed line 76", LogLevel.Alert);
                        }
                    }
                }
                /*
                foreach (var location in Game1.locations)
                {
                    if (location.isFarm && (location.name.Contains("FarmExpan")))
                    {

                    }
                }*/
            }
            if (e.KeyPressed == Keys.F5)
            {
                this.Config = Helper.ReadConfig<ModConfig>();
                this.ModEnabled = this.Config.ModEnabled;
                this.ShroomChance = this.Config.ShroomChance;
                this.MaxToShroom = this.Config.MaxRandomtoShroom;
                Monitor.Log($"Config reloaded", LogLevel.Info);
            }
            if (e.KeyPressed == Keys.NumPad8)
            {
                Farm farm = Game1.getFarm();
                //Go through and get rid of objects
                foreach (KeyValuePair<Vector2, SObject> pair in farm.objects)
                {
                    //farm.removeObject(pair.Key, false);
                    pair.Value.performRemoveAction(pair.Key, farm);
                }
                
            }
        }
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {

        }

        private void GraphicEvents_OnPreRenderHudEvent(object sender, EventArgs e)
        {

        }
        //
        private List<TerrainFeature> GetTerrain()
        {
            List<TerrainFeature> terrain = Game1.getFarm().terrainFeatures.Values.ToList();

            return terrain;
        }
        //Gather Locations
        public static IEnumerable<GameLocation> GetLocations()
        {

            foreach (GameLocation location in Game1.locations)
            {
                //Gets Current Location
                yield return location;
            }
        }
    }
}
