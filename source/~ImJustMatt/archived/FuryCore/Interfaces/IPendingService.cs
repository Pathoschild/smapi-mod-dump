/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Interfaces;

/// <summary>
///     An <see cref="IModService" /> which has not been instantiated yet.
/// </summary>
internal interface IPendingService
{
    /// <summary>
    ///     Forces the Lazy service value to be evaluated.
    /// </summary>
    public void ForceEvaluation();
}