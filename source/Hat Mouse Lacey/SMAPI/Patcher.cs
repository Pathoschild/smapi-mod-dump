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
     * The special construction "_nest_" between two parts of the class name
     * causes the reflection code to check nested types for that step, instead
     * of just descending namespaces.
     * (see e.g. Event_nest_DefaultCommands: DefaultCommands is a nested class
     * inside StardewValley.Event).
     *
     *
     * Why do this instead of using annotations?
     * I already had this class structure set up, and I prefer it to making
     * a new class for every patch.
     */
    internal class Patcher
    {

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
            if (Game1.mouseCursor > Game1.cursor_default) {
                return;
            }
            if (!who.hasOrWillReceiveMail($"{HML.MailPrefix}HatReactions")) {
                return;
            }
            NPC Lacey = Game1.currentLocation.isCharacterAtTile(tileLocation);
            if (Lacey != null && Lacey.Name.Equals(HML.LaceyInternalName)) {
                string hatstr = LCHatString.GetCurrentHatString(who);
                if (hatstr != null && !LCModData.HasShownHat(hatstr)) {
                    Game1.mouseCursor = Game1.cursor_talk;
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
         * Sadly, reinstate a patch to correct Lacey's emote positions.
         * There is a supported offset value in Data/Characters, but event
         * emotes and normal gameplay emotes use wildly different base
         * positions, so it's not possible to use one value that works well
         * for both.
         * This patch applies to NPC.DrawEmote, since that one can be easily
         * limited to work only for Lacey. The Data/Characters field is set
         * to a good value for the event emotes.
         */
        public static bool NPC__DrawEmote__Prefix(
                StardewValley.NPC __instance,
                SpriteBatch b)
        {
            if (!__instance.Name.Equals(HML.LaceyInternalName)) {
                return true;
            }
            if (!__instance.IsEmoting || Game1.eventUp) {
                return false;
            }
            Vector2 emotePosition = __instance.getLocalPosition(Game1.viewport);
            b.Draw(
                    position: new Vector2(emotePosition.X, emotePosition.Y - (float)(-20 + __instance.Sprite.SpriteHeight * 4)),
                    texture: Game1.emoteSpriteSheet,
                    sourceRectangle: new Microsoft.Xna.Framework.Rectangle(__instance.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, __instance.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: (float)__instance.StandingPixel.Y / 10000f
            );
            return false;
        }


        /*
         * Cause blocking checks on MapSeats to return false (not blocked)
         * during events in Lacey's house.
         * This is to cover an edge case: if Lacey is sitting on her couch
         * when the player starts her 10-heart event, the player won't be able
         * to sit on the couch during the event, since the real-world spot
         * is occupied (but the pocket dimension spot is free).
         */
        public static void MapSeat__IsBlocked__Postfix(
                StardewValley.MapSeat __instance,
                StardewValley.GameLocation location,
                ref bool __result)
        {
            if (Game1.eventUp && Game1.currentLocation.Name.Equals(
                    $"{HML.CPId}_MouseHouse")) {
                __result = false;
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
            if ((__instance.Name.Equals(HML.LaceyInternalName) &&
                    c.Name.Equals("Andy")) ||
                    (__instance.Name.Equals("Andy") &&
                    c.Name.Equals(HML.LaceyInternalName))) {
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
            if (!__instance.Name.Equals(HML.LaceyInternalName)) {
                return true;
            }
            if (!who.hasOrWillReceiveMail($"{HML.MailPrefix}HatReactions")) {
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
            string asset = $"Strings\\{HML.CPId}_HatReactions";

            Dialogue freshHat = Dialogue.FromTranslation(__instance,
                    $"{asset}:newHat");
            __instance.faceTowardFarmerForPeriod(4000, 4, faceAway: false, who);
            __instance.doEmote(32);
            who.currentLocation.playSound("give_gift", __instance.Tile);
            __instance.CurrentDialogue.Push(freshHat);
            Game1.drawDialogue(__instance);
            Game1.afterDialogues = delegate {
                int nowFacing = who.FacingDirection;
                Action turn = delegate {
                    who.faceDirection(++nowFacing % 4);
                };
                int turntime = 500;
                who.freezePause = 4*turntime+800;
                DelayedAction[] anims = new DelayedAction[5] {
                        new (turntime, turn),
                        new (2*turntime, turn),
                        new (3*turntime, turn),
                        new (4*turntime, turn),
                        new (4*turntime+600, delegate {
                            Dialogue reaction = Dialogue.TryGetDialogue(__instance,
                                    $"{asset}:{hatkey}");
                            if (reaction is null) {
                                reaction = Dialogue.FromTranslation(__instance,
                                    $"{asset}:404");
                            }
                            __instance.CurrentDialogue.Push(reaction);
                            Game1.drawDialogue(__instance);
                            Game1.player.changeFriendship(10, __instance);
                            who.completeQuest($"{HML.QuestPrefix}HatReactions");
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
         * Prefix NPC.tryToReceiveActiveObject to implement special bouquet
         * reaction dialogue.
         *
         * Character-specific bouquet reactions are vanilla behavior now, so
         * those are just dialogue keys. But Lacey keeps track of how mean you
         * have been to her in her heart events. If your score is high enough,
         * she'll reject you ("RejectBouquet_Cruel" and
         * "RejectBouquet_Cruel_Repeat"), unless you have seen the apology
         * event ("AcceptBouquet_Apologized").
         */
        public static bool NPC__tryToReceiveActiveObject__Prefix(
                StardewValley.NPC __instance,
                StardewValley.Farmer who,
                bool probe)
        {
            if (probe) {
                return true;
            }
            var obj = who.ActiveObject;
            if (!__instance.Name.Equals(HML.LaceyInternalName) ||
                    obj.QualifiedItemId != "(O)458") {
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
            if (friendship.Points < 2000) {
                return true;
            }

            who.Halt();
            who.faceGeneralDirection(__instance.getStandingPosition(),
                    0, opposite: false, useTileCalculations: false);
            string toLoad = $"Characters\\Dialogue\\{__instance.Name}:";
            bool accepted = false;
            bool addApologyQuest = false;

            if (Game1.player.hasOrWillReceiveMail($"{HML.MailPrefix}ApologyAccepted")) {
                toLoad += "AcceptBouquet_Apologized";
                accepted = true;
            }
            else if (Game1.player.hasOrWillReceiveMail($"{HML.MailPrefix}ApologySummons")) {
                toLoad += "RejectBouquet_Cruel_Repeat";
            }
            else if (LCModData.CrueltyScore >= 4) {
                toLoad += "RejectBouquet_Cruel";
                addApologyQuest = true;
            }
            else {
                return true;
            }

            if (accepted) {
                if (!friendship.IsDating()) {
                    friendship.Status = FriendshipStatus.Dating;
                    // more reflection abuse
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
            Dialogue response = Dialogue.FromTranslation(__instance, toLoad);
            __instance.CurrentDialogue.Push(response);
            Game1.drawDialogue(__instance);
            if (addApologyQuest) {
                Game1.afterDialogues = delegate {
                    Game1.addMailForTomorrow($"{HML.MailPrefix}ApologySummons");
                };
            }
            return false;
        }


        /*
         * Postfix patch for Event->DefaultCommands.Viewport.
         * If the current map is Lacey's house interior, and the command was
         * of the form "viewport x y" or "viewport x y true", and the viewport
         * is large enough to fit the entire map, honor the command coordinates
         * instead of forcing the viewport to the center of the map.
         * (I submit this is the correct behavior on all maps, but I'm trying
         * not to break anything)
         */
        public static void Event_nest_DefaultCommands__Viewport__Postfix(
                StardewValley.Event @event,
                string[] args,
                EventContext context)
        {
            if (!Game1.currentLocation.Name.Equals($"{HML.CPId}_MouseHouse")) {
                return;
            }
            // just redoing the normal calculation and not doing the map size part
            if (args.Length == 3 || (args.Length == 4 && args[3].Equals("true"))) {
                int tx = @event.OffsetTileX(Convert.ToInt32(args[1]));
                int ty = @event.OffsetTileX(Convert.ToInt32(args[2]));
                Game1.viewport.X = tx * 64 + 32 - Game1.viewport.Width / 2;
                Game1.viewport.Y = ty * 64 + 32 - Game1.viewport.Height / 2;
            }
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
         *
         * 1.6 now has a $query dialogue command which can do this by using
         * GSQs, but it works like $d. I might use it later on; for now I'm
         * leaving this in.
         */
        public static void Dialogue__parseDialogueString__Postfix(
                StardewValley.Dialogue __instance,
                string masterString,
                string translationKey)
        {
            // the way the parsing works, if we find a dialogue that says
            // "$m <mail id>", the next one will be the two options separated
            // by a "|" character.
            // we'll check the mail id, choose the correct half, then remove
            // the $m message.
            for (int i = 0; i < __instance.dialogues.Count - 1; ++i) {
                string command = __instance.dialogues[i].Text;
                if (!command.StartsWith("$m ") || command.Length <= 3) {
                    continue;
                }
                string mailId = command.Substring(3);
                string[] options = __instance.dialogues[i+1].Text.Split('|');
                // put the '{' at the end of text1 if text2 has one. this lets
                // us continue with #$b#
                if (options.Length >= 2 && options[1].EndsWith("{")) {
                    options[0] += "{";
                }
                if (options.Length == 1) {
                    Log.Warn($"WARNING: couldn't find '|' separator in" +
                           " $m dialogue command");
                }
                else if (Game1.player.hasOrWillReceiveMail(mailId)) {
                    __instance.dialogues[i+1].Text = options[0];
                }
                else {
                    __instance.dialogues[i+1].Text = options[1];
                }
                __instance.dialogues.RemoveAt(i);
            }
        }


        internal static Farmer getFarmerFromUniqueMultiplayerID(long id)
        {
            if (!Game1.IsMultiplayer) {
                return Game1.player;
            }
            if (Game1.serverHost.Value != null) {
                return Game1.serverHost.Value;
            }
            return (from f in Game1.otherFarmers.Values
                    where f.UniqueMultiplayerID == id
                    select f).ElementAt(0);
        }

        /*
         * Loads mouse-child graphics for babies and toddlers if the conditions
         * are right (biological children with Lacey).
         * This is done by checking a custom field on the Child's modData, and
         * writing that field if not already set. If the value is right, change
         * the texture name so the game loads our sprites.
         */
        public static void Characters_Child__reloadSprite__Postfix(
                Child __instance,
                bool onlyAppearance)
        {
            string variant;
            if (!__instance.modData.TryGetValue($"{HML.CPId}/ChildVariant", out variant) ||
                    variant is null) {
                // the Child already has idOfParent set to the parent farmer's
                // uniqueMultiplayerID. find that farmer and check the spouse.
                Farmer parent = getFarmerFromUniqueMultiplayerID(__instance.idOfParent.Value);
                if (parent is null) {
                    Log.Warn($"Found child {__instance.Name} with missing parent.");
                    return;
                }
                NPC l = parent.getSpouse();
                // I don't like dumping out here, but on a normal load this
                // postfix runs three times, and the first time spouse is null.
                // so I can't save the -1 value yet
                if (l is null) {
                    Log.Warn($"Spouse missing for unsaved child {__instance.Name}");
                    return;
                }
                variant = "-1";
                if (l.Name.Equals(HML.LaceyInternalName) && !l.isAdoptionSpouse()) {
                    variant = "0";
                    // if darkSkinned is set (50% for dark farmers), use brown
                    // mouse child. otherwise, pick one randomly.
                    if (__instance.darkSkinned.Value) {
                        variant = "1";
                    }
                    else if (Game1.random.NextDouble() < 0.33) {
                        variant = "1";
                    }
                }
                Log.Trace("Setting variant {variant} for child " +
                        $"'{__instance.Name ?? "(name not set)"}'");
                __instance.modData[$"{HML.CPId}/ChildVariant"] = variant;
            }
            if (variant == "-1") {
                return;
            }
            // only need to set the name. the other fields are already handled
            if (__instance.Age >= 3) {
                __instance.Sprite.textureName.Value = $"Characters\\{HML.CPId}\\Toddler_" +
                        $"{(__instance.Gender == 0 ? "boy" : "girl")}_" +
                        $"{variant}";
            }
            else {
                __instance.Sprite.textureName.Value = $"Characters\\{HML.CPId}\\Baby_{variant}";
            }
        }


        /*
         * Make TerrainFeatures.Grass honor the "isTemporarilyInvisible" flag.
         * This is set by the "makeInvisible" event command, which I use only
         * for SVE compatibility in the picnic event (as a stopgap until I
         * implement the temporary location version).
         */
        public static bool TerrainFeatures_Grass__draw__Prefix(
                StardewValley.TerrainFeatures.Grass __instance,
                SpriteBatch spriteBatch)
        {
            if (Game1.CurrentEvent != null &&
                    Game1.CurrentEvent.id.StartsWith($"{HML.EventPrefix}14Hearts") &&
                    __instance.isTemporarilyInvisible) {
                return false;
            }
            return true;
        }

    }
}
