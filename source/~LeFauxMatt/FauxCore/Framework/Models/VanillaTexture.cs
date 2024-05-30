/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>The raw data from a texture loaded from game content.</summary>
internal sealed class VanillaTexture : RawTexture
{
    /// <summary>Initializes a new instance of the <see cref="VanillaTexture" /> class.</summary>
    /// <param name="texture">The texture.</param>
    public VanillaTexture(Texture2D texture)
        : base(texture.Width, texture.Height, new Color[texture.Width * texture.Height]) =>
        texture.GetData(this.Data);
}