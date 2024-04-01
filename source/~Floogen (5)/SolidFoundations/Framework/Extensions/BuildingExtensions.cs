/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using static SolidFoundations.Framework.Models.ContentPack.InputFilter;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Extensions
{
    internal static class BuildingExtensions
    {
        // TODO: Review that this works
        public static bool ValidateConditions(this Building building, string condition, string[] modDataFlags = null, GameLocation locationContext = null)
        {
            if (GameStateQuery.CheckConditions(condition, locationContext))
            {
                if (modDataFlags is not null)
                {
                    foreach (string flag in modDataFlags)
                    {
                        // Clear whitespace
                        var cleanedFlag = flag.Replace(" ", String.Empty);
                        bool flagShouldNotExist = String.IsNullOrEmpty(cleanedFlag) || cleanedFlag[0] != '!' ? false : true;
                        if (flagShouldNotExist)
                        {
                            cleanedFlag = cleanedFlag[1..];
                        }
                        cleanedFlag = String.Concat(ModDataKeys.FLAG_BASE, ".", cleanedFlag.ToLower());

                        if (building.modData.ContainsKey(cleanedFlag) is false == flagShouldNotExist is false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public static bool ValidateLayer(this Building building, ExtendedBuildingDrawLayer layer)
        {
            if (building.ValidateConditions(layer.Condition, layer.ModDataFlags) is false)
            {
                return false;
            }

            if (building.skinId.Value is not null && layer.SkinFilter is not null && layer.SkinFilter.Any(id => building.skinId.Value.Equals("Base_Texture", StringComparison.OrdinalIgnoreCase) || building.skinId.Value.Equals(id, StringComparison.OrdinalIgnoreCase)) is false)
            {
                return false;
            }

            return true;
        }

        public static void RefreshModel(this Building building, ExtendedBuildingModel model)
        {
            building.LoadFromBuildingData(model);

            if (model.Chests is not null)
            {
                foreach (ExtendedBuildingChest chestData in model.Chests)
                {
                    if (building.GetBuildingChest(chestData.Name) is Chest chest && chest is not null)
                    {
                        chest.modData[ModDataKeys.CUSTOM_CHEST_CAPACITY] = chestData.Capacity.ToString();
                    }
                }
            }

            // Activate any lights
            building.ResetLights(building.GetParentLocation());
        }

        public static void ProcessItemConversion(this Building building, ExtendedBuildingItemConversion conversion, ItemQueryContext itemQueryContext, bool isDayStart = false, int minutesElapsed = 0)
        {
            // Handle "minutely" update tracking
            if (conversion.ShouldTrackTime)
            {
                if (isDayStart)
                {
                    conversion.MinutesRemaining = 0;
                }
                else
                {
                    conversion.MinutesRemaining = conversion.MinutesRemaining is null ? conversion.MinutesPerConversion : conversion.MinutesRemaining - minutesElapsed;
                }

                if (conversion.MinutesRemaining.Value > 0)
                {
                    return;
                }
            }

            int convertAmount = 0;
            int currentCount = 0;
            Chest sourceChest = building.GetBuildingChest(conversion.SourceChest);
            Chest destinationChest = building.GetBuildingChest(conversion.DestinationChest);

            if (sourceChest == null)
            {
                return;
            }

            Item inputItem = null;
            foreach (Item item3 in sourceChest.Items)
            {
                if (item3 == null)
                {
                    continue;
                }
                bool fail2 = false;
                foreach (string requiredTag2 in conversion.RequiredTags)
                {
                    if (!item3.HasContextTag(requiredTag2))
                    {
                        fail2 = true;
                        break;
                    }

                    if (inputItem is null)
                    {
                        inputItem = item3;
                    }
                }

                if (fail2)
                {
                    continue;
                }

                currentCount += item3.Stack;
                if (currentCount >= conversion.RequiredCount)
                {
                    int conversions = currentCount / conversion.RequiredCount;
                    if (conversion.MaxDailyConversions >= 0)
                    {
                        conversions = Math.Min(conversions, conversion.MaxDailyConversions - convertAmount);
                    }
                    convertAmount += conversions;
                    currentCount -= conversions * conversion.RequiredCount;
                }
                if (conversion.MaxDailyConversions >= 0 && convertAmount >= conversion.MaxDailyConversions)
                {
                    break;
                }
            }
            if (convertAmount == 0)
            {
                return;
            }
            int totalConversions = 0;
            for (int k = 0; k < convertAmount; k++)
            {
                bool conversionCreatedItem = false;
                for (int i = 0; i < conversion.ProducedItems.Count; i++)
                {
                    ExtendedGenericSpawnItemDataWithCondition producedItemData = conversion.ProducedItems[i];
                    if (building.ValidateConditions(producedItemData.Condition, locationContext: building.GetParentLocation()))
                    {
                        Item outputItem = ItemQueryResolver.TryResolveRandomItem(producedItemData, itemQueryContext);
                        int producedCount = outputItem.Stack;

                        if (outputItem is Object outputObject)
                        {
                            if (producedItemData.CopyPrice)
                            {
                                Object inputObj = inputItem as Object;
                                if (inputObj is not null)
                                {
                                    outputObject.Price = inputObj.Price;
                                }
                            }

                            List<QuantityModifier> priceModifiers = producedItemData.PriceModifiers;
                            if (priceModifiers != null && priceModifiers.Count > 0)
                            {
                                outputObject.Price = (int)Utility.ApplyQuantityModifiers(outputObject.Price, producedItemData.PriceModifiers, producedItemData.PriceModifierMode, itemQueryContext.Location, itemQueryContext.Player, outputItem, inputItem);
                            }
                            else
                            {
                                outputObject.Price = producedItemData.AddPrice + (int)(outputObject.Price * producedItemData.MultiplyPrice);
                            }

                            if (!string.IsNullOrEmpty(producedItemData.PreserveType))
                            {
                                outputObject.preserve.Value = (Object.PreserveType)Enum.Parse(typeof(Object.PreserveType), producedItemData.PreserveType);
                            }
                            if (!string.IsNullOrEmpty(producedItemData.PreserveId))
                            {
                                outputObject.preservedParentSheetIndex.Value = ((!(producedItemData.PreserveId == "DROP_IN")) ? producedItemData.PreserveId : inputItem?.ItemId);
                            }
                        }

                        Item item4 = destinationChest.addItem(outputItem);
                        if (item4 == null || item4.Stack != producedCount)
                        {
                            conversionCreatedItem = true;
                        }
                    }
                }
                if (conversionCreatedItem)
                {
                    totalConversions++;
                }
            }
            if (totalConversions <= 0)
            {
                return;
            }
            int requiredAmount = totalConversions * conversion.RequiredCount;
            for (int j = 0; j < sourceChest.Items.Count; j++)
            {
                Item item2 = sourceChest.Items[j];
                if (item2 == null)
                {
                    continue;
                }
                bool fail = false;
                foreach (string requiredTag in conversion.RequiredTags)
                {
                    if (!item2.HasContextTag(requiredTag))
                    {
                        fail = true;
                        break;
                    }
                }
                if (!fail)
                {
                    int consumedAmount = Math.Min(requiredAmount, item2.Stack);
                    sourceChest.Items[j] = item2.ConsumeStack(consumedAmount);
                    requiredAmount -= consumedAmount;
                    if (requiredAmount <= 0)
                    {
                        break;
                    }
                }
            }
        }

        public static bool IsObjectFilteredForChest(this Building building, Item inputItem, Chest chest, bool performSilentCheck = false)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false || inputItem is null || chest is null)
            {
                return false;
            }

            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);
            foreach (InputFilter inputFilter in extendedModel.InputFilters)
            {
                if (chest.Name != inputFilter.InputChest)
                {
                    continue;
                }

                bool isFiltered = false;
                foreach (RestrictedItem restriction in inputFilter.RestrictedItems)
                {
                    if (restriction.RequiredTags.Any(t => inputItem.HasContextTag(t) is false))
                    {
                        continue;
                    }

                    if (restriction.RejectWhileProcessing && extendedModel.ItemConversions is not null && extendedModel.ItemConversions.Any(c => c.ShouldTrackTime))
                    {
                        isFiltered = true;
                        break;
                    }
                    else if (String.IsNullOrEmpty(restriction.Condition) is false && building.ValidateConditions(restriction.Condition, restriction.ModDataFlags))
                    {
                        isFiltered = true;
                        break;
                    }
                    else if (restriction.ModDataFlags is not null && building.ValidateConditions(restriction.Condition, restriction.ModDataFlags))
                    {
                        isFiltered = true;
                        break;
                    }

                    if (restriction.MaxAllowed >= 0)
                    {
                        int currentTagStackCount = 0;
                        foreach (var chestItem in chest.Items.Where(i => i is not null))
                        {
                            if (restriction.RequiredTags.Any(t => chestItem.HasContextTag(t) is false))
                            {
                                continue;
                            }

                            currentTagStackCount += chestItem.Stack;
                            if (currentTagStackCount > restriction.MaxAllowed)
                            {
                                isFiltered = true;
                                break;
                            }
                        }
                    }
                }

                if (isFiltered)
                {
                    if (performSilentCheck is false && inputFilter.FilteredItemMessage is not null)
                    {
                        Game1.showRedMessage(TokenParser.ParseText(extendedModel.GetTranslation(inputFilter.FilteredItemMessage)));
                    }

                    return true;
                }
            }

            return false;
        }

        public static bool IsAuxiliaryTile(this Building building, Vector2 tileLocation)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return false;
            }

            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);
            if (extendedModel.AuxiliaryHumanDoors.Count == 0)
            {
                return false;
            }

            var tilePoint = new Point((int)tileLocation.X, (int)tileLocation.Y);
            foreach (var door in extendedModel.AuxiliaryHumanDoors)
            {
                if (door.X + building.tileX.Value == tilePoint.X && door.Y + building.tileY.Value == tilePoint.Y)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<LightSource> GetLightSources(this Building building)
        {
            return SolidFoundations.lightManager.GetLightSources(building);
        }

        public static void SetLightSources(this Building building, List<LightSource> lightSources)
        {
            SolidFoundations.lightManager.SetLightSources(building, lightSources);
        }

        public static void ClearLightSources(this Building building, GameLocation gameLocation)
        {
            SolidFoundations.lightManager.ClearLights(building, gameLocation);
        }

        public static void UpdateLightSources(this Building building, GameLocation gameLocation, GameTime time)
        {
            SolidFoundations.lightManager.UpdateLights(building, gameLocation, time);
        }

        public static void ResetLights(this Building building, GameLocation gameLocation)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);

            if (extendedModel.Lights is not null && gameLocation is not null && gameLocation.sharedLights is not null)
            {
                var startingTile = new Point(building.tileX.Value, building.tileY.Value);

                // Clear the current lights
                building.ClearLightSources(gameLocation);

                // Add the required lights
                var lightSources = new List<LightSource>();
                foreach (var lightModel in extendedModel.Lights)
                {
                    var lightTilePosition = lightModel.Tile + startingTile;
                    int lightIdentifier = Toolkit.GetLightSourceIdentifierForBuilding(startingTile, lightSources.Count);
                    if (gameLocation.hasLightSource(lightIdentifier))
                    {
                        gameLocation.removeLightSource(lightIdentifier);
                    }

                    var lightPosition = new Vector2((lightTilePosition.X * 64f) + lightModel.TileOffsetInPixels.X, (lightTilePosition.Y * 64f) + lightModel.TileOffsetInPixels.Y);
                    var lightSource = new LightSource(lightModel.GetTextureSource(), lightPosition, lightModel.GetRadius(), lightModel.GetColor(), lightIdentifier, LightSource.LightContext.None);

                    gameLocation.sharedLights[lightIdentifier] = lightSource;
                    lightSources.Add(lightSource);
                }

                building.SetLightSources(lightSources);
            }
        }
    }
}
