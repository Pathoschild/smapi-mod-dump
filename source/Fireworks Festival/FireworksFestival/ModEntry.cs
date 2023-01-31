/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/FireworksFestival
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace FireworksFestival
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        // The DGA API
        private static IDynamicGameAssetsApi DGA_API;

        // Storing whether or not free gift has been received
        private static bool hasReceivedFreeGift;

        // Monitor
        private static IMonitor monitorStatic;

        // Helper
        private static IModHelper helperStatic;

        /// <summary>Game1.multiplayer from reflection.</summary>
        private static Multiplayer multiplayer;

        // Shop stocks
        private static Dictionary<ISalable, int[]> clothingShopStock;
        private static Dictionary<ISalable, int[]> blueBoatStock;
        private static Dictionary<ISalable, int[]> purpleBoatStock;
        private static Dictionary<ISalable, int[]> brownBoatStock;

        // Explosions state of being
        private static bool isExploding;

        // Explosions state of color
        private static Color explodeColor = Color.White;

        // List of fireworks
        private static Dictionary<String,Dictionary<Vector2,Color>> fireworkLocs = new Dictionary<String, Dictionary<Vector2, Color>>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // Add in festival tile stuff for shaved ice
            harmony.Patch(
               original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogue)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.answerDialogue_Postfix))
            );

            // Fix fishing minigame player position
            harmony.Patch(
               original: AccessTools.Method(typeof(FishingGame), nameof(FishingGame.gameDoneAfterFade)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.gameDoneAfterFade_Postfix))
            );

            // Fix fishing minigame player position
            harmony.Patch(
               original: AccessTools.Method(typeof(FishingGame), nameof(FishingGame.startMe)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.startMe_Postfix))
            );

            // Cause explosion when fireworks placed
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PlacementAction_Prefix))
            );

            // Color the explosions in a location
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.explode_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.explode_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.broadcastSprites), new Type[] {
                typeof(GameLocation), typeof(List<TemporaryAnimatedSprite>)}),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.broadcastSprites_Prefix))
            );

            var Game1_multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer = Game1_multiplayer;

            monitorStatic = Monitor;
            helperStatic = helper;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            DGA_API = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            if (DGA_API == null)
            {
                Monitor.Log("Could not get DGA API, mod will not work", LogLevel.Error);
                return;
            }

            DGA_API.AddEmbeddedPack(this.ModManifest, Path.Combine(Helper.DirectoryPath, "assets", "dga"));
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            hasReceivedFreeGift = false;
            clothingShopStock = getClothingShopStock();
            blueBoatStock = getBlueBoatStock();
            purpleBoatStock = getPurpleBoatStock();
            brownBoatStock = getBrownBoatStock();

            if (Game1.currentSeason == "summer" && Game1.dayOfMonth == 18)
            {
                Monitor.Log("Adding festival mail", LogLevel.Trace);
                Game1.addMailForTomorrow("vl.FireworksFestival");
            }

            fireworkLocs.Clear();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.CurrentEvent == null)
            {
                return;
            }
            Monitor.Log($"{Game1.CurrentEvent.FestivalName}", LogLevel.Trace);
            if (!Game1.CurrentEvent.isSpecificFestival("summer20"))
            {
                //Monitor.Log("Not my ants!", LogLevel.Debug);
                return;
            }
            if (e.Button.IsActionButton())
            {
                // Submarine warp
                if (e.Cursor.GrabTile.X == 5 && e.Cursor.GrabTile.Y == 34)
                {
                    Response[] responses2 = new Response[2]
                    {
                        new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1662")),
                        new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1663"))
                    };
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1681"), responses2, "fishingGame");
                    return;
                }

                // Free shaved ice
                else if ((e.Cursor.GrabTile.X == 13 || e.Cursor.GrabTile.X == 14) && e.Cursor.GrabTile.Y == 37)
                {
                    if (!hasReceivedFreeGift)
                    {
                        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverQuestion"), Game1.currentLocation.createYesNoResponses(), "GiftGiverQuestion");
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
                    }
                }

                // Traveling merchant
                else if (e.Cursor.GrabTile.X == 39 && e.Cursor.GrabTile.Y == 30)
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getTravelingMerchantStock((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), 0, "TravelerSummerNightMarket", Utility.onTravelingMerchantShopPurchase);
                }

                // Fried foods shop
                else if (e.Cursor.GrabTile.X == 19 && e.Cursor.GrabTile.Y == 33)
                {
                    Game1.activeClickableMenu = new ShopMenu(blueBoatStock);
                }

                // Fireworks shop
                else if (e.Cursor.GrabTile.X == 25 && e.Cursor.GrabTile.Y == 39)
                {
                    ShopMenu purpleShop = new ShopMenu(purpleBoatStock, 0, null, ModEntry.postFireworkBuy, null, "STF.violetlizabet.FireworkShop");
                    Game1.activeClickableMenu = purpleShop;
                }

                // Fruits shop
                else if ((e.Cursor.GrabTile.X == 47 || e.Cursor.GrabTile.X == 48) && e.Cursor.GrabTile.Y == 34)
                {
                    Game1.activeClickableMenu = new ShopMenu(brownBoatStock);
                }

                // Yukata shop
                else if ((e.Cursor.GrabTile.X == 34 || e.Cursor.GrabTile.X == 35) && e.Cursor.GrabTile.Y == 15)
                {
                    Game1.activeClickableMenu = new ShopMenu(clothingShopStock);
                }
            }            
        }

        // Trigger explosion properly when placed
        private static bool PlacementAction_Prefix(StardewValley.Object __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            // Not our item, we don't care
            if (DGA_API.GetDGAItemId(__instance) == null || !__instance.Name.Contains("Firework", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                Color color = Color.White;
                if (__instance.Name.Contains("RedFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Red;
                }
                else if(__instance.Name.Contains("OrangeFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Orange;
                }
                else if (__instance.Name.Contains("YellowFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Yellow;
                }
                else if (__instance.Name.Contains("GreenFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Green;
                }
                else if (__instance.Name.Contains("BlueFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Blue;
                }
                else if (__instance.Name.Contains("PurpleFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Purple;
                }
                bool success = DoFireworkExplosionAnimation(location, x, y, who, color);
                if (success)
                {
                    __result = true;
                }
                return false;
            }
        }

        private static void answerDialogue_Postfix(string questionKey, int answerChoice)
        {
            if (questionKey == null)
            {
                return;
            }
            if (questionKey.Equals("GiftGiverQuestion",StringComparison.OrdinalIgnoreCase))
            {
                switch (answerChoice)
                {
                    case 0:
                        if (hasReceivedFreeGift)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
                        }
                        else
                        {
                            Game1.player.freezePause = 1000;
                            Game1.soundBank.PlayCue("snowyStep");
                            Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/ShavedIce"));
                            Game1.player.modData["violetlizabet.FireworksFestival"] = "true";
                            hasReceivedFreeGift = true;
                        } 
                        break;
                    case 1:
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
                        break;
                }
            }
        }

        private static void startMe_Postfix()
        {
            if (Game1.currentSeason.Equals("summer",StringComparison.OrdinalIgnoreCase) && Game1.dayOfMonth == 20)
            {
                Game1.player.Position = new Vector2(14f, 15f) * 64f;
                //monitor.Log($"Player is in {Game1.currentLocation.Name}");
            }
        }

        private static void gameDoneAfterFade_Postfix()
        {
            if (Game1.currentSeason.Equals("summer", StringComparison.OrdinalIgnoreCase) && Game1.dayOfMonth == 20)
            {
                Game1.player.Position = new Vector2(5f, 36f) * 64f;
                //monitor.Log($"Player is in {Game1.currentLocation.Name}");
            }
        }

        public static bool postFireworkBuy(ISalable item, Farmer farmer, int amount)
        {
            monitorStatic.Log($"Post buy {item.Name}", LogLevel.Trace);
            if (item.Name.Equals("FireworksLicense") && !Game1.player.mailReceived.Contains("vl.fireworkslicense"))
            {
                Game1.player.mailReceived.Add("vl.fireworkslicense");
            }
            return false;
        }

        private Dictionary<ISalable, int[]> getClothingShopStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add(new StardewValley.Objects.Clothing(1226), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1270), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1193), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1016), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1043), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1212), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1081), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1085), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1144), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(10), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(11), new int[2] { 2000, 1 });
            stock.Add(new StardewValley.Objects.Clothing(12), new int[2] { 2000, 1 });
            return stock;
        }

        private Dictionary<ISalable, int[]> getBlueBoatStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/Takoyaki"), new int[2] { 500, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/Yakisoba"), new int[2] { 500, int.MaxValue });
            stock.Add(new StardewValley.Object(202, 1), new int[2] { 1500, 1 });
            stock.Add(new StardewValley.Object(214, 1), new int[2] { 1500, 1 });
            stock.Add(new StardewValley.Object(205, 1), new int[2] { 1500, 1 });
            return stock;
        }

        private Dictionary<ISalable, int[]> getPurpleBoatStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/RedFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/OrangeFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/YellowFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/GreenFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/BlueFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/PurpleFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/WhiteFirework"), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem("violetlizabet.FireworksFestival/FireworksLicense"), new int[2] { 50000, 1 });
            return stock;
        }

        private Dictionary<ISalable, int[]> getBrownBoatStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add(new StardewValley.Object(254, 1), new int[2] { 1000, int.MaxValue });
            stock.Add(new StardewValley.Object(400, 1), new int[2] { 1000, int.MaxValue });
            stock.Add(new StardewValley.Object(398, 1), new int[2] { 1000, int.MaxValue });
            stock.Add(new StardewValley.Object(636, 1), new int[2] { 5000, 1 });
            stock.Add(new StardewValley.Object(268, 1), new int[2] { 5000, 1 });
            return stock;
        }

        // Generate explosion animation when placed
        private static bool DoFireworkExplosionAnimation(GameLocation location, int x, int y, Farmer who, Color color)
        {
            Vector2 placementTile = new Vector2(x / 64, y / 64);
            foreach (TemporaryAnimatedSprite temporarySprite2 in location.temporarySprites)
            {
                if (temporarySprite2.position.Equals(placementTile * 64f))
                {
                    return false;
                }
            }
            int idNum = Game1.random.Next();
            location.playSound("thudStep");
            TemporaryAnimatedSprite pearlTAS = new TemporaryAnimatedSprite(1720, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
            {
                bombRadius = 3,
                bombDamage = 1,
                shakeIntensity = 0.5f,
                shakeIntensityChange = 0.002f,
                extraInfoForEndBehavior = idNum,
                endFunction = location.removeTemporarySpritesWithID
            };
            // White firework is default
            Rectangle fireworkRect = new Rectangle(0,0,16,16);
            String fireworkName = "WhiteFirework.png";
            if (color.Equals(Color.Red))
            {
                monitorStatic.Log("Setting source to red firework", LogLevel.Trace);
                fireworkName = "RedFirework.png";
            }
            else if (color.Equals(Color.Orange))
            {
                monitorStatic.Log("Setting source to orange firework", LogLevel.Trace);
                fireworkName = "OrangeFirework.png";
            }
            else if (color.Equals(Color.Yellow))
            {
                monitorStatic.Log("Setting source to yellow firework", LogLevel.Trace);
                fireworkName = "YellowFirework.png";
            }
            else if (color.Equals(Color.Green))
            {
                monitorStatic.Log("Setting source to green firework", LogLevel.Trace);
                fireworkName = "GreenFirework.png";
            }
            else if (color.Equals(Color.Blue))
            {
                monitorStatic.Log("Setting source to blue firework", LogLevel.Trace);
                fireworkName = "BlueFirework.png";
            }
            else if (color.Equals(Color.Purple))
            {
                monitorStatic.Log("Setting source to purple firework", LogLevel.Trace);
                fireworkName = "PurpleFirework.png";
            }
            pearlTAS.texture = helperStatic.ModContent.Load<Texture2D>("assets/fireworks/" + fireworkName);
            pearlTAS.sourceRect = fireworkRect;
            pearlTAS.scale = 4f;
            multiplayer.broadcastSprites(location, pearlTAS);
            location.netAudio.StartPlaying("fuse");

            // Log the firework for later
            if (!fireworkLocs.ContainsKey(location.Name))
            {
                fireworkLocs.Add(location.Name, new Dictionary<Vector2, Color>());
                fireworkLocs[location.Name].Add(placementTile, color);
            }
            else
            {
                fireworkLocs[location.Name].Add(placementTile, color);
            }
            return true;
        }

        private static void broadcastSprites_Prefix(GameLocation location, List<TemporaryAnimatedSprite> sprites)
        {
            if (isExploding)
            {
                monitorStatic.Log($"Changing sprites to {explodeColor}", LogLevel.Trace);
                foreach (TemporaryAnimatedSprite spr in sprites)
                {
                    spr.color = explodeColor;
                }
            }
        }

        private static void explode_Prefix(GameLocation __instance, Vector2 tileLocation)
        {
            isExploding = true;
            monitorStatic.Log("Prefix on GameLocation.Explode hit", LogLevel.Trace);
            if (fireworkLocs.TryGetValue(__instance.Name, out Dictionary<Vector2,Color> localDict))
            {
                if (localDict.TryGetValue(tileLocation, out Color thisCol ))
                {
                    explodeColor = thisCol;
                    monitorStatic.Log($"Setting explosion color to {explodeColor}", LogLevel.Trace);
                }
            }
        }

        private static void explode_Postfix(GameLocation __instance, Vector2 tileLocation)
        {
            isExploding = false;
            monitorStatic.Log("Postfix on GameLocation.Explode hit",LogLevel.Trace);
            if (fireworkLocs.TryGetValue(__instance.Name, out Dictionary<Vector2, Color> localDict))
            {
                monitorStatic.Log("Removing firework location from dictionary", LogLevel.Trace);
                localDict.Remove(tileLocation);
            }
            explodeColor = Color.White;
        }
    }
}