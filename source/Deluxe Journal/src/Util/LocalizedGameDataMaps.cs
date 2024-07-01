/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using DeluxeJournal.Task;

namespace DeluxeJournal.Util
{
    /// <summary>Provides a collection of <see cref="LocalizedGameDataMap{T}"/> objects.</summary>
    public sealed class LocalizedGameDataMaps(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor = null)
    {
        /// <inheritdoc cref="LocalizedItemMap"/>
        public LocalizedGameDataMap LocalizedItems { get; } = new LocalizedItemMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedNpcMap"/>
        public LocalizedGameDataMap LocalizedNpcs { get; } = new LocalizedNpcMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedBuildingMap"/>
        public LocalizedGameDataMap LocalizedBuildings { get; } = new LocalizedBuildingMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedFarmAnimalMap"/>
        public LocalizedGameDataMap LocalizedFarmAnimals { get; } = new LocalizedFarmAnimalMap(translation, settings, monitor);

        public LocalizedGameDataMaps(ITranslationHelper translation, IMonitor? monitor = null)
            : this(translation, new(), monitor)
        {
        }

        /// <summary>Maps localized item names to qualified item ID's.</summary>
        private class LocalizedItemMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.items.", translation, settings, monitor)
        {
            private static readonly HashSet<string> IgnoredItemIds = ["30", "73", "858", "892", "922", "923", "924", "925", "927", "929", "930", "DriedFruit", "DriedMushrooms", "SmokedFish"];
            private static readonly HashSet<string> IgnoredItemTypes = ["Litter", "Quest", "asdf", "interactive"];

            protected override void PopulateDataMap()
            {
                if (Settings.AllItemsEnabled || Settings.SetItemCategoryObject || Settings.SetItemCategoryCraftable)
                {
                    HashSet<string> roeFishNames = DataLoader.FishPondData(Game1.content)
                        .Where(data => data.ProducedItems.Any(reward => reward.ItemId == "(O)812"))
                        .Select(data => data.Id)
                        .ToHashSet();

                    AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12852"), SObject.FishCategory.ToString());

                    foreach (KeyValuePair<string, ObjectData> pair in Game1.objectData)
                    {
                        if (!IgnoredItemIds.Contains(pair.Key) && pair.Value != null && !IgnoredItemTypes.Contains(pair.Value.Type))
                        {
                            if (!Settings.SetItemCategoryObject && Settings.SetItemCategoryCraftable
                                && !CraftingRecipe.craftingRecipes.ContainsKey(pair.Value.Name))
                            {
                                continue;
                            }

                            if (TokenParser.ParseText(pair.Value.DisplayName) is string parsedName)
                            {
                                string itemId = ItemRegistry.type_object + pair.Key;

                                AddPlural(parsedName, itemId);
                                AddFlavored(parsedName, pair.Key, pair.Value, roeFishNames);
                                AddConvenienceAlternates(itemId, pair.Value);
                            }
                        }
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryBigCraftable)
                {
                    foreach (KeyValuePair<string, BigCraftableData> pair in Game1.bigCraftableData)
                    {
                        AddPlural(TokenParser.ParseText(pair.Value?.DisplayName), ItemRegistry.type_bigCraftable + pair.Key);
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryTool)
                {
                    foreach (string toolId in Game1.toolData.Keys)
                    {
                        string toolQid = ItemRegistry.type_tool + toolId;

                        if (ItemRegistry.GetData(toolQid) is ParsedItemData itemData)
                        {
                            if (itemData.RawData is ToolData toolData
                                && toolData.ApplyUpgradeLevelToDisplayName
                                && toolData.UpgradeLevel > 0
                                && ItemRegistry.Create(toolQid) is Tool tool)
                            {
                                Add(tool.DisplayName.ToLower(), toolQid);
                            }
                            else
                            {
                                Add(itemData.DisplayName.ToLower(), toolQid);
                            }
                        }
                    }

                    Add(TokenParser.ParseText("[LocalizedText Strings\\1_6_Strings:PanToolBaseName]"), "(T)Pan");
                    Add(TokenParser.ParseText("[LocalizedText Strings\\\\StringsFromCSFiles:TrashCan]"), "(T)CopperTrashCan");
                }

                if (Settings.AllItemsEnabled)
                {
                    foreach (KeyValuePair<string, WeaponData> pair in Game1.weaponData)
                    {
                        Add(TokenParser.ParseText(pair.Value?.DisplayName), ItemRegistry.type_weapon + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in DataLoader.Boots(Game1.content))
                    {
                        string[] fields = pair.Value.Split('/');
                        Add(ArgUtility.Get(fields, fields.Length < 7 ? 0 : 6), ItemRegistry.type_boots + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in DataLoader.Hats(Game1.content))
                    {
                        Add(ArgUtility.Get(pair.Value.Split('/'), 5), ItemRegistry.type_hat + pair.Key);
                    }
                }
            }

            /// <summary>Populate data map with flavored items.</summary>
            /// <param name="localizedName">Localized ingredient name.</param>
            /// <param name="unqualifiedId">Unqualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            /// <param name="roeFishNames">Set of fish (internal) names that may produce roe.</param>
            private void AddFlavored(string localizedName, string unqualifiedId, ObjectData objectData, IReadOnlySet<string> roeFishNames)
            {
                // Wild Honey
                if (unqualifiedId == "340")
                {
                    Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12750"), ItemRegistry.type_object + unqualifiedId);
                    return;
                }

                switch (objectData.Category)
                {
                    case SObject.FruitsCategory:
                        // Jelly
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12739", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)344", unqualifiedId));

                        // Wine
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12730", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)348", unqualifiedId));

                        // Dried Fruit (not grapes)
                        if (unqualifiedId != "398")
                        {
                            AddPlural(Game1.content.LoadString("Strings\\1_6_Strings:DriedFruit_DisplayName", localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedFruit", unqualifiedId));
                        }
                        break;
                    case SObject.GreensCategory when objectData.ContextTags?.Contains("edible_mushroom") == true:
                        //Dried Mushrooms
                        AddPlural(Game1.content.LoadString("Strings\\1_6_Strings:DriedFruit_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedMushrooms", unqualifiedId));
                        break;
                    case SObject.GreensCategory when objectData.ContextTags?.Contains("preserves_pickle") == true:
                        // Special Pickles
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12735", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", unqualifiedId));
                        break;
                    case SObject.VegetableCategory:
                        // Pickles
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12735", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", unqualifiedId));

