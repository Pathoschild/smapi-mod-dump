/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley.Monsters;

namespace BattleRoyale.Patches
{
    class ParryInvincibilityPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Monster), "handleParried");

        public static bool Prefix()
        {
            return false;
        }
    }
}
