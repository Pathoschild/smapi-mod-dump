/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule.Framework.Models;

using System.Collections.Generic;
using StardewMods.Common.Integrations.OrdinaryCapsule;

/// <inheritdoc />
internal sealed class CapsuleItem : ICapsuleItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CapsuleItem" /> class.
    /// </summary>
    /// <param name="contextTags">Tag(s) to identify the item.</param>
    /// <param name="productionTime">The time between each item duplication.</param>
    /// <param name="sound">The sound to play when item is loaded or collected.</param>
    public CapsuleItem(HashSet<string> contextTags, int productionTime, string? sound)
    {
        this.ContextTags = contextTags;
        this.ProductionTime = productionTime;
        this.Sound = !string.IsNullOrWhiteSpace(sound) ? sound : "select";
    }

    /// <inheritdoc />
    public HashSet<string> ContextTags { get; set; }

    /// <inheritdoc />
    public int ProductionTime { get; set; }

    /// <inheritdoc />
    public string? Sound { get; set; }
}