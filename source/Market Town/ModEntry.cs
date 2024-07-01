/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using static MarketTown.ModEntry;
using static MarketTown.PlayerChat;
using static System.Net.Mime.MediaTypeNames;
using Object = StardewValley.Object;
using static StardewValley.Minigames.TargetGame;
using StardewModdingAPI.Utilities;
using xTile.ObjectModel;
using System.Text.RegularExpressions;
using xTile.Dimensions;
using StardewValley.Pathfinding;
using MailFrameworkMod;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Buildings;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using MarketTown.Data;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;
using ContentPatcher;
using StardewValley.GameData.Shops;
using xTile.Tiles;
using xTile;
using static System.Net.WebRequestMethods;
using static StardewValley.Pathfinding.PathFindController;
using System.Xml.Linq;
using static StardewValley.Minigames.CraneGame;
using StardewValley.ItemTypeDefinitions;

namespace MarketTown
{
    /// <summary>The mod entry point.</summary>

    public partial class ModEntry : Mod
    {

        public static ModEntry Instance;

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;

        private Harmony harmony;

        // ==============================================================================================
        // ============================================GLOBAL============================================

        /// <summary>List of valid villagers NPC (has schedule and moving sprite, and not be listed in GlobalNPCBlackList).</summary>
        internal static List<string> GlobalNPCList = new List<string>();

        /// <summary>List of blacklist villagers NPC that will not be used in most case.</summary>
        internal static List<string> GlobalNPCBlackList = new List<string> { "Marlon", "Gunther", "Morris"};

        /// <summary>List of kids NPC (age = 2).</summary>
        internal static List<string> GlobalKidList = new List<string>();

        /// <summary>Dictionary of Location and its list of 'valid' tile for NPC schedule.</summary>
        public static IDictionary<GameLocation, List<Vector2>> RandomOpenSpot = new Dictionary<GameLocation, List<Vector2>>();

        /// <summary>Dictionary of table that pending for restock from nearby storage</summary>
        public static IDictionary<IEnumerator<Furniture>, int> RecentSoldTable = new Dictionary<IEnumerator<Furniture>, int>();

        /// <summary>Handle player typing interact</summary>
        private PlayerChat playerChatInstance;


        // ==============================================================================================
        // ==============================================================================================


        internal static List<Action> ActionList { get; private set; } = new();
        internal static List<Response> KidResponseList { get; private set; } = new();

        /// <summary>List of Kids that will visit farm is request accepted.</summary>
        internal static Dictionary<string, int> TodaySelectedKid = new Dictionary<string, int>();


        //***********************************************************************************************


        /// <summary> List of NPC name that has been given Customer Note today.</summary>
        internal static List<string> TodayCustomerNoteName = new List<string>();


        //***********************************************************************************************


        /// <summary>List of Builings on Farm valid as store or museum.</summary>
        public static List<BuildingObjectPair> validBuildingObjectPairs = new List<BuildingObjectPair>();


        //***********************************************************************************************


        public static string orderKey = "marketTown/order";
        public static Texture2D emoteSprite;
        public static PerScreen<Dictionary<string, int>> npcOrderNumbers = new PerScreen<Dictionary<string, int>>();

        /// <summary>Dictionary of Restaurant and its list of tile where NPC will stand for special order.</summary>
        public static IDictionary<GameLocation, List<Vector2>> RestaurantSpot = new Dictionary<GameLocation, List<Vector2>>();

        /// <summary>Dictionary of Location and its lists of tile that has properties modified.</summary>
        public static IDictionary<GameLocation, List<Vector2>> TilePropertyChanged = new Dictionary<GameLocation, List<Vector2>>();


        //***********************************************************************************************


        /// <summary>List of Builings on Island.</summary>
        public static List<IslandBuildingProperties> IslandValidBuilding = new List<IslandBuildingProperties>();

        /// <summary>List of NPC that will visit the Island today. This is a subset of GlobalNPCList.</summary>
        internal static List<string> IslandNPCList = new List<string>();

        /// <summary>List of list Island visitor will be at when day start.</summary>
        internal static List<Vector2> islandWarp = new List<Vector2>();


        //***********************************************************************************************


        public static bool IsFestivalToday = false;
        public static bool IsFestivalIsCurrent = false;

        /// <summary>List of all Festival shop owners.</summary>
        public static List<string> TodayFestivalOwner = new List<string>();

        /// <summary>Clickable tile that will open the shop menu.</summary>
        public static IDictionary<Vector2, string> OpenShopTile = new Dictionary<Vector2, string>();

        /// <summary>List of Shop, Tile and Shopstock list.</summary>
        public static List<MarketShopData> TodayShopInventory = new List<MarketShopData>();

        /// <summary>Dictionary of shop id and shopstock read from Json file.</summary>
        public static IDictionary<string, List<string>> ShopsJsonData = new Dictionary<string, List<string>>();


        //***********************************************************************************************


        internal static MailData mailData = new MailData();
        public static string TodaySell = "";
        public static int TodayCustomerInteraction = 0;
        public static int TodayCustomerNote = 0;
        public static int TodayCustomerNoteYes = 0;
        public static int TodayCustomerNoteNo = 0;
        public static int TodayCustomerNoteNone = 0;
        public static int TodayVisitorVisited = 0;
        public static int TodayMoney = 0;
        public static int TodayForageSold = 0;
        public static int TodayFlowerSold = 0;
        public static int TodayFruitSold = 0;
        public static int TodayVegetableSold = 0;
        public static int TodaySeedSold = 0;
        public static int TodayMonsterLootSold = 0;
        public static int TodaySyrupSold = 0;
        public static int TodayArtisanGoodSold = 0;
        public static int TodayAnimalProductSold = 0;
        public static int TodayResourceMetalSold = 0;
        public static int TodayMineralSold = 0;
        public static int TodayCraftingSold = 0;
        public static int TodayCookingSold = 0;
        public static int TodayFishSold = 0;
        public static int TodayGemSold = 0;
        public static int TodayMuseumVisitor = 0;
        public static int TodayMuseumEarning = 0;


        // ===============================================================================================================================
        // ===============================================================================================================================


        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            emoteSprite = SHelper.ModContent.Load<Texture2D>(Path.Combine("Assets", "emote.png"));

            new MailLoader(SHelper);

            if (KidResponseList?.Count is not 2)
            {
                KidResponseList.Add(new Response("Yes", SHelper.Translation.Get("foodstore.kidresponselist.yes")));
                KidResponseList.Add(new Response("No", SHelper.Translation.Get("foodstore.kidresponselist.no")));
            }

            GlobalKidList.Clear();
            GlobalNPCList.Clear();

            //Assign visit value
            foreach (NPC __instance in Utility.getAllVillagers())
            {
                if (__instance is not null && __instance.IsVillager)
                {
                    __instance.modData["hapyke.FoodStore/shedEntry"] = "-1,-1";
                    __instance.modData["hapyke.FoodStore/gettingFood"] = "false";
                    __instance.modData["hapyke.FoodStore/invited"] = "false";
                    __instance.modData["hapyke.FoodStore/inviteDate"] = "-99";
                    __instance.modData["hapyke.FoodStore/timeVisitShed"] = "0";
                    __instance.modData["hapyke.FoodStore/LastFood"] = "0";
                    __instance.modData["hapyke.FoodStore/LastCheck"] = "0";
                    __instance.modData["hapyke.FoodStore/LocationControl"] = ",";
                    __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "-1";
                    __instance.modData["hapyke.FoodStore/LastFoodDecor"] = "-1";
                    __instance.modData["hapyke.FoodStore/LastSay"] = "0";
                    __instance.modData["hapyke.FoodStore/TotalCustomerResponse"] = "0";
                    __instance.modData["hapyke.FoodStore/inviteTried"] = "false";
                    __instance.modData["hapyke.FoodStore/stuckCounter"] = "0";
                    __instance.modData["hapyke.FoodStore/festivalLastPurchase"] = "600";
                    __instance.modData["hapyke.FoodStore/specialOrder"] = "-1,-1";
                    __instance.modData["hapyke.FoodStore/shopOwnerToday"] = "-1,-1";

                    if (__instance.getMasterScheduleRawData() != null && !GlobalNPCBlackList.Contains(__instance.Name))
                    {
                        GlobalNPCList.Add(__instance.Name);
                        if (__instance.Age == 2) GlobalKidList.Add(__instance.Name);
                    }
                }
            }

            // Set up tile where visitor spawn
            islandWarp.Clear();
            for (int x = 14; x <= 29; x++)
            {
                for (int y = 51; y <= 53; y++)
                {
                    islandWarp.Add(new Vector2(x, y));
                }
            }
        }

        [EventPriority(EventPriority.Low-9999)]
        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (Config.AdvanceOutputItemId) OutputItemId();

            TodayVisitorVisited = 0;
            TodaySell = "";
            TodayMoney = 0;
            TodayCustomerInteraction = 0;
            TodayCustomerNote = 0;
            TodayCustomerNoteYes = 0;
            TodayCustomerNoteNo = 0;
            TodayCustomerNoteNone = 0;
            TodayForageSold = 0;
            TodayFlowerSold = 0;
            TodayFruitSold = 0;
            TodayVegetableSold = 0;
            TodaySeedSold = 0;
            TodayMonsterLootSold = 0;
            TodaySyrupSold = 0;
            TodayArtisanGoodSold = 0;
            TodayAnimalProductSold = 0;
            TodayResourceMetalSold = 0;
            TodayMineralSold = 0;
            TodayCraftingSold = 0;
            TodayCookingSold = 0;
            TodayFishSold = 0;
            TodayGemSold = 0;
            TodayMuseumVisitor = 0;
            TodayMuseumEarning = 0;

