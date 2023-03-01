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
using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;

using IdentifiableCombinedRings.DataModels;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

namespace IdentifiableCombinedRings.Framework;

/// <summary>
/// Manages assets for this mod.
/// </summary>
internal static class AssetManager
{
    private static readonly Dictionary<RingPair, Lazy<Texture2D>> TextureOverrides = new();

    private static IAssetName RingLocation = null!;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">Game Content Helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        RingLocation = parser.ParseAssetName("Mods/atravita/IdentifiableCombinedRings/Data");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(RingLocation))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, RingDataModel>, AssetLoadPriority.Exclusive);
        }
    }

    /// <summary>
    /// Gets the override texture associated with a ring pair.
    /// </summary>
    /// <param name="pair">The ring pair. Note the smaller ID should be first.</param>
    /// <returns>Override texture if it exists.</returns>
    internal static Texture2D? GetOverrideTexture(RingPair pair) => TextureOverrides.TryGetValue(pair, out Lazy<Texture2D>? tex) ? tex.Value : null;

    /// <summary>
    /// Loads in the ring overrides.
    /// </summary>
    internal static void Load()
    {
        TextureOverrides.Clear();

        Dictionary<string, RingDataModel> models = Game1.content.Load<Dictionary<string, RingDataModel>>(RingLocation.BaseName);
        foreach (RingDataModel model in models.Values)
        {
            if (model.RingIdentifiers is not string identifiers
                || !identifiers.TrySplitOnce(',', out ReadOnlySpan<char> first, out ReadOnlySpan<char> second)
                || second.Contains(','))
            {
                Globals.ModMonitor.Log($"'{model.RingIdentifiers ?? string.Empty}' was not a valid identifier set, skipping.", LogLevel.Error);
                continue;
            }

            if (string.IsNullOrWhiteSpace(model.TextureLocation))
            {
                Globals.ModMonitor.Log($"Texture cannot be null or whitespace", LogLevel.Error);
                continue;
            }

            if (!TryParseToRing(first, out int firstring) || !TryParseToRing(second, out int secondring))
            {
                Globals.ModMonitor.Log($"'{identifiers}' refer to rings that could not be resolved, skipping.", LogLevel.Warn);
                continue;
            }

            if (firstring == secondring)
            {
                Globals.ModMonitor.Log($"'{identifiers}' refer to the same ring, skipping.", LogLevel.Warn);
                continue;
            }

            RingPair pair = firstring > secondring
                ? new(secondring, firstring)
                : new(firstring, secondring);

            TextureOverrides[pair] = new(() => Game1.content.Load<Texture2D>(model.TextureLocation));
        }
    }

    private static bool TryParseToRing(ReadOnlySpan<char> span, out int ringID)
    {
        span = span.Trim();
        if (int.TryParse(span, out ringID) && ringID > 0 && DataToItemMap.IsActuallyRing(ringID))
        {
            return true;
        }

        ringID = DataToItemMap.GetID(ItemTypeEnum.Ring, span.ToString());
        return ringID > 0;
    }
}
