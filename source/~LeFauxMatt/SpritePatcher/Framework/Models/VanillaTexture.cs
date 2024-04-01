/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <inheritdoc />
internal sealed class VanillaTexture : IRawTextureData
{
    private readonly string path;

    private Color[]? data;
    private int? height;
    private int? width;

    /// <summary>Initializes a new instance of the <see cref="VanillaTexture" /> class.</summary>
    /// <param name="path">The path to the texture.</param>
    public VanillaTexture(string path) => this.path = path;

    /// <inheritdoc />
    public Color[] Data
    {
        get
        {
            if (this.data is null)
            {
                this.Reinitialize();
            }

            return this.data;
        }
    }

    /// <inheritdoc />
    public int Width
    {
        get
        {
            if (this.width is null)
            {
                this.Reinitialize();
            }

            return this.width.Value;
        }
    }

    /// <inheritdoc />
    public int Height
    {
        get
        {
            if (this.height is null)
            {
                this.Reinitialize();
            }

            return this.height.Value;
        }
    }

    /// <summary>Clears the cache by setting the data, width, and height variables to null.</summary>
    public void ClearCache()
    {
        this.data = null;
        this.width = null;
        this.height = null;
    }

    [MemberNotNull(nameof(VanillaTexture.data), nameof(VanillaTexture.width), nameof(VanillaTexture.height))]
    private void Reinitialize()
    {
        var texture = Game1.content.Load<Texture2D>(this.path);
        this.width = texture.Width;
        this.height = texture.Height;
        this.data = new Color[texture.Width * texture.Height];
        texture.GetData(0, new Rectangle(0, 0, texture.Width, texture.Height), this.data, 0, this.data.Length);
    }
}