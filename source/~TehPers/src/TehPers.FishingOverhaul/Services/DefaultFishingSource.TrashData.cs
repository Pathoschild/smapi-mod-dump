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
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource
    {
        private FishingContent GetDefaultTrashData()
        {
            return new(this.manifest) {AddTrash = GenerateTrashData().ToImmutableArray()};

            IEnumerable<TrashEntry> GenerateTrashData()
            {
                // Joja cola
                yield return new(
                    NamespacedKey.SdvObject(167),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Trash
                yield return new(
                    NamespacedKey.SdvObject(168),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Driftwood
                yield return new(
                    NamespacedKey.SdvObject(169),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Broken Glasses
                yield return new(
                    NamespacedKey.SdvObject(170),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Broken CD
                yield return new(
                    NamespacedKey.SdvObject(171),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Soggy newspaper
                yield return new(
                    NamespacedKey.SdvObject(172),
                    new(1.0D) {ExcludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Pearl
                yield return new(
                    NamespacedKey.SdvObject(797),
                    new(0.01D) {IncludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Seaweed
                yield return new(
                    NamespacedKey.SdvObject(152),
                    new(0.99D) {IncludeLocations = ImmutableArray.Create("Submarine")}
                );
                // Void mayonnaise
                yield return new(
                    NamespacedKey.SdvObject(308),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("WitchSwamp"),
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=henchmanGone"] = "false",
                            ["TehPers.FishingOverhaul/HasItem: 308, 1"] = "false",
                        }.ToImmutableDictionary(),
                    }
                );
                // Secret notes
                yield return new SecretNoteEntry(
                    new(0.08)
                    {
                        When = new Dictionary<string, string?>
                        {
                            ["HasWalletItem"] = "MagnifyingGlass",
                            ["LocationContext"] = "Valley",
                            ["query: {{Count: {{TehPers.FishingOverhaul/MissingSecretNotes}}}} > 0"] =
                                "true",
                        }.ToImmutableDictionary(),
                    }
                );
                // Journal scraps
                yield return new JournalScrapEntry(
                    new(0.08)
                    {
                        When = new Dictionary<string, string?>
                        {
                            ["LocationContext"] = "Island",
                            ["query: {{Count: {{TehPers.FishingOverhaul/MissingJournalScraps}}}} > 0"] =
                                "true",
                        }.ToImmutableDictionary()
                    }
                );
                // Random golden walnuts
                yield return new GoldenWalnutEntry(
                    new(0.5)
                    {
                        When = new Dictionary<string, string?>
                        {
                            ["LocationContext"] = "Island",
                            ["TehPers.FishingOverhaul/RandomGoldenWalnuts"] = "{{Range: 0, 4}}",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "RandomGoldenWalnut")
                        )
                    }
                };
                // Tidepool golden walnut
                yield return new GoldenWalnutEntry(
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandSouthEast"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        Position = new()
                        {
                            X = new()
                            {
                                GreaterThanEq = 18,
                                LessThan = 20,
                            },
                            Y = new()
                            {
                                GreaterThanEq = 20,
                                LessThan = 22,
                            },
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["TehPers.FishingOverhaul/TidePoolGoldenWalnut"] = "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "TidePoolGoldenWalnut")
                        ),
                    },
                };
                // Iridium Krobus
                yield return new(
                    NamespacedKey.SdvFurniture(2396),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Forest"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        Position = new()
                        {
                            Y = new()
                            {
                                GreaterThan = 108,
                            },
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=caughtIridiumKrobus"] = "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        SetFlags = ImmutableArray.Create("caughtIridiumKrobus"),
                    },
                };
                // 'Physics 101'
                yield return new(
                    NamespacedKey.SdvFurniture(2732),
                    new(0.05)
                    {
                        IncludeLocations = ImmutableArray.Create("Caldera"),
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=CalderaPainting"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("CalderaPainting")}};
                // Pyramid decal
                yield return new(
                    NamespacedKey.SdvFurniture(2334),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("Desert"),
                        Position = new() {Y = new() {GreaterThan = 55}},
                    }
                );
                // 'Lifesaver'
                yield return new(
                    NamespacedKey.SdvFurniture(2418),
                    new(0.2) {IncludeLocations = ImmutableArray.Create("Willys Ship")}
                );
                // Caroline's necklace
                yield return new(
                    NamespacedKey.SdvObject(GameLocation.CAROLINES_NECKLACE_ITEM),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        PriorityTier = DefaultFishingSource.questItemTier,
                        When = new Dictionary<string, string?>
                        {
                            [$"HasFlag |contains={GameLocation.CAROLINES_NECKLACE_MAIL}"] = "false",
                            [$"TehPers.FishingOverhaul/MissingSecretNotes |contains={GameLocation.NECKLACE_SECRET_NOTE_INDEX}"] =
                                "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        StartQuests = ImmutableArray.Create(128, 129),
                        AddMail = ImmutableArray.Create(
                            $"{GameLocation.CAROLINES_NECKLACE_MAIL}%&NL&%"
                        ),
                    },
                };
                // 'Vista'
                yield return new(
                    NamespacedKey.SdvFurniture(2423),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=gotSpaFishing"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("gotSpaFishing")}};
                yield return new(
                    NamespacedKey.SdvFurniture(2423),
                    new(0.08)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "gotSpaFishing",
                        }.ToImmutableDictionary(),
                    }
                );
                // Decorative trash can
                yield return new(
                    NamespacedKey.SdvFurniture(2427),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        Position = new()
                        {
                            X = new() {LessThan = 30},
                            Y = new() {LessThan = 30},
                        }
                    }
                );
                // Other fountain trash
                yield return new(
                    NamespacedKey.SdvObject(388),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        PriorityTier = DefaultFishingSource.prioritizedTier,
                        Position = new()
                        {
                            X = new() {LessThan = 30},
                            Y = new() {LessThan = 30},
                        }
                    }
                );
                yield return new(
                    NamespacedKey.SdvObject(390),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        PriorityTier = DefaultFishingSource.prioritizedTier,
                        Position = new()
                        {
                            X = new() {LessThan = 30},
                            Y = new() {LessThan = 30},
                        }
                    }
                );
                // Wall basket
                yield return new(
                    NamespacedKey.SdvFurniture(2425),
                    new(0.08) {IncludeLocations = ImmutableArray.Create("Woods")}
                );
                // Frog hat
                yield return new(
                    NamespacedKey.SdvHat(78),
                    new(0.1) {IncludeLocations = ImmutableArray.Create("IslandFarmCave")}
                );
                // Fossilized Spine
                yield return new(
                    NamespacedKey.SdvObject(821),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "Island_UpgradeBridge",
                        }.ToImmutableDictionary()
                    }
                );
                // Foliage print
                yield return new(
                    NamespacedKey.SdvFurniture(2419),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        Position = new() {Y = new() {GreaterThan = 72}},
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=gotSecretIslandNPainting"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("gotSecretIslandNPainting")}};
                yield return new(
                    NamespacedKey.SdvFurniture(2419),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new() {Y = new() {GreaterThan = 72}},
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "gotSecretIslandNPainting",
                        }.ToImmutableDictionary(),
                    }
                );
                // Squirrel figurine
                yield return new(
                    NamespacedKey.SdvFurniture(2814),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        Position = new()
                        {
                            X = new() {LessThan = 4},
                            Y = new() {LessThan = 35},
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=gotSecretIslandNSquirrel"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("gotSecretIslandNSquirrel")}};
                yield return new(
                    NamespacedKey.SdvFurniture(2814),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new()
                        {
                            X = new() {LessThan = 4},
                            Y = new() {LessThan = 35},
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "gotSecretIslandNSquirrel",
                        }.ToImmutableDictionary(),
                    }
                );
                // Gourmand's statue
                yield return new(
                    NamespacedKey.SdvFurniture(2332),
                    new(0.05) {IncludeLocations = ImmutableArray.Create("IslandSouthEastCave")}
                );
                // Snake skull
                yield return new(
                    NamespacedKey.SdvObject(825),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandWest"),
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "islandNorthCaveOpened",
                        }.ToImmutableDictionary(),
                    }
                );
                // Beach farm
                var beachPositions = new PositionConstraint[]
                {
                    new() {X = new() {LessThan = 26}},
                    new() {X = new() {GreaterThan = 26 + 31}},
                    new()
                    {
                        X = new()
                        {
                            GreaterThanEq = 26,
                            LessThan = 26 + 31,
                        },
                        Y = new() {LessThan = 45},
                    },
                    new()
                    {
                        X = new()
                        {
                            GreaterThanEq = 26,
                            LessThan = 26 + 31,
                        },
                        Y = new() {GreaterThanEq = 45 + 39},
                    },
                };
                foreach (var position in beachPositions)
                {
                    // Seaweed
                    yield return new(
                        NamespacedKey.SdvObject(152),
                        new(0.15)
                        {
                            IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                            Position = position,
                        }
                    );
                    // Oyster
                    yield return new(
                        NamespacedKey.SdvObject(723),
                        new(0.015)
                        {
                            IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                            Position = position,
                        }
                    );
                    // Coral
                    yield return new(
                        NamespacedKey.SdvObject(393),
                        new(0.015)
                        {
                            IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                            Position = position,
                        }
                    );
                    // Mussel
                    yield return new(
                        NamespacedKey.SdvObject(719),
                        new(0.015)
                        {
                            IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                            Position = position,
                        }
                    );
                    // Cockle
                    yield return new(
                        NamespacedKey.SdvObject(718),
                        new(0.015)
                        {
                            IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                            Position = position,
                        }
                    );
                }

                // 'Boat'
                yield return new(
                    NamespacedKey.SdvFurniture(2421),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Farm/Beach"),
                        PriorityTier = DefaultFishingSource.specialItemTier,
                        FarmerPosition = new()
                        {
                            X = new()
                            {
                                GreaterThanEq = 23,
                                LessThan = 24,
                            },
                            Y = new()
                            {
                                GreaterThanEq = 98,
                                LessThan = 99,
                            },
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=gotBoatPainting"] = "false"
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("gotBoatPainting")}};
                // Dove children (https://stardewvalleywiki.com/Secrets#Dove_Children)
                yield return new(
                    NamespacedKey.SdvObject(103),
                    new(0.5)
                    {
                        IncludeLocations = ImmutableArray.Create("Farm/FourCorners"),
                        FarmerPosition = new()
                        {
                            X = new() {LessThan = 40},
                            Y = new() {GreaterThan = 54},
                        },
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag"] = "cursed_doll",
                            ["HasFlag |contains=eric's_prank_1"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) {OnCatch = new() {SetFlags = ImmutableArray.Create("eric's_prank_1")}};
            }
        }
    }
}
