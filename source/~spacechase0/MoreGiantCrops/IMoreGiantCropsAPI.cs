/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

#nullable enable
using System;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

namespace MoreGiantCrops;
public interface IMoreGiantCropsAPI
{
    public Texture2D? GetTexture(int productIndex);

    public int[] RegisteredCrops();
}

public sealed class MoreGiantCropsAPI: IMoreGiantCropsAPI
{
    public Texture2D? GetTexture(int productIndex)
        => Mod.Sprites.TryGetValue(productIndex, out Lazy<Texture2D>? tex) ? tex.Value : null;

    public int[] RegisteredCrops() => Mod.Sprites.Keys.ToArray();
}
