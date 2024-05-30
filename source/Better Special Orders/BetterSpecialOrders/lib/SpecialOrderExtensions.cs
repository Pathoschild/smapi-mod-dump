/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.SpecialOrders;
using StardewValley.SpecialOrders;

namespace BetterSpecialOrders;

public static class SpecialOrderExtensions
{
    /* Modified version of SpecialOrder.SetOrderDuration to
     * avoid orders collected on sunday only having 1 day
     */
    public static void SetHardOrderDuration(this SpecialOrder order, QuestDuration duration)
    {
        WorldDate date = WorldDate.Now();
        switch (duration)
        {
            // Month long are usually season dependent so we don't edit those
            case QuestDuration.Month:
                date = new WorldDate(Game1.year, Game1.season, 0);
                date.TotalDays++;
                date.TotalDays += 28;
                break;
            case QuestDuration.TwoWeeks:
                date.TotalDays += 14;
                break;
            case QuestDuration.Week:
                date.TotalDays += 7;
                break;
            case QuestDuration.ThreeDays:
                date.TotalDays += 3;
                break;
            case QuestDuration.TwoDays:
                date.TotalDays += 2;
                break;
            case QuestDuration.OneDay:
                date.TotalDays++;
                break;
        }

        order.dueDate.Set(date.TotalDays);
    }

    public static void SetHardOrderDuration(this SpecialOrder order)
    {
        order.SetHardOrderDuration(order.questDuration.Get());
    }
}