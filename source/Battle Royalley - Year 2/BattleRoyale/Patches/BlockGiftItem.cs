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
    class BlockGiftItem : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "checkAction");

        public static bool Prefix(Farmer who, GameLocation location, Farmer __instance)
        {
            if (Game1.CurrentEvent != null && Game1.CurrentEvent.isSpecificFestival("spring24") && who.dancePartner.Value == null)
                return true;

            if (who.CurrentItem != null && (int)who.CurrentItem.parentSheetIndex == 801 && !__instance.isMarried() && !__instance.isEngaged() && !who.isMarried() && !who.isEngaged())
                return true;

            if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift())
                return false;

            return true;
        }
    }
}
