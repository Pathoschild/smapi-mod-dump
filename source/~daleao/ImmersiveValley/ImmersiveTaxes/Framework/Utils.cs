/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Framework;

internal static class Utils
{
    /// <summary>Calculate the corresponding income tax percentage based on the specified income.</summary>
    /// <param name="income">The monthly income.</param>
    internal static float GetTaxBracket(int income) =>
        income switch
        {
            <= 9950 => 0.1f,
            <= 40525 => 0.12f,
            <= 86375 => 0.22f,
            <= 164925 => 0.24f,
            <= 209425 => 0.32f,
            <= 523600 => 0.35f,
            _ => 0.37f
        } * ModEntry.Config.IncomeTaxCeiling / 0.37f;
}