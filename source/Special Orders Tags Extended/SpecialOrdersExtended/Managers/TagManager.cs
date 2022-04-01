/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// Static class to hold tag-management functions.
/// </summary>
internal class TagManager
{
    private static Random? random;

    /// <summary>
    /// Gets a seeded random that changes once per in-game week.
    /// </summary>
    internal static Random Random
    {
        get
        {
            random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed / 7 * 36));
            return random;
        }
    }

    /// <summary>
    /// Delete's the random so it can be reset later.
    /// </summary>
    public static void ResetRandom() => random = null;

    /// <summary>
    /// Prefixes CheckTag to handle special mod tags.
    /// </summary>
    /// <param name="__result">the result for the original function.</param>
    /// <param name="__0">string - tag to check.</param>
    /// <returns>true to continue to the vanilla function, false otherwise.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Naming convention for Harmony")]
    public static bool PrefixCheckTag(ref bool __result, string __0)
    {
#if DEBUG
        ModEntry.ModMonitor.Log($"Checking tag {__0}", LogLevel.Trace);
#endif
        try
        {
            string[] vals = __0.Split('_');
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
                    __result = Game1.getCharacterFromName(vals[1])?.getSpouse() is not null;
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
                    __result = vals[1] switch
                    {
                        "mining" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.miningLevel.Value >= levelwanted),
                        "farming" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.farmingLevel.Value >= levelwanted),
                        "fishing" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.fishingLevel.Value >= levelwanted),
                        "foraging" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.foragingLevel.Value >= levelwanted),
                        "combat" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.combatLevel.Value >= levelwanted),
                        "luck" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.luckLevel.Value >= levelwanted),
                        _ => ModEntry.SpaceCoreAPI is not null && ModEntry.SpaceCoreAPI.GetCustomSkills().Contains(vals[1])
                                && Game1.getAllFarmers().Any((Farmer farmer) => ModEntry.SpaceCoreAPI.GetLevelForCustomSkill(farmer, vals[1]) >= levelwanted),
                    };
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
                    __result = vals[1] switch
                    {
                        "bearsKnowledge" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.eventsSeen.Contains(2120303)),
                        "clubCard" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasClubCard),
                        "rustyKey" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasRustyKey),
                        "skullKey" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasSkullKey),
                        "specialCharm" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasSpecialCharm),
                        "springOnion" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.eventsSeen.Contains(3910979)),
                        "translationGuide" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.canUnderstandDwarves),
                        "townKey" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.HasTownKey),
                        _ => false,
                    };
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
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
                    // craftingrecipe_X, craftingrecipe_X_not
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.craftingRecipes.ContainsKey(vals[1].Replace('-', ' ')));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
                case "cookingrecipe":
                    // cookingrecipe_X, cookingrecipe_X_not
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.cookingRecipes.ContainsKey(vals[1].Replace('-', ' ')));
                    if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase))
                    {
                        __result = !__result;
                    }
                    return false;
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
            ModEntry.ModMonitor.Log($"Failed while checking tag {__0}\n{ex}", LogLevel.Error);
        }
        return true; // continue to base code.
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
            catch (InvalidOperationException)
            {
                ModEntry.ModMonitor.Log(I18n.SkillNotFound(profession, skill), LogLevel.Debug);
            }
            catch (NullReferenceException)
            {
                ModEntry.ModMonitor.Log(I18n.SkillNotFound(profession, skill), LogLevel.Debug);
            }
        }
        return professionNumber;
    }
}
