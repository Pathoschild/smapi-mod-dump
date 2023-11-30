/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using BirbCore.Attributes;
using BirbShared;
using HarmonyLib;
using SpaceCore;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Quests;
using StardewValley.SpecialOrders.Rewards;

namespace SocializingSkill;

// Grant XP
[HarmonyPatch(typeof(Dialogue), nameof(Dialogue.chooseResponse))]
class Dialogue_ChooseResponse
{
    internal static void Prefix(
        Response response,
        Dialogue __instance,
        List<NPCDialogueResponse> ___playerResponses,
        bool ___quickResponse)
    {

        try
        {
            for (int i = 0; i < ___playerResponses.Count; i++)
            {
                if (___playerResponses[i].responseKey == null || response.responseKey == null || !___playerResponses[i].responseKey.Equals(response.responseKey))
                {
                    continue;
                }
                if (__instance.answerQuestionBehavior != null)
                {
                    return;
                }
                if (___quickResponse)
                {
                    return;
                }
                if (Game1.isFestival())
                {
                    return;
                }
                if (___playerResponses[i].friendshipChange <= 0)
                {
                    return;
                }
                Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromEvents);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// Smooth Talker Profession
//  - adjust friendship change during dialogue
[HarmonyPatch(typeof(NPCDialogueResponse), MethodType.Constructor, new Type[] { typeof(string), typeof(int), typeof(string), typeof(string) })]
class NPCDialogueResponse_Constructor
{

    internal static void Postfix(
        int friendshipChange,
        NPCDialogueResponse __instance)
    {
        try
        {
            if (Game1.player.HasProfession("SmoothTalker"))
            {
                if (Game1.player.HasProfession("SmoothTalker", true))
                {
                    if (friendshipChange < 0)
                    {
                        __instance.friendshipChange = 0;
                    }
                    else
                    {
                        __instance.friendshipChange = (int)(friendshipChange * ModEntry.Config.SmoothTalkerPositiveMultiplier * 1.5);
                    }
                }
                else
                {
                    if (friendshipChange < 0)
                    {
                        __instance.friendshipChange = (int)(friendshipChange * ModEntry.Config.SmoothTalkerNegativeMultiplier);
                    }
                    else
                    {
                        __instance.friendshipChange = (int)(friendshipChange * ModEntry.Config.SmoothTalkerPositiveMultiplier);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// Grant XP
// Smooth Talker Profession
//  - adjust friendship change during event
[HarmonyPatch(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.Friendship))]
class Event_CommandFriendship
{
    static void Postfix(
            string[] args)
    {
        try
        {
            if (Game1.player.HasProfession("SmoothTalker"))
            {
                NPC character = Game1.getCharacterFromName(args[1]);
                if (character == null)
                {
                    return;
                }

                int friendship = Convert.ToInt32(args[2]);

                // Undo original method friendship change
                Game1.player.changeFriendship(-friendship, character);

                if (friendship > 0)
                {
                    Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromEvents);
                }

                if (Game1.player.HasProfession("SmoothTalker", true))
                {
                    if (friendship < 0)
                    {
                        friendship = 0;
                    }
                    else
                    {
                        friendship = (int)(friendship * ModEntry.Config.SmoothTalkerPositiveMultiplier * 1.5);
                    }
                }
                else
                {
                    if (friendship < 0)
                    {
                        friendship = (int)(friendship * ModEntry.Config.SmoothTalkerNegativeMultiplier);
                    }
                    else
                    {
                        friendship = (int)(friendship * ModEntry.Config.SmoothTalkerPositiveMultiplier);
                    }
                }

                Game1.player.changeFriendship(friendship, character);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}


// Grant XP
[HarmonyPatch]
class Quest_CheckIfComplete
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(FishingQuest), nameof(FishingQuest.checkIfComplete));
        yield return AccessTools.Method(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.checkIfComplete));
        yield return AccessTools.Method(typeof(ResourceCollectionQuest), nameof(ResourceCollectionQuest.checkIfComplete));
        yield return AccessTools.Method(typeof(SlayMonsterQuest), nameof(SlayMonsterQuest.checkIfComplete));
    }

    static void Postfix(
        bool __result,
        NPC n)
    {
        try
        {
            if (!__result)
            {
                return;
            }
            if (n != null)
            {
                Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromQuests);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// Helpful Profession
//  - Increase quest rewards
[HarmonyPatch]
class Quest_GetMoneyReward
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Quest), nameof(Quest.GetMoneyReward));
        yield return AccessTools.Method(typeof(MoneyReward), nameof(MoneyReward.GetRewardMoneyAmount));
    }

    static void Postfix(ref int __result)
    {
        try
        {
            if (Game1.player.HasProfession("Helpful"))
            {
                if (Game1.player.HasProfession("Helpful", true))
                {
                    __result = (int)(__result * ModEntry.Config.HelpfulRewardMultiplier * 2);
                }
                else
                {
                    __result = (int)(__result * ModEntry.Config.HelpfulRewardMultiplier);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// TODO: other shop constructors
// Haggler Profession
//  - Decrease shop prices if friends with the owner
/*[HarmonyPatch(typeof(ShopMenu), MethodType.Constructor, new Type[] { typeof(string), typeof(ShopData), typeof(ShopOwnerData), typeof(NPC), typeof(Func<ISalable, Farmer, int, bool>), typeof(Func<ISalable, bool>), typeof(bool) })]
class ShopMenu_Constructor1
{
    internal static void Postfix(int currency, NPC owner, ShopMenu __instance)
    {
        try
        {
            if (!Game1.player.HasCustomProfession(SocializingSkill.Haggler))
            {
                return;
            }
            if (currency != 0)
            {
                return;
            }
            if (owner == null)
            {
                return;
            }
            if (owner.Name == null)
            {
                return;
            }
            if (!Game1.player.friendshipData.ContainsKey(owner.Name))
            {
                return;
            }

            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(owner.Name);
            if (heartLevel < ModEntry.Config.HagglerMinHeartLevel)
            {
                return;
            }
            if (heartLevel > 10)
            {
                heartLevel = 10;
            }

            int discountPercent = (heartLevel - ModEntry.Config.HagglerMinHeartLevel + 1) * ModEntry.Config.HagglerDiscountPercentPerHeartLevel;
            if (Game1.player.HasCustomPrestigeProfession(SocializingSkill.Haggler))
            {
                discountPercent += 10;
            }

            float discount = (100f - discountPercent) / 100;

            foreach (KeyValuePair<ISalable, ItemStockInformation> item in __instance.itemPriceAndStock)
            {
                ItemStockInformation info = item.Value;
                info.Price = (int)Math.Max(0, discount * info.Price);
                __instance.itemPriceAndStock[item.Key] = info;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}*/

// Skill perk
//  - Reduce friendship decay
[HarmonyPatch(typeof(Farmer), nameof(Farmer.resetFriendshipsForNewDay))]
class Farmer_ResetFriendshipForNewDay
{
    static void Postfix(Farmer __instance)
    {
        try
        {
            Random random = new();
            int level = Skills.GetSkillLevel(__instance, "drbirbdev.Socializing");
            if (random.Next(100) < level * ModEntry.Config.ChanceNoFriendshipDecayPerLevel)
            {
                // Undo vanilla friendship decay
                // TODO: check other mods for friendship loss prevention maybe
                foreach (string name in __instance.friendshipData.Keys)
                {
                    bool single = false;
                    NPC i = Game1.getCharacterFromName(name);
                    i ??= Game1.getCharacterFromName<Child>(name, mustBeVillager: false);
                    if (i != null)
                    {
                        if (i != null && i.datable.Value && !__instance.friendshipData[name].IsDating() && !i.isMarried())
                        {
                            single = true;
                        }
                        if (__instance.spouse != null && name.Equals(__instance.spouse) && !__instance.hasPlayerTalkedToNPC(name))
                        {
                            __instance.changeFriendship(20, i);
                        }
                        else if (i != null && __instance.friendshipData[name].IsDating() && !__instance.hasPlayerTalkedToNPC(name) && __instance.friendshipData[name].Points < 2500)
                        {
                            __instance.changeFriendship(10, i);
                        }
                        else if ((!single && __instance.friendshipData[name].Points < 2500) || (single && __instance.friendshipData[name].Points < 2000))
                        {
                            __instance.changeFriendship(2, i);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// Grant XP
// Friendly
//  - Give extra friendship
[HarmonyPatch(typeof(NPC), nameof(NPC.grantConversationFriendship))]
class NPC_GrantConversationFriendship
{
    static void Prefix(
        Farmer who,
        int amount,
        NPC __instance)
    {
        try
        {
            if (!__instance.Name.Contains("King")
        && !who.hasPlayerTalkedToNPC(__instance.Name)
        && who.friendshipData.ContainsKey(__instance.Name)
        && !__instance.isDivorcedFrom(who)
        && amount > 0)
            {
                Skills.AddExperience(who, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromTalking);
                if (who.HasProfession("Friendly"))
                {
                    if (who.HasProfession("Friendly", true))
                    {
                        who.changeFriendship((int)(ModEntry.Config.FriendlyExtraFriendship * 1.5), __instance);
                    }
                    else
                    {
                        who.changeFriendship(ModEntry.Config.FriendlyExtraFriendship, __instance);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

// Beloved Profession
//  - Sometimes get random gifts
[HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
class NPC_CheckAction
{
    static void Postfix(
        Farmer who,
        NPC __instance,
        ref bool __result)
    {
        try
        {
            if (__result)
            {
                return;
            }
            if (__instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove)
            {
                return;
            }
            if (!who.HasProfession("Beloved"))
            {
                return;
            }
            ModEntry.BelovedCheckedToday.Value ??= new List<string>();
            if (ModEntry.BelovedCheckedToday.Value.Contains(__instance.Name))
            {
                return;
            }
            ModEntry.BelovedCheckedToday.Value.Add(__instance.Name);

            int giftPercentChance = ModEntry.Config.BelovedGiftPercentChance;
            if (who.HasProfession("Beloved", true))
            {
                giftPercentChance *= 2;
            }

            //TODO: 1.6 update NPC gifts.  Use Item Queries maybe?
            /*            int rarity = 1;
                        if (rarity < 0)
                        {
                            return;
                        }
                        // Get a gift from Beloved profession

                        string heartLevel = who.getFriendshipHeartLevelForNPC(__instance.Name).ToString();

                        string dropString = Utilities.GetRandomDropStringFromLootTable(ModEntry.Assets.BelovedTable, __instance.Name, heartLevel, rarity.ToString());
                        Item gift = Utilities.ParseDropString(dropString);

                        string dialogue = rarity switch
                        {
                            0 => (string)ModEntry.Instance.I18n.Get("dialogue.beloved"),
                            1 => (string)ModEntry.Instance.I18n.Get("dialogue.beloved.rare", new { name = Game1.player.displayName }),
                            _ => (string)ModEntry.Instance.I18n.Get("dialogue.beloved.superrare"),
                        };
                        __instance.CurrentDialogue.Push(new Dialogue(__instance, dialogue));
                        Game1.drawDialogue(__instance);
                        // Game1.player.addItemByMenuIfNecessary(gift);
                        __result = true;
            */
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}
