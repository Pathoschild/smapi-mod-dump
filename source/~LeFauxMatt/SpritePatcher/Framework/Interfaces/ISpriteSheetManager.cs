/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Interfaces;

using Microsoft.Xna.Framework.Graphics;
using StardewMods.SpritePatcher.Framework.Models;

/// <summary>Build and cache textures from patch layers.</summary>
public interface ISpriteSheetManager
{
    /// <summary>Tries to get the texture data from the specified game path.</summary>
    /// <param name="path">The path of the texture.</param>
    /// <param name="texture">
    /// When this method returns, contains the texture data if successful, or null if the texture could
    /// not be found. This parameter is passed uninitialized.
    /// </param>
    /// <returns><c>true</c> if the texture data is successfully retrieved; otherwise, <c>false</c>.</returns>
    public bool TryGetTexture(string path, [NotNullWhen(true)] out IRawTextureData? texture);

    /// <summary>Tries to build a texture by combining multiple texture layers.</summary>
    /// <param name="sprite">The sprite requesting the patch.</param>
    /// <param name="spriteKey">A key for the original texture method.</param>
    /// <param name="texture">The base texture to use as a background.</param>
    /// <param name="patches">The list of texture layers to combine.</param>
    /// <param name="spriteSheet">When this method returns, contains the built texture if successful, otherwise null.</param>
    /// <returns>True if the texture was successfully built, otherwise false.</returns>
    public bool TryBuildSpriteSheet(
        ISprite sprite,
        SpriteKey spriteKey,
        Texture2D texture,
        IList<ISpritePatch> patches,
        [NotNullWhen(true)] out ISpriteSheet? spriteSheet);
}