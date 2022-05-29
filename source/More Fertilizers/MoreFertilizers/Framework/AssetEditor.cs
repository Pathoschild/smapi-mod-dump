/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
internal static class AssetEditor
{
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private static readonly string JUNK_CATEGORY = $"Basic {SObject.junkCategory}";
    private static readonly string FERTILIZER_CATEGORY = $"Basic {SObject.fertilizerCategory}";
    private static readonly string OBJECTDATA = PathUtilities.NormalizeAssetName("Data/ObjectInformation");
#pragma warning restore SA1310 // Field names should not contain underscore

    /// <summary>
    /// Handles asset editing.
    /// </summary>
    /// <param name="e">Asset requested event arguments.</param>
    internal static void Edit(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(OBJECTDATA))
        {
            e.Edit(EditImpl, AssetEditPriority.Late);
        }
    }

    private static void EditImpl(IAssetData asset)
    {
        List<int> idsToEdit = new(ModEntry.PlantableFertilizerIDs);
        idsToEdit.AddRange(ModEntry.SpecialFertilizerIDs);

        IAssetDataForDictionary<int, string>? editor = asset.AsDictionary<int, string>();
        foreach (int item in idsToEdit)
        {
            if (editor.Data.TryGetValue(item, out string? val))
            {
                editor.Data[item] = val.Replace(JUNK_CATEGORY, FERTILIZER_CATEGORY);
            }
            else
            {
                ModEntry.ModMonitor.Log($"Could not find {item} in ObjectInformation to edit! This mod may not function properly.", LogLevel.Error);
            }
        }
    }
}