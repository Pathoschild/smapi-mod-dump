/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Collections.Immutable;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Effects;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource
    {
        private FishingContent GetDefaultEffectData()
        {
            static IEnumerable<FishingEffectEntry> GenerateEffectData()
            {
                // Town fountain
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        Position = new()
                        {
                            X = new() {LessThan = 30},
                            Y = new() {LessThan = 30},
                        },
                    },
                };

                // Tide pool golden walnut
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("IslandSouthEast"),
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
                };

                // Iridium Krobus
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("Forest"),
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
                };

                // Caroline's necklace
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        When = new Dictionary<string, string?>
                        {
                            [$"HasFlag |contains={GameLocation.CAROLINES_NECKLACE_MAIL}"] =
                                "false",
                            [$"TehPers.FishingOverhaul/MissingSecretNotes |contains={GameLocation.NECKLACE_SECRET_NOTE_INDEX}"] =
                                "false",
                        }.ToImmutableDictionary(),
                    }
                };

                // Foliage print
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new() {Y = new() {GreaterThan = 72}},
                        When = new Dictionary<string, string?>
                        {
                            ["HasFlag |contains=gotSecretIslandNPainting"] = "false",
                        }.ToImmutableDictionary(),
                    }
                };

                // Squirrel figurine
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
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
                };

                // 'Boat'
                yield return new ModifyChanceEffectEntry(ModifyChanceType.MaxFish, "0")
                {
                    Conditions = new()
                    {
                        IncludeLocations = ImmutableArray.Create("Farm/Beach"),
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
                };
            }

            return new(this.manifest)
            {
                AddEffects = GenerateEffectData().ToImmutableArray(),
            };
        }
    }
}
