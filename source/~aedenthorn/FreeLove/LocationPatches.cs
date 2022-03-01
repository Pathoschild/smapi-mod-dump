/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace FreeLove
{
    public static class LocationPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;
        private static IModHelper Helper;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }
        public static void FarmHouse_GetSpouseBed_Postfix(FarmHouse __instance, ref BedFurniture __result)
        {
            try
            {

                if (!Config.EnableMod || __result != null)
                    return;
                __result = __instance.GetBed(BedFurniture.BedType.Double, 0);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(FarmHouse_GetSpouseBed_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
        
        public static bool FarmHouse_getSpouseBedSpot_Prefix(FarmHouse __instance, string spouseName, ref Point __result)
        {
            try
            {
                if (spouseName == null || !Config.EnableMod)
                    return true;
                var spouses = Misc.GetSpouses(__instance.owner, true);
                
                if (!spouses.TryGetValue(spouseName, out NPC spouse) || spouse is null || spouse.isMoving() || !Misc.IsInBed(__instance, spouse.GetBoundingBox()))
                    return true;

                __result = spouse.getTileLocationPoint();
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(FarmHouse_GetSpouseBed_Postfix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        public static void Beach_resetLocalState_Postfix(Beach __instance)
        {
            try
            {

                if (ModEntry.Config.BuyPendantsAnytime)
                {
                    ModEntry.PHelper.Reflection.GetField<NPC>(__instance, "oldMariner").SetValue(new NPC(new AnimatedSprite("Characters\\Mariner", 0, 16, 32), new Vector2(80f, 5f) * 64f, 2, "Old Mariner", null));
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Beach_resetLocalState_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static bool Beach_checkAction_Prefix(Beach __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result, NPC ___oldMariner)
        {
            try
            {
                if (___oldMariner != null && ___oldMariner.getTileX() == tileLocation.X && ___oldMariner.getTileY() == tileLocation.Y)
                {
                    string playerTerm = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (who.IsMale ? "Male" : "Female"));
                    if (who.hasAFriendWithHeartLevel(10, true) && who.HouseUpgradeLevel == 0)
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNotUpgradedHouse", playerTerm)));
                    }
                    else if (who.hasAFriendWithHeartLevel(10, true))
                    {
                        Response[] answers = new Response[]
                        {
                            new Response("Buy", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerYes")),
                            new Response("Not", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerNo"))
                        };
                        __instance.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_Question", playerTerm)), answers, "mariner");
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNoRelationship", playerTerm)));
                    }
                    __result = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Beach_checkAction_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

   
        public static void GameLocation_checkEventPrecondition_Prefix(ref string precondition)
        {
            try
            {
                if (precondition == null || precondition == "")
                    return;
                string[] split = precondition.Split('/');
                if (split.Length == 0)
                    return;
                int eventId;
                if (!int.TryParse(split[0], out eventId))
                {
                    return;
                }
                if (Game1.player.eventsSeen.Contains(eventId))
                {
                    return;
                }
                Dictionary<string, NPC> spouses = Misc.GetSpouses(Game1.player, true);
                for (int i = 1; i < split.Length; i++)
                {
                    if (split[i].Length == 0)
                        continue;

                    if (split[i][0] == 'O')
                    {
                        string name = split[i].Substring(2);
                        if (Game1.player.spouse != name && spouses.ContainsKey(name))
                        {
                            Monitor.Log($"Got unofficial spouse requirement for event: {name}, switching event condition to isSpouse O");
                            split[i] = $"o {name}";
                        }
                    }
                    else if (split[i][0] == 'o')
                    {
                        string name = split[i].Substring(2);
                        if (Game1.player.spouse != name && spouses.ContainsKey(name))
                        {
                            Monitor.Log($"Got unofficial spouse barrier to event: {name}, switching event condition to notSpouse o");
                            split[i] = $"O {name}";
                        }
                    }
                }
                precondition = string.Join("/", split);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GameLocation_checkEventPrecondition_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }


        public static void Desert_getDesertMerchantTradeStock_Postfix(Farmer who, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                if (who != null && who.getFriendshipHeartLevelForNPC("Krobus") >= 10 && !who.friendshipData["Krobus"].RoommateMarriage && who.HouseUpgradeLevel >= 1 && (who.isMarried() || who.isEngaged()) && !who.hasItemInInventory(808, 1, 0))
                {
                    ISalable i = new StardewValley.Object(808, 1, false, -1, 0);
                    __result.Add(i, new int[]
                    {
                        0,
                        1,
                        769,
                        200
                    });
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Desert_getDesertMerchantTradeStock_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }


        public static bool ManorHouse_performAction_Prefix(ManorHouse __instance, string action, Farmer who, ref bool __result)
        {
            try
            {
                Misc.ResetSpouses(who);
                Dictionary<string, NPC> spouses = Misc.GetSpouses(who, true);
                if (action != null && who.IsLocalPlayer && !Game1.player.divorceTonight.Value && (Game1.player.isMarried() || spouses.Count > 0))
                {
                    string a = action.Split(new char[]
                    {
                    ' '
                    })[0];
                    if (a == "DivorceBook")
                    {
                        string str = Helper.Translation.Get("divorce_who");
                        List<Response> responses = new List<Response>();
                        foreach (NPC spouse in spouses.Values)
                        {
                            responses.Add(new Response(spouse.Name, spouse.displayName));
                        }
                        responses.Add(new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")));
                        __instance.createQuestionDialogue(str, responses.ToArray(), Divorce.afterDialogueBehavior);
                        //__instance.createQuestionDialogue(s2, responses.ToArray(), "divorce");
                        __result = true;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(ManorHouse_performAction_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        public static void GameLocation_answerDialogue_prefix(GameLocation __instance, Response answer)
        {
            try
            {
                if (answer.responseKey.StartsWith("divorce_"))
                    __instance.afterQuestion = Divorce.afterDialogueBehavior;

            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GameLocation_answerDialogue_prefix)}:\n{ex}", LogLevel.Error);
            }
        }
        public static bool GameLocation_answerDialogueAction_Prefix(string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            if (!Config.EnableMod || questionAndAnswer != "mariner_Buy")
                return true;

            if (Game1.player.Money < Config.PendantPrice)
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
            }
            else
            {
                Game1.player.Money -= Config.PendantPrice;
                Game1.player.addItemByMenuIfNecessary(new Object(460, 1, false, -1, 0)
                {
                    specialItem = true
                }, null);
                if (Game1.activeClickableMenu == null)
                {
                    Game1.player.holdUpItemThenMessage(new Object(460, 1, false, -1, 0), true);
                }
            }
            __result = true;
            return false;
        }

    }
}