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

using AtraCore;
using AtraCore.Framework.Caches;
using AtraCore.Models;

using AtraShared.Caching;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.GameData;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Preference.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Effective constants are all caps.")]
internal static class AssetEditor
{
    /// <summary>
    /// The mail key for the organic veggies reward.
    /// </summary>
    internal const string ORGANICVEGGIEMAIL = "atravita_OrganicCrops_Reward";

    /// <summary>
    /// The letter key used for the bountiful bush fertilizer's unlock.
    /// </summary>
    internal const string BOUNTIFUL_BUSH_UNLOCK = "atravita_Bountiful_Bush";

    /// <summary>
    /// A letter used to tell the player that robin now sells fertilizer after George's leek special order.
    /// </summary>
    internal const string GEORGE_EVENT = "atravita_George_Letter";

    /// <summary>
    /// Our special orders.
    /// </summary>
    private static readonly Lazy<Dictionary<string, SpecialOrderData>> SpecialOrders = new(() =>
    {
        Dictionary<string, SpecialOrderData> ret = new();
        int i = 0;
        foreach (string? filename in Directory.GetFiles(PathUtilities.NormalizePath(ModEntry.DIRPATH + "/assets/special-orders/"), "*.json"))
        {
            try
            {
                Dictionary<string, SpecialOrderData> orders = ModEntry.ModContentHelper.Load<Dictionary<string, SpecialOrderData>>(Path.GetRelativePath(ModEntry.DIRPATH, filename));
                foreach ((string key, SpecialOrderData order) in orders)
                {
                    ret[key] = order;
                    i++;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"{filename} may not be valid:\n\n{ex}", LogLevel.Error);
            }
        }
        ModEntry.ModMonitor.Log($"Found {i} Special Orders");
        return ret;
    });

    private static readonly TickCache<bool> HasSeenBoat = new(static () => FarmerHelpers.HasAnyFarmerRecievedFlag("seenBoatJourney"));

    private static IAssetName SPECIAL_ORDERS_LOCATION = null!;
    private static IAssetName SPECIAL_ORDERS_STRINGS = null!;
    private static IAssetName MAIL = null!;
    private static IAssetName LEWIS_DIALOGUE = null!;
    private static IAssetName RADIOACTIVE_DENYLIST = null!;

    private static HashSet<int>? denylist = null;

    /// <summary>
    /// Initializes the AssetEditor.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        SPECIAL_ORDERS_LOCATION = parser.ParseAssetName("Data/SpecialOrders");
        SPECIAL_ORDERS_STRINGS = parser.ParseAssetName("Strings/SpecialOrderStrings");
        MAIL = parser.ParseAssetName("Data/mail");
        LEWIS_DIALOGUE = parser.ParseAssetName("Characters/Dialogue/Lewis");
        RADIOACTIVE_DENYLIST = parser.ParseAssetName("Mods/atravita/MoreFertilizers/RadioactiveDenylist");
    }

    /// <summary>
    /// Handles asset editing.
    /// </summary>
    /// <param name="e">Asset requested event arguments.</param>
    internal static void Edit(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(AtraCoreConstants.PrismaticMaskData))
        {
            e.Edit(EditPrismaticMasks);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(RADIOACTIVE_DENYLIST))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Exclusive);
        }
        else if (HasSeenBoat.GetValue())
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

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (assets is null || assets.Contains(RADIOACTIVE_DENYLIST))
        {
            denylist = null;
        }
    }

    /// <summary>
    /// Gets a hashset of ids that should be excluded from the radioactive fertilizer.
    /// </summary>
    /// <returns>ids to exclude.</returns>
    internal static HashSet<int> GetRadioactiveExclusions()
    {
        if (denylist is not null)
        {
            return denylist;
        }

        ModEntry.ModMonitor.DebugOnlyLog("Resolving radioactive fertilizer denylist", LogLevel.Info);

        HashSet<int> ret = new();
        foreach (string item in Game1.content.Load<Dictionary<string, string>>(RADIOACTIVE_DENYLIST.BaseName).Keys)
        {
            int? id = MFUtilities.ResolveID(item);
            if (id is not null)
            {
                ret.Add(id.Value);
            }
        }

        denylist = ret;
        return denylist;
    }

    /// <summary>
    /// Handles editing special order dialogue. This is separate so it's only
    /// registered if necessary.
    /// </summary>
    /// <param name="e">event args.</param>
    internal static void EditSpecialOrderDialogue(AssetRequestedEventArgs e)
    {
        if (HasSeenBoat.GetValue() && e.NameWithoutLocale.IsEquivalentTo(LEWIS_DIALOGUE))
        {
            e.Edit(EditLewisDialogueImpl, AssetEditPriority.Early);
        }
    }

    #region editors

    private static void EditPrismaticMasks(IAssetData asset)
    {
        IAssetDataForDictionary<string, DrawPrismaticModel>? editor = asset.AsDictionary<string, DrawPrismaticModel>();

        DrawPrismaticModel? prismatic = new()
        {
            ItemType = ItemTypeEnum.SObject,
            Identifier = "Prismatic Fertilizer - More Fertilizers",
        };

        if (!editor.Data.TryAdd(prismatic.Identifier, prismatic))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic fertilizer to DrawPrismatic", LogLevel.Warn);
        }
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
        editor.Data[ORGANICVEGGIEMAIL] = $"@,^{I18n.Specialorder_Organic_Mail_Text()}^^   --{NPCCache.GetByVillagerName("Lewis")?.displayName ?? I18n.Lewis()}%item bigobject 272 %% [#]{I18n.Specialorder_Organic_Mail_Text()}";
        editor.Data[GEORGE_EVENT] = $"{I18n.George_Mail()}%item object {ModEntry.SeedyFertilizerID} 5 %% [#]{I18n.George_Mail_Title()}";
        editor.Data[BOUNTIFUL_BUSH_UNLOCK] = $"{I18n.Bountiful_Bush_Mail()}%item object {ModEntry.BountifulBushID} 3 %% [#]{I18n.Bountiful_Bush_Mail_Title()}";
    }

    private static void EditLewisDialogueImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        editor.Data["atravita.OrganicCrops_InProgress"] = I18n.Specialorder_Organic_LewisInprogress();
    }

    #endregion
}