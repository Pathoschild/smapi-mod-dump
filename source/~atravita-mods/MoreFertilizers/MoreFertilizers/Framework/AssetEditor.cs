/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.GameData;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
internal static class AssetEditor
{
    /// <summary>
    /// The mail key for the organic veggies reward.
    /// </summary>
    internal const string ORGANICVEGGIEMAIL = "atravita_OrganicCrops_Reward";
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private static readonly string JUNK_CATEGORY = $"Basic {SObject.junkCategory}";
    private static readonly string FERTILIZER_CATEGORY = $"Basic {SObject.fertilizerCategory}";
    private static readonly string OBJECTDATA = PathUtilities.NormalizeAssetName("Data/ObjectInformation");

    private static readonly string SPECIAL_ORDERS_LOCATION = PathUtilities.NormalizeAssetName("Data/SpecialOrders");
    private static readonly string SPECIAL_ORDERS_STRINGS = PathUtilities.NormalizeAssetName("Strings/SpecialOrderStrings");
    private static readonly string MAIL = PathUtilities.NormalizeAssetName("Data/mail");
    private static readonly string LEWIS_DIALOGUE = PathUtilities.NormalizeAssetName("Characters/Dialogue/Lewis");
#pragma warning restore SA1310 // Field names should not contain underscore

    private static readonly Lazy<Dictionary<string, SpecialOrderData>> SpecialOrders = new(() =>
    {
        Dictionary<string, SpecialOrderData> ret = new();
        int i = 0;
        foreach (string? filename in Directory.GetFiles(PathUtilities.NormalizePath(ModEntry.DIRPATH + "/assets/special-orders/"), "*.json"))
        {
            Dictionary<string, SpecialOrderData> orders = ModEntry.ModContentHelper.Load<Dictionary<string, SpecialOrderData>>(Path.GetRelativePath(ModEntry.DIRPATH, filename));
            foreach ((string key, SpecialOrderData order) in orders)
            {
                ret[key] = order;
                i++;
            }
        }
        ModEntry.ModMonitor.Log($"Found {i} Special Orders");
        return ret;
    });

    /// <summary>
    /// Handles asset editing.
    /// </summary>
    /// <param name="e">Asset requested event arguments.</param>
    internal static void Edit(AssetRequestedEventArgs e)
    {
        if ((ModEntry.PlantableFertilizerIDs.Count > 0 || ModEntry.SpecialFertilizerIDs.Count > 0) && e.NameWithoutLocale.IsEquivalentTo(OBJECTDATA))
        {
            e.Edit(EditSObjectsImpl, AssetEditPriority.Late);
        }
        else if (Utility.doesAnyFarmerHaveOrWillReceiveMail("seenBoatJourney"))
        {
            if (e.NameWithoutLocale.IsEquivalentTo(SPECIAL_ORDERS_LOCATION))
            {
                e.Edit(EditSpecialOrdersImpl, AssetEditPriority.Early);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(SPECIAL_ORDERS_STRINGS))
            {
                e.Edit(EditSpecialOrdersStringsImpl, AssetEditPriority.Early);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(MAIL))
            {
                e.Edit(EditMailImpl, AssetEditPriority.Early);
            }
        }
    }

    /// <summary>
    /// Handles editing special order dialogue. This is seperate so it's only
    /// registed if necessary.
    /// </summary>
    /// <param name="e">event args.</param>
    internal static void EditSpecialOrderDialogue(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(LEWIS_DIALOGUE) && Utility.doesAnyFarmerHaveOrWillReceiveMail("seenBoatJourney"))
        {
            e.Edit(EditLewisDialogueImpl, AssetEditPriority.Early);
        }
    }

    private static void EditSObjectsImpl(IAssetData asset)
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
        ModEntry.ModMonitor.Log($"Edited {idsToEdit.Count} fertilizers");
    }

    private static void EditSpecialOrdersImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, SpecialOrderData>? editor = asset.AsDictionary<string, SpecialOrderData>();
        foreach ((string key, SpecialOrderData order) in SpecialOrders.Value)
        {
            editor.Data[key] = order;
        }
    }

    private static void EditSpecialOrdersStringsImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        editor.Data["atravita.OrganicCrops.Name"] = I18n.Specialorder_Organic_Name();
        editor.Data["atravita.OrganicCrops.Text"] = I18n.Specialorder_Organic_Text();
        editor.Data["atravita.OrganicCrops.gather"] = I18n.Specialorder_Organic_Gather();
        editor.Data["atravita.OrganicCrops.ship"] = I18n.Specialorder_Organic_Ship();
    }

    private static void EditMailImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        editor.Data[ORGANICVEGGIEMAIL] = $"@,^{I18n.Specialorder_Organic_Mail_Text()}^^   --{Game1.getCharacterFromName("Lewis")?.displayName ?? I18n.Lewis()}%item bigobject 272 %%[#]{I18n.Specialorder_Organic_Mail_Text()}";
    }

    private static void EditLewisDialogueImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        editor.Data["atravita.OrganicCrops_InProgress"] = I18n.Specialorder_Organic_LewisInprogress();
    }
}