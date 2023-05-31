/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D GemSocketTx { get; set; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");

    internal static Texture2D ShieldTx { get; set; } = ModHelper.ModContent.Load<Texture2D>("assets/vfx/shield.png");

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/GemstoneSockets")))
        {
            GemSocketTx = ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets");
        }
    }
}
