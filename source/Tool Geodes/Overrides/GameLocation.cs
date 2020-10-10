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

namespace ToolGeodes.Overrides
{
    public static class MonsterDamageHook
    {
        public static bool Prefix(GameLocation __instance, Microsoft.Xna.Framework.Rectangle areaOfEffect,ref int minDamage, ref int maxDamage, bool isBomb, ref float knockBackModifier, int addedPrecision, ref float critChance, float critMultiplier, bool triggerMonsterInvincibleTimer, Farmer who)
        {
            double mult = 1.0 + who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_MORE_DAMAGE) * 0.20;
            minDamage = (int)(minDamage * mult);
            maxDamage = (int)(maxDamage * mult);

            mult = 1.0 + who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_MORE_KNOCKBACK) * 0.50;
            knockBackModifier = (float)(knockBackModifier * mult);

            critChance += (float)(who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_MORE_CRITCHANCE) * 0.05);

            return true;
        }
    }
}
