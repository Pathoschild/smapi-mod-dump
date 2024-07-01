/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CombinedRing_StatBuffer
{
    internal static ConditionalWeakTable<CombinedRing, StatBuffer> Values { get; } = new();

    internal static StatBuffer Get_StatBuffer(this CombinedRing combined)
    {
        return Values.GetValue(combined, Create);
    }

    private static StatBuffer Create(CombinedRing combined)
    {
        var buffer = new StatBuffer();
        combined.combinedRings.ForEach(r => Gemstone.FromRing(r.ParentSheetIndex).Buffer(buffer));
        combined.Get_Chord()?.Buffer(buffer);
        return buffer;
    }
}
