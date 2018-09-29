using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace TreeShaker
{
    class ModConfig
    {
        public Vector2 ChestCoords = new Vector2(69f, 16f);
    }
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            TimeEvents.AfterDayStarted += this.DayStarted;
        }
                   
        private void DayStarted(object sender, EventArgs e)
        {
            Farm farm = Game1.getFarm();
            if (farm.objects.TryGetValue(this.Config.ChestCoords, out SObject obj) && obj is Chest chest)
            {
                foreach (Farm location in Game1.locations.OfType<Farm>())
                {
                    List<Vector2> seedsToRemove = new List<Vector2>();
                    foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
                    {
                        Vector2 tile = pair.Key;
                        Tree tree = pair.Value as Tree;
                        if (tree == null)

                            continue;
                        {                            
                            bool added = false;
                            switch (tree.treeType.Value)
                            {
                                case 1:
                                    Item a = chest.addItem(new SObject(309, 1, false, -1, 0));
                                    if (a == null)
                                    {
                                        added = true;
                                    }
                                    break;
                                case 2:
                                    Item m = chest.addItem(new SObject(310, 1, false, -1, 0));
                                    if (m == null)
                                    {
                                        added = true;
                                    }
                                    break;
                                case 3:
                                    Item p = chest.addItem(new SObject(311, 1, false, -1, 0));
                                    if (p == null)
                                    {
                                        added = true;
                                    }
                                    break;
                            }

                            if (added && tree.growthStage.Value != Tree.seedStage)
                            {
                                tree.hasSeed.Value = false;
                            }
                            if (added && tree.growthStage.Value == Tree.seedStage)
                            {
                                seedsToRemove.Add(tile);                                
                            }                            
                        }
                    }
                    foreach (Vector2 seedTile in seedsToRemove)
                    {
                        location.terrainFeatures.Remove(seedTile);
                    }
                }
            }
        }
    }
}