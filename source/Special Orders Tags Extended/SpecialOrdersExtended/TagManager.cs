/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

namespace SpecialOrdersExtended;

/// <summary>
/// Static class to hold tag-management functions.
/// </summary>
internal class TagManager
{
    /// <summary>
    /// Prefixes CheckTag to handle special mod tags.
    /// </summary>
    /// <param name="__result">the result for the original function.</param>
    /// <param name="__0">string - tag to check.</param>
    /// <returns>true to continue to the vanilla function, false otherwise.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Naming convention for Harmony")]
    public static bool PrefixCheckTag(ref bool __result, string __0)
    {
        ModEntry.ModMonitor.DebugLog($"Checking tag {__0}");
        try
        {
            string[] vals = __0.Split('_');
            switch(vals[0])
            {
                case "year":
                    // year_X
                    __result = Game1.year == int.Parse(__0["year_".Length..]);
                    return false;
                case "atleastyear":
                    // atleastyear_X
                    __result = Game1.year >= int.Parse(__0["atleastyear_".Length..]);
                    return false;
                case "yearunder":
                    // yearunder_X
                    __result = Game1.year < int.Parse(__0["yearunder_".Length..]);
                    return false;
                case "week":
                    // week_X
                    __result = __0 switch
                    {
                        "week_1" => Game1.dayOfMonth is >= 1 and <= 7,
                        "week_2" => Game1.dayOfMonth is >= 8 and <= 14,
                        "week_3" => Game1.dayOfMonth is >= 15 and <= 21,
                        "week_4" => Game1.dayOfMonth is >= 22 and <= 28,
                        _ => false,
                    };
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
                case "dropboxRoom":
                    // dropboxRoom_roomName
                    string roomname = __0["dropboxRoom_".Length..];
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
                        __result = int.TryParse(vals[3], out int numkilled) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.stats.getMonstersKilled(monster) < numkilled);
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
                        __result = int.TryParse(vals[3], out int friendshipNeeded) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.getFriendshipLevelForNPC(vals[1]) < friendshipNeeded);
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
                        __result = int.TryParse(vals[2], out int houseLevel) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.HouseUpgradeLevel < houseLevel);
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
                        "mining" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.MiningLevel >= levelwanted),
                        "farming" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.FarmingLevel >= levelwanted),
                        "fishing" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.FishingLevel >= levelwanted),
                        "foraging" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.ForagingLevel >= levelwanted),
                        "combat" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.CombatLevel >= levelwanted),
                        _ => false,
                    };
                    if (negate)
                    {
                        __result = !__result;
                    }
                    return false;
                case "hasspecialitem":
                    // hasspecialitem_X, hasspecialitem_X_not
                    __result = vals[1] switch
                    {
                        "clubCard" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasClubCard),
                        "specialCharm" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasSpecialCharm),
                        "skullKey" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasSkullKey),
                        "rustyKey" => Game1.getAllFarmers().Any((Farmer farmer) => farmer.hasRustyKey),
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
                        __result = uint.TryParse(vals[3], out uint stat) && Game1.getAllFarmers().Any((Farmer farmer) => StatsManager.GrabBasicProperty(vals[1], farmer.stats) < stat);
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
}
