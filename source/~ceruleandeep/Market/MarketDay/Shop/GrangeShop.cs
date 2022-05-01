/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MarketDay.ItemPriceAndStock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace MarketDay.Shop
{
    public class GrangeShop : ItemShop
    {
        private const int WOOD_SIGN = 37;
        private const double BUY_CHANCE = 0.25;

        private const int DisplayChestHidingOffsetY = 36;

        public Vector2 Origin => StockChest.TileLocation - new Vector2(3, 1);

        public const string GrangeChestKey = "GrangeDisplay";
        public Chest GrangeChest => FindDisplayChest();

        public const string StockChestKey = "GrangeStorage";
        public Chest StockChest => FindStorageChest();

        public const string ShopSignKey = "GrangeSign";
        public Sign ShopSign => FindSign();

        public const string VisitorsTodayKey = "VisitorsToday";
        public const string GrumpyVisitorsTodayKey = "GrumpyVisitorsToday";

        public Dictionary<NPC, int> recentlyLooked = new();
        public Dictionary<NPC, int> recentlyTended = new();
        public List<SalesRecord> Sales = new();

        public Texture2D OpenSign;
        public Texture2D ClosedSign;

        // state queries
        public bool IsPlayerShop()
        {
            return ShopName == "Player";
        }

        private static bool OutsideOpeningHours =>
            Game1.timeOfDay < MarketDay.Config.OpeningTime * 100 ||
            Game1.timeOfDay > MarketDay.Config.ClosingTime * 100;

        public void OpenAt(Vector2 Origin)
        {
            Debug.Assert(Context.IsMainPlayer, "OpenAt: only main player can open shops");

            Log($"Opening at {Origin}", LogLevel.Trace);

            MakeFurniture(Origin);

            recentlyLooked = new Dictionary<NPC, int>();
            recentlyTended = new Dictionary<NPC, int>();
            Sales = new List<SalesRecord>();

            StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{VisitorsTodayKey}"] = "0";
            StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{GrumpyVisitorsTodayKey}"] = "0";

            if (!IsPlayerShop())
            {
                StockChestForTheDay();
                RestockGrangeFromChest(true);
            }

            DecorateFurniture();
        }

        private static Vector2 AvailableTileForFurniture(int minX, int minY)
        {
            var location = Game1.getLocationFromName("Town");
            var freeTile = new Vector2(minX, minY);
            while (location.Objects.ContainsKey(freeTile))
            {
                freeTile += Vector2.UnitX;
            }

            return freeTile;
        }


        private Vector2 OwnerPosition()
        {
            return Origin + new Vector2(3, 2);
        }

        public void RestockThroughDay(bool IsMarketDay)
        {
            if (!Context.IsMainPlayer) return;
            if (!IsMarketDay) return;

            if (IsPlayerShop())
            {
                if (MarketDay.Config.RestockItemsPerHour > 0) RestockGrangeFromChest();
            }
            else
            {
                RestockGrangeFromChest();
            }
        }

        public void EmptyGrangeAndDestroyFurniture()
        {
            if (!Context.IsMainPlayer) return;

            Log($"    EmptyGrangeAndDestroyFurniture: IsPlayerShop: {IsPlayerShop()}", LogLevel.Trace);

            if (IsPlayerShop())
            {
                EmptyStoreIntoChest();
                EmptyPlayerDayStorageChest();
            }

            DestroyFurniture();
        }

        private void EmptyPlayerDayStorageChest()
        {
            if (StockChest is null)
            {
                Log("EmptyPlayerDayStorageChest: DayStockChest is null", LogLevel.Error);
                return;
            }

            if (StockChest.items.Count <= 0) return;
            for (var i = 0; i < StockChest.items.Count; i++)
            {
                var item = StockChest.items[i];
                StockChest.items[i] = null;
                if (item == null) continue;
                Game1.player.team.returnedDonations.Add(item);
                Game1.player.team.newLostAndFoundItems.Value = true;
            }
        }

        public void CheckForBrowsingNPCs()
        {
            if (OutsideOpeningHours) return;

            Debug.Assert(Context.IsMainPlayer, "CheckForBrowsingNPCs: only main player can access recentlyLooked");
            foreach (var npc in NearbyNPCs().Where(npc => npc.movementPause <= 0 && !RecentlyBought(npc)))
            {
                if (recentlyLooked.TryGetValue(npc, out var time))
                {
                    if (Game1.timeOfDay - time < 100) continue;
                }

                npc.Halt();
                npc.faceDirection(0);
                npc.movementPause = 5000;
                recentlyLooked[npc] = Game1.timeOfDay;

                var visitors =
                    int.Parse(StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{VisitorsTodayKey}"]);
                visitors++;
                StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{VisitorsTodayKey}"] = $"{visitors}";
            }
        }

        internal void InteractWithNearbyNPCs()
        {
            if (!Context.IsMainPlayer) return;
            CheckForBrowsingNPCs();
            SellSomethingToOnlookers();
            SeeIfOwnerIsAround();
        }

        internal void OnActionButton(ButtonPressedEventArgs e)
        {
            if (GrangeChest is null)
            {
                Log($"DisplayChest is null", LogLevel.Error);
                return;
            }

            if (IsPlayerShop())
            {
                Game1.activeClickableMenu = new StorageContainer(GrangeChest.items, 9, 3, onGrangeChange,
                    StardewValley.Utility.highlightSmallObjects);
            }
            else DisplayShop();

            MarketDay.SMod.Helper.Input.Suppress(e.Button);
        }


        /// <summary>
        /// Opens the shop if conditions are met. If not, display the closed message
        /// </summary>
        public void DisplayShop(bool debug = false)
        {
            MarketDay.Log($"Attempting to open the shop \"{ShopName}\" at {Game1.timeOfDay}", LogLevel.Debug, true);

            if (!debug && OutsideOpeningHours)
            {
                if (ClosedMessage != null)
                {
                    Game1.activeClickableMenu = new DialogueBox(ClosedMessage);
                }
                else
                {
                    var openingTime = (MarketDay.Config.OpeningTime * 100).ToString();
                    openingTime = openingTime[..^2] + ":" + openingTime[^2..];

                    var closingTime = (MarketDay.Config.ClosingTime * 100).ToString();
                    closingTime = closingTime[..^2] + ":" + closingTime[^2..];

                    Game1.activeClickableMenu = new DialogueBox(Get("closed", new {openingTime, closingTime}));
                }

                return;
            }

            var currency = StoreCurrency switch
            {
                "festivalScore" => 1,
                "clubCoins" => 2,
                _ => 0
            };

            var shopMenu = new ShopMenu(ShopStock(), currency, null, OnPurchase);

            if (CategoriesToSellHere != null)
                shopMenu.categoriesToSellHere = CategoriesToSellHere;

            if (_portrait is null)
            {
                // try to load portrait from NpcName
                var npc = Game1.getCharacterFromName(NpcName);
                if (npc is not null) _portrait = npc.Portrait;
            }

            if (_portrait != null)
            {
                shopMenu.portraitPerson = new NPC();
                //only add a shop name the first time store is open each day so that items added from JA's side are only added once
                if (!_shopOpenedToday) shopMenu.portraitPerson.Name = "MD." + ShopName;
                shopMenu.portraitPerson.Portrait = _portrait;
            }

            if (Quote != null)
            {
                shopMenu.potraitPersonDialogue = Game1.parseText(Quote, Game1.dialogueFont, 304);
            }

            Game1.activeClickableMenu = shopMenu;
            _shopOpenedToday = true;
        }


        private bool OnPurchase(ISalable item, Farmer who, int stack)
        {
            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (item is not Item i) continue;
                if (GrangeChest.items[j] is null) continue;
                if (GrangeChest.items[j].ParentSheetIndex != i.ParentSheetIndex) continue;
                GrangeChest.items[j] = null;
                return false;
            }

            return false;
        }

        private int[] getSellPriceArrayFromShopStock(Item item)
        {
            foreach (var (stockItem, priceAndQty) in StockManager.ItemPriceAndStock)
            {
                if (item.ParentSheetIndex != ((Item) stockItem).ParentSheetIndex || item.Category != ((Item) stockItem).Category) continue;
                return priceAndQty;
            }
            return null;
        }

        /// <summary>
        /// Generate a dictionary of goods for sale and quantities, to be consumed by the game's shop menu
        /// </summary>
        private Dictionary<ISalable, int[]> ShopStock()
        {
            // this needs to filter StockManager.ItemPriceAndStock
            var stock = new Dictionary<ISalable, int[]>();

            if (GrangeChest is null)
            {
                Log("ShopStock: DisplayChest is null", LogLevel.Warn);
                return stock;
            }

            foreach (var stockItem in GrangeChest.items)
            {
                if (stockItem is null) continue;
                
                // var price = getSellPriceFromShopStock(stockItem);
                
                var price = getSellPriceArrayFromShopStock(stockItem) 
                            ?? new[] {
                                (int) (StardewValley.Utility.getSellToStorePriceOfItem(stockItem, false) * SellPriceMultiplier(stockItem, null))};

                var sellItem = stockItem.getOne();
                sellItem.Stack = 1;
                if (sellItem is Object sellObj && stockItem is Object stockObj) sellObj.Quality = stockObj.Quality;

                stock[sellItem] = price;
            }

            return stock;
        }

        private void SeeIfOwnerIsAround()
        {
            if (OutsideOpeningHours) return;

            Debug.Assert(Context.IsMainPlayer, "SeeIfOwnerIsAround: only main player can access recentlyTended");

            var owner = OwnerNearby();
            if (owner is null) return;

            // busy
            if (owner.movementPause is < 10000 and > 0) return;

            // already tended
            if (recentlyTended.TryGetValue(owner, out var time))
            {
                if (time > Game1.timeOfDay - 100) return;
            }

            owner.Halt();
            owner.faceDirection(2);
            owner.movementPause = 10000;

            var dialog = Get("spruik");
            owner.showTextAboveHead(dialog, -1, 2, 1000);
            owner.Sprite.UpdateSourceRect();

            recentlyTended[owner] = Game1.timeOfDay;
        }

        private NPC OwnerNearby()
        {
            var (ownerX, ownerY) = OwnerPosition();
            var location = Game1.getLocationFromName("Town");
            var npc = location.characters.FirstOrDefault(n => n.Name == ShopName);
            if (npc is null) return null;
            if (npc.getTileX() == (int) ownerX && npc.getTileY() == (int) ownerY) return npc;
            return null;
        }

        private void SellSomethingToOnlookers()
        {
            if (OutsideOpeningHours) return;

            foreach (var npc in NearbyNPCs())
            {
                // the owner
                if (npc.Name == ShopName) continue;

                // busy looking
                if (npc.movementPause is > 2000 or < 500) continue;

                // already bought
                if (RecentlyBought(npc)) continue;

                // unlucky
                if (Game1.random.NextDouble() < BUY_CHANCE + Game1.player.DailyLuck) continue;

                // check stock                
                // also remove items the NPC dislikes
                var available = GrangeChest.items.Where(gi => gi is not null && ItemPreferenceIndex(gi, npc) > 0)
                    .ToList();
                if (available.Count == 0)
                {
                    // no stock
                    var grumpy =
                        int.Parse(StockChest.modData[
                            $"{MarketDay.SMod.ModManifest.UniqueID}/{GrumpyVisitorsTodayKey}"]);
                    grumpy++;
                    StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{GrumpyVisitorsTodayKey}"] = $"{grumpy}";

                    npc.doEmote(12);
                    return;
                }

                // find what the NPC likes best
                available.Sort((a, b) =>
                    ItemPreferenceIndex(a, npc).CompareTo(ItemPreferenceIndex(b, npc)));

                var i = GrangeChest.items.IndexOf(available[0]);
                var item = GrangeChest.items[i];

                // buy it
                if (IsPlayerShop()) AddToPlayerFunds(item, npc);
                GrangeChest.items[i] = null;

                EmoteForPurchase(npc, item);
            }
        }


        private static void EmoteForPurchase(NPC npc, Item item)
        {
            if (Game1.random.NextDouble() < 0.25)
            {
                npc.doEmote(20);
            }
            else
            {
                int taste;
                try
                {
                    // so many other mods hack up NPCs that we have to wrap this
                    taste = npc.getGiftTasteForThisItem(item);
                }
                catch (Exception)
                {
                    taste = NPC.gift_taste_neutral;
                }
                
                string dialog = Get("buy", new {ItemName = item.DisplayName});
                if (taste == NPC.gift_taste_love)
                {
                    dialog = Get("love", new {ItemName = item.DisplayName});
                }
                else if (taste == NPC.gift_taste_like)
                {
                    dialog = Get("like", new {ItemName = item.DisplayName});
                }
                else if (item is Object o)
                {
                    dialog = o.Quality switch
                    {
                        Object.bestQuality => Get("iridium", new {ItemName = item.DisplayName}),
                        Object.highQuality => Get("gold", new {ItemName = item.DisplayName}),
                        Object.medQuality => Get("silver", new {ItemName = item.DisplayName}),
                        _ => dialog
                    };
                }

                npc.showTextAboveHead(dialog, -1, 2, 1000);
                npc.Sprite.UpdateSourceRect();
            }
        }

        private void AddToPlayerFunds(Item item, NPC npc)
        {
            Debug.Assert(Context.IsMainPlayer, "AddToPlayerFunds: only main player can access Sales");

            var mult = SellPriceMultiplier(item, npc);
            var obj = (Object) item;
            if (obj is null) return;

            var salePrice = obj.sellToStorePrice();

            salePrice = Convert.ToInt32(salePrice * mult);

            Game1.player.Money += salePrice;
            Log($"Item {item.ParentSheetIndex} sold for {salePrice}", LogLevel.Debug);

            var newSale = new SalesRecord()
            {
                item = item,
                price = salePrice,
                npc = npc,
                mult = mult,
                timeOfDay = Game1.timeOfDay
            };

            Sales.Add(newSale);
        }

        public void ShowSummary()
        {
            if (!Context.IsMainPlayer)
            {
                Log($"ShowSummary: summary for farmhands cannot list items sold", LogLevel.Warn);
            }

            var salesDescriptions = (
                from sale in Sales
                let displayMult = Convert.ToInt32((sale.mult - 1) * 100)
                select Get("sales-desc",
                    new
                    {
                        ItemName = sale.item.DisplayName, NpcName = sale.npc.displayName, Price = sale.price,
                        Mult = displayMult
                    })
            ).ToList();
            if (salesDescriptions.Count == 0) salesDescriptions.Add(Get("no-sales-today"));

            string AverageMult;
            string TotalGold;
            if (Sales.Count > 0)
            {
                var avgBonus = Sales.Select(s => s.mult).Average() - 1;
                AverageMult = $"{Convert.ToInt32(avgBonus * 100)}%";
                TotalGold = $"{Sales.Select(s => s.price).Sum()}";
            }
            else
            {
                AverageMult = Get("no-sales-today");
                TotalGold = Get("no-sales-today");
            }

            var VisitorsToday = StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{VisitorsTodayKey}"];
            var GrumpyVisitorsToday = StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{GrumpyVisitorsTodayKey}"];
            var message = Get("daily-summary",
                new
                {
                    FarmName = Game1.player.farmName.Value,
                    ItemsSold = Sales.Count,
                    AverageMult,
                    VisitorsToday,
                    GrumpyVisitorsToday,
                    TotalGold,
                    SalesSummary = string.Join("^", salesDescriptions)
                }
            );
            Game1.drawLetterMessage(message);
        }

        private double SellPriceMultiplier(Item item, NPC npc)
        {
            var mult = 1.0;

            // * general quality of display
            mult += GetPointsMultiplier(GetGrangeScore());

            // * farmer is nearby
            if (Game1.player.currentLocation.Name == "Town") mult += 0.2;

            // * value of item on sign
            if (ShopSign.displayItem.Value is Object o)
            {
                var signSellPrice = o.sellToStorePrice();
                signSellPrice = Math.Min(signSellPrice, 1000);
                mult += signSellPrice / 1000.0 / 10.0;
            }

            if (npc is null) return mult;

            // * gift taste
            switch (npc.getGiftTasteForThisItem(item))
            {
                case NPC.gift_taste_like:
                    mult += 1.2;
                    break;
                case NPC.gift_taste_love:
                    mult += 1.4;
                    break;
            }

            // * friendship
            var hearts = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
            mult += hearts / 100.0;

            // * talked today;
            if (Game1.player.hasPlayerTalkedToNPC(npc.Name)) mult += 0.1;

            return mult;
        }

        private static double ItemPreferenceIndex(Item item, NPC npc)
        {
            if (item is null || npc is null) return 1.0;

            // * gift taste
            int taste;
            try
            {
                // so many other mods hack up NPCs that we have to wrap this
                taste = npc.getGiftTasteForThisItem(item);
            }
            catch (Exception)
            {
                taste = NPC.gift_taste_neutral;
            }
            
            switch (taste)
            {
                case NPC.gift_taste_dislike:
                case NPC.gift_taste_hate:
                    return 0.0;
                case NPC.gift_taste_neutral:
                    return 1.0;
                case NPC.gift_taste_like:
                    return 2.0;
                case NPC.gift_taste_love:
                    return 4.0;
                default:
                    return 1.0;
            }
        }

        private List<NPC> NearbyNPCs()
        {
            var location = Game1.getLocationFromName("Town");
            if (location is null) return new List<NPC>();

            var nearby = (from npc in location.characters
                where npc.getTileX() >= Origin.X && npc.getTileX() <= Origin.X + 2 && npc.getTileY() == Origin.Y + 4
                select npc).ToList();
            return nearby;
        }

        private bool RecentlyBought(NPC npc)
        {
            Debug.Assert(Context.IsMainPlayer, "RecentlyBought: only main player can access Sales");

            return Sales.Any(sale => sale.npc == npc && sale.timeOfDay > Game1.timeOfDay - 100);
        }

        internal void MakeFurniture(Vector2 OriginTile)
        {
            Log($"MakeFurniture [{OriginTile}]", LogLevel.Trace);

            if (StockChest is not null && GrangeChest is not null && ShopSign is not null)
            {
                Log(
                    $"    All furniture in place: {StockChest.TileLocation} {GrangeChest.TileLocation} {ShopSign.TileLocation}",
                    LogLevel.Trace);
                return;
            }

            if (!MarketDay.IsMarketDay())
            {
                Log("MakeFurniture called on non-market day", LogLevel.Error);
                return;
            }

            var location = Game1.getLocationFromName("Town");

            // the storage chest
            if (StockChest is null)
                if (Context.IsMainPlayer) MakeStorageChest(location, OriginTile);
                else Log($"    StorageChest still null", LogLevel.Warn);

            // the display chest
            if (GrangeChest is null)
                if (Context.IsMainPlayer) MakeDisplayChest(location);
                else Log($"    DisplayChest still null", LogLevel.Warn);

            // the grange sign
            if (ShopSign is null)
                if (Context.IsMainPlayer) MakeSign(location, OriginTile);
                else Log($"    {ShopSignKey} still null", LogLevel.Warn);

            // the results
            Log($"    ... StorageChest at {StockChest?.TileLocation}", LogLevel.Trace);
            Log($"    ... DisplayChest at {GrangeChest?.TileLocation}", LogLevel.Trace);
            Log($"    ... {ShopSignKey} at {ShopSign?.TileLocation}", LogLevel.Trace);
        }

        private Sign FindSign()
        {
            var location = Game1.getLocationFromName("Town");

            foreach (var (tile, item) in location.Objects.Pairs)
            {
                if (item is not Sign sign) continue;
                if (!sign.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{ShopSignKey}",
                    out var owner))
                    continue;
                if (owner != ShopKey) continue;
                return sign;
            }

            return null;
        }

        private Chest FindStorageChest()
        {
            var location = Game1.getLocationFromName("Town");

            foreach (var (tile, item) in location.Objects.Pairs)
            {
                if (item is not Chest chest) continue;
                if (!chest.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{StockChestKey}",
                    out var owner)) continue;
                if (owner != ShopKey) continue;
                chest.TileLocation = tile; // ensure the chest thinks it's where the location thinks it is
                return chest;
            }

            return null;
        }

        private Chest FindDisplayChest()
        {
            var location = Game1.getLocationFromName("Town");
            foreach (var (tile, item) in location.Objects.Pairs)
            {
                if (item is not Chest chest) continue;
                if (!chest.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{GrangeChestKey}",
                    out var owner)) continue;
                if (owner != ShopKey) continue;
                return chest;
            }

            return null;
        }

        private void MakeSign(GameLocation location, Vector2 OriginTile)
        {
            var VisibleSignPosition = OriginTile + new Vector2(3, 3);

            Log($"    Creating new {ShopSignKey} at {VisibleSignPosition}", LogLevel.Trace);
            var sign = new Sign(VisibleSignPosition, WOOD_SIGN)
            {
                modData = {[$"{MarketDay.SMod.ModManifest.UniqueID}/{ShopSignKey}"] = ShopKey}
            };
            location.objects[VisibleSignPosition] = sign;
        }

        private void MakeDisplayChest(GameLocation location)
        {
            var freeTile = AvailableTileForFurniture(11778, DisplayChestHidingOffsetY);
            Log($"    Creating new DisplayChest at {freeTile}", LogLevel.Trace);
            var chest = new Chest(true, freeTile, 232)
            {
                modData = {[$"{MarketDay.SMod.ModManifest.UniqueID}/{GrangeChestKey}"] = ShopKey}
            };
            location.setObject(freeTile, chest);
            while (GrangeChest.items.Count < 9) GrangeChest.items.Add(null);
        }

        private void MakeStorageChest(GameLocation location, Vector2 OriginTile)
        {
            var VisibleChestPosition = OriginTile + new Vector2(3, 1);

            Log($"    Creating new StorageChest at {VisibleChestPosition}", LogLevel.Trace);
            var chest = new Chest(true, VisibleChestPosition, 232)
            {
                modData = {[$"{MarketDay.SMod.ModManifest.UniqueID}/{StockChestKey}"] = ShopKey}
            };
            location.setObject(VisibleChestPosition, chest);
        }

        private void DecorateFurniture()
        {
            if (ShopColor.R > 0 || ShopColor.G > 0 || ShopColor.B > 0)
            {
                var color = ShopColor;
                color.A = 255;
                StockChest.playerChoiceColor.Value = color;
            }
            else
            {
                var ci = Game1.random.Next(20);
                var c = new DiscreteColorPicker(0, 0).getColorFromSelection(ci);
                Log($"    ShopColor randomized to {c}", LogLevel.Trace);
                StockChest.playerChoiceColor.Value = c;
            }

            Log($"    SignObjectIndex {SignObjectIndex}", LogLevel.Trace);
            Item SignItem = GrangeChest.items.ToList().Find(item => item is not null);
            if (SignItem is null && StockChest.items.Count > 0) SignItem = StockChest.items[0].getOne();
            if (SignObjectIndex > 0) SignItem = new Object(SignObjectIndex, 1);

            if (SignItem is null) return;
            Log($"    {ShopSignKey}.displayItem.Value to {SignItem.DisplayName}", LogLevel.Trace);
            ShopSign.displayItem.Value = SignItem;
            ShopSign.displayType.Value = 1;
        }

        // private void MoveFurnitureToVisible()
        // {
        //     var location = Game1.getLocationFromName("Town");
        //     var VisibleChestPosition = new Vector2(X + 3, Y + 1);
        //     var VisibleSignPosition = new Vector2(X + 3, Y + 3);
        //
        //     Debug.Assert(DayStockChest is not null, "StorageChest is not null");
        //     Debug.Assert(ShopSign is not null, "{ShopSignKey} is not null");
        //     Debug.Assert(ShopSign.TileLocation.X > 0, "GrangeSign.TileLocation.X assigned");
        //     Debug.Assert(ShopSign.TileLocation.Y > 0, "GrangeSign.TileLocation.Y assigned");
        //
        //     location.moveObject(
        //         (int) DayStockChest.TileLocation.X, (int) DayStockChest.TileLocation.Y,
        //         (int) VisibleChestPosition.X, (int) VisibleChestPosition.Y);
        //
        //     location.moveObject(
        //         (int) ShopSign.TileLocation.X, (int) ShopSign.TileLocation.Y,
        //         (int) VisibleSignPosition.X, (int) VisibleSignPosition.Y);
        // }

        private void DestroyFurniture()
        {
            Log($"    DestroyFurniture: {ShopName}", LogLevel.Trace);

            var toRemove = new Dictionary<Vector2, Object>();

            var location = Game1.getLocationFromName("Town");
            foreach (var (tile, item) in location.Objects.Pairs)
            {
                foreach (var key in new List<string> {"{ShopSignKey}", "{DayStockChestKey}", "{DisplayChestKey}"})
                {
                    if (!item.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{key}",
                        out var owner)) continue;
                    if (owner != ShopKey) continue;
                    Log($"    Scheduling {item.displayName} from {tile}", LogLevel.Debug);
                    toRemove[tile] = item;
                }
            }

            foreach (var (tile, itemToRemove) in toRemove)
            {
                Log($"    Removing {itemToRemove.displayName} from {tile}", LogLevel.Debug);
                location.Objects.Remove(tile);
            }
        }

        private void StockChestForTheDay()
        {
            foreach (var (Salable, priceAndStock) in StockManager.ItemPriceAndStock)
            {
                // priceAndStock: price, stock, currency obj, currency stack
                Log($"    Stock item {Salable.DisplayName} price {priceAndStock[0]} stock {priceAndStock[1]}", LogLevel.Trace);
                var stack = Math.Min(priceAndStock[1], 13);
                while (stack-- > 0)
                {
                    if (Salable is Item item)
                    {
                        var newItem = item.getOne();
                        newItem.Stack = 1;
                        StockChest.addItem(newItem);
                    }
                    else
                    {
                        Log($"    Stock item {Salable.DisplayName} is not an Item", LogLevel.Warn);
                    }
                }
            }
        }

        private void RestockGrangeFromChest(bool fullRestock = false)
        {
            if (StockChest is null)
            {
                Log($"RestockGrangeFromChest: StorageChest is null", LogLevel.Warn);
                return;
            }

            if (GrangeChest is null)
            {
                Log($"RestockGrangeFromChest: DisplayChest is null", LogLevel.Warn);
                return;
            }

            var restockLimitRemaining = MarketDay.Config.RestockItemsPerHour;

            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (StockChest.items.Count == 0)
                {
                    Log($"RestockGrangeFromChest: {ShopName} out of stock", LogLevel.Debug, true);
                    return;
                }

                if (restockLimitRemaining <= 0) return;
                if (GrangeChest.items[j] != null) continue;

                var stockItem = StockChest.items[Game1.random.Next(StockChest.items.Count)];
                var grangeItem = stockItem.getOne();

                grangeItem.Stack = 1;
                addItemToGrangeDisplay(grangeItem, j, false);

                if (stockItem.Stack == 1)
                {
                    StockChest.items.Remove(stockItem);
                }
                else
                {
                    stockItem.Stack--;
                }

                if (!fullRestock) restockLimitRemaining--;
            }
        }

        private void EmptyStoreIntoChest()
        {
            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (GrangeChest.items[j] == null) continue;
                StockChest.addItem(GrangeChest.items[j]);
                GrangeChest.items[j] = null;
            }
        }

        private bool onGrangeChange(Item i, int position, Item old, StorageContainer container, bool onRemoval)
        {
            // Log(
            //     $"onGrangeChange: item {i.ParentSheetIndex} position {position} old item {old?.ParentSheetIndex} onRemoval: {onRemoval}",
            //     LogLevel.Debug);

            if (!onRemoval)
            {
                if (i.Stack > 1 || i.Stack == 1 && old is {Stack: 1} && i.canStackWith(old))
                {
                    // Log($"onGrangeChange: big stack", LogLevel.Debug);

                    if (old != null && old.canStackWith(i))
                    {
                        // tried to add extra of same item to a slot that's already taken, 
                        // reset the stack size to 1
                        // Log(
                        //     $"onGrangeChange: can stack: heldItem now {old.Stack} of {old.ParentSheetIndex}, rtn false",
                        //     LogLevel.Debug);

                        container.ItemsToGrabMenu.actualInventory[position].Stack = 1;
                        container.heldItem = old;
                        return false;
                    }

                    if (old != null)
                    {
                        // tried to add item to a slot that's already taken, 
                        // swap the old item back in
                        // Log(
                        //     $"onGrangeChange: cannot stack: helditem now {i.Stack} of {i.ParentSheetIndex}, {old.ParentSheetIndex} to inventory, rtn false",
                        //     LogLevel.Debug);

                        StardewValley.Utility.addItemToInventory(old, position,
                            container.ItemsToGrabMenu.actualInventory);
                        container.heldItem = i;
                        return false;
                    }


                    int allButOne = i.Stack - 1;
                    Item reject = i.getOne();
                    reject.Stack = allButOne;
                    container.heldItem = reject;
                    i.Stack = 1;
                    // Log(
                    //     $"onGrangeChange: only accept 1, reject {allButOne}, heldItem now {reject.Stack} of {reject.ParentSheetIndex}",
                    //     LogLevel.Debug);
                }
            }
            else if (old is {Stack: > 1})
            {
                // Log($"onGrangeChange: old {old.ParentSheetIndex} stack {old.Stack}", LogLevel.Debug);

                if (!old.Equals(i))
                {
                    // Log($"onGrangeChange: item {i.ParentSheetIndex} old {old.ParentSheetIndex} return false",
                    //     LogLevel.Debug);
                    return false;
                }
            }

            var itemToAdd = onRemoval && (old == null || old.Equals(i)) ? null : i;
            // Log($"onGrangeChange: force-add {itemToAdd?.ParentSheetIndex} at {position}", LogLevel.Debug);

            addItemToGrangeDisplay(itemToAdd, position, true);
            return true;
        }

        private void addItemToGrangeDisplay(Item i, int position, bool force)
        {
            while (GrangeChest.items.Count < 9) GrangeChest.items.Add(null);
            // Log($"addItemToGrangeDisplay: item {i?.ParentSheetIndex} position {position} force {force}",
            //     LogLevel.Debug);

            if (position < 0) return;
            if (position >= GrangeChest.items.Count) return;
            if (GrangeChest.items[position] != null && !force) return;

            GrangeChest.items[position] = i;
        }

        internal void DrawSign(Vector2 tileLocation, SpriteBatch spriteBatch, float layerDepth)
        {
            var start = Game1.GlobalToLocal(Game1.viewport, tileLocation * 64);

            var sign = OutsideOpeningHours ? ClosedSign : OpenSign;
            if (sign is null) return;
            
            var center = start + new Vector2(24 * 4, 55 * 4);
            var signLoc = center - new Vector2(
                (int) (sign.Width * Game1.pixelZoom / 2),
                (int) (sign.Height * Game1.pixelZoom / 2));

            spriteBatch.Draw(
                texture: sign,
                position: signLoc,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom,
                effects: SpriteEffects.None,
                layerDepth: layerDepth
            );
        }


        // aedenthorn
        internal void drawGrangeItems(Vector2 tileLocation, SpriteBatch spriteBatch, float layerDepth)
        {
            if (GrangeChest is null) return;

            var start = Game1.GlobalToLocal(Game1.viewport, tileLocation * 64);

            start.X += 4f;
            var xCutoff = (int) start.X + 168;
            start.Y += 8f;

            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (GrangeChest.items[j] != null)
                {
                    start.Y += 42f;
                    start.X += 4f;
                    spriteBatch.Draw(Game1.shadowTexture, start,
                        Game1.shadowTexture.Bounds, Color.White, 0f,
                        Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.02f);
                    start.Y -= 42f;
                    start.X -= 4f;
                    GrangeChest.items[j].drawInMenu(spriteBatch, start, 1f, 1f,
                        layerDepth + 0.0201f + j / 10000f, StackDrawType.Hide);
                }

                start.X += 60f;
                if (start.X >= xCutoff)
                {
                    start.X = xCutoff - 168;
                    start.Y += 64f;
                }
            }
        }

        // aedenthorn
        private int GetGrangeScore()
        {
            int pointsEarned = 14;
            Dictionary<int, bool> categoriesRepresented = new Dictionary<int, bool>();
            int nullsCount = 0;
            foreach (Item i in GrangeChest.items)
            {
                if (i != null && i is Object)
                {
                    if (Event.IsItemMayorShorts(i as Object))
                    {
                        return -666;
                    }

                    pointsEarned += (i as Object).Quality + 1;
                    int num = (i as Object).sellToStorePrice();
                    if (num >= 20)
                    {
                        pointsEarned++;
                    }

                    if (num >= 90)
                    {
                        pointsEarned++;
                    }

                    if (num >= 200)
                    {
                        pointsEarned++;
                    }

                    if (num >= 300 && (i as Object).Quality < 2)
                    {
                        pointsEarned++;
                    }

                    if (num >= 400 && (i as Object).Quality < 1)
                    {
                        pointsEarned++;
                    }

                    int category = (i as Object).Category;
                    if (category <= -27)
                    {
                        switch (category)
                        {
                            case -81:
                            case -80:
                                break;
                            case -79:
                                categoriesRepresented[-79] = true;
                                continue;
                            case -78:
                            case -77:
                            case -76:
                                continue;
                            case -75:
                                categoriesRepresented[-75] = true;
                                continue;
                            default:
                                if (category != -27)
                                {
                                    continue;
                                }

                                break;
                        }

                        categoriesRepresented[-81] = true;
                    }
                    else if (category != -26)
                    {
                        if (category != -18)
                        {
                            switch (category)
                            {
                                case -14:
                                case -6:
                                case -5:
                                    break;
                                case -13:
                                case -11:
                                case -10:
                                case -9:
                                case -8:
                                case -3:
                                    continue;
                                case -12:
                                case -2:
                                    categoriesRepresented[-12] = true;
                                    continue;
                                case -7:
                                    categoriesRepresented[-7] = true;
                                    continue;
                                case -4:
                                    categoriesRepresented[-4] = true;
                                    continue;
                                default:
                                    continue;
                            }
                        }

                        categoriesRepresented[-5] = true;
                    }
                    else
                    {
                        categoriesRepresented[-26] = true;
                    }
                }
                else if (i == null)
                {
                    nullsCount++;
                }
            }

            pointsEarned += Math.Min(30, categoriesRepresented.Count * 5);
            int displayFilledPoints = 9 - 2 * nullsCount;
            pointsEarned += displayFilledPoints;
            return pointsEarned;
        }

        private static double GetPointsMultiplier(int score)
        {
            return score switch
            {
                >= 90 => 0.15,
                >= 75 => 0.1,
                >= 60 => 0.05,
                < 0 => 0,
                _ => 0
            };
        }

        private void Log(string message, LogLevel level, bool VerboseOnly = false)
        {
            if (VerboseOnly && !MarketDay.Config.VerboseLogging) return;
            MarketDay.Log($"[{Game1.player.Name}] [{ShopName}] {message}", level);
        }

        private string Get(string key)
        {
            return MarketDay.SMod.Helper.Translation.Get(key);
        }

        private static string Get(string key, object tokens)
        {
            return MarketDay.SMod.Helper.Translation.Get(key, tokens);
        }
    }
}