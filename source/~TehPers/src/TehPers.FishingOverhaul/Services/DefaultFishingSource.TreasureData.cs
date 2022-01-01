/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal partial class DefaultFishingSource
    {
        private FishingContent GetDefaultTreasureData()
        {
            return new(this.manifest) { AddTreasure = GenerateTreasureData().ToImmutableArray() };

            IEnumerable<TreasureEntry> GenerateTreasureData()
            {
                const double archaeologyChance = 0.015625;

                // Dressed spinner
                yield return new(
                    new(0.025) { MinFishingLevel = 6 },
                    ImmutableArray.Create(NamespacedKey.SdvObject(687))
                ) { AllowDuplicates = false };

                // Bait
                yield return new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(685)))
                {
                    MinQuantity = 2,
                    MaxQuantity = 4,
                };

                // Archaeology
                yield return new LostBookEntry(
                    new(0.025 * 100000)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["TehPers.FishingOverhaul/BooksFound"] = "{{Range: 0, 19}}",
                            // TODO: remove this when CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    AllowDuplicates = false,
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "LostBook")
                        ),
                    },
                };
                yield return new(
                    new(archaeologyChance * 4),
                    Enumerable.Range(585, 4).Select(NamespacedKey.SdvObject).ToImmutableArray()
                );
                yield return new(
                    new(archaeologyChance * 6),
                    Enumerable.Range(96, 6).Select(NamespacedKey.SdvObject).ToImmutableArray()
                );
                yield return new(
                    new(archaeologyChance * 25),
                    Enumerable.Range(103, 25).Select(NamespacedKey.SdvObject).ToImmutableArray()
                );

                // Geodes
                yield return new(new(0.2), ImmutableArray.Create(NamespacedKey.SdvObject(535)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                };
                yield return new(new(0.125), ImmutableArray.Create(NamespacedKey.SdvObject(536)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                };
                yield return new(new(0.125), ImmutableArray.Create(NamespacedKey.SdvObject(537)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                };
                yield return new(new(0.0625), ImmutableArray.Create(NamespacedKey.SdvObject(749)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                };

                // Ores + coal
                yield return new(new(0.0075), ImmutableArray.Create(NamespacedKey.SdvObject(386)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                };
                yield return new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(384)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                };
                yield return new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(380)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10
                };
                yield return new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(378)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                };
                yield return new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(382)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                };

                // Gemstones
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(60)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(62)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(64)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(66)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(68)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(70)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.028), ImmutableArray.Create(NamespacedKey.SdvObject(72)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 2,
                };
                yield return new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(82)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(84)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };
                yield return new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(86)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                };

                // Junk
                yield return new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(388)))
                {
                    MinQuantity = 10,
                    MaxQuantity = 25,
                };
                yield return new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(390)))
                {
                    MinQuantity = 10,
                    MaxQuantity = 25,
                };
                yield return new(
                    new(0.5) { MaxFishingLevel = 1 },
                    ImmutableArray.Create(NamespacedKey.SdvObject(770))
                )
                {
                    MinQuantity = 3,
                    MaxQuantity = 5,
                };
                yield return new(new(0.005), ImmutableArray.Create(NamespacedKey.SdvObject(166)))
                {
                    AllowDuplicates = false,
                };
                yield return new(new(0.00025), ImmutableArray.Create(NamespacedKey.SdvObject(74)))
                {
                    AllowDuplicates = false,
                };
                yield return new(
                    new(0.01)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag: hostPlayer"] = "Farm_Eternal",
                        }.ToImmutableDictionary(),
                    },
                    ImmutableArray.Create(NamespacedKey.SdvObject(928))
                );

                // Rice shoot
                yield return new(
                    new(0.1) { Seasons = Seasons.Spring },
                    ImmutableArray.Create(NamespacedKey.SdvObject(273))
                )
                {
                    MinQuantity = 2,
                    MaxQuantity = 11,
                };

                // Qi beans
                yield return new(
                    new(0.33)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["TehPers.FishingOverhaul/SpecialOrderRuleActive"] =
                                "DROP_QI_BEANS",
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary()
                    },
                    ImmutableArray.Create(NamespacedKey.SdvObject(890))
                )
                {
                    MinQuantity = 1,
                    MaxQuantity = 4,
                };

                // Weapons
                yield return new(new(0.001), ImmutableArray.Create(NamespacedKey.SdvWeapon(14)))
                {
                    AllowDuplicates = false,
                };
                yield return new(new(0.001), ImmutableArray.Create(NamespacedKey.SdvWeapon(51)))
                {
                    AllowDuplicates = false,
                };

                // Boots
                yield return new(
                    new(0.005),
                    Enumerable.Range(504, 10).Select(NamespacedKey.SdvBoots).ToImmutableArray()
                ) { AllowDuplicates = false };

                // Rings
                yield return new(new(0.0025), ImmutableArray.Create(NamespacedKey.SdvRing(527)))
                {
                    AllowDuplicates = false,
                };
                yield return new(
                    new(0.005),
                    Enumerable.Range(516, 4).Select(NamespacedKey.SdvRing).ToImmutableArray()
                ) { AllowDuplicates = false };
                yield return new(
                    new(0.005),
                    Enumerable.Range(529, 6).Select(NamespacedKey.SdvRing).ToImmutableArray()
                ) { AllowDuplicates = false };
            }
        }
    }
}