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

internal class TagManager
{
    public static bool PrefixCheckTag(ref bool __result, string __0)
    {
        ModEntry.ModMonitor.VerboseLog($"Checking tag {__0}");
        try
        {
            if (__0.StartsWith("year_"))
            {
                __result = Game1.year == int.Parse(__0["year_".Length..]);
                return false;
            }
            else if (__0.StartsWith("atleastyear_"))
            {
                __result = Game1.year >= int.Parse(__0["atleastyear_".Length..]);
                return false;
            }
            else if (__0.StartsWith("yearunder_"))
            {
                __result = Game1.year < int.Parse(__0["yearunder_".Length..]);
                return false;
            }
            else if (__0.StartsWith("week_"))
            {
                __result = __0 switch
                {
                    "week_1" => (1 <= Game1.dayOfMonth) && (Game1.dayOfMonth <= 7),
                    "week_2" => (8 <= Game1.dayOfMonth) && (Game1.dayOfMonth <= 14),
                    "week_3" => (15 <= Game1.dayOfMonth) && (Game1.dayOfMonth <= 21),
                    "week_4" => (22 <= Game1.dayOfMonth) && (Game1.dayOfMonth <= 28),
                    _ => false,
                };
                return false;
            }
            else if (__0.StartsWith("daysplayed_"))
            {
                string[] vals = __0.Split('_');
                if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.stats.DaysPlayed < int.Parse(vals[2]);
                }
                else
                {
                    __result = Game1.stats.DaysPlayed >= int.Parse(vals[1]);
                }
                return false;
            }
            else if (__0.StartsWith("dropboxRoom_"))
            {
                string roomname = __0["dropboxRoom_".Length..];
                foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                {
                    if (specialOrder.questState.Value != SpecialOrder.QuestState.InProgress) { continue; }
                    foreach (OrderObjective objective in specialOrder.objectives)
                    {
                        if (objective is DonateObjective && (objective as DonateObjective).dropBoxGameLocation.Value.Equals(roomname, StringComparison.OrdinalIgnoreCase))
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                __result = false;
                return false;
            }
            else if (__0.StartsWith("conversation_"))
            {
                string[] vals = __0.Split('_');
                string conversationTopic = vals[1];
                __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.activeDialogueEvents.ContainsKey(conversationTopic));
                if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase)) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("haskilled_"))
            {
                string[] vals = __0.Split('_');
                string monster = vals[1].Replace('-', ' ');
                if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.stats.getMonstersKilled(monster) < int.Parse(vals[3]));
                }
                else
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.stats.getMonstersKilled(monster) >= int.Parse(vals[2]));
                }
                return false;
            }
            else if (__0.StartsWith("friendship_")) //Consider marriage?
            {
                string[] vals = __0.Split('_');
                if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.getFriendshipLevelForNPC(vals[1]) < int.Parse(vals[3]));
                }
                else
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.getFriendshipLevelForNPC(vals[1]) >= int.Parse(vals[2]));
                }
                return false;
            }
            else if (__0.StartsWith("minelevel_"))
            {
                string[] vals = __0.Split('_');
                if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Utility.GetAllPlayerDeepestMineLevel() < int.Parse(vals[2]);
                }
                else
                {
                    __result = Utility.GetAllPlayerDeepestMineLevel() >= int.Parse(vals[1]);
                }
                return false;
            }
            else if (__0.StartsWith("houselevel_"))
            {
                string[] vals = __0.Split('_');
                if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.HouseUpgradeLevel < int.Parse(vals[2]));
                }
                else
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.HouseUpgradeLevel >= int.Parse(vals[1]));
                }
                return false;
            }
            else if (__0.StartsWith("moneyearned_"))
            {
                string[] vals = __0.Split('_');
                if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.MasterPlayer.totalMoneyEarned < uint.Parse(vals[2]);
                }
                else
                {
                    __result = Game1.MasterPlayer.totalMoneyEarned >= uint.Parse(vals[1]);
                }
                return false;
            }
            else if (__0.StartsWith("skilllevel_"))
            {
                string[] vals = __0.Split('_');
                int levelwanted;
                bool negate = false;
                if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    levelwanted = int.Parse(vals[3]);
                    negate = true;
                }
                else
                {
                    levelwanted = int.Parse(vals[2]);
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
                if (negate) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("hasspecialitem_"))
            {
                string[] vals = __0.Split('_');
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
                if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase)) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("achievement_"))
            {
                string[] vals = __0.Split('_');
                __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.achievements.Contains(int.Parse(vals[1])));
                if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase)) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("craftingrecipe_"))
            {
                string[] vals = __0.Split('_');
                __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.craftingRecipes.ContainsKey(vals[1].Replace('-', ' ')));
                if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase)) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("cookingrecipe_"))
            {
                string[] vals = __0.Split('_');
                __result = Game1.getAllFarmers().Any((Farmer farmer) => farmer.cookingRecipes.ContainsKey(vals[1].Replace('-', ' ')));
                if (vals.Length >= 3 && vals[2].Equals("not", StringComparison.OrdinalIgnoreCase)) { __result = !__result; }
                return false;
            }
            else if (__0.StartsWith("stats_"))
            {
                string[] vals = __0.Split('_');
                if (vals[2].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => ModEntry.StatsManager.GrabBasicProperty(vals[1], farmer.stats) < uint.Parse(vals[3]));
                }
                else
                {
                    __result = Game1.getAllFarmers().Any((Farmer farmer) => ModEntry.StatsManager.GrabBasicProperty(vals[1], farmer.stats) >= uint.Parse(vals[2]));
                }
                return false;
            }
            else if (__0.StartsWith("walnutcount_"))
            {
                string[] vals = __0.Split('_');
                if (vals[1].Equals("under", StringComparison.OrdinalIgnoreCase))
                {
                    __result = (int)Game1.netWorldState.Value.GoldenWalnutsFound.Value < int.Parse(vals[2]);
                }
                else
                {
                    __result = (int)Game1.netWorldState.Value.GoldenWalnutsFound.Value >= int.Parse(vals[1]);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while checking tag {__0}\n{ex}", LogLevel.Error);
        }
        return true; //continue to base code.

    }
}
