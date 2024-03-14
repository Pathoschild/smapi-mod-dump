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
    using System.Collections.Generic;
    using System.Linq;

    internal class FernAndBurgerLogic
    {
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

            if (!bundle[2].StartsWith("259 1 0") && !bundle[2].Contains(" 259 1 0"))
            {
                bundle[2] += " 259 1 0";
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
            }

            if (config.ForageSurvivalBurger)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/CookingRecipes"))
                {
                    e.Edit((asset) => ApplyCraftingorCookingChanges(asset, translation, true), AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
                {
                    e.Edit((asset) => ApplyCraftingorCookingChanges(asset, translation, false), AssetEditPriority.Late);
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
                    data["Wild Seeds (Su)"] = "396 1 398 1 402 1 259 1" + val[index..];
                }
            }
        }

        private const string FiddleheadFernID = "(O)259";

        private const string SpiceBerryId = "(O)396";
        private const string GrapeID = "(O)398";
        private const string SweetPeaID = "(O)402";
        private const string RedMushroomID = "(O)420";

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
                        locationChangesToApply = new LocationChanges(0.6, 0.0, new HashSet<string>() { SpiceBerryId, GrapeID, SweetPeaID });
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
                        locationChangesToApply = new LocationChanges(0.8, 0.8, new HashSet<string>() { SpiceBerryId, GrapeID, FiddleheadFernID });
                        break;

                    case "Mountain":
                        // before: "396 .5 398 .8"
                        // after: "396 .7 398 .7 259 .8";
                        locationChangesToApply = new LocationChanges(0.7, 0.8, new HashSet<string>() { SpiceBerryId, GrapeID, FiddleheadFernID });
                        break;

                    case "Backwoods":
                        // before: "396 .5 398 .8"
                        // after: "396 .7 398 .7 259 .8";
                        locationChangesToApply = new LocationChanges(0.7, 0.8, new HashSet<string>() { SpiceBerryId, GrapeID, FiddleheadFernID });
                        break;

                    case "Railroad":
                        // before: "396 .4 398 .4 402 .7"
                        // after: "396 .6 398 .6 402 .6";
                        locationChangesToApply = new LocationChanges(0.6, 0.0, new HashSet<string>() { SpiceBerryId, GrapeID, SweetPeaID });
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

        private const string craftingInfix = "/Field/241/false/s Foraging 2/";
        private const string cookingInfix = "/70 1/241 2/s Foraging 2/";

        private const string springCraftingRecipe = "216 1 16 1 20 1 22 1";
        private const string summerCraftingRecipe = "216 1 398 1 396 1 259 1";
        private const string fallCraftingRecipe = "216 1 404 1 406 1 408 1";
        private const string winterCraftingRecipe = "216 1 412 1 414 1 416 1";

        private static void ApplyCraftingorCookingChanges(IAssetData asset, ITranslationHelper translation, bool useCookingRecipes)
        {
            var springBurgerName = translation.Get("SpringBurger");
            var summerBurgerName = translation.Get("SummerBurger");
            var fallBurgerName = translation.Get("FallBurger");
            var winterBurgerName = translation.Get("WinterBurger");

            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            string infix = useCookingRecipes ? cookingInfix : craftingInfix;

            data.Remove("Survival Burger");
            data["Survival Burger (Sp)"] = $"{springCraftingRecipe}{infix}{springBurgerName}";
            data["Survival Burger (Su)"] = $"{summerCraftingRecipe}{infix}{summerBurgerName}";
            data["Survival Burger (Fa)"] = $"{fallCraftingRecipe}{infix}{fallBurgerName}";
            data["Survival Burger (Wi)"] = $"{winterCraftingRecipe}{infix}{winterBurgerName}";
        }

        public static string GetWildSeedSummerForage()
        {
            int ran = Game1.random.Next(4);

            return ran switch
            {
                0 => "(O)259",
                1 => "(O)396",
                2 => "(O)398",
                _ => "(O)402",
            };
        }
    }
}