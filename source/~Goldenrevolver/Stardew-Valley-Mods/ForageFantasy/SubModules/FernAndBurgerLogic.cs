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
    using StardewValley;
    using System.Collections.Generic;
    using System.Linq;

    internal class FernAndBurgerLogic
    {
        public static bool CanEdit<T>(IAssetInfo asset, ForageFantasyConfig config)
        {
            if (config.TapperDaysNeededChangesEnabled && asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }

            if (config.CommonFiddleheadFern)
            {
                if (asset.AssetNameEquals("Data/Locations"))
                {
                    return true;
                }
                else if (asset.AssetNameEquals("Data/CraftingRecipes"))
                {
                    return true;
                }
            }

            if (config.ForageSurvivalBurger)
            {
                if (asset.AssetNameEquals("Data/CookingRecipes"))
                {
                    return true;
                }
                else if (asset.AssetNameEquals("Data/CraftingRecipes"))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Edit<T>(IAssetData asset, ForageFantasy mod)
        {
            if (mod.Config.TapperDaysNeededChangesEnabled && asset.AssetNameEquals("Data/ObjectInformation"))
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

                var priceChanges = new Dictionary<int, int>() { { 724, mod.Config.MapleDaysNeeded }, { 725, mod.Config.OakDaysNeeded }, { 726, mod.Config.PineDaysNeeded } };

                foreach (var item in priceChanges)
                {
                    var entry = data[item.Key];
                    var fields = entry.Split('/');
                    var newPrice = TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(item.Value);
                    fields[1] = newPrice.ToString();
                    data[item.Key] = string.Join("/", fields);
                }
            }

            if (mod.Config.CommonFiddleheadFern)
            {
                if (asset.AssetNameEquals("Data/CraftingRecipes"))
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var entry = data["Wild Seeds (Su)"];
                    var fields = entry.Split('/');
                    fields[0] = "396 1 398 1 402 1 259 1";
                    data["Wild Seeds (Su)"] = string.Join("/", fields);
                }

                if (asset.AssetNameEquals("Data/Locations"))
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var keys = data.Keys.ToList();

                    for (int i = 0; i < keys.Count; i++)
                    {
                        string location = keys[i];
                        string[] fields = data[location].Split('/');

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

                        data[location] = string.Join("/", fields);
                    }
                }
            }

            if (mod.Config.ForageSurvivalBurger)
            {
                var spring = mod.Helper.Translation.Get("SpringBurger");
                var summer = mod.Helper.Translation.Get("SummerBurger");
                var fall = mod.Helper.Translation.Get("FallBurger");
                var winter = mod.Helper.Translation.Get("WinterBurger");

                if (asset.AssetNameEquals("Data/CookingRecipes"))
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    data.Remove("Survival Burger");
                    data.Add("Survival Burger (Sp)", $"216 1 16 1 20 1 22 1/70 1/241 2/s Foraging 2/{spring}");
                    data.Add("Survival Burger (Su)", $"216 1 398 1 396 1 259 1/70 1/241 2/s Foraging 2/{summer}");
                    data.Add("Survival Burger (Fa)", $"216 1 404 1 406 1 408 1/70 1/241 2/s Foraging 2/{fall}");
                    data.Add("Survival Burger (Wi)", $"216 1 412 1 414 1 416 1/70 1/241 2/s Foraging 2/{winter}");
                }

                if (asset.AssetNameEquals("Data/CraftingRecipes"))
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    data.Add("Survival Burger (Sp)", $"216 1 16 1 20 1 22 1/Field/241/false/s Foraging 2/{spring}");
                    data.Add("Survival Burger (Su)", $"216 1 398 1 396 1 259 1/Field/241/false/s Foraging 2/{summer}");
                    data.Add("Survival Burger (Fa)", $"216 1 404 1 406 1 408 1/Field/241/false/s Foraging 2/{fall}");
                    data.Add("Survival Burger (Wi)", $"216 1 412 1 414 1 416 1/Field/241/false/s Foraging 2/{winter}");
                }
            }
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

            string[] bundle = bundleData[key].Split('/');

            if (!bundle[2].Contains("259 1 0"))
            {
                bundle[2] += " 259 1 0";
            }

            bundleData[key] = string.Join("/", bundle);
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