/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class Textures
{
    private static readonly Lazy<Texture2D> _energizedTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/energized.png"));

    private static Lazy<Texture2D> _gemSocketTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets"));

    internal static Texture2D EnergizedTx => _energizedTx.Value;

    internal static Texture2D GemSocketTx => _gemSocketTx.Value;

    internal static void Reload(IEnumerable<IAssetName> assets)
    {
        var names = assets.Select(a => a.BaseName).ToHashSet();
        if (names.Contains("GemstoneSockets"))
        {
            _gemSocketTx = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets"));
        }
    }
}