            validBuildingObjectPairs.Clear();

            IslandNPCList.Clear();
            IslandValidBuilding.Clear();
            TodayCustomerNoteName.Clear();
            RandomOpenSpot.Clear();

            TodayShopInventory.Clear();
            IsFestivalToday = false;
            TodayFestivalOwner.Clear();

            npcOrderNumbers.Value.Clear();
            RestaurantSpot.Clear();
            TilePropertyChanged.Clear();

            var keys = new List<IEnumerator<Furniture>>(RecentSoldTable.Keys);
            foreach (var key in keys)
            {
                RecentSoldTable[key] = 0;
            }

            Random rand = new Random();

            if (Game1.IsMasterGame)             // Generate Dish of the week
            {
                DishPrefer.dishDay = GetRandomDish();

                if (Game1.dayOfMonth == 1 || Game1.dayOfMonth == 8 || Game1.dayOfMonth == 15 || Game1.dayOfMonth == 22)
                {
                    DishPrefer.dishWeek = GetRandomDish();      //Get dish of the week

                    if (!Config.DisableChatAll && !Config.DisableChat)
                    {
                        MyMessage messageToSend = new MyMessage("");

                        //Send hidden reveal
                        Random random = new Random();
                        int randomIndex = random.Next(11);
                        Game1.chatBox.addInfoMessage(SHelper.Translation.Get("foodstore.hidden." + randomIndex.ToString()));
                        messageToSend = new MyMessage(SHelper.Translation.Get("foodstore.hidden." + randomIndex.ToString()));
                        SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");
                    }
                }
            }

