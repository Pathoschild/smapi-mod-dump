/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

namespace SpecialOrdersExtended.Managers;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "This is are record.")]
public record EmojiData(string AssetName, Point Location);

/// <summary>
/// Handles asset management for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName durationOverride = null!;

    /// <summary>
    /// Gets the assetname used to register emoji overrides.
    /// </summary>
    internal static IAssetName EmojiOverride { get; private set; } = null!;

    /// <summary>
    /// Gets the orders to make untimed.
    /// </summary>
    internal static Lazy<HashSet<string>> Untimed { get; private set; } = new(GetUntimed);

    /// <summary>
    /// Initializes assets for this mod.
    /// </summary>
    /// <param name="parser">Game Content Helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        durationOverride = parser.ParseAssetName("Mods/atravita_SpecialOrdersExtended_DurationOverride");
        EmojiOverride = parser.ParseAssetName("Mods/atravita_SpecialOrdersExtended_EmojiOverride");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void OnLoadAsset(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(durationOverride))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low);
        }
        else if(e.NameWithoutLocale.IsEquivalentTo(EmojiOverride))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, EmojiData>, AssetLoadPriority.Low);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (Untimed.IsValueCreated && (assets is null || assets.Contains(durationOverride)))
        {
            Untimed = new(GetUntimed);
        }
    }

    /// <summary>
    /// Gets the duration override dictionary.
    /// </summary>
    /// <returns>The duration override dictionary.</returns>
    internal static Dictionary<string, string> GetDurationOverride()
        => Game1.content.Load<Dictionary<string, string>>(durationOverride.BaseName);

    /// <summary>
    /// Gets the untiled special order quest keys I manage.
    /// </summary>
    /// <returns>Hashset of quest keys.</returns>
    private static HashSet<string> GetUntimed()
        => GetDurationOverride().Where(kvp => kvp.Value.AsSpan().Trim().Equals("-1", StringComparison.Ordinal))
                                .Select(kvp => kvp.Key).ToHashSet();
}