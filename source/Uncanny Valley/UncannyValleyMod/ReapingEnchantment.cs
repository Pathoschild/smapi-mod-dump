/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace UncannyValleyMod
{
    [XmlType("Mods_UncannyValley_Reaping")]
    [XmlInclude(typeof(BaseWeaponEnchantment))]
    [Serializable]
    public class ReapingEnchantment : BaseWeaponEnchantment
    {
        protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
        {
                int num = Math.Max(1, (int)((float)(monster.MaxHealth + Game1.random.Next(-monster.MaxHealth / 10, monster.MaxHealth / 15 + 1)) * 0.1f));
                who.stamina = Math.Min(who.MaxStamina, Game1.player.stamina + num);
                //location.debris.Add(new Debris(num, new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY()), Color.Lime, 1f, who));
        }

        protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
        {
            if (Game1.random.NextDouble() < 0.090000003576278687)
            {
                int num = Math.Max(1, (int)((float)(m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15 + 1)) * 0.1f));
                who.health = Math.Min(who.maxHealth, Game1.player.health + num);
                //location.debris.Add(new Debris(num, new Vector2(Game1.player.lo(), Game1.player.getStandingY()), Color.Lime, 1f, who));
                Game1.playSound("healSound");
            }
        }

        public override string GetName()
        {
            return "Reaping";
        }
    }
}
