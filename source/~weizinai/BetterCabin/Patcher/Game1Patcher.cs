/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<Game1>(nameof(Game1.warpFarmer), new[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(bool) }),
            this.GetHarmonyMethod(nameof(WarpFarmerPrefix))
        );
    }

    private static bool WarpFarmerPrefix(string locationName)
    {
        if (Game1.getLocationFromName(locationName) is Cabin cabin)
        {
            if (cabin.owner.Equals(Game1.player)) return true;

            if (LockCabinHandler.CheckCabinLock(cabin))
            {
                Game1.addHUDMessage(new HUDMessage(I18n.UI_LockCabin_VisitLockedCabin(), 3));
                return false;
            }

            return true;
        }

        return true;
    }
}