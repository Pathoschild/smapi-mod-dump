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
    using StardewValley.GameData.Machines;
    using StardewValley.GameData.Objects;
    using StardewValley.TerrainFeatures;
    using System.Collections.Generic;
    using System.Linq;
    using StardewObject = StardewValley.Object;

    internal class GrapeLogic
    {
        private const string grapeNonQID = "398";
        private const string grapeStarterNonQID = "301";

        public static readonly string fineGrapeNonQID = $"{ForageFantasy.Manifest?.UniqueID}.FineGrape";

        internal static void Apply(AssetRequestedEventArgs e, ForageFantasyConfig config, ITranslationHelper translation)
        {
            if (!config.WildAndFineGrapes)
            {
                return;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, MachineData> data = asset.AsDictionary<string, MachineData>().Data;

                    if (!data.TryGetValue("(BC)Dehydrator", out var dehydrator))
                    {
                        return;
                    }

                    var raisinRule = dehydrator.OutputRules.Where((r) => r.Id == "Raisins").FirstOrDefault();

                    if (raisinRule != null)
                    {
                        var fineRaisinRule = raisinRule.Triggers.Where((r) => r.Id == fineGrapeNonQID).FirstOrDefault();

                        if (fineRaisinRule == null)
                        {
                            fineRaisinRule = new MachineOutputTriggerRule
                            {
                                Id = fineGrapeNonQID,
                                Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                                RequiredItemId = $"(O){fineGrapeNonQID}",
                                RequiredCount = 4, // one less than normal grapes due to price difference
                                RequiredTags = null,
                                Condition = null
                            };

                            raisinRule.Triggers.Add(fineRaisinRule);
                        }
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

                    if (!data.TryGetValue(grapeNonQID, out var grapeData))
                    {
                        return;
                    }

                    grapeData.DisplayName = translation.Get("WildGrape");

                    string baseGameGrapeDescription = grapeData.Description;

                    Translation wildGrapeDescriptionOverwrite = translation.Get("WildGrapeDescriptionOverwrite");

                    if (wildGrapeDescriptionOverwrite.HasValue() && !string.IsNullOrWhiteSpace(wildGrapeDescriptionOverwrite.ToString()))
                    {
                        grapeData.Description = wildGrapeDescriptionOverwrite.ToString().Trim();
                    }

                    int fineGrapePrice = 110;

                    if (grapeData.CustomFields != null)
                    {
                        if (grapeData.CustomFields.TryGetValue($"{ForageFantasy.Manifest?.UniqueID}.WildGrapePrice", out string wildGrapePriceString) && int.TryParse(wildGrapePriceString, out int wildGrapePriceOverride))
                        {
                            grapeData.Price = wildGrapePriceOverride;
                        }

                        if (grapeData.CustomFields.TryGetValue($"{ForageFantasy.Manifest?.UniqueID}.FineGrapePrice", out string fineGrapePriceString) && int.TryParse(fineGrapePriceString, out int fineGrapePriceOverride))
                        {
                            fineGrapePrice = fineGrapePriceOverride;
                        }
                    }

                    var fineGrapeData = new ObjectData
                    {
                        Name = fineGrapeNonQID,
                        DisplayName = translation.Get("FineGrape"),
                        Description = baseGameGrapeDescription,
                        Type = "Basic",
                        Category = StardewObject.FruitsCategory,
                        Price = fineGrapePrice,
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

                    Translation fineGrapeDescriptionOverwrite = translation.Get("FineGrapeDescriptionOverwrite");

                    if (fineGrapeDescriptionOverwrite.HasValue() && !string.IsNullOrWhiteSpace(fineGrapeDescriptionOverwrite.ToString()))
                    {
                        fineGrapeData.Description = fineGrapeDescriptionOverwrite.ToString().Trim();
                    }

                    data[fineGrapeNonQID] = fineGrapeData;
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