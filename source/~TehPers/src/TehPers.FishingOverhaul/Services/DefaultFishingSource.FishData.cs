/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource
    {
        private static readonly ImmutableDictionary<string, string> hasCaughtFish =
            new Dictionary<string, string>
            {
                ["HasValue:{{HasCaughtFish}}"] = "true",
            }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> isLegendaryFamilyActive =
            new Dictionary<string, string>
            {
                ["TehPers.FishingOverhaul/SpecialOrderRuleActive"] = "LEGENDARY_FAMILY",
                // TODO: remove this once CP updates
                ["HasMod"] = "TehPers.FishingOverhaul",
            }.ToImmutableDictionary();

        // Legendary fish
        private static readonly NamespacedKey crimsonfishKey = NamespacedKey.SdvObject(159);
        private const string crimsonfishFlag = "TehPers.FishingOverhaul/crimsonfishCaught";

        private static readonly FishEntry crimsonfishEntry = new(
            DefaultFishingSource.crimsonfishKey,
            new(0.02)
            {
                DepthMultiplier = 0.02 / 4,
                IncludeLocations = ImmutableArray.Create("Beach", "BeachNightMarket"),
                Seasons = Seasons.Summer,
                MinFishingLevel = 5,
                FarmerPosition = new()
                {
                    X = new()
                    {
                        GreaterThanEq = 82,
                    },
                },
                MinDepth = 3,
                When = DefaultFishingSource.hasCaughtFish.Add(
                    $"HasFlag |contains={DefaultFishingSource.crimsonfishFlag}",
                    "false"
                ),
            }
        )
        {
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.crimsonfishFlag),
            }
        };

        private static readonly NamespacedKey anglerKey = NamespacedKey.SdvObject(160);
        private const string anglerFlag = "TehPers.FishingOverhaul/anglerCaught";

        private static readonly FishEntry anglerEntry = new(
            DefaultFishingSource.anglerKey,
            new(0.02)
            {
                DepthMultiplier = 0.02 / 4,
                IncludeLocations = ImmutableArray.Create("Town"),
                Seasons = Seasons.Fall,
                MinFishingLevel = 3,
                FarmerPosition = new()
                {
                    Y = new()
                    {
                        LessThan = 15,
                    },
                },
                When = DefaultFishingSource.hasCaughtFish.Add(
                    $"HasFlag |contains={DefaultFishingSource.anglerFlag}",
                    "false"
                ),
            }
        )
        {
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.anglerFlag),
            }
        };

        private static readonly NamespacedKey legendKey = NamespacedKey.SdvObject(163);
        private const string legendFlag = "TehPers.FishingOverhaul/legendCaught";

        private static readonly FishEntry legendEntry = new(
            DefaultFishingSource.legendKey,
            new(0.02)
            {
                DepthMultiplier = 0.02 / 4,
                IncludeLocations = ImmutableArray.Create("Mountain"),
                Seasons = Seasons.Spring,
                Weathers = Weathers.Rainy,
                WaterTypes = WaterTypes.PondOrOcean,
                MinFishingLevel = 10,
                MinDepth = 4,
                When = DefaultFishingSource.hasCaughtFish.Add(
                    $"HasFlag |contains={DefaultFishingSource.legendFlag}",
                    "false"
                ),
            }
        )
        {
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.legendFlag),
            }
        };

        private static readonly NamespacedKey mutantCarpKey = NamespacedKey.SdvObject(682);
        private const string mutantCarpFlag = "TehPers.FishingOverhaul/mutantCarpCaught";

        private static readonly FishEntry mutantCarpEntry = new(
            DefaultFishingSource.mutantCarpKey,
            new(0.02)
            {
                DepthMultiplier = 0.02 / 4,
                IncludeLocations = ImmutableArray.Create("Sewer"),
                FarmerPosition = new()
                {
                    X = new()
                    {
                        LessThanEq = 14,
                    },
                    Y = new()
                    {
                        LessThanEq = 42,
                    },
                },
                When = DefaultFishingSource.hasCaughtFish.Add(
                    $"HasFlag |contains={DefaultFishingSource.mutantCarpFlag}",
                    "false"
                ),
            }
        )
        {
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.mutantCarpFlag),
            }
        };

        private static readonly NamespacedKey glacierfishKey = NamespacedKey.SdvObject(775);
        private const string glacierfishFlag = "TehPers.FishingOverhaul/glacierfishCaught";

        private static readonly FishEntry glacierfishEntry = new(
            DefaultFishingSource.glacierfishKey,
            new(0.02)
            {
                DepthMultiplier = 0.02 / 4,
                IncludeLocations = ImmutableArray.Create("Forest"),
                Seasons = Seasons.Winter,
                WaterTypes = WaterTypes.River,
                MinFishingLevel = 6,
                FarmerPosition = new()
                {
                    X = new()
                    {
                        GreaterThanEq = 58,
                        LessThan = 59,
                    },
                    Y = new()
                    {
                        GreaterThanEq = 87,
                        LessThan = 88,
                    },
                },
                MinDepth = 3,
                When = DefaultFishingSource.hasCaughtFish.Add(
                    $"HasFlag |contains={DefaultFishingSource.glacierfishFlag}",
                    "false"
                ),
            }
        )
        {
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.glacierfishFlag),
            }
        };

        // Legendary II fish
        private static readonly NamespacedKey sonOfCrimsonfishKey = NamespacedKey.SdvObject(898);

        private const string sonOfCrimsonfishFlag =
            "TehPers.FishingOverhaul/sonOfCrimsonfishCaught";

        private static readonly FishEntry sonOfCrimsonfishEntry =
            DefaultFishingSource.crimsonfishEntry with
            {
                FishKey = DefaultFishingSource.sonOfCrimsonfishKey,
                AvailabilityInfo =
                DefaultFishingSource.crimsonfishEntry.AvailabilityInfo with
                {
                    When = DefaultFishingSource.isLegendaryFamilyActive,
                },
                OnCatch = new()
                {
                    SetFlags = ImmutableArray.Create(
                        DefaultFishingSource.sonOfCrimsonfishFlag
                    ),
                }
            };

        private static readonly NamespacedKey msAnglerKey = NamespacedKey.SdvObject(899);
        private const string msAnglerFlag = "TehPers.FishingOverhaul/msAnglerCaught";

        private static readonly FishEntry msAnglerEntry = DefaultFishingSource.anglerEntry with
        {
            FishKey = DefaultFishingSource.msAnglerKey,
            AvailabilityInfo =
            DefaultFishingSource.anglerEntry.AvailabilityInfo with
            {
                When = DefaultFishingSource.isLegendaryFamilyActive,
            },
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.msAnglerFlag),
            }
        };

        private static readonly NamespacedKey legend2Key = NamespacedKey.SdvObject(900);
        private const string legend2Flag = "TehPers.FishingOverhaul/legendIICaught";

        private static readonly FishEntry legend2Entry = DefaultFishingSource.legendEntry with
        {
            FishKey = DefaultFishingSource.legend2Key,
            AvailabilityInfo =
            DefaultFishingSource.legendEntry.AvailabilityInfo with
            {
                When = DefaultFishingSource.isLegendaryFamilyActive,
            },
            OnCatch = new()
            {
                SetFlags = ImmutableArray.Create(DefaultFishingSource.legend2Flag),
            }
        };

        private static readonly NamespacedKey radioactiveCarpKey = NamespacedKey.SdvObject(901);
        private const string radioactiveCarpFlag = "TehPers.FishingOverhaul/radioactiveCarpCaught";

        private static readonly FishEntry radioactiveCarpEntry =
            DefaultFishingSource.mutantCarpEntry with
            {
                FishKey = DefaultFishingSource.radioactiveCarpKey,
                AvailabilityInfo =
                DefaultFishingSource.mutantCarpEntry.AvailabilityInfo with
                {
                    When = DefaultFishingSource.isLegendaryFamilyActive,
                },
                OnCatch = new()
                {
                    SetFlags =
                        ImmutableArray.Create(DefaultFishingSource.radioactiveCarpFlag),
                }
            };

        private static readonly NamespacedKey glacierfishJrKey = NamespacedKey.SdvObject(902);
        private const string glacierfishJrFlag = "TehPers.FishingOverhaul/glacierfishJrCaught";

        private static readonly FishEntry glacierfishJrEntry =
            DefaultFishingSource.glacierfishEntry with
            {
                FishKey = DefaultFishingSource.glacierfishJrKey,
                AvailabilityInfo =
                DefaultFishingSource.glacierfishEntry.AvailabilityInfo with
                {
                    When = DefaultFishingSource.isLegendaryFamilyActive,
                },
                OnCatch = new()
                {
                    SetFlags =
                        ImmutableArray.Create(DefaultFishingSource.glacierfishJrFlag),
                }
            };

        // Set of vanilla legendary fish
        private static readonly HashSet<NamespacedKey> vanillaLegendaries = new()
        {
            // Legendary fish
            DefaultFishingSource.crimsonfishKey,
            DefaultFishingSource.anglerKey,
            DefaultFishingSource.legendKey,
            DefaultFishingSource.mutantCarpKey,
            DefaultFishingSource.glacierfishKey,

            // Legendary II fish
            DefaultFishingSource.sonOfCrimsonfishKey,
            DefaultFishingSource.msAnglerKey,
            DefaultFishingSource.legend2Key,
            DefaultFishingSource.radioactiveCarpKey,
            DefaultFishingSource.glacierfishJrKey,
        };

        private FishingContent GetDefaultFishData()
        {
            var fishEntries = new List<FishEntry>();
            var fishTraits = new Dictionary<NamespacedKey, FishTraits>();
            var trashEntries = new List<TrashEntry>();

            // Parse the fish traits
            var fish = this.assetProvider.Load<Dictionary<int, string>>(@"Data\Fish.xnb");
            var fishAvailabilities = new Dictionary<int, List<FishAvailabilityInfo>>(fish.Count);
            var trashAvailabilities = new Dictionary<int, List<AvailabilityInfo>>();
            foreach (var (fishId, rawFishInfo) in fish)
            {
                var fishInfo = rawFishInfo.Split('/');

                // TODO: maybe do something with fish caught in pots

                var fishKey = NamespacedKey.SdvObject(fishId);
                if (fishInfo[1] != "5")
                {
                    // Parse fish fields
                    if (!DefaultFishingSource.TryParseFishInfo(
                            fishInfo,
                            out var traits,
                            out var availabilities
                        ))
                    {
                        continue;
                    }

                    // Add traits and availabilities
                    fishTraits[fishKey] = traits with
                    {
                        IsLegendary = DefaultFishingSource.vanillaLegendaries.Contains(fishKey),
                    };
                    fishAvailabilities[fishId] = availabilities.ToList();
                }
                else
                {
                    // Parse trash fields
                    if (!DefaultFishingSource.TryGetAvailabilities(
                            fishInfo,
                            baseChance => new AvailabilityInfo(baseChance),
                            out var availabilities
                        ))
                    {
                        continue;
                    }

                    // Add availabilities
                    trashAvailabilities[fishId] = availabilities.ToList();
                }
            }

            // Parse the location data
            var locations =
                this.assetProvider.Load<Dictionary<string, string>>(@"Data\Locations.xnb");
            foreach (var (locationName, rawLocationData) in locations)
            {
                var locationData = rawLocationData.Split('/');
                const int offset = 4;

                // Parse each season's data
                var seasons = Seasons.None;
                foreach (var seasonData in locationData.Skip(offset)
                             .Take(4)
                             .Select(data => data.Split(' ')))
                {
                    // Cycle season
                    seasons = seasons switch
                    {
                        Seasons.None => Seasons.Spring,
                        Seasons.Spring => Seasons.Summer,
                        Seasons.Summer => Seasons.Fall,
                        Seasons.Fall => Seasons.Winter,
                        _ => Seasons.None
                    };

                    // Check if too many iterations
                    if (seasons == Seasons.None)
                    {
                        break;
                    }

                    // Parse each fish's data
                    for (var i = 0; i < seasonData.Length - 1; i += 2)
                    {
                        // Fish ID
                        if (!int.TryParse(seasonData[i], out var fishId))
                        {
                            continue;
                        }

                        // Water type
                        if (!int.TryParse(seasonData[i + 1], out var waterTypeId))
                        {
                            continue;
                        }

                        var waterTypes = waterTypeId switch
                        {
                            -1 => WaterTypes.All,
                            0 => WaterTypes.River,
                            1 => WaterTypes.PondOrOcean,
                            2 => WaterTypes.Freshwater,
                            _ => WaterTypes.All,
                        };

                        // Add availabilities
                        if (fishAvailabilities.TryGetValue(fishId, out var f))
                        {
                            // Fish availabilities
                            DefaultFishingSource.AddEntries(
                                fishEntries,
                                f,
                                seasons,
                                waterTypes,
                                locationName,
                                availability => new(NamespacedKey.SdvObject(fishId), availability)
                            );
                        }
                        else if (trashAvailabilities.TryGetValue(fishId, out var t))
                        {
                            // Trash availabilities
                            DefaultFishingSource.AddEntries(
                                trashEntries,
                                t,
                                seasons,
                                waterTypes,
                                locationName,
                                availability => new(NamespacedKey.SdvObject(fishId), availability)
                            );
                        }
                    }
                }
            }

            // Special entries
            fishEntries.AddRange(
                new[]
                {
                    // Legendary fish
                    DefaultFishingSource.crimsonfishEntry,
                    DefaultFishingSource.anglerEntry,
                    DefaultFishingSource.legendEntry,
                    DefaultFishingSource.mutantCarpEntry,
                    DefaultFishingSource.glacierfishEntry,

                    // Legendary II fish
                    DefaultFishingSource.sonOfCrimsonfishEntry,
                    DefaultFishingSource.msAnglerEntry,
                    DefaultFishingSource.legend2Entry,
                    DefaultFishingSource.radioactiveCarpEntry,
                    DefaultFishingSource.glacierfishJrEntry,

                    // Forest farm (woodskip)
                    new(
                        NamespacedKey.SdvObject(734),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Farm/Forest"),
                        }
                    ),

                    // UndergroundMine
                    new(
                        NamespacedKey.SdvObject(158),
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = ImmutableArray.Create("UndergroundMine/20"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(161),
                        new(0.015)
                        {
                            DepthMultiplier = 0.015 / 4,
                            IncludeLocations = ImmutableArray.Create("UndergroundMine/60"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(162),
                        new(0.01)
                        {
                            DepthMultiplier = 0.01 / 4,
                            IncludeLocations = ImmutableArray.Create("UndergroundMine/100"),
                        }
                    ),

                    // Submarine
                    new(
                        NamespacedKey.SdvObject(149),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(154),
                        new(0.08)
                        {
                            DepthMultiplier = 0.08 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(155),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(798),
                        new(0.28)
                        {
                            DepthMultiplier = 0.28 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(799),
                        new(0.18)
                        {
                            DepthMultiplier = 0.18 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(800),
                        new(0.1)
                        {
                            DepthMultiplier = 0.1 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),

                    // Volcano caldera
                    new(
                        NamespacedKey.SdvObject(162),
                        new(0.1)
                        {
                            DepthMultiplier = 0.1 / 4,
                            IncludeLocations = ImmutableArray.Create("Caldera"),
                        }
                    )
                }
            );

            // Request reload
            this.ReloadRequested?.Invoke(this, EventArgs.Empty);

            return new(this.manifest)
            {
                SetFishTraits = fishTraits.ToImmutableDictionary(),
                AddFish = fishEntries.ToImmutableArray(),
                AddTrash = trashEntries.ToImmutableArray(),
            };
        }

        private static void AddEntries<TAvailability, TEntry>(
            List<TEntry> entries,
            IEnumerable<TAvailability> availabilities,
            Seasons seasons,
            WaterTypes waterTypes,
            string locationName,
            Func<TAvailability, TEntry> toEntry
        )
            where TAvailability : AvailabilityInfo
        {
            entries.AddRange(
                availabilities.Select(
                    availability => toEntry(
                        availability with
                        {
                            Seasons = seasons,
                            WaterTypes = waterTypes,
                            IncludeLocations = locationName switch
                            {
                                // Include both beach locations and beach farm
                                "Beach" => ImmutableArray.Create(
                                    locationName,
                                    "BeachNightMarket",
                                    "Farm/Beach"
                                ),
                                // Many farms use forest fish
                                "Forest" => ImmutableArray.Create(
                                    locationName,
                                    "Farm/Riverland",
                                    "Farm/Forest",
                                    "Farm/Hills",
                                    "Farm/FourCorners"
                                ),
                                // Riverland farm uses town fish
                                "Town" => ImmutableArray.Create(locationName, "Farm/Riverland"),
                                // Some farms use mountain fish
                                "Mountain" => ImmutableArray.Create(
                                    locationName,
                                    "Farm/Mountain",
                                    "Farm/FourCorners"
                                ),
                                // Normal handling
                                _ => ImmutableArray.Create(locationName),
                            },
                        }
                    )
                )
            );
        }

        private static bool TryParseFishInfo(
            IReadOnlyList<string> fishInfo,
            [NotNullWhen(true)] out FishTraits? traits,
            [NotNullWhen(true)] out IEnumerable<FishAvailabilityInfo>? availabilities
        )
        {
            traits = default;
            availabilities = default;

            if (!DefaultFishingSource.TryGetAvailabilities(
                    fishInfo,
                    baseChance => new FishAvailabilityInfo(baseChance),
                    out var baseAvailabilities
                ))
            {
                return false;
            }

            // Dart frequency
            if (!int.TryParse(fishInfo[1], out var dartFrequency))
            {
                return false;
            }

            // Dart behavior
            var dartBehavior = fishInfo[2] switch
            {
                "mixed" => DartBehavior.Mixed,
                "dart" => DartBehavior.Dart,
                "smooth" => DartBehavior.Smooth,
                "sink" => DartBehavior.Sink,
                "floater" => DartBehavior.Floater,
                _ => DartBehavior.Mixed,
            };

            // Min size
            if (!int.TryParse(fishInfo[3], out var minSize))
            {
                return false;
            }

            // Max size
            if (!int.TryParse(fishInfo[4], out var maxSize))
            {
                return false;
            }

            // Depth multiplier
            if (!float.TryParse(fishInfo[11], out var depthMultiplier))
            {
                return false;
            }

            // Max depth
            if (!int.TryParse(fishInfo[9], out var maxDepth))
            {
                return false;
            }

            // Parse times and populate spawn availabilities
            availabilities = baseAvailabilities.Select(
                availability => availability with
                {
                    DepthMultiplier = depthMultiplier,
                    MaxDepth = maxDepth
                }
            );

            // Set traits
            traits = new(dartFrequency, dartBehavior, minSize, maxSize);

            return true;
        }

        private static bool TryGetAvailabilities<T>(
            IReadOnlyList<string> fishInfo,
            Func<double, T> createAvailability,
            [NotNullWhen(true)] out IEnumerable<T>? availabilities
        )
            where T : AvailabilityInfo
        {
            availabilities = default;

            // Check if the fish info is valid
            if (fishInfo.Count < 13)
            {
                return false;
            }

            // Weathers
            var weathers = fishInfo[7]
                .Split(' ')
                .Aggregate(
                    Weathers.None,
                    (weathers, cur) => weathers
                        | cur switch
                        {
                            "sunny" => Weathers.Sunny,
                            "rainy" => Weathers.Rainy,
                            "both" => Weathers.All,
                            _ => Weathers.None,
                        }
                );

            // Spawn multiplier
            if (!float.TryParse(fishInfo[10], out var weightedChance))
            {
                return false;
            }

            // Min fishing level
            if (!int.TryParse(fishInfo[12], out var minFishingLevel))
            {
                return false;
            }

            availabilities = GenerateAvailabilities();
            return true;

            IEnumerable<T> GenerateAvailabilities()
            {
                // Parse times and populate spawn availabilities
                var times = fishInfo[5].Split(' ');
                for (var i = 0; i < times.Length - 1; i += 2)
                {
                    // Start time
                    if (!int.TryParse(times[i], out var startTime))
                    {
                        continue;
                    }

                    // End time
                    if (!int.TryParse(times[i + 1], out var endTime))
                    {
                        continue;
                    }

                    yield return createAvailability(weightedChance) with
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        Weathers = weathers,
                        MinFishingLevel = minFishingLevel,
                    };
                }
            }
        }
    }
}
