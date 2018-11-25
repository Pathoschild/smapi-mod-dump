using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
