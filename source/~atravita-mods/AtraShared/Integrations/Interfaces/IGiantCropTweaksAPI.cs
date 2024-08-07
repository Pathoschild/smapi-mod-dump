/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/*********************************************
 * The following file was copied from: https://github.com/KhloeLeclair/StardewMods/blob/main/GiantCropTweaks/IGiantCropTweaks.cs.
 *
 * The original license is as follows:

Copyright 2021 Khloe Leclair

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 * *******************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// A custom giant crop that may spawn in-game. This implementation is based
/// upon the new custom giant crops being added in 1.6, and once 1.6 is
/// released Giant Crop Tweaks will begin using the base game's data format
/// rather than its own implementation of the same.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "API was like this.")]
public interface IGiantCropData
{
    /// <summary>
    /// The unqualified item ID produced by the underlying crop (i.e. the 'index of harvest' field in <c>Data/Crops</c>). The giant crop has a chance of growing when there's a grid of fully-grown crops which produce this item ID in a grid of <see cref="TileSize"/> tiles.
    /// </summary>
    string ID { get; }

    /// <summary>
    /// The asset name for the texture containing the giant crop's sprite.
    /// </summary>
    string Texture { get; }

    /// <summary>
    /// The top-left pixel position of the sprite within the Texture, specified as a model with X and Y fields. Defaults to (0, 0).
    /// </summary>
    Point Corner { get; }

    /// <summary>
    /// The area in tiles occupied by the giant crop, specified as a model with X and Y fields. This affects both its sprite size (which should be 16 pixels per tile) and the grid of crops needed for it to grow. Note that giant crops are drawn with an extra tile's height. Defaults to (3, 3).
    /// </summary>
    Point TileSize { get; }

    /// <summary>
    /// The percentage chance a given grid of crops will grow into the giant crop each night, as a value between 0 (never) and 1 (always). Default 0.01.
    /// </summary>
    double Chance { get; }

    /// <summary>
    /// The item ID which is harvested when you break the giant crop. Defaults to the <see cref="ID"/>.
    /// </summary>
    string? HarvestedItemId { get; }

    /// <summary>
    /// The minimum number of the <see cref="HarvestedItemId"/> to drop when the giant crop is broken. Defaults to 15.
    /// </summary>
    int MinYields { get; }

    /// <summary>
    /// The maximum number of the <see cref="HarvestedItemId"/> to drop when the giant crop is broken. Default to 21.
    /// </summary>
    int MaxYields { get; }
}

/// <summary>
/// The main API interface for Giant Crop Tweaks.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "API was like this.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "API was like this.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:Element return value should be documented", Justification = "API was like this.")]
public interface IGiantCropTweaks
{
    /// <summary>
    /// A dictionary of giant crop data. This is an easy to read version of the
    /// game's <c>"Data\GiantCrops"</c> asset.
    /// </summary>
    IReadOnlyDictionary<string, IGiantCropData> GiantCrops { get; }

    /// <summary>
    /// Try to get a giant crop's texture.
    /// </summary>
    /// <param name="id">The ID of the giant crop.</param>
    bool TryGetTexture(string id, [NotNullWhen(true)] out Texture2D? texture);

    /// <summary>
    /// Try to get a giant crop's source rectangle.
    /// </summary>
    /// <param name="id">The ID of the giant crop.</param>
    bool TryGetSource(string id, [NotNullWhen(true)] out Rectangle? source);
}