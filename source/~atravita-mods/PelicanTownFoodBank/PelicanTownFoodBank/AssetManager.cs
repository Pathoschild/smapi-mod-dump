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
using StardewModdingAPI.Utilities;

namespace PelicanTownFoodBank;

/// <summary>
/// Manages assets for this mod.
/// </summary>
internal static class AssetManager
{
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private const string ASSET_PREFIX = "Mods/atravita_FoodBank_";
    private static readonly string DENYLIST_LOC = PathUtilities.NormalizeAssetName(ASSET_PREFIX + "Denylist");
#pragma warning restore SA1310 // Field names should not contain underscore

    private static IAssetName Denylist = null!;

    /// <summary>
    /// Initialize the asset manager.
    /// </summary>
    /// <param name="parser">Game Content Helper.</param>
    internal static void Initialize(IGameContentHelper parser)
        => Denylist = parser.ParseAssetName(DENYLIST_LOC);

    /// <summary>
    /// Applies the load.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void Load(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Denylist))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low);
        }
    }
}