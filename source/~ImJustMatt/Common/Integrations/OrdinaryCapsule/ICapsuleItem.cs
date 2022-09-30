/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.OrdinaryCapsule;

using System.Collections.Generic;

/// <summary>
///     Represents an item that can be duplicated in an Ordinary Capsule.
/// </summary>
public interface ICapsuleItem
{
    /// <summary>
    ///     Gets or sets the tag to identify supported item(s).
    /// </summary>
    public HashSet<string> ContextTags { get; set; }

    /// <summary>
    ///     Gets or sets the time between duplicating the item.
    /// </summary>
    public int ProductionTime { get; set; }

    /// <summary>
    ///     Gets or sets the sound to play when the item is added.
    /// </summary>
    public string? Sound { get; set; }
}