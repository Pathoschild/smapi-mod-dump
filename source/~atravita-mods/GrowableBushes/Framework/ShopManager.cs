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

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

using StardewValley.Locations;
using StardewValley.Menus;

using AtraUtils = AtraShared.Utils.Utils;

namespace GrowableBushes.Framework;

/// <summary>
/// Manages Caroline's bush shop.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class ShopManager
{
    private const string BUILDING = "Buildings";
    private const string SHOPNAME = "atravita.BushShop";

    private static readonly TickCache<bool> IslandUnlocked = new(() => FarmerHelpers.HasAnyFarmerRecievedFlag("seenBoatJourney"));

    private static IAssetName sunHouse = null!;
    private static IAssetName mail = null!;

    private static StringUtils stringUtils = null!;

    /// <summary>
    /// Initializes asset names.
    /// </summary>
    /// <param name="parser">asset name parser.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        sunHouse = parser.ParseAssetName("Maps/Sunroom");
        mail = parser.ParseAssetName("Data/mail");

        stringUtils = new(ModEntry.ModMonitor);
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
                apply: static (asset) => asset.AsMap().AddTileProperty(
                    monitor: ModEntry.ModMonitor,
                    layer: BUILDING,
                    key: "Action",
                    property: SHOPNAME,
                    placementTile: ModEntry.Config.ShopLocation),
                priority: AssetEditPriority.Default + 10);
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

    /// <summary>
    /// Adds a small bush graphic to the shop.
    /// </summary>
    /// <param name="e">On Warped event arguments.</param>
    internal static void AddSignToShop(WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer || !ModEntry.Config.ShowBushShopGraphic)
        {
            return;
        }

        if (e.NewLocation.Name.Equals("SunRoom", StringComparison.OrdinalIgnoreCase))
        {
            Vector2 tile = ModEntry.Config.ShopLocation;

            // add box
            e.NewLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                textureName: AssetManager.BushShopTexture.BaseName,
                sourceRect: new Rectangle(0, 0, 16, 48),
                position: new Vector2(tile.X, tile.Y - 1) * Game1.tileSize,
                flipped: false,
                alphaFade: 0f,
                color: Color.White)
            {
                animationLength = 1,
                sourceRectStartingPos = Vector2.Zero,
                interval = 50000f,
                totalNumberOfLoops = 9999,
                scale = 4f,
                layerDepth = (((tile.Y - 0.5f) * Game1.tileSize) / 10000f) + 0.01f, // a little offset so it doesn't show up on the floor.
                id = 777f,
            });
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
        shop.potraitPersonDialogue = stringUtils.ParseAndWrapText(I18n.Shop_Message(), Game1.dialogueFont, 304);
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
            if (!ModEntry.Config.BushesInFurnitureCatalogue)
            {
                return;
            }
            __result.PopulateSellablesWithBushes(free: true);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to add bushes to the catalogue\n\n{ex}", LogLevel.Error);
        }
    }

    private static void PopulateSellablesWithBushes(this Dictionary<ISalable, int[]> sellables, bool free = false)
    {
        foreach (BushSizes bushIndex in BushSizesExtensions.GetValues())
        {
            int[] sellData;
            if (bushIndex is BushSizes.Invalid)
            {
                continue;
            }
            else if (bushIndex is BushSizes.Walnut or BushSizes.Harvested)
            {
                if (IslandUnlocked.GetValue())
                {
                    sellData = new[] { free ? 0 : 1_500 * ModEntry.Config.ShopCostScale, ShopMenu.infiniteStock };
                }
                else
                {
                    continue;
                }
            }
            else if (bushIndex is BushSizes.Medium)
            {
                sellData = new[] { free ? 0 : 1_000 * ModEntry.Config.ShopCostScale, ShopMenu.infiniteStock };
            }
            else
            {
                sellData = new[] { free ? 0 : 500 * ModEntry.Config.ShopCostScale, ShopMenu.infiniteStock };
            }

            InventoryBush bush = new(bushIndex, 1);
            _ = sellables.TryAdd(bush, sellData);
        }
    }
}
