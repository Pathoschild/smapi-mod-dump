/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework.Extensions;

#region using directives

using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
internal static class RandomExtensions
{
    /// <summary>Gets the item index of a random algae.</summary>
    /// <param name="random">The <see cref="Random"/> number generator.</param>
    /// <param name="bias">A particular type of algae that should be favored.</param>
    /// <returns>The <see cref="string"/> id of an algae <see cref="Item"/>.</returns>
    internal static string NextAlgae(this Random random, string? bias = null)
    {
        if (bias is not null && random.NextDouble() > 2d / 3d)
        {
            return bias;
        }

        return random.NextDouble() switch
        {
            > 2d / 3d => QualifiedObjectIds.GreenAlgae,
            > 1d / 3d => QualifiedObjectIds.Seaweed,
            _ => QualifiedObjectIds.WhiteAlgae,
        };
    }
}
