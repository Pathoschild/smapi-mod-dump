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
    using StardewValley.GameData.Objects;
    using StardewValley.TerrainFeatures;
    using System.Collections.Generic;
    using StardewObject = StardewValley.Object;

    internal class GrapeLogic
    {
        private const string grapeNonQID = "398";
        private const string grapeStarterNonQID = "301";

        public static readonly string fineGrapeNonQID = $"{ForageFantasy.Manifest?.UniqueID}.FineGrape";
        //public static readonly string fineGrapeQualifiedID = $"(O){fineGrapeNonQID}";

        internal static void Apply(AssetRequestedEventArgs e, ForageFantasyConfig config, ITranslationHelper translation)
        {
            if (!config.WildAndFineGrapes)
            {
                return;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

                    if (data.TryGetValue(grapeNonQID, out var grapeData))
                    {
                        grapeData.DisplayName = translation.Get("WildGrape");

                        var fineGrapeData = new ObjectData
                        {
                            Name = fineGrapeNonQID,
                            DisplayName = translation.Get("FineGrape"),
                            Description = grapeData.Description,
                            Type = "Basic",
                            Category = StardewObject.FruitsCategory,
                            Price = 110,
                            Texture = ForageFantasy.FineGrapeAssetPath,
                            SpriteIndex = 0,
                            Edibility = 17,
                            IsDrink = false,
                            Buffs = null,
                            GeodeDropsDefaultItems = false,
                            GeodeDrops = null,
                            ArtifactSpotChances = null,
                            ExcludeFromFishingCollection = false,
                            ExcludeFromShippingCollection = false,
                            ExcludeFromRandomSale = false,
                            ContextTags = new List<string>() { "color_purple", "season_summer" }, // removed forage tag
                            CustomFields = null
                        };

                        data[fineGrapeNonQID] = fineGrapeData;
                    }
                }, AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var giftOptions = new string[] { $" {grapeNonQID} ", $" {grapeNonQID}/", $"/{grapeNonQID} ", $"/{grapeNonQID}/" };
                    var fixedGiftOptions = new string[] { $" {grapeNonQID} {fineGrapeNonQID} ", $" {grapeNonQID} {fineGrapeNonQID}/", $"/{grapeNonQID} {fineGrapeNonQID}", $"/{grapeNonQID} {fineGrapeNonQID}/" };
                    string startOption = $"{grapeNonQID} ";
                    string endOption = $" {grapeNonQID}";

                    foreach (var taste in data)
                    {
                        if (taste.Value.StartsWith(endOption))
                        {
                            data[taste.Key] = $"{fineGrapeNonQID} {taste.Value}";
                        }
                        else if (taste.Value.EndsWith(endOption))
                        {
                            data[taste.Key] = $"{taste.Value} {fineGrapeNonQID}";
                        }
                        else
                        {
                            for (int i = 0; i < giftOptions.Length; i++)
                            {
                                if (taste.Value.Contains(giftOptions[i]))
                                {
                                    data[taste.Key] = taste.Value.Replace(giftOptions[i], fixedGiftOptions[i]);
                                }
                            }
                        }
                    }
                }, AssetEditPriority.Late);
            }
        }

        public static void SetDropToNewGrapes(ForageFantasy mod)
        {
            if (!Context.IsMainPlayer || !mod.Config.WildAndFineGrapes)
            {
                return;
            }

            ReplaceGrapeStarterDrop(fineGrapeNonQID);
        }

        // reset every end of the day, so we don't accidentally save and then permanently edit a crop if the mod gets uninstalled
        public static void ResetGrapes(ForageFantasy mod)
        {
            if (!Context.IsMainPlayer || !mod.Config.WildAndFineGrapes)
            {
                return;
            }

            ReplaceGrapeStarterDrop(grapeNonQID);
        }

        private static void ReplaceGrapeStarterDrop(string id)
        {
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is HoeDirt dirt && dirt?.crop?.netSeedIndex?.Value == grapeStarterNonQID)
                    {
                        dirt.crop.indexOfHarvest.Value = id;
                    }
                }

                return true;
            });
        }
    }
}