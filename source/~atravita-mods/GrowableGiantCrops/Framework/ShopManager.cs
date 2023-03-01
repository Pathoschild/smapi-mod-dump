/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Diagnostics;

using AtraBase.Models.RentedArrayHelpers;
using AtraBase.Models.Result;
using AtraBase.Models.WeightedRandom;
using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.Caches;

using AtraShared.Caching;
using AtraShared.Menuing;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using GrowableGiantCrops.Framework.Assets;
using GrowableGiantCrops.Framework.InventoryModels;
using GrowableGiantCrops.HarmonyPatches.GrassPatches;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley.Menus;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// Manages shops for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:Readonly fields should appear before non-readonly fields", Justification = "Reviewed.")]
internal static class ShopManager
{
    private const string BUILDING = "Buildings";
    private const string RESOURCE_SHOP_NAME = "atravita.ResourceShop";
    private const string GIANT_CROP_SHOP_NAME = "atravita.GiantCropShop";

    private const string ROBIN_MAIL_TWO = $"{RESOURCE_SHOP_NAME}/Two";

    private static readonly PerScreen<bool> HaveSentAllRobinMail = new(() => false);

    private static readonly TickCache<bool> HasCompletedQiChallenge = new(() => FarmerHelpers.HasAnyFarmerRecievedFlag("qiChallengeComplete"));
    private static readonly TickCache<bool> PerfectFarm = new(() => FarmerHelpers.HasAnyFarmerRecievedFlag("Farm_Eternal"));

    #region shop state

    // giant crop shop state.
    private static readonly PerScreen<Dictionary<int, int>?> Stock = new();
    private static WeightedManager<int>? weighted;

    // node shop state.
    private static readonly PerScreen<Dictionary<int, int>?> NodeStock = new();
    private static int[] nodes = null!;

    #endregion

    #region assetnames

    private static IAssetName robinHouse = null!;
    private static IAssetName witchHouse = null!;

    private static IAssetName mail = null!;
    private static IAssetName dataObjectInfo = null!;

    #endregion

    private static StringUtils stringUtils = null!;

    /// <summary>
    /// Initializes the asset names.
    /// </summary>
    /// <param name="parser">Game Content Helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        robinHouse = parser.ParseAssetName("Maps/ScienceHouse");
        witchHouse = parser.ParseAssetName("Maps/WitchHut");
        mail = parser.ParseAssetName("Data/mail");
        dataObjectInfo = parser.ParseAssetName("Data/ObjectInformation");

        stringUtils = new(ModEntry.ModMonitor);

        HashSet<int> nodesList = new();
        foreach ((int index, string data) in Game1.objectInformation)
        {
            if (index == 290)
            {
                continue;
            }
            ReadOnlySpan<char> name = data.GetNthChunk('/');
            if (name.Equals("Stone", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Weeds", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Twig", StringComparison.OrdinalIgnoreCase))
            {
                nodesList.Add(index);
            }
        }

