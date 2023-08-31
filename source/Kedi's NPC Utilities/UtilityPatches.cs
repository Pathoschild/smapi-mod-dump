/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/NPCUtilities
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Network;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Characters;

namespace KediNPCUtilities 
{
    public class UtilityPatches
    {
        public static bool CheckNPCUtility(string feature, string NPCname)
        {
            if (ModEntry.UtilityData.ContainsKey(NPCname))
            {
                foreach (var item in ModEntry.UtilityData[NPCname])
                    if (item.Key == feature)
                        return true;
            }
            return false;
        }
        public static int VanillaWayOfFrame(string name) //Why. Just why. <.< -- Yeet this for 1.6
        {
            return name switch
            {
                "Emily" or "Abigail" => 33,
                "Alex" => 42,
                "Elliott" or "Penny" => 35,
                "Harvey" => 31,
                "Leah" => 25,
                "Sam" => 36,
                "Sebastian" => 40,
                "Shane" => 34,
                "Krobus" => 16,
                _ => 28,
            };
        }
        public static int DirectionBasedOnSpouseName(string name) //<.< -- Yeet this for 1.6
        {
            return name switch
            {
                "Abigail" or "Emily" or "Elliott" or "Harvey" or "Maru" or "Sebastian" or "Shane" or "Krobus" => 3,
                _ => 1,
            };
        }
        public static bool tryToReceiveActiveObject_Prefix(Farmer who, NPC __instance)
        {
            string itemID = who.ActiveObject.ParentSheetIndex.ToString();
            var trim = StringSplitOptions.TrimEntries;
            bool isCustom = CheckNPCUtility("marriageItem", __instance.Name) ? ModEntry.UtilityData[__instance.Name]["marriageItem"].Split(",", trim).Contains(itemID) : false;
            if ((itemID == "460" || isCustom) && CheckNPCUtility("datableNotMarriable", __instance.Name))
            {
                if (who.friendshipData.TryGetValue(__instance.Name, out Friendship friendship) && friendship?.Status == FriendshipStatus.Dating)
                {
                    who.changeFriendship(-20, __instance);
                    if (!who.friendshipData[__instance.Name].ProposalRejected)
                    {
                        __instance.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3973"), __instance));
                        who.friendshipData[__instance.Name].ProposalRejected = true;
                    }
                    else
                    {
                        __instance.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.LoadStringByGender(__instance.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3974") : Game1.LoadStringByGender(__instance.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3975"), __instance));
                        who.changeFriendship(-30, __instance);
                    }
                    Game1.drawDialogue(__instance);
                    return false;
                }
            }
            else if (who.friendshipData.TryGetValue(__instance.Name, out Friendship friendship) && friendship is not null && __instance.Age != 2 && __instance is not Child) //Stay away from children, creepers!
            {
                if (CheckNPCUtility("marriageItem", __instance.Name) || CheckNPCUtility("dateItem", __instance.Name) || CheckNPCUtility("breakupItem", __instance.Name) || CheckNPCUtility("divorceItem", __instance.Name))
                {
                    if (ModEntry.UtilityData[__instance.Name]["marriageItem"].Split(",", trim).Contains(itemID) && !CheckNPCUtility("datableNotMarriable", __instance.Name) && __instance.datable.Value)
                    {
                        var method = ModEntry.Helper.Reflection.GetMethod(__instance, "engagementResponse");
                        method.Invoke(new object[2] { who, false});
                        return false;
                    }
                    else if (ModEntry.UtilityData[__instance.Name]["dateItem"].Split(",", trim).Contains(itemID) && __instance.datable.Value)
                    {
                        Game1.player.friendshipData[__instance.Name].Status = FriendshipStatus.Dating;
                        __instance.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.LoadStringByGender(__instance.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3962") : Game1.LoadStringByGender(__instance.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3963"), __instance));
                        who.changeFriendship(25, __instance);
                        who.reduceActiveItemByOne();
                        who.completelyStopAnimatingOrDoingAction();
                        __instance.doEmote(28);
                        Game1.drawDialogue(__instance);
                        who.reduceActiveItemByOne();
                        return false;
                    }
                    else if (ModEntry.UtilityData[__instance.Name]["breakupItem"].Split(",", trim).Contains(itemID))
                    {                   
                        who.completelyStopAnimatingOrDoingAction();
                        who.friendshipData[__instance.Name].Points = Math.Min(who.friendshipData[__instance.Name].Points, 1250);
                        who.friendshipData[__instance.Name].Status = FriendshipStatus.Friendly;
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Effect", __instance.displayName));
                        __instance.doEmote(Game1.random.NextDouble() > 0.5 ? 20 : 12);
                        __instance.CurrentDialogue.Clear();
                        __instance.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Characters\\Dialogue\\" + __instance.GetDialogueSheetName() + ":breakUp"), __instance));
                        Game1.drawDialogue(__instance);
                        who.reduceActiveItemByOne();
                        return false;
                    }
                    else if (ModEntry.UtilityData[__instance.Name]["divorceItem"].Split(",", trim).Contains(itemID))
                    {
                        who.divorceTonight.Value = true;
                        if (Game1.player.Money >= 50000)
                            Game1.player.Money -= 50000;
                        else
                        {
                            if (!Game1.player.modData.ContainsKey("KediDili.KNU.divorceDebt"))
                                Game1.player.modData.Add("KediDili.KNU.divorceDebt", (50000 - Game1.player.Money).ToString());

                            else if (Game1.player.modData["KediDili.KNU.divorceDebt"] == "0")
                                Game1.player.modData["KediDili.KNU.divorceDebt"] = (50000 - Game1.player.Money).ToString();

                            else if (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) > 0)
                                Game1.player.modData["KediDili.KNU.divorceDebt"] = (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) + (50000 - Game1.player.Money)).ToString();

