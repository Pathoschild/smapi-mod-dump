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
 * The following file was copied from: https://github.com/spacechase0/StardewValleyMods/blob/develop/MoreGiantCrops/IMoreGiantCropsAPI.cs.
 *
 * The original license is as follows:
 *
 * MIT License
 *
 * Copyright (c) 2021 Chase W
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 *
 * *******************************************/

using Microsoft.Xna.Framework.Graphics;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// API to interface with MoreGiantCrops
/// </summary>
/// <remarks>Copied from: https://github.com/spacechase0/StardewValleyMods/blob/develop/MoreGiantCrops/IMoreGiantCropsAPI.cs .</remarks>
public interface IMoreGiantCropsAPI
{
    /// <summary>
    /// Gets the texture associated with a MoreGiantCrops giant crop.
    /// </summary>
    /// <param name="productIndex">The index of the product.</param>
    /// <returns>Texture, or null if not recognized.</returns>
    public Texture2D? GetTexture(int productIndex);

    /// <summary>
    /// Gets the crops associated with More Giant crops.
    /// </summary>
    /// <returns>array of product indexes.</returns>
    public int[] RegisteredCrops();
}