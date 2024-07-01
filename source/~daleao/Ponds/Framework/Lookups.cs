/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static Dictionary<string, string> FamilyPairs { get; } = new()
    {
        { QualifiedObjectIds.Crimsonfish, QualifiedObjectIds.SonOfCrimsonfish },
        { QualifiedObjectIds.Angler, QualifiedObjectIds.MsAngler },
        { QualifiedObjectIds.Legend, QualifiedObjectIds.LegendII },
        { QualifiedObjectIds.MutantCarp, QualifiedObjectIds.RadioactiveCarp },
        { QualifiedObjectIds.Glacierfish, QualifiedObjectIds.GlacierfishJr },
        { QualifiedObjectIds.SonOfCrimsonfish, QualifiedObjectIds.Crimsonfish },
        { QualifiedObjectIds.MsAngler, QualifiedObjectIds.Angler },
        { QualifiedObjectIds.LegendII, QualifiedObjectIds.Legend },
        { QualifiedObjectIds.RadioactiveCarp, QualifiedObjectIds.MutantCarp },
        { QualifiedObjectIds.GlacierfishJr, QualifiedObjectIds.Glacierfish },
        { "MNF.MoreNewFish_tui", "MNF.MoreNewFish_la" },
        { "MNF.MoreNewFish_la", "MNF.MoreNewFish_tui" },
    };
}
