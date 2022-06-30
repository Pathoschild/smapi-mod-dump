/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MarketDay.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace MarketDay.Shop
{
    public class GrangeShop : ItemShop
    {
        private const int WOOD_SIGN = 37;
        private const double BUY_CHANCE = 0.75;

        private const int DisplayChestHidingOffsetY = 36;

        public string ShopKey => IsPlayerShop ? ShopName : $"{ContentPack.Manifest.UniqueID}/{ShopName}";
        public long PlayerID;

        public Vector2 Origin
        {
            get
            {
                if (StockChest != null) return StockChest.TileLocation - new Vector2(3, 1);
                MarketDay.Log("OwnerTile: origin is null", LogLevel.Error);
                return Vector2.Zero;
            }
        }

        public Vector2 OwnerTile => Origin + new Vector2(3, 2);

        public const string GrangeChestKey = "GrangeDisplay";
        private Chest? GrangeChest => FindDisplayChest();

        // all shared state is stored on the stock chest
        public const string StockChestKey = "GrangeStorage";
        private Chest? StockChest => FindStorageChest();

        public const string ShopSignKey = "GrangeSign";
        private Sign? ShopSign => FindSign();

        private const string OwnerKey = "Owner";
        private const string VisitorsTodayKey = "VisitorsToday";
        private const string GrumpyVisitorsTodayKey = "GrumpyVisitorsToday";
        private const string SalesTodayKey = "SalesToday";
        public const string GoldTodayKey = "GoldToday";

        private Dictionary<NPC, int> recentlyLooked = new();
        private Dictionary<NPC, int> recentlyTended = new();
        internal List<SalesRecord> Sales = new();

        public Texture2D? OpenSign;
        public Texture2D? ClosedSign;

        // state queries

        public bool IsPlayerShop => PlayerID != 0;

        public string? Owner()
        {
            if (IsPlayerShop) return ShopName.Replace("Farmer:", "");
            return NpcName.Length > 0 ? NpcName : null;
        }

        private Character? OwnerCharacter
        {
            get
            {
                if (Owner() is null) return null; 
                if (IsPlayerShop)
                {
                    return Game1.getAllFarmers().FirstOrDefault(f => f.Name == Owner());
                }
                foreach (var c in StardewValley.Utility.getAllCharacters())
                {
                    if (c.Name == Owner()) return c;
                }
                return null;
            }
        }

        private static bool OutsideOpeningHours =>
            Game1.timeOfDay < MarketDay.Config.OpeningTime * 100 ||
            Game1.timeOfDay > MarketDay.Config.ClosingTime * 100;

        public void OpenAt(Vector2 origin)
        {
            Debug.Assert(Context.IsMainPlayer, "OpenAt: only main player can open shops");

            Log($"Opening at {origin}", LogLevel.Trace);

            MakeFurniture(origin);

            recentlyLooked = new Dictionary<NPC, int>();
            recentlyTended = new Dictionary<NPC, int>();
            Sales = new List<SalesRecord>();

            SetSharedValue(VisitorsTodayKey, 0);
            SetSharedValue(GrumpyVisitorsTodayKey, 0);
            SetSharedValue(SalesTodayKey, 0);
            SetSharedValue(GoldTodayKey, 0);

            if (!IsPlayerShop)
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

            if (IsPlayerShop)
            {
                if (MarketDay.Progression.AutoRestock > 0) RestockGrangeFromChest();
            }
            else
            {
                RestockGrangeFromChest();
            }
        }

        public void CloseShop()
        {
            if (!Context.IsMainPlayer) return;
            if (StockChest is null || GrangeChest is null)
            {
                MarketDay.Log($"Tried to close shop but shop isn't open", LogLevel.Warn);
                return;
            }
            
            Log($"    EmptyGrangeAndDestroyFurniture: IsPlayerShop: {IsPlayerShop}", LogLevel.Trace);

            if (IsPlayerShop)
            {
                MarketDay.IncrementSharedValue(MarketDay.TotalGoldKey, GetSharedValue(GoldTodayKey));
                SendSalesReport();

                EmptyStoreIntoChest();
                EmptyPlayerDayStorageChest();
            }

            DestroyFurniture();
        }

        private void SendSalesReport()
        {
            var dayProfit = GetSharedValue(GoldTodayKey);
            var mailKey = $"md_prize_{Owner()}_{Game1.currentSeason}_{Game1.dayOfMonth}_Y{Game1.year}";
            string text;
            if (MarketDay.Config.Progression)
            {
                var prize = MarketDay.Progression.CurrentLevel.PrizeForEarnings(dayProfit);
                if (prize is not null)
                {
                    text = SalesReport("mail-summary", prize.Name);
                    MarketDay.Log($"Submitting prize mail {mailKey}", LogLevel.Trace);
                    Mail.Send(mailKey, text, Owner(), 1, 8, prize);
                    return;
                }
            }

            text = SalesReport("mail-summary");
            MarketDay.Log($"Submitting non-prize mail {mailKey}", LogLevel.Trace);
            Mail.Send(mailKey, text, Owner(), 1, 8);
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

        private void CheckForBrowsingNPCs()
        {
            if (OutsideOpeningHours) return;

            Debug.Assert(Context.IsMainPlayer, "CheckForBrowsingNPCs: only main player can access recentlyLooked");
            foreach (var npc in NearbyNPCs().Where(npc => npc.movementPause <= 0 && !RecentlyBought(npc)))
            {
                if (recentlyLooked.TryGetValue(npc, out var time))
                {
                    if (Game1.timeOfDay - time < 100) continue;
                }

                if (MatureContent && NPCUtility.IsChild(npc)) continue;

                npc.Halt();
                npc.faceDirection(0);
                npc.movementPause = 5000;
                recentlyLooked[npc] = Game1.timeOfDay;

                IncrementSharedValue(VisitorsTodayKey);
            }
        }

        private void IncrementSharedValue(string key, int amount = 1)
        {
            if (StockChest is null)
            {
                MarketDay.Log($"IncrementSharedValue: StockChest is null while accessing {key}", LogLevel.Warn);
                return;
            }

            if (StockChest.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{key}", out var stringVal))
            {
                var val = int.Parse(stringVal);
                val += amount;
                StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{key}"] = $"{val}";
                return;
            }

            MarketDay.Log($"GetSharedValue: StockChest modData does not contain {key}", LogLevel.Warn);
        }

        internal int GetSharedValue(string key)
        {
            if (StockChest is null)
            {
                MarketDay.Log($"GetSharedValue: StockChest is null while accessing {key}", LogLevel.Warn);
                return 0;
            }

            if (StockChest.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{key}", out var stringVal))
            {
                var val = int.Parse(stringVal);
                return val;
            }

            MarketDay.Log($"GetSharedValue: StockChest modData does not contain {key}", LogLevel.Warn);
            return 0;
        }


        internal void SetSharedValue(string key, int val = 1)
        {
            SetSharedValue(key, $"{val}");
        }

        private void SetSharedValue(string key, string val)
        {
            if (StockChest is null)
            {
                MarketDay.Log($"SetSharedValue: StockChest is null while accessing {key}", LogLevel.Warn);
                return;
            }
            
            StockChest.modData[$"{MarketDay.SMod.ModManifest.UniqueID}/{key}"] = val;
        }

        // ReSharper disable once InconsistentNaming
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

            if (IsPlayerShop)
            {
                if (PlayerID != Game1.player.UniqueMultiplayerID)
                {
                    var Owner = ShopName.Replace("Farmer:", "");
                    Game1.activeClickableMenu = new DialogueBox(Get("not-your-shop", new {Owner}));
                }
                else
                {
                    var rows = MarketDay.Progression.ShopSize / 3;
                    Game1.activeClickableMenu = new StorageContainer(GrangeChest.items, MarketDay.Progression.ShopSize,
                        rows, onGrangeChange,
                        StardewValley.Utility.highlightSmallObjects);
                }
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
            if (GrangeChest is null)
            {
                MarketDay.Log($"OnPurchase: GrangeChest is null", LogLevel.Error);
                return false;
            }
            
            MarketDay.Log($"OnPurchase {item.Name} {stack}", LogLevel.Trace);

            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (GrangeChest.items[j] is null) continue;
                if (!ItemsUtil.Equal(item, GrangeChest.items[j])) continue;
                GrangeChest.items[j] = null;
                return false;
            }

            return false;
        }

        private int[] getSellPriceArrayFromShopStock(Item item)
        {
            if (StockManager?.ItemPriceAndStock is null)
            {
                MarketDay.Log($"getSellPriceArrayFromShopStock: no Stockmanager or no StockManager.ItemPriceAndStock", LogLevel.Warn);
                return new[]
                {
                    StardewValley.Utility.getSellToStorePriceOfItem(item, false)
                };
            }
            foreach (var (stockItem, priceAndQty) in StockManager.ItemPriceAndStock)
            {
                if (!ItemsUtil.Equal(item, stockItem)) continue;
                return priceAndQty;
            }

            MarketDay.Log($"getSellPriceArrayFromShopStock: returning empty for item {item.Name}", LogLevel.Warn);
            return new[]
            {
                StardewValley.Utility.getSellToStorePriceOfItem(item, false)
            };
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

                var price = getSellPriceArrayFromShopStock(stockItem);

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

        private NPC? OwnerNearby()
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
            if (GrangeChest is null) return;
            
            foreach (var npc in NearbyNPCs())
            {
                // the owner
                if (npc.Name == Owner()) continue;

                // busy looking
                if (npc.movementPause is > 2000 or < 500)
                {
                    continue;
                }

                // already bought
                if (RecentlyBought(npc))
                {
                    continue;
                }

                // unlucky
                var rnd = Game1.random.NextDouble();
                if (rnd > BUY_CHANCE + Game1.player.DailyLuck)
                {
                    continue;
                }

                if (MatureContent && NPCUtility.IsChild(npc)) continue;
                
                // find what the NPC likes best
                var best = GrangeChest.items
                    .Where(gi => gi is not null && ItemPreferenceIndex(gi, npc) > 0)
                    .OrderByDescending(a => ItemPreferenceIndex(a, npc)).FirstOrDefault();
                if (best is null)
                {
                    // no stock 
                    if (GrangeChest.items.Where(gi => gi is not null).ToList().Count > 0)
                    {
                        // debug: stock in chest

                        MarketDay.Log($"    {ShopName} has stock but {npc.Name} doesn't like any of it", LogLevel.Trace);

                        var itemsInChest = GrangeChest.items
                            .Where(gi => gi is not null)
                            .OrderByDescending(a => ItemPreferenceIndex(a, npc));
                        foreach (var stockItem in itemsInChest)
                        {
                            MarketDay.Log(
                                $"    ItemPreferenceIndex({stockItem.Name}, {npc.Name}) = {ItemPreferenceIndex(stockItem, npc)}",
                                LogLevel.Trace);
                        }

                    }
                    IncrementSharedValue(GrumpyVisitorsTodayKey);
                    npc.doEmote(12);
                    return;
                }

                // buy it
                SellToNPC(best, npc);
                var i = GrangeChest.items.IndexOf(best);
                GrangeChest.items[i] = null;

                EmoteForPurchase(npc, best);
            }
        }


        private void EmoteForPurchase(NPC npc, Item item)
        {
            if (Game1.random.NextDouble() < 0.25)
            {
                npc.doEmote(20);
            }
            else
            {
                var taste = GetGiftTasteForThisItem(item, npc);

                var dialog = Get("buy", new {ItemName = item.DisplayName});
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

        private void SellToNPC(Item item, NPC npc)
        {
            Debug.Assert(Context.IsMainPlayer, "AddToPlayerFunds: only main player can access Sales");

            var mult = SellPriceMultiplier(item, npc);
            var salePrice = StardewValley.Utility.getSellToStorePriceOfItem(item, false);
            salePrice = Convert.ToInt32(salePrice * mult);

            var newSale = new SalesRecord()
            {
                item = item,
                price = salePrice,
                npc = npc,
                mult = mult,
                timeOfDay = Game1.timeOfDay
            };
            Sales.Add(newSale);

            if (!IsPlayerShop) return;
            IncrementSharedValue(SalesTodayKey);
            IncrementSharedValue(GoldTodayKey, salePrice);
            AddToPlayerFunds(salePrice);
        }

        private void AddToPlayerFunds(int salePrice)
        {
            if (Game1.player.team.useSeparateWallets.Value)
            {
                try
                {
                    Log($"Paying {PlayerID}", LogLevel.Trace);
                    Log($"Paying {Game1.getFarmer(PlayerID).Name}", LogLevel.Trace);
                    var farmer = Game1.getFarmer(PlayerID);
                    Game1.player.team.AddIndividualMoney(farmer, salePrice);
                }
                catch (Exception ex)
                {
                    Log($"Error while paying {PlayerID}: {ex}", LogLevel.Error);
                    Game1.player.Money += salePrice;
                }
            }
            else
            {
                Game1.player.Money += salePrice;
            }
        }

        public void ShowSummary()
        {
            var message = SalesReport();
            Game1.drawLetterMessage(message);
        }

        private string SalesReport(string key = "sign-summary", string prizeName = "")
        {
            var LevelStrapline = "";
            var WeeklyGoalProgress = "";
            var LevelGoalProgress = "";
            if (MarketDay.Config.Progression)
            {
                LevelStrapline = Get("level-strapline", new {LevelStrapline = MarketDay.Progression.CurrentLevel.Name});
                var weeklyProgress = (int) (GetSharedValue(GoldTodayKey) * 100.0 /
                                            (double) MarketDay.Progression.WeeklyGoldTarget);
                WeeklyGoalProgress = Get("weekly-goal-progress", new {Progress = $"{weeklyProgress}"});

                if (MarketDay.Progression.NextLevel is not null)
                {
                    var levelProgress = (int) (MarketDay.GetSharedValue(MarketDay.TotalGoldKey) * 100.0 /
                                               MarketDay.Progression.NextLevel.UnlockAtEarningsForDifficulty);
                    levelProgress = Math.Max(0, Math.Min(100, levelProgress));
                    LevelGoalProgress = Get("level-goal-progress",
                        new {Progress = $"{levelProgress}", NextLevelName = MarketDay.Progression.NextLevel.Name});
                }
            }

            var FarmerName = ShopName.Replace("Farmer:", "");
            var FarmName = Game1.player.farmName.Value;
            var VisitorsToday = GetSharedValue(VisitorsTodayKey);
            var GrumpyVisitorsToday = GetSharedValue(GrumpyVisitorsTodayKey);
            var ItemsSold = GetSharedValue(SalesTodayKey);
            var TotalGoldToday = StardewValley.Utility.getNumberWithCommas(GetSharedValue(GoldTodayKey));
            var TotalGold = StardewValley.Utility.getNumberWithCommas(MarketDay.GetSharedValue(MarketDay.TotalGoldKey));
            string Date;
            {
                int year = Game1.year;
                string season =
                    StardewValley.Utility.getSeasonNameFromNumber(
                        StardewValley.Utility.getSeasonNumber(Game1.currentSeason));
                int day = Game1.dayOfMonth;
                Date = Get("date", new {year, season, day});
            }

            var Prize = prizeName.Length > 0
                ? Get("summary.prize", new {PrizeName = prizeName})
                : "";

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

            string SalesDetail = "";
            if (salesDescriptions.Count > 0)
            {
                var ItemSalesList = string.Join("^", salesDescriptions);
                SalesDetail = Get("sales-detail", new {ItemSalesList});
            }

            string AverageMult;
            if (Sales.Count > 0)
            {
                var avgBonus = Sales.Select(s => s.mult).Average() - 1;
                AverageMult = $"{Convert.ToInt32(avgBonus * 100)}%";
            }
            else if (ItemsSold > 0) // we sold stuff but the data's not available to this player
            {
                AverageMult = Get("not-available-mp");
            }
            else
            {
                AverageMult = Get("no-sales-today");
            }

            var message = Get(key,
                new
                {
                    LevelStrapline,
                    Date,
                    Farmer = FarmerName,
                    Farm = FarmName,
                    Prize,
                    ItemsSold,
                    AverageMult,
                    VisitorsToday,
                    GrumpyVisitorsToday,
                    TotalGoldToday,
                    TotalGold,
                    SalesDetail,
                    WeeklyGoalProgress,
                    LevelGoalProgress
                }
            );
            return message;
        }

        private double SellPriceMultiplier(Item item, NPC npc)
        {
            var mult = 1.0;

            // * general quality of display
            mult += GetPointsMultiplier(GetGrangeScore());

            // * value of item on sign
            if (ShopSign?.displayItem.Value is Object o)
            {
                var signSellPrice = o.sellToStorePrice();
                signSellPrice = Math.Min(signSellPrice, 1000);
                mult += signSellPrice / 1000.0 / 10.0;
            }
            
            // * gift taste
            switch (GetGiftTasteForThisItem(item, npc))
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

            mult = Math.Min(mult, MarketDay.Progression.SellPriceMultiplierLimit);

            // * talked today;
            if (Game1.player.hasPlayerTalkedToNPC(npc.Name)) mult += 0.1;
            
            // * owner or owner's spouse is nearby
            if (OwnerCharacter?.currentLocation.Name == "Town") mult += 0.2;    
            else if (OwnerCharacter is Farmer farmer)
            {
                if (farmer.getSpouse().currentLocation.Name == "Town") mult += 0.2;
            }

            return mult;
        }

        private int GetGiftTasteForThisItem(Item item, NPC npc)
        {
            int taste = IsPlayerShop ? NPC.gift_taste_dislike : NPC.gift_taste_like;
            try
            {
                // so many other mods hack up NPCs that we have to wrap this
                taste = npc.getGiftTasteForThisItem(item);
                MarketDay.Log($"GetGiftTasteForThisItem: {item.Name} for {npc.Name}: {TasteName(taste)}", LogLevel.Debug, true);
            }
            catch (Exception)
            {
                // stop players selling rubbish but also stop NPCs hating NPC merch
                MarketDay.Log($"GetGiftTasteForThisItem: exception while checking {item.Name} for {npc.Name}", LogLevel.Warn);
            }

            return taste;
        }

        private string TasteName(int taste)
        {
            return taste switch
            {
                NPC.gift_taste_dislike => "dislike",
                NPC.gift_taste_hate => "hate",
                NPC.gift_taste_like => "like",
                NPC.gift_taste_love => "love",
                NPC.gift_taste_neutral => "neutral",
                _ => $"unknown [{taste}]"
            };
        }

        private double ItemPreferenceIndex(Item item, NPC npc)
        {
            if (item is null || npc is null) return 1.0;

            // * gift taste
            var taste = GetGiftTasteForThisItem(item, npc);
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
                where npc.getTileX() >= Origin.X 
                      && npc.getTileX() <= Origin.X + 3 
                      && npc.getTileY() == (int)Origin.Y + 4
                select npc).ToList();
            return nearby;
        }

        private bool RecentlyBought(NPC npc)
        {
            Debug.Assert(Context.IsMainPlayer, "RecentlyBought: only main player can access Sales");

            return Sales.Any(sale => sale.npc == npc && sale.timeOfDay > Game1.timeOfDay - 100);
        }

        private void MakeFurniture(Vector2 OriginTile)
        {
            Log($"MakeFurniture [{OriginTile}]", LogLevel.Trace);

            if (StockChest is not null && GrangeChest is not null && ShopSign is not null)
            {
                Log(
                    $"    All furniture in place: {StockChest.TileLocation} {GrangeChest.TileLocation} {ShopSign.TileLocation}",
                    LogLevel.Trace);
                return;
            }

            if (!MarketDay.IsMarketDay)
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

        private Sign? FindSign()
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

        private Chest? FindStorageChest()
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

        private Chest? FindDisplayChest()
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
                modData =
                {
                    ["furyx639.BetterChests/AutoOrganize"] = "Disabled",
                    ["furyx639.BetterChests/CarryChest"] = "Disabled",
                    ["furyx639.BetterChests/CraftFromChest"] = "Disabled",
                    ["furyx639.BetterChests/StashToChest"] = "Disabled",
                    ["Pathoschild.ChestsAnywhere/IsIgnored"] = "true",
                    [$"{MarketDay.SMod.ModManifest.UniqueID}/{GrangeChestKey}"] = ShopKey
                }
            };
            location.setObject(freeTile, chest);
            if (GrangeChest is null)
            {
                MarketDay.Log($"MakeDisplayChest: GrangeChest is null", LogLevel.Warn);
                return;
            }
            while (GrangeChest.items.Count < MarketDay.Progression.ShopSize) GrangeChest.items.Add(null);
        }

        private void MakeStorageChest(GameLocation location, Vector2 OriginTile)
        {
            var VisibleChestPosition = OriginTile + new Vector2(3, 1);
            string owner = IsPlayerShop
                ? ShopName.Replace("Farmer:", "")
                : NpcName;

            Log($"    Creating new StorageChest at {VisibleChestPosition}", LogLevel.Trace);
            var chest = new Chest(true, VisibleChestPosition, 232)
            {
                modData =
                {
                    ["furyx639.BetterChests/AutoOrganize"] = "Disabled",
                    ["furyx639.BetterChests/CarryChest"] = "Disabled",
                    ["furyx639.BetterChests/CraftFromChest"] = "Disabled",
                    ["furyx639.BetterChests/StashToChest"] = "Disabled",
                    ["Pathoschild.ChestsAnywhere/IsIgnored"] = "true",
                    [$"{MarketDay.SMod.ModManifest.UniqueID}/{StockChestKey}"] = ShopKey,
                    [$"{MarketDay.SMod.ModManifest.UniqueID}/{OwnerKey}"] = owner
                }
            };
            location.setObject(VisibleChestPosition, chest);
        }

        private void DecorateFurniture()
        {
            if (StockChest is null)
            {
                MarketDay.Log("DecorateFurniture: StockChest is null", LogLevel.Error);
                return;
            }
            if (GrangeChest is null)
            {
                MarketDay.Log("DecorateFurniture: GrangeChest is null", LogLevel.Error);
                return;
            }
            if (ShopSign is null)
            {
                MarketDay.Log("DecorateFurniture: ShopSign is null", LogLevel.Error);
                return;
            }

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
            var SignItem = GrangeChest.items.ToList().Find(item => item is not null);
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
                foreach (var key in new List<string> {$"{ShopSignKey}", $"{GrangeChestKey}", $"{StockChestKey}"})
                {
                    if (!item.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{key}", out var owner))
                        continue;
                    if (owner != ShopKey) continue;
                    Log($"    Scheduling removal of {item.displayName} from {tile}", LogLevel.Trace);
                    toRemove[tile] = item;
                }
            }

            foreach (var (tile, itemToRemove) in toRemove)
            {
                Log($"    Removing {itemToRemove.displayName} from {tile}", LogLevel.Trace);
                location.Objects.Remove(tile);
            }
        }

        private void StockChestForTheDay()
        {
            if (StockChest is null)
            {
                MarketDay.Log("StockChestForTheDay: StockChest is null", LogLevel.Error);
                return;
            }
            foreach (var (Salable, priceAndStock) in StockManager.ItemPriceAndStock)
            {
                // priceAndStock: price, stock, currency obj, currency stack
                Log($"    Stock item {Salable.DisplayName} price {priceAndStock[0]} stock {priceAndStock[1]}",
                    LogLevel.Trace);
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

            var restockLimitRemaining =
                IsPlayerShop ? MarketDay.Progression.AutoRestock : MarketDay.Config.RestockItemsPerHour;

            for (var j = 0; j < GrangeChest.items.Count; j++)
            {
                if (StockChest.items.Count == 0)
                {
                    Log($"RestockGrangeFromChest: {ShopName} out of stock", LogLevel.Debug, true);
                    return;
                }

                if (!fullRestock && restockLimitRemaining <= 0) return;
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
            if (StockChest is null)
            {
                MarketDay.Log("EmptyStoreIntoChest: StockChest is null", LogLevel.Error);
                return;
            }
            if (GrangeChest is null)
            {
                MarketDay.Log("EmptyStoreIntoChest: GrangeChest is null", LogLevel.Error);
                return;
            }
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

        private void addItemToGrangeDisplay(Item? i, int position, bool force)
        {
            if (GrangeChest is null)
            {
                MarketDay.Log("addItemToGrangeDisplay: GrangeChest is null", LogLevel.Error);
                return;
            }

            while (GrangeChest.items.Count < MarketDay.Progression.ShopSize) GrangeChest.items.Add(null);

            if (position < 0) return;
            if (position >= GrangeChest.items.Count) return;
            if (GrangeChest.items[position] != null && !force) return;

            GrangeChest.items[position] = i;
        }

        internal void DrawSign(Vector2 tileLocation, SpriteBatch spriteBatch, float layerDepth)
        {
            var start = Game1.GlobalToLocal(Game1.viewport, tileLocation * 64);

            var sign = OutsideOpeningHours ? ClosedSign : OpenSign;
            if (sign is null || MarketDay.Config.ShowShopPositions)
            {
                DrawTextSign(tileLocation, spriteBatch, layerDepth);
                return;
            }

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

        private string SignText()
        {
            if (MarketDay.Config.ShowShopPositions)
            {
                var town = Game1.getLocationFromName("Town");
                var tileProperty = MapUtility.GetTileProperty(town, "Back", Origin);
                if (tileProperty is not null)
                {
                    if (tileProperty.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}.Position",
                        out var positionName))
                    {
                        return positionName;
                    }
                }
            }

            var text = (OutsideOpeningHours ? ClosedSignText : OpenSignText) ?? "";
            if (text.Length == 0)
            {
                text = OutsideOpeningHours ? Get("closed-sign") : Get("shop-sign", new {Owner = Owner()});
            }

            return text ?? "";
        }

        private static string TrimSign(string shopName, SpriteFont font, int maxLen)
        {
            if (font.MeasureString(shopName).X > maxLen)
            {
                shopName = System.Text.RegularExpressions.Regex.Replace(shopName, Get("shop-sign", new {Owner = ""}),
                    "");
                shopName = System.Text.RegularExpressions.Regex.Replace(shopName, Get("farm-sign", new {FarmName = ""}),
                    "");
            }

            while (shopName.Length > 1 && font.MeasureString(shopName).X > maxLen)
            {
                shopName = shopName.Remove(shopName.Length - 1, 1);
            }

            return shopName;
        }

        private void DrawTextSign(Vector2 tileLocation, SpriteBatch spriteBatch, float layerDepth)
        {
            string text;
            Vector2 textSize;

            if (MarketDay.Font is null) return;
            if (MarketDay.BlankSign is null) return;

            try
            {
                text = TrimSign(SignText(), MarketDay.Font, 36 * Game1.pixelZoom);
                textSize = MarketDay.Font.MeasureString(text);
            }
            catch (Exception)
            {
                return;
            }

            var start = Game1.GlobalToLocal(Game1.viewport, tileLocation * 64);
            var center = start + new Vector2(24 * 4, 55 * 4);

            var textLoc = center - new Vector2(
                              (int) (textSize.X / 2),
                              (int) (textSize.Y / 2))
                          + new Vector2(0, 1 * Game1.pixelZoom);


            // textLoc -= new Vector2(textLoc.X % Game1.pixelZoom, textLoc.Y % Game1.pixelZoom);
            // don't do this, it causes jitter


            var signLoc = center - new Vector2(
                MarketDay.BlankSign.Width * Game1.pixelZoom / 2,
                MarketDay.BlankSign.Height * Game1.pixelZoom / 2);

            spriteBatch.Draw(
                texture: MarketDay.BlankSign,
                position: signLoc,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom,
                effects: SpriteEffects.None,
                layerDepth: layerDepth
            );
            spriteBatch.DrawString(
                MarketDay.Font,
                text,
                textLoc,
                new Color(95, 20, 0, 255),
                0,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                layerDepth + 0.0001f
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
            if (GrangeChest is null) return 0;
            var pointsEarned = 14;
            Dictionary<int, bool> categoriesRepresented = new();
            int nullsCount = 0;
            foreach (Item i in GrangeChest.items)
            {
                switch (i)
                {
                    case Object obj when Event.IsItemMayorShorts(obj):
                        return -666;
                    case Object obj:
                    {
                        pointsEarned += obj.Quality + 1;
                        var num = obj.sellToStorePrice();
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

                        if (num >= 300 && obj.Quality < 2)
                        {
                            pointsEarned++;
                        }

                        if (num >= 400 && obj.Quality < 1)
                        {
                            pointsEarned++;
                        }

                        var category = obj.Category;
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

                        break;
                    }
                    case null:
                        nullsCount++;
                        break;
                }
            }

            pointsEarned += Math.Min(30, categoriesRepresented.Count * 5);
            var displayFilledPoints = 9 - 2 * nullsCount;
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