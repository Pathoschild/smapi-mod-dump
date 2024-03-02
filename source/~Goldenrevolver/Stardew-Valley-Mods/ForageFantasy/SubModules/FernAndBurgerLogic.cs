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
    using System.Collections.Generic;

    internal class FernAndBurgerLogic
    {
        internal static void Apply(AssetRequestedEventArgs e, ForageFantasyConfig config, ITranslationHelper translation)
        {
            if (config.TapperDaysNeededChangesEnabled && e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit((asset) => ApplyTapperEdits(asset, config), AssetEditPriority.Late);
                return;
            }

            if (config.CommonFiddleheadFern)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
                {
                    e.Edit(ApplyLocationChanges, AssetEditPriority.Late);
                    return;
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
                    e.Edit((asset) => ApplyCookingChanges(asset, translation), AssetEditPriority.Late);
                }
                else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
                {
                    e.Edit((asset) => ApplyCraftingChanges(asset, translation), AssetEditPriority.Late);
                }
            }
        }

        private static void ApplyTapperEdits(IAssetData asset, ForageFantasyConfig config)
        {
                    /*  here is the reasoning for the math

                    normal tapper:
                    maple syrup 9 days 200g
                    oak resin 7 days 150g
                    pine tar 5 days 100g

                    so 22,2g per day, 21,4g per day, 20g per day

                    heavy tapper:
                    maple syrup 4 days 200g
                    oak resin 3 days 150g
                    pine tar 2 days 100g

                    so 50g per day for all of them

                    ----

                    wanted values:
                    maple syrup 7 days 150g
                    oak resin 7 days 150g
                    pine tar 7 days 150g

                    so the calculation is:
                    newSellPrice = (int)Math.Round(daysNeeded * (150f / 7f), MidpointRounding.AwayFromZero);
                 */

            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

            var priceChanges = new Dictionary<int, int>() { { 724, config.MapleDaysNeeded }, { 725, config.OakDaysNeeded }, { 726, config.PineDaysNeeded } };

            foreach (var item in priceChanges)
                {
                    var entry = data[item.Key];
                    var fields = entry.Split('/', 3);
                    var newPrice = TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(item.Value);
                    fields[1] = newPrice.ToString();
                    data[item.Key] = string.Join('/', fields);
                }
        }

        private static void ApplyCommonFernCrafting(IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            var entry = data["Wild Seeds (Su)"];
            var fields = entry.Split('/', 2);
            fields[0] = "396 1 398 1 402 1 259 1";
            data["Wild Seeds (Su)"] = string.Join('/', fields);
        }

        private static void ApplyLocationChanges(IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            foreach (var (location, value) in data)
            {
                var fields = value.Split('/', 3);
                switch (location)
                {
                    case "BusStop":
                        fields[1] = "396 .6 398 .6 402 .6";
                        break;

                    case "Forest":
                        fields[1] = "396 .8 398 .8 259 .8";
                        break;

                    case "Mountain":
                        fields[1] = "396 .7 398 .7 259 .8";
                        break;

                    case "Backwoods":
                        fields[1] = "396 .7 398 .7 259 .8";
                        break;

                    case "Railroad":
                        fields[1] = "396 .6 398 .6 402 .6";
                        break;

                    case "Woods":
                        fields[1] = "259 .7 420 .7";
                        break;
                }
                data[location] = string.Join('/', fields);
            }
        }

        private static void ApplyCookingChanges(IAssetData asset, ITranslationHelper translation)
        {
            var spring = translation.Get("SpringBurger");
            var summer = translation.Get("SummerBurger");
            var fall = translation.Get("FallBurger");
            var winter = translation.Get("WinterBurger");

            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            data.Remove("Survival Burger");
            data.Add("Survival Burger (Sp)", $"216 1 16 1 20 1 22 1/70 1/241 2/s Foraging 2/{spring}");
            data.Add("Survival Burger (Su)", $"216 1 398 1 396 1 259 1/70 1/241 2/s Foraging 2/{summer}");
            data.Add("Survival Burger (Fa)", $"216 1 404 1 406 1 408 1/70 1/241 2/s Foraging 2/{fall}");
            data.Add("Survival Burger (Wi)", $"216 1 412 1 414 1 416 1/70 1/241 2/s Foraging 2/{winter}");
        }

        private static void ApplyCraftingChanges(IAssetData asset, ITranslationHelper translation)
        {
            var spring = translation.Get("SpringBurger");
            var summer = translation.Get("SummerBurger");
            var fall = translation.Get("FallBurger");
            var winter = translation.Get("WinterBurger");

            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            data.Add("Survival Burger (Sp)", $"216 1 16 1 20 1 22 1/Field/241/false/s Foraging 2/{spring}");
            data.Add("Survival Burger (Su)", $"216 1 398 1 396 1 259 1/Field/241/false/s Foraging 2/{summer}");
            data.Add("Survival Burger (Fa)", $"216 1 404 1 406 1 408 1/Field/241/false/s Foraging 2/{fall}");
            data.Add("Survival Burger (Wi)", $"216 1 412 1 414 1 416 1/Field/241/false/s Foraging 2/{winter}");
        }

        public static void ChangeBundle(ForageFantasy mod)
        {
            if (!mod.Config.CommonFiddleheadFern)
            {
                return;
            }

            Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;

            // Summer Foraging
            string key = "Crafts Room/14";

            string[] bundle = bundleData[key].Split('/', 3);

            if (!bundle[2].Contains("259 1 0"))
            {
                bundle[2] += " 259 1 0";
            }

            bundleData[key] = string.Join('/', bundle);
        }

        public static int GetWildSeedSummerForage()
        {
            int ran = Game1.random.Next(4);

            return ran switch
            {
                0 => 259,
                1 => 396,
                2 => 398,
                _ => 402,
            };
        }
    }
}