/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions for FarmerTeam.
/// </summary>
public static class FarmerTeamExtensions
{
    /// <summary>
    /// Gets a value indicating of a special order is currently active or has been completed.
    /// </summary>
    /// <param name="farmerTeam">FarmerTeam to check.</param>
    /// <param name="special_order_key">Special order key to check for.</param>
    /// <returns>True if completed.</returns>
    public static bool SpecialOrderActiveOrCompleted(this FarmerTeam farmerTeam, string special_order_key)
    {
        Guard.IsNotNull(farmerTeam);
        Guard.IsNotNullOrEmpty(special_order_key);

        if (farmerTeam.completedSpecialOrders.ContainsKey(special_order_key))
        {
            return true;
        }

        foreach (SpecialOrder? order in farmerTeam.specialOrders)
        {
            if (order.questKey.Value == special_order_key && order.questState.Value is SpecialOrder.QuestState.Complete or SpecialOrder.QuestState.InProgress)
            {
                return true;
            }
        }

        return false;
    }
}
