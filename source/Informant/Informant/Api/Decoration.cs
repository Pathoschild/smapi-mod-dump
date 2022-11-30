/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace Slothsoft.Informant.Api; 

/// <summary>
/// A class that contains all information to decorate a vanilla tooltip.
/// </summary>
/// <param name="Texture">the texture to display.</param>
public record Decoration(Texture2D Texture) {
    /// <summary>
    /// Optionally displays a little number over the texture.
    /// </summary>
    public int? Counter { get; init; }
}