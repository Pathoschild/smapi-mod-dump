/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using xTile.Dimensions;
using StardewObject = StardewValley.Object;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace MaritimeSecrets
{
    internal class Patcher
    {
        private static MaritimeSecrets mod;

        private static readonly string wormBinUpgradeKey = $"{mod?.ModManifest?.UniqueID}/WormBinUpgrade";
        private static readonly string summerForageCalendarKey = $"{mod?.ModManifest?.UniqueID}/SummerForageCalendar";

        private static readonly string receivedFrogHatKey = $"{mod?.ModManifest?.UniqueID}/ReceivedFrogHat";
        private static readonly string receivedLifeSaverKey = $"{mod?.ModManifest?.UniqueID}/ReceivedLifeSaver";
        private static readonly string receivedTrashCanKey = $"{mod?.ModManifest?.UniqueID}/ReceivedTrashCan";
        private static readonly string receivedWallBasketKey = $"{mod?.ModManifest?.UniqueID}/ReceivedWallBasket";
        private static readonly string receivedPyramidDecalKey = $"{mod?.ModManifest?.UniqueID}/ReceivedPyramidDecal";
        private static readonly string receivedGourmandStatueKey = $"{mod?.ModManifest?.UniqueID}/ReceivedGourmandStatue";

        private static readonly XnaRectangle summerForage = new(144, 256, 16, 16);
        private const string driftWoodId = "(O)169";
        private const string pearlId = "(O)797";

        private const string mermaidPendantUnqualifiedId = "460";
        private const string mermaidPendantId = $"(O){mermaidPendantUnqualifiedId}";

        public static void PatchAll(MaritimeSecrets maritimeSecrets)
        {
            mod = maritimeSecrets;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(GameLocation_AnswerDialogueAction_Prefix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.createQuestionDialogue), new Type[] { typeof(string), typeof(Response[]), typeof(string) }),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(GameLocation_CreateQuestionDialogue_Prefix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Beach), nameof(Beach.checkAction)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Beach_CheckAction_Prefix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Billboard), nameof(Billboard.performHoverAction)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Billboard_PerformHoverAction_Postfix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Billboard_Draw_Postfix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(GameLocation_GetFish_Postfix)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.performObjectDropInAction), new[] { typeof(Item), typeof(bool), typeof(Farmer), typeof(bool) }),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(WormBin_PerformObjectDropInAction_Prefix)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        // patch with very high priority so we get called before mods like 'Free Love' who want to change the cost
        // if people want to use the 'Free Love' feature they can disable my feature, but not the other way around
        [HarmonyPriority(Priority.High)]
        public static bool GameLocation_AnswerDialogueAction_Prefix(string questionAndAnswer, ref bool __result)
        {
            if (!mod.Config.ChangePendantPriceToPearl)
            {
                return true;
            }

            if (questionAndAnswer == "mariner_Buy")
            {
                if (Game1.player.Items.ContainsId(pearlId, 1))
                {
                    Game1.player.Items.ReduceId(pearlId, 1);

                    var mermaidPendant = ItemRegistry.Create<StardewObject>(mermaidPendantId, 1, 0, false);
                    mermaidPendant.specialItem = true;

                    Game1.player.addItemByMenuIfNecessary(mermaidPendant, null, false);
                    if (Game1.activeClickableMenu == null)
                    {
                        Game1.player.holdUpItemThenMessage(ItemRegistry.Create(mermaidPendantId, 1, 0, false), true);
                    }
                }
                else
                {
                    var pearlItem = ItemRegistry.Create(pearlId);
                    pearlItem.stack.Value = 0;
                    // this means 'I need {0}, if it's not too inconvenient.'
                    var iNeedString = Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13494", $"1 {pearlItem.DisplayName}");
                    Game1.drawObjectDialogue(iNeedString);
                }

                __result = true;
                return false;
            }
            else if (questionAndAnswer == "Mariner_Buy")
            {
                // the capital mariner case is probably no longer used by the game or was used for debugging, but let's change it anyway (the mod 'Free Love' does not change it for example)
                if (Game1.player.Items.ContainsId(pearlId, 1))
                {
                    Game1.player.Items.ReduceId(pearlId, 1);

                    var mermaidPendant = ItemRegistry.Create<StardewObject>(mermaidPendantId, 1, 0, false);
                    mermaidPendant.CanBeSetDown = false;

                    Game1.player.grabObject(mermaidPendant);
                }
                else
                {
                    var pearlItem = ItemRegistry.Create(pearlId);
                    pearlItem.stack.Value = 0;
                    // this means 'I need {0}, if it's not too inconvenient.'
                    var iNeedString = Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13494", $"1 {pearlItem.DisplayName}");
                    Game1.drawObjectDialogue(iNeedString);
                }

                __result = true;
                return false;
            }

            return true;
        }

        // patch with very high priority so this gets called if and only if GameLocation_AnswerDialogueAction_Prefix didn't get overridden by a mod that patches both with even higher priority
        [HarmonyPriority(Priority.VeryHigh)]
        public static bool GameLocation_CreateQuestionDialogue_Prefix(string question, ref Response[] answerChoices, string dialogKey)
        {
            if (!mod.Config.ChangePendantPriceToPearl)
            {
                return true;
            }

            if (dialogKey == "mariner")
            {
                foreach (var item in answerChoices)
                {
                    if (item.responseKey == "Buy")
                    {
                        var pearlItem = ItemRegistry.Create(pearlId);
                        pearlItem.stack.Value = 0;
                        var buyString = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1654");
                        item.responseText = $"{buyString} (1 {pearlItem.DisplayName})";
                    }
                }
            }

            return true;
        }

        public static bool WormBin_PerformObjectDropInAction_Prefix(ref StardewObject __instance, ref bool __result, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool returnFalseIfItemConsumed)
        {
            if (probe)
            {
                return true;
            }

            if (!(__instance.QualifiedItemId is "(BC)154" or "(BC)DeluxeWormBin") || !(who?.modData?.ContainsKey(wormBinUpgradeKey) == true))
            {
                return true;
            }

            if (__instance.heldObject.Value != null && !__instance.readyForHarvest.Value && __instance.heldObject.Value.Stack <= 5)
            {
                if (dropInItem is StardewObject o && o.QualifiedItemId == driftWoodId && who.IsLocalPlayer)
                {
                    who.Items.ReduceId(driftWoodId, 1);

                    who.currentLocation.playSound("slimeHit");

                    // 8 to 10 (11 is exclusive)
                    var newStackSize = Game1.random.Next(8, 11);

                    __instance.heldObject.Value.Stack = newStackSize;

                    __result = false;
                    return false;
                }
            }

            return true;
        }

        public static void Billboard_PerformHoverAction_Postfix(ref Billboard __instance, int x, int y, ref string ___hoverText)
        {
            if (__instance.calendarDays == null || !Game1.IsSummer || (Game1.player?.modData?.ContainsKey(summerForageCalendarKey)) != true)
            {
                return;
            }

            for (int day = 0; day < __instance.calendarDays.Count;)
            {
                ClickableTextureComponent c = __instance.calendarDays[day++];

                if (c.bounds.Contains(x, y))
                {
                    if (12 <= day && day <= 14)
                    {
                        if (___hoverText.Length > 0)
                        {
                            ___hoverText += Environment.NewLine;
                        }

                        ___hoverText += GetTranslation("GreenOcean");
                    }
                }
            }
        }

        public static void Billboard_Draw_Postfix(Billboard __instance, SpriteBatch b, bool ___dailyQuestBoard, string ___hoverText)
        {
            if (!___dailyQuestBoard)
            {
                for (int day = 1; day <= 28; day++)
                {
                    XnaRectangle toDraw = XnaRectangle.Empty;

                    if (Game1.IsSummer && 12 <= day && day <= 14 && Game1.player?.modData?.ContainsKey(summerForageCalendarKey) == true)
                    {
                        toDraw = summerForage;
                    }

                    if (toDraw != XnaRectangle.Empty)
                    {
                        Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float)(__instance.calendarDays[day - 1].bounds.X + 12), (float)(__instance.calendarDays[day - 1].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), toDraw, Color.White, 0f, Vector2.Zero, 2f, false, 1f, -1, -1, 0.35f);

                        Game1.mouseCursorTransparency = 1f;
                        __instance.drawMouse(b);
                        if (___hoverText.Length > 0)
                        {
                            IClickableMenu.drawHoverText(b, ___hoverText, Game1.dialogueFont);
                        }
                    }
                }
            }
        }

        private static void CheckReceivedItemKey(Farmer who, string moddedKeyToAdd)
        {
            who.modData[moddedKeyToAdd] = "true";
        }

        public static void GameLocation_GetFish_Postfix(GameLocation __instance, Farmer who, string locationName, Item __result)
        {
            // getFish recursively calls itself, we only check the case where locationName is null
            if (locationName != null)
            {
                return;
            }

            if (mod?.ModManifest?.UniqueID == null)
            {
                return;
            }

            if (__instance is IslandFarmCave && __result is Hat && __result.QualifiedItemId == "(H)78")
            {
                CheckReceivedItemKey(who, receivedFrogHatKey);
            }

            if (__result is Furniture)
            {
                switch (__result.QualifiedItemId)
                {
                    case "(F)2418":
                        if (__instance is BoatTunnel)
                            CheckReceivedItemKey(who, receivedLifeSaverKey);

                        break;

                    case "(F)2427":
                        if (__instance is Town)
                            CheckReceivedItemKey(who, receivedTrashCanKey);

                        break;

                    case "(F)2425":
                        if (__instance is Woods)
                            CheckReceivedItemKey(who, receivedWallBasketKey);

                        break;

                    case "(F)2334":
                        if (__instance is Desert)
                            CheckReceivedItemKey(who, receivedPyramidDecalKey);

                        break;

                    case "(F)2332":
                        if (__instance is IslandSouthEastCave)
                            CheckReceivedItemKey(who, receivedGourmandStatueKey);

                        break;
                }
            }
        }

        private static bool TryAskToBuyPendantWithoutMeetingRequirements(Beach beach, Farmer who)
        {
            if (who.isMarriedOrRoommates() || who.specialItems.Contains(mermaidPendantUnqualifiedId))
            {
                // shouldn't get a second pendant (if you have polyamory mods, they will handle this case themselves)
                return false;
            }
            else if (who.hasAFriendWithHeartLevel(10, datablesOnly: true) && who.HouseUpgradeLevel != 0)
            {
                // has requirements (let base game handle it)
                // '!= 0' is correct since the base game uses '== 0', so technically negative house upgrade level is valid
                return false;
            }
            else
            {
                string playerTerm = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (who.IsMale ? "Male" : "Female"));

                Response[] answers = new Response[2]
                {
                    new Response("Buy", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerYes")),
                    new Response("Not", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerNo"))
                };

                beach.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_Question", playerTerm)), answers, "mariner");

                return true;
            }
        }

        // patch with very high priority so we get called before mods like 'Free Love' which want to remove the marriage restriction
        [HarmonyPriority(Priority.VeryHigh)]
        public static bool Beach_CheckAction_Prefix(Beach __instance, Location tileLocation, Farmer who, NPC ___oldMariner, ref bool __result)
        {
            if (who == null || ___oldMariner == null
                || ___oldMariner.Tile.X != tileLocation.X || ___oldMariner.Tile.Y != tileLocation.Y)
            {
                return true;
            }

            if (mod?.ModManifest?.UniqueID == null || who.modData.ContainsKey(mod.talkedToMarinerTodayKey))
            {
                if (!mod.Config.CanBuyPendantWithoutHeartsAndHouseUpgrade)
                {
                    return true;
                }

                bool askedToBuyWithoutReq = TryAskToBuyPendantWithoutMeetingRequirements(__instance, who);

                if (askedToBuyWithoutReq)
                {
                    __result = true;
                }

                return !askedToBuyWithoutReq;
            }

            who.modData[mod.talkedToMarinerTodayKey] = "true";

            var marinerName = GetMarinerName();

            string speechTypeSuffix = "";

            if ((mod.Config.MarinerSpeechType == SpeechType.dynamic && !mod.IsUsingMermaidMod) || mod.Config.MarinerSpeechType == SpeechType.sailor)
            {
                speechTypeSuffix = "_Sailor";
            }

            // maybe I should refactor everything to a string builder instead, but it doesn't seem worth it yet for only about 3 string concatenations
            string secret = GetTranslation("Secret" + speechTypeSuffix, marinerName) + " ";

            __result = true;

            // major secrets

            if (!who.modData.ContainsKey(summerForageCalendarKey))
            {
                who.modData[summerForageCalendarKey] = "true";
                Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation("SummerForageCalendar" + speechTypeSuffix)));
                return false;
            }

            // check the fishingLevel netfield, because the upper case FishingLevel property also adds buffs and enchantments. the 'crafting recipe OR' check is for mod compatibility
            if ((who.fishingLevel.Value >= 8 || who.craftingRecipes.ContainsKey("Worm Bin")) && !who.modData.ContainsKey(wormBinUpgradeKey))
            {
                who.modData[wormBinUpgradeKey] = "true";
                Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation("WormBinUpgrade" + speechTypeSuffix)));
                return false;
            }

            // removed the 'who.hasMagnifyingGlass' condition so you can do it in year 1
            // the 'Vanilla' name check is for compatibility with Nayas mermaid mod for lore consistency
            if (!who.secretNotesSeen.Contains(15))
            {
                who.secretNotesSeen.Add(15);

                // yes, if people set the speech pattern to sailor and use a mermaid mod, the mermaid will say the mermaid specific line like a sailor
                string transl = mod.IsUsingMermaidMod ? marinerName == "Vanilla" ? "MermaidSecretNote_MermaidBlueHair" : "MermaidSecretNote_Mermaid" : "MermaidSecretNote";

                Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation(transl + speechTypeSuffix)));
                return false;
            }

            if (Game1.stats.DaysPlayed >= 31 && !Game1.IsWinter && (!who.mailReceived.Contains("gotSpaFishing") || !FoundSpaNecklace(who)))
            {
                if (!who.secretNotesSeen.Contains(GameLocation.NECKLACE_SECRET_NOTE_INDEX))
                {
                    who.secretNotesSeen.Add(GameLocation.NECKLACE_SECRET_NOTE_INDEX);
                }

                // xor
                bool oneDone = who.mailReceived.Contains("gotSpaFishing") ^ FoundSpaNecklace(who);

                string transl = oneDone ? "SpaPaintingOrNecklace" : "SpaPaintingAndNecklace";

                Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation(transl + speechTypeSuffix)));
                return false;
            }

            // minor secrets

            string translation = null;

            bool selectedMinorSecret = false;
            while (!selectedMinorSecret)
            {
                if (Game1.whichFarm == Farm.beach_layout && !who.mailReceived.Contains("gotBoatPainting"))
                {
                    translation = "BeachFarmBoatPainting";
                    selectedMinorSecret = true;
                    break;
                }

                if (!who.modData.ContainsKey(receivedTrashCanKey))
                {
                    if (CheckJojaMartComplete())
                    {
                        translation = "TownTrashCanJojaWareHouse";
                    }
                    else if (CheckCommunityCenterComplete())
                    {
                        translation = "TownTrashCanRestoredCommunityCenter";
                    }
                    else
                    {
                        translation = "TownTrashCanBrokenCommunityCenter";
                    }

                    selectedMinorSecret = true;
                    break;
                }

                // if caught woodskip as secret woods condition
                if (who.fishCaught.ContainsKey("(O)734") && !who.modData.ContainsKey(receivedWallBasketKey))
                {
                    translation = "SecretWoodsWallBasket";
                    selectedMinorSecret = true;
                    break;
                }

                if (CheckDesertUnlocked() && !who.modData.ContainsKey(receivedPyramidDecalKey))
                {
                    translation = "DesertPyramidDecal";
                    selectedMinorSecret = true;
                    break;
                }

                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("seenBoatJourney"))
                {
                    if (!who.modData.ContainsKey(receivedLifeSaverKey))
                    {
                        translation = "WillyLifeSaver";
                        selectedMinorSecret = true;
                        break;
                    }

                    if (who.hasOrWillReceiveMail("talkedToGourmand"))
                    {
                        if (!who.modData.ContainsKey(receivedFrogHatKey))
                        {
                            translation = "IslandGourmandStatue";
                            selectedMinorSecret = true;
                            break;
                        }
                    }

                    if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoBridge"))
                    {
                        if (!who.mailReceived.Contains("gotSecretIslandNSquirrel"))
                        {
                            translation = "IslandSquirrel";
                            selectedMinorSecret = true;
                            break;
                        }
                    }

                    if (Game1.MasterPlayer.hasOrWillReceiveMail("reachedCaldera"))
                    {
                        if (!who.mailReceived.Contains("CalderaPainting"))
                        {
                            translation = "VolcanoPainting";
                            selectedMinorSecret = true;
                            break;
                        }
                    }

                    if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_Resort"))
                    {
                        if (!who.modData.ContainsKey(receivedGourmandStatueKey))
                        {
                            translation = "IslandGourmandStatue";
                            selectedMinorSecret = true;
                            break;
                        }
                    }

                    if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_Turtle"))
                    {
                        if (!who.mailReceived.Contains("gotSecretIslandNPainting"))
                        {
                            translation = "IslandPainting";
                            selectedMinorSecret = true;
                            break;
                        }
                    }
                }

                // check the fishingLevel netfield, because the upper case FishingLevel property also adds buffs and enchantments.
                if (who.fishingLevel.Value >= 10 && !Game1.player.mailReceived.Contains("caughtIridiumKrobus"))
                {
                    // now we want to calculate with temporary buffs
                    translation = who.FishingLevel < 15 ? "IridiumKrobusNotReady" : "IridiumKrobusReady";

                    selectedMinorSecret = true;
                    break;
                }

                break;
            }

            // if managed to place a crate
            if (TryToSpawnSupplyCrate(__instance))
            {
                if (selectedMinorSecret && translation != null)
                {
                    Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation(translation) + " " + GetTranslation("AlsoBeachSupplyCrate" + speechTypeSuffix)));
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.parseText(secret + GetTranslation("BeachSupplyCrate" + speechTypeSuffix)));
                }

                return false;
            }

            // if somehow failed to place a crate
            Game1.drawObjectDialogue(Game1.parseText(GetTranslation("NoSecret" + speechTypeSuffix, marinerName)));
            return false;
        }

        private static string GetTranslation(string key)
        {
            if (mod.marinerDialogueOverride.TryGetValue(key, out string overrideTranslation))
            {
                return overrideTranslation;
            }
            else
            {
                return mod.Helper.Translation.Get(key);
            }
        }

        private static string GetTranslation(string key, string marinerName)
        {
            if (mod.marinerDialogueOverride.TryGetValue(key, out string overrideTranslation))
            {
                return overrideTranslation.Replace("{{name}}", marinerName);
            }
            else
            {
                return mod.Helper.Translation.Get(key, new { name = marinerName });
            }
        }

        private static string GetMarinerName()
        {
            if (!string.IsNullOrWhiteSpace(mod.Config.MarinerNameOverwrite))
            {
                return mod.Config.MarinerNameOverwrite.Trim();
            }

            var sampleMarinerSentence = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerMarried");

            // some asian languages use a different unicode colon character, the fullwidth colon.
            // for some edge cases, this is checked before the normal unicode colon
            var index = sampleMarinerSentence.IndexOf('：');

            if (index < 0)
            {
                index = sampleMarinerSentence.IndexOf(':');
            }

            if (index > 0)
            {
                return sampleMarinerSentence[..index];
            }
            else
            {
                // if we couldn't find a name, use fallback
                return GetTranslation("FallbackOldMarinerName");
            }
        }

        // based mostly on Beach.DayUpdate foraging code
        private static bool TryToSpawnSupplyCrate(Beach beach)
        {
            // taken from Farm.DayUpdate
            var whichSupplyCrate = Game1.random.Next(922, 925).ToString();

            // arbitrary max number (it's about 500*2)
            int maxTries = beach.Map.Layers[0].LayerWidth * beach.Map.Layers[0].LayerHeight * 2;

            for (int tries = 0; tries < maxTries; tries++)
            {
                Vector2 randomPosition = beach.getRandomTile();
                randomPosition.Y /= 2f;
                randomPosition.X /= 2f;

                if (randomPosition.X < 10)
                {
                    randomPosition.X += 10;
                }

                if (randomPosition.Y < 10)
                {
                    randomPosition.Y += 15;
                }

                string prop = beach.doesTileHaveProperty((int)randomPosition.X, (int)randomPosition.Y, "Type", "Back");

                if (beach.CanItemBePlacedHere(randomPosition, false, CollisionMask.All, ~CollisionMask.Objects, false, false) && (prop == null || !prop.Equals("Wood")))
                {
                    beach.dropObject(new StardewObject(whichSupplyCrate, 1, false, -1, 0)
                    {
                        Fragility = 2,
                        MinutesUntilReady = 3
                    }, randomPosition * 64f, Game1.viewport, true, null);

                    return true;
                }
            }

            return false;
        }

        private static bool FoundSpaNecklace(Farmer who)
        {
            return who.secretNotesSeen.Contains(GameLocation.NECKLACE_SECRET_NOTE_INDEX) && who.hasOrWillReceiveMail(GameLocation.CAROLINES_NECKLACE_MAIL);
        }

        private static bool CheckDesertUnlocked()
        {
            return Game1.MasterPlayer.mailReceived.Contains("ccVault") || Game1.MasterPlayer.mailReceived.Contains("jojaVault");
        }

        private static bool CheckCommunityCenterComplete()
        {
            return Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.hasCompletedCommunityCenter();
        }

        // based on Utility.hasFinishedJojaRoute, just replaced Game1.player with Game1.MasterPlayer
        private static bool CheckJojaMartComplete()
        {
            bool foundJoja = false;
            if (Game1.MasterPlayer.mailReceived.Contains("jojaVault"))
            {
                foundJoja = true;
            }
            else if (!Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
                return false;
            }

            if (Game1.MasterPlayer.mailReceived.Contains("jojaPantry"))
            {
                foundJoja = true;
            }
            else if (!Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
            {
                return false;
            }

            if (Game1.MasterPlayer.mailReceived.Contains("jojaBoilerRoom"))
            {
                foundJoja = true;
            }
            else if (!Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
            {
                return false;
            }

            if (Game1.MasterPlayer.mailReceived.Contains("jojaCraftsRoom"))
            {
                foundJoja = true;
            }
            else if (!Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
            {
                return false;
            }

            if (Game1.MasterPlayer.mailReceived.Contains("jojaFishTank"))
            {
                foundJoja = true;
            }
            else if (!Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
            {
                return false;
            }

            if (foundJoja || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            {
                return true;
            }

            return false;
        }
    }
}