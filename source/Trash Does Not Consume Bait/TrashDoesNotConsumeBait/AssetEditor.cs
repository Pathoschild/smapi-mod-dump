/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/TrashDoesNotConsumeBait
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace TrashDoesNotConsumeBait;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
public static class AssetEditor
{
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private static readonly string FORGE_MENU_CHOICE = PathUtilities.NormalizeAssetName("Mods/atravita_ForgeMenuChoice_Tooltip_Data");
    private static readonly string SECRET_NOTE_LOCATION = PathUtilities.NormalizeAssetName("Data/SecretNotes");
#pragma warning restore SA1310 // Field names should not contain underscore

    /// <summary>
    /// Edits the secret note and ForgeMenuChoice's tooltips to match.
    /// </summary>
    /// <param name="e">Event params.</param>
    internal static void EditAssets(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(FORGE_MENU_CHOICE))
        {
            e.Edit(EditForgeMenu);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(SECRET_NOTE_LOCATION))
        {
            e.Edit(EditSecretNote);
        }
    }

    /// <summary>
    /// Asks SMAPI to invalidate the assets.
    /// </summary>
    internal static void Invalidate()
    {
        ModEntry.GameContentHelper.InvalidateCache(SECRET_NOTE_LOCATION);
        ModEntry.GameContentHelper.InvalidateCache(FORGE_MENU_CHOICE);
    }

    private static void EditForgeMenu(IAssetData editor)
    {
        IAssetDataForDictionary<string, string> data = editor.AsDictionary<string, string>();
        if (data.Data.TryGetValue("Preserving", out string? val))
        {
            data.Data["Preserving"] = val.Replace("50", ((1 - ModEntry.Config.ConsumeChancePreserving) * 100).ToString());
        }
        else
        {
            ModEntry.ModMonitor.Log("ForgeMenuChoice's Preserving key not found....", LogLevel.Debug);
        }
    }

    private static void EditSecretNote(IAssetData editor)
    {
        IAssetDataForDictionary<int, string> data = editor.AsDictionary<int, string>();
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