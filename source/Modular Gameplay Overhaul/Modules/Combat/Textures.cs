/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D GemstonesTx { get; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/objects/gemstones");

    internal static Texture2D GemSocketTx { get; set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");

    internal static Texture2D RingsTx { get; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/objects/rings");

    internal static Texture2D ShieldTx { get; set; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/shield");

    internal static Texture2D PatternedResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/resonance_patterned");

    internal static Texture2D StrongerResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/resonance_stronger");

    internal static Texture2D EnergizedTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/energized_hd");

    internal static Texture2D TooltipsTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/interface/tooltips");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/GemstoneSockets")))
        {
            GemSocketTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");
        }
    }
}
