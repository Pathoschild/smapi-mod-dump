/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.Caches;

using AtraShared.Caching;
using AtraShared.Menuing;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;

using HarmonyLib;

using StardewModdingAPI.Events;
using StardewValley.Menus;

using xTile.Dimensions;
using xTile.ObjectModel;

using XTile = xTile.Tiles.Tile;

namespace GrowableBushes.Framework;

/// <summary>
/// Manages Caroline's bush shop.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class ShopManager
{
    private const string BUILDING = "Buildings";
    private const string SHOPNAME = "atravita.BushShop";

    private static IAssetName sunHouse = null!;
    private static IAssetName mail = null!;

    private static TickCache<bool> islandUnlocked = new(() => FarmerHelpers.HasAnyFarmerRecievedFlag("seenBoatJourney"));

    /// <summary>
    /// Initializes asset names.
    /// </summary>
    /// <param name="parser">asset name parser.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        sunHouse = parser.ParseAssetName("Maps/Sunroom");
        mail = parser.ParseAssetName("Data/mail");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(mail))
        {
            e.Edit(static (asset) =>
            {
                asset.AsDictionary<string, string>().Data[SHOPNAME] = I18n.Caroline_Mail();
            });
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(sunHouse))
        {
            e.Edit(
                static (asset) =>
                {
                    IAssetDataForMap? map = asset.AsMap();
                    XTile? tile = map.Data.GetLayer(BUILDING).PickTile(new Location((int)ModEntry.Config.ShopLocation.X * 64, (int)ModEntry.Config.ShopLocation.Y * 64), Game1.viewport.Size);
                    if (tile is null)
                    {
                        ModEntry.ModMonitor.Log($"Tile could not be edited for shop, please let atra know!", LogLevel.Warn);
                        return;
                    }
                    tile.Properties["Action"] = new PropertyValue(SHOPNAME);
                },
                AssetEditPriority.Default + 10);
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    internal static void OnDayEnd()
    {
        if (Game1.player.getFriendshipLevelForNPC("Caroline") > 1500
            && !Game1.player.mailReceived.Contains(SHOPNAME)
            && Game1.player.mailReceived.Contains("CarolineTea"))
        {
            Game1.addMailForTomorrow(mailName: SHOPNAME);
        }
    }

    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    internal static void OnButtonPressed(ButtonPressedEventArgs e, IInputHelper input)
    {
        if ((!e.Button.IsActionButton() && !e.Button.IsUseToolButton())
            || !MenuingExtensions.IsNormalGameplay())
        {
            return;
        }

        if (Game1.currentLocation.Name != "Sunroom"
            || Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) != SHOPNAME)
        {
            return;
        }

        input.SurpressClickInput();

        Dictionary<ISalable, int[]> sellables = new(BushSizesExtensions.Length);

        sellables.PopulateSellablesWithBushes();

        ShopMenu shop = new(sellables, who: "Caroline") { storeContext = SHOPNAME };

        if (NPCCache.GetByVillagerName("Caroline") is NPC caroline)
        {
            shop.portraitPerson = caroline;
        }
        shop.potraitPersonDialogue = I18n.Shop_Message();
        Game1.activeClickableMenu = shop;
    }

    /// <summary>
    /// Postfix to add bushes to the catalog.
    /// </summary>
    /// <param name="__result">shop inventory to add to.</param>
    [HarmonyPatch(nameof(Utility.getAllFurnituresForFree))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            __result.PopulateSellablesWithBushes();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to add bushes to the catalogue\n\n{ex}", LogLevel.Error);
        }
    }

    private static void PopulateSellablesWithBushes(this Dictionary<ISalable, int[]> sellables)
    {
        sellables ??= new();

        foreach (BushSizes bushIndex in BushSizesExtensions.GetValues())
        {
            int[] sellData;
            if (bushIndex is BushSizes.Invalid)
            {
                continue;
            }
            else if (bushIndex is BushSizes.Walnut or BushSizes.Harvested)
            {
                if (!islandUnlocked.GetValue())
                {
                    continue;
                }
                else
                {
                    sellData = new[] { 750, ShopMenu.infiniteStock };
                }
            }
            else if (bushIndex is BushSizes.Medium)
            {
                sellData = new[] { 750, ShopMenu.infiniteStock };
            }
            else
            {
                sellData = new[] { 300, ShopMenu.infiniteStock };
            }

            InventoryBush bush = new(bushIndex, 1);
            _ = sellables.TryAdd(bush, sellData);
        }
    }
}
