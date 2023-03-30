/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

namespace CritterRings.Framework;

/// <summary>
/// Manages assets for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName buffTextureLocation = null!;

    private static Lazy<Texture2D> buffTex = new(() => Game1.content.Load<Texture2D>(buffTextureLocation.BaseName));

    /// <summary>
    /// The location of the buff icon texture.
    /// </summary>
    internal static Texture2D BuffTexture => buffTex.Value;

    /// <summary>
    /// Initializes this asset manager.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        buffTextureLocation = parser.ParseAssetName("Mods/atravita/CritterRings/BuffIcon");
    }

    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(buffTextureLocation))
        {
            e.LoadFromModFile<Texture2D>("assets/bunnies_fast.png", AssetLoadPriority.Exclusive);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (buffTex.IsValueCreated && (assets is null || assets.Contains(buffTextureLocation)))
        {
            buffTex = new(static () => Game1.content.Load<Texture2D>(buffTextureLocation.BaseName));
        }
    }
}
