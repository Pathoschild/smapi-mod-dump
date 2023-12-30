/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.Resonance;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CombinedRing_Chord
{
    internal static ConditionalWeakTable<CombinedRing, Chord> Values { get; } = new();

    internal static Chord? Get_Chord(this CombinedRing combined)
    {
        return CombatModule.Config.RingsEnchantments.EnableGemstoneResonance && JsonAssetsIntegration.InfinityBandIndex.HasValue &&
               combined.ParentSheetIndex == JsonAssetsIntegration.InfinityBandIndex.Value && combined.combinedRings.Count >= 2
            ? Values.GetValue(combined, Create)
            : null;
    }

    private static Chord Create(CombinedRing combined)
    {
        var first = Gemstone.FromRing(combined.combinedRings[0].ParentSheetIndex);
        var second = Gemstone.FromRing(combined.combinedRings[1].ParentSheetIndex);
        if (combined.combinedRings.Count == 2)
        {
            return new Chord(first, second);
        }

        var third = Gemstone.FromRing(combined.combinedRings[2].ParentSheetIndex);
        if (combined.combinedRings.Count == 3)
        {
            return new Chord(first, second, third);
        }

        var fourth = Gemstone.FromRing(combined.combinedRings[3].ParentSheetIndex);
        return new Chord(first, second, third, fourth);
    }
}