                            Game1.player.Money = 0;
                        }
                        string s = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:ManorHouse_DivorceBook_Filed");
                        Game1.drawObjectDialogue(s);
                        who.reduceActiveItemByOne();
                        return false;
                    }
                }
            }
            return true;
        }
        public static void isGaySpouse_Postfix(NPC __instance, ref bool __result) 
        {
            if (CheckNPCUtility("alwaysAdopt", __instance.Name))
            {
                __result = true;
                return;
            }
            else if (CheckNPCUtility("alwaysPregnant", __instance.Name))
            {
                __result = false;
                return;
            }
        }
        public static void canGetPregnant_Postfix(NPC __instance, ref bool __result)
        {
            if (CheckNPCUtility("noChildren", __instance.Name))
            {
                __result = false;
                return;
            }
            else if (CheckNPCUtility("alwaysPregnant", __instance.Name) || CheckNPCUtility("alwaysAdopt", __instance.Name))
            {
                __result = true;
                return;
            }
        }
        public static bool checkAction_Prefix(Farmer who, GameLocation l, NPC __instance, ref bool __result)
        {
            if (CheckNPCUtility("sidedInteractionFrame", __instance.Name))
            {
                if (__instance.Sprite.CurrentAnimation == null && !__instance.hasTemporaryMessageAvailable() && __instance.currentMarriageDialogue.Count == 0 && __instance.CurrentDialogue.Count == 0 && Game1.timeOfDay < 2200 && !__instance.isMoving() && who.ActiveObject == null)
                {
                    if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                    {
                        int spouseFrame = 0;

                        __instance.faceGeneralDirection(who.getStandingPosition(), 0, false, false);
                        who.faceGeneralDirection(__instance.getStandingPosition(), 0, false, false);

                        if (__instance.FacingDirection == DirectionBasedOnSpouseName(__instance.Name))
                            spouseFrame = VanillaWayOfFrame(__instance.Name);
                        else
                            spouseFrame = Convert.ToInt32(ModEntry.UtilityData[__instance.Name]["sidedInteractionFrame"]);

                        if (who.getFriendshipHeartLevelForNPC(__instance.Name) > 9 && __instance.sleptInBed.Value)
                        {
                            int delay = __instance.movementPause = Game1.IsMultiplayer ? 1000 : 10;
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                 new FarmerSprite.AnimationFrame(spouseFrame, delay, secondaryArm: false, false, __instance.haltMe, behaviorAtEndOfFrame: true)
                            });
                        }
                        if (!__instance.hasBeenKissedToday.Value)
                        {
                            who.changeFriendship(10, __instance);
                            l.playSound("dwop", NetAudio.SoundContext.NPC);
                            who.exhausted.Value = false;

                            __instance.hasBeenKissedToday.Value = true;
                            __instance.Sprite.UpdateSourceRect();
                        }
                        else
                        {
                            __instance.faceDirection((Game1.random.NextDouble() < 0.5) ? 2 : 0);
                            __instance.doEmote(12);
                        }
                        int playerFaceDirection = 1;
                        if (__instance.FacingDirection == 1)
                        {
                            playerFaceDirection = 3;
                        }
                        who.PerformKiss(playerFaceDirection);
                        __result = true;
                        return false;
                    }
                }
            }
            return true;
        }
        public static void setUp_Postfix(ref bool __result, ref bool ___isMale, ref string ___message)
        {
            if (CheckNPCUtility("alwaysPregnant", Game1.player.spouse) || CheckNPCUtility("alwaysAdopt", Game1.player.spouse))
            {
                Random r = new((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
                NPC spouse = Game1.getCharacterFromName(Game1.player.spouse);
                Game1.player.CanMove = false;

                ___isMale = Game1.player.getNumberOfChildren() == 0 ? (r.NextDouble() < 0.5) : (Game1.player.getChildren()[0].Gender == 1);
                ___message = Game1.content.LoadString("Strings\\Events:BirthMessage_" + (CheckNPCUtility("alwaysAdopt", Game1.player.spouse) ? "Adoption" : "SpouseMother"), Lexicon.getGenderedChildTerm(___isMale), spouse.displayName);

                __result = false;
                return;
            }
        }
        public static void shouldPortraitShake_Postfix(DialogueBox __instance, ref bool __result, Dialogue d)
        {
            if (CheckNPCUtility("shakePortraits", d.speaker.Name))
            {
                int view = d.getPortraitIndex();
                string[] indexes = ModEntry.UtilityData[d.speaker.Name]["shakePortraits"].Split(",", StringSplitOptions.TrimEntries);
                __result = indexes.ToList().Contains(view.ToString());
                if (__result is true && __instance.newPortaitShakeTimer == 0 && !ModEntry.isShakedAlready)
                {
                    __instance.newPortaitShakeTimer = 250;
                    ModEntry.isShakedAlready = true;
                }
                return;
            }
        }
    }
}
/*

datableNotMarriable,    //Will always reject mermaid pendant
noChildren,             //Will never ask for children
sidedInteractionFrame,  //Different right and left kiss/hug frames
alwaysPregnant,         //This NPC should be always the pregnant one, regardless of player's gender.
alwaysAdopt,            //All farmers married to this NPC will only be able to adopt children, regardless of farmer's gender.
shakePortraits,          //Shakes portraits with specified indexes when they are displayed

 * Keep in mind that some keys are reserved for spesific purposes:
 * marriageProposal
 * platonicProposal
 * dateProposal
 * breakupProposal
 * divorceProposal
 * 
 * marriageItem
 * platonicItem
 * dateItem
 * breakupItem
 * divorceItem
 * 
 * datableNotMarriable
 * noChildren
 * sidedInteractionFrame
 * alwaysPregnant
 * alwaysAdopt
 * shakePortraits
 */
