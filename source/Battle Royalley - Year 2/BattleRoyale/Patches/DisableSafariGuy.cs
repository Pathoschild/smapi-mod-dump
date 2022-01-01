/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Locations;

namespace BattleRoyale.Patches
{
    class DisableSafariGuy : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(IslandFieldOffice), "resetLocalState");

        public static bool Prefix(IslandFieldOffice __instance)
        {
            ModEntry.BRGame.Helper.Reflection.GetField<NPC>(__instance, "safariGuy").SetValue(null);
            return false;
        }
    }
}