            // Island logic
            try
            {
                float islandCount = 0f;
                int tried = GlobalNPCList.Count;
                IslandValidBuilding.Add(new IslandBuildingProperties("Custom_MT_Island_House", new Vector2(9, 31), new Vector2(73, 18)));

                GameLocation locat = Game1.getLocationFromName("Custom_MT_Island");
                GameLocation locatHouse = Game1.getLocationFromName("Custom_MT_Island_House");
                GameLocation locatHouseGarden = Game1.getLocationFromName("Custom_MT_Island_House_Garden");

                // Set properties for island tile
                foreach (var x in locat._activeTerrainFeatures)
                {
                    if (x is Tree tree) tree.GetData().GrowthChance = 0.3f;
                    if (!locat.doesEitherTileOrTileIndexPropertyEqual((int)x.Tile.X, (int)x.Tile.Y, "NoPath", "Back", "T"))
                    {
                        locat.setTileProperty((int)x.Tile.X, (int)x.Tile.Y, "Back", "NoPath", "T");

                        if (!TilePropertyChanged.ContainsKey(locat))
                        {
                            TilePropertyChanged[locat] = new List<Vector2>();
                        }
                        TilePropertyChanged[locat].Add(x.Tile);
                    }
                }

                foreach (var y in locat.Objects)
                {
                    foreach (var z in y.Values)
                    {
                        if (!locat.doesEitherTileOrTileIndexPropertyEqual((int)z.TileLocation.X, (int)z.TileLocation.Y, "NoPath", "Back", "T"))
                        {
                            locat.setTileProperty((int)z.TileLocation.X, (int)z.TileLocation.Y, "Back", "NoPath", "T");

                            if (!TilePropertyChanged.ContainsKey(locat))
                            {
                                TilePropertyChanged[locat] = new List<Vector2>();
                            }
                            TilePropertyChanged[locat].Add(z.TileLocation);
                        }
                    }
                }

                foreach (var z in locat.furniture)
                {
                    Vector2 tableTileLocation = z.TileLocation;

                    int tableWidth = z.getTilesWide();
                    int tableHeight = z.getTilesHigh();

                    for (int x = 0; x < tableWidth; x++)
                    {
                        for (int y = 0; y < tableHeight; y++)
                        {
                            if (!locat.doesEitherTileOrTileIndexPropertyEqual((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "NoPath", "Back", "T"))
                            {
                                locat.setTileProperty((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "Back", "NoPath", "T");


                                if (!TilePropertyChanged.ContainsKey(locat))
                                {
                                    TilePropertyChanged[locat] = new List<Vector2>();
                                }
                                TilePropertyChanged[locat].Add(new Vector2((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y));
                            }
                        }
                    }
                }

                // Set properties for island house tile
                foreach (var x in locatHouse._activeTerrainFeatures)
                {
                    if (!locatHouse.doesEitherTileOrTileIndexPropertyEqual((int)x.Tile.X, (int)x.Tile.Y, "NoPath", "Back", "T"))
                    {
                        locatHouse.setTileProperty((int)x.Tile.X, (int)x.Tile.Y, "Back", "NoPath", "T");
                        if (!TilePropertyChanged.ContainsKey(locatHouse))
                        {
                            TilePropertyChanged[locatHouse] = new List<Vector2>();
                        }
                        TilePropertyChanged[locatHouse].Add(x.Tile);
                    }
                }

                foreach (var y in locatHouse.Objects)
                {
                    foreach (var z in y.Values)
                    {
                        if (!locatHouse.doesEitherTileOrTileIndexPropertyEqual((int)z.TileLocation.X, (int)z.TileLocation.Y, "NoPath", "Back", "T"))
                        {
                            locatHouse.setTileProperty((int)z.TileLocation.X, (int)z.TileLocation.Y, "Back", "NoPath", "T");

                            if (!TilePropertyChanged.ContainsKey(locatHouse))
                            {
                                TilePropertyChanged[locatHouse] = new List<Vector2>();
                            }
                            TilePropertyChanged[locatHouse].Add(z.TileLocation);
                        }
                    }
                }

                foreach (var z in locatHouse.furniture)
                {
                    Vector2 tableTileLocation = z.TileLocation;

                    int tableWidth = z.getTilesWide();
                    int tableHeight = z.getTilesHigh();

                    for (int x = 0; x < tableWidth; x++)
                    {
                        for (int y = 0; y < tableHeight; y++)
                        {
                            if (!locatHouse.doesEitherTileOrTileIndexPropertyEqual((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "NoPath", "Back", "T"))
                            {
                                locatHouse.setTileProperty((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "Back", "NoPath", "T");

                                if (!TilePropertyChanged.ContainsKey(locatHouse))
                                {
                                    TilePropertyChanged[locatHouse] = new List<Vector2>();
                                }
                                TilePropertyChanged[locatHouse].Add(new Vector2((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y));
                            }
                        }
                    }
                }

                if (locat.Map.Properties.ContainsKey("skipWeedGrowth")) locat.Map.Properties.Remove("skipWeedGrowth"); // Only spawn on Spring 1

                // Set properties for island garden
                foreach (var x in locatHouseGarden._activeTerrainFeatures)
                {
                    if (x is Tree tree && Config.IslandPlantBoost) tree.GetData().GrowthChance = 0.3f;
                }

                // Set island is greenhouse in Spring
                if (Config.IslandPlantBoost && locat.GetSeason().ToString() == "Spring")
                {
                    locat.IsGreenhouse = true;
                    locatHouseGarden.IsGreenhouse = true;
                }
                else
                {
                    locat.IsGreenhouse = false;
                    locatHouseGarden.IsGreenhouse = false;
                }

                // Check if Island open for visitor. Island is open when the Brazier is On, and it is not festival
                var islandBrazier = locat.getObjectAtTile(22, 36);
                if (locat != null && islandBrazier != null && islandBrazier.ItemId != null && islandBrazier.ItemId == "MT.Objects.ParadiseIslandBrazier" && islandBrazier.IsOn
                    && !( ( Game1.dayOfMonth == 15 || Game1.dayOfMonth == 16 || Game1.dayOfMonth == 17 ) && Game1.currentSeason == "winter" || Game1.isFestival() )  )
                {
                    // Set island is not build-able
                    locat.isAlwaysActive.Value = false;

                    int dayOfWeek = Game1.dayOfMonth % 7;
                    bool festivalDay = false;

                    switch (dayOfWeek)
                    {
                        case 0:
                            festivalDay = Config.FestivalSun;
                            break;
                        case 1:
                            festivalDay = Config.FestivalMon;
                            break;
                        case 2:
                            festivalDay = Config.FestivalTue;
                            break;
                        case 3:
                            festivalDay = Config.FestivalWed;
                            break;
                        case 4:
                            festivalDay = Config.FestivalThu;
                            break;
                        case 5:
                            festivalDay = Config.FestivalFri;
                            break;
                        case 6:
                            festivalDay = Config.FestivalSat;
                            break;
                        default:
                            break;
                    }

                    FarmOutside.UpdateRandomLocationOpenTile(locat);
                    FarmOutside.UpdateRandomLocationOpenTile(locatHouse);


                    foreach (var buildingLocation in locat.buildings)
                    {
                        var buildingInstanceName = buildingLocation.GetIndoorsName();
                        FarmOutside.UpdateRandomLocationOpenTile(Game1.getLocationFromName(buildingInstanceName));
                    }

                    while ( islandCount < Config.ParadiseIslandNPC / IslandProgress() && islandCount <= GlobalNPCList.Count && tried > 0) // Get Visitor List
                    {
                        var tempNPC = GlobalNPCList[rand.Next(GlobalNPCList.Count)];
                        tried--;
                        if (tempNPC == null || Game1.getCharacterFromName(tempNPC) == null || Game1.getCharacterFromName(tempNPC).getMasterScheduleRawData() == null) continue;
                        bool available = false;

                        if (!IslandNPCList.Contains(tempNPC))  available = true;

                        // more people in sunny, less in rainny
                        if (available)
                        {
                            IslandNPCList.Add(tempNPC);
                            if (festivalDay) islandCount += 0.5f;
                            else if (locat.IsGreenRainingHere()) islandCount += 2f;
                            else if (locat.IsLightningHere()) islandCount += 1.6f;
                            else if (locat.IsRainingHere()) islandCount += 1.3f;
                            else islandCount += 1f;
                        }
                    }

                    foreach (Building building in locat.buildings)      // Get available building, creat building warp point
                    {
                        if (building != null && building.GetIndoorsName() != null)
                        {
                            Vector2 outdoorTile = new Vector2(building.humanDoor.X + building.tileX.Value, building.humanDoor.Y + building.tileY.Value + 2);

                            Vector2 indoorTile = new Vector2(0, 0);
                            if (Game1.getLocationFromName(building.GetIndoorsName()) != null)
                            {
                                foreach (var warp in Game1.getLocationFromName(building.GetIndoorsName()).warps)
                                {
                                    if (warp.TargetName == "Custom_MT_Island") indoorTile = new(warp.X, warp.Y - 3);
                                    break;
                                }
                            }

                            IslandValidBuilding.Add(new IslandBuildingProperties(building.GetIndoorsName(), indoorTile, outdoorTile));

                            var newInWarp = new Warp((int)outdoorTile.X, (int)outdoorTile.Y - 1, building.GetIndoorsName(), (int)indoorTile.X, (int)indoorTile.Y, false, true);
                            if ( !locat.warps.Contains(newInWarp) ) locat.warps.Add( newInWarp );
                        }
                    }

                    // If today is Festival day, init chest, sign and visitor schedule
                    if (festivalDay)
                    {
                        IsFestivalToday = true;

                        var chest = new Chest(true);
                        var sign = new Sign(new Vector2(69, 21), "37");

                        chest.destroyOvernight = true;
                        chest.Fragility = 2;
                        while (chest.Items.Count < 3) chest.Items.Add(ItemRegistry.Create<Item>("MT.Objects.CustomerNote"));

                        sign.destroyOvernight = true;
                        sign.Fragility = 2;

                        locat.setObjectAt(69, 19, chest);
                        locat.setObjectAt(69, 21, sign);

                        // create Player shop
                        var displayChest = new Chest(true);
                        displayChest.destroyOvernight = true;
                        while (displayChest.Items.Count < 9) displayChest.Items.Add(null);
                        locat.setObjectAt(19309, 19309, displayChest);

                        List<string> displayChestItem = new List<string>();
                        foreach (var item in displayChest.Items)
                        {
                            displayChestItem.Add(null);
                        }

                        TodayShopInventory.Add(new MarketShopData("PlayerShop", new Vector2(66, 21), displayChestItem));

                        SetupShop(true);
                    }


                    // *** Warp Visitors, clear schedule and init a new schedule
                    foreach (var islandVisitorName in IslandNPCList)
                    {
                        NPC islandVisitor = Game1.getCharacterFromName(islandVisitorName);
                        if (islandVisitor != null)
                        {
                            Vector2 defautlIslandWarp = islandWarp[rand.Next(islandWarp.Count)];
                            int initFaceDirection = rand.Next(0, 4);

                            Game1.warpCharacter(islandVisitor, locat, defautlIslandWarp);
                            islandVisitor.faceDirection(initFaceDirection);

                            TodayVisitorVisited++;

                            ResetErrorNpc(islandVisitor);
                            islandVisitor.TryLoadSchedule("default", $"600 Custom_MT_Island {defautlIslandWarp.X} {defautlIslandWarp.Y} {initFaceDirection}/");

                            var randomTile = FarmOutside.getRandomOpenPointInFarm(islandVisitor, islandVisitor.currentLocation, false).ToVector2();
                            if (randomTile != Vector2.Zero)
                            {
                                FarmOutside.AddRandomSchedulePoint(islandVisitor, $"610", $"{islandVisitor.currentLocation.NameOrUniqueName}",
                                    $"{randomTile.X}", $"{randomTile.Y}", $"{Game1.random.Next(0, 4)}");
                            }

                            islandVisitor.wearIslandAttire();

                            if (IsFestivalToday) SetVisitorSchedule(islandVisitor);
                        }
                    }
                }
                else locat.isAlwaysActive.Value = true;         // If Fire is off, location is build-able
            } catch { }

            // Check store and museum
            List<int> categoryKeys = new List<int> { 0, -2, -12, -28, -102 };
            int museumPieces = 0;
            foreach (Building building in Game1.getFarm().buildings)
            {
                bool valid = false;

                if ( building != null && building.GetIndoorsName() != null)
                {
                    bool isMuseumBuilding = false;
                    foreach (var obj in Game1.getLocationFromName(building.GetIndoorsName()).Objects.Values)
                    {
                        // Case Museum
                        if (obj is Sign sign && sign.displayItem.Value != null && sign.displayItem.Value.Name == "Museum License")
                        {
                            GameLocation location = Game1.getLocationFromName(building.GetIndoorsName());

                            FarmOutside.UpdateRandomLocationOpenTile(location);
                            foreach (var f in location.furniture)
                            {
                                if (f.heldObject.Value != null && categoryKeys.Contains(f.heldObject.Value.Category)) { museumPieces++; }
                                if (f is FishTankFurniture fishtank) { museumPieces += (int)(fishtank.tankFish.Count / 2); }
                                if (f.Name.Contains("Statue")) { museumPieces += 2; }
                            }

                            validBuildingObjectPairs.Add(new BuildingObjectPair(building, obj, "museum", museumPieces));
                            if (!Config.RestaurantLocations.Contains(Game1.getLocationFromName(building.GetIndoorsName()).Name)) 
                                Config.RestaurantLocations.Add(Game1.getLocationFromName(building.GetIndoorsName()).Name);
                            isMuseumBuilding = true;

                            Vector2 indoorTile = new Vector2(0, 0);
                            if (Game1.getLocationFromName(building.GetIndoorsName()) != null)
                            {
                                foreach (var warp in Game1.getLocationFromName(building.GetIndoorsName()).warps)
                                {
                                    if (warp.TargetName == "Farm") indoorTile = new(warp.X, warp.Y - 3);
                                    break;
                                }
                            }
                            var busWarp = new Warp(19309, 19309, building.GetIndoorsName(), (int)indoorTile.X, (int)indoorTile.Y, false, false);
                            if (!Game1.getLocationFromName("BusStop").warps.Contains(busWarp)) Game1.getLocationFromName("BusStop").warps.Add(busWarp);

                            valid = true;
                            break;
                        }
                    }

                    if (!isMuseumBuilding)
                    {
                        foreach (var obj in Game1.getLocationFromName(building.GetIndoorsName()).Objects.Values)
                        {
                            //Case Market or Restaurant
                            if (obj is Sign sign1 && sign1.displayItem.Value != null && (sign1.displayItem.Value.Name == "Market License" || sign1.displayItem.Value.Name == "Restaurant License"))
                            {
                                FarmOutside.UpdateRandomLocationOpenTile(Game1.getLocationFromName(building.GetIndoorsName()));

                                validBuildingObjectPairs.Add(new BuildingObjectPair(building, obj, "market", 0));
                                if (!Config.RestaurantLocations.Contains(Game1.getLocationFromName(building.GetIndoorsName()).Name)) 
                                    Config.RestaurantLocations.Add(Game1.getLocationFromName(building.GetIndoorsName()).Name);

                                Vector2 indoorTile = new Vector2(0, 0);
                                if (Game1.getLocationFromName(building.GetIndoorsName()) != null)
                                {
                                    foreach (var warp in Game1.getLocationFromName(building.GetIndoorsName()).warps)
                                    {
                                        if (warp.TargetName == "Farm") indoorTile = new(warp.X, warp.Y - 3);
                                        break;
                                    }
                                }
                                var busWarp = new Warp(19309, 19309, building.GetIndoorsName(), (int)indoorTile.X, (int)indoorTile.Y, false, false);
                                if (!Game1.getLocationFromName("BusStop").warps.Contains(busWarp)) Game1.getLocationFromName("BusStop").warps.Add(busWarp);

                                valid = true;
                                break;
                            }
                        }
                    }
                }

                if (valid)
                {
                    foreach (var y in Game1.getLocationFromName(building.GetIndoorsName()).Objects)
                    {
                        foreach (var z in y.Values)
                        {
                            if (!Game1.getLocationFromName(building.GetIndoorsName()).doesEitherTileOrTileIndexPropertyEqual((int)z.TileLocation.X, (int)z.TileLocation.Y, "NoPath", "Back", "T"))
                            {
                                Game1.getLocationFromName(building.GetIndoorsName()).setTileProperty((int)z.TileLocation.X, (int)z.TileLocation.Y, "Back", "NoPath", "T");

                                if (!TilePropertyChanged.ContainsKey(Game1.getLocationFromName(building.GetIndoorsName())))
                                {
                                    TilePropertyChanged[Game1.getLocationFromName(building.GetIndoorsName())] = new List<Vector2>();
                                }
                                TilePropertyChanged[Game1.getLocationFromName(building.GetIndoorsName())].Add(z.TileLocation);
                            }
                        }
                    }

                    foreach (var z in Game1.getLocationFromName(building.GetIndoorsName()).furniture)
                    {
                        Vector2 tableTileLocation = z.TileLocation;

                        int tableWidth = z.getTilesWide();
                        int tableHeight = z.getTilesHigh();

                        for (int x = 0; x < tableWidth; x++)
                        {
                            for (int y = 0; y < tableHeight; y++)
                            {
                                if (!Game1.getLocationFromName(building.GetIndoorsName()).doesEitherTileOrTileIndexPropertyEqual((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "NoPath", "Back", "T"))
                                {
                                    Game1.getLocationFromName(building.GetIndoorsName()).setTileProperty((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y, "Back", "NoPath", "T");

                                    if (!TilePropertyChanged.ContainsKey(Game1.getLocationFromName(building.GetIndoorsName())))
                                    {
                                        TilePropertyChanged[Game1.getLocationFromName(building.GetIndoorsName())] = new List<Vector2>();
                                    }
                                    TilePropertyChanged[Game1.getLocationFromName(building.GetIndoorsName())].Add(new Vector2((int)tableTileLocation.X + x, (int)tableTileLocation.Y + y));
                                }
                            }
                        }
                    }
                }
            }

            // Update open tile list
            foreach (var buildingLocation in validBuildingObjectPairs)
            {
                var buildingInstanceName = buildingLocation.Building.GetIndoorsName();
                FarmOutside.UpdateRandomLocationOpenTile(Game1.getLocationFromName(buildingInstanceName));
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            // Wipe invitation
            try
            {
                foreach (NPC __instance in Utility.getAllVillagers())
                {

                    TodayCustomerInteraction += Int32.Parse(__instance.modData["hapyke.FoodStore/TotalCustomerResponse"]);
                    if ((__instance.IsVillager && __instance.modData.ContainsKey("hapyke.FoodStore/invited") && __instance.modData.ContainsKey("hapyke.FoodStore/inviteDate")
                        && __instance.modData["hapyke.FoodStore/invited"] == "true" && Int32.Parse(__instance.modData["hapyke.FoodStore/inviteDate"]) <= (Game1.stats.DaysPlayed - 1))
                        || (__instance.IsVillager && (!__instance.modData.ContainsKey("hapyke.FoodStore/invited")) || !__instance.modData.ContainsKey("hapyke.FoodStore/inviteDate")))
                    {
                        __instance.modData["hapyke.FoodStore/invited"] = "false";
                        __instance.modData["hapyke.FoodStore/inviteDate"] = "-99";
                    }
                }
            }
            catch { }

            try
            {
                Config.RestaurantLocations.Clear();
                validBuildingObjectPairs.Clear();

                var mailHistory = MailRepository.FindLetter("MT.SellLogMail");
                var weeklyHistory = MailRepository.FindLetter("MT.WeeklyLogMail");

                if (mailHistory != null) MailRepository.RemoveLetter(mailHistory);
                if (weeklyHistory != null) MailRepository.RemoveLetter(weeklyHistory);

            }
            catch { }

            EndOfDaySave();

            // Island logic
            GameLocation locat = Game1.getLocationFromName("Custom_MT_Island");

            if (locat != null && ( Game1.dayOfMonth != 28 && locat.GetSeasonKey().ToLower() == "fall" || locat.GetSeasonKey().ToLower() != "fall") 
                && !locat.Map.Properties.ContainsKey("skipWeedGrowth") && !locat.IsGreenRainingHere()) 
                locat.Map.Properties.Add("skipWeedGrowth", true);

            CloseShop(true);

            try
            {
                locat.warps.Clear();
                locat.updateWarps();

                foreach (var warp in Game1.getLocationFromName("BusStop").warps)
                {
                    if (warp.X == 19309 && warp.Y == 19309) Game1.getLocationFromName("BusStop").warps.Remove(warp);
                }
            } catch { }

            foreach (var entry in TilePropertyChanged)
            {
                var location = entry.Key;
                var tileList = entry.Value;

                foreach (var tile in tileList)
                {
                    int i = (int)tile.X;
                    int j = (int)tile.Y;

                    location.removeTileProperty(i, j, "Back", "NoPath");
                }
            }

            foreach (var animal in locat.Animals)
            {
                foreach (var realAni in animal.Values)
                {
                    if (realAni.modData.ContainsKey("hapyke.FoodStore/isFakeAnimal") && realAni.modData["hapyke.FoodStore/isFakeAnimal"] == "true")
                        locat.animals.Remove(realAni.myID.Value); 

                }
            }

            // restock table item
            RestockTable(true);
            RecentSoldTable.Clear();
        }

        // ----------- End of Day -----------

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            if (e.IsMultipleOf(30) && !Config.DisableTextChat)
            {
                if (playerChatInstance == null)
                {
                    playerChatInstance = new PlayerChat();
                }
                playerChatInstance.Validate();

                if (RecentSoldTable.Count > 0)
                {
                    foreach (var kvp in RecentSoldTable)
                    {
                        var enumerator = kvp.Key;
                        if (enumerator != null)
                        {
                            var location = enumerator.Current.Location;
                            var tile = enumerator.Current.TileLocation;

                            if (location.getObjectAtTile((int)tile.X, (int)tile.Y) == null || location.getObjectAtTile((int)tile.X, (int)tile.Y).QualifiedItemId != enumerator.Current.QualifiedItemId) 
                                RecentSoldTable.Remove(kvp);
                        }
                    }
                }
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!Game1.hasLoadedGame) return;

            Random random = new Random();

            if (Context.IsPlayerFree && Game1.player != null && Game1.player.currentLocation != null
                && (Config.RestaurantLocations.Contains(Game1.player.currentLocation.Name) || Game1.player.currentLocation.GetParentLocation() == Game1.getLocationFromName("Custom_MT_Island")))
            {
                UpdateOrders();
            }

            NpcFestivalPurchase();
        }

        private void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            Random random = new Random();

            //foreach (var asd in Game1.player.currentLocation.furniture)
            //{
            //    if ( asd.heldObject.Value is Chest chest)
            //    {
            //        foreach (var asdasd in chest.Items)
            //        {
            //            if (asdasd != null) Game1.chatBox.addInfoMessage(asdasd.Name);
            //            else Game1.chatBox.addInfoMessage("null");
            //        }
            //    }
            //}

            // restock table item
            RestockTable();

            if ( Game1.timeOfDay % 200 == 0)
            {
                var islandInstance = Game1.getLocationFromName("Custom_MT_Island");
                var islandHouseInstance = Game1.getLocationFromName("Custom_MT_Island_House");

                FarmOutside.UpdateRandomLocationOpenTile(islandInstance);
                FarmOutside.UpdateRandomLocationOpenTile(islandHouseInstance);

                foreach (var buildingLocation in islandInstance.buildings)
                {
                    var buildingInstanceName = buildingLocation.GetIndoorsName();
                    FarmOutside.UpdateRandomLocationOpenTile(Game1.getLocationFromName(buildingInstanceName));
                }

                foreach (var building in validBuildingObjectPairs)
                {
                    var buildingInstanceName = Game1.getLocationFromName(building.Building.GetIndoorsName());
                    FarmOutside.UpdateRandomLocationOpenTile(buildingInstanceName);
                }
            }

            // Island Festival manager
            if (e.NewTime == Config.FestivalTimeEnd && IsFestivalToday) CloseShop(false);
            if ( (e.NewTime - Config.FestivalTimeStart) % 300 == 0 && IsFestivalIsCurrent) SetupShop(false);
            if (e.NewTime == Config.FestivalTimeStart && IsFestivalToday) OpenShop(OpenShopTile);

            //Send dish of the day
            if (Game1.timeOfDay == 900 && !Config.DisableChatAll && Game1.IsMasterGame)
            {
                int randomIndex = random.Next(10);
                Game1.chatBox.addInfoMessage(SHelper.Translation.Get("foodstore.dishday." + randomIndex.ToString(), new { dishToday = DishPrefer.dishDay }));
                MyMessage messageToSend = new MyMessage(SHelper.Translation.Get("foodstore.dishday." + randomIndex.ToString(), new { dishToday = DishPrefer.dishDay }));
                SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");
            }

            if (Game1.timeOfDay == 630 && Game1.player.currentLocation.Name == "Farm" && !Config.DisableKidAsk && random.NextDouble() < Config.KidAskChance
                && !(Game1.player.isRidingHorse()
                    || Game1.currentLocation == null
                    || Game1.eventUp
                    || Game1.isFestival()
                    || Game1.IsFading()
                    || Game1.activeClickableMenu != null
                    || !Game1.player.CanMove
                    || Game1.dialogueUp
                    || Game1.player.UsingTool
                    || Game1.player.ActiveItem is Tool
                    || Game1.player.ActiveItem is MeleeWeapon) )
            {
                TodaySelectedKid.Clear();

                foreach (string kid in GlobalKidList)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        int friendshipLevel = Game1.player.getFriendshipHeartLevelForNPC(kid);
                        double addKid = 0;
                        switch (friendshipLevel)
                        {
                            case int lv when lv < 2:
                                addKid = 0.1;
                                break;

                            case int lv when lv >= 2 && lv < 4:
                                addKid = 0.25;
                                break;

                            case int lv when lv >= 4 && lv < 6:
                                addKid = 0.4;
                                break;

                            case int lv when lv >= 6:
                                addKid = 0.6;
                                break;
                            default:
                                break;
                        }
                        try
                        {
                            if (random.NextDouble() < addKid && Game1.getCharacterFromName(kid).modData["hapyke.FoodStore/invited"] != "true") TodaySelectedKid.Add(kid, friendshipLevel);
                        }
                        catch { }
                    }
                }
                if (TodaySelectedKid.Count != 0)
                {
                    string[] keysArray = new List<string>(TodaySelectedKid.Keys).ToArray();
                    string randomKey = keysArray[new Random().Next(keysArray.Length)];
                    var formattedQuestion = "";

                    if (TodaySelectedKid.Count() == 1) formattedQuestion = string.Format(SHelper.Translation.Get("foodstore.kidask", new { kidName = Game1.getCharacterFromName(randomKey).Name }), Game1.getCharacterFromName(randomKey).Name);
                    else formattedQuestion = string.Format(SHelper.Translation.Get("foodstore.groupkidask"));

                    var entryQuestion = new EntryQuestion(formattedQuestion, KidResponseList, ActionList);
                    Game1.activeClickableMenu = entryQuestion;

                    ActionList.Add(() => KidJoin(TodaySelectedKid));
                    ActionList.Add(() => Game1.DrawDialogue(new Dialogue(Game1.getCharacterFromName(randomKey), "key", SHelper.Translation.Get("foodstore.kidresponselist.boring"))));

                }
            }

            // ******* Farm building visitors *******
            if (random.NextDouble() < Config.ShedVisitChance && Game1.timeOfDay <= Config.CloseHour && Game1.timeOfDay >= Config.OpenHour)
            {
                foreach (var pair in validBuildingObjectPairs)
                {
                    Building building = pair.Building;
                    Object obj = pair.Object;
                    string buildingType = pair.buildingType;
                    int ticketValue = pair.ticketValue;

                    Vector2 doorTile = new(0f, 0f);

                    if (GlobalNPCList.Count == 0 || building == null || obj == null || building.GetIndoorsName() == null
                        || CountShedVisitor(Game1.getLocationFromName(building.GetIndoorsName())) >= Config.MaxShedCapacity 
                        || ( buildingType == "museum" && Config.MuseumPriceMarkup / 4.1 > random.NextDouble()) ) return;

                    foreach (var warp in Game1.getLocationFromName(building.GetIndoorsName()).warps)
                    {
                        if (warp.TargetName == "Farm") doorTile = new(warp.X, warp.Y - 3);
                        break;
                    }
                    
                    string randomNPCName = GlobalNPCList[new Random().Next(0, GlobalNPCList.Count)];
                    var visit = Game1.getCharacterFromName(randomNPCName);

                    bool blockedNPC = false;
                    try
                    {
                        if (visit != null)
                        {
                            blockedNPC = visit.currentLocation.IsFarm
                                || (visit.currentLocation.GetParentLocation() != null && visit.currentLocation.GetParentLocation().IsFarm)
                                || visit.currentLocation == Game1.player.currentLocation
                                || (Game1.player.friendshipData.ContainsKey(randomNPCName) 
                                    && ( Game1.player.friendshipData[randomNPCName].IsMarried() || Game1.player.friendshipData[randomNPCName].IsRoommate()) )
                                || visit.currentLocation.Name.Contains("Custom_MT_Island")
                                || (visit.currentLocation.GetParentLocation() != null && visit.currentLocation.GetParentLocation().Name == "Custom_MT_Island")
                                || Int32.Parse(visit.modData["hapyke.FoodStore/timeVisitShed"]) >= (Game1.timeOfDay - Config.TimeStay * 3);
                        }
                    }
                    catch { }

                    if (visit == null || blockedNPC) return;
                    ResetErrorNpc(visit);
                    List<Vector2> clearTiles = new List<Vector2>();
                    if (Config.DoorEntry)          // Alow warp at door
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                Vector2 checkLocation = doorTile + new Vector2(x, y);
                                if (!Game1.currentLocation.IsTileBlockedBy(checkLocation)) clearTiles.Add(checkLocation);
                            }
                        }

                        if (clearTiles.Count > 0)
                        {
                            Vector2 randomClearTile = clearTiles[Game1.random.Next(clearTiles.Count)];

                            if (Game1.player.currentLocation.Name == "BusStop" && Config.BusWalk && Game1.MasterPlayer.mailReceived.Contains("ccVault"))
                            {
                                Game1.warpCharacter(visit, "BusStop", new Point(24, 11));
                                visit.isCharging = true;
                                visit.addedSpeed = 1;
                                visit.temporaryController = new PathFindController(visit, visit.currentLocation, new Point(13, 24), 3,
                                (character, location) => Game1.warpCharacter(visit, building.GetIndoorsName(), randomClearTile));
                            }
                            else Game1.warpCharacter(visit, building.GetIndoorsName(), randomClearTile);

                            visit.modData["hapyke.FoodStore/shedEntry"] = $"{randomClearTile.X},{randomClearTile.Y}";
                        }
                        else
                        {
                            if (Game1.player.currentLocation.Name == "BusStop" && Config.BusWalk && Game1.MasterPlayer.mailReceived.Contains("ccVault"))
                            {
                                Game1.warpCharacter(visit, "BusStop", new Point(24, 11));
                                visit.isCharging = true;
                                visit.addedSpeed = 1;
                                visit.temporaryController = new PathFindController(visit, visit.currentLocation, new Point(13, 24), 3,
                                (character, location) => Game1.warpCharacter(visit, building.GetIndoorsName(), doorTile));
                            }
                            else Game1.warpCharacter(visit, building.GetIndoorsName(), doorTile);

                            visit.modData["hapyke.FoodStore/shedEntry"] = doorTile.X.ToString() + doorTile.Y.ToString();
                        }
                        clearTiles.Clear();
                    }

                    else                                                                                // Warp at Sign
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                Vector2 checkLocation = new Vector2(obj.TileLocation.X, obj.TileLocation.Y) + new Vector2(x, y);
                                if (!Game1.currentLocation.IsTileBlockedBy(checkLocation)) clearTiles.Add(checkLocation);
                            }
                        }

                        if (clearTiles.Count > 0)
                        {
                            Vector2 randomClearTile = clearTiles[Game1.random.Next(clearTiles.Count)];

                            if (Game1.player.currentLocation.Name == "BusStop" && Config.BusWalk && Game1.MasterPlayer.mailReceived.Contains("ccVault"))
                            {
                                Game1.warpCharacter(visit, "BusStop", new Point(24, 11));
                                visit.isCharging = true;
                                visit.addedSpeed = 1;
                                visit.temporaryController = new PathFindController(visit, visit.currentLocation, new Point(13, 24), 3,
                                (character, location) => Game1.warpCharacter(visit, building.GetIndoorsName(), randomClearTile));
                            }
                            else Game1.warpCharacter(visit, building.GetIndoorsName(), randomClearTile);

                            visit.modData["hapyke.FoodStore/shedEntry"] = $"{randomClearTile.X},{randomClearTile.Y}";
                        }
                        else
                        {
                            if (Game1.player.currentLocation.Name == "BusStop" && Config.BusWalk && Game1.MasterPlayer.mailReceived.Contains("ccVault"))
                            {
                                Game1.warpCharacter(visit, "BusStop", new Point(24, 11));
                                visit.isCharging = true;
                                visit.addedSpeed = 1;
                                visit.temporaryController = new PathFindController(visit, visit.currentLocation, new Point(13, 24), 3,
                                (character, location) => Game1.warpCharacter(visit, building.GetIndoorsName(), new Vector2(obj.TileLocation.X, obj.TileLocation.Y)));
                            }
                            else Game1.warpCharacter(visit, building.GetIndoorsName(), new Vector2(obj.TileLocation.X, obj.TileLocation.Y));


                            visit.modData["hapyke.FoodStore/shedEntry"] = (obj.TileLocation.X).ToString() + "," + (obj.TileLocation.Y).ToString();
                        }
                        clearTiles.Clear();
                    }

                    // Try add museum ticket money, or do special order table sit
                    if (buildingType == "museum")
                    {
                        TodayMuseumVisitor++;

                        AddToPlayerFunds((int)(10 * ticketValue * Config.MuseumPriceMarkup));
                        TodayMuseumEarning += (int)(10 * ticketValue * Config.MuseumPriceMarkup);

                    }
                    else if (random.NextDouble() < Config.TableSit)
                    {
                        Dictionary<Vector2, int> surroundingTiles = new Dictionary<Vector2, int>();

                        foreach ( var table in Game1.getLocationFromName(building.GetIndoorsName()).furniture)
                        {
                            if (table != null && table.heldObject.Value != null && table.heldObject.Value.QualifiedItemId == "(F)MT.Objects.RestaurantDecor")
                            {
                                Vector2 topLeft = table.TileLocation;
                                int width = table.getTilesWide();
                                int height = table.getTilesHigh();

                                for (int x = 0; x < width; x++) surroundingTiles.Add(new Vector2(topLeft.X + x, topLeft.Y - 1), 2); // down
                                for (int x = 0; x < width; x++) surroundingTiles.Add(new Vector2(topLeft.X + x, topLeft.Y + height), 0); // up
                                for (int y = 0; y < height; y++) surroundingTiles.Add(new Vector2(topLeft.X - 1, topLeft.Y + y), 1); // right
                                for (int y = 0; y < height; y++) surroundingTiles.Add(new Vector2(topLeft.X + width, topLeft.Y + y), 3); // left
                            }
                        }

                        int totalTile = surroundingTiles.Count;
                        while (totalTile > 0 && totalTile > 0)
                        {
                            totalTile--;
                            var randomPair = surroundingTiles.ElementAt(random.Next(surroundingTiles.Count));

                            var randomTile = randomPair.Key;
                            var randomDirection = randomPair.Value;


                            if (visit.currentLocation.CanSpawnCharacterHere(randomTile) && (!RestaurantSpot.ContainsKey(visit.currentLocation) || !RestaurantSpot[visit.currentLocation].Contains(randomTile)) )
                            {
                                FarmOutside.AddRandomSchedulePoint(visit, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{visit.currentLocation.NameOrUniqueName}", $"{randomTile.X}", $"{randomTile.Y}", $"{randomDirection}");
                                visit.modData["hapyke.FoodStore/specialOrder"] = $"{randomTile.X},{randomTile.Y}";

                                if (!RestaurantSpot.ContainsKey(visit.currentLocation)) RestaurantSpot[visit.currentLocation] = new List<Vector2>();
                                RestaurantSpot[visit.currentLocation].Add(randomTile);

                                CheckOrder(visit, visit.currentLocation, true);

                                break;
                            }
                        }
                    }

                    visit.modData["hapyke.FoodStore/timeVisitShed"] = Game1.timeOfDay.ToString();
                }
            }   // ****** end of shed visitor ******
        }

        // ------------ End of Tick -----------

        private static bool TryToEatFood(NPC __instance, DataPlacedFood food)
        {
            try
            {
                if (food != null && __instance.IsVillager && !__instance.Name.Contains("derby") && food.furniture != null && Vector2.Distance(food.foodTile, __instance.Tile) < Config.MaxDistanceToEat && !__instance.Name.EndsWith("_DA") && !bool.Parse(__instance.modData["hapyke.FoodStore/gettingFood"]))
                {
                    if ((__instance.currentLocation is Farm || __instance.currentLocation is FarmHouse) && !Config.AllowRemoveNonFood && food.foodObject.Edibility <= 0)
                    {
                        return false;
                    }
                    using (IEnumerator<Furniture> enumerator = __instance.currentLocation.furniture.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {

                            //Remove food /// **************************************************************************************************************************************************************************
                            if (enumerator.Current.boundingBox.Value != food.furniture.boundingBox.Value)
                                continue;
                            if (enumerator.Current.heldObject.Value is not Chest) enumerator.Current.heldObject.Value = null;
                            else if (enumerator.Current.heldObject.Value is Chest chest) chest.Items[food.slot] = null;

                            if (!RecentSoldTable.ContainsKey(enumerator)) RecentSoldTable.Add(enumerator, Game1.timeOfDay);
                            else RecentSoldTable[enumerator] = Game1.timeOfDay;

                            int taste = 8;
                            try
                            {
                                taste = __instance.getGiftTasteForThisItem(food.foodObject);
                                if (__instance.Name == "Gus" && (taste == 0 || taste == 2)) taste = 8;
                            }
                            catch { }
                            string reply = "";
                            int salePrice = food.foodObject.sellToStorePrice();
                            int tip = 0;
                            double decorPoint = GetDecorPoint(food.foodTile, __instance.currentLocation);
                            Random rand = new Random();
                            string itemName = (food == null || food.foodObject == null || food.foodObject.DisplayName == null)
                                ? "Item" : food.foodObject.displayName;

                            if (food.foodObject.Category == -7)
                            {
                                // Get Reply, Sale Price, Tip for each taste
                                if (taste == 0)         //Love
                                {
                                    reply = SHelper.Translation.Get("foodstore.loverep." + rand.Next(20).ToString());

                                    if (Config.LoveMultiplier == -1 || !Config.EnablePrice)
                                    {
                                        salePrice = (int)(salePrice * (1.75 + rand.NextDouble()));
                                    }
                                    else salePrice = (int)(salePrice * Config.LoveMultiplier);

                                    if (Config.TipLove == -1 || !Config.EnableTip)
                                    {
                                        tip = (int)(salePrice * 0.3);
                                    }
                                    else tip = (int)(salePrice * Config.TipLove);

                                    if (tip < 20) { tip = 20; }
                                }               //love
                                else if (taste == 2)    //Like
                                {
                                    reply = SHelper.Translation.Get("foodstore.likerep." + rand.Next(20).ToString());

                                    if (Config.LikeMultiplier == -1 || !Config.EnablePrice)
                                    {
                                        salePrice = (int)(salePrice * (1.25 + (rand.NextDouble() / 2)));
                                    }
                                    else salePrice = (int)(salePrice * Config.LikeMultiplier);

                                    if (Config.TipLike == -1 || !Config.EnableTip)
                                    {
                                        tip = (int)(salePrice * 0.2);
                                    }
                                    else tip = (int)(salePrice * Config.TipLike);

                                    if (tip < 10) { tip = 10; }
                                }          //like
                                else if (taste == 4)    //Dislike
                                {
                                    reply = SHelper.Translation.Get("foodstore.dislikerep." + rand.Next(20).ToString());

                                    if (Config.DislikeMultiplier == -1 || !Config.EnablePrice)
                                    {
                                        salePrice = (int)(salePrice * (0.75 + (rand.NextDouble() / 3)));
                                    }
                                    else salePrice = (int)(salePrice * Config.DislikeMultiplier);

                                    if (Config.TipDislike == -1 || !Config.EnableTip)
                                    {
                                        tip = 2;
                                    }
                                    else tip = (int)(salePrice * Config.TipDislike);
                                }          //dislike
                                else if (taste == 6)    //Hate
                                {
                                    reply = SHelper.Translation.Get("foodstore.haterep." + rand.Next(20).ToString());
                                    if (Config.HateMultiplier == -1 || !Config.EnablePrice)
                                    {
                                        salePrice = (int)(salePrice / 2);
                                    }
                                    else salePrice = (int)(salePrice * Config.HateMultiplier);

                                    if (Config.TipHate == -1 || !Config.EnableTip)
                                    {
                                        tip = 0;
                                    }
                                    else tip = (int)(salePrice * Config.TipHate);

                                }          //hate
                                else                    //Neutral
                                {
                                    reply = SHelper.Translation.Get("foodstore.neutralrep." + rand.Next(20).ToString());


                                    if (Config.NeutralMultiplier == -1 || !Config.EnablePrice)
                                    {
                                        salePrice = (int)(salePrice * (1 + (rand.NextDouble() / 5)));
                                    }
                                    else salePrice = (int)(salePrice * Config.NeutralMultiplier);

                                    if (Config.TipNeutral == -1 || !Config.EnableTip)
                                    {
                                        tip = (int)(salePrice * 0.1);
                                    }
                                    else tip = (int)(salePrice * Config.TipNeutral);

                                    if (tip < 5) { tip = 5; }

                                }                          //neutral

                                try
                                {
                                    switch (food.foodObject.Quality)
                                    {
                                        case 4:
                                            salePrice = (int)(salePrice * 1.75);
                                            break;
                                        case 2:
                                            salePrice = (int)(salePrice * 1.4);
                                            break;
                                        case 1:
                                            salePrice = (int)(salePrice * 1.15);
                                            break;
                                        default:
                                            salePrice = (int)(salePrice * 1);
                                            break;
                                    }
                                } catch { }

                            } // **** SALE and TIP block ****
                            
                            //else if (food.foodObject is WeaponProxy)
                            //{

                            //    WeaponProxy weaponProxy = (WeaponProxy)food.foodObject;
                            //    int weaponName = weaponProxy.WeaponName;
                            //    int weaponSalePrice = weaponProxy.SalePrice;

                            //    itemName = weaponName;
                            //    reply = SHelper.Translation.Get("foodstore.weaponText");
                            //    salePrice = (int)(weaponSalePrice * 1.5);
                            //    tip = 0;
                            //}
                            
                            else    // Non-food case
                            {
                                tip = 0;
                                switch (food.foodObject.Quality)
                                {
                                    case 4:
                                        reply = SHelper.Translation.Get("foodstore.nonfood." + food.foodObject.Quality.ToString() + "." + rand.Next(9));
                                        salePrice = (int)(salePrice * 3);
                                        break;
                                    case 2:
                                        reply = SHelper.Translation.Get("foodstore.nonfood." + food.foodObject.Quality.ToString() + "." + rand.Next(9));
                                        salePrice = (int)(salePrice * 2.5);
                                        break;
                                    case 1:
                                        reply = SHelper.Translation.Get("foodstore.nonfood." + food.foodObject.Quality.ToString() + "." + rand.Next(9));
                                        salePrice = (int)(salePrice * 2);
                                        break;
                                    default:
                                        reply = SHelper.Translation.Get("foodstore.nonfood." + food.foodObject.Quality.ToString() + "." + rand.Next(9));
                                        salePrice = (int)(salePrice * 1.5);
                                        break;
                                }
                            }

                            //Multiply with decoration point
                            if (Config.EnableDecor) salePrice = (int)(salePrice * (1 + decorPoint));

                            //Feature dish
                            if (food.foodObject.Name == DishPrefer.dishDay) { salePrice = (int)(salePrice * 1.5); }
                            if (food.foodObject.Name == DishPrefer.dishWeek) { salePrice = (int)(salePrice * 1.3); }

                            //Config Rush hours Price
                            if (Config.RushHour && tip != 0 
                                && ((800 < Game1.timeOfDay && Game1.timeOfDay < 930) || (1200 < Game1.timeOfDay && Game1.timeOfDay < 1330) || (1800 < Game1.timeOfDay && Game1.timeOfDay < 2000)))
                            {
                                salePrice = (int)(salePrice * 0.8);
                                tip = (int)(tip * 2);
                            }

                            // If Location is under Island
                            if (__instance.currentLocation == Game1.getLocationFromName("Custom_MT_Island") 
                                || __instance.currentLocation.GetParentLocation() == Game1.getLocationFromName("Custom_MT_Island"))
                            {
                                if (!Config.IslandProgress) salePrice = (int)(salePrice * 1.5);
                                else
                                {
                                    MailData model = null;

                                    if (Game1.IsMasterGame) model = SHelper.Data.ReadSaveData<MailData>("MT.MailLog");

                                    int totalCustomerNoteYes = model.TotalCustomerNoteYes;
                                    int totalCustomerNoteNo = model.TotalCustomerNoteNo;

                                    salePrice = (int)(salePrice * (1 + Math.Min(totalCustomerNoteYes / (totalCustomerNoteYes + totalCustomerNoteNo + 20) , 0.5)) );
                                }
                            }

                            //Number of customer interaction
                            try
                            {
                                if (__instance.modData["hapyke.FoodStore/TotalCustomerResponse"] != null)
                                {
                                    double totalInteract = Int32.Parse(__instance.modData["hapyke.FoodStore/TotalCustomerResponse"]) / 6.67;
                                    if (totalInteract > 0.3) totalInteract = 0.3;
                                    salePrice = (int)(salePrice * (1 + totalInteract));
                                }
                            }
                            catch (Exception) { }

                            //Config Tip when nearby
                            if (Config.TipWhenNeaBy && Utility.isThereAFarmerWithinDistance(food.foodTile, 15, __instance.currentLocation) == null) { tip = 0; }

                            //Money on/off farm
                            if (__instance.currentLocation is not FarmHouse && __instance.currentLocation is not Farm && !Config.DisableChatAll)
                            {
                                //Generate chat box
                                if (tip != 0)
                                    NPCShowTextAboveHead(__instance, reply + SHelper.Translation.Get("foodstore.tip", new { tipValue = tip }));
                                else
                                    NPCShowTextAboveHead(__instance, reply);

                                //Generate chat box
                                if (Game1.IsMultiplayer)
                                {
                                    Game1.chatBox.addInfoMessage(SHelper.Translation.Get("foodstore.sold", new { foodObjName = itemName, locationint = __instance.currentLocation.DisplayName, saleint = salePrice }));
                                    MyMessage messageToSend = new MyMessage(SHelper.Translation.Get("foodstore.sold", new { foodObjName = itemName, locationint = __instance.currentLocation.DisplayName, saleint = salePrice }));
                                    SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");

                                    if (!Config.DisableChat)
                                    {
                                        if (tip != 0)
                                        {
                                            Game1.chatBox.addInfoMessage($"   {__instance.displayName}: " + reply + SHelper.Translation.Get("foodstore.tip", new { tipValue = tip }));
                                            messageToSend = new MyMessage($"   {__instance.displayName}: " + reply + SHelper.Translation.Get("foodstore.tip", new { tipValue = tip }));
                                            SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");
                                        }
                                        else
                                        {
                                            Game1.chatBox.addInfoMessage($"   {__instance.displayName}: " + reply);
                                            messageToSend = new MyMessage($"   {__instance.displayName}: " + reply);
                                            SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");
                                        }
                                    }
                                }
                                else
                                {
                                    Game1.chatBox.addInfoMessage(SHelper.Translation.Get("foodstore.sold", new { foodObjName = itemName, locationint = __instance.currentLocation.DisplayName, saleint = salePrice }));
                                    if (!Config.DisableChat)
                                    {
                                        if (tip != 0)
                                            Game1.chatBox.addInfoMessage($"   {__instance.displayName}: " + reply + SHelper.Translation.Get("foodstore.tip", new { tipValue = tip }));
                                        else
                                            Game1.chatBox.addInfoMessage($"   {__instance.displayName}: " + reply);
                                    }
                                }
                            }           //Food outside farmhouse

                            else if (__instance.currentLocation is FarmHouse || __instance.currentLocation is Farm)
                            {
                                if (__instance.currentLocation is FarmHouse)
                                {
                                    Farmer owner = (__instance.currentLocation as FarmHouse).owner;

                                    try
                                    {
                                        if (__instance != null && __instance.Name != null)
                                        {
                                            string[] parts = __instance.Name.Split('_');
                                            string realName = "";
                                            if (parts.Length >= 2)
                                            {
                                                realName = parts[1];
                                            }

                                            NPC realNPC = Game1.getCharacterFromName(realName);

                                            if (owner.friendshipData.ContainsKey(realName))
                                            {
                                                int points = 3;
                                                switch (taste)
                                                {
                                                    case 0:
                                                        points = 8;
                                                        break;
                                                    case 2:
                                                        points = 5;
                                                        break;
                                                    case 4:
                                                        points = 0;
                                                        break;
                                                    case 6:
                                                        points = -3;
                                                        break;
                                                    case 8:
                                                        points = 3;
                                                        break;
                                                    default:
                                                        __instance.doEmote(20);
                                                        break;
                                                }
                                                owner.friendshipData[realName].Points += (int)points;
                                            }
                                        }
                                    }
                                    catch { }

                                    if (owner.friendshipData.ContainsKey(__instance.Name))
                                    {
                                        int points = 5;
                                        switch (taste)
                                        {
                                            case 0:
                                                points = 15;
                                                break;
                                            case 2:
                                                points = 10;
                                                break;
                                            case 4:
                                                points = 0;
                                                break;
                                            case 6:
                                                points = -5;
                                                break;
                                            case 8:
                                                points = 5;
                                                break;
                                            default:
                                                __instance.doEmote(20);
                                                break;
                                        }
                                        owner.friendshipData[__instance.Name].Points += (int)points;
                                    }
                                }

                                Random random = new Random();
                                int randomNumber = random.Next(12);
                                salePrice = tip = 0;
                                if (!Config.DisableChatAll && food.foodObject.Edibility > 0) NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.visitoreat." + randomNumber));
                                else if (!Config.DisableChatAll && food.foodObject.Edibility <= 0) NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.visitorpickup." + randomNumber));
                            }           //Food in farmhouse



                            TodaySell += SHelper.Translation.Get("foodstore.sold", new { foodObjName = itemName, locationint = __instance.currentLocation.DisplayName, saleint = salePrice }) + "^";
                            TodayMoney += salePrice + tip;

                            UpdateCount(food.foodObject.Category);

                            AddToPlayerFunds(salePrice + tip);


                            __instance.modData["hapyke.FoodStore/LastFood"] = Game1.timeOfDay.ToString();
                            if (food.foodObject.Category == -7)
                            {
                                __instance.modData["hapyke.FoodStore/LastFoodTaste"] = taste.ToString();
                            }
                            else if ( food.foodObject.Quality >= 1)
                            {
                                switch (food.foodObject.Quality)
                                {
                                    case 1:
                                        __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "8";
                                        break;
                                    case 2:
                                        __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "2";
                                        break;
                                    case 4:
                                        __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "0";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "-1";
                            }
                            __instance.modData["hapyke.FoodStore/LastFoodDecor"] = decorPoint.ToString();
                            __instance.modData["hapyke.FoodStore/gettingFood"] = "false";

                            return true;
                        }
                    }
                }
                else if (food != null && __instance.IsVillager && food.obj != null && Vector2.Distance(food.foodTile, __instance.Tile) < Config.MaxDistanceToEat && !__instance.Name.EndsWith("_DA") && bool.Parse(__instance.modData["hapyke.FoodStore/gettingFood"]))
                {
                    using (IEnumerator<Object> enumerator = __instance.currentLocation.Objects.Values.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Object currentObject = enumerator.Current;
                            Random rand = new Random();
                            int salePrice = 0;

                            if (enumerator.Current.boundingBox.Value != food.obj.boundingBox.Value)
                                continue;

                            if (currentObject is Mannequin man)
                            {
                                if (man.hat.Value != null)
                                {
                                    salePrice += rand.Next(1100, 1600);
                                    man.hat.Value = null;
                                }
                                if (man.shirt.Value != null)
                                {
                                    salePrice += rand.Next(1300, 1800);
                                    man.shirt.Value = null;
                                }
                                if (man.pants.Value != null)
                                {
                                    salePrice += rand.Next(1400, 1900);
                                    man.pants.Value = null;
                                }
                                if (man.boots.Value != null)
                                {
                                    salePrice += (int)(man.boots.Value.salePrice() * 4);
                                    man.boots.Value = null;
                                }
                            }

                            if (!Config.DisableChatAll && !Config.DisableChat) Game1.chatBox.addInfoMessage(SHelper.Translation.Get("foodstore.soldclothes", new { locationint = __instance.currentLocation.DisplayName, saleint = salePrice }));
                            MyMessage messageToSend = new MyMessage(SHelper.Translation.Get("foodstore.soldclothes", new { locationint = __instance.currentLocation.DisplayName, saleint = salePrice }));
                            SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");

                            if (!Config.DisableChatAll) NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.soldclothesText." + rand.Next(7).ToString()));
          
                            AddToPlayerFunds(salePrice);

                            __instance.modData["hapyke.FoodStore/LastFood"] = Game1.timeOfDay.ToString();
                            __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "-1";
                            __instance.modData["hapyke.FoodStore/gettingFood"] = "false";

                            return true;

                        }
                    }
                }
            }
            catch { }

            __instance.modData["hapyke.FoodStore/gettingFood"] = "false";
            return false;
        }

        private static DataPlacedFood GetClosestFood(NPC npc, GameLocation location)
        {
            List<int> categoryKeys = new List<int> { -81, -80, -79, -75, -74, -28, -27, -26, -23, -22, -21, -20, -19, -18, -17, -16, -15, -12, -8, -7, -6, -5, -4, -2};

            foreach (var pair in validBuildingObjectPairs)
            {
                Building building = pair.Building;
                string buildingType = pair.buildingType;

                var museumCheck = Game1.getLocationFromName(building.GetIndoorsName());

                if (museumCheck == location && buildingType == "museum") return null ;
            }

            List<DataPlacedFood> foodList = new List<DataPlacedFood>();
            foodList.Clear();
            // **************************************************************change way of check to validBuildingObjectPairs
            bool buildingIsFarm = false;
            bool buildingIsMuseum = false;
            bool buildingIsMarket = false;
            bool buildingIsRestaurant = false;

            foreach (var building in Game1.getFarm().buildings)
            {
                if (building != null && building.GetIndoorsName() != null && building.GetIndoorsName().Contains(location.NameOrUniqueName)) buildingIsFarm = true;
            }

            if (buildingIsFarm)
            {
                foreach (var obj in location.Objects.Values)
                {
                    buildingIsMuseum = true;
                    if (obj != null && obj is Sign sign && sign != null && sign.displayItem != null && sign.displayItem.Value != null && sign.displayItem.Value.Name != null
                        && sign.displayItem.Value.Name == "Museum License") break;
                    else buildingIsMuseum = false;
                }

                foreach (var obj in location.Objects.Values)            // Case Market or Restaurant
                {
                    if (obj != null && obj is Sign sign && sign != null && sign.displayItem != null && sign.displayItem.Value != null && sign.displayItem.Value.Name != null)
                    {
                        if (sign.displayItem.Value.Name == "Restaurant License") buildingIsRestaurant = true;
                        else if (sign.displayItem.Value.Name == "Market License") buildingIsMarket = true;
                    }
                }
            }

            if (buildingIsMuseum) return null;

            foreach (var x in location.Objects)                 // Check valid Mannequin
            {
                foreach (var obj in x.Values)
                {
                    if (obj != null && obj is Mannequin mannequin)
                    {
                        bool hasHat = mannequin.hat.Value != null;
                        bool hasShirt = mannequin.shirt.Value != null;
                        bool hasPants = mannequin.pants.Value != null;
                        bool hasBoots = mannequin.boots.Value != null;

                        int xLocation = (obj.boundingBox.X / 64) + (obj.boundingBox.Width / 64 / 2);
                        int yLocation = (obj.boundingBox.Y / 64) + (obj.boundingBox.Height / 64 / 2);
                        var fLocation = new Vector2(xLocation, yLocation);

                        bool hasSignInRange = x.Values.Any(otherObj => otherObj is Sign sign && Vector2.Distance(fLocation, sign.TileLocation) <= Config.SignRange);

                        if (Config.SignRange == 0 || buildingIsFarm) hasSignInRange = true;

                        // Add to foodList only if there is sign within the range
                        if (hasSignInRange && Vector2.Distance(fLocation, npc.Tile) < Config.MaxDistanceToFind && (hasHat || hasPants || hasShirt || hasBoots))
                        {
                            foodList.Add(new DataPlacedFood(obj, fLocation, obj, -1));
                        }
                    }
                }
            }

            foreach (var f in location.furniture)
            {
                if (f.heldObject.Value != null
                    && (categoryKeys.Contains(f.heldObject.Value.Category)
                    // || (f.heldObject.Value is WeaponProxy && Config.EnableSaleWeapon))
                    ))         // ***** Validate category items *****
                {
                    int xLocation = (f.boundingBox.X / 64) + (f.boundingBox.Width / 64 / 2);
                    int yLocation = (f.boundingBox.Y / 64) + (f.boundingBox.Height / 64 / 2);
                    var fLocation = new Vector2(xLocation, yLocation);

                    bool hasSignInRange = location.Objects.Values.Any(obj => obj is Sign sign && Vector2.Distance(fLocation, sign.TileLocation) <= Config.SignRange);

                    if ((Config.SignRange == 0 && !buildingIsFarm)
                          || (buildingIsFarm && buildingIsMarket)
                          || (buildingIsFarm && f.heldObject.Value.Category == -7 && buildingIsRestaurant)) hasSignInRange = true;

                    // Add to foodList only if there is sign within the range
                    if (hasSignInRange && Vector2.Distance(fLocation, npc.Tile) < Config.MaxDistanceToFind)
                    {
                        foodList.Add(new DataPlacedFood(f, fLocation, f.heldObject.Value, -1));
                    }
                }

                if (f.heldObject.Value != null && f.heldObject.Value is Chest chest) 
                {
                    if (chest.Items.Any(item => item != null))
                    {
                        int xLocation = (f.boundingBox.X / 64) + (f.boundingBox.Width / 64 / 2);
                        int yLocation = (f.boundingBox.Y / 64) + (f.boundingBox.Height / 64 / 2);
                        var fLocation = new Vector2(xLocation, yLocation);

                        bool hasSignInRange = location.Objects.Values.Any(obj => obj is Sign sign && Vector2.Distance(fLocation, sign.TileLocation) <= Config.SignRange);

                        if ((Config.SignRange == 0 && !buildingIsFarm)
                              || (buildingIsFarm && buildingIsMarket)
                              || (buildingIsFarm && buildingIsRestaurant)) hasSignInRange = true;
                        // ******************************************************************************************* fix sell item at framwork table outdoor
                        
                        Game1.chatBox.addInfoMessage(hasSignInRange.ToString());

                        if (buildingIsRestaurant && hasSignInRange && Vector2.Distance(fLocation, npc.Tile) < Config.MaxDistanceToFind )
                        { 
                            var item = chest.Items.FirstOrDefault(item => item != null && item.Category == -7);
                            if ( item != null) { foodList.Add(new DataPlacedFood(f, fLocation, new Object(item.ItemId, 1), chest.Items.IndexOf(item))); }
                        }
                        else if (buildingIsMarket && hasSignInRange && Vector2.Distance(fLocation, npc.Tile) < Config.MaxDistanceToFind)
                        {
                            var item = chest.Items.FirstOrDefault(item => item != null);
                            if (item != null) { foodList.Add(new DataPlacedFood(f, fLocation, new Object(item.ItemId, 1), chest.Items.IndexOf(item))); }
                        }
                        //else if (hasSignInRange && Vector2.Distance(fLocation, npc.Tile) < Config.MaxDistanceToFind)
                        //{
                        //    var item = chest.Items.FirstOrDefault(item => item != null);
                        //    if (item != null) { foodList.Add(new DataPlacedFood(f, fLocation, new Object(item.ItemId, 1), chest.Items.IndexOf(item))); }
                        //}
                    }
                }
            }
            if (foodList.Count == 0 || buildingIsFarm && buildingIsMuseum)
            {
                //SMonitor.Log("Got no food");
                return null;
            }

            for (int i = foodList.Count - 1; i >= 0; i--)
            {
                foodList[i].value = 0;
            }

            foodList.Sort(delegate (DataPlacedFood a, DataPlacedFood b)
            {
                var compare = b.value.CompareTo(a.value);
                if (compare != 0)
                    return compare;
                return Vector2.Distance(a.foodTile, npc.Tile).CompareTo(Vector2.Distance(b.foodTile, npc.Tile));
            });

            if (!Config.RandomPurchase)         //Return the closest
            {
                return foodList[0];
            }
            else                                //Return a random item
            {
                Random random = new Random();
                return foodList[random.Next(foodList.Count)];
            }
        }


        // unused for weapon
        /*
        private static T XmlDeserialize<T>(int toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (intReader textReader = new intReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        private static int XmlSerialize<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (intWriter textWriter = new intWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        private void makePlaceholderObjects()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Furniture furniture in location.furniture)
                {
                    if (furniture.heldObject.Value is WeaponProxy weaponProxy)
                    {
                        StardewValley.Object placeholder = new StardewValley.Object(furniture.heldObject.Value.TileLocation, "(O)0");
                        placeholder.Name = $"WeaponProxy:{XmlSerialize(weaponProxy.Weapon)}";
                        furniture.heldObject.Set(placeholder);
                    }
                }
            }

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.indoors.Value != null && building.indoors.Value.furniture != null)
                {
                    foreach (Furniture furniture in building.indoors.Value.furniture)
                    {
                        if (furniture.heldObject.Value is WeaponProxy weaponProxy)
                        {
                            StardewValley.Object placeholder = new StardewValley.Object(furniture.heldObject.Value.TileLocation, "(O)0");
                            placeholder.Name = $"WeaponProxy:{XmlSerialize(weaponProxy.Weapon)}";
                            furniture.heldObject.Set(placeholder);
                        }
                    }
                }
            }
        }

        private void restoreMeleeWeapon(Furniture furniture, int xmlint)
        {
            MeleeWeapon weapon = XmlDeserialize<MeleeWeapon>(xmlint);
            furniture.heldObject.Set(new WeaponProxy(weapon));
        }

        private void restorePlaceholderObjects()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Furniture furniture in location.furniture)
                {
                    if (furniture.heldObject.Value != null && furniture.heldObject.Value.Name.Contains("Proxy:"))
                    {
                        int xmlint = furniture.heldObject.Value.Name;
                        if (xmlint.StartsWith("WeaponProxy:"))
                        {
                            try
                            {
                                restoreMeleeWeapon(furniture, xmlint.Subint("WeaponProxy:".Length));
                            }
                            catch { }
                        }
                    }
                }
            }

            foreach (Building building in Game1.getFarm().buildings)
            {
                try
                {
                    if (building.indoors != null && building.indoors.Value != null && building.indoors.Value.furniture != null)
                    {
                        foreach (Furniture furniture in building.indoors.Value.furniture)
                        {
                            if (furniture.heldObject.Value != null && furniture.heldObject.Value.Name.Contains("Proxy:"))
                            {
                                int xmlint = furniture.heldObject.Value.Name;
                                if (xmlint.StartsWith("WeaponProxy:"))
                                {
                                    try
                                    {
                                        restoreMeleeWeapon(furniture, xmlint.Subint("WeaponProxy:".Length));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                } catch { }
            }
        }
        */
    }
}