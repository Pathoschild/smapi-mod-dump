/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using QuestEssentials.Messages;
using QuestEssentials.Quests;
using QuestFramework.Extensions;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Linq;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace QuestEssentials.Framework
{
    internal static class Patches
    {
        public static void Before_receiveLeftClick(ShopMenu __instance, int x, int y, int ___sellPercentage)
        {
            try
            {
                if (Game1.activeClickableMenu == null)
                    return;

                if (__instance is ShopMenu)
                {
                    if (__instance.heldItem == null && __instance.onSell == null)
                    {
                        Item itemToSell = __instance.inventory.leftClick(x, y, null, false);

                        if (itemToSell != null)
                        {
                            int price = CalculatePrice(___sellPercentage, itemToSell);

                            QuestCheckers.CheckSellQuests(itemToSell, price);
                            __instance.inventory.leftClick(x, y, itemToSell, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QuestEssentialsMod.ModMonitor
                    .Log(
                        $"Error in {nameof(Before_receiveLeftClick)} harmony patch: {ex}",
                        LogLevel.Error
                    );
            }
        }

        public static void Before_receiveRightClick(ShopMenu __instance, int x, int y, int ___sellPercentage)
        {
            try
            {
                if (Game1.activeClickableMenu == null)
                    return;

                if (__instance is ShopMenu)
                {
                    if (__instance.heldItem == null && __instance.onSell == null)
                    {
                        Item itemToSell = __instance.inventory.rightClick(x, y, null, false, false);

                        if (itemToSell != null)
                        {
                            int price = CalculatePrice(___sellPercentage, itemToSell);

                            QuestCheckers.CheckSellQuests(itemToSell, price);

                            if (itemToSell.Stack == 1)
                                __instance.inventory.leftClick(x, y, itemToSell, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QuestEssentialsMod.ModMonitor
                    .Log(
                        $"Error in {nameof(Before_receiveRightClick)} harmony patch: {ex}",
                        LogLevel.Error
                    );
            }
        }

        public static void After_hasTemporaryMessageAvailable(NPC __instance, ref bool __result)
        {
            bool ActiveQuestTalkFilter(TalkQuest q) => q.IsInQuestLog() 
                && !q.GetInQuestLog().ShouldDisplayAsComplete() 
                && q.TalkTo == __instance.Name;

            if (QuestEssentialsMod.QuestApi.GetAllManagedQuests<TalkQuest>().Any(ActiveQuestTalkFilter))
            {
                __result = true;
            }
        }

        private static int CalculatePrice(int sellPercentage, Item itemToSell)
        {
            int itemPrice = (int)(itemToSell is StardewValley.Object objToSell 
                ? (objToSell.sellToStorePrice(-1L) * sellPercentage) 
                : (float)(itemToSell.salePrice() / 2) * sellPercentage);

            return itemPrice * itemToSell.Stack;
        }

        public static void Before_set_Money(Farmer __instance, int value)
        {
            int oldMoney = __instance._money;

            if (value <= oldMoney)
                return;

            QuestCheckers.CheckEarnQuests(value - oldMoney);
        }

        public static bool Before_tryToReceiveActiveObject(NPC __instance, Farmer who)
        {
            if (__instance.Name.Equals("Henchman") && Game1.currentLocation.Name.Equals("WitchSwamp"))
                return true;

            if (QuestEssentialsMod.QuestApi.CheckForQuestComplete(new DeliverMessage(who, __instance, who.ActiveObject)))
            {
                if (who.ActiveObject == null || who.ActiveObject.Stack <= 0)
                {
                    if (who.ActiveObject != null)
                    {
                        who.ActiveObject = null;
                    }

                    who.showNotCarrying();
                }

                return false;
            }

            return true;
        }

        public static void After_playerCaughtFishEndFunction(FishingRod __instance, Farmer ___lastUser, int ___whichFish, int ___fishQuality, string ___itemCategory)
        {
            if (___lastUser.IsLocalPlayer && !Game1.isFestival() && !__instance.fromFishPond)
            {
                if (___itemCategory == "Object")
                {
                    SObject fish = new SObject(___whichFish, 1, isRecipe: false, -1, ___fishQuality);
                    if (___whichFish == GameLocation.CAROLINES_NECKLACE_ITEM)
                    {
                        fish.questItem.Value = true;
                    }
                    if (___whichFish == 79 || ___whichFish == 842)
                    {
                        fish = ___lastUser.currentLocation.tryToCreateUnseenSecretNote(___lastUser);
                        if (fish == null)
                        {
                            return;
                        }
                    }
                    if (__instance.caughtDoubleFish)
                    {
                        fish.Stack = 2;
                    }

                    QuestEssentialsMod.QuestApi.CheckForQuestComplete(new FishMessage(___lastUser, fish));
                }
            }
        }

        public static void Before_checkForAction(CrabPot __instance, Farmer who, bool justCheckingForActivity)
        {
            if (__instance.tileIndexToShow == 714 && !justCheckingForActivity && __instance.heldObject.Value != null)
            {
                QuestEssentialsMod.QuestApi.CheckForQuestComplete(new FishMessage(who, __instance.heldObject.Value));
            }
        }

        public static void After_onGiftGiven(Farmer __instance, NPC npc, SObject item)
        {
            QuestEssentialsMod.QuestApi.CheckForQuestComplete(new GiftMessage(__instance, npc, item));
        }

        public static bool Before_LocationCheckAction(GameLocation __instance, Location tileLocation, Farmer who, ref bool __result)
        {
            bool isColligingWithEntity = __instance.isCollidingWithCharacter(new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64)) != null;
            
            if (!isColligingWithEntity && QuestEssentialsMod.QuestApi.CheckForQuestComplete<SpecialQuest>(new TileActionMessage(who, __instance, new Point(tileLocation.X, tileLocation.Y))))
            {
                __result = true;

                return false;
            }

            return true;
        }
    }
}
