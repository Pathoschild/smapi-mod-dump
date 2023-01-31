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

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;

using StardewModdingAPI.Events;

namespace TapGiantCrops.Framework;

/// <summary>
/// A data class indicating an SObject with optional preserve values.
/// </summary>
public sealed class ObjectDefinition
{
    /// <summary>
    /// Gets or sets identifier for the object.
    /// </summary>
    public string Object { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets identifier for the preserve.
    /// </summary>
    public string? Preserve { get; set; } = null;
}

/// <summary>
/// Manages assets for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:Readonly fields should appear before non-readonly fields", Justification = "Keeping relevant fields together.")]
internal static class AssetManager
{
    private static IAssetName assetName = null!;

    private static readonly Dictionary<int, SObject> OverridesCache = new();
    private static Lazy<Dictionary<int, ObjectDefinition>> overrides = new(() => Game1.content.Load<Dictionary<int, ObjectDefinition>>(assetName.BaseName));

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        assetName = parser.ParseAssetName("Mods/atravita.TapGiantCrops/TappedObjectOverride");
    }

    /// <summary>
    /// Gets the relevant override item given a certain input parent sheet index.
    /// </summary>
    /// <param name="input">ParentSheetIndex.</param>
    /// <returns>The tapper's product if an override is found.</returns>
    internal static SObject? GetOverrideItem(int input)
    {
        if (OverridesCache.TryGetValue(input, out SObject? obj))
        {
            return obj.getOne() as SObject;
        }

        if (overrides.Value.TryGetValue(input, out ObjectDefinition? objectDefinition))
        {
            if (!int.TryParse(objectDefinition.Object, out int id))
            {
                id = DataToItemMap.GetID(ItemTypeEnum.SObject, objectDefinition.Object);
            }

            if (id < 0)
            {
                ModEntry.ModMonitor.Log($"{objectDefinition.Object} corresponds to an object that could not be resolved", LogLevel.Warn);
                return null; // not valid
            }

            SObject ret = new(id, 1);

            if (objectDefinition.Preserve is not null)
            {
                if (!int.TryParse(objectDefinition.Preserve, out int preserveId))
                {
                    preserveId = DataToItemMap.GetID(ItemTypeEnum.SObject, objectDefinition.Preserve);
                }

                if (preserveId > 0)
                {
                    ret.preservedParentSheetIndex.Value = preserveId;
                }
            }

            OverridesCache[input] = ret;
            return ret.getOne() as SObject;
        }

        return null;
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void Load(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(assetName))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<int, ObjectDefinition>, AssetLoadPriority.Exclusive);
        }
    }

    /// <summary>
    /// Handles invalidations.
    /// </summary>
    /// <param name="assets">The assets to invalidate, or null to invalidate anyways.</param>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (assets is null || assets.Contains(assetName))
        {
            if (overrides.IsValueCreated)
            {
                overrides = new(() => Game1.content.Load<Dictionary<int, ObjectDefinition>>(assetName.BaseName));
            }
            OverridesCache.Clear();
        }
    }
}
