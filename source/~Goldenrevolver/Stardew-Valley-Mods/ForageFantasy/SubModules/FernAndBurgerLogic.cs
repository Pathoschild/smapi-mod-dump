/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.GameData;
    using StardewValley.GameData.Locations;
    using StardewValley.GameData.Objects;
    using System.Collections.Generic;
    using System.Linq;

    internal class FernAndBurgerLogic
    {
        private const string SurvivalBurgerNonQID = "241";

        private const string FiddleheadFernNonQID = "259";
        private const string FiddleheadFernID = $"(O){FiddleheadFernNonQID}";

        private const string SpiceBerryID = "(O)396";
        private const string GrapeID = "(O)398";
        private const string SweetPeaID = "(O)402";
        private const string RedMushroomID = "(O)420";

        public static void UpdateExistingBundle(ForageFantasy mod)
        {
            if (!mod.Config.CommonFiddleheadFern)
            {
                return;
            }

            Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;

            ApplyBundleChanges(bundleData);
        }

        private static void UpdateNewBundle(IAssetData asset)
        {
            IDictionary<string, string> bundleData = asset.AsDictionary<string, string>().Data;

            ApplyBundleChanges(bundleData);
        }

        private static void ApplyBundleChanges(IDictionary<string, string> bundleData)
        {
            // Summer Foraging
            string key = "Crafts Room/14";

            string[] bundle = bundleData[key].Split('/');

            if (!bundle[2].StartsWith($"{FiddleheadFernNonQID} 1 0") && !bundle[2].Contains($" {FiddleheadFernNonQID} 1 0"))
            {
                bundle[2] += $" {FiddleheadFernNonQID} 1 0";
            }

            bundleData[key] = string.Join('/', bundle);
        }

        internal static void Apply(AssetRequestedEventArgs e, ForageFantasyConfig config, ITranslationHelper translation)
        {
            if (config.CommonFiddleheadFern)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Bundles"))
                {
                    e.Edit(UpdateNewBundle, AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
                {
                    e.Edit(ApplyLocationChanges, AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
                {
                    e.Edit(ApplyCommonFernCrafting, AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
                {
                    e.Edit((asset) => ApplyCompatibilityPriceChange(asset, FiddleheadFernNonQID, "CommonFiddleheadFernPrice"), AssetEditPriority.Late);
                }
            }

            if (config.ForageSurvivalBurger)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/CookingRecipes"))
                {
                    e.Edit((asset) => ApplyCraftingOrCookingChanges(asset, translation, true), AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
                {
                    e.Edit((asset) => ApplyCraftingOrCookingChanges(asset, translation, false), AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
                {
                    e.Edit((asset) => ApplyCompatibilityPriceChange(asset, SurvivalBurgerNonQID, "ForageSurvivalBurgerPrice"), AssetEditPriority.Late);
                }
            }
        }

        private static void ApplyCompatibilityPriceChange(IAssetData asset, string itemId, string customFieldName)
        {
            IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

            if (!data.TryGetValue(itemId, out var itemData))
            {
                return;
            }

            if (itemData.CustomFields != null)
            {
                if (itemData.CustomFields.TryGetValue($"{ForageFantasy.Manifest?.UniqueID}.{customFieldName}", out string priceString) && int.TryParse(priceString, out int priceOverride))
                {
                    itemData.Price = priceOverride;
                }
            }
        }

        private static void ApplyCommonFernCrafting(IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            if (data.TryGetValue("Wild Seeds (Su)", out var val))
            {
                var index = val.IndexOf('/');

                if (index > 0)
                {
                    data["Wild Seeds (Su)"] = $"{SpiceBerryID} 1 {GrapeID} 1 {SweetPeaID} 1 {FiddleheadFernID} 1" + val[index..];
                }
            }
        }

        private class LocationChanges
        {
            internal HashSet<string> idsToCheck;
            internal double summerForageChance;
            internal double newfiddleheadChance;

            public LocationChanges(double summerForageChance, double newfiddleheadChance, HashSet<string> idsToCheck)
            {
                this.idsToCheck = idsToCheck;
                this.summerForageChance = summerForageChance;
                this.newfiddleheadChance = newfiddleheadChance;
            }
        }

        private static void ApplyLocationChanges(IAssetData asset)
        {
            IDictionary<string, LocationData> data = asset.AsDictionary<string, LocationData>().Data;

            foreach (var location in data)
            {
                LocationChanges locationChangesToApply = null;

                switch (location.Key)
                {
                    case "BusStop":

                        // before: "396 .4 398 .4 402 .7"
                        // after: "396 .6 398 .6 402 .6";
                        locationChangesToApply = new LocationChanges(0.6, 0.0, new HashSet<string>() { SpiceBerryID, GrapeID, SweetPeaID });
                        break;

                    case "Forest":

                        int foundSweetPeaIndex = -1;
                        int foundGrapeIndex = -1;

                        // we replace sweet peas with grapes, if found
                        for (int i = 0; i < location.Value.Forage.Count; i++)
                        {
                            SpawnForageData forage = location.Value.Forage[i];

                            if (forage.Season != Season.Summer)
                            {
                                continue;
                            }

                            if (forage.Id == SweetPeaID)
                            {
                                foundSweetPeaIndex = i;
                            }
                            else if (forage.Id == GrapeID)
                            {
                                foundGrapeIndex = i;
                            }
                        }

                        if (foundSweetPeaIndex != -1 && foundGrapeIndex == -1)
                        {
                            location.Value.Forage[foundSweetPeaIndex].Id = GrapeID;
                            location.Value.Forage[foundSweetPeaIndex].ItemId = GrapeID;
                        }

                        // before: "396 .6 402 .9"
                        // after: "396 .8 398 .8 259 .8";
                        locationChangesToApply = new LocationChanges(0.8, 0.8, new HashSet<string>() { SpiceBerryID, GrapeID, FiddleheadFernID });
                        break;

                    case "Mountain":
                        // before: "396 .5 398 .8"
                        // after: "396 .7 398 .7 259 .8";
                        locationChangesToApply = new LocationChanges(0.7, 0.8, new HashSet<string>() { SpiceBerryID, GrapeID, FiddleheadFernID });
                        break;

                    case "Backwoods":
                        // before: "396 .5 398 .8"
                        // after: "396 .7 398 .7 259 .8";
                        locationChangesToApply = new LocationChanges(0.7, 0.8, new HashSet<string>() { SpiceBerryID, GrapeID, FiddleheadFernID });
                        break;

                    case "Railroad":
                        // before: "396 .4 398 .4 402 .7"
                        // after: "396 .6 398 .6 402 .6";
                        locationChangesToApply = new LocationChanges(0.6, 0.0, new HashSet<string>() { SpiceBerryID, GrapeID, SweetPeaID });
                        break;

                    case "Woods":
                        // before: "259 .9 420 .25"
                        // after: "259 .7 420 .7";
                        locationChangesToApply = new LocationChanges(0.7, 0.7, new HashSet<string>() { FiddleheadFernID, RedMushroomID });
                        break;
                }

                if (locationChangesToApply == null)
                {
                    continue;
                }

                var forages = location.Value.Forage.Where((f) => f.Season == Season.Summer && locationChangesToApply.idsToCheck.Contains(f.Id));

                bool foundFiddle = false;
                foreach (var forage in forages)
                {
                    forage.Chance = locationChangesToApply.summerForageChance;

                    if (forage.Id == FiddleheadFernID)
                    {
                        foundFiddle = true;
                    }
                }

                if (!foundFiddle && locationChangesToApply.newfiddleheadChance > 0.0)
                {
                    location.Value.Forage.Add(CreateFiddleheadForageData(locationChangesToApply.newfiddleheadChance));
                }
            }
        }

        private static SpawnForageData CreateFiddleheadForageData(double spawnChance)
        {
            return new SpawnForageData
            {
                Id = FiddleheadFernID,
                ItemId = FiddleheadFernID,
                Season = Season.Summer,
                Condition = null,
                RandomItemId = null,
                MaxItems = null,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                PerItemCondition = null,
                StackModifiers = null,
                QualityModifiers = null,
                StackModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                QualityModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                ToolUpgradeLevel = -1,
                Quality = -1,
                MinStack = -1,
                MaxStack = -1,
                IsRecipe = false,
                Chance = spawnChance
            };
        }

        // revert forage level increase
        private const string craftingInfix = $"/Field/{SurvivalBurgerNonQID}/ false/s Foraging 2/";

        // revert forage level increase
        private const string cookingInfix = $"/70 1/{SurvivalBurgerNonQID} 2/s Foraging 3/";

        private const string springCraftingRecipe = "216 1 16 n 20 n 22 n";
        private const string summerCraftingRecipe = "216 1 398 n 396 n 259 n";
        private const string fallCraftingRecipe = "216 1 404 n 406 n 408 n";
        private const string winterCraftingRecipe = "216 1 412 n 414 n 416 n";

        private static void ApplyCraftingOrCookingChanges(IAssetData asset, ITranslationHelper translation, bool useCookingRecipes)
        {
            var springBurgerName = translation.Get("SpringBurger");
            var summerBurgerName = translation.Get("SummerBurger");
            var fallBurgerName = translation.Get("FallBurger");
            var winterBurgerName = translation.Get("WinterBurger");

            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            string infix = useCookingRecipes ? cookingInfix : craftingInfix;

            data.Remove("Survival Burger");
            data["Survival Burger (Sp)"] = $"{springCraftingRecipe.Replace('n', useCookingRecipes ? '2' : '1')}{infix}{springBurgerName}";
            data["Survival Burger (Su)"] = $"{summerCraftingRecipe.Replace('n', useCookingRecipes ? '2' : '1')}{infix}{summerBurgerName}";
            data["Survival Burger (Fa)"] = $"{fallCraftingRecipe.Replace('n', useCookingRecipes ? '2' : '1')}{infix}{fallBurgerName}";
            data["Survival Burger (Wi)"] = $"{winterCraftingRecipe.Replace('n', useCookingRecipes ? '2' : '1')}{infix}{winterBurgerName}";
        }

        public static void AllowSurvivalBurgerRecipeCompletion()
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                if (farmer.cookingRecipes.ContainsKey("Survival Burger") && !HasCookedRecipe(farmer, "Survival Burger"))
                {
                    if (HasCookedRecipe(farmer, "Survival Burger (Sp)")
                        || HasCookedRecipe(farmer, "Survival Burger (Su)")
                        || HasCookedRecipe(farmer, "Survival Burger (Fa)")
                        || HasCookedRecipe(farmer, "Survival Burger (Wi)"))
                    {
                        farmer.recipesCooked["Survival Burger"] = 1;
                    }
                }

                // this one technically doesn't exist, but better safe than sorry
                if (farmer.craftingRecipes.TryGetValue("Survival Burger", out var timesCrafted) && timesCrafted <= 0)
                {
                    if (HasCraftedRecipe(farmer, "Survival Burger (Sp)")
                        || HasCraftedRecipe(farmer, "Survival Burger (Su)")
                        || HasCraftedRecipe(farmer, "Survival Burger (Fa)")
                        || HasCraftedRecipe(farmer, "Survival Burger (Wi)"))
                    {
                        farmer.craftingRecipes["Survival Burger"] = 1;
                    }
                }
            }
        }

        private static bool HasCookedRecipe(Farmer farmer, string recipe)
        {
            return farmer.recipesCooked.TryGetValue(recipe, out var timesCooked) && timesCooked > 0;
        }

        private static bool HasCraftedRecipe(Farmer farmer, string recipe)
        {
            return farmer.craftingRecipes.TryGetValue(recipe, out var timesCrafted) && timesCrafted > 0;
        }

        public static string GetWildSeedSummerForage()
        {
            int ran = Game1.random.Next(4);

            return ran switch
            {
                0 => FiddleheadFernID,
                1 => SpiceBerryID,
                2 => GrapeID,
                _ => SweetPeaID,
            };
        }
    }
}