                        // Juice
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12726", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)350", unqualifiedId));
                        break;
                    case SObject.flowersCategory:
                        // Honey
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12760", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)340", unqualifiedId));
                        break;
                    case SObject.FishCategory:
                        if (roeFishNames.Contains(objectData.Name))
                        {
                            // Roe
                            Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Roe_DisplayName", localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)812", unqualifiedId));

                            // Aged Roe (not sturgeon)
                            if (unqualifiedId != "698")
                            {
                                Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:AgedRoe_DisplayName", localizedName),
                                    FlavoredItemHelper.EncodeFlavoredItemId("(O)447", unqualifiedId));
                            }
                        }

                        // Bait
                        Add(Game1.content.LoadString("Strings\\1_6_Strings:SpecificBait_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SpecificBait", unqualifiedId));

                        // Smoked Fish
                        AddPlural(Game1.content.LoadString("Strings\\1_6_Strings:SmokedFish_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SmokedFish", unqualifiedId));
                        break;
                }
            }

            /// <summary>Add alternative item ID matches to specific translation groups for convenience.</summary>
            /// <param name="itemId">Qualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            private void AddConvenienceAlternates(string itemId, ObjectData objectData)
            {
                if (objectData.ContextTags is not List<string> contextTags)
                {
                    return;
                }

                switch (objectData.Category)
                {
                    case SObject.EggCategory when contextTags.Contains("large_egg_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:WhiteEgg_Name]"), itemId);
                        break;
                    case SObject.MilkCategory when contextTags.Contains("cow_milk_item") && contextTags.Contains("large_milk_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:Milk_Name]"), itemId);
                        break;
                    case SObject.MilkCategory when contextTags.Contains("goat_milk_item") && contextTags.Contains("large_milk_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:GoatMilk_Name]"), itemId);
                        break;
                }
            }
        }

        /// <summary>Maps localized character names to their internal names.</summary>
        private class LocalizedNpcMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.npcs.", translation, settings, monitor)
        {
            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, CharacterData> pair in Game1.characterData)
                {
                    if (pair.Value?.CanReceiveGifts == true && Game1.NPCGiftTastes.ContainsKey(pair.Key))
                    {
                        Add(TokenParser.ParseText(pair.Value.DisplayName) ?? pair.Key, pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized building names to their internal names.</summary>
        private class LocalizedBuildingMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.buildings.", translation, settings, monitor)
        {
            private static readonly HashSet<string> IgnoredBuildingNames = ["Greenhouse"];

            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, BuildingData> pair in Game1.buildingData)
                {
                    if (!IgnoredBuildingNames.Contains(pair.Key))
                    {
                        AddPlural(TokenParser.ParseText(pair.Value?.Name) ?? pair.Key, pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized farm animal shop names to their internal names, including alterative variants.</summary>
        private class LocalizedFarmAnimalMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.animalshop.", translation, settings, monitor)
        {
            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, FarmAnimalData> pair in Game1.farmAnimalData)
                {
                    if (pair.Value?.ShopDisplayName is string shopDisplayName)
                    {
                        string localizedKey = TokenParser.ParseText(shopDisplayName) ?? pair.Key;

                        AddPlural(localizedKey, pair.Key);

                        if (pair.Value.AlternatePurchaseTypes is List<AlternatePurchaseAnimals> alternates)
                        {
                            foreach (var alternate in alternates)
                            {
                                foreach (string id in alternate.AnimalIds)
                                {
                                    AddPlural(localizedKey, id);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
