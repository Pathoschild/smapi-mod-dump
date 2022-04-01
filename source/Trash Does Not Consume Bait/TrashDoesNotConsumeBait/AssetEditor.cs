/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/TrashDoesNotConsumeBait
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace TrashDoesNotConsumeBait;

/// <inheritdoc />
public class AssetEditor : IAssetEditor
{
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private static readonly string FORGE_MENU_CHOICE = PathUtilities.NormalizeAssetName("Mods/atravita_ForgeMenuChoice_Tooltip_Data");
    private static readonly string SECRET_NOTE_LOCATION = PathUtilities.NormalizeAssetName("Data/SecretNotes");
#pragma warning restore SA1310 // Field names should not contain underscore

    private AssetEditor()
    {
    }

    /// <summary>
    /// Gets the instance of the asset editor for this mod.
    /// </summary>
    public static AssetEditor Instance { get; } = new();

    /// <inheritdoc/>
    public bool CanEdit<T>(IAssetInfo asset)
        => asset.AssetNameEquals(FORGE_MENU_CHOICE) || asset.AssetNameEquals(SECRET_NOTE_LOCATION);

    /// <inheritdoc/>
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(FORGE_MENU_CHOICE))
        {
            IAssetDataForDictionary<string, string> data = asset.AsDictionary<string, string>();
            if (data.Data.TryGetValue("Preserving", out string? val))
            {
                data.Data["Preserving"] = val.Replace("50", ((1 - ModEntry.Config.ConsumeChancePreserving) * 100).ToString());
            }
            else
            {
                ModEntry.ModMonitor.Log("ForgeMenuChoice's Preserving key not found....", LogLevel.Debug);
            }
        }
        else
        {
            IAssetDataForDictionary<int, string> data = asset.AsDictionary<int, string>();
            if (data.Data.TryGetValue(1008, out string? val))
            {
                data.Data[1008] = val.Replace("50", ((1 - ModEntry.Config.ConsumeChancePreserving) * 100).ToString());
            }
            else
            {
                ModEntry.ModMonitor.Log("Data for secret note 1008 not found?", LogLevel.Debug);
            }
        }
    }

    /// <summary>
    /// Asks SMAPI to invalidate the assets.
    /// </summary>
    internal static void Invalidate()
    {
        ModEntry.ContentHelper.InvalidateCache(SECRET_NOTE_LOCATION);
        ModEntry.ContentHelper.InvalidateCache(FORGE_MENU_CHOICE);
    }
}