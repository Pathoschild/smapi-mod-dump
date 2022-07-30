/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

using AtraUtils = AtraShared.Utils.Utils;

namespace HolidaySales.HarmonyPatches;

/// <summary>
/// Adjusts quests of the day.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal static class AdjustQuestOfTheDay
{
    /// <summary>
    /// Prefixes RefreshQuestOfTheDay() to allow quests when there are events going on that are outside of town.
    /// </summary>
    /// <returns>True to continue to the vanilla function, false otherwise.</returns>
    [UsedImplicitly]
    [HarmonyPatch(nameof(Game1.RefreshQuestOfTheDay))]
    private static bool Prefix()
    {
        if (ModEntry.Config.StoreFestivalBehavior != FestivalsShopBehavior.Closed)
        {
            try
            {
                // just get the quest if the shops are forced open.
                if (ModEntry.Config.StoreFestivalBehavior == FestivalsShopBehavior.Open)
                {
                    Game1.questOfTheDay = Utility.getQuestOfTheDay();
                    return false;
                }

                // else, check if today or tomorrow is a festival day for vanilla locations.
                if (!HSUtils.IsFestivalDayForMap(Game1.dayOfMonth, Game1.currentSeason, "Town"))
                {
                    (string season, int day) = AtraUtils.GetTomorrow(Game1.currentSeason, Game1.dayOfMonth);
                    if (!HSUtils.IsFestivalDayForMap(day, season, "Town"))
                    {
                        Game1.questOfTheDay = Utility.getQuestOfTheDay();
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Mod failed while adjusting Daily Quest\n\n{ex}", LogLevel.Error);
            }
        }
        return true;
    }
}
