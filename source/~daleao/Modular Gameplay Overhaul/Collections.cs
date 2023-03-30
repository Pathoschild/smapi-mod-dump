/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#endregion using directives

/// <summary>Holds collections which may be referenced by different modules.</summary>
internal static class Collections
{
    /// <summary>Gets the recognized artisan machines.</summary>
    internal static ImmutableHashSet<string> ArtisanMachines { get; } = new HashSet<string>
    {
        "Cheese Press",
        "Keg",
        "Loom",
        "Mayonnaise Machine",
        "Oil Maker",
        "Preserves Jar",
    }.Concat(ProfessionsModule.Config.CustomArtisanMachines).ToImmutableHashSet();

    /// <summary>Gets the names of the legendary fish.</summary>
    internal static ImmutableHashSet<string> LegendaryFishNames { get; } = new HashSet<string>
    {
        "Crimsonfish", // vanilla
        "Angler", // vanilla
        "Legend", // vanilla
        "Glacierfish", // vanilla
        "Mutant Carp", // vanilla
        "Son of Crimsonfish", // qi extended
        "Ms. Angler", // qi extended
        "Legend II", // qi extended
        "Glacierfish Jr.", // qi extended
        "Radioactive Carp", // qi extended
        "Pufferchick", // stardew aquarium
        "Deep Ridge Angler", // ridgeside
        "Sockeye Salmon", // ridgeside
        "Waterfall Snakehead", // ridgeside
    }.ToImmutableHashSet();

    /// <summary>Gets the resource that should spawn from a given stone.</summary>
    internal static ImmutableDictionary<int, int> ResourceFromStoneId { get; } = new Dictionary<int, int>
    {
        // stone
        { 668, 390 },
        { 670, 390 },
        { 845, 390 },
        { 846, 390 },
        { 847, 390 },

        // ores
        { 751, 378 },
        { 849, 378 },
        { 290, 380 },
        { 850, 380 },
        { 764, 384 },
        { 765, 386 },
        { 95, 909 },

        // geodes
        { 75, 535 },
        { 76, 536 },
        { 77, 537 },
        { 819, 749 },

        // gems
        { 8, 66 },
        { 10, 68 },
        { 12, 60 },
        { 14, 62 },
        { 6, 70 },
        { 4, 64 },
        { 2, 72 },

        // other
        { 25, 719 },
        { 816, 881 },
        { 817, 881 },
        { 818, 330 },
        { 843, 848 },
        { 844, 848 },
    }.ToImmutableDictionary();

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static ImmutableHashSet<int> ResourceNodeIds { get; set; } = new HashSet<int>
    {
        // ores
        751, // copper node
        849, // copper ?
        290, // silver node
        850, // silver ?
        764, // gold node
        765, // iridium node
        95, // radioactive node

        // geodes
        75, // geode node
        76, // frozen geode node
        77, // magma geode node
        819, // omni geode node

        // gems
        8, // amethyst node
        10, // topaz node
        12, // emerald node
        14, // aquamarine node
        6, // jade node
        4, // ruby node
        2, // diamond node
        44, // gem node

        // other
        25, // mussel node
        816, // bone node
        817, // bone node
        818, // clay node
        843, // cinder shard node
        844, // cinder shard node
        46, // mystic stone
    }.ToImmutableHashSet();

    /// <summary>Gets or sets the ids of (valuable) resource clumps.</summary>
    internal static HashSet<int> ResourceClumpIds { get; set; } = new();

    /// <summary>Gets the corresponding extended family pair by legendary fish id.</summary>
    internal static ImmutableDictionary<int, int> ExtendedFamilyPairs { get; } = new Dictionary<int, int>
    {
        { ItemIDs.Crimsonfish, ItemIDs.SonOfCrimsonfish },
        { ItemIDs.Angler, ItemIDs.MsAngler },
        { ItemIDs.Legend, ItemIDs.Legend2 },
        { ItemIDs.MutantCarp, ItemIDs.RadioactiveCarp },
        { ItemIDs.Glacierfish, ItemIDs.GlacierfishJr },
        { ItemIDs.SonOfCrimsonfish, ItemIDs.Crimsonfish },
        { ItemIDs.MsAngler, ItemIDs.Angler },
        { ItemIDs.Legend2, ItemIDs.Legend },
        { ItemIDs.RadioactiveCarp, ItemIDs.MutantCarp },
        { ItemIDs.GlacierfishJr, ItemIDs.Glacierfish },
    }.ToImmutableDictionary();

    /// <summary>Gets the swords that should be converted to Stabbing Swords.</summary>
    internal static HashSet<int> StabbingSwords { get; } = new()
    {
        ItemIDs.SteelSmallsword,
        ItemIDs.Cutlass,
        ItemIDs.Rapier,
        ItemIDs.SteelFalchion,
        ItemIDs.PiratesSword,
        ItemIDs.ForestSword,
        ItemIDs.LavaKatana,
        ItemIDs.DragontoothCutlass,
        ItemIDs.DarkSword,
    };
}
