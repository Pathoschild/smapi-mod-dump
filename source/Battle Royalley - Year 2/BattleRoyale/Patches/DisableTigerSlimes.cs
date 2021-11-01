/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Netcode;
using StardewValley.Locations;

namespace BattleRoyale.Patches
{
    class DisableTigerSlimes : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(IslandWest), "resetSharedState");

        public static bool Prefix(IslandWest __instance)
        {
            NetBool addedSlimes = ModEntry.BRGame.Helper.Reflection.GetField<NetBool>(__instance, "addedSlimesToday").GetValue();
            addedSlimes.Value = true;
            return true;
        }
    }
}
