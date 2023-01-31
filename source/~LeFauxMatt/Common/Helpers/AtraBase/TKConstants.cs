/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Helpers.AtraBase;

using System.Runtime.CompilerServices;

/// <summary>
///     A class that contains useful constants.
/// </summary>
public class TKConstants
{
    /// <summary>
    ///     For use when asking the compiler to both inline and aggressively optimize.
    /// </summary>
    public const MethodImplOptions
        Hot = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
}