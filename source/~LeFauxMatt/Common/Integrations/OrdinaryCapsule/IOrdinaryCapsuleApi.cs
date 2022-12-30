/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.OrdinaryCapsule;

/// <summary>
///     API for Ordinary Capsule.
/// </summary>
public interface IOrdinaryCapsuleApi
{
    /// <summary>
    ///     Registers an item for use with Ordinary Capsule.
    /// </summary>
    /// <param name="item">The item(s) that can be duplicated..</param>
    public void RegisterItem(ICapsuleItem item);
}