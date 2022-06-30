/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Extensions;
using System.Collections;
using System.Collections.Generic;

#endregion using directives

/// <summary>Represents a pair of profession choices offered to the player during level-up.</summary>
/// <param name="First">The first profession in the pair.</param>
/// <param name="Second">The second profession in the pair.</param>
/// <param name="Requires">The level 5 profession from which this pair branches out of, if this is a level 10 pair.</param>
/// <param name="Level">Either <c>5</c> or <c>10</c>.</param>
public record ProfessionPair
    (IProfession First, IProfession Second, IProfession? Requires, int Level) : IEnumerable<IProfession>
{
    public IEnumerator<IProfession> GetEnumerator() => First.Collect(Second).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}