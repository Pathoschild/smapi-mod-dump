/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.GameData.Objects;
using Object = StardewValley.Object;
using System.Security.AccessControl;
using StardewValley.Inventories;

namespace ProducerFrameworkMod.Controllers
{
    public class ProducerRuleController
    {
        /// <summary>
        /// Check if an input is excluded by a producer rule.
        /// </summary>
        /// <param name="producerRule">The producer rule to check.</param>
        /// <param name="input">The input to check</param>
        /// <returns>true if should be excluded</returns>
        public static bool IsInputExcluded(ProducerRule producerRule, Object input)
        {
            return producerRule.ExcludeIdentifiers != null && (producerRule.ExcludeIdentifiers.Contains(input.QualifiedItemId.ToString())
                                                               ||producerRule.ExcludeIdentifiers.Contains(input.ItemId.ToString())
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Name)
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Category.ToString())
                                                               || producerRule.ExcludeIdentifiers.Intersect(input.GetContextTags()).Any());
        }

        /// <summary>
        /// Check if an input has the required stack for the producer rule.
        /// </summary>
        /// <param name="producerRule">the producer rule to check</param>
        /// <param name="input">The input to check</param>
        public static void ValidateIfInputStackLessThanRequired(ProducerRule producerRule, Object input, bool probe)
        {
            int requiredStack = producerRule.InputStack;
            if (input.Stack < requiredStack)
            {
                if (!probe)
                {
                    throw new RestrictionException(DataLoader.Helper.Translation.Get(
                        "Message.Requirement.Amount"
                        , new { amount = requiredStack, objectName = Lexicon.makePlural(input.DisplayName, requiredStack == 1) }
                    ));
                }
                else
                {
                    throw new RestrictionException();
                }
            }
        }

        /// <summary>
        /// Check if a farmer has the required fules and stack for a given producer rule.
        /// </summary>
        /// <param name="producerRule">the producer tule to check</param>
        /// <param name="who">The farmer to check</param>
        public static void ValidateIfAnyFuelStackLessThanRequired(ProducerRule producerRule, Farmer who, bool probe)
        {
            foreach (Tuple<string, int> fuel in producerRule.FuelList)
            {
                IInventory inventory = Object.autoLoadFrom ?? who.Items;
                if (!(who.getItemCountInList(inventory,fuel.Item1) >= fuel.Item2))
                {
                    if (!probe)
                    {
                        if (ItemRegistry.Exists(fuel.Item1))
                        {
                                var objectName = Lexicon.makePlural(ItemRegistry.GetData(fuel.Item1).DisplayName, fuel.Item2 == 1);
                                throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = fuel.Item2, objectName }));
                        }
                        else
                        {
                            var objectName = fuel.Item1;
                            if (int.TryParse(fuel.Item1, out int item1))
                            {
                                objectName = ObjectUtils.GetCategoryName(item1);
                            }
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = fuel.Item2, objectName }));
                        }
                    }
                    else
                    {
                        throw new RestrictionException();
                    }
                }
            }
        }

        public static OutputConfig ProduceOutput(ProducerRule producerRule, Object producer,
            Func<string, int, bool> fuelSearch, Farmer who, GameLocation location
            , ProducerConfig producerConfig = null, Object input = null
            , bool probe = false, bool noSoundAndAnimation = false)
        {
            who ??= Game1.getFarmer((long)producer.owner.Value);
            Vector2 tileLocation = producer.TileLocation;
            Random random = ProducerRuleController.GetRandomForProducing(tileLocation);
            OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random, fuelSearch, location, input);
            if (outputConfig != null)
            {
                Object output = producerRule.LookForInputWhenReady == null ? OutputConfigController.CreateOutput(outputConfig, input, random) : ItemRegistry.Create<Object>(outputConfig.OutputItemId);
                
                if (!probe)
                {
                    producer.heldObject.Value = output;
                    if (producerRule.LookForInputWhenReady == null)
                    {
                        OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);
                    }

                    if (!noSoundAndAnimation)
                    {
                        SoundUtil.PlaySound(producerRule.Sounds, location);
                        SoundUtil.PlayDelayedSound(producerRule.DelayedSounds, location, producer.TileLocation);
                    }

                    producer.MinutesUntilReady = outputConfig.MinutesUntilReady ?? producerRule.MinutesUntilReady;
                    if (producerRule.SubtractTimeOfDay)
                    {
                        producer.MinutesUntilReady = Math.Max(producer.MinutesUntilReady - Utility.ConvertTimeToMinutes(Game1.timeOfDay) + 360, 1);
                    }

                    producer.lastOutputRuleId.Value = null;
                    if (producerConfig != null)
                    {
                        producer.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }
                    else if (producer.GetMachineData() is { ShowNextIndexWhileWorking: true })
                    {
                        producer.showNextIndex.Value = true;
                    }

                    if (producerRule.PlacingAnimation.HasValue && !noSoundAndAnimation)
                    {
                        AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value,
                            producerRule.PlacingAnimationColor, location, tileLocation,
                            new(producerRule.PlacingAnimationOffsetX, producerRule.PlacingAnimationOffsetY));
                    }

                    if (location.hasLightSource(LightSourceConfigController.GenerateIdentifier(tileLocation)))
                    {
                        location.removeLightSource(LightSourceConfigController.GenerateIdentifier(tileLocation));
                    }
                    producer.initializeLightSource(tileLocation, false);

                    producerRule.IncrementStatsOnInput.ForEach(s => StatsController.IncrementStardewStats(s, outputConfig.RequiredInputStack ?? producerRule.InputStack));
                    producerRule.IncrementStatsLabelOnInput.ForEach(s => StatsController.IncrementStardewStats(s, outputConfig.RequiredInputStack ?? producerRule.InputStack));
                }

            }
            return outputConfig;
        }

        /// <summary>
        /// Create a Random instance using the seed rules for a given positioned machine.
        /// </summary>
        /// <param name="tileLocation">The position of the machines the random should be created for.</param>
        /// <returns>The random instnace</returns>
        public static Random GetRandomForProducing(Vector2 tileLocation)
        {
            return new((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed * (int)Game1.stats.DaysPlayed * 1000000531 + (int)tileLocation.X * (int)tileLocation.X * 100207 + (int)tileLocation.Y * (int)tileLocation.Y * 1031 + Game1.timeOfDay/10);
        }

        public static void ClearProduction(Object producer, GameLocation location)
        {
            producer.heldObject.Value = (Object)null;
            producer.readyForHarvest.Value = false;
            producer.showNextIndex.Value = false;
            producer.MinutesUntilReady = -1;
            producer.ResetParentSheetIndex();

            if (ProducerController.GetProducerConfig(producer.QualifiedItemId) is ProducerConfig producerConfig && producerConfig.LightSource?.AlwaysOn == true)
            {
                int identifier = LightSourceConfigController.GenerateIdentifier(producer.TileLocation);
                if (location.hasLightSource(identifier))
                {
                    location.removeLightSource(identifier);
                    producer.initializeLightSource(producer.TileLocation);
                }
            }
        }

        public static void PrepareOutput(Object producer, GameLocation location, Farmer who)
        {
            foreach (ProducerRule producerRule in ProducerController.GetProducerRules(producer.QualifiedItemId))
            {
                if (producerRule.LookForInputWhenReady is InputSearchConfig inputSearchConfig)
                {
                    if (producerRule.OutputConfigs.Find(o => o.OutputItemId == producer.heldObject.Value.QualifiedItemId) is
                        OutputConfig outputConfig)
                    {
                        Object input = ProducerRuleController.SearchInput(location, producer.TileLocation,
                            inputSearchConfig);
                        producer.heldObject.Value = OutputConfigController.CreateOutput(outputConfig, input,
                            ProducerRuleController.GetRandomForProducing(producer.TileLocation));
                        OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);
                        break;
                    }
                }
            }
        }

        private static Object SearchInput(
            GameLocation location,
            Vector2 startTileLocation,
            InputSearchConfig inputSearchConfig)
        {
            Queue<Vector2> tilesQueue = new();
            HashSet<Vector2> visitedTiles = new();
            tilesQueue.Enqueue(startTileLocation);
            int maxRange = inputSearchConfig.Range;
            for (int currentRange = 0; (maxRange >= 0 || maxRange < 0 && currentRange <= 150) && tilesQueue.Count > 0; ++currentRange)
            {
                Vector2 currentTile = tilesQueue.Dequeue();
                if (inputSearchConfig.GardenPot || inputSearchConfig.Crop)
                {
                    Crop crop = null;
                    if (inputSearchConfig.Crop && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is HoeDirt hoeDirt && hoeDirt.crop != null && hoeDirt.readyForHarvest() && (!inputSearchConfig.ExcludeForageCrops || !(hoeDirt.crop.forageCrop.Value)))
                    {
                        crop = hoeDirt.crop;
                    }
                    else if (inputSearchConfig.GardenPot && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is IndoorPot indoorPot && indoorPot.hoeDirt.Value is HoeDirt potHoeDirt && potHoeDirt.crop != null && potHoeDirt.readyForHarvest() && (!inputSearchConfig.ExcludeForageCrops || !(potHoeDirt.crop.forageCrop.Value)))
                    {
                        crop = potHoeDirt.crop;
                    }
                    if (crop != null)
                    {
                        bool found = false;
                        if (inputSearchConfig.InputIdentifier.Contains(crop.indexOfHarvest.Value.ToString()))
                        {
                            found = true;
                        }
                        else
                        {
                            Object obj = new(crop.indexOfHarvest.Value, 1, false, -1, 0);
                            if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            if (!crop.programColored.Value)
                            {
                                return new(crop.indexOfHarvest.Value, 1);
                            }
                            else
                            {
                                return new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value);
                            }
                        }
                    }
                }
                if (inputSearchConfig.FruitTree && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is FruitTree fruitTree && fruitTree.fruit.Count > 0)
                {
                    Item foundFruit = null;
                    foundFruit = fruitTree.fruit.FirstOrDefault(f => inputSearchConfig.InputIdentifier.Contains(f.ItemId));
                    
                    if (foundFruit == null)
                    {
                        foreach (var fruit in fruitTree.fruit)
                        {
                            foundFruit = fruitTree.fruit.FirstOrDefault(f => inputSearchConfig.InputIdentifier.Contains(fruit.Name) || inputSearchConfig.InputIdentifier.Contains(fruit.Category.ToString()));
                        }
                    }
                    if (foundFruit != null)
                    {
                        return (Object)foundFruit.getOne();
                    }
                }
                if (inputSearchConfig.BigCraftable && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is Object bigCraftable && bigCraftable.bigCraftable.Value && bigCraftable.heldObject.Value is Object heldObject && bigCraftable.readyForHarvest.Value)
                {
                    bool found = false;
                    if (inputSearchConfig.InputIdentifier.Contains(heldObject.ParentSheetIndex.ToString()))
                    {
                        found = true;
                    }
                    else
                    {
                        Object obj = new(heldObject.ItemId, 1, false, -1, 0);
                        if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        return (Object)heldObject.getOne();
                    }
                }
                //Look for in nearby tiles.
                foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(currentTile))
                {
                    if (!visitedTiles.Contains(adjacentTileLocation) && (maxRange < 0 || (double)Math.Abs(adjacentTileLocation.X - startTileLocation.X) + (double)Math.Abs(adjacentTileLocation.Y - startTileLocation.Y) <= (double)maxRange))
                        tilesQueue.Enqueue(adjacentTileLocation);
                }
                visitedTiles.Add(currentTile);
            }
            return (Object)null;
        }
    }
}
