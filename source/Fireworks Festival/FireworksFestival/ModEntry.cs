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
        // Storing whether or not free gift has been received
        private static bool hasReceivedFreeGift;

        // Monitor
        public static IMonitor monitorStatic;

        // Helper
        public static IModHelper helperStatic;

        /// <summary>Game1.multiplayer from reflection.</summary>
        private static Multiplayer multiplayer;

        // Useful strings
        private static string thisModID = "violetlizabet.FireworksFestival";
        private static string fireworkTexLoc = $"Mods/{thisModID}/Fireworks";
        private static string burstTexLoc = $"Mods/{thisModID}/FireworkBurst";
        private static string fireworkTexLocInGame = $"Mods\\{thisModID}\\Fireworks";
        private static string burstTexLocInGame = $"Mods\\{thisModID}\\FireworkBurst";
        private static string isExplodingString = thisModID + "/isExploding";
        private static string explodeColorString = thisModID + "/explodeColor";
        private static string fishingGameString = thisModID + "/fishingGame";
        private static string festivalLetter = "vl.FireworksFestival";
        private static string licenseLetter = "vl.fireworkslicense";
        private static string msgTypeRemove = "fireworkRemovalMessage";
        private static string msgTypeAdd = "fireworkAddMessage";
        private static string chemizerRecipeName = thisModID + "_Chemizer";
        private static string blackPowderRecipeName = thisModID + "_BlackPowder";
        private static string redFireworkRecipeName = thisModID + "_RedFirework";
        private static string orangeFireworkRecipeName = thisModID + "_OrangeFirework";
        private static string yellowFireworkRecipeName = thisModID + "_YellowFirework";
        private static string greenFireworkRecipeName = thisModID + "_GreenFirework";
        private static string blueFireworkRecipeName = thisModID + "_BlueFirework";
        private static string purpleFireworkRecipeName = thisModID + "_PurpleFirework";
        private static string whiteFireworkRecipeName = thisModID + "_WhiteFirework";

        // Carp index
        public static string carpIndex = "142";

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
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // Add in festival tile stuff for shaved ice and minigame
            harmony.Patch(
               original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogue)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.answerDialogue_Postfix))
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
                typeof(GameLocation), typeof(TemporaryAnimatedSpriteList)}),
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
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            hasReceivedFreeGift = false;

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
                    removeCraftingRecipe("FireworksFestivalChemizer");
                }
                if (!Game1.player.craftingRecipes.ContainsKey(blackPowderRecipeName))
                {
                    Monitor.Log("Adding black powder recipe directly", LogLevel.Trace);
                    Game1.player.craftingRecipes.Add(blackPowderRecipeName, 0);
                    removeCraftingRecipe("FireworksFestivalBlackPowder");
                }
            }

            // Forcibly add some recipes if needed
            if (Game1.player.mailReceived.Contains(licenseLetter))
            {
                Monitor.Log("Adding fireworks recipes directly", LogLevel.Trace);
                addCraftingRecipe(redFireworkRecipeName);
                addCraftingRecipe(orangeFireworkRecipeName);
                addCraftingRecipe(yellowFireworkRecipeName);
                addCraftingRecipe(greenFireworkRecipeName);
                addCraftingRecipe(blueFireworkRecipeName);
                addCraftingRecipe(purpleFireworkRecipeName);
                addCraftingRecipe(whiteFireworkRecipeName);
            }

            // Migrate old recipes if needed
            if (removeCraftingRecipe("FireworksFestivalRedFirework"))
            {
                Monitor.Log("Migrating old red firework recipe", LogLevel.Trace);
                addCraftingRecipe(redFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalOrangeFirework"))
            {
                Monitor.Log("Migrating old orange firework recipe", LogLevel.Trace);
                addCraftingRecipe(orangeFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalYellowFirework"))
            {
                Monitor.Log("Migrating old yellow firework recipe", LogLevel.Trace);
                addCraftingRecipe(yellowFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalGreenFirework"))
            {
                Monitor.Log("Migrating old green firework recipe", LogLevel.Trace);
                addCraftingRecipe(greenFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalBlueFirework"))
            {
                Monitor.Log("Migrating old blue firework recipe", LogLevel.Trace);
                addCraftingRecipe(blueFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalPurpleFirework"))
            {
                Monitor.Log("Migrating old purple firework recipe", LogLevel.Trace);
                addCraftingRecipe(purpleFireworkRecipeName);
            }
            if (removeCraftingRecipe("FireworksFestivalWhiteFirework"))
            {
                Monitor.Log("Migrating old white firework recipe", LogLevel.Trace);
                addCraftingRecipe(whiteFireworkRecipeName);
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
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1681"), responses2, "fireworksFestivalFishingMinigame");
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
            string itemID = __instance.ItemId;
            if (itemID == null || !(itemID.Contains(thisModID, StringComparison.OrdinalIgnoreCase) && itemID.EndsWith("Firework", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            else
            {
                Color color = Color.White;
                if (itemID.Equals(thisModID + ".RedFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Red;
                }
                else if(itemID.Equals(thisModID + ".OrangeFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Orange;
                }
                else if (itemID.Equals(thisModID + ".YellowFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Yellow;
                }
                else if (itemID.Equals(thisModID + ".GreenFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Green;
                }
                else if (itemID.Equals(thisModID + ".BlueFirework", StringComparison.OrdinalIgnoreCase))
                {
                    color = Color.Blue;
                }
                else if (itemID.Equals(thisModID + ".PurpleFirework", StringComparison.OrdinalIgnoreCase))
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
                            Game1.player.addItemByMenuIfNecessaryElseHoldUp(new StardewValley.Object(thisModID + ".ShavedIce",1));
                            Game1.player.modData[thisModID] = "true";
                            hasReceivedFreeGift = true;
                        } 
                        break;
                    case 1:
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
                        break;
                }
            }
            else if (questionKey.Equals("fireworksFestivalFishingMinigame",StringComparison.OrdinalIgnoreCase))
            {
                if (answerChoice == 0)
                {
                    if (Game1.player.Money >= 50)
                    {
                        Game1.globalFadeToBlack(FireworksFestivalFishingMinigame.startMe, 0.01f);
                        Game1.player.Money -= 50;
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
                    }
                }
            }
        }

        private static void specificTemporarySprite_Postfix(string key, GameLocation location, string[] args)
        {
            if (key.Equals("vlFireworkBurst", StringComparison.OrdinalIgnoreCase))
            {
                // Make sure there's enough arguments
                if (args.Length < 4)
                {
                    monitorStatic.Log("Not enough arguments to specificTAS vlFireworkBurst",LogLevel.Warn);
                    return;
                }
                // Check that location is correct
                if (int.TryParse(args[2], out int xLoc) && int.TryParse(args[3], out int yLoc))
                {
                    int type = 0;
                    // Check that sprite details are correct
                    if (args.Length < 5 || !int.TryParse(args[4], out type))
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
                    if (args.Length < 6 || !float.TryParse(args[5], out animInterval))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation interval, setting to {animInterval}", LogLevel.Warn);
                    }

                    int animLen = 8;
                    if (args.Length < 7 || !int.TryParse(args[6], out animLen))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation length, setting to {animLen}", LogLevel.Warn);
                    }

                    int numLoops = 1;
                    if (args.Length < 8 || !int.TryParse(args[7], out numLoops))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set animation length, setting to {numLoops}", LogLevel.Trace);
                    }

                    bool flicker = false;
                    if (args.Length < 9 || !bool.TryParse(args[8], out flicker))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set flicker, setting to {flicker}", LogLevel.Trace);
                    }

                    bool flipped = false;
                    if (args.Length < 10 || !bool.TryParse(args[9], out flipped))
                    {
                        monitorStatic.Log($"specificTAS vlFireworkBurst failed to set flip, setting to {flipped}", LogLevel.Trace);
                    }

                    float layerDepth = 99f;
                    if (args.Length < 11 || !float.TryParse(args[10], out layerDepth))
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

        // Make it actually register the fish caught
        private static void caughtFish_Postfix(Event __instance, string itemId, int size)
        {
            if (itemId != null && Game1.currentMinigame != null && isSummer && Game1.dayOfMonth == 20)
            {
                (Game1.currentMinigame as FireworksFestivalFishingMinigame).score += ((size <= 0) ? 1 : (size + 5));
                if (size > 0)
                {
                    (Game1.currentMinigame as FireworksFestivalFishingMinigame).fishCaught++;
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
                (Game1.currentMinigame as FireworksFestivalFishingMinigame).perfections++;
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

        private static void broadcastSprites_Prefix(GameLocation location, TemporaryAnimatedSpriteList sprites)
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

        private static void addCraftingRecipe(string recipeName)
        {
            if (!Game1.player.craftingRecipes.ContainsKey(recipeName))
            {
                Game1.player.craftingRecipes.Add(recipeName, 0);
            }
        }

        private static bool removeCraftingRecipe(string recipeName)
        {
            if (Game1.player.craftingRecipes.ContainsKey(recipeName))
            {
                Game1.player.craftingRecipes.Remove(recipeName);
                return true;
            }
            return false;
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