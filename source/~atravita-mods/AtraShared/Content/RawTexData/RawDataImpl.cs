/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtraShared.Content.RawTextData;

#warning - consider doing a from-file thing?

public static class IRawDataExtensions
{
    /// <summary>
    /// Copies the data out of a texture2D into a IRawTexture.
    /// </summary>
    /// <param name="texture">Texture to copy from.</param>
    /// <returns>IRawTextureData.</returns>
    /// <remarks>This is probably hellishly expensive.</remarks>
    public static IRawTextureData CopyToIRawTexture(this Texture2D texture)
    {
        Guard.IsNotNull(texture, nameof(texture));
        return new RawDataImpl(texture);
    }
}

/// <inheritdoc />
internal class RawDataImpl : IRawTextureData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RawDataImpl"/> class.
    /// </summary>
    /// <param name="texture">Texture to get data from.</param>
    /// <remarks>This is probably hellishly expensive.</remarks>
    internal RawDataImpl(Texture2D texture)
    {
        this.Data = GC.AllocateUninitializedArray<Color>(texture.Width * texture.Height);
        texture.GetData(this.Data);

        this.Width = texture.Width;
        this.Height = texture.Height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RawDataImpl"/> class.
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="data">Data.</param>
    internal RawDataImpl(int width, int height, Color[] data)
    {
        this.Width = width;
        this.Height = height;
        this.Data = data;
    }

    /// <inheritdoc />
    public int Width { get; init; }

    /// <inheritdoc />
    public int Height { get; init; }

    /// <inheritdoc />
    public Color[] Data { get; init; }
}
