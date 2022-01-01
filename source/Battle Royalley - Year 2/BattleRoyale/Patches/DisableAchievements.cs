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

namespace BattleRoyale.Patches
{
    class DisableAchievements : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Game1), "getAchievement");

        public static bool Prefix()
        {
            return false;
        }
    }
}
