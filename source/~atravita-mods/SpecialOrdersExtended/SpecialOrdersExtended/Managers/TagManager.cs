/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Numerics;

using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.Caches;

using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// Static class to hold tag-management functions.
/// </summary>
[HarmonyPatch(typeof(SpecialOrder))]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
internal static class TagManager
{
#region random

    private static Random? random;

    /// <summary>
    /// Gets a seeded random that changes once per in-game week.
    /// </summary>
    internal static Random Random
    {
        get
        {
            if (random is null)
            {
                random = new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)((Game1.stats.DaysPlayed / 7) * 36));
                random.PreWarm();
            }
            return random;
}
    }

    /// <summary>
    /// Delete's the random so it can be reset later.
    /// </summary>
    internal static void ResetRandom()
    {
        if (Game1.stats.DaysPlayed % 7 == 0)
        {
            random = null;
        }
    }

#endregion

#region cache

    private static readonly Dictionary<string, bool> Cache = new();
    private static int lastTick = -1;

    /// <summary>
    /// Clears the cache.
    /// </summary>
    internal static void ClearCache()
    {
        lastTick = -1;
        Cache.Clear();
    }

#endregion

    /// <summary>
    /// Prefixes CheckTag to handle special mod tags.
    /// </summary>
    /// <param name="__result">the result for the original function.</param>
    /// <param name="tag">tag to check.</param>
    /// <returns>true to continue to the vanilla function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPatch("CheckTag")]
    [HarmonyPriority(Priority.VeryHigh)]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Naming convention for Harmony")]
    private static bool PrefixCheckTag(ref bool __result, string tag)
    {
        {
            if (ModEntry.Config.UseTagCache && Cache.TryGetValue(tag, out bool result))
            {
                if (Game1.ticks != lastTick)
                {
                    Cache.Clear();
                    lastTick = Game1.ticks;
                }
                else
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"Hit cache: {tag}, {result}", LogLevel.Trace);
                    __result = result;
                    return false;
                }
            }
        }

        ModEntry.ModMonitor.DebugOnlyLog($"Checking tag {tag}", LogLevel.Trace);
        try
        {
            string[] vals = tag.Split('_', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            switch(vals[0].ToLowerInvariant())
            {
                case "year":
                    // year_X
                    __result = int.TryParse(vals[1], out int yearval) && Game1.year == yearval;
                    return false;
                case "atleastyear":
                    // atleastyear_X
                    __result = int.TryParse(vals[1], out int yearnum) && Game1.year >= yearnum;
                    return false;
                case "yearunder":
                    // yearunder_X
                    __result = int.TryParse(vals[1], out int yearint) && Game1.year < yearint;
                    return false;
                case "week":
                    // week_X
                    __result = int.TryParse(vals[1], out int weeknum) && (Game1.dayOfMonth + 6) / 7 == weeknum;
                    return false;
                case "daysplayed":
                    // daysplayed_X, daysplayed_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int daysplayed) && Game1.stats.DaysPlayed < daysplayed;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int daysplayed) && Game1.stats.DaysPlayed >= daysplayed;
                    }
                    return false;
                case "anyplayermail":
                    // anyplayermail_mailkey, anyplayermail_mailkey_not
                    __result = Game1.getAllFarmers().Any((Farmer f) => f.mailReceived.Contains(vals[1]));
                    if (vals.Length >= 2 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "anyplayerseenevent":
                    // anyplayerseenevent_eventID, anyplayerseenevent_eventID_not
                    if (vals.Length >= 2 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[1], out int eventid) && Game1.getAllFarmers().All((Farmer f) => !f.eventsSeen.Contains(eventid));
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int eventid) && Game1.getAllFarmers().Any((Farmer f) => f.eventsSeen.Contains(eventid));
                    }
                    return false;
                case "dropboxroom":
                    // dropboxRoom_roomName
                    string roomname = vals[1];
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if (specialOrder.questState.Value != SpecialOrder.QuestState.InProgress)
                        {
                            continue;
                        }
                        foreach (OrderObjective objective in specialOrder.objectives)
                        {
                            if (objective is DonateObjective donateobjective && donateobjective.dropBoxGameLocation.Value.Equals(roomname, StringComparison.OrdinalIgnoreCase))
                            {
                                __result = true;
                                return false;
                            }
                        }
                    }
                    __result = false;
                    return false;
                case "conversation":
                    // conversation_CTname, conversation_CTname_not
                    string conversationTopic = vals[1];
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.activeDialogueEvents.ContainsKey(conversationTopic));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "haskilled":
                    // haskilled_Monster-name_X, haskilled_Monster-name_under_X
                    string monster = vals[1].Replace('-', ' ');
                    if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[3], out int numkilled) && Game1.getAllFarmers().All((Farmer farmer) => farmer.stats.getMonstersKilled(monster) < numkilled);
                    }
                    else
                    {
                        __result = int.TryParse(vals[2], out int numkilled) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.stats.getMonstersKilled(monster) >= numkilled);
                    }
                    return false;
                case "friendship":
                    // friendship_NPCname_X, friendship_NPCname_under_X
                    if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[3], out int friendshipNeeded) && Game1.getAllFarmers().All((Farmer farmer) => farmer.getFriendshipLevelForNPC(vals[1]) < friendshipNeeded);
                    }
                    else
                    {
                        __result = int.TryParse(vals[2], out int friendshipNeeded) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.getFriendshipLevelForNPC(vals[1]) >= friendshipNeeded);
                    }
                    return false;
                case "married":
                    // married_NPCname, married_NPCname_not
                    __result = NPCCache.GetByVillagerName(vals[1])?.getSpouse() is not null;
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "minelevel":
                    // minelevel_X, minelevel_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int mineLevel) && Utility.GetAllPlayerDeepestMineLevel() < mineLevel;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int mineLevel) && Utility.GetAllPlayerDeepestMineLevel() >= mineLevel;
                    }
                    return false;
                case "houselevel":
                    // houselevel_X, houselevel_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int houseLevel) && Game1.getAllFarmers().All((Farmer farmer) => farmer.HouseUpgradeLevel < houseLevel);
                    }
                    else
                    {
                        __result = int.TryParse(vals[21], out int houseLevel) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.HouseUpgradeLevel >= houseLevel);
                    }
                    return false;
                case "moneyearned":
                    // moneyearned_X, moneyearned_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = uint.TryParse(vals[2], out uint moneyEarned) && Game1.MasterPlayer.totalMoneyEarned < moneyEarned;
                    }
                    else
                    {
                        __result = uint.TryParse(vals[1], out uint moneyEarned) && Game1.MasterPlayer.totalMoneyEarned >= moneyEarned;
                    }
                    return false;
                case "skilllevel":
                    // skilllevel_skill_X, skilllevel_skill_under_X
                    int levelwanted;
                    bool negate = false;
                    if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!int.TryParse(vals[3], out levelwanted))
                        {
                            __result = false;
                            return false;
                        }
                        negate = true;
                    }
                    else
                    {
                        if (!int.TryParse(vals[2], out levelwanted))
                        {
                            __result = false;
                            return false;
                        }
                    }
                    if (SkillsExtensions.TryParse(vals[1], out Skills skill, ignoreCase: true) && BitOperations.PopCount((uint)skill) == 1)
                    {
                        __result = Game1.getAllFarmers().Any(farmer => farmer.GetSkillLevelFromEnum(skill, includeBuffs: false) >= levelwanted);
                    }
                    else
                    {
                        __result = ModEntry.SpaceCoreAPI is not null && ModEntry.SpaceCoreAPI.GetCustomSkills().Contains(vals[1])
                                && Game1.getAllFarmers().Any(farmer => ModEntry.SpaceCoreAPI.GetLevelForCustomSkill(farmer, vals[1]) >= levelwanted);
                    }

                    if (negate)
                    {
                        __result = !__result;
                    }
                    return false;
                case "profession":
                    // profession_name_skill, profession_name_skill_not
                    __result = false;
                    int? profession = GetProfession(vals[1], vals.Length >= 3 ? vals[2] : null);
                    if (profession is not null)
                    {
                        __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.professions.Contains(profession.Value));
                    }
                    if (vals.Length >= 4 && vals[3].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "hasspecialitem":
                    // hasspecialitem_X, hasspecialitem_X_not
                    __result = WalletItemsExtensions.TryParse(vals[1], out WalletItems item, ignoreCase: true)
                                && BitOperations.PopCount((uint)item) == 1
                                && Game1.getAllFarmers().Any(farmer => farmer is not null && farmer.HasSingleWalletItem(item));
                    if (vals.Length > 2 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "achievement":
                    // achievement_X, achievement_X_not
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.achievements.Contains(int.Parse(vals[1])));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "craftingrecipe":
                {
                    // craftingrecipe_X, craftingrecipe_X_not
                    string key = vals[1].Replace('-', ' ');
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.craftingRecipes.ContainsKey(key));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                }
                case "cookingrecipe":
                {
                    // cookingrecipe_X, cookingrecipe_X_not
                    string key = vals[1].Replace('-', ' ');
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.cookingRecipes.ContainsKey(key));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                }
                case "stats":
                    // stats_statsname_X, stats_statsname_under_X
                    if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = uint.TryParse(vals[3], out uint stat) && Game1.getAllFarmers().All((Farmer farmer) => StatsManager.GrabBasicProperty(vals[1], farmer.stats) < stat);
                    }
                    else
                    {
                        __result = uint.TryParse(vals[3], out uint stat) && Game1.getAllFarmers().Any((Farmer farmer) => StatsManager.GrabBasicProperty(vals[1], farmer.stats) >= stat);
                    }
                    return false;
                case "walnutcount":
                    // walnutcount_X, walnutcount_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int walnutcount) && Game1.netWorldState.Value.GoldenWalnutsFound.Value < walnutcount;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int walnutcount) && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= walnutcount;
                    }
                    return false;
                case "specialorderscompleted":
                    // specialorderscompleted_X, specialorderscompleted_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int required) && Game1.player.team.completedSpecialOrders.Count() < required;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int required) && Game1.player.team.completedSpecialOrders.Count() >= required;
                    }
                    return false;
                case "slots":
                    // slots_X, slots_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int required) && Club.timesPlayedSlots < required;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int required) && Club.timesPlayedSlots >= required;
                    }
                    return false;
                case "blackjack":
                    // blackjack_X, blackjac_under_X
                    if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = int.TryParse(vals[2], out int required) && Club.timesPlayedCalicoJack < required;
                    }
                    else
                    {
                        __result = int.TryParse(vals[1], out int required) && Club.timesPlayedCalicoJack >= required;
                    }
                    return false;
                case "random":
                    // random_x
                    return float.TryParse(vals[1], out float result) && Random.NextDouble() < result; // not convinced on this implementation. Should I save values instead?
                default:
                    // Not a tag I recognize, return true.
                    return true;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while checking tag {tag}\n{ex}", LogLevel.Error);
        }
        return true; // continue to base code.
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last - 200)]
    [HarmonyPatch("CheckTag")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Naming convention for Harmony")]
    private static void WatchTag(bool __result, string __0)
    {
        if (ModEntry.Config.UseTagCache)
        {
            if (Game1.ticks != lastTick)
            {
                Cache.Clear();
                lastTick = Game1.ticks;
            }

            Cache[__0] = __result;
        }
    }

    /// <summary>
    /// Returns the integer ID of a profession.
    /// </summary>
    /// <param name="profession">Profession name.</param>
    /// <param name="skill">Skill name (leave null for vanilla skill).</param>
    /// <returns>Integer profession, null for not found.</returns>
    private static int? GetProfession(string profession, string? skill = null)
    {
        int? professionNumber = profession switch
        {
            /* Farming professions */
            "rancher" => Farmer.rancher,
            "tiller" => Farmer.tiller,
            "coopmaster" => Farmer.butcher, // [sic]
            "shepherd" => Farmer.shepherd,
            "artisan" => Farmer.artisan,
            "agriculturist" => Farmer.agriculturist,
            /* Fishing professions */
            "fisher" => Farmer.fisher,
            "trapper" => Farmer.trapper,
            "angler" => Farmer.angler,
            "pirate" => Farmer.pirate,
            "mariner" => Farmer.baitmaster, // [sic]
            "luremaster" => Farmer.mariner, // [sic]
            /* Foraging professions */
            "forester" => Farmer.forester,
            "gatherer" => Farmer.gatherer,
            "lumberjack" => Farmer.lumberjack,
            "tapper" => Farmer.tapper,
            "botanist" => Farmer.botanist,
            "tracker" => Farmer.tracker,
            /* Mining professions */
            "miner" => Farmer.miner,
            "geologist" => Farmer.geologist,
            "blacksmith" => Farmer.blacksmith,
            "prospector" => Farmer.burrower, // [sic]
            "excavator" => Farmer.excavator,
            "gemologist" => Farmer.gemologist,
            /* Combat professions */
            "fighter" => Farmer.fighter,
            "scout" => Farmer.scout,
            "brute" => Farmer.brute,
            "defender" => Farmer.defender,
            "acrobat" => Farmer.acrobat,
            "desperado" => Farmer.desperado,
            _ => null
        };
        if (professionNumber is null && skill is not null)
        {
            try
            {
                if (ModEntry.SpaceCoreAPI is not null && ModEntry.SpaceCoreAPI.GetCustomSkills().Contains(skill))
                {
                    professionNumber = ModEntry.SpaceCoreAPI.GetProfessionId(skill, profession);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or NullReferenceException)
            {
                ModEntry.ModMonitor.Log(I18n.SkillNotFound(profession, skill), LogLevel.Debug);
            }
        }
        return professionNumber;
    }
}
