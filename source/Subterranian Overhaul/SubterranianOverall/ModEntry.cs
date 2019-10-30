using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SubterranianOverhaul
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        //remember to set all of this into a config file.
        private const double STONE_BASE_CHANCE_TO_CHANGE = 0.0010; //0.1% chance at level 90.
        private const double STONE_INCREASE_PER_LEVEL = 0.002; //results in a maximum chance at level 119 of 5.9% for each stone to change.
        private const double PURPLE_MUSHROOM_CHANCE_TO_CHANGE = 0.05; //5% chance of each purple mushroom changing.
        private const double PURPLE_MUSHROOM_INCREASE_PER_LEVEL = 0.005; //results in a maximum chance at level 119 of 19.5% for each purple mushroom to change.
        private const double RED_MUSHROOM_CHANCE_TO_CHANGE = 0.20; //10% chance of each red mushroom changing.
        private const double RED_MUSHROOM_INCREASE_PER_LEVEL = 0.010; //results in a maximum chance at level 119 of 59% for each red mushroom to change.

        private static ModEntry mod;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.mod = this;
            helper.Events.Player.Warped += this.OnPlayerWarped;
        }

        public static IModHelper GetHelper()
        {
            return ModEntry.mod?.Helper;
        }


        /*********
        ** Private methods
        *********/

        private void LoadTextures(IModHelper helper)
        {   
            helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_spore.png"), ContentSource.ModFolder);
            helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_spore.png"), ContentSource.ModFolder);
        }
        /// <summary>
        /// Processes checks when the player warps so we can try to override some forage spawns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            this.Monitor.Log($"{Game1.player.Name} warped from {e.OldLocation.Name} to {e.NewLocation.Name}.");
            if (e.NewLocation.Name.StartsWith("UndergroundMine")) {
                GameLocation loc = e.NewLocation;
                String floorText = loc.Name.Substring(15);
                int floorNumber = 0;
                if (int.TryParse(floorText, out floorNumber))
                {
                    int floorsAbove90 = floorNumber - 90;
                    double extraStoneChance = floorsAbove90 * STONE_INCREASE_PER_LEVEL;
                    double stoneChanceToChange = STONE_BASE_CHANCE_TO_CHANGE + extraStoneChance;
                    double extraPurpleMushroomChance = floorsAbove90 * PURPLE_MUSHROOM_INCREASE_PER_LEVEL;
                    double purpleMushroomChanceToChange = PURPLE_MUSHROOM_CHANCE_TO_CHANGE + extraPurpleMushroomChance;
                    double extraRedMushroomChance = floorsAbove90 * RED_MUSHROOM_INCREASE_PER_LEVEL;
                    double redMushroomChanceToChange = RED_MUSHROOM_CHANCE_TO_CHANGE + extraRedMushroomChance;
                    //int level = 0;
                    //int.TryParse(loc.Name.Substring(15), out level);
                    //this.Monitor.Log($"I'm in the mines on level " +level);
                    List<Vector2> toReplace = new List<Vector2>();

                    //only proceed if there is a chance of something actually changing into a mushroom tree. For red mushrooms this chance starts at level 70, for purple mushrooms it starts at 80 and for rocks it starts at 90
                    if(stoneChanceToChange >= 0 || purpleMushroomChanceToChange >= 0 || redMushroomChanceToChange >= 0)
                    {
                        foreach (StardewValley.Object o in loc.Objects.Values)
                        {
                            double hit = Game1.random.NextDouble();
                            if (o.Name == "Stone" && hit <= stoneChanceToChange)
                            {
                                toReplace.Add(o.TileLocation);
                            }
                            else if (o.ParentSheetIndex == 420 && hit <= redMushroomChanceToChange)
                            {
                                toReplace.Add(o.TileLocation);
                            }
                            else if (o.ParentSheetIndex == 422 && hit <= purpleMushroomChanceToChange)
                            {
                                toReplace.Add(o.TileLocation);
                            }
                        }

                        foreach (Vector2 location in toReplace)
                        {   
                            Game1.currentLocation.Objects.Remove(location);
                            loc.terrainFeatures.Add(location, (TerrainFeature)new VoidshroomTree(Game1.random.Next(4, 6)));
                        }
                    }
                }
            }
        }
    }
}
