/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ichortower_HatMouseLacey
{
    /*
     * Each function in this class is a Harmony patch. It should be public and
     * static, and its name determines how it is applied by a loop in ModEntry:
     *
     *   ClassName_Within_StardewValley__MethodName__Type
     *
     * Two underscores separate the class name, method name, and type
     * (Prefix or Postfix; Transpiler in the future if needed).
     * In the class name, single underscores are converted to dots to resolve
     * the targeted class.
     *
     * Why do this instead of using annotations?
     * I already had this class structure set up, and I prefer it to making
     * a new class for every patch.
     */
    internal class Patcher
    {
        private static IMonitor Monitor = ModEntry.MONITOR;
        private static IModHelper Helper = ModEntry.HELPER;

        private static string MailPrefix = ModEntry.LCInternalName + "_";

        /*
         * Utility.getCelebrationPositionsForDatables is what makes the
         * commands to add the datable characters to the wedding event.
         * Lacey is datable, so add her in.
         */
        public static void Utility__getCelebrationPositionsForDatables__Postfix(
                StardewValley.Utility __instance,
                List<string> people_to_exclude,
                ref string __result)
        {
            if (!people_to_exclude.Contains(ModEntry.LCInternalName)) {
                __result += $"{ModEntry.LCInternalName} 31 68 0 ";
            }
        }

        /*
         * getDefaultWarpLocation checks the map name against a hardcoded list
         * in a switch/case.
         * Here, check the map properties for a DefaultWarpLocation key/value
         * (should be e.g. "12 15", space-separated like warp coordinates).
         * If found, use it for the default location.
         */
        public static void Utility__getDefaultWarpLocation__Postfix(
                StardewValley.Utility __instance,
                string location_name,
                ref int x,
                ref int y)
        {
            /* save what we find into these temps, then save to x and y only
             * after succeeding. this way, if ty throws, we don't break */
            int tx = 0;
            int ty = 0;
            GameLocation loc = Game1.getLocationFromName(location_name);
            if (loc is null || loc.map is null) {
                return;
            }
            string property = loc.getMapProperty("DefaultWarpLocation");
            if (property == "") {
                return;
            }
            string[] coords = property.Split(' ');
            try {
                tx = Convert.ToInt32(coords[0]);
                ty = Convert.ToInt32(coords[1]);
            }
            catch (Exception e) {
                Monitor.Log($"Ignoring incorrect format for DefaultWarpLocation in location {location_name}: found '{property}', expected '<int> <int>'.\n{e}",
                        LogLevel.Warn);
                return;
            }
            x = tx;
            y = ty;
        }

        /*
         * Load full song names ("HML_" cues) for the jukebox.
         */
        public static void Utility__getSongTitleFromCueName__Postfix(
                string cueName,
                ref string __result)
        {
            if (cueName.StartsWith("HML_")) {
                __result = Game1.content.LoadString($"Strings\\StringsFromCSFiles:{cueName}");
            }
        }

        /*
         * The default Utility.isMale checks the string against a hardcoded
         * list of seven names.
         * Return false (female) for Lacey.
         */
        public static void Utility__isMale__Postfix(
                StardewValley.Utility __instance,
                string who, ref bool __result)
        {
            if (who.Equals(ModEntry.LCInternalName)) {
                __result = false;
            }
        }

        /*
         * Add an extra check for the "can interact/which cursor" NPC code, to
         * display the dialogue cursor when you are pointing to Lacey and
         * wearing an unseen hat.
         */
        public static void Utility__checkForCharacterInteractionAtTile__Postfix(
                StardewValley.Utility __instance,
                Vector2 tileLocation,
                Farmer who,
                ref bool __result)
        {
            if (Game1.mouseCursor > 0) {
                return;
            }
            NPC Lacey = Game1.currentLocation.isCharacterAtTile(tileLocation);
            if (Lacey != null && Lacey.Name.Equals(ModEntry.LCInternalName)) {
                string hatstr = LCHatString.GetCurrentHatString(who);
                if (hatstr != null && !LCModData.HasShownHat(hatstr)) {
                    Game1.mouseCursor = 4;
                    __result = true;
                    if (Utility.tileWithinRadiusOfPlayer(
                            (int)tileLocation.X, (int)tileLocation.Y, 1, who)) {
                        Game1.mouseCursorTransparency = 1f;
                    }
                    else {
                        Game1.mouseCursorTransparency = 0.5f;
                    }
                }
            }
        }

        /*
         * NPC.sayHiTo() generates the "Hi, <NPC>!" etc. speech bubbles when
         * NPCs are walking near each other. This patch makes Lacey and Andy
         * say "..." to each other instead, since they don't get along.
         */
        public static bool NPC__sayHiTo__Prefix(
                StardewValley.NPC __instance,
                StardewValley.Character c)
        {
            if ((__instance.Name.Equals(ModEntry.LCInternalName) &&
                    c.Name.Equals("Andy")) ||
                    (__instance.Name.Equals("Andy") &&
                    c.Name.Equals(ModEntry.LCInternalName))) {
                __instance.showTextAboveHead("...");
                if (c is NPC && Game1.random.NextDouble() < 0.66) {
                    (c as NPC).showTextAboveHead("...", preTimer:
                            1000 + Game1.random.Next(500));
                }
                return false;
            }
            return true;
        }

        /*
         * NPC.isGaySpouse is only used to decide between pregnancy and
         * adoption for your children (including by the mouse children patch,
         * for the same purpose).
         * If the AlwaysAdopt config setting is set to true, this patch will
         * cause Lacey to return true (gay) every time, forcing adoption.
         */
        public static void NPC__isGaySpouse__Postfix(
                StardewValley.NPC __instance,
                ref bool __result)
        {
            if (__instance.Name.Equals(ModEntry.LCInternalName) &&
                    ModEntry.Config.AlwaysAdopt) {
                __result = true;
            }
        }

        /*
         * NPC.getMugShotSourceRect checks NPC.Age and adds 4 to the source
         * rect Y position for children.
         * Lacey isn't a child, but she needs the child rect. This returns
         * that rect for Lacey.
         */
        public static void NPC__getMugShotSourceRect__Postfix(
                StardewValley.NPC __instance,
                ref Microsoft.Xna.Framework.Rectangle __result)
        {
            if (__instance.Name.Equals(ModEntry.LCInternalName)) {
                __result = new Microsoft.Xna.Framework.Rectangle(0, 4, 16, 24);
            }
        }

        /*
         * Prefix NPC.checkAction to load Lacey's reactions when you are
         * wearing a hat she hasn't seen you in.
         * Requires a mail id which is set by watching the 2-heart event.
         */
        public static bool NPC__checkAction__Prefix(
                StardewValley.NPC __instance,
                StardewValley.Farmer who,
                StardewValley.GameLocation l,
                ref bool __result)
        {
            if (!__instance.Name.Equals(ModEntry.LCInternalName)) {
                return true;
            }
            if (!who.hasOrWillReceiveMail($"{MailPrefix}HatReactions")) {
                return true;
            }
            if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift()) {
                return true;
            }
            if (who.isRidingHorse()) {
                return true;
            }
            string hatstr = LCHatString.GetCurrentHatString(who);
            if (hatstr is null || LCModData.HasShownHat(hatstr)) {
                return true;
            }
            string hatkey = hatstr.Replace(" ", "").Replace("'", "").Replace("|", ".");

            string newHatText = Game1.content.LoadString(
                    $"Strings\\{__instance.Name}HatReactions:newHat");
            __instance.faceTowardFarmerForPeriod(4000, 4, faceAway: false, who);
            __instance.doEmote(32);
            who.currentLocation.localSound("give_gift");
            __instance.CurrentDialogue.Push(new Dialogue(newHatText, __instance));
            Game1.drawDialogue(__instance);
            Game1.afterDialogues = delegate {
                int nowFacing = who.FacingDirection;
                DelayedAction.delayedBehavior turn = delegate {
                    who.faceDirection(++nowFacing % 4);
                };
                int turntime = 500;
                who.freezePause = 4*turntime+800;
                DelayedAction[] anims = new DelayedAction[5] {
                        new DelayedAction(turntime, turn),
                        new DelayedAction(2*turntime, turn),
                        new DelayedAction(3*turntime, turn),
                        new DelayedAction(4*turntime, turn),
                        new DelayedAction(4*turntime+600, delegate {
                            string reactionText = Game1.content.LoadStringReturnNullIfNotFound(
                                    $"Strings\\{__instance.Name}HatReactions:{hatkey}");
                            if (reactionText is null) {
                                reactionText = Game1.content.LoadString(
                                    $"Strings\\{__instance.Name}HatReactions:404");
                            }
                            __instance.CurrentDialogue.Push(
                                    new Dialogue(reactionText, __instance));
                            Game1.drawDialogue(__instance);
                            Game1.player.changeFriendship(10, __instance);
                            who.completeQuest(236750210);
                            LCModData.AddShownHat(hatstr);
                        })
                };
                foreach (var a in anims) {
                    Game1.delayedActions.Add(a);
                }
            };
            __result = true;
            return false;
        }

        /*
         * Prefix NPC.receiveGift to set Lacey-specific birthday gift dialogue.
         * I think the only NPC birthday she collides with is Eloise, but she
         * comes with East Scarp, which is popular.
         */
        public static bool NPC__receiveGift__Prefix(
                StardewValley.NPC __instance,
                StardewValley.Object o,
                StardewValley.Farmer giver,
                bool updateGiftLimitInfo = true,
                float friendshipChangeMultiplier = 1f,
                bool showResponse = true)
        {
            if (!__instance.Name.Equals(ModEntry.LCInternalName)) {
                return true;
            }
            if (__instance.Birthday_Season is null ||
                    !Game1.currentSeason.Equals(__instance.Birthday_Season) ||
                    Game1.dayOfMonth != __instance.Birthday_Day) {
                return true;
            }
            // we only need to pull this to dump out if it's null, since a
            // null will barf in getGiftTasteForThisItem.
            Game1.NPCGiftTastes.TryGetValue(__instance.Name, out var NPCLikes);
            if (NPCLikes is null) {
                return true;
            }

            giver?.onGiftGiven(__instance, o);
            Game1.stats.GiftsGiven++;
            giver.currentLocation.localSound("give_gift");
            if (updateGiftLimitInfo)
            {
                giver.friendshipData[__instance.Name].GiftsToday++;
                giver.friendshipData[__instance.Name].GiftsThisWeek++;
                giver.friendshipData[__instance.Name].LastGiftDate = new WorldDate(Game1.Date);
            }
            switch (giver.FacingDirection)
            {
            case 0:
                ((FarmerSprite)giver.Sprite).animateBackwardsOnce(80, 50f);
                break;
            case 1:
                ((FarmerSprite)giver.Sprite).animateBackwardsOnce(72, 50f);
                break;
            case 2:
                ((FarmerSprite)giver.Sprite).animateBackwardsOnce(64, 50f);
                break;
            case 3:
                ((FarmerSprite)giver.Sprite).animateBackwardsOnce(88, 50f);
                break;
            }

            float qualityMult = 1f;
            if (o.Quality == 1) {
                qualityMult = 1.1f;
            }
            else if (o.Quality == 2) {
                qualityMult = 1.25f;
            }
            else if (o.Quality == 4) {
                qualityMult = 1.5f;
            }
            friendshipChangeMultiplier = 8f;
            if (__instance.getSpouse() != null && __instance.getSpouse().Equals(giver)) {
                friendshipChangeMultiplier = 4f;
            }
            // taste is as follows:
            //   love 0, like 2, dislike 4, hate 6, neutral 8
            // only love and like apply the quality multiplier.
            int taste = __instance.getGiftTasteForThisItem(o);
            float baseValue = 20f;
            string tasteName = "Neutral";
            if (taste == 0) {
                baseValue = 80f * qualityMult;
                tasteName = "Love";
                __instance.doEmote(20);
                __instance.faceTowardFarmerForPeriod(15000, 4, false, giver);
            }
            else if (taste == 2) {
                baseValue = 45f * qualityMult;
                tasteName = "Like";
                __instance.faceTowardFarmerForPeriod(7000, 3, true, giver);
            }
            else if (taste == 4) {
                baseValue = -20f;
                tasteName = "Dislike";
            }
            else if (taste == 6) {
                baseValue = -40f;
                tasteName = "Hate";
                __instance.doEmote(12);
                __instance.faceTowardFarmerForPeriod(15000, 4, true, giver);
            }
            string text = Game1.content.LoadString(
                    $"Characters\\Dialogue\\{ModEntry.LCInternalName}:birthday{tasteName}");
            Game1.drawDialogue(__instance, text);
            giver.changeFriendship((int)(baseValue * friendshipChangeMultiplier), __instance);
            return false;
        }

        /*
         * Prefix NPC.tryToReceiveActiveObject to implement bouquet reaction
         * dialogue.
         * This is to give character-specific reactions for <4 hearts
         * ("bouquetLow"), <8 hearts ("bouquetMid"), or 8+ acceptance
         * ("bouquetAccept").
         *
         * But there's also "bouquetRejectCruelty" and
         * "bouquetRejectCrueltyRepeat", which apply if you have been mean
         * enough to her in her heart events. The former queues a letter which
         * adds a quest enabling an extra event where you can apologize.
         * "bouquetAcceptApologized" is used if you did the apology event and
         * are giving the bouquet afterward (could be cold feet in the event
         * or after a breakup).
         */
        public static bool NPC__tryToReceiveActiveObject__Prefix(
                StardewValley.NPC __instance,
                StardewValley.Farmer who)
        {
            var obj = who.ActiveObject;
            if (!__instance.Name.Equals(ModEntry.LCInternalName) || obj.ParentSheetIndex != 458) {
                return true;
            }
            if (__instance.isMarriedOrEngaged()) {
                return true;
            }
            if (!who.friendshipData.ContainsKey(__instance.Name)) {
                return true;
            }
            Friendship friendship = who.friendshipData[__instance.Name];
            if (friendship.IsDating() || friendship.IsDivorced()) {
                return true;
            }
            who.Halt();
            who.faceGeneralDirection(__instance.getStandingPosition(),
                    0, opposite: false, useTileCalculations: false);
            string toLoad = $"Characters\\Dialogue\\{__instance.Name}:";
            bool accepted = false;
            bool addApologyQuest = false;
            if (friendship.Points < 1000) {
                toLoad += "bouquetLow";
            }
            else if (friendship.Points < 2000) {
                toLoad += "bouquetMid";
            }
            else {
                if (Game1.player.hasOrWillReceiveMail($"{MailPrefix}ApologyAccepted")) {
                    toLoad += "bouquetAcceptApologized";
                    accepted = true;
                }
                else if (Game1.player.hasOrWillReceiveMail($"{MailPrefix}ApologySummons")) {
                    toLoad += "bouquetRejectCrueltyRepeat";
                }
                else if (LCModData.CrueltyScore >= 4) {
                    toLoad += "bouquetRejectCruelty";
                    addApologyQuest = true;
                }
                else {
                    toLoad += "bouquetAccept";
                    accepted = true;
                }
            }
            if (accepted) {
                if (!friendship.IsDating()) {
                    friendship.Status = FriendshipStatus.Dating;
                    /* more reflection abuse */
                    Multiplayer mp = (Multiplayer)typeof(Game1)
                            .GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)
                            .GetValue(null);
                    mp.globalChatInfoMessage("Dating",
                            Game1.player.Name, __instance.displayName);
                }
                who.changeFriendship(25, __instance);
                who.reduceActiveItemByOne();
                who.completelyStopAnimatingOrDoingAction();
                __instance.doEmote(20);
            }
            string response = Game1.content.LoadString(toLoad);
            __instance.CurrentDialogue.Push(new Dialogue(response, __instance));
            Game1.drawDialogue(__instance);
            if (addApologyQuest) {
                Game1.afterDialogues = delegate {
                    Game1.addMailForTomorrow($"{MailPrefix}ApologySummons");
                };
            }
            return false;
        }

        /*
         * Prefix for StardewValley/Event.checkAction, used to implement the
         * hat shop at the Stardew Valley Fair.
         * Checks for a tile property "Action": "HatShop" on the buildings
         * layer, then generates the hat shop menu just like the forest shop.
         */
        public static bool Event__checkAction__Prefix(
                StardewValley.Event __instance,
                Location tileLocation,
                xTile.Dimensions.Rectangle viewport,
                Farmer who,
                ref bool __result)
        {
            try {
                if (!__instance.isFestival) {
                    return true;
                }
                string tileAction = Game1.currentLocation.doesTileHaveProperty(
                        tileLocation.X, tileLocation.Y, "Action", "Buildings");
                if (tileAction is null) {
                    return true;
                }
                string word = tileAction.Split(' ')[0];
                if (word.Equals("HatShop")) {
                    var stock = Utility.getHatStock();
                    if (stock.Count == 0) {
                        return true;
                    }
                    string dialogue = Game1.content.LoadString(
                        $"Characters\\Dialogue\\{ModEntry.LCInternalName}:" +
                        "fall_16.fair.shopdialogue");
                    var menu = new ShopMenu(stock, 0, "default");
                    menu.portraitPerson = Game1.getCharacterFromName(ModEntry.LCInternalName);
                    menu.potraitPersonDialogue = Game1.parseText(
                            dialogue, Game1.dialogueFont, 304);
                    Game1.activeClickableMenu = menu;
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Monitor.Log($"Harmony patch failed in {nameof(Event__checkAction__Prefix)}:\n{e}",
                        LogLevel.Error);
                return true;
            }
        }

        /*
         * Prefix to use character-specific flower dance acceptance dialogue,
         * for non-spouse and spouse statuses.
         * Vanilla checks a hardcoded switch block of the original 10 singles
         * (everyone else gets generic text). There are dialogue strings for
         * Emily and Shane in Strings/Events, but the game doesn't use them.
         */
        public static bool Event__answerDialogueQuestion__Prefix(
                StardewValley.Event __instance,
                StardewValley.NPC who,
                string answerKey)
        {
            if (!__instance.isFestival) {
                return false;
            }
            if (!who.Name.Equals(ModEntry.LCInternalName)) {
                return true;
            }
            if (answerKey == "yes") {
                return true;
            }
            if (answerKey == "no" || answerKey != "danceAsk") {
                return false;
            }
            string responseText;
            if (who.Name.Equals(Game1.player.spouse)) {
                responseText = Game1.content.LoadString(
                        $"Characters\\Dialogue\\{who.Name}:danceAcceptSpouse");
            }
            else if (!who.HasPartnerForDance &&
                    Game1.player.getFriendshipLevelForNPC(who.Name) >= 1000 &&
                    !who.isMarried()) {
                responseText = Game1.content.LoadString(
                        $"Characters\\Dialogue\\{who.Name}:danceAccept");
            }
            else {
                return true;
            }
            Game1.player.dancePartner.Value = who;
            who.setNewDialogue(responseText);
            foreach (NPC j in __instance.actors) {
                if (j.CurrentDialogue != null && j.CurrentDialogue.Count > 0 &&
                        j.CurrentDialogue.Peek().getCurrentDialogue().Equals("...")) {
                    j.CurrentDialogue.Clear();
                }
            }
            Game1.drawDialogue(who);
            who.immediateSpeak = true;
            who.facePlayer(Game1.player);
            who.Halt();
            return false;
        }

        /*
         * Postfix patch for Event.command_viewport.
         * If the current map is Lacey's house interior, and the command was
         * of the form "viewport x y" or "viewport x y true", and the viewport
         * is large enough to fit the entire map, honor the command coordinates
         * instead of forcing the viewport to the center of the map.
         * (I submit this is the correct behavior on all maps, but I'm trying
         * not to break anything)
         */
        public static void Event__command_viewport__Postfix(
                StardewValley.Event __instance,
                GameLocation location,
                GameTime time,
                string[] split)
        {
            if (!Game1.currentLocation.Name.Equals("Custom_HatMouseLacey_MouseHouse")) {
                return;
            }
            /* just redoing the normal calculation and not doing the map size part */
            if (split.Length == 3 || (split.Length == 4 && split[3].Equals("true"))) {
                int tx = __instance.OffsetTileX(Convert.ToInt32(split[1]));
                int ty = __instance.OffsetTileX(Convert.ToInt32(split[2]));
                Game1.viewport.X = tx * 64 + 32 - Game1.viewport.Width / 2;
                Game1.viewport.Y = ty * 64 + 32 - Game1.viewport.Height / 2;
            }
        }

        /*
         * Prefix patch for Event.skipEvent.
         * In vanilla, any events with end behavior other than "end", or with
         * important things to do which rely on certain event commands to
         * execute (giving items or crafting recipes, setting flags, etc.),
         * have their ids hardcoded into a switch statement so that those tasks
         * can be done when skipped.
         *
         * This adds checks for Lacey's events. It is a prefix in order to run
         * the desired endBehaviors before the vanilla function, which defaults
         * to running simply "end".
         *
         * Except in one case (14 hearts), this does not avoid the default
         * function. This means exitEvent is called twice, and the rest of
         * event cleanup happens after endBehaviors instead of before.
         * This does not seem to cause any problems.
         */
        public static bool Event__skipEvent__Prefix(
                StardewValley.Event __instance)
        {
            if (__instance.id == 236750200) {
                if (!Game1.player.mailReceived.Contains($"{MailPrefix}HatReactions")) {
                    Game1.player.mailReceived.Add($"{MailPrefix}HatReactions");
                }
                Game1.player.addQuest(236750210);
                __instance.endBehaviors(new string[1]{"end"},
                        Game1.currentLocation);
            }
            else if (__instance.id == 236751001) {
                LCEventCommands.command_timeAfterFade(Game1.currentLocation,
                        Game1.currentGameTime,
                        new string[2]{"timeAfterFade", "2200"});
                __instance.endBehaviors(new string[2]{"end", "warpOut"},
                        Game1.currentLocation);
            }
            /* for this one, we have to skip the default function, since we
             * need to warp to the farmhouse and *then* use "end warpOut".
             * that takes time, so we can't let the base game run "end". */
            else if (__instance.id == 236751400 || __instance.id == 236751401) {
                LCEventCommands.command_timeAfterFade(Game1.currentLocation,
                        Game1.currentGameTime,
                        new string[2]{"timeAfterFade", "2100"});
                LocationRequest req = Game1.getLocationRequest("FarmHouse");
                /* save our current location. null out its event reference
                 * when the warp finishes */
                GameLocation skipLocation = Game1.currentLocation;
                req.OnLoad += delegate {
                    skipLocation.currentEvent = null;
                    Game1.currentLocation.currentEvent = __instance;
                    __instance.endBehaviors(new string[2]{"end", "warpOut"},
                            Game1.currentLocation);
                };
                Game1.warpFarmer(req, Game1.player.getTileX(),
                        Game1.player.getTileY(), Game1.player.FacingDirection);

                if (__instance.playerControlSequence) {
                    __instance.EndPlayerControlSequence();
                }
                Game1.playSound("drumkit6");
                /* reflection abuse */
                var apam = (Dictionary<string, Vector3>)__instance.GetType()
                        .GetField("actorPositionsAfterMove", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(__instance);
                apam.Clear();
                foreach (NPC i in __instance.actors) {
                    bool ignore_stop_animation = i.Sprite.ignoreStopAnimation;
                    i.Sprite.ignoreStopAnimation = true;
                    i.Halt();
                    i.Sprite.ignoreStopAnimation = ignore_stop_animation;
                    __instance.resetDialogueIfNecessary(i);
                }
                Game1.player.Halt();
                Game1.player.ignoreCollisions = false;
                Game1.exitActiveMenu();
                Game1.dialogueUp = false;
                Game1.dialogueTyping = false;
                Game1.pauseTime = 0f;
                return false;
            }
            return true;
        }

        /*
         * Postfix patch Event.setupEventCommands to register new event
         * commands.
         * Checks LCEventCommands (EventCommands.cs) for public static methods
         * named "command_<name>", then registers event commands for them
         * called "HML_<name>" by adding them to Event._commandLookup.
         */
        public static void Event__setupEventCommands__Postfix(
                StardewValley.Event __instance)
        {
            /* more reflection abuse: _commandLookup is protected */
            var dict = (Dictionary<string, MethodInfo>)__instance.GetType()
                    .GetField("_commandLookup", BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(__instance);
            MethodInfo[] commands = typeof(LCEventCommands).GetMethods(
                    BindingFlags.Static | BindingFlags.Public);
            try {
                foreach (var info in commands) {
                    if (!info.Name.StartsWith("command_")) {
                        continue;
                    }
                    string key = info.Name.Replace("command_", "HML_");
                    if (!dict.ContainsKey(key)) {
                        dict.Add(key, info);
                        ModEntry.MONITOR.Log($"Registered event command '{key}'", LogLevel.Trace);
                    }
                }
            }
            catch {}
        }

        /*
         * Postfix patch Dialogue.parseDialogueString to add a new dialogue
         * command: $m.
         *      $m <mail id>#<text1>|<text2>
         * Display text1 if the given mail has been sent, or text2 otherwise.
         *
         * Works sort of like $d (world state), but checks a mail id. Only
         * switches the next command (# to #), instead of the whole dialogue
         * string like $d.
         */
        public static void Dialogue__parseDialogueString__Postfix(
                StardewValley.Dialogue __instance,
                string masterString)
        {
            /* the way the parsing works, if we find a dialogue that says
             * "$m <mail id>", the next one will be the two options separated
             * by a "|" character.
             * we'll check the mail id, choose the correct half, then remove
             * the $m message. */
            for (int i = 0; i < __instance.dialogues.Count - 1; ++i) {
                string command = __instance.dialogues[i];
                if (!command.StartsWith("$m ") || command.Length <= 3) {
                    continue;
                }
                string mailId = command.Substring(3);
                string[] options = __instance.dialogues[i+1].Split('|');
                /* put the '{' at the end of text1 if text2 has one. this lets
                 * us continue with #$b# */
                if (options.Length >= 2 && options[1].EndsWith("{")) {
                    options[0] += "{";
                }
                if (options.Length == 1) {
                    Monitor.Log($"WARNING: couldn't find '|' separator in $m dialogue command",
                            LogLevel.Warn);
                }
                else if (Game1.player.hasOrWillReceiveMail(mailId)) {
                    __instance.dialogues[i+1] = options[0];
                }
                else {
                    __instance.dialogues[i+1] = options[1];
                }
                __instance.dialogues.Remove(command);
            }
        }

        /*
         * This patch is huge and ugly but makes a very small change: fix
         * the breathing animation. The default code calculates a position
         * based on some heuristics which don't work for Lacey (she's too
         * short). 
         * If this NPC isn't Lacey, run the original code. Otherwise, repro-
         * duce it all here but change the Breather code to use a specific
         * rectangle instead of the calculated one.
         *
         * All of the code was taken from a decompilation of NPC.cs, so it
         * expects to be executed as a member function of NPC. This means we
         * have to mangle a lot of it to access things from outside, including
         * reflection abuse to access the protected 'shakeTimer'.
         *
         * TODO change this to a transpile in the future
         */
        public static bool NPC__draw__Prefix(
                StardewValley.NPC __instance,
                SpriteBatch b, float alpha = 1f)
        {
            try
            {
                if (!__instance.Name.Equals(ModEntry.LCInternalName)) {
                    return true;
                }
                if (__instance.Sprite == null || __instance.IsInvisible ||
                        (!Utility.isOnScreen(__instance.Position, 128) &&
                        (!__instance.eventActor || !(__instance.currentLocation is Summit)))) {
                    return false;
                }
                int shakeTimer = (int)__instance.GetType()
                            .GetField("shakeTimer", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(__instance);
                bool swimming = __instance.swimming.Value;
                Vector2 shakeOffset = (shakeTimer > 0 ?
                        new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) :
                        Vector2.Zero);
                if (swimming) {
                    b.Draw(__instance.Sprite.Texture,
                            __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, 80 + __instance.yJumpOffset * 2) + shakeOffset - new Vector2(0f, __instance.yOffset),
                            new Microsoft.Xna.Framework.Rectangle(__instance.Sprite.SourceRect.X, __instance.Sprite.SourceRect.Y, __instance.Sprite.SourceRect.Width, __instance.Sprite.SourceRect.Height / 2 - (int)(__instance.yOffset / 4f)),
                            Color.White,
                            __instance.rotation,
                            new Vector2(32f, 96f) / 4f,
                            Math.Max(0.2f, __instance.Scale) * 4f,
                            __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                            Math.Max(0f, __instance.drawOnTop ? 0.991f : ((float)__instance.getStandingY() / 10000f)));
                    Vector2 localPosition = __instance.getLocalPosition(Game1.viewport);
                    b.Draw(Game1.staminaRect,
                            new Microsoft.Xna.Framework.Rectangle((int)localPosition.X + (int)__instance.yOffset + 8, (int)localPosition.Y - 128 + __instance.Sprite.SourceRect.Height * 4 + 48 + __instance.yJumpOffset * 2 - (int)__instance.yOffset, __instance.Sprite.SourceRect.Width * 4 - (int)__instance.yOffset * 2 - 16, 4),
                            Game1.staminaRect.Bounds,
                            Color.White * 0.75f,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            (float)__instance.getStandingY() / 10000f + 0.001f);
                }
                else {
                    b.Draw(__instance.Sprite.Texture,
                            __instance.getLocalPosition(Game1.viewport) + new Vector2(__instance.GetSpriteWidthForPositioning() * 4 / 2, __instance.GetBoundingBox().Height / 2) + shakeOffset,
                            __instance.Sprite.SourceRect,
                            Color.White * alpha,
                            __instance.rotation,
                            new Vector2(__instance.Sprite.SpriteWidth / 2, (float)__instance.Sprite.SpriteHeight * 3f / 4f),
                            Math.Max(0.2f, __instance.Scale) * 4f,
                            (__instance.flip || (__instance.Sprite.CurrentAnimation != null && __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                            Math.Max(0f, __instance.drawOnTop ? 0.991f : ((float)__instance.getStandingY() / 10000f)));
                }
                /*
                 * This is the breathing section with the chestBox code. The
                 * numbers are very difficult to get right, because of sprite
                 * coordinates (1x) vs. screen coordinates (4x), and the
                 * Vector2 origin parameter in the draw call which shifts the
                 * origin of the drawn sprite (affects rotation, flip, and
                 * position).
                 * Also, getLocalPosition's return value doesn't seem to be
                 * anchored to a useful spot, so it's mostly trial and error.
                 */
                if (__instance.Breather && shakeTimer <= 0 && !swimming &&
                        __instance.Sprite.currentFrame < 16 &&
                        !__instance.farmerPassesThrough) {
                    Microsoft.Xna.Framework.Rectangle chestBox = __instance.Sprite.SourceRect;
                    /* sprite coordinates */
                    chestBox.X += 5;
                    chestBox.Y += 25;
                    chestBox.Width = 6;
                    chestBox.Height = 3;
                    Vector2 chestCenter = new Vector2(chestBox.Width/2, chestBox.Height/2+1);
                    /* screen coordinates */
                    Vector2 chestPosition = new Vector2(8*4, 7*4);

                    float breathScale = Math.Max(0f, (float)Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double)(__instance.DefaultPosition.X * 20f))) / 4f);
                    b.Draw(__instance.Sprite.Texture,
                            __instance.getLocalPosition(Game1.viewport) + chestPosition,
                            chestBox,
                            Color.White * alpha,
                            __instance.rotation,
                            chestCenter,
                            Math.Max(0.2f, __instance.Scale) * 4f + breathScale,
                            __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                            Math.Max(0f, __instance.drawOnTop ? 0.992f : ((float)__instance.getStandingY() / 10000f + 0.001f)));
                }
                if (__instance.isGlowing) {
                    b.Draw(__instance.Sprite.Texture,
                            __instance.getLocalPosition(Game1.viewport) + new Vector2(__instance.GetSpriteWidthForPositioning() * 4 / 2, __instance.GetBoundingBox().Height / 2) + shakeOffset,
                            __instance.Sprite.SourceRect,
                            __instance.glowingColor * __instance.glowingTransparency,
                            __instance.rotation,
                            new Vector2(__instance.Sprite.SpriteWidth / 2, (float)__instance.Sprite.SpriteHeight * 3f / 4f),
                            Math.Max(0.2f, __instance.Scale) * 4f,
                            __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                            Math.Max(0f, __instance.drawOnTop ? 0.99f : ((float)__instance.getStandingY() / 10000f + 0.001f)));
                }
                if (__instance.isEmoting && !Game1.eventUp) {
                    /* since Lacey is so short, we also place her emotes
                     * lower (18 instead of SpriteHeight) */
                    Vector2 emotePosition = __instance.getLocalPosition(Game1.viewport);
                    emotePosition.Y -= (32 + 18 * 4);
                    b.Draw(Game1.emoteSpriteSheet,
                            emotePosition,
                            new Microsoft.Xna.Framework.Rectangle(__instance.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, __instance.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            (float)__instance.getStandingY() / 10000f);
                }
                return false;
            }
            catch(Exception e) {
                Monitor.Log($"Harmony patch failed in NPC__draw__Prefix:\n{e}",
                        LogLevel.Error);
                return true;
            }
        }

        internal static Farmer getFarmerFromUniqueMultiplayerID(long id)
        {
            if (!Game1.IsMultiplayer) {
                return Game1.player;
            }
            if (Game1.serverHost.Value != null) {
                return Game1.serverHost;
            }
            return (from f in Game1.otherFarmers.Values
                    where f.UniqueMultiplayerID == id
                    select f).ElementAt(0);
        }

        /*
         * Loads mouse-child graphics for babies and toddlers if the conditions
         * are right (biological children with Lacey).
         * This is done by checking a custom field on the Child's modData, and
         * writing that field if not already set.
         */
        public static void Characters_Child__reloadSprite__Postfix(
                Child __instance)
        {
            string lc = ModEntry.LCInternalName;
            string variant;
            if (!__instance.modData.TryGetValue($"{lc}/ChildVariant", out variant) ||
                    variant is null) {
                /*
                 * the Child already has idOfParent set to the parent farmer's
                 * uniqueMultiplayerID. find that farmer and check the spouse.
                 */
                Farmer parent = getFarmerFromUniqueMultiplayerID(__instance.idOfParent.Value);
                if (parent is null) {
                    Monitor.Log($"Found child {__instance.Name} with missing parent.",
                            LogLevel.Warn);
                    return;
                }
                NPC l = parent.getSpouse();
                /* I don't like dumping out here, but on a normal load this
                 * postfix runs three times, and the first time spouse is null.
                 * so I can't save the -1 value yet */
                if (l is null) {
                    Monitor.Log($"Spouse missing for unsaved child {__instance.Name}",
                            LogLevel.Warn);
                    return;
                }
                variant = "-1";
                if (l.Name.Equals(lc) && !l.isGaySpouse()) {
                    variant = "0";
                    /* if darkSkinned is set (50% for dark farmers), use brown
                     * mouse child. otherwise, pick one randomly. */
                    if (__instance.darkSkinned.Value) {
                        variant = "1";
                    }
                    else if (Game1.random.NextDouble() < 0.33) {
                        variant = "1";
                    }
                }
                __instance.modData[$"{lc}/ChildVariant"] = variant;
            }
            if (variant == "-1") {
                return;
            }
            /* only need to set the name. the other fields are already handled */
            if (__instance.Age >= 3) {
                __instance.Sprite.textureName.Value = $"Characters\\{lc}\\Toddler_" +
                        $"{(__instance.Gender == 0 ? "boy" : "girl")}_" +
                        $"{variant}";
            }
            else {
                __instance.Sprite.textureName.Value = $"Characters\\{lc}\\Baby_{variant}";
            }
        }

        /*
         * Stop Lacey from going to the island by returning blanket false for
         * the island sanity checker.
         *
         * I don't really want to stop her, but the schedule that the island
         * hijacking process creates starts with "a1150 IslandSouth", and
         * Lacey can't path directly through the forest without a waypoint,
         * so that would cause her to lock up on the forest bridges.
         *
         * So for now, just prevent it.
         */
        public static bool Locations_IslandSouth__CanVisitIslandToday__Prefix(
                StardewValley.Locations.IslandSouth __instance,
                StardewValley.NPC npc,
                ref bool __result)
        {
            if (npc.Name.Equals(ModEntry.LCInternalName)) {
                __result = false;
                return false;
            }
            return true;
        }

        /*
         * Make TerrainFeatures.Grass honor the "isTemporarilyInvisible" flag.
         * This is set by the "makeInvisible" event command, which I use only
         * for SVE compatibility in the picnic event (as a stopgap until I
         * implement the temporary location version).
         */
        public static bool TerrainFeatures_Grass__draw__Prefix(
                StardewValley.TerrainFeatures.Grass __instance,
                SpriteBatch spriteBatch,
                Vector2 tileLocation)
        {
            if (__instance.isTemporarilyInvisible) {
                return false;
            }
            return true;
        }

    }
}
