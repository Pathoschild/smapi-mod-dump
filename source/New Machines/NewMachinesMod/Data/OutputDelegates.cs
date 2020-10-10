/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    /// <param name="p">Base price of input item.</param>
    /// <param name="q">Quality of input item.</param>
    /// <param name="r1">Random value [0..1]</param>
    /// <param name="r2">Random value [0..1]</param>
    /// <returns>One of new values: 0, 1, 2, 3.</returns>
    public delegate int QualityExpression(int p, int q, double r1, double r2);

    /// <param name="p">Base price of input item.</param>
    /// <param name="q">Quality of input item.</param>
    /// <param name="r1">Random value [0..1]</param>
    /// <param name="r2">Random value [0..1]</param>
    /// <returns>Positive value.</returns>
    public delegate int CountExpression(int p, int q, double r1, double r2);

    /// <param name="p">Base price of input item.</param>
    /// <param name="q">Quality of input item.</param>
    /// <returns>Non-negative value</returns>
    public delegate int PriceExpression(int p, int q);
}
