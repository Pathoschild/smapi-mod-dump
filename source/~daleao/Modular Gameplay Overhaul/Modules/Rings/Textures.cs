/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D RingsTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/rings");

    internal static Texture2D GemstonesTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/gemstones");

    internal static Texture2D ResonanceLightTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/lights/resonance");
}
