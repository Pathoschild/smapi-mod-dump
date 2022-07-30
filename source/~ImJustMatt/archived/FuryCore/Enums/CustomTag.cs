/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Enums;

using NetEscapades.EnumGenerators;
using StardewMods.FuryCore.Interfaces;

/// <summary>
///     Custom Item Context Tags that can be added by <see cref="ICustomTags" />.
/// </summary>
[EnumExtensions]
public enum CustomTag
{
    /// <summary>Context tag for Artifacts.</summary>
    CategoryArtifact,

    /// <summary>Context tag for Furniture.</summary>
    CategoryFurniture,

    /// <summary>Context tag for items that can be donated to the Community Center.</summary>
    DonateBundle,

    /// <summary>Context tag for items that can be donated to the Museum.</summary>
    DonateMuseum,
}