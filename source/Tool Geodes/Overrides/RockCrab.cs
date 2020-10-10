/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ToolGeodes
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Monsters;

namespace ToolGeodes.Overrides
{
    public static class RockCrabPiercingHook
    {
        public static void Prefix(RockCrab __instance, int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_PIERCE_ARMOR) > 0)
            {
                var shellGone = Mod.instance.Helper.Reflection.GetField<NetBool>(__instance, "shellGone").GetValue();
                if ( !shellGone.Value )
                {
                    shellGone.Value = true;
                    __instance.shake(500);
                    Mod.instance.Helper.Reflection.GetField<bool>(__instance, "waiter").SetValue(false);
                    __instance.moveTowardPlayerThreshold.Value = 3;
                    __instance.setTrajectory(Utility.getAwayFromPlayerTrajectory(__instance.GetBoundingBox(), who));
                    __instance.moveTowardPlayer(-1);
                    __instance.currentLocation.playSound("stoneCrack");
                    Game1.createRadialDebris(__instance.currentLocation, 14, __instance.getTileX(), __instance.getTileY(), Game1.random.Next(2, 7), false, -1, false, -1);
                    Game1.createRadialDebris(__instance.currentLocation, 14, __instance.getTileX(), __instance.getTileY(), Game1.random.Next(2, 7), false, -1, false, -1);
                }
            }
        }
    }
}
