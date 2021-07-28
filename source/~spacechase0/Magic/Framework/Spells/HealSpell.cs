/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Magic.Framework.Schools;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class HealSpell : Spell
    {
        /*********
        ** Public methods
        *********/
        public HealSpell()
            : base(SchoolId.Life, "heal") { }

        public override int GetManaCost(Farmer player, int level)
        {
            return 7;
        }

        public override bool CanCast(Farmer player, int level)
        {
            return base.CanCast(player, level) && player.health != player.maxHealth;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            int health = 10 + 15 * level + (player.CombatLevel + 1) * 2;
            player.health += health;
            if (player.health >= player.maxHealth)
                player.health = player.maxHealth;
            player.currentLocation.debris.Add(new Debris(health, new Vector2(Game1.player.getStandingX() + 8, Game1.player.getStandingY()), Color.Green, 1f, Game1.player));
            player.LocalSound("healSound");
            player.AddCustomSkillExperience(Magic.Skill, health / 2);

            return null;
        }
    }
}
