/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// Manages loading and editing assets for this mod.
/// </summary>
internal static class AssetManager
{
    /// <summary>
    /// Gets the IAssetName corresponding to the shovel's texture.
    /// </summary>
    internal static IAssetName ToolTextureName { get; private set; } = null!;

    private static Lazy<Texture2D> toolTex = new(() => Game1.content.Load<Texture2D>(ToolTextureName.BaseName));

    internal static Texture2D ToolTexture => toolTex.Value;

    /// <summary>
    /// Initializes the AssetManager.
    /// </summary>
    /// <param name="parser">GameContent helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        ToolTextureName = parser.ParseAssetName("Mods/atravita.GrowableGiantCrops/Shovel");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested" />
    internal static void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(ToolTextureName))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/shovel.png", AssetLoadPriority.Exclusive);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated" />
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if ((assets is null || assets.Contains(ToolTextureName)) && toolTex.IsValueCreated)
        {
            toolTex = new(() => Game1.content.Load<Texture2D>(ToolTextureName.BaseName));
        }
    }
}
