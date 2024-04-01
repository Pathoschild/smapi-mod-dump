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
    using StardewValley.GameData.BigCraftables;
    using StardewValley.GameData.Machines;
    using StardewValley.GameData.Objects;
    using StardewValley.GameData.WildTrees;
    using StardewValley.TerrainFeatures;
    using System.Collections.Generic;

    internal class TapperAssetChanges
    {
        private static readonly HashSet<string> tappers = new();

        internal static void Apply(AssetRequestedEventArgs e, ForageFantasyConfig config)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit((asset) => CheckForTappers(asset), AssetEditPriority.Late + 1);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit((asset) => ApplyTapperMachineEdits(asset, config), AssetEditPriority.Late);
            }

            if (!config.TapperDaysNeededChangesEnabled)
            {
                return;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit((asset) => ApplyTapperProductPriceEdits(asset, config), AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/WildTrees"))
            {
                e.Edit((asset) => ApplyTapperDurationEdits(asset, config), AssetEditPriority.Late);
            }
        }

        private static void CheckForTappers(IAssetData asset)
        {
            IDictionary<string, BigCraftableData> data = asset.AsDictionary<string, BigCraftableData>().Data;

            tappers.Clear();

            foreach (KeyValuePair<string, BigCraftableData> item in data)
            {
                if (item.Value.ContextTags == null)
                {
                    continue;
                }

                foreach (var tag in item.Value.ContextTags)
                {
                    if (ItemContextTagManager.SanitizeContextTag(tag) == "tapper_item")
                    {
                        tappers.Add(item.Key);
                    }
                }
            }
        }

        private static void ApplyTapperMachineEdits(IAssetData asset, ForageFantasyConfig config)
        {
            IDictionary<string, MachineData> data = asset.AsDictionary<string, MachineData>().Data;

            foreach (KeyValuePair<string, MachineData> item in data)
            {
                // remove "(BC)" prefix
                if (item.Key.StartsWith("(BC)") && tappers.Contains(item.Key.Remove(0, 4)))
                {
                    int exp = config.TapperXPAmount < 0 ? 0 : config.TapperXPAmount;

                    item.Value.ExperienceGainOnHarvest = $"Foraging {exp}";
                }
            }
        }

        private static void ApplyTapperDurationEdits(IAssetData asset, ForageFantasyConfig config)
        {
            IDictionary<string, WildTreeData> data = asset.AsDictionary<string, WildTreeData>().Data;

            if (data.TryGetValue(Tree.bushyTree, out var oakData))
            {
                foreach (var item in oakData.TapItems)
                {
                    if (item.Id == "Default")
                    {
                        item.DaysUntilReady = config.OakTapperDaysNeeded;
                    }
                }
            }

            if (data.TryGetValue(Tree.leafyTree, out var mapleData))
            {
                foreach (var item in mapleData.TapItems)
                {
                    if (item.Id == "Default")
                    {
                        item.DaysUntilReady = config.MapleTapperDaysNeeded;
                    }
                }
            }

            if (data.TryGetValue(Tree.pineTree, out var pineData))
            {
                foreach (var item in pineData.TapItems)
                {
                    if (item.Id == "Default")
                    {
                        item.DaysUntilReady = config.PineTapperDaysNeeded;
                    }
                }
            }

            if (config.MushroomTreeTappersConsistentDaysNeeded && data.TryGetValue(Tree.mushroomTree, out var mushroomData))
            {
                WildTreeTapItemData redTap = null;
                WildTreeTapItemData purpleTap = null;
                WildTreeTapItemData fallTap = null;
                WildTreeTapItemData defaultTap = null;

                foreach (var item in mushroomData.TapItems)
                {
                    switch (item.Id)
                    {
                        case "Default":
                            defaultTap = item;
                            break;

                        case "Initial_Fall":
                            fallTap = item;
                            break;

                        case "Day10Or20":
                            purpleTap = item;
                            break;

                        case "FromPurpleMushroom":
                            redTap = item;
                            break;
                    }
                }

                redTap.Id = "RedMushroomDay";
                purpleTap.Id = "PurpleMushroomDay";
                fallTap.Id = "FallDefault";
                defaultTap.Id = "Default";

                // if mods changed the output rules before, I will not overwrite them again
                //redTap.ItemId = "(O)420";
                //purpleTap.ItemId = "(O)422";
                //fallTap.ItemId = "(O)420";
                //defaultTap.ItemId = "(O)404";

                redTap.DaysUntilReady = 2;
                purpleTap.DaysUntilReady = 2;
                fallTap.DaysUntilReady = 2;
                defaultTap.DaysUntilReady = 2;

                redTap.PreviousItemId = null;
                purpleTap.PreviousItemId = null;
                fallTap.PreviousItemId = null;
                defaultTap.PreviousItemId = null;

                redTap.Condition = "DAY_OF_MONTH 2 12 22";
                purpleTap.Condition = "DAY_OF_MONTH 10 20 28";
                fallTap.Condition = null;
                defaultTap.Condition = null;

                if (mushroomData.IsStumpDuringWinter)
                {
                    redTap.Condition += ", !LOCATION_SEASON Target Winter";
                    purpleTap.Condition += ", !LOCATION_SEASON Target Winter";
                    defaultTap.Condition = "!LOCATION_SEASON Target Winter";
                }

                mushroomData.TapItems = new List<WildTreeTapItemData> { redTap, purpleTap, fallTap, defaultTap };
            }
        }

        private static void ApplyTapperProductPriceEdits(IAssetData asset, ForageFantasyConfig config)
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

            IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

            var priceChanges = new Dictionary<string, int>()
            {
                { "724", config.MapleTapperDaysNeeded },
                { "725", config.OakTapperDaysNeeded },
                { "726", config.PineTapperDaysNeeded }
            };

            foreach (var item in priceChanges)
            {
                if (data.TryGetValue(item.Key, out var objectData))
                {
                    objectData.Price = TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(item.Value);
                }
            }
        }
    }
}