/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes;

/// <summary>The ephemeral runtime state for Taxes.</summary>
internal sealed class State
{
    internal int LatestAmountDue { get; set; }

    internal int LatestAmountWithheld { get; set; }
}
