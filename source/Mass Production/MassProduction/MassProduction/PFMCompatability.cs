/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using ProducerFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction
{
    /// <summary>
    /// Helper class with methods to help integrate with ProducerFrameworkMod.
    /// </summary>
    public class PFMCompatability
    {
        /// <summary>
        /// Check if an input is excluded by a producer rule or a mass production machine definition.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ProducerRuleController.cs
        /// </summary>
        /// <param name="producerRule">The producer rule to check.</param>
        /// <param name="mpm">The definition of the mass production machine to check.</param>
        /// <param name="input">The input to check.</param>
        /// <returns>True if it should be excluded.</returns>
        public static bool IsInputExcluded(ProducerRule producerRule, MassProductionMachineDefinition mpm, SObject input)
        {
            bool isExcludedByRule = producerRule.ExcludeIdentifiers != null &&
                (producerRule.ExcludeIdentifiers.Contains(input.ParentSheetIndex.ToString()) || producerRule.ExcludeIdentifiers.Contains(input.Name) ||
                producerRule.ExcludeIdentifiers.Contains(input.Category.ToString()) || producerRule.ExcludeIdentifiers.Intersect(input.GetContextTags()).Any());
            bool isExcludedByMPM = mpm.BlacklistedInputKeys.Contains(input.ParentSheetIndex.ToString()) || mpm.BlacklistedInputKeys.Contains(input.Name) ||
                mpm.BlacklistedInputKeys.Contains(input.Category.ToString()) || mpm.BlacklistedInputKeys.Intersect(input.GetContextTags()).Any();

            return isExcludedByRule || isExcludedByMPM;
        }

        /// <summary>
        /// Check if the input and fuels are of the required quantities for the producer rule and machine settings.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ProducerRuleController.cs
        /// </summary>
        /// <param name="producerRule">the producer rule to check.</param>
        /// <param name="settings">Mass production machine settings to check.</param>
        /// <param name="inputs">The inputs to check.</param>
        /// <param name="who">The farmer placing an item in the machine.</param>
        public static void ValidateIfInputsLessThanRequired(ProducerRule producerRule, MPMSettings settings, List<InputInfo> inputs, Farmer who)
        {
            foreach (InputInfo input in inputs)
            {
                int quantityRequired = settings.CalculateInputRequired(input);
                
                if ((!input.IsFuel && who.ActiveObject.Stack < quantityRequired) ||
                    (input.IsFuel && !who.hasItemInInventory(input.ID, quantityRequired)))
                {
                    throw new RestrictionException(string.Format("{1}x{0} required", quantityRequired, input.Name));
                }
            }
        }

        /// <summary>
        /// Makes the machine produce an appropriate output.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/OutputConfigController.cs
        /// </summary>
        /// <param name="producerRule"></param>
        /// <param name="settings"></param>
        /// <param name="producer"></param>
        /// <param name="fuelSearch"></param>
        /// <param name="who"></param>
        /// <param name="location"></param>
        /// <param name="producerConfig"></param>
        /// <param name="input"></param>
        /// <param name="probe"></param>
        /// <param name="noSoundAndAnimation"></param>
        /// <returns>Base output config - no values altered for mass production machine</returns>
        public static OutputConfig ProduceOutput(ProducerRule producerRule, MPMSettings settings, SObject producer, Func<int, int, bool> fuelSearch, Farmer who, GameLocation location,
            ProducerConfig producerConfig = null, SObject input = null, int inputQuantity = 0, bool probe = false, bool noSoundAndAnimation = false,
            List<InputInfo> inputInfo = null)
        {
            if (who == null)
            {
                who = Game1.getFarmer((long)producer.owner);
            }

            if (inputInfo == null)
            {
                inputInfo = new List<InputInfo>();
            }

            Vector2 tileLocation = producer.TileLocation;
            Random random = ProducerRuleController.GetRandomForProducing(tileLocation);
            OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random, fuelSearch, location, input);

            if (outputConfig != null)
            {
                SObject output = producerRule.LookForInputWhenReady == null ? OutputConfigController.CreateOutput(outputConfig, input, random) : new SObject(outputConfig.OutputIndex, 1);
                output.Stack = settings.CalculateOutputProduced(output.Stack, inputInfo.ToArray());

                if (settings.Quality.HasValue)
                {
                    if (settings.Quality == QualitySetting.KeepInput)
                    {
                        output.Quality = input.Quality;
                    }
                    else
                    {
                        output.Quality = settings.GetOutputQuality();
                    }
                }

                producer.heldObject.Value = output;

                if (!probe)
                {
                    if (producerRule.LookForInputWhenReady == null)
                    {
                        OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);
                    }

                    //if (!noSoundAndAnimation)
                    //{
                    //    SoundUtil.PlaySound(producerRule.Sounds, location);
                    //    SoundUtil.PlayDelayedSound(producerRule.DelayedSounds, location);
                    //}

                    int minutesUntilReadyBase = outputConfig.MinutesUntilReady ?? producerRule.MinutesUntilReady;
                    int minutesUntilReady = settings.CalculateTimeRequired(minutesUntilReadyBase, inputInfo.ToArray());

                    producer.minutesUntilReady.Value = minutesUntilReady;
                    if (producerRule.SubtractTimeOfDay)
                    {
                        producer.minutesUntilReady.Value = Math.Max(producer.minutesUntilReady.Value - Game1.timeOfDay, 1);
                    }

                    if (producerConfig != null)
                    {
                        producer.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    //if (producerRule.PlacingAnimation.HasValue && !noSoundAndAnimation)
                    //{
                    //    AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value,
                    //        producerRule.PlacingAnimationColor, location, tileLocation,
                    //        new Vector2(producerRule.PlacingAnimationOffsetX, producerRule.PlacingAnimationOffsetY));
                    //}

                    if (location.hasLightSource(LightSourceConfigController.GenerateIdentifier(tileLocation)))
                    {
                        location.removeLightSource(LightSourceConfigController.GenerateIdentifier(tileLocation));
                    }
                    producer.initializeLightSource(tileLocation, false);

                    int statsIncrement = inputQuantity;
                    producerRule.IncrementStatsOnInput.ForEach(s => StatsController.IncrementStardewStats(s, statsIncrement));
                }

            }
            return outputConfig;
        }

        /// <summary>
        /// Gets an object's name.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectUtils.cs
        /// </summary>
        /// <param name="objectString"></param>
        /// <returns></returns>
        internal static string GetObjectName(string objectString)
        {
            return objectString.Split('/')[4];
        }

        /// <summary>
        /// Gets the name of an object category.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectUtils.cs
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <returns></returns>
        internal static string GetCategoryName(int categoryIndex)
        {
            switch (categoryIndex)
            {
                case -6:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
                case -5:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
                case -4:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
                case -2:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
                case -81:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12869");
                case -80:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12866");
                case -79:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12854");
                case -75:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12851");
                case -74:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12855");
                case -28:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12867");
                case -27:
                    return ModEntry.Instance.Helper.Translation.Get("Object.Category.TappedTreeProduct");
                case -26:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12862");
                case -25:
                case -7:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12853");
                case -24:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859");
                case -22:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858");
                case -21:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12857");
                case -20:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12860");
                case -19:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12856");
                case -18:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12864");
                case -14:
                    return ModEntry.Instance.Helper.Translation.Get("Object.Category.Meat");
                case -16:
                    return ModEntry.Instance.Helper.Translation.Get("Object.Category.BuildingResources");
                case -15:
                    return ModEntry.Instance.Helper.Translation.Get("Object.Category.MetalResources");
                case -12:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12850");
                case -8:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12863");
                default:
                    return "???";
            }
        }

        /// <summary>
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ProducerRuleController.cs
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="location"></param>
        /// <param name="who"></param>
        public static void PrepareOutput(SObject producer, GameLocation location, Farmer who)
        {
            if(string.IsNullOrEmpty(producer.GetMassProducerKey())) { return; }

            MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(producer.name, producer.GetMassProducerKey());

            if (mpm == null) { return; }

            foreach (ProducerRule producerRule in ProducerController.GetProducerRules(mpm.BaseProducerName))
            {
                if (producerRule.LookForInputWhenReady is InputSearchConfig inputSearchConfig)
                {
                    if (producerRule.OutputConfigs.Find(o => o.OutputIndex == producer.heldObject.Value.ParentSheetIndex) is OutputConfig outputConfig)
                    {
                        SObject input = SearchInput(location, producer.tileLocation, inputSearchConfig);
                        List<InputInfo> inputInfo = InputInfo.ConvertPFMInputs(producerRule, input);
                        SObject output = OutputConfigController.CreateOutput(outputConfig, input, ProducerRuleController.GetRandomForProducing(producer.tileLocation));

                        output.Stack = mpm.Settings.CalculateOutputProduced(output.stack, inputInfo.ToArray());

                        if (mpm.Settings.Quality.HasValue)
                        {
                            if (mpm.Settings.Quality == QualitySetting.KeepInput)
                            {
                                output.Quality = input.Quality;
                            }
                            else
                            {
                                output.Quality = mpm.Settings.GetOutputQuality();
                            }
                        }

                        producer.heldObject.Value = output;
                        OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Copied from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ProducerRuleController.cs
        /// </summary>
        /// <param name="location"></param>
        /// <param name="startTileLocation"></param>
        /// <param name="inputSearchConfig"></param>
        /// <returns></returns>
        private static SObject SearchInput(GameLocation location, Vector2 startTileLocation, InputSearchConfig inputSearchConfig)
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
                    if (inputSearchConfig.Crop && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is 
                        HoeDirt hoeDirt && hoeDirt.crop != null && hoeDirt.readyForHarvest() && (!inputSearchConfig.ExcludeForageCrops || !(hoeDirt.crop.forageCrop)))
                    {
                        crop = hoeDirt.crop;
                    }
                    else if (inputSearchConfig.GardenPot && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is IndoorPot indoorPot &&
                        indoorPot.hoeDirt.Value is HoeDirt potHoeDirt && potHoeDirt.crop != null && potHoeDirt.readyForHarvest() &&
                        (!inputSearchConfig.ExcludeForageCrops || !(potHoeDirt.crop.forageCrop)))
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
                            SObject obj = new SObject(crop.indexOfHarvest.Value, 1, false, -1, 0);
                            if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            if (!crop.programColored.Value)
                            {
                                return new SObject(crop.indexOfHarvest.Value, 1);
                            }
                            else
                            {
                                return new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value);
                            }
                        }
                    }
                }
                if (inputSearchConfig.FruitTree && location.terrainFeatures.ContainsKey(currentTile) && location.terrainFeatures[currentTile] is FruitTree fruitTree &&
                    fruitTree.fruitsOnTree.Value > 0)
                {
                    bool found = false;
                    if (inputSearchConfig.InputIdentifier.Contains(fruitTree.indexOfFruit.Value.ToString()))
                    {
                        found = true;
                    }
                    else
                    {
                        SObject obj = new SObject(fruitTree.indexOfFruit.Value, 1, false, -1, 0);
                        if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        return new SObject(fruitTree.indexOfFruit.Value, 1);
                    }
                }
                if (inputSearchConfig.BigCraftable && location.Objects.ContainsKey(currentTile) && location.Objects[currentTile] is SObject bigCraftable &&
                    bigCraftable.bigCraftable && bigCraftable.heldObject.Value is SObject heldObject && bigCraftable.readyForHarvest.Value)
                {
                    bool found = false;
                    if (inputSearchConfig.InputIdentifier.Contains(heldObject.ParentSheetIndex.ToString()))
                    {
                        found = true;
                    }
                    else
                    {
                        SObject obj = new SObject(heldObject.ParentSheetIndex, 1, false, -1, 0);
                        if (inputSearchConfig.InputIdentifier.Any(i => i == obj.Name || i == obj.Category.ToString()))
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        return (SObject)heldObject.getOne();
                    }
                }
                //Look for in nearby tiles.
                foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(currentTile))
                {
                    if (!visitedTiles.Contains(adjacentTileLocation) && (maxRange < 0 || (double)Math.Abs(adjacentTileLocation.X - startTileLocation.X) +
                        (double)Math.Abs(adjacentTileLocation.Y - startTileLocation.Y) <= (double)maxRange))
                        tilesQueue.Enqueue(adjacentTileLocation);
                }
                visitedTiles.Add(currentTile);
            }

            return null;
        }

        /// <summary>
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ProducerRuleController.cs
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="location"></param>
        public static void ClearProduction(SObject producer, GameLocation location)
        {
            producer.heldObject.Value = null;
            producer.readyForHarvest.Value = false;
            producer.showNextIndex.Value = false;
            producer.minutesUntilReady.Value = -1;

            if (!string.IsNullOrEmpty(producer.GetMassProducerKey()))
            {
                MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(producer.name, producer.GetMassProducerKey());

                if (mpm != null &&
                    mpm.GetBaseProducerConfig() is ProducerConfig producerConfig &&
                    producerConfig.LightSource?.AlwaysOn == true)
                {
                    int identifier = LightSourceConfigController.GenerateIdentifier(producer.tileLocation);
                    if (location.hasLightSource(identifier))
                    {
                        location.removeLightSource(identifier);
                        producer.initializeLightSource(producer.tileLocation);
                    }
                }
            }
        }
    }
}
