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
using System.Globalization;

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

        // Useful strings
        private static string contentPackModID = "violetlizabet.DGA.FireworksFestival";
        private static string fireworkTexLoc = "Mods/FireworksFestival/Fireworks";
        private static string burstTexLoc = "Mods/FireworksFestival/FireworkBurst";
        private static string fireworkTexLocInGame = "Mods\\FireworksFestival\\Fireworks";
        private static string burstTexLocInGame = "Mods\\FireworksFestival\\FireworkBurst";
        private static string isExplodingString = "violetlizabet.FireworksFestival/isExploding";
        private static string explodeColorString = "violetlizabet.FireworksFestival/explodeColor";
        private static string fishingGameString = "violetlizabet.FireworksFestival/fishingGame";
        private static string thisModID;
        private static string festivalLetter = "vl.FireworksFestival";
        private static string licenseLetter = "vl.fireworkslicense";
        private static string msgTypeRemove = "fireworkRemovalMessage";
        private static string msgTypeAdd = "fireworkAddMessage";
        private static string chemizerRecipeName = "FireworksFestivalChemizer";
        private static string blackPowderRecipeName = "FireworksFestivalBlackPowder";
        private static string redFireworkRecipeName = "FireworksFestivalRedFirework";
        private static string orangeFireworkRecipeName = "FireworksFestivalOrangeFirework";
        private static string yellowFireworkRecipeName = "FireworksFestivalYellowFirework";
        private static string greenFireworkRecipeName = "FireworksFestivalGreenFirework";
        private static string blueFireworkRecipeName = "FireworksFestivalBlueFirework";
        private static string purpleFireworkRecipeName = "FireworksFestivalPurpleFirework";
        private static string whiteFireworkRecipeName = "FireworksFestivalWhiteFirework";

        // DGA item names
        private static string redFWName = contentPackModID  + "/RedFirework";
        private static string orangeFWName = contentPackModID + "/OrangeFirework";
        private static string yellowFWName = contentPackModID + "/YellowFirework";
        private static string greenFWName = contentPackModID + "/GreenFirework";
        private static string blueFWName = contentPackModID + "/BlueFirework";
        private static string purpleFWName = contentPackModID + "/PurpleFirework";
        private static string whiteFWName = contentPackModID + "/WhiteFirework";
        private static string fireworksLicenseName = contentPackModID + "/FireworksLicense";
        private static string takoyakiName = contentPackModID + "/Takoyaki";
        private static string yakisobaName = contentPackModID + "/Yakisoba";

        // Carp index
        private static int carpIndex = 142;

        // List of fireworks
        private static Dictionary<String,Dictionary<Vector2,Color>> fireworkLocs = new Dictionary<String, Dictionary<Vector2, Color>>();

        // Check if summer
        private static bool isSummer => Game1.currentSeason.Equals("summer", StringComparison.OrdinalIgnoreCase);

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
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;

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

            // Fix fishing minigame fish catch
            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.caughtFish)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.caughtFish_Postfix))
            );

            // Fix fishing minigame perfect fish catch
            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.perfectFishing)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.perfectFishing_Postfix))
            );

            // Change fishing minigame prize
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingGame), nameof(FishingGame.tick)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FishingGame_Tick_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingGame), nameof(FishingGame.unload)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FishingGame_Unload_Postfix))
            );

            // Add specific temporary sprite
            harmony.Patch(
                original: AccessTools.Method(typeof(Event), "addSpecificTemporarySprite"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.specificTemporarySprite_Postfix))
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
            thisModID = this.ModManifest.UniqueID;
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

        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            hasReceivedFreeGift = false;
            clothingShopStock = getClothingShopStock();
            blueBoatStock = getBlueBoatStock();
            purpleBoatStock = getPurpleBoatStock();
            brownBoatStock = getBrownBoatStock();

            if (isSummer && Game1.dayOfMonth == 19)
            {
                Monitor.Log("Adding festival mail", LogLevel.Trace);

                Game1.player.mailReceived.Remove(festivalLetter);
                Game1.addMail(festivalLetter);
            }

            fireworkLocs.Clear();

            // Forcibly add some recipes if needed
            if (Game1.player.miningLevel.Value >= 6)
            {
                if (!Game1.player.craftingRecipes.ContainsKey(chemizerRecipeName))
                {
                    Monitor.Log("Adding chemizer recipe directly", LogLevel.Trace);
                    Game1.player.craftingRecipes.Add(chemizerRecipeName, 0);
                }
                if (!Game1.player.craftingRecipes.ContainsKey(blackPowderRecipeName))
                {
                    Monitor.Log("Adding black powder recipe directly", LogLevel.Trace);
                    Game1.player.craftingRecipes.Add(blackPowderRecipeName, 0);
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.CurrentEvent == null)
            {
                return;
            }
            if (!Game1.CurrentEvent.isSpecificFestival("summer20"))
            {
                return;
            }
            // Exit if something is in progress/player shouldn't be able to interact with things
            if (Game1.activeClickableMenu != null)
            {
                return;
            }
            if (!Context.CanPlayerMove)
            {
                return;
            }

            if (e.Button.IsActionButton())
            {
                // Submarine warp
                if (e.Cursor.GrabTile.X == 5 && e.Cursor.GrabTile.Y == 34)
                {
                    suppressClick();
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
                    suppressClick();
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
                    suppressClick();
                    Game1.activeClickableMenu = new ShopMenu(Utility.getTravelingMerchantStock((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), 0, "TravelerSummerNightMarket", Utility.onTravelingMerchantShopPurchase);
                }

                // Fried foods shop
                else if (e.Cursor.GrabTile.X == 19 && e.Cursor.GrabTile.Y == 33)
                {
                    suppressClick();
                    Game1.activeClickableMenu = new ShopMenu(blueBoatStock);
                }

                // Fireworks shop
                else if (e.Cursor.GrabTile.X == 25 && e.Cursor.GrabTile.Y == 39)
                {
                    suppressClick();
                    ShopMenu purpleShop = new ShopMenu(purpleBoatStock, 0, "Birdie", ModEntry.postFireworkBuy, null, "STF.violetlizabet.Fireworks");
                    purpleShop.portraitPerson = new NPC(new AnimatedSprite("Characters\\Birdie"), new Vector2(0,0), 1, "Birdie");
                    string dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:vlFireworks.Birdie");
                    purpleShop.potraitPersonDialogue = Game1.parseText(dialogue, Game1.dialogueFont, 304);
                    Game1.activeClickableMenu = purpleShop;
                }

                // Fruits shop
                else if ((e.Cursor.GrabTile.X == 47 || e.Cursor.GrabTile.X == 48) && e.Cursor.GrabTile.Y == 34)
                {
                    suppressClick();
                    Game1.activeClickableMenu = new ShopMenu(brownBoatStock);
                }

                // Yukata shop
                else if ((e.Cursor.GrabTile.X == 34 || e.Cursor.GrabTile.X == 35) && e.Cursor.GrabTile.Y == 15)
                {
                    suppressClick();
                    ShopMenu clothesShop = new ShopMenu(clothingShopStock, 0, "FireworksFox", null, null, "STF.violetlizabet.FireworkClothing");
                    clothesShop.portraitPerson = new NPC(new AnimatedSprite("Characters\\Birdie"), new Vector2(0, 0), 1, "FireworksFox");
                    string dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:vlFireworks.Fox");
                    clothesShop.potraitPersonDialogue = Game1.parseText(dialogue, Game1.dialogueFont, 304);
                    Game1.activeClickableMenu = clothesShop;
                }
            }            
        }

        // Load the TAS textures
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(fireworkTexLoc))
            {
                e.LoadFromModFile<Texture2D>("assets/Fireworks.png", AssetLoadPriority.Medium);
            }
            if (e.NameWithoutLocale.IsEquivalentTo(burstTexLoc))
            {
                e.LoadFromModFile<Texture2D>("assets/FireworkBurst.png", AssetLoadPriority.Medium);
            }
        }

        // Remove the TAS location
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == thisModID)
            {
                if (e.Type == msgTypeRemove)
                {
                    (string, Vector2) message = e.ReadAs<(string, Vector2)>();
                    if (fireworkLocs.TryGetValue(message.Item1, out Dictionary<Vector2, Color> localDict))
                    {
                        monitorStatic.Log("Removing firework from local dictionary", LogLevel.Trace);
                        localDict.Remove(message.Item2);
                    }
                }
                else if (e.Type == msgTypeAdd)
                {
                    if (Game1.player.IsMainPlayer)
                    {
                        monitorStatic.Log("Adding farmhand firework to local dictionary", LogLevel.Trace);
                        (string, Vector2, Color) message = e.ReadAs<(string, Vector2, Color)>();
                        if (!fireworkLocs.ContainsKey(message.Item1))
                        {
                            fireworkLocs.Add(message.Item1, new Dictionary<Vector2, Color>());
                            fireworkLocs[message.Item1].Add(message.Item2, message.Item3);
                        }
                        else
                        {
                            fireworkLocs[message.Item1].Add(message.Item2, message.Item3);
                        }
                    }
                }
            }

        }

        // Check for farmhand versions and warn about mismatches
        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (e.Peer.GetMod(thisModID) == null)
            {
                Monitor.Log("Player connected without this mod installed, problems likely to result!", LogLevel.Error);
            }
            else if (e.Peer.GetMod(thisModID).Version.IsOlderThan(this.ModManifest.Version))
            {
                Monitor.Log("Player connected with an older version of this mod installed, problems likely to result!", LogLevel.Error);
            }
        }

        // Trigger explosion properly when placed
        private static bool PlacementAction_Prefix(StardewValley.Object __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            // Not our item, we don't care
            string itemID = DGA_API.GetDGAItemId(__instance);
            if (itemID == null || !(itemID.Contains(contentPackModID, StringComparison.OrdinalIgnoreCase) && itemID.EndsWith("Firework", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            else
            {
                Color color = Color.White;
                if (itemID.Equals(redFWName, StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Red;
                }
                else if(itemID.Equals(orangeFWName, StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Orange;
                }
                else if (itemID.Equals(yellowFWName, StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Yellow;
                }
                else if (itemID.Equals(greenFWName, StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Green;
                }
                else if (itemID.Equals(blueFWName, StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Blue;
                }
                else if (itemID.Equals(purpleFWName, StringComparison.OrdinalIgnoreCase))
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
                            Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item)DGA_API.SpawnDGAItem(contentPackModID + "/ShavedIce"));
                            Game1.player.modData[thisModID] = "true";
                            hasReceivedFreeGift = true;
                        } 
                        break;
                    case 1:
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
                        break;
                }
            }
        }

        private static void specificTemporarySprite_Postfix(string key, GameLocation location, string[] split)
        {
            if (key.Equals("vlFireworkBurst", StringComparison.OrdinalIgnoreCase))
            {
                // Make sure there's enough arguments
                if (split.Length < 4)
                {
                    monitorStatic.Log("Not enough arguments to specificTAS vlFireworkBurst",LogLevel.Warn);
                    return;
                }
                // Check that location is correct
                if (int.TryParse(split[2], out int xLoc) && int.TryParse(split[3], out int yLoc))
                {
                    int type = 0;
                    // Check that sprite details are correct
                    if (split.Length < 5 || !int.TryParse(split[4], out type))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set type arguments, setting to {type}", LogLevel.Warn);
                    }
                    if (type < 0 || type > 6)
                    {
                        type = 0;
                        monitorStatic.Log($"specificTAS vlFireworkBurst set type to invalid number, resetting to {type}", LogLevel.Warn);
                    }
                    Rectangle sourceRect = new Rectangle(0, 48 * type, 64, 48);

                    float animInterval = 100f;
                    if (split.Length < 6 || !float.TryParse(split[5], out animInterval))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation interval, setting to {animInterval}", LogLevel.Warn);
                    }

                    int animLen = 8;
                    if (split.Length < 7 || !int.TryParse(split[6], out animLen))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation length, setting to {animLen}", LogLevel.Warn);
                    }

                    int numLoops = 1;
                    if (split.Length < 8 || !int.TryParse(split[7], out numLoops))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation length, setting to {numLoops}", LogLevel.Trace);
                    }

                    bool flicker = false;
                    if (split.Length < 9 || !bool.TryParse(split[8], out flicker))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set flicker, setting to {flicker}", LogLevel.Trace);
                    }

                    bool flipped = false;
                    if (split.Length < 10 || !bool.TryParse(split[9], out flipped))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set flip, setting to {flipped}", LogLevel.Trace);
                    }

                    float layerDepth = 99f;
                    if (split.Length < 11 || !float.TryParse(split[10], out layerDepth))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set layer depth, setting to {layerDepth}", LogLevel.Trace);
                    }

                    TemporaryAnimatedSprite burst = new TemporaryAnimatedSprite(burstTexLocInGame, sourceRect, animInterval, animLen, numLoops, new Vector2(xLoc * 64, yLoc * 64), flicker, flipped);
                    burst.layerDepth = layerDepth;
                    burst.scale = 4f;
                    location.TemporarySprites.Add(burst);
                    
                }
                else
                {
                    monitorStatic.Log("specificTAS vlFireworkBurst failed in location arguments", LogLevel.Warn);
                }
            }
        }

        // Move player to right starting location
        private static void startMe_Postfix()
        {
            if (isSummer && Game1.dayOfMonth == 20)
            {
                Game1.player.Position = new Vector2(14f, 15f) * 64f;
                Game1.player.festivalScore = 0;
            }
        }

        // Move player to right ending location
        private static void gameDoneAfterFade_Postfix(FishingGame __instance, int ___showResultsTimer)
        {
            if (isSummer && Game1.dayOfMonth == 20)
            {
                Game1.player.Position = new Vector2(5f, 36f) * 64f;
            }
        }

        // Make it actually register the fish caught
        private static void caughtFish_Postfix(Event __instance, int whichFish, int size)
        {
            if (whichFish != -1 && Game1.currentMinigame != null && isSummer && Game1.dayOfMonth == 20)
            {
                (Game1.currentMinigame as FishingGame).score += ((size <= 0) ? 1 : (size + 5));
                if (size > 0)
                {
                    (Game1.currentMinigame as FishingGame).fishCaught++;
                }
                Game1.player.FarmerSprite.PauseForSingleAnimation = false;
                Game1.player.FarmerSprite.StopAnimation();
            }
        }

        // Make it actually register the perfect fish caught
        private static void perfectFishing_Postfix(Event __instance)
        {
            if (__instance.isFestival && Game1.currentMinigame != null && isSummer && Game1.dayOfMonth == 20)
            {
                (Game1.currentMinigame as FishingGame).perfections++;
            }
        }

        // Give 1 carp per 100 star tokens
        private static void FishingGame_Tick_Postfix(FishingGame __instance)
        {
            if (isSummer && Game1.dayOfMonth == 20)
            {
                if (__instance.starTokensWon > 0 && !(Game1.player.modData.TryGetValue(fishingGameString, out string done) && done.Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    monitorStatic.Log($"Currently won {__instance.starTokensWon}", LogLevel.Trace);
                    __instance.starTokensWon = __instance.starTokensWon / 100;
                    Game1.player.festivalScore = __instance.starTokensWon;
                    Game1.player.modData[fishingGameString] = "true";
                    monitorStatic.Log($"Now won {__instance.starTokensWon}", LogLevel.Trace);
                }                
            }
        }

        // Give the carp out
        private static void FishingGame_Unload_Postfix(FishingGame __instance)
        {
            if (isSummer && Game1.dayOfMonth == 20 && __instance.starTokensWon > 0)
            {
                int numCarp = __instance.starTokensWon;
                monitorStatic.Log($"Rewarding with {numCarp} Carp", LogLevel.Trace);
                StardewValley.Object carp = new StardewValley.Object(carpIndex, numCarp);
                Game1.player.addItemToInventory(carp);
                Game1.player.festivalScore = 0;
                Game1.player.modData.Remove(fishingGameString);
            }
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
            location.playSound("thudStep");


            // TAS ID
            int idNum = Game1.random.Next();

            // White firework is default
            Rectangle fireworkRect = getFireworksRect(color);
            //string fireworkName = getFireworksTexture(color);

            monitorStatic.Log($"Creating TAS with source rect {fireworkRect}",LogLevel.Trace);

            TemporaryAnimatedSprite fireworkTAS = new TemporaryAnimatedSprite(0, 100f, 1, 24, placementTile * 64f, flicker: true, flipped: false, location, who)
            {
                bombRadius = 3,
                bombDamage = 1,
                shakeIntensity = 0.5f,
                shakeIntensityChange = 0.002f,
                extraInfoForEndBehavior = idNum,
                endFunction = location.removeTemporarySpritesWithID,
                sourceRect = fireworkRect,
                sourceRectStartingPos = new Vector2(fireworkRect.X,fireworkRect.Y),
                scale = 4f
            };

            // Try forcing the texture name
            helperStatic.Reflection.GetField<string>(fireworkTAS, "textureName").SetValue(fireworkTexLocInGame);
            helperStatic.Reflection.GetMethod(fireworkTAS, "loadTexture").Invoke();

            // Log the firework for later
            monitorStatic.Log("Saving fireworks location to dictionary",LogLevel.Trace);
            if (!fireworkLocs.ContainsKey(location.Name))
            {
                fireworkLocs.Add(location.Name, new Dictionary<Vector2, Color>());
                fireworkLocs[location.Name].Add(placementTile, color);
            }
            else
            {
                fireworkLocs[location.Name].Add(placementTile, color);
            }

            // If player is a farmhand, transmit firework location to host
            if (!Game1.player.IsMainPlayer)
            {
                helperStatic.Multiplayer.SendMessage((location.Name, placementTile, color), msgTypeAdd, modIDs: new[] { thisModID });
            }

            // Send out the TAS
            multiplayer.broadcastSprites(location, fireworkTAS);
            location.netAudio.StartPlaying("fuse");
            return true;
        }

        private static void explode_Prefix(GameLocation __instance, Vector2 tileLocation)
        {
            __instance.modData[isExplodingString] = "true";
            monitorStatic.Log("Prefix on GameLocation.Explode hit", LogLevel.Trace);
            if (fireworkLocs.TryGetValue(__instance.Name, out Dictionary<Vector2, Color> localDict))
            {
                if (localDict.TryGetValue(tileLocation, out Color thisCol))
                {
                    __instance.modData[explodeColorString] = thisCol.PackedValue.ToString("X", CultureInfo.InvariantCulture); ;
                    monitorStatic.Log($"Setting explosion color to {thisCol}", LogLevel.Trace);
                }
            }
        }

        private static void broadcastSprites_Prefix(GameLocation location, List<TemporaryAnimatedSprite> sprites)
        {
            // Make the bomb color the right color
            if (location.modData.ContainsKey(isExplodingString) && location.modData[isExplodingString].Equals("true", StringComparison.OrdinalIgnoreCase) && location.modData.ContainsKey(explodeColorString))
            {
                bool parsedColor = uint.TryParse(location.modData[explodeColorString], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result);
                if (parsedColor)
                {
                    Color explodeColor = new Color(result);
                    monitorStatic.Log($"Changing sprites to {explodeColor}", LogLevel.Trace);
                    foreach (TemporaryAnimatedSprite spr in sprites)
                    {
                        spr.color = explodeColor;
                    }
                }
                else
                {
                    monitorStatic.Log("Failed to parse color from moddata, color conversion issue", LogLevel.Warn);
                }
             }
        }

        private static void explode_Postfix(GameLocation __instance, Vector2 tileLocation)
        {
            __instance.modData[isExplodingString] = "false";
            __instance.modData.Remove(explodeColorString);
            monitorStatic.Log("Postfix on GameLocation.Explode hit",LogLevel.Trace);
            if (fireworkLocs.TryGetValue(__instance.Name, out Dictionary<Vector2, Color> localDict))
            {
                monitorStatic.Log("Removing firework location from dictionary", LogLevel.Trace);
                localDict.Remove(tileLocation);
                helperStatic.Multiplayer.SendMessage((__instance.Name,tileLocation), msgTypeRemove, modIDs: new[] { thisModID });
            }
        }

        private static Rectangle getFireworksRect(Color color)
        {
            if (color.Equals(Color.Red))
            {
                return new Rectangle(0, 0, 16, 16);
            }
            else if (color.Equals(Color.Orange))
            {
                return new Rectangle(16, 0, 16, 16);
            }
            else if (color.Equals(Color.Yellow))
            {
                return new Rectangle(32, 0, 16, 16);
            }
            else if (color.Equals(Color.Green))
            {
                return new Rectangle(48, 0, 16, 16);
            }
            else if (color.Equals(Color.Blue))
            {
                return new Rectangle(64, 0, 16, 16);
            }
            else if (color.Equals(Color.Purple))
            {
                return new Rectangle(80, 0, 16, 16);
            }
            return new Rectangle(96, 0, 16, 16);
        }

        public static bool postFireworkBuy(ISalable item, Farmer farmer, int amount)
        {
            monitorStatic.Log($"Post buy {item.Name}", LogLevel.Trace);
            if (DGA_API.GetDGAItemId(item).Equals(fireworksLicenseName,StringComparison.OrdinalIgnoreCase) && !Game1.player.mailReceived.Contains(licenseLetter))
            {
                Game1.player.mailReceived.Add(licenseLetter);
                addCraftingRecipe(redFireworkRecipeName);
                addCraftingRecipe(orangeFireworkRecipeName);
                addCraftingRecipe(yellowFireworkRecipeName);
                addCraftingRecipe(greenFireworkRecipeName);
                addCraftingRecipe(blueFireworkRecipeName);
                addCraftingRecipe(purpleFireworkRecipeName);
                addCraftingRecipe(whiteFireworkRecipeName);
            }
            return false;
        }

        private Dictionary<ISalable, int[]> getClothingShopStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add(new StardewValley.Objects.Clothing(1226), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1270), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1193), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1016), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1043), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1212), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1081), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1085), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(1144), new int[4] { 0, 1, carpIndex, 1 });
            stock.Add(new StardewValley.Objects.Clothing(10), new int[4] { 0, 1, carpIndex, 2 });
            stock.Add(new StardewValley.Objects.Clothing(11), new int[4] { 0, 1, carpIndex, 2 });
            stock.Add(new StardewValley.Objects.Clothing(12), new int[4] { 0, 1, carpIndex, 2 });
            stock.Add(new StardewValley.Objects.Hat(44), new int[4] { 0, 1, carpIndex, 3 });
            stock.Add(new StardewValley.Objects.Hat(67), new int[4] { 0, 1, carpIndex, 3 });
            stock.Add(new StardewValley.Objects.Hat(42), new int[4] { 0, 1, carpIndex, 3 });
            stock.Add(new StardewValley.Objects.Hat(36), new int[4] { 0, 1, carpIndex, 3 });
            if (Game1.player.achievements.Contains(34))
            {
                stock.Add(new StardewValley.Objects.Hat(9), new int[4] { 0, 1, carpIndex, 3 });
            }
            return stock;
        }

        private Dictionary<ISalable, int[]> getBlueBoatStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add((ISalable)DGA_API.SpawnDGAItem(takoyakiName), new int[2] { 500, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(yakisobaName), new int[2] { 500, int.MaxValue });
            stock.Add(new StardewValley.Object(202, 1), new int[2] { 1500, 1 });
            stock.Add(new StardewValley.Object(214, 1), new int[2] { 1500, 1 });
            stock.Add(new StardewValley.Object(205, 1), new int[2] { 1500, 1 });
            return stock;
        }

        private Dictionary<ISalable, int[]> getPurpleBoatStock()
        {
            Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
            stock.Add((ISalable)DGA_API.SpawnDGAItem(redFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(orangeFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(yellowFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(greenFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(blueFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(purpleFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(whiteFWName), new int[2] { 5000, int.MaxValue });
            stock.Add((ISalable)DGA_API.SpawnDGAItem(fireworksLicenseName), new int[2] { 50000, 1 });
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

        private static void addCraftingRecipe(string recipeName)
        {
            if (!Game1.player.craftingRecipes.ContainsKey(recipeName))
            {
                Game1.player.craftingRecipes.Add(recipeName, 0);
            }
        }

        private static void suppressClick()
        {
            helperStatic.Input.Suppress(Game1.options.actionButton[0].ToSButton());
            helperStatic.Input.Suppress(Game1.options.useToolButton[0].ToSButton());
            helperStatic.Input.Suppress(SButton.MouseLeft);
            helperStatic.Input.Suppress(SButton.MouseRight);
        }
    }
}