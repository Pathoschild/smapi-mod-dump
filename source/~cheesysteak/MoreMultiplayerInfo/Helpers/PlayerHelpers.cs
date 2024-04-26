/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace MoreMultiplayerInfo.Helpers
{
    public static class PlayerHelpers
    {
        public static List<Farmer> GetAllCreatedFarmers()
        {
            return Game1.getAllFarmers()
                .Where(f => !string.IsNullOrEmpty(f.Name))
                .Where(f => f.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                .ToList();
        }

        public static Farmer GetPlayerWithUniqueId(long id)
        {
            return Game1.getAllFarmers()
                .FirstOrDefault(f => f.UniqueMultiplayerID == id);
        }

        public static bool IsPlayerOffline(long playerId)
        {
            var onlineFarmerIds = Game1.getOnlineFarmers().Select(f => f.UniqueMultiplayerID);
            return !onlineFarmerIds.Contains(playerId);
        }
    }
}
