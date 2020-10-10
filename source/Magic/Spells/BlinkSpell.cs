/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using Magic.Schools;
using SpaceCore;
using StardewValley;

namespace Magic.Spells
{
    public class BlinkSpell : Spell
    {
        public BlinkSpell() : base(SchoolId.Toil, "blink")
        {
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 10;
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            player.position.X = targetX - player.GetBoundingBox().Width / 2;
            player.position.Y = targetY - player.GetBoundingBox().Height / 2;
            Game1.playSound("powerup");
            player.AddCustomSkillExperience(Magic.Skill, 4);

            return null;
        }
    }
}
