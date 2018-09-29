using AutoGrabberMod.Models;
using AutoGrabberMod.UserInterfaces;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoGrabberMod.Features
{
    using SVObject = StardewValley.Object;

    class Harvest : Feature
    {
        public override string FeatureName => "Auto Harvest Crops";

        public override string FeatureConfig => "crops";

        public string FlowersConfig = "flowers";

        public bool Flowers { get; set; } = false;

        public override int Order => 2;

        public override bool IsAllowed => Utilities.Config.AllowAutoHarvest;

        public Harvest()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;

            FruitTrees();

            KeyValuePair<Vector2, HoeDirt>[] hoeDirts = (Grabber.Dirts).Where((arg) => arg.Value.readyForHarvest()).ToArray();
            //Utilities.Monitor.Log($"  {Grabber.InstanceName} Attempting to harvest crops {(bool)Value} {hoeDirts.Count()}", StardewModdingAPI.LogLevel.Trace);
            foreach (KeyValuePair<Vector2, HoeDirt> pair in hoeDirts)
            {
                HoeDirt dirt = pair.Value;
                Vector2 tile = pair.Key;
                Crop crop = pair.Value.crop;
                SVObject harvest;
                int stack = 0;

                if (Grabber.IsChestFull || !(bool)Value) break;
                if (crop == null) continue;
                if (Utilities.IsCropFlower(crop) && (!Flowers || SDate.Now().Day != 28))
                {
                    Utilities.Monitor.Log($"    {Grabber.InstanceName} Crop is a flower and AutoHarvestFlowers: is false = {Flowers} or its not the 28 {SDate.Now()}", StardewModdingAPI.LogLevel.Trace);
                    continue;
                }

                if (crop != null && crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0))
                {
                    int num1 = 1;
                    int num2 = 0;
                    int num3 = 0;

                    Random random = new Random((int)tile.X * 7 + (int)tile.Y * 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
                    switch ((dirt.fertilizer.Value))
                    {
                        case 368:
                            num3 = 1;
                            break;
                        case 369:
                            num3 = 2;
                            break;
                    }
                    double num4 = 0.2 * (Game1.player.FarmingLevel / 10.0) + 0.2 * num3 * ((Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
                    double num5 = Math.Min(0.75, num4 * 2.0);

                    if (random.NextDouble() < num4) num2 = 2;
                    else if (random.NextDouble() < num5) num2 = 1;

                    if ((crop.minHarvest.Value) > 1 || (crop.maxHarvest.Value) > 1)
                    {
                        var calculated = Math.Min(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + Game1.player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value);                        
                        num1 = crop.minHarvest.Value == calculated ? calculated : random.Next(crop.minHarvest.Value, calculated);
                    }                    

                    if (crop.chanceForExtraCrops.Value > 0.0)
                    {
                        while (random.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value)) ++num1;
                    }
                    for (int i = 0; i < num1; i++) stack += 1;
                    if (crop.harvestMethod.Value == 1)
                    {                        

                        if (crop.regrowAfterHarvest.Value != -1)
                        {
                            crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                            crop.fullyGrown.Value = true;
                        }
                        else
                        {
                            dirt.crop = null;
                        }                        
                        harvest = new SVObject(crop.indexOfHarvest.Value, stack, false, -1, num2);
                    }
                    else
                    {
                        harvest = !crop.programColored.Value
                                       ? new SVObject(crop.indexOfHarvest.Value, stack, false, -1, num2)
                                       : new ColoredObject(crop.indexOfHarvest.Value, stack, crop.tintColor.Value) { Quality = num2 };

                        if (crop.regrowAfterHarvest.Value != -1)
                        {
                            crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                            crop.fullyGrown.Value = true;
                        }
                        else
                        {
                            dirt.crop = null;
                        }
                    }

                    if (harvest != null)
                    {
                        //Utilities.Monitor.Log($"    {InstanceName} Harvested: {harvest.Name} {harvest.Stack} {pair.Key.X},{pair.Key.Y}", StardewModdingAPI.LogLevel.Trace);
                        Grabber.GrabberChest.addItem(harvest);
                        if (Grabber.GainExperience) Utilities.GainExperience(Grabber.FARMING, 3);
                        //PlantSeed(tile, dirt);
                    }

                }
            }

            Grabber.Grabber.showNextIndex.Value |= Grabber.GrabberChest.items.Count != 0;
        }

        private void FruitTrees()
        {
            var nearbyTiles = (Grabber.RangeEntireMap ? Utilities.GetLocationObjectTiles(Grabber.Location) : Grabber.NearbyTilesRange)
                .Where(t => Grabber.Location.terrainFeatures.ContainsKey(t) && Grabber.Location.terrainFeatures[t] is FruitTree)
                .GroupBy(t => t).Select(g => g.First())
                .ToDictionary(t => t, t => Grabber.Location.terrainFeatures[t] as FruitTree);

            foreach (var pair in nearbyTiles)
            {
                if (Grabber.IsChestFull) break;

                FruitTree tree = pair.Value;
                if (tree.growthStage.Value >= FruitTree.treeStage && tree.fruitsOnTree.Value > 0 && !tree.stump.Value)
                {
                    //Utilities.Monitor.Log($"    {Grabber.InstanceName} harvesting fruit tree: {(new SVObject(tree.indexOfFruit.Value, 1)).Name} {tree.fruitsOnTree.Value} {pair.Key.X},{pair.Key.Y}", StardewModdingAPI.LogLevel.Trace);
                    SVObject item;
                    if (tree.struckByLightningCountdown.Value > 0)
                    {
                        item = new SVObject(382, tree.fruitsOnTree.Value);
                    }
                    else
                    {
                        int quality = SVObject.lowQuality;
                        if (tree.daysUntilMature.Value <= -112)
                            quality = SVObject.medQuality;
                        if (tree.daysUntilMature.Value <= -224)
                            quality = SVObject.highQuality;
                        if (tree.daysUntilMature.Value <= -336)
                            quality = SVObject.bestQuality;
                        item = new SVObject(tree.indexOfFruit.Value, tree.fruitsOnTree.Value, quality: quality);
                    }
                    Grabber.GrabberChest.addItem(item);
                    tree.fruitsOnTree.Value = 0;
                }
            }
        }

        public new void ConfigParse(string[] config)
        {
            if (config.Contains(FeatureConfig)) Value = true;
            if (config.Contains(FlowersConfig)) Flowers = true;
        }

        public new string ConfigValue()
        {
            var output = ((bool)Value) ? $" |{FeatureConfig}|" : "";
            if (Flowers) output += $" |{FlowersConfig}|";
            return output;
        }

        public new OptionsElement[] InterfaceElement()
        {
            return new OptionsElement[] {
                new OptionsCheckbox(FeatureName, (bool)Value, value => Value = value),
                new OptionsCheckbox("Auto Harvest Flowers on the 28th", Flowers, value =>
            {
                Flowers = value;
                Grabber.Update();
            })
            };
        }
    }
}
