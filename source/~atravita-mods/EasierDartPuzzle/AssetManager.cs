/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using EasierDartPuzzle.HarmonyPatches;
using StardewModdingAPI.Events;

namespace EasierDartPuzzle;

/// <summary>
/// Manages assets for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName stringsFromMaps = null!;

    /// <summary>
    /// Initialize the asset manager.
    /// </summary>
    /// <param name="parser">game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
        => stringsFromMaps = parser.ParseAssetName(@"Strings\StringsFromMaps");

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(stringsFromMaps))
        {
            e.Edit(ApplyImpl, AssetEditPriority.Late);
        }
    }

    private static void ApplyImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? data = asset.AsDictionary<string, string>();
        if (data.Data.TryGetValue("Pirates7_0", out var maxstring))
        {
            data.Data["Pirates7_0"] = maxstring.Replace("20", AdjustDartNumberTranspiler.GetMaximumDartNumber().ToString());
        }

        if (data.Data.TryGetValue("Pirates7_1", out var midstring))
        {
            data.Data["Pirates7_1"] = midstring.Replace("15", AdjustDartNumberTranspiler.GetMidddleDartNumber().ToString());
        }

        if (data.Data.TryGetValue("Pirates7_2", out var minstring))
        {
            data.Data["Pirates7_2"] = minstring.Replace("10", AdjustDartNumberTranspiler.GetMinimumDartNumber().ToString());
        }
    }
}
