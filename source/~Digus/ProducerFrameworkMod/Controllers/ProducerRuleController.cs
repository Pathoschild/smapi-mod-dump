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
using Object = StardewValley.Object;

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
            return producerRule.ExcludeIdentifiers != null && (producerRule.ExcludeIdentifiers.Contains(input.ParentSheetIndex.ToString())
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
            foreach (Tuple<int, int> fuel in producerRule.FuelList)
            {
                if (!who.hasItemInInventory(fuel.Item1, fuel.Item2))
                {
                    if (!probe)
                    {
                        if (fuel.Item1 >= 0)
                        {
                            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation",ContentSource.GameContent);
                            var objectName = Lexicon.makePlural(ObjectUtils.GetObjectParameter(objects[fuel.Item1], (int) ObjectParameter.DisplayName), fuel.Item2 == 1);
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
                        }
                        else
                        {
                            var objectName = ObjectUtils.GetCategoryName(fuel.Item1);
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
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
            Func<int, int, bool> fuelSearch, Farmer who, GameLocation location
            , ProducerConfig producerConfig = null, Object input = null
            , bool probe = false, bool noSoundAndAnimation = false)
        {
            if (who == null)
            {
                who = Game1.getFarmer((long)producer.owner);
            }
            Vector2 tileLocation = producer.TileLocation;
            Random random = ProducerRuleController.GetRandomForProducing(tileLocation);
            OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random, fuelSearch, location, input);
            if (outputConfig != null)
            {
                Object output = producerRule.LookForInputWhenReady == null ? OutputConfigController.CreateOutput(outputConfig, input, random) : new Object(outputConfig.OutputIndex,1);

                producer.heldObject.Value = output;
                if (!probe)
                {
                    if (producerRule.LookForInputWhenReady == null)
                    {
                        OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);
                    }

                    if (!noSoundAndAnimation)
                    {
                        SoundUtil.PlaySound(producerRule.Sounds, location);
                        SoundUtil.PlayDelayedSound(producerRule.DelayedSounds, location);
                    }

                    producer.minutesUntilReady.Value = outputConfig.MinutesUntilReady ?? producerRule.MinutesUntilReady;
                    if (producerRule.SubtractTimeOfDay)
                    {
                        producer.minutesUntilReady.Value = Math.Max(producer.minutesUntilReady.Value - Utility.ConvertTimeToMinutes(Game1.timeOfDay) + 360, 1);
                    }

                    if (producerConfig != null)
                    {
                        producer.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    if (producerRule.PlacingAnimation.HasValue && !noSoundAndAnimation)
                    {
                        AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value,
                            producerRule.PlacingAnimationColor, location, tileLocation,
                            new Vector2(producerRule.PlacingAnimationOffsetX, producerRule.PlacingAnimationOffsetY));
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
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed * (int)Game1.stats.DaysPlayed * 1000000531 + (int)tileLocation.X * (int)tileLocation.X * 100207 + (int)tileLocation.Y * (int)tileLocation.Y * 1031 + Game1.timeOfDay/10);
        }

        public static void ClearProduction(Object producer, GameLocation location)
        {
            producer.heldObject.Value = (Object)null;
            producer.readyForHarvest.Value = false;
            producer.showNextIndex.Value = false;
            producer.minutesUntilReady.Value = -1;

            if (ProducerController.GetProducerConfig(producer.Name) is ProducerConfig producerConfig && producerConfig.LightSource?.AlwaysOn == true)
            {
                int identifier = LightSourceConfigController.GenerateIdentifier(producer.tileLocation);
                if (location.hasLightSource(identifier))
                {
                    location.removeLightSource(identifier);
                    producer.initializeLightSource(producer.tileLocation);
                }
            }
        }

        public static void PrepareOutput(Object producer, GameLocation location, Farmer who)
        {
            foreach (ProducerRule producerRule in ProducerController.GetProducerRules(producer.Name))
            {
                if (producerRule.LookForInputWhenReady is InputSearchConfig inputSearchConfig)
                {
                    if (producerRule.OutputConfigs.Find(o => o.OutputIndex == producer.heldObject.Value.ParentSheetIndex) is
                        OutputConfig outputConfig)
                    {
                        Object input = ProducerRuleController.SearchInput(location, producer.tileLocation,
                            inputSearchConfig);
                        producer.heldObject.Value = OutputConfigController.CreateOutput(outputConfig, input,
                            ProducerRuleController.GetRandomForProducing(producer.tileLocation));
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
            Queue<Vector2> tilesQueue = new Queue<Vector2>();
            HashSet<Vector2> visitedTiles = new HashSet<Vector2>();
            tilesQueue.Enqueue(startTileLocation);
            int maxRange = inputSearchConfig.Range;
            for (int currentRange = 0; (maxRange >= 0 || maxRange < 0 && currentRange <= 150) && tilesQueue.Count > 0; ++currentRange)
            {
                Vector2 currentTile = tilesQueue.Dequeue();
                if (inputSearchConfig.GardenPot || inputSearchConfig.Crop)
                {
                    Crop crop = null;
                    if (inputSearchConfig.Crop && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is HoeDirt hoeDirt && hoeDirt.crop != null && hoeDirt.readyForHarvest() && (!inputSearchConfig.ExcludeForageCrops || !(hoeDirt.crop.forageCrop)))
                    {
                        crop = hoeDirt.crop;
                    }
                    else if (inputSearchConfig.GardenPot && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is IndoorPot indoorPot && indoorPot.hoeDirt.Value is HoeDirt potHoeDirt && potHoeDirt.crop != null && potHoeDirt.readyForHarvest() && (!inputSearchConfig.ExcludeForageCrops || !(potHoeDirt.crop.forageCrop)))
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
                            Object obj = new Object(crop.indexOfHarvest.Value, 1, false, -1, 0);
                            if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            if (!crop.programColored.Value)
                            {
                                return new Object(crop.indexOfHarvest.Value, 1);
                            }
                            else
                            {
                                return new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value);
                            }
                        }
                    }
                }
                if (inputSearchConfig.FruitTree && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is FruitTree fruitTree && fruitTree.fruitsOnTree.Value > 0)
                {
                    bool found = false;
                    if (inputSearchConfig.InputIdentifier.Contains(fruitTree.indexOfFruit.Value.ToString()))
                    {
                        found = true;
                    }
                    else
                    {
                        Object obj = new Object(fruitTree.indexOfFruit.Value, 1, false, -1, 0);
                        if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        return new Object(fruitTree.indexOfFruit.Value, 1);
                    }
                }
                if (inputSearchConfig.BigCraftable && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is Object bigCraftable && bigCraftable.bigCraftable && bigCraftable.heldObject.Value is Object heldObject && bigCraftable.readyForHarvest.Value)
                {
                    bool found = false;
                    if (inputSearchConfig.InputIdentifier.Contains(heldObject.ParentSheetIndex.ToString()))
                    {
                        found = true;
                    }
                    else
                    {
                        Object obj = new Object(heldObject.ParentSheetIndex, 1, false, -1, 0);
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
