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

using StardewModdingAPI.Events;

namespace LastDayToPlantRedux.Framework;

/// <summary>
/// Manages assets for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
internal static class AssetManager
{
    /// <summary>
    /// The mail flag used for this mod.
    /// </summary>
    internal const string MailFlag = "atravita_LastDayLetter";

    #region denylist and allowlist
    private static readonly HashSet<int> AllowedFertilizersValue = new();
    private static readonly HashSet<int> DeniedFertilizersValue = new();
    private static readonly HashSet<int> AllowedSeedsValue = new();
    private static readonly HashSet<int> DeniedSeedsValue = new();
    private static bool accessProcessed = false;

    /// <summary>
    /// Gets fertilizers that should always be allowed.
    /// </summary>
    internal static HashSet<int> AllowedFertilizers
    {
        get
        {
            ProcessAccessLists();
            return AllowedFertilizersValue;
        }
    }

    /// <summary>
    /// Gets fertilizers that should always be hidden.
    /// </summary>
    internal static HashSet<int> DeniedFertilizers
    {
        get
        {
            ProcessAccessLists();
            return DeniedFertilizersValue;
        }
    }

    /// <summary>
    /// Gets seeds that should always be allowed.
    /// </summary>
    internal static HashSet<int> AllowedSeeds
    {
        get
        {
            ProcessAccessLists();
            return AllowedSeedsValue;
        }
    }

    /// <summary>
    /// Gets seeds that should always be hidden.
    /// </summary>
    internal static HashSet<int> DeniedSeeds
    {
        get
        {
            ProcessAccessLists();
            return DeniedSeedsValue;
        }
    }

    #endregion

    /// <summary>
    /// The current mail for the player.
    /// </summary>
    private static string message = string.Empty;

    /// <summary>
    /// The location of our access identifier->access dictionary.
    /// </summary>
    private static IAssetName accessLists = null!;

    /// <summary>
    /// The data asset for objects.
    /// </summary>
    private static IAssetName objectInfoName = null!;

    /// <summary>
    /// Gets the data asset for mail.
    /// </summary>
    internal static IAssetName DataMail { get; private set; } = null!;

    /// <summary>
    /// Gets the data asset for Data/crops.
    /// </summary>
    internal static IAssetName CropName { get; private set; } = null!;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">the game content parser.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        DataMail = parser.ParseAssetName("Data/mail");
        CropName = parser.ParseAssetName("Data/Crops");
        objectInfoName = parser.ParseAssetName("Data/ObjectInformation");
        accessLists = parser.ParseAssetName("Mods/atravita.LastDayToPlantRedux/AccessControl");
    }

    /// <summary>
    /// Updates mail on the start of a new day.
    /// </summary>
    /// <returns>True if there's crops with their last day today.</returns>
    internal static bool UpdateOnDayStart()
    {
        (string message, bool showplayer) = CropAndFertilizerManager.GenerateMessageString();
        AssetManager.message = message;
        return showplayer;
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(accessLists))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(DataMail) && !string.IsNullOrWhiteSpace(message))
        {
            e.Edit(
            static (asset) =>
            {
                IDictionary<string, string>? data = asset.AsDictionary<string, string>().Data;
                data[MailFlag] = message;
            }, AssetEditPriority.Late);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void InvalidateCache(AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Contains(CropName))
        {
            CropAndFertilizerManager.RequestInvalidateCrops();
            AssetManager.accessProcessed = false;
        }
        if (e.NamesWithoutLocale.Contains(objectInfoName))
        {
            CropAndFertilizerManager.RequestInvalidateFertilizers();
            AssetManager.accessProcessed = false;
        }
        if (e.NamesWithoutLocale.Contains(accessLists))
        {
            AssetManager.accessProcessed = false;
        }
    }

    private static void ProcessAccessLists()
    {
        if (AssetManager.accessProcessed)
        {
            return;
        }

        AssetManager.accessProcessed = true;

        AllowedFertilizersValue.Clear();
        DeniedFertilizersValue.Clear();
        AllowedSeedsValue.Clear();
        DeniedSeedsValue.Clear();

        ModEntry.ModMonitor.Log("Processing access lists");

        foreach ((string item, string access) in Game1.content.Load<Dictionary<string, string>>(AssetManager.accessLists.BaseName))
        {
            (int id, int type)? tup = LDUtils.ResolveIDAndType(item);
            if (tup is null)
            {
                continue;
            }
            int id = tup.Value.id;
            int type = tup.Value.type;

            ReadOnlySpan<char> trimmed = access.AsSpan().Trim();
            bool isAllow = trimmed.Equals("Allow", StringComparison.OrdinalIgnoreCase);
            bool isDeny = !isAllow && trimmed.Equals("Deny", StringComparison.OrdinalIgnoreCase);

            if (!isAllow && !isDeny)
            {
                ModEntry.ModMonitor.Log($"Invalid access term {access}, skipping", LogLevel.Info);
                continue;
            }

            switch (type)
            {
                case SObject.SeedsCategory:
                    if (isAllow)
                    {
                        AllowedSeedsValue.Add(id);
                    }
                    else if (isDeny)
                    {
                        DeniedSeedsValue.Add(id);
                    }
                    break;
                case SObject.fertilizerCategory:
                    if (isAllow)
                    {
                        AllowedFertilizersValue.Add(id);
                    }
                    else if (isDeny)
                    {
                        DeniedFertilizersValue.Add(id);
                    }
                    break;
                default:
                    ModEntry.ModMonitor.Log($"{item} with {id} is type {type}, not a seed or fertilizer, skipping.", LogLevel.Info);
                    break;
            }
        }
    }
}
