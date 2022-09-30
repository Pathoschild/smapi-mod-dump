/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework;

/// <summary>
///     Implementation of a Better Chest feature.
/// </summary>
internal interface IFeature
{
    /// <summary>
    ///     Subscribe to events and apply any Harmony patches.
    /// </summary>
    public void Activate();

    /// <summary>
    ///     Unsubscribe from events, and reverse any Harmony patches.
    /// </summary>
    public void Deactivate();
}