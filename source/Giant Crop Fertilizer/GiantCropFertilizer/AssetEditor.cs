/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace GiantCropFertilizer;

/// <inheritdoc />
internal class AssetEditor : IAssetEditor
{
    private static readonly string OBJECTDATA = PathUtilities.NormalizeAssetName("Data/ObjectInformation");

    private AssetEditor()
    {
    }

    /// <summary>
    /// Gets the isntance of this asseteditor.
    /// </summary>
    internal static AssetEditor Instance { get; } = new();

    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
        => asset.AssetNameEquals(OBJECTDATA);

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (ModEntry.GiantCropFertilizerID != -1)
        {
            IAssetDataForDictionary<int, string>? editor = asset.AsDictionary<int, string>();
            if (editor.Data.TryGetValue(ModEntry.GiantCropFertilizerID, out string? val))
            {
                editor.Data[ModEntry.GiantCropFertilizerID] = val.Replace("Basic -20", "Basic -19");
            }
            else
            {
                ModEntry.ModMonitor.Log($"Could not find {ModEntry.GiantCropFertilizerID} in ObjectInformation to edit! This mod may not function properly.", LogLevel.Error);
            }
        }
    }
}