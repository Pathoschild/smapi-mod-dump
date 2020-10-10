/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ToolGeodes
**
*************************************************/

using StardewValley;
using StardewValley.Tools;

namespace ToolGeodes.Overrides
{
    class MeleeWeaponSpeedHook
    {
        public static void Prefix(MeleeWeapon __instance, Farmer who)
        {
            if (who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_SWIPE_SPEED) > 0)
                who.weaponSpeedModifier += 0.5f;
        }

        public static void Postfix(MeleeWeapon __instance, Farmer who)
        {
            if (who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_SWIPE_SPEED) > 0)
                who.weaponSpeedModifier -= 0.5f;
        }
    }
}