        nodes = nodesList.ToArray();
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void OnAssetInvalidated(IReadOnlySet<IAssetName>? assets)
    {
        if (assets is null || assets.Contains(dataObjectInfo))
        {
            weighted = null;
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(mail))
        {
            e.Edit(static (asset) =>
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data[RESOURCE_SHOP_NAME] = I18n.RobinMail();
                data[GIANT_CROP_SHOP_NAME] = I18n.WitchMail();
            });
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(robinHouse))
        {
            e.Edit(
                apply: static (asset) => asset.AsMap().AddTileProperty(
                    monitor: ModEntry.ModMonitor,
                    layer: BUILDING,
                    key: "Action",
                    property: RESOURCE_SHOP_NAME,
                    placementTile: ModEntry.Config.ResourceShopLocation),
                priority: AssetEditPriority.Default + 10);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(witchHouse))
        {
            e.Edit(
                apply: static (asset) => asset.AsMap().AddTileProperty(
                    monitor: ModEntry.ModMonitor,
                    layer: BUILDING,
                    key: "Action",
                    property: GIANT_CROP_SHOP_NAME,
                    placementTile: ModEntry.Config.GiantCropShopLocation),
                priority: AssetEditPriority.Default + 10);
        }
    }

    /// <summary>
    /// Adds a small bush graphic to the shop.
    /// </summary>
    /// <param name="e">On Warped event arguments.</param>
    internal static void AddBoxToShop(WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer || !ModEntry.Config.ShowShopGraphics)
        {
            return;
        }

        if (e.NewLocation.Name.Equals("ScienceHouse", StringComparison.OrdinalIgnoreCase) && Game1.player.hasOrWillReceiveMail(RESOURCE_SHOP_NAME))
        {
            Vector2 tile = ModEntry.Config.ResourceShopLocation;

            // add box
            e.NewLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                textureName: AssetManager.ShopGraphics.BaseName,
                sourceRect: new Rectangle(16, 16, 16, 32),
                position: new Vector2(tile.X, tile.Y - 2) * Game1.tileSize,
                flipped: false,
                alphaFade: 0f,
                color: Color.White)
            {
                animationLength = 1,
                sourceRectStartingPos = Vector2.One * 16,
                interval = 50000f,
                totalNumberOfLoops = 9999,
                scale = 4f,
                layerDepth = (((tile.Y - 0.5f) * Game1.tileSize) / 10000f) + 0.01f, // a little offset so it doesn't show up on the floor.
                id = 777f,
            });
        }
        else if (e.NewLocation.Name.Equals("WitchHut", StringComparison.OrdinalIgnoreCase))
        {
            Vector2 tile = ModEntry.Config.GiantCropShopLocation;

            // add box
            e.NewLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                textureName: AssetManager.ShopGraphics.BaseName,
                sourceRect: new Rectangle(32, 0, 32, 48),
                position: new Vector2(tile.X, tile.Y - 2) * Game1.tileSize,
                flipped: false,
                alphaFade: 0f,
                color: Color.White)
            {
                animationLength = 1,
                sourceRectStartingPos = Vector2.UnitX * 32,
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

        if (Game1.currentLocation.Name == "ScienceHouse"
            && Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) == RESOURCE_SHOP_NAME
            && Game1.player.hasOrWillReceiveMail(RESOURCE_SHOP_NAME))
        {
            input.SurpressClickInput();

            Dictionary<ISalable, int[]> sellables = new(ResourceClumpIndexesExtensions.Length);
            sellables.PopulateSellablesWithResourceClumps();

            ShopMenu shop = new(sellables, who: "Robin") { storeContext = RESOURCE_SHOP_NAME };
            if (NPCCache.GetByVillagerName("Robin") is NPC robin)
            {
                shop.portraitPerson = robin;
            }
            shop.potraitPersonDialogue = stringUtils.ParseAndWrapText(I18n.ShopMessage_Robin(), Game1.dialogueFont, 304);
            Game1.activeClickableMenu = shop;
        }
        else if (Game1.currentLocation.Name == "WitchHut"
            && Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) == GIANT_CROP_SHOP_NAME)
        {
            input.SurpressClickInput();

            Dictionary<ISalable, int[]> sellables = new();
            sellables.PopulateWitchShop();

            Game1.activeClickableMenu = new ShopMenu(sellables, on_purchase: TrackStock) { storeContext = GIANT_CROP_SHOP_NAME };
        }
    }

    /// <summary>
    /// Clears the shop state.
    /// </summary>
    internal static void Reset()
    {
        Stock.Value = null;
        NodeStock.Value = null;
    }

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    /// <remarks>Used to reset the shop inventory and send mail about the shops.</remarks>
    internal static void OnDayEnd()
    {
        Reset();

        // add Robin letter for tomorrow.
        if (!HaveSentAllRobinMail.Value
            && Game1.player.getFriendshipLevelForNPC("Robin") > 250
            && Game1.player.mailReceived.Contains("robinKitchenLetter"))
        {
            if (Game1.player.mailReceived.Contains(RESOURCE_SHOP_NAME))
            {
                Game1.addMailForTomorrow(mailName: RESOURCE_SHOP_NAME);
            }
            else if (Game1.player.hasSkullKey && !Game1.player.mailReceived.Contains(ROBIN_MAIL_TWO))
            {
                Game1.addMailForTomorrow(mailName: ROBIN_MAIL_TWO);
                HaveSentAllRobinMail.Value = true;
            }
        }

        // add Witch letter for tomorrow.
        if (Game1.player.hasMagicInk && !Game1.player.mailReceived.Contains(GIANT_CROP_SHOP_NAME))
        {
            Game1.addMailForTomorrow(mailName: GIANT_CROP_SHOP_NAME);
        }
    }

    private static bool TrackStock(ISalable salable, Farmer farmer, int count)
    {
        if (salable is InventoryGiantCrop crop && Stock.Value?.TryGetValue(crop.ParentSheetIndex, out int remaining) == true)
        {
            remaining -= count;
            if (remaining <= 0)
            {
                Stock.Value.Remove(crop.ParentSheetIndex);
            }
            else
            {
                Stock.Value[crop.ParentSheetIndex] = remaining;
            }
        }
        else if (salable.GetType() == typeof(SObject) && salable is SObject obj && !obj.bigCraftable.Value
            && NodeStock.Value?.TryGetValue(obj.ParentSheetIndex, out int remainder) == true)
        {
            remainder -= count;
            if (remainder <= 0)
            {
                NodeStock.Value.Remove(obj.ParentSheetIndex);
            }
            else
            {
                NodeStock.Value[obj.ParentSheetIndex] = remainder;
            }
        }
        return false; // do not want to yeet the menu.
    }

    private static void PopulateSellablesWithResourceClumps(this Dictionary<ISalable, int[]> sellables)
    {
        Debug.Assert(sellables is not null, "Sellables cannot be null.");

        if (Game1.player.hasSkullKey)
        {
            foreach (ResourceClumpIndexes clump in ResourceClumpIndexesExtensions.GetValues())
            {
                int[] sellData;
                if (clump == ResourceClumpIndexes.Invalid)
                {
                    continue;
                }
                else if (clump == ResourceClumpIndexes.Meteorite)
                {
                    if (HasCompletedQiChallenge.GetValue())
                    {
                        sellData = new[] { 10_000, ShopMenu.infiniteStock };
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    sellData = new[] { 7_500, ShopMenu.infiniteStock };
                }

                InventoryResourceClump clumpItem = new(clump, 1);
                _ = sellables.TryAdd(clumpItem, sellData);
            }
        }

        // grass
        foreach (GrassIndexes grass in GrassIndexesExtensions.GetValues())
        {
            if (grass == GrassIndexes.Invalid)
            {
                continue;
            }

            SObject grassStarter = new(SObjectPatches.GrassStarterIndex, 1);
            grassStarter.modData?.SetInt(SObjectPatches.ModDataKey, (int)grass);
            _ = sellables.TryAdd(grassStarter, new[] { 500, ShopMenu.infiniteStock });
        }
    }

    private static void PopulateWitchShop(this IDictionary<ISalable, int[]> sellables)
    {
        Debug.Assert(sellables is not null, "Sellables cannot be null.");

        if (!Game1.player.Items.Any(item => item is ShovelTool))
        {
            sellables.TryAdd(new ShovelTool(), new[] { 5_000, 1 });
        }

        const int GIANT_CROP_PRICE_MULITPLIER = 60;
        const int NODE_COST = 1_000;

        if (PerfectFarm.GetValue())
        {
            foreach (int idx in ModEntry.YieldAllGiantCropIndexes())
            {
                int price = GetPriceOfProduct(idx) ?? 0;
                _ = sellables.TryAdd(new InventoryGiantCrop(idx, 1), new int[] { Math.Max(price * GIANT_CROP_PRICE_MULITPLIER, 5_000), ShopMenu.infiniteStock });
            }

            foreach (int idx in nodes)
            {
                int price = PriceNode(idx) ?? NODE_COST;
                _ = sellables.TryAdd(new SObject(idx, 1) { MinutesUntilReady = 25, Edibility = SObject.inedible }, new[] { price, ShopMenu.infiniteStock });
            }
        }
        else
        {
            Stock.Value ??= GenerateDailyStock();
            if (Stock.Value is not null)
            {
                foreach ((int index, int count) in Stock.Value)
                {
                    if (count <= 0)
                    {
                        continue;
                    }

                    int price = GetPriceOfProduct(index) ?? 0;
                    _ = sellables.TryAdd(new InventoryGiantCrop(index, count), new int[] { Math.Max(price * GIANT_CROP_PRICE_MULITPLIER, 5_000), count });
                }
            }

            NodeStock.Value ??= GenerateNodeShop();
            if (NodeStock.Value is not null)
            {
                foreach ((int index, int count) in NodeStock.Value)
                {
                    if (count <= 0)
                    {
                        continue;
                    }
                    int price = PriceNode(index) ?? NODE_COST;
                    _ = sellables.TryAdd(new SObject(index, count) { MinutesUntilReady = 25, Edibility = SObject.inedible }, new[] { price, count });
                }
            }
        }

        foreach (TreeIndexes idx in TreeIndexesExtensions.GetValues())
        {
            if (idx == TreeIndexes.Invalid || idx == TreeIndexes.Mushroom)
            {
                continue;
            }
            _ = sellables.TryAdd(new InventoryTree(idx, 1, 3), new[] { 5_000, ShopMenu.infiniteStock });
        }
    }

    private static WeightedManager<int> GetWeightedManager()
    {
        WeightedManager<int> manager = new();

        foreach (int idx in ModEntry.YieldAllGiantCropIndexes())
        {
            int? price = GetPriceOfProduct(idx);
            if (price is not null)
            {
                manager.Add(new(2500d / Math.Clamp(price.Value, 50, 1250), idx));
            }
        }
        ModEntry.ModMonitor.DebugOnlyLog($"Got {manager.Count} giant crop entries for shop.", LogLevel.Info);
        return manager;
    }

    private static Dictionary<int, int>? GenerateNodeShop()
    {
        Dictionary<int, int> chosen = new(4);
        ShuffledYielder<int> shuffler = new(nodes);
        int total = 4;
        while (shuffler.MoveNext() && total-- > 0)
        {
            chosen[shuffler.Current] = 10;
        }
        ModEntry.ModMonitor.DebugOnlyLog($"Got {chosen.Count} node entries for shop.", LogLevel.Info);
        return chosen.Count > 0 ? chosen : null;
    }

    private static Dictionary<int, int>? GenerateDailyStock()
    {
        weighted ??= GetWeightedManager();
        if (weighted.Count == 0)
        {
            return null;
        }

        Dictionary<int, int> chosen = new();

        int totalCount = Math.Max(5, ModEntry.GetTotalValidIndexes() / 7);
        for (int i = 0; i < totalCount; i++)
        {
            Option<int> picked = weighted.GetValue();
            if (picked.IsNone)
            {
                continue;
            }

            int idx = picked.Unwrap();
            if (!chosen.TryGetValue(idx, out int prev))
            {
                prev = 0;
            }
            chosen[idx] = prev + 5;
        }

        ModEntry.ModMonitor.DebugOnlyLog($"Got {chosen.Count} giant crop entries for shop.", LogLevel.Info);
        return chosen;
    }

    private static int? GetPriceOfProduct(int idx)
    => Game1Wrappers.ObjectInfo.TryGetValue(idx, out string? info) &&
       int.TryParse(info.GetNthChunk('/', SObject.objectInfoPriceIndex), out int price)
       ? price
       : null;

    private static int? PriceNode(int idx)
        => Game1Wrappers.ObjectInfo.TryGetValue(idx, out string? info)
            ? info.GetNthChunk('/').Equals("Stone", StringComparison.OrdinalIgnoreCase) ? 2_750 : 1_000
            : null;
}
