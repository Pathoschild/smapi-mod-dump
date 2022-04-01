/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweaks.Framework;

#region using directives

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D HoneyMeadTx { get; set; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "BetterHoneyMeadIcons"));

    internal static bool TryGetSourceRectForMead(int preserveIndex, out Rectangle sourceRectangle)
    {
        sourceRectangle = preserveIndex switch
        {
            376 => new(48, 0, 16, 16), // poppy
            402 => new(16, 16, 16, 16), // sweet pea
            418 => new(0, 16, 16, 16), // crocus
            591 => new(0, 0, 16, 16), // tulip
            593 => new(32, 0, 16, 16), // summer spangle
            595 => new(64, 0, 16, 16), // fairy rose
            597 => new(16, 0, 16, 16), // blue jazz
            _ => new(32, 16, 16, 16)
        };

        return sourceRectangle != default;
    }
}