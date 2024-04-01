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
using StardewValley.Delegates;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.SpecialOrders.Rewards;

namespace SocializingSkill;

// Grant XP from event dialogue
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
            foreach (var playerResponse in ___playerResponses)
            {
                if (playerResponse.responseKey == null || response.responseKey == null ||
                    !playerResponse.responseKey.Equals(response.responseKey))
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

                if (playerResponse.friendshipChange <= 0)
                {
                    return;
                }

                Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromEvents);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

// Smooth Talker Profession
//  - adjust friendship change during dialogue
[HarmonyPatch(typeof(NPCDialogueResponse), MethodType.Constructor,
    [typeof(string), typeof(int), typeof(string), typeof(string), typeof(string)])]
class NpcDialogueResponse_Constructor
{
    internal static void Postfix(
        int friendshipChange,
        NPCDialogueResponse __instance)
    {
        try
        {
            if (!Game1.player.HasProfession("SmoothTalker"))
            {
                return;
            }

            if (Game1.player.HasProfession("SmoothTalker", true))
            {
                if (friendshipChange < 0)
                {
                    __instance.friendshipChange =
                        (int)(friendshipChange * ModEntry.Config.SmoothTalkerPrestigeNegativeMultiplier);
                }
                else
                {
                    __instance.friendshipChange =
                        (int)(friendshipChange * ModEntry.Config.SmoothTalkerPrestigePositiveMultiplier);
                }
            }
            else
            {
                if (friendshipChange < 0)
                {
                    __instance.friendshipChange =
                        (int)(friendshipChange * ModEntry.Config.SmoothTalkerNegativeMultiplier);
                }
                else
                {
                    __instance.friendshipChange =
                        (int)(friendshipChange * ModEntry.Config.SmoothTalkerPositiveMultiplier);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

// Grant XP from event dialogue
// Smooth Talker Profession
//  - adjust friendship change during event
[HarmonyPatch(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.Friendship))]
class Event_CommandFriendship
{
    static void Postfix(string[] args)
    {
        try
        {
            NPC character = Game1.getCharacterFromName(args[1]);
            if (character == null)
            {
                return;
            }

            // Add XP
            int friendship = Convert.ToInt32(args[2]);

            if (friendship > 0)
            {
                Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromEvents);
            }

            if (!Game1.player.HasProfession("SmoothTalker"))
            {
                return;
            }

            // Undo original method friendship change
            Game1.player.changeFriendship(-friendship, character);

            if (Game1.player.HasProfession("SmoothTalker", true))
            {
                if (friendship < 0)
                {
                    friendship = (int)(friendship * ModEntry.Config.SmoothTalkerPrestigeNegativeMultiplier);
                }
                else
                {
                    friendship = (int)(friendship * ModEntry.Config.SmoothTalkerPrestigePositiveMultiplier);
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
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
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
            if (!Game1.player.HasProfession("Helpful"))
            {
                return;
            }

            if (Game1.player.HasProfession("Helpful", true))
            {
                __result = (int)(__result * ModEntry.Config.HelpfulPrestigeRewardMultiplier);
            }
            else
            {
                __result = (int)(__result * ModEntry.Config.HelpfulRewardMultiplier);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

// Haggler Profession
//  - Decrease shop prices if friends with the owner
[HarmonyPatch(typeof(ShopMenu), MethodType.Constructor, [
    typeof(string),
    typeof(ShopData),
    typeof(ShopOwnerData),
    typeof(NPC),
    typeof(Func<ISalable, Farmer, int, bool>),
    typeof(Func<ISalable, bool>),
    typeof(bool)
])]
class ShopMenu_Constructor1
{
    internal static void Postfix(NPC owner, ShopMenu __instance)
    {
        try
        {
            if (!Game1.player.HasProfession("Haggler"))
            {
                return;
            }

            if (owner?.Name == null)
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

            int discountPercent = (heartLevel - ModEntry.Config.HagglerMinHeartLevel + 1) *
                                  ModEntry.Config.HagglerDiscountPercentPerHeartLevel;
            if (Game1.player.HasProfession("Haggler", true))
            {
                discountPercent += ModEntry.Config.HagglerPrestigeDiscountPercent;
            }

            float discount = (100f - discountPercent) / 100f;

            foreach (KeyValuePair<ISalable, ItemStockInformation> item in __instance.itemPriceAndStock)
            {
                ItemStockInformation info = item.Value;
                info.Price = (int)Math.Max(0, discount * info.Price);
                __instance.itemPriceAndStock[item.Key] = info;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

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
            if (random.Next(100) >= level * ModEntry.Config.ChanceNoFriendshipDecayPerLevel)
            {
                return;
            }

            // Undo vanilla friendship decay
            // TODO: check other mods for friendship loss prevention maybe
            foreach (string name in __instance.friendshipData.Keys)
            {
                bool single = false;
                NPC i = Game1.getCharacterFromName(name);
                i ??= Game1.getCharacterFromName<Child>(name, mustBeVillager: false);
                if (i == null)
                {
                    continue;
                }

                if (i.datable.Value && !__instance.friendshipData[name].IsDating() && !i.isMarried())
                {
                    single = true;
                }

                if (__instance.spouse != null && name.Equals(__instance.spouse) &&
                    !__instance.hasPlayerTalkedToNPC(name))
                {
                    __instance.changeFriendship(20, i);
                }
                else if (__instance.friendshipData[name].IsDating() && !__instance.hasPlayerTalkedToNPC(name) &&
                         __instance.friendshipData[name].Points < 2500)
                {
                    __instance.changeFriendship(10, i);
                }
                else if ((!single && __instance.friendshipData[name].Points < 2500) ||
                         (single && __instance.friendshipData[name].Points < 2000))
                {
                    __instance.changeFriendship(2, i);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

// Grant XP
// Friendly
//  - Give extra friendship
[HarmonyPatch(typeof(NPC), nameof(NPC.grantConversationFriendship))]
class Npc_GrantConversationFriendship
{
    static void Prefix(
        Farmer who,
        int amount,
        NPC __instance)
    {
        try
        {
            if (__instance.Name.Contains("King")
                || who.hasPlayerTalkedToNPC(__instance.Name)
                || !who.friendshipData.ContainsKey(__instance.Name)
                || __instance.isDivorcedFrom(who)
                || amount <= 0)
            {
                return;
            }

            Skills.AddExperience(who, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromTalking);
            if (!who.HasProfession("Friendly"))
            {
                return;
            }

            if (who.HasProfession("Friendly", true))
            {
                who.changeFriendship(ModEntry.Config.FriendlyPrestigeExtraFriendship, __instance);
            }
            else
            {
                who.changeFriendship(ModEntry.Config.FriendlyExtraFriendship, __instance);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}

// Beloved Profession
//  - Sometimes get random gifts
[HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
class Npc_CheckAction
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

            ModEntry.BELOVED_CHECKED_TODAY.Value ??= [];
            if (!ModEntry.BELOVED_CHECKED_TODAY.Value.Add(__instance.Name))
            {
                return;
            }

            int baseChance = who.HasProfession("Beloved", true)
                ? ModEntry.Config.BelovedPrestigeGiftPercentChance
                : ModEntry.Config.BelovedGiftPercentChance;

            Random belovedRandom =
                Utility.CreateDaySaveRandom(3701 * Game1.hash.GetDeterministicHashCode(__instance.Name));

            if (baseChance < belovedRandom.Next(100))
            {
                return;
            }

            List<Assets.BelovedEntry> data = ModEntry.Assets.BelovedData.GetValueOrDefault(__instance.Name, []);
            data.AddRange(ModEntry.Assets.BelovedData.GetValueOrDefault("default", []));

            Item selected = ItemRegistry.Create("(O)168");
            Dialogue dialogue = new(__instance, ModEntry.Instance.I18N.Get(""));
            foreach (Assets.BelovedEntry entry in data)
            {
                if (string.IsNullOrWhiteSpace(entry.Id))
                {
                    Log.Error($"Ignored item entry with no Id field for {__instance.Name}");
                    continue;
                }

                ItemQueryContext itemQueryContext =
                    new ItemQueryContext(__instance.currentLocation, who, belovedRandom);

                Item result = ItemQueryResolver.TryResolveRandomItem(entry, itemQueryContext);

                GameStateQueryContext context = new(__instance.currentLocation, who, result, null, belovedRandom, null,
                    new Dictionary<string, object>
                    {
                        {"NPC", __instance}
                    });

                if (!GameStateQuery.CheckConditions(entry.Condition, context))
                {
                    continue;
                }

                selected = result;
                if (entry.Dialogue is not null)
                {
                    dialogue = new Dialogue(__instance, entry.Dialogue);
                }

                break;
            }

            __instance.CurrentDialogue.Push(dialogue);
            Game1.drawDialogue(__instance);
            Game1.player.addItemByMenuIfNecessary(selected);
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
    }
}
