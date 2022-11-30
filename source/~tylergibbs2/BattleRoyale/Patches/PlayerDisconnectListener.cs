/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using System.Collections.Generic;

namespace BattleRoyale.Patches
{
    class PlayerDisconnectListener : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Multiplayer), "playerDisconnected");

        public static bool Prefix(long id, List<long> ___disconnectingFarmers)
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            if (Game1.IsServer && Game1.otherFarmers.ContainsKey(id) && !___disconnectingFarmers.Contains(id))
                round?.HandleDeath(DamageSource.WORLD, Game1.otherFarmers[id]);

            return true;
        }
    }
}